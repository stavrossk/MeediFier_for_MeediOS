using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace ToolBox
{
    public class ImpAwards
    {
        public static List<string> GetPoster(string MovieName, string ReleaseYear, string ImageSize, bool Log, StreamWriter swWriteLog)
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

            ToolBox.MeedioUtils.LogEntry(Log, swWriteLog, "[ImpAwards] - Input String: " + PosterName);
            ToolBox.MeedioUtils.LogEntry(Log, swWriteLog, "[ImpAwards] - Release Year: " + RYear);

            //Get Release Years
            releaseYears = GetReleaseYear(PosterName, RYear, Log, swWriteLog);

            //Generate Name List
            NameList.Add(PosterName);

            string[] volList = { "vol. i", "vol i", "volume i", "vol. 1", "vol 1", "volume 1", "part i", "part 1" };
            foreach (string v in volList)
            {
                if (PosterName.ToLower().Contains(v))
                {
                    PosterName2 = PosterName.ToLower().Replace(v, "");
                    NameList.Add(PosterName2.Trim());
                    break;
                }
            }

            if (ToolBox.StringFunctions.containsRomanNumerals(PosterName) == true)
            {
                String[] rNumbers = {" i ", " ii ", " iii ", " iv ", " v ", " vi ", " vii ", " viii ", " ix ", " x ",
                               " xi ", " xii ", " xiii ", " xiv ", " xv ", " xvi ", " xvii ", " xviii ", " xix ", " xx "};
                C3 = CleanUpName(PosterName.ToLower().Trim(), Log, swWriteLog).Trim() + " ";
                foreach (string s in rNumbers)
                {
                    if (C3.Contains(s))
                    {
                        int r = C3.LastIndexOf(s);
                        PosterName2 = C3.ToLower().Remove(r);
                        C3 = PosterName2.Trim() + s;
                        NameList.Add(C3.Trim());
                        break;
                    }
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
                    ToolBox.MeedioUtils.LogEntry(Log, swWriteLog, "[ImpAwards] - Running Search #1");
                    C3 = CleanUpName(n.ToLower().Trim(), Log, swWriteLog);
                    PosterName2 = NameNum2Text(C3, "NumberToText");
                    ToolBox.MeedioUtils.LogEntry(Log, swWriteLog, "[ImpAwards] - Search name: " + PosterName2);
                    Search1Term = PosterName2;
                    urlList = CreateUrls(PosterName2, releaseYears, ImageSize, Log, swWriteLog);
                    if (urlList.Count > 0)
                        break;
                }

                //SEARCH #2 - REPEAT SEARCH IF EMPTY - Alternative search for titles with individual numbers
                if (ToolBox.StringFunctions.HasNumbers(n) == true && urlList.Count == 0)
                {
                    ToolBox.MeedioUtils.LogEntry(Log, swWriteLog, "[ImpAwards] - Running Search #2");
                    C3 = CleanUpName(n.ToLower().Trim(), Log, swWriteLog);
                    string P2 = ToolBox.StringFunctions.IndividualNumToText(C3).Trim();
                    PosterName2 = P2.Replace(" ", "_");
                    ToolBox.MeedioUtils.LogEntry(Log, swWriteLog, "[ImpAwards] - Search name: " + PosterName2);
                    Search2Term = PosterName2;
                    if (Search2Term != Search1Term)
                    {
                        urlList = CreateUrls(PosterName2, releaseYears, ImageSize, Log, swWriteLog);
                    }
                    if (urlList.Count > 0)
                        break;
                }

                //SEARCH #3 - REPEAT SEARCH IF EMPTY - Alternative search for titles with years
                if (ToolBox.StringFunctions.HasNumbers(n) == true && urlList.Count == 0)
                {
                    ToolBox.MeedioUtils.LogEntry(Log, swWriteLog, "[ImpAwards] - Running Search #3: Years to Text");
                    C3 = CleanUpName(n.ToLower().Trim(), Log, swWriteLog);
                    PosterName2 = NameNum2Text(C3, "YearToText");
                    ToolBox.MeedioUtils.LogEntry(Log, swWriteLog, "[ImpAwards] - Search name: " + PosterName2);
                    Search3Term = PosterName2;
                    if (Search3Term != Search2Term && Search3Term != Search1Term)
                    {
                        urlList = CreateUrls(PosterName2, releaseYears, ImageSize, Log, swWriteLog);
                    }
                    if (urlList.Count > 0)
                        break;
                }

                //SEARCH #4 - REPEAT SEARCH IF EMPTY - Alternative search for nTh Numbers
                if (ToolBox.StringFunctions.HasNumbers(n) == true && urlList.Count == 0)
                {
                    ToolBox.MeedioUtils.LogEntry(Log, swWriteLog, "[ImpAwards] - Running Search #4: nTh Numbers to Text");
                    C3 = CleanUpName(n.ToLower().Trim(), Log, swWriteLog);
                    PosterName2 = NameNum2Text(C3, "NthNumberToText");
                    ToolBox.MeedioUtils.LogEntry(Log, swWriteLog, "[ImpAwards] - Search name: " + PosterName2);
                    Search4Term = PosterName2;
                    if (Search4Term != Search3Term && Search4Term != Search2Term && Search4Term != Search1Term)
                    {
                        urlList = CreateUrls(PosterName2, releaseYears, ImageSize, Log, swWriteLog);
                    } 
                    if (urlList.Count > 0)
                        break;
                }

                //SEARCH #5 - Alternative Search for roman numerals
                if (ToolBox.StringFunctions.containsRomanNumerals(n) == true && urlList.Count == 0)
                {
                    ToolBox.MeedioUtils.LogEntry(Log, swWriteLog, "[ImpAwards] - Running Search #5: Roman Numbers to Text");
                    C3 = CleanUpName(n.ToLower().Trim(), Log, swWriteLog);
                    PosterName2 = ToolBox.StringFunctions.RomanNumberToText(C3);
                    ToolBox.MeedioUtils.LogEntry(Log, swWriteLog, "[ImpAwards] - Search name: " + PosterName2);
                    Search5Term = PosterName2;
                    if (Search5Term != Search4Term && Search5Term != Search3Term && Search5Term != Search2Term && Search5Term != Search1Term)
                    {
                        urlList = CreateUrls(PosterName2, releaseYears, ImageSize, Log, swWriteLog);
                    }
                    if (urlList.Count > 0)
                        break;
                }
            }


            //RESULTS           
            if (urlList.Count == 0)
            {
                ToolBox.MeedioUtils.LogEntry(Log, swWriteLog, "[ImpAwards] - No images could be found.");
            }
            else
            {
                foreach (string m in urlList)
                {
                    ToolBox.MeedioUtils.LogEntry(Log, swWriteLog, "[ImpAwards] - Urls in list: " + m.ToString());
                }
            }
            return urlList;
        }

        //REPLACE AND FORMAT TEXT
        private static string CleanUpName(string MovieName, bool Log, StreamWriter swWriteLog)
        {
            ToolBox.MeedioUtils.LogEntry(Log, swWriteLog, "[ImpAwards] - Cleaning up input name: " + MovieName);
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
                ToolBox.MeedioUtils.LogEntry(Log, swWriteLog, "[ImpAwards] - Error replacing characters: " + e.ToString());
            }
            ToolBox.MeedioUtils.LogEntry(Log, swWriteLog, "[ImpAwards] - Cleaned up name: " + CleanedName);
            return CleanedName;
        }

        //REPLACE NUMBERS WITH TEXT
        private static string NameNum2Text(string NameWithNumbers, string StringFunctions)
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
                    if (StringFunctions == "YearToText")
                    {
                        if (ToolBox.StringFunctions.HasNumbers(s))
                        {
                            n = Convert.ToInt16(s);
                            nString = ToolBox.StringFunctions.YearToText(n);
                            newString.Add(nString);
                        }
                        else
                        {
                            newString.Add(s);
                        }
                    }
                    if (StringFunctions == "NumberToText")
                    {
                        if (ToolBox.StringFunctions.HasNumbers(s))
                        {
                            n = Convert.ToInt16(s);
                            nString = ToolBox.StringFunctions.NumberToText(n);
                            newString.Add(nString);
                        }
                        else
                        {
                            newString.Add(s);
                        }
                    }
                    if (StringFunctions == "NthNumberToText")
                    {
                        if (ToolBox.StringFunctions.HasNumbers(s))
                        {
                            int n2 = 0;
                            if (s.EndsWith("st") || s.EndsWith("nd") || s.EndsWith("rd") || s.EndsWith("th"))
                            {
                                string textNum = s.Remove(s.Length - 2);
                                n2 = Convert.ToInt16(textNum);
                            }
                            nString = ToolBox.StringFunctions.NthNumberToText(n2);
                            newString.Add(nString);
                        }
                        else
                        {
                            newString.Add(s);
                        }
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
        private static List<String> GetReleaseYear(string MovieName, string Year, bool Log, StreamWriter swWriteLog)
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
        private static List<string> CreateUrls(String CleanPosterName, List<String> releaseYears, String ImageSize, bool Log, StreamWriter swWriteLog)
        {
            List<string> urlList = new List<string>();
            if (releaseYears.Count > 0)
            {
                ToolBox.MeedioUtils.LogEntry(Log, swWriteLog, "[ImpAwards] - Creating Urls...");
                try
                {
                    String ImpAwardsUrl = "";
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
                            if (ImageSize == "ExtraLarge" || ImageSize == "Both")
                            {
                                try
                                {
                                    ImpAwardsUrl = @"http://www.impawards.com/" + ReleaseYear + "/posters/" + PosterName2 + "_xlg.jpg";  //@"http://www.impawards.com/1995/desperado_xlg.html";  
                                    ToolBox.MeedioUtils.LogEntry(Log, swWriteLog, "[ImpAwards] - Trying: " + ImpAwardsUrl);
                                    HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(ImpAwardsUrl);
                                    myRequest.AllowAutoRedirect = false;
                                    HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
                                    if (myResponse.StatusCode.ToString() == "OK")
                                    {
                                        urlList.Add(ImpAwardsUrl);
                                        ToolBox.MeedioUtils.LogEntry(Log, swWriteLog, "[ImpAwards] - Added to list: " + ImpAwardsUrl);
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
                                        ImpAwardsUrl = @"http://www.impawards.com/" + ReleaseYear + "/posters/" + PosterName2 + "_ver" + v + "_xlg.jpg";  //@"http://www.impawards.com/1995/desperado_xlg.html";  
                                        ToolBox.MeedioUtils.LogEntry(Log, swWriteLog, "[ImpAwards] - Trying: " + ImpAwardsUrl);
                                        HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(ImpAwardsUrl);
                                        myRequest.AllowAutoRedirect = false;
                                        HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
                                        if (myResponse.StatusCode.ToString() == "OK")
                                        {
                                            urlList.Add(ImpAwardsUrl);
                                            ToolBox.MeedioUtils.LogEntry(Log, swWriteLog, "[ImpAwards] - Added to list: " + ImpAwardsUrl);
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
                                    ImpAwardsUrl = @"http://www.impawards.com/" + ReleaseYear + "/posters/" + PosterName2 + ".jpg";
                                    ToolBox.MeedioUtils.LogEntry(Log, swWriteLog, "[ImpAwards] - Trying: " + ImpAwardsUrl);
                                    HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(ImpAwardsUrl);
                                    myRequest.AllowAutoRedirect = false;
                                    HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
                                    if (myResponse.StatusCode.ToString() == "OK")
                                    {
                                        urlList.Add(ImpAwardsUrl);
                                        ToolBox.MeedioUtils.LogEntry(Log, swWriteLog, "[ImpAwards] - Added to list: " + ImpAwardsUrl);
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
                                        ImpAwardsUrl = @"http://www.impawards.com/" + ReleaseYear + "/posters/" + PosterName2 + "_ver" + v + ".jpg";
                                        ToolBox.MeedioUtils.LogEntry(Log, swWriteLog, "[ImpAwards] - Trying: " + ImpAwardsUrl);
                                        HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(ImpAwardsUrl);
                                        myRequest.AllowAutoRedirect = false;
                                        HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
                                        if (myResponse.StatusCode.ToString() == "OK")
                                        {
                                            urlList.Add(ImpAwardsUrl);
                                            ToolBox.MeedioUtils.LogEntry(Log, swWriteLog, "[ImpAwards] - Added to list: " + ImpAwardsUrl);
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
                            ToolBox.MeedioUtils.LogEntry(Log, swWriteLog, "[ImpAwards] - Could not find year needed for ImpAwards search");
                        }
                    }
                }
                catch (Exception e)
                {
                    ToolBox.MeedioUtils.LogEntry(Log, swWriteLog, "[ImpAwards] - Error: " + e.ToString());
                }
            }
            else
            {
                ToolBox.MeedioUtils.LogEntry(Log, swWriteLog, "[ImpAwards] - Meedio Library does not contain year needed for ImpAwards search");
            }
            return urlList;
        }
    }
}

