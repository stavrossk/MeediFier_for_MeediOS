//'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
//'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
//''    MediaFairy                                                               ''
//''    Copyright (C) 2008-2010  Stavros Skamagkis                               ''
//''                                                                             ''
//''    This program is free software: you can redistribute it and/or modify     ''
//''    it under the terms of the GNU General Public License as published by     ''
//''    the Free Software Foundation, either version 3 of the License, or        ''
//''    (at your option) any later version.                                      ''
//''                                                                             ''
//''    This program is distributed in the hope that it will be useful,          ''
//''    but WITHOUT ANY WARRANTY; without even the implied warranty of           ''
//''    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the            ''
//''    GNU General Public License for more details.                             ''
//''                                                                             ''
//''    You should have received a copy of the GNU General Public License        ''
//''    along with this program.  If not, see <http://www.gnu.org/licenses/>.    ''
//'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
//'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''



using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using MeediFier.ImportingEngine;


#if USE_MEEDIO
using Meedio;
#elif USE_MEEDIOS
using MeediFier.ToolBox.Utils;
using MeediOS;
#endif

namespace MeediFier
{
    public class ImpAwards
    {
        public static List<string> GetPoster(IMLItem Item, string MovieName, string ReleaseYear, string ImageSize)
        {
            List<string> urlList = new List<string>();
            List<String> releaseYears;
            List<string> NameList = new List<string>();
            String PosterName = MovieName;
            String RYear = ReleaseYear;
            String PosterName2 = "";
            String C3 = "";
            string Search1Term = "";
            string Search2Term = "";
            string Search3Term = "";
            string Search4Term = "";
            string Search5Term = "";

           Debugger.LogMessageToFile("[ImpAwards] - Input String: " + PosterName);
           Debugger.LogMessageToFile("[ImpAwards] - Release Year: " + RYear);

            //Get Release Years
            releaseYears = GetReleaseYear(PosterName, RYear);

            //Generate Name List
            NameList.Add(PosterName);

            string[] volList = { "vol. i", "vol i", "volume i", "vol. 1", "vol 1", "volume 1", "part i", "part 1" };
         
            foreach (string v in volList.Where(v => PosterName.ToLower().Contains(v)))
            {
                PosterName2 = PosterName.ToLower().Replace(v, "");
                NameList.Add(PosterName2.Trim());
                break;
            }

            if (StringProcessors.ContainsRomanNumerals(PosterName) )
            {
                String[] rNumbers = {" i ", " ii ", " iii ", " iv ", " v ", " vi ", " vii ", " viii ", " ix ", " x ",
                               " xi ", " xii ", " xiii ", " xiv ", " xv ", " xvi ", " xvii ", " xviii ", " xix ", " xx "};
                C3 = CleanUpName(PosterName.ToLower().Trim()).Trim() + " ";
                foreach (string s in rNumbers)
                {
                    if (!C3.Contains(s)) continue;

                    int r = C3.LastIndexOf(s);
                    PosterName2 = C3.ToLower().Remove(r);
                    C3 = PosterName2.Trim() + s;
                    NameList.Add(C3.Trim());
                    break;
                }
            }

            if (PosterName.Contains(":"))
            {
                int r = PosterName.IndexOf(":");
                PosterName2 = PosterName.ToLower().Remove(r);
                NameList.Add(PosterName2.Trim());
            }


            foreach (string n in NameList)
            {
                //SEARCH #1     
                {
                    Helpers.UpdateProgress(MainImportingEngine.GeneralStatus, "Searching ImpAwards for posters...", Item);
                    Debugger.LogMessageToFile("[ImpAwards] - Running Search #1");
                    C3 = CleanUpName(n.ToLower().Trim());
                    PosterName2 = NameNum2Text(C3, "NumberToText");
                    Debugger.LogMessageToFile("[ImpAwards] - Search name: " + PosterName2);
                    Search1Term = PosterName2;
                    urlList = CreateUrls(PosterName2, releaseYears, ImageSize);
                    if (urlList.Count > 0)
                        break;
                }

                //SEARCH #2 - REPEAT SEARCH IF EMPTY - Alternative search for titles with individual numbers
                if (StringProcessors.HasNumbers(n) == true && urlList.Count == 0)
                {
                    Debugger.LogMessageToFile("[ImpAwards] - Running Search #2");
                    C3 = CleanUpName(n.ToLower().Trim());
                    string P2 = StringProcessors.IndividualNumToText(C3).Trim();
                    PosterName2 = P2.Replace(" ", "_");
                    Debugger.LogMessageToFile("[ImpAwards] - Search name: " + PosterName2);
                    Search2Term = PosterName2;
                    if (Search2Term != Search1Term)
                    {
                        urlList = CreateUrls(PosterName2, releaseYears, ImageSize);
                    }
                    if (urlList.Count > 0)
                        break;
                }

                //SEARCH #3 - REPEAT SEARCH IF EMPTY - Alternative search for titles with years
                if (StringProcessors.HasNumbers(n) == true && urlList.Count == 0)
                {
                    Debugger.LogMessageToFile("[ImpAwards] - Running Search #3: Years to Text");
                    C3 = CleanUpName(n.ToLower().Trim());
                    PosterName2 = NameNum2Text(C3, "YearToText");
                    Debugger.LogMessageToFile("[ImpAwards] - Search name: " + PosterName2);
                    Search3Term = PosterName2;
                    if (Search3Term != Search2Term && Search3Term != Search1Term)
                    {
                        urlList = CreateUrls(PosterName2, releaseYears, ImageSize);
                    }
                    if (urlList.Count > 0)
                        break;
                }

                //SEARCH #4 - REPEAT SEARCH IF EMPTY - Alternative search for nTh Numbers
                if (StringProcessors.HasNumbers(n) == true && urlList.Count == 0)
                {
                    Debugger.LogMessageToFile("[ImpAwards] - Running Search #4: nTh Numbers to Text");
                    C3 = CleanUpName(n.ToLower().Trim());
                    PosterName2 = NameNum2Text(C3, "NthNumberToText");
                    Debugger.LogMessageToFile("[ImpAwards] - Search name: " + PosterName2);
                    Search4Term = PosterName2;
                    if (Search4Term != Search3Term && Search4Term != Search2Term && Search4Term != Search1Term)
                    {
                        urlList = CreateUrls(PosterName2, releaseYears, ImageSize);
                    } 
                    if (urlList.Count > 0)
                        break;
                }

                //SEARCH #5 - Alternative Search for roman numerals
                if (!StringProcessors.ContainsRomanNumerals(n) || urlList.Count != 0) 
                    continue;

                Debugger.LogMessageToFile("[ImpAwards] - Running Search #5: Roman Numbers to Text");
                
                C3 = CleanUpName(n.ToLower().Trim());
                
                PosterName2 = StringProcessors.RomanNumberToText(C3);
                
                Debugger.LogMessageToFile("[ImpAwards] - Search name: " + PosterName2);
                
                Search5Term = PosterName2;

                if (Search5Term != Search4Term && Search5Term != Search3Term && Search5Term != Search2Term && Search5Term != Search1Term)
                {
                    urlList = CreateUrls(PosterName2, releaseYears, ImageSize);
                }

                if (urlList.Count > 0)
                    break;
            }


            //RESULTS           
            if (urlList.Count == 0)
            {
                Debugger.LogMessageToFile("[ImpAwards] - No images could be found.");
            }
            else
            {
                foreach (string m in urlList)
                {
                    Debugger.LogMessageToFile("[ImpAwards] - Urls in list: " + m.ToString());
                }
            }
            return urlList;
        }


