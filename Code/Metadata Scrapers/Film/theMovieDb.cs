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
using System.Net;
using System.IO;
using System.Web.Script.Serialization;
using System.Xml;
using System.Windows.Forms;
#if USE_MEEDIO
using Meedio;
#elif USE_MEEDIOS
using MediaFairy.ImportingEngine;
using MeediOS;
#endif



namespace MediaFairy
{
    class TheMovieDb
    {

        public static string GetTmdbID(IMLItem item)
        {
            if (!Settings.TMDbFilmDetailsEnabled && !Settings.FilmBackdropArtDownloaderIsEnabled && !Settings.TMDbPostersEnabled)
                return string.Empty;

            string imdbID = Helpers.GetTagValueFromItem(item, "ImdbID");

            
            string tmdbID = string.Empty;

            string tmDbUrl = "http://api.themoviedb.org/2.1/Movie.imdbLookup/en/xml/" + Settings.TMDbApiKey + "/" + imdbID;
            
            
            Debugger.LogMessageToFile("Trying to get TMDb ID from url " + tmDbUrl + "...");


            #region request and return TMDbID


            WebResponse objResponse = Downloaders.TryDatabaseRequest(tmDbUrl, "Movies", "TheMovieDatabase", item);

            if (objResponse == null)
            {
                Debugger.LogMessageToFile("The web response from TMdb was not valid. The TMDb ID for this film cannot be retrieved.");
                return string.Empty;
            }


            Debugger.LogMessageToFile("Getting Response stream...");
            Stream stream = objResponse.GetResponseStream();
            #endregion


            XmlDocument doc = new XmlDocument();
            try
            {
                if (stream != null) doc.Load(stream);
            }
            catch (Exception)
            {
                try
                {
                    if (stream != null) doc.Load(stream);
                }
                catch (Exception e)
                {
                    StatusForm.statusForm.TrayIcon.ShowBalloonTip(5000, "Unable to parse online response.", "An unexpected error occurred while trying to read themoviedb.org response stream." + Environment.NewLine + "Please check the log file for more details on this error.", ToolTipIcon.Warning);
                    Debugger.LogMessageToFile("An unexpected error occurred while trying to read themoviedb.org response stream. The error was: " + e.ToString());
                    return string.Empty;
                }
            }




            XmlNodeList nodelist = doc.ChildNodes;
            bool foundFirst = false;


            foreach (XmlNode node in nodelist)
            {
                //MessageBox.Show("First level: " + node.Name);
                if (!node.HasChildNodes) continue;

                foreach (XmlNode childnode in node.ChildNodes)
                {

                    foreach (XmlNode result in node.ChildNodes)
                    {

                        foreach (XmlNode unknown in result.ChildNodes)
                        {

                            if (unknown.Name != "movie" || foundFirst) 
                                continue;

                            foundFirst = true;

                            XmlNode tag = unknown.SelectSingleNode("id");

                            if (tag == null)
                                return string.Empty;

                            if (String.IsNullOrEmpty(tag.InnerText))
                                return string.Empty;

                            tmdbID = tag.InnerText;
                            Debugger.LogMessageToFile("The film's TMDbID " + tmdbID + " was succesfully extracted from the response stream.");
                            item.Tags["TMDbID"] = tmdbID;
                            Debugger.LogMessageToFile("Saving TMDbID to item..");
                            item.SaveTags();



                        }
                    }
                }
            }


            return tmdbID;

        }




