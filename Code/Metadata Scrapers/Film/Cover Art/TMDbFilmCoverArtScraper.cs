using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using MeediFier.ImportingEngine;
using MeediOS;

namespace MeediFier.Code.Metadata_Scrapers.Film.Cover_Art
{
    class TMDbFilmCoverArtScraper
    {
        public static List<string> GetFilmPostersFromTMDb(IMLItem item, ref List<string> thumbUrls)
        {
            List<string> urlList = new List<string>();

            if (!MeediFier.Settings.TMDbPostersEnabled)
                return urlList;

            #region Contruct URL
            string tmdbID = Helpers.GetTagValueFromItem(item, "TMDbID");

            string tmDbUrl = "http://api.themoviedb.org/2.1/Movie.getInfo/en/xml/" + MeediFier.Settings.TMDbApiKey + "/" + tmdbID;
           
            #endregion


            #region request data stream
            Debugger.LogMessageToFile("Searching themoviedb.org for film posters...");
            Helpers.UpdateProgress(MainImportingEngine.GeneralStatus, "Searcing themoviedb.org for film posters...", item);

            WebResponse objResponse = MeediFier.Downloaders.TryDatabaseRequest(tmDbUrl, "Movies", "TheMovieDatabase", item);
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
    }
}