        //REPLACE AND FORMAT TEXT
        private static string CleanUpName(string MovieName)
        {
            Debugger.LogMessageToFile("[ImpAwards] - Cleaning up input name: " + MovieName);
            String PosterName2 = MovieName.ToLower();
            String CleanedName = "";

            try
            {
                //adjust for movies that start with "the" 
                if (PosterName2.StartsWith("the"))
                {
                    int Pos3 = PosterName2.Length;
                    String C2 = PosterName2.Substring(3, Pos3 - 3).Trim();
                    PosterName2 = C2;
                }
                //adjust for movies that start with "A"
                if (PosterName2.StartsWith("a "))
                {
                    int Pos3 = PosterName2.Length;
                    String C2 = PosterName2.Substring(1, Pos3 - 1).Trim();
                    PosterName2 = C2;
                }

                String C3 = PosterName2.Replace("'", "").ToLower();
                PosterName2 = C3;
                C3 = PosterName2.Replace(".", " ").ToLower();
                PosterName2 = C3;
                C3 = PosterName2.Replace("&", "and").ToLower();
                PosterName2 = C3;
                C3 = PosterName2.Replace("-", " ").ToLower();
                PosterName2 = C3;
                C3 = PosterName2.Replace(":", " ").ToLower();
                PosterName2 = C3;
                C3 = PosterName2.Replace(",", "").ToLower();
                PosterName2 = C3;
                C3 = PosterName2.Replace("!", "").ToLower();
                PosterName2 = C3;
                C3 = PosterName2.Replace("  ", " ").ToLower();
                PosterName2 = C3;                
                C3 = PosterName2.Replace("³", " 3").ToLower();
                PosterName2 = C3;
                C3 = PosterName2.Replace("·", " ").ToLower();
                PosterName2 = C3;
                C3 = PosterName2.Replace("  ", " ").ToLower();
                PosterName2 = C3;
                if (PosterName2.Contains("Æ"))
                {
                    int d = C3.IndexOf('Æ');
                    String C4 = C3.Remove(d);
                    int d2 = C3.Length;
                    String C5 = C3.Substring(d + 1, (d2 - (d + 1)));
                    String C6 = (C4 + "ae" + C5);
                    C3 = C6;
                }
                if (PosterName2.Contains("æ"))
                {
                    int d = C3.IndexOf('æ');
                    String C4 = C3.Remove(d);
                    int d2 = C3.Length;
                    String C5 = C3.Substring(d + 1, (d2 - (d + 1)));
                    String C6 = (C4 + "ae" + C5);
                    C3 = C6;
                }
                CleanedName = C3;
            }
            catch (Exception e)
            {
                Debugger.LogMessageToFile("[ImpAwards] - Error replacing characters: " + e.ToString());
            }
            Debugger.LogMessageToFile("[ImpAwards] - Cleaned up name: " + CleanedName);
            return CleanedName;
        }