        public static void DownloadFilmDetailsFromTMDb(IMLItem item)
        {


            if (!Settings.TMDbFilmDetailsEnabled)
                return;
            

            string tmDbRating = Helpers.GetTagValueFromItem(item, "TMDbRating");

            if (!String.IsNullOrEmpty(tmDbRating))
                return;

            string tmdbID = Helpers.GetTagValueFromItem(item, "TMDbID");

            if (String.IsNullOrEmpty(tmdbID))
            {
                Debugger.LogMessageToFile("The TMDb id for this film is unknown. Unable to download adittional film information from themoviedb.org.");
                return;
            }


            string tmDbUrl = "http://api.themoviedb.org/2.1/Movie.getInfo/en/xml/" + Settings.TMDbApiKey + "/" + tmdbID;
            
            
            Debugger.LogMessageToFile("Attempting to download film information from url " + tmDbUrl + "...");
            Helpers.UpdateProgress(MainImportingEngine.GeneralStatus, "Downloading film details from TMDb...", item);

            #region request film info
            WebResponse objResponse = Downloaders.TryDatabaseRequest(tmDbUrl, "Movies", "TheMovieDatabase", item);
            if (objResponse == null)
            {
                Debugger.LogMessageToFile("The web response from TMdb was not valid. Unable to download film info.");
                return;
            }
            Debugger.LogMessageToFile("Getting Response stream...");
            Stream stream = objResponse.GetResponseStream();
            #endregion


            #region Try to load XML stream
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(stream);
            }
            catch (Exception e)
            {
                Debugger.LogMessageToFile("An error occurred while trying to load TMDb's XML stream: " + e.ToString());
                return;
            }
            #endregion



            XmlNodeList nodelist = doc.ChildNodes;
            bool foundFirst = false;

            #region Parse node tree and get film details

            foreach (XmlNode node in nodelist)
            {
                //MessageBox.Show("First level: " + node.Name);
                if (node.HasChildNodes)
                {
                    foreach (XmlNode childnode in node.ChildNodes)
                    {
                        //MessageBox.Show("Second level: " + node.Name);
                        foreach (XmlNode result in node.ChildNodes)
                        {
                            //MessageBox.Show("Third level: " + result.Name);
                            foreach (XmlNode unknown in result.ChildNodes)
                            {

                                if (unknown.Name != "movie" || foundFirst)
                                    continue;

                                foundFirst = true;

                                //MessageBox.Show("Fifth level: " + unknown.Name);
                                XmlNode Tag = unknown.SelectSingleNode("id");

                                foreach (XmlNode tag in unknown.ChildNodes)
                                {

                                    if (tag.Name == "rating")
                                    {
                                        if (!String.IsNullOrEmpty(tag.InnerText))
                                        {
                                            if (tag.InnerText != "0")
                                            {
                                                if (tag.InnerText != "0.0")
                                                {
                                                    item.Tags["TMDbRating"] = tag.InnerText;
                                                    item.SaveTags();
                                                }
                                            }
                                        }
                                    }

                                    if (tag.Name == "budget")
                                    {
                                        if (!String.IsNullOrEmpty(tag.InnerText))
                                        {
                                            if (tag.InnerText != "0")
                                            {
                                                item.Tags["Budget"] = tag.InnerText;
                                                item.SaveTags();
                                            }
                                        }
                                    }

                                    if (tag.Name == "revenue")
                                    {
                                        if (!String.IsNullOrEmpty(tag.InnerText))
                                        {
                                            if (tag.InnerText != "0")
                                            {
                                                item.Tags["Revenue"] = tag.InnerText;
                                                item.SaveTags();
                                            }
                                        }
                                    }
                                    if (tag.Name == "homepage")
                                    {
                                        if (!String.IsNullOrEmpty(tag.InnerText))
                                        {
                                            item.Tags["FilmHomepage"] = tag.InnerText;
                                            item.SaveTags();
                                        }
                                    }

                                    if (tag.Name == "production_countries")
                                    {
                                        string productionCountries = "|";

                                        foreach (XmlNode country in tag.ChildNodes)
                                        {
                                            foreach (XmlNode countryName in country.ChildNodes)
                                            {
                                                if (!String.IsNullOrEmpty(countryName.InnerText))
                                                {
                                                    productionCountries = productionCountries + countryName.InnerText + "|";

                                                }
                                            }
                                        }

                                        item.Tags["ProductionCountries"] = tag.InnerText;
                                        item.SaveTags();

                                    }

                                    if (tag.Name != "trailer") 
                                        continue;

                                    if (String.IsNullOrEmpty(tag.InnerText)) 
                                        continue;

                                    item.Tags["Trailer"] = tag.InnerText;

                                    item.SaveTags();

                                }
                            }
                        }
                    }
                }
            }
            #endregion

