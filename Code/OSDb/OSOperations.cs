﻿//'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
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
using System.Collections;
using System.Threading;
using System.IO;
using System.Net;
using System.Xml;
using System.Windows.Forms;
using CookComputing.XmlRpc;

#if USE_MEEDIO 
using Meedio; 
#elif USE_MEEDIOS
using MeediFier.ImportingEngine;
using MeediOS;
#endif


namespace MeediFier.OSDb
{

    public class OSoperations
    {
        private const string TxtUrl = "http://api.opensubtitles.org/xml-rpc";

        internal static IOpenSubtitlesRemoteFunctions Proxy;
      
        private static XmlRpcStruct mystruct;
        public static string token = "4782378472834782378372";


        public bool ServerInfo()
        {
            CreateProxy();

            try
            {
                XmlRpcStruct serverinfo = Proxy.ServerInfo();
                return true;
            }
            catch (Exception ex)
            {
                Debugger.LogMessageToFile(ex.Message);
                return false;
            }
        }


        public LoginResult SiteLogin()
        {
            CreateProxy();
            const string userAgent = "MeediFier for MeediOS v0.7.9";
            LoginResult loginresult = Proxy.LogIn(Settings.Username, Settings.Password,"eng", userAgent);
            return loginresult;
        }

        //TODO: Make CreateProxy static and test it.
        private void CreateProxy()
        {
            Proxy = XmlRpcProxyGen.Create<IOpenSubtitlesRemoteFunctions>();
            Proxy.Timeout = 5000;

            Proxy.UserAgent = "MeediFier for MeediOS v0.7.9";
            Proxy.Url = TxtUrl;
        }

        #region Methods to find ImdbId.
        //Old method of finding the video's ImdbId by hash. This is not used anymore. Searching is now done with XMLRPC.
        public static string FindImdbIDbyHash(string moviehash, IMLItem item)
        {
            string imdbid = string.Empty;
            var hash = new string[1];
            hash[0] = moviehash;

            //TODO: Have only one user agent variable in Settings.cs instead of many.
            const string useragent = "MeediFier for MeediOS v0.7.9";

            const string prefix = "http://www.opensubtitles.org/en/search2/sublanguageid-eng/moviehash-";
            string hashSearchurl = prefix + moviehash + "/xml";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(hashSearchurl);
            request.UserAgent = useragent;
            request.Timeout = 10000;
            WebResponse response = request.GetResponse();

            var xDoc = new XmlDocument();




            Stream stream = response.GetResponseStream();
            //Stream stream2 = stream;
            if (stream != null)
            {
                StreamReader sr = new StreamReader(stream);
                //MessageBox.Show(sr.ReadToEnd());
                string htmlLine = sr.ReadLine();
                //MessageBox.Show("html_line: " + html_line);
                stream.Close();

                if (htmlLine != null && htmlLine.Contains("DOCTYPE html"))
                {
                    //MessageBox.Show("Passed inside html parser");

                    htmlLine = sr.ReadLine();

                    #region see if imdbid exists in the html and if yes, return it
                    if (htmlLine.Contains("imdbid-"))
                    {
                   
                        MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress,item.Name + " was succesfully identified by fingerprint! ");
                        Thread.Sleep(1500);

                        int imdbid_index = htmlLine.IndexOf("imdbid-", System.StringComparison.Ordinal);
                        //MessageBox.Show("imdb_index: " + Convert.ToString(imdbid_index));
                        imdbid = htmlLine.Substring(imdbid_index + 7, 7);
                        if (imdbid.EndsWith("/"))
                        {
                            imdbid = imdbid.TrimEnd('/');
                            imdbid = "0" + imdbid;
                        }
                        //MessageBox.Show("imdbid after parsing html: " + imdbid);
                        return imdbid;
                    }
                    else return "";
                    #endregion

                }
                xDoc.Load(hashSearchurl);

                XmlNodeList imdbidTags = xDoc.GetElementsByTagName("MovieImdbID");

                if (imdbidTags.Count == 0)
                    return "";

                MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress,item.Name + " was succesfully identified by fingerprint! ");
                Thread.Sleep(1500);

                imdbid = imdbidTags[0].InnerText;

                //MessageBox.Show("imdbid inside xml search" + imdbid);
                //Importer.thisProgress.Progress(0, "imdbid inside xml search: " + imdbid);
                //Thread.Sleep(2000);


                return imdbid;
            }