        //REPLACE NUMBERS WITH TEXT
        private static string NameNum2Text(string NameWithNumbers, string stringFunctions)
        {

            //Replace numbers with text
            String PosterName2 = NameWithNumbers.ToLower().Trim();
            
            string[] split = PosterName2.Split(new Char[] { ' ' });
            
            List<string> newString = new List<string>();
            
            foreach (string s in split)
            {
                try
                {
                    int n;
                    string nString = "";
                    if (stringFunctions == "YearToText")
                    {
                        if (StringProcessors.HasNumbers(s))
                        {
                            n = Convert.ToInt16(s);
                            nString = StringProcessors.YearToText(n);
                            newString.Add(nString);
                        }
                        else
                        {
                            newString.Add(s);
                        }
                    }
                    if (stringFunctions == "NumberToText")
                    {
                        if (StringProcessors.HasNumbers(s))
                        {
                            n = Convert.ToInt16(s);
                            nString = StringProcessors.NumberToText(n);
                            newString.Add(nString);
                        }
                        else
                        {
                            newString.Add(s);
                        }
                    }

                    if (stringFunctions != "NthNumberToText") continue;
                    
                    if (StringProcessors.HasNumbers(s))
                    {
                        int n2 = 0;
                        if (s.EndsWith("st") || s.EndsWith("nd") || s.EndsWith("rd") || s.EndsWith("th"))
                        {
                            string textNum = s.Remove(s.Length - 2);
                            n2 = Convert.ToInt16(textNum);
                        }
                        nString = StringProcessors.NthNumberToText(n2);
                        newString.Add(nString);
                    }
                    else
                    {
                        newString.Add(s);
                    }
                }
                catch
                {
                    newString.Add(s);
                }
            }

            //Rebuild string
            StringBuilder sb = new StringBuilder();
            foreach (string t in newString)
            {
                sb.Append(t + " ");
            }
            PosterName2 = sb.ToString().Trim();
            String C3 = PosterName2.Replace("  ", " ").ToLower().Trim();
            PosterName2 = C3;
            C3 = PosterName2.Replace(" ", "_").ToLower().Trim();
            PosterName2 = C3;
            return PosterName2;
        }

        //GET RELEASE YEAR
        private static List<String> GetReleaseYear(string MovieName, string Year)
        {
            List<String> yearList = new List<String>();
            try
            {
                yearList.Add(Year);
                int Y1 = Convert.ToInt16(Year);
                int Y2 = Y1 + 1;
                int Y3 = Y1 - 1;
                string sY2 = Convert.ToString(Y2);
                yearList.Add(sY2);
                string sY3 = Convert.ToString(Y3);
                yearList.Add(sY3);
            }
            catch
            {
            }
            return yearList;
        }