            item.SaveTags();


        }



        public static List<string> GetFilmPostersFromTMDb(IMLItem item, ref List<string> thumbUrls)
        {
            List<string> urlList = new List<string>();

            if (!Settings.TMDbPostersEnabled)
                return urlList;

            #region Contruct URL
            string tmdbID = Helpers.GetTagValueFromItem(item, "TMDbID");

            string tmDbUrl = "http://api.themoviedb.org/2.1/Movie.getInfo/en/xml/" + Settings.TMDbApiKey + "/" + tmdbID;
           
            #endregion


            #region request data stream
            Debugger.LogMessageToFile("Searching themoviedb.org for film posters...");
            Helpers.UpdateProgress(MainImportingEngine.GeneralStatus, "Searcing themoviedb.org for film posters...", item);

            WebResponse objResponse = Downloaders.TryDatabaseRequest(tmDbUrl, "Movies", "TheMovieDatabase", item);
            if (objResponse == null)
            {
                Debugger.LogMessageToFile("The web response from TMdb was not valid. Unable to get posters for this film.");
                return urlList;
            }
            Debugger.LogMessageToFile("Getting Response stream...");
            Stream stream = objResponse.GetResponseStream();
            #endregion


            #region Try to load XML stream
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(stream);
            }
            catch (Exception e)
            {
                Debugger.LogMessageToFile("An error occurred while trying to load TMDb's XML stream: " + e.ToString());
                return urlList;
            }
            #endregion


            XmlNodeList nodelist = doc.ChildNodes;
            bool FoundFirst = false;

            #region Parse node tree and get film details

            foreach (XmlNode node in nodelist)
            {
                //MessageBox.Show("First level: " + node.Name);
                if (node.HasChildNodes)
                {
                    foreach (XmlNode childnode in node.ChildNodes)
                    {
                        //MessageBox.Show("Second level: " + node.Name);
                        foreach (XmlNode result in node.ChildNodes)
                        {
                            //MessageBox.Show("Third level: " + result.Name);
                            foreach (XmlNode unknown in result.ChildNodes)
                            {
                                //MessageBox.Show("Fourth level: " + unknown.Name);
                                if (unknown.Name == "movie" && !FoundFirst)
                                {
                                    FoundFirst = true;
                                    //MessageBox.Show("Fifth level: " + unknown.Name);

                                    foreach (XmlNode tag in unknown.ChildNodes)
                                    {
                                        if (tag.Name == "poster")
                                        {
                                            XmlAttributeCollection atrributes = tag.Attributes;
                                            foreach (XmlAttribute attribute in atrributes)
                                            {
                                                if (attribute.Name == "size")
                                                {
                                                    if (attribute.Value == "original")
                                                    {
                                                        //if ( Helpers.RemoteFileExists(tag.InnerText) )
                                                        //{
                                                        //    MessageBox.Show("Image exists!");
                                                            urlList.Add(tag.InnerText);
                                                        //}
                                                        //else
                                                        //{
                                                        //    MessageBox.Show("Image does not exist");
                                                        //}
                                                    }
                                                    if (attribute.Value == "mid")
                                                    {
                                                        //if (Helpers.RemoteFileExists(tag.InnerText))
                                                        //{
                                                            //MessageBox.Show("Image exists!");
                                                            thumbUrls.Add(tag.InnerText);
                                                        //}
                                                        //else
                                                        //{
                                                        //  MessageBox.Show("Image does not exist");
                                                        //}
                                                        
                                                    }

                                                }
                                            }
                                        }




                                    }

                                }
                            }
                        }
                    }
                }
            }
            #endregion