            return imdbid;
        }


        public static string FindImdbIDbyHashUsingXmlRpc
            (string moviehash, IMLItem item, string connectionToken,ref bool osIsOnline)
        {

            #region init vars
            var hash = new string[1];
            hash[0] = moviehash;
            string imdbid = "";
            XmlRpcStruct checkMovieHashStruct;
            #endregion


            #region verify connection token
            if (connectionToken == null)
            {
                MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress, "Unable to request data from OSdb. Connection token is not available.");
                Debugger.LogMessageToFile("Unable to request data from OSdb. Connection token is not available");
                Thread.Sleep(3000);
                return "";
            }
            #endregion


            Debugger.LogMessageToFile("Attempting online identification of video " + item.Name + "...");
            Helpers.UpdateProgress("", "Identifying video " + item.Name + "...",  item);
            try
            {

                checkMovieHashStruct = Proxy.CheckMovieHash(connectionToken, hash);

                imdbid = GetImdbidFromXmlRpcStruct(item, checkMovieHashStruct);
                
                return imdbid;
            }
            catch (Exception)
            {
                Application.DoEvents();
                MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress, "OSdb did not respond. Retrying... (1)");
                //Thread.Sleep(2000);

                try
                {
                    checkMovieHashStruct = Proxy.CheckMovieHash(connectionToken, hash);
                    imdbid = GetImdbidFromXmlRpcStruct(item, checkMovieHashStruct);
                    return imdbid;
                }
                catch (Exception)
                {
                    MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress, "OSdb did not respond. Retrying... (2) ");
                    //Thread.Sleep(2000);

                    try
                    {
                        checkMovieHashStruct = Proxy.CheckMovieHash(connectionToken, hash);
                        imdbid = GetImdbidFromXmlRpcStruct(item, checkMovieHashStruct);
                        return imdbid;
                    }
                    catch (Exception)
                    {
                        Debugger.LogMessageToFile("Unable to connect to OSdb. Video fingerpint cannot be matched online.");
                        MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress, "Unable to connect to OSdb.");
                        StatusForm.statusForm.TrayIcon.ShowBalloonTip(5000, "Connection to OSdb was not possible", "MediaFairy is experiencing difficulties communicating with OpenSubtitles.org database. Please check your connection availability, otherwise the server may be temporarily unreachable.", ToolTipIcon.Info);
                        Thread.Sleep(2000);

                        if (Settings.WantToDisableSources)
                            osIsOnline = false;

                        return String.Empty;
                    }

                }

            }


        }


        private static string GetImdbidFromXmlRpcStruct(IMLItem item, IEnumerable imdbIdStruct)
        {
            string imdbid = String.Empty;

            Debugger.LogMessageToFile("Parsing the XMLRPC structure returned by OSdb...");
            try
            {
                #region scan entries and get imdbid
                foreach (DictionaryEntry d in imdbIdStruct)
                {
                    string Token = Convert.ToString(d.Key);

                    if (Token != "data")
                        continue;
                    
                    Debugger.LogMessageToFile("data structure found.");

                    object value = d.Value;
                    var s = (XmlRpcStruct)value; //unboxing

                    foreach (DictionaryEntry e in s)
                    {
                        object objb = e.Value;

                        Type type = objb.GetType();
                        string typename = type.Name;

                        if (typename == "Object[]")
                        {
                            //Importer.thisProgress.Progress(Item.Name + " could not be identified by fingerpint. Will search by film's title...");
                            //Thread.Sleep(Importer.Importer.SleepValue);
                            Debugger.LogMessageToFile("OSdb returned multiple XMLRPC structures. " + item.Name + " could not be identified by video fingerprint.");
                            item.Tags["OSdbMatched"] = "false";
                            item.SaveTags();
                            return "";
                        }



                        var movie = (XmlRpcStruct)objb; //unboxing

                        foreach (DictionaryEntry f in movie)
                        {
                            Token = Convert.ToString(f.Key);

                            if (Token != "MovieImdbID") continue;
                            object g = f.Value;
                            imdbid = (string)g;
                            item.Tags["OSdbMatched"] = "true";
                            item.SaveTags();
                            Debugger.LogMessageToFile("The film's IMDbID was successfully extracted from OSdb response.");
                        }//End of foreach d in dictionary

                    }//End of foreach e in dictionary

                }
                #endregion
            }
            catch (Exception e)
            {
                StatusForm.statusForm.TrayIcon.ShowBalloonTip(5000, "Online video identification was unsuccesfull", "An error occured while reading the response from OSdb. The error was logged to the plugin's debug.log file.", ToolTipIcon.Error);
                Debugger.LogMessageToFile("An error occured on the XMLRPC structure parser. The error was: " + e.ToString() );
            }

            return imdbid;
        }
        #endregion


        public static void GetDetailsFromOSdb
            (string imdbid, IMLItem item, bool osDbIsOnline, bool filmIsListed  )
        {

            if (!Settings.FilmOSDbDetailsDownloaderIsEnabled)
                return;

            if (!osDbIsOnline)
                return;

            if (!filmIsListed)
                return;

            

            var m = new Movie();

            Debugger.LogMessageToFile
                ("Attempting to download" +
                 " film details for item "
                 + item.Name + " from OSdb ..."); 


            mystruct = Proxy.GetImdbMovieDetails
                ("token", imdbid);

            Debugger.LogMessageToFile
                ("Retrieving data from OSdb response...");

            #region parse rsults and return details

            foreach (DictionaryEntry d in mystruct)
            {
                token = Convert.ToString(d.Key);
                if (token == "data")
                {
                    object value = d.Value;
                    Console.WriteLine(value.GetType());
                    XmlRpcStruct s = (XmlRpcStruct)value;
                    foreach (DictionaryEntry e in s)
                    {
                        token = Convert.ToString(e.Key);

                        if (token == "title")
                        {
                            string str = (string)e.Value;
                            m.Name = str;
                        }

                        if (token == "tagline")
                        {
                            string str = (string)e.Value;
                            m.Tagline = str;
                        }

                        if (token == "year")
                        {
                            string str = (string)e.Value;
                            m.Year = str;
                        }

                        if (token == "duration")
                        {
                            string str = (string)e.Value;
                            m.Duration = str;
                        }

                        if (token == "plot")
                        {
                            string str = (string)e.Value;
                            m.Plot = str;
                        }

                        if (token == "trivia")
                        {
                            string str = (string)e.Value;
                            m.Trivia = str;
                        }

                        if (token == "goofs")
                        {
                            string str = (string)e.Value;
                            m.Goofs = str;
                        }


                        token = Convert.ToString(e.Key);
                        if (token == "directors")
                        {
                            value = e.Value;
                            s = (XmlRpcStruct)value;
                            string str = "";
                            int i = 1;

                            foreach (DictionaryEntry f in s)
                            {
                                if (i == s.Count) str = (string)f.Value;
                                else str = f.Value + "|";
                                i++;
                            }
                            m.Directors = str;
                        }



                        token = Convert.ToString(e.Key);
                        if (token == "cast")
                        {
                            value = e.Value;
                            s = (XmlRpcStruct)value;
                            string str = "";
                            int i = 1;

                            foreach (DictionaryEntry f in s)
                            {
                                if (i == s.Count) str += (string)f.Value;
                                else str += f.Value + "|";
                                i++;
                            }
                            m.Cast = str;
                        }




                        token = Convert.ToString(e.Key);
                        if (token == "writers")
                        {
                            value = e.Value;
                            s = (XmlRpcStruct)value;
                            string str = "";
                            int i = 1;

                            foreach (DictionaryEntry f in s)
                            {
                                if (i == s.Count) str += (string)f.Value;
                                else str += f.Value + "|";
                                i++;
                            }
                            m.Writers = str;
                        }




                        token = Convert.ToString(e.Key);
                        if (token == "genres")
                        {
                            value = e.Value;
                            string[] genres = (string[])value;
                            string str = "";
                            int i = 1;

                            foreach (string genre in genres)
                            {
                                if (i == genres.Length)
                                    str += genre;
                                else
                                    str += genre + "|";
                                i++;
                            }
                            m.Genres = str;
                        }



                        token = Convert.ToString(e.Key);
                        if (token == "cover")
                        {
                            string str = (string)e.Value;
                            m.Cover = str;
                        }




                    }
                }
            }
            #endregion


            Debugger.LogMessageToFile("Saving results from item's tags...");
            #region save results to item's tags

            if (!String.IsNullOrEmpty(m.Name))
            {
                item.Name = m.Name;
                item.Tags["Title"] = m.Name;
            }
            //Item.Tags["Poster URL"] = m.Cover;

            if (Settings.FilmOSDbDetailsDownloaderIsEnabled)
            {
                if (!String.IsNullOrEmpty(m.Tagline))
                {
                    string TaglineTag;
                    if (Settings.MovieNightCompatibility)
                        TaglineTag = "Subtitle";
                    else TaglineTag = "Tagline";

                    item.Tags[TaglineTag] = m.Tagline;
                }

                if (!String.IsNullOrEmpty(m.Year))
                item.Tags["Year"] = m.Year;

                if (!String.IsNullOrEmpty(m.Duration))
                item.Tags["Runtime"] = m.Duration;

                if (!String.IsNullOrEmpty(m.Plot))
                item.Tags["Overview"] = m.Plot;

                if (!String.IsNullOrEmpty(m.Directors))
                item.Tags["Director"] = m.Directors;

                if (!String.IsNullOrEmpty(m.Cast))
                item.Tags["Actors"] = m.Cast;

                if (!String.IsNullOrEmpty(m.Genres))
                item.Tags["Genre"] = m.Genres;

                if (!String.IsNullOrEmpty(m.Writers))
                item.Tags["Writer"] = m.Writers;

                if (!String.IsNullOrEmpty(m.Trivia))
                item.Tags["Trivia"] = m.Trivia;

                if (!String.IsNullOrEmpty(m.Goofs))
                item.Tags["Mistakes"] = m.Goofs;

            }


            item.SaveTags();
            #endregion


        }


    }


}