        //CREATE AND QUALIFY URLS
        private static List<string> CreateUrls(String CleanPosterName, List<String> releaseYears, String ImageSize)
        {
            List<string> urlList = new List<string>();
            if (releaseYears.Count > 0)
            {
                Debugger.LogMessageToFile("[ImpAwards] - Creating Urls...");
                try
                {
                    String PosterName2 = CleanPosterName;
                    String C3 = PosterName2.Replace("  ", " ").ToLower().Trim();
                    PosterName2 = C3;
                    C3 = PosterName2.Replace(" ", "_").ToLower().Trim();
                    PosterName2 = C3;

                    foreach (string y in releaseYears)
                    {
                        string ReleaseYear = y;
                        if (ReleaseYear != "")
                        {
                            String impAwardsUrl;
                            if (ImageSize == "ExtraLarge" || ImageSize == "Both")
                            {
                                try
                                {
                                    impAwardsUrl = @"http://www.impawards.com/" + ReleaseYear + "/posters/" + PosterName2 + "_xlg.jpg";  //@"http://www.impawards.com/1995/desperado_xlg.html";  
                                    Debugger.LogMessageToFile("[ImpAwards] - Trying: " + impAwardsUrl);
                                    HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(impAwardsUrl);
                                    myRequest.AllowAutoRedirect = false;
                                    HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
                                    if (myResponse.StatusCode.ToString() == "OK")
                                    {
                                        urlList.Add(impAwardsUrl);
                                        Debugger.LogMessageToFile("[ImpAwards] - Added to list: " + impAwardsUrl);
                                    }
                                    myResponse.Close();
                                }
                                catch (Exception)
                                {
                                }

                                try
                                {
                                    for (int v = 1; v < 30; v++)
                                    {
                                        impAwardsUrl = @"http://www.impawards.com/" + ReleaseYear + "/posters/" + PosterName2 + "_ver" + v + "_xlg.jpg";  //@"http://www.impawards.com/1995/desperado_xlg.html";  
                                        Debugger.LogMessageToFile("[ImpAwards] - Trying: " + impAwardsUrl);
                                        HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(impAwardsUrl);
                                        myRequest.AllowAutoRedirect = false;
                                        HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
                                        if (myResponse.StatusCode.ToString() == "OK")
                                        {
                                            urlList.Add(impAwardsUrl);
                                            Debugger.LogMessageToFile("[ImpAwards] - Added to list: " + impAwardsUrl);
                                        }
                                        else
                                        {
                                            myResponse.Close();
                                            if (v > 2)
                                                break;
                                        }
                                        myResponse.Close();
                                    }
                                }
                                catch
                                {
                                }
                            }
                            if (ImageSize == "Large" || ImageSize == "Both")
                            {
                                try
                                {
                                    impAwardsUrl = @"http://www.impawards.com/" + ReleaseYear + "/posters/" + PosterName2 + ".jpg";
                                    Debugger.LogMessageToFile("[ImpAwards] - Trying: " + impAwardsUrl);
                                    HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(impAwardsUrl);
                                    myRequest.AllowAutoRedirect = false;
                                    HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
                                    if (myResponse.StatusCode.ToString() == "OK")
                                    {
                                        urlList.Add(impAwardsUrl);
                                        Debugger.LogMessageToFile("[ImpAwards] - Added to list: " + impAwardsUrl);
                                    }
                                    myResponse.Close();
                                }
                                catch
                                {
                                }

                                try
                                {
                                    for (int v = 1; v < 30; v++)
                                    {
                                        impAwardsUrl = @"http://www.impawards.com/" + ReleaseYear + "/posters/" + PosterName2 + "_ver" + v + ".jpg";
                                        Debugger.LogMessageToFile("[ImpAwards] - Trying: " + impAwardsUrl);
                                        HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(impAwardsUrl);
                                        myRequest.AllowAutoRedirect = false;
                                        HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
                                        if (myResponse.StatusCode.ToString() == "OK")
                                        {
                                            urlList.Add(impAwardsUrl);
                                            Debugger.LogMessageToFile("[ImpAwards] - Added to list: " + impAwardsUrl);
                                        }
                                        else
                                        {
                                            myResponse.Close();
                                            if (v > 2)
                                                break;
                                        }
                                        myResponse.Close();
                                    }
                                }
                                catch
                                {
                                }
                            }
                        }
                        else
                        {
                            Debugger.LogMessageToFile("[ImpAwards] - Could not find year needed for ImpAwards search");
                        }
                    }
                }
                catch (Exception e)
                {
                    Debugger.LogMessageToFile("[ImpAwards] - Error: " + e.ToString());
                }
            }
            else
            {
                Debugger.LogMessageToFile("[ImpAwards] - Meedio Library does not contain year needed for ImpAwards search");
            }
            return urlList;
        }
    }
}