            return urlList;

        }


        public static List<string> GetFilmBackdropsFromTMDb(IMLItem item, ref List<string> thumbUrls)
        {
            List<string> urlList = new List<string>();

            if (!Settings.FilmBackdropArtDownloaderIsEnabled)
                return urlList;

            #region Contruct URL
            string tmdbID = Helpers.GetTagValueFromItem(item, "TMDbID");

            //string tmDbUrl = "http://api.themoviedb.org/2.1/Movie.getInfo/en/xml/" + Settings.TMDbApiKey + "/" + tmdbID;

            string tmDbUrl = "http://api.themoviedb.org/3/movie/" + tmdbID + "/images?api_key=" + Settings.TMDbApiKey;
            #endregion


            string resultJSON = HtmlDownloaders.DownloadHTMLfromURL(tmDbUrl);

            JavaScriptSerializer serializer = new JavaScriptSerializer();

            Foo foo = serializer.Deserialize<Foo>(resultJSON);


            foreach (var subObject in foo.objects)
            {

                string objName = subObject.attributes.OBJECT_NAME;
                MessageBox.Show(objName);

                string objType = subObject.attributes.OBJECT_TYPE;
                MessageBox.Show(objType);

            }



            Debugger.LogMessageToFile(resultJSON);

            #region request data stream
            Debugger.LogMessageToFile("Searching themoviedb.org for film fanart images...");
            Helpers.UpdateProgress(MainImportingEngine.GeneralStatus, "Searcing themoviedb.org for film fanart images...", item);

            WebResponse objResponse = Downloaders.TryDatabaseRequest(tmDbUrl, "Movies", "TheMovieDatabase", item);
            if (objResponse == null)
            {
                Debugger.LogMessageToFile("The web response from TMdb was not valid. Unable to get backdrops for this film.");
                return urlList;
            }
            Debugger.LogMessageToFile("Getting Response stream...");
            Stream stream = objResponse.GetResponseStream();
            #endregion


           

            #region Try to load XML stream
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(stream);
            }
            catch (Exception e)
            {
                Debugger.LogMessageToFile("An error occurred while trying to load TMDb's XML stream: " + e.ToString());
                return urlList;
            }
            #endregion


            XmlNodeList nodelist = doc.ChildNodes;
            bool FoundFirst = false;

            #region Parse node tree and get film details

            foreach (XmlNode node in nodelist)
            {
                //MessageBox.Show("First level: " + node.Name);
                if (node.HasChildNodes)
                {
                    foreach (XmlNode childnode in node.ChildNodes)
                    {
                        //MessageBox.Show("Second level: " + node.Name);
                        foreach (XmlNode result in node.ChildNodes)
                        {
                            //MessageBox.Show("Third level: " + result.Name);
                            foreach (XmlNode unknown in result.ChildNodes)
                            {
                                //MessageBox.Show("Fourth level: " + unknown.Name);
                                if (unknown.Name == "movie" && !FoundFirst)
                                {
                                    FoundFirst = true;
                                    //MessageBox.Show("Fifth level: " + unknown.Name);

                                    foreach (XmlNode tag in unknown.ChildNodes)
                                    {
                                        if (tag.Name == "backdrop")
                                        {
                                            XmlAttributeCollection atrributes = tag.Attributes;
                                            foreach (XmlAttribute attribute in atrributes)
                                            {
                                                if (attribute.Name == "size")
                                                {
                                                    if (attribute.Value == "original")
                                                    {

                                                        //if (Helpers.RemoteFileExists(tag.InnerText))
                                                        //{
                                                        //  MessageBox.Show("Remote image exists!");
                                                            urlList.Add(tag.InnerText);
                                                        //}
                                                        //else
                                                        //{
                                                        //    MessageBox.Show("Remote image does not exist");
                                                        //}
                                               
                                                    }
                                                    if (attribute.Value == "mid")
                                                    {

                                                        //if (Helpers.RemoteFileExists(tag.InnerText))
                                                        //{
                                                        //    MessageBox.Show("Remote image exists!");
                                                              thumbUrls.Add(tag.InnerText);
                                                        //}
                                                        //else
                                                        //{
                                                        //    MessageBox.Show("Remote image does not exist");
                                                        //}
                                                       
                                                    }

                                                }
                                            }
                                        }




                                    }

                                }
                            }
                        }
                    }
                }
            }
            #endregion

            return urlList;

        }



    }
}
