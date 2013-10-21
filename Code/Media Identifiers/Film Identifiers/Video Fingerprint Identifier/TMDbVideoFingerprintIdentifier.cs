using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using MeediFier.ImportingEngine;
using MeediOS;





namespace MeediFier.VideoFingerprintIdentifier
{




    class TMDbVideoFingerprintIdentifier
    {




        internal static string IdentifyMovieByHashTMDb
            (IMLItem item, ConnectionResult connectionresult)
        {

            //TmdbAPI api = new TmdbAPI(Importer.ApiKey); 

            

            #region function variables
            string name = item.Name;
            string moviehash = Helpers.GetTagValueFromItem(item, "Hash");
            string imdbid = Helpers.GetTagValueFromItem(item, "ImdbID");
            string location = item.Location;
            FileInfo fi = new FileInfo(location);
            long byteSize = fi.Length;
            #endregion


            if (!Settings.EnableVideoHasher)
                return imdbid;

            if (!connectionresult.InternetConnectionAvailable && Settings.ConnectionDiagnosticsEnabled)
                return imdbid;

            if (!String.IsNullOrEmpty(imdbid))
                return imdbid;

            if (String.IsNullOrEmpty(moviehash))
                return imdbid;


            #region Get ImdBID

            string TMDbUrl = "http://api.themoviedb.org/2.1/Media.getInfo/en/xml/" + Settings.TMDbApiKey + "/" + moviehash + "/" + byteSize;
            Debugger.LogMessageToFile("Trying to get IMDb ID from url " + TMDbUrl + "...");
       
            #region request and return Response stream
            WebResponse objResponse = Downloaders.TryDatabaseRequest(TMDbUrl, "Movies", "TheMovieDatabase", item);
            if (objResponse == null)
            {
                Debugger.LogMessageToFile("The web response from TMdb was not valid. The IMDb ID for this film cannot be retrieved.");
                return imdbid;
            }
            Debugger.LogMessageToFile("Getting Response stream...");
            Stream stream = objResponse.GetResponseStream();
            #endregion

            

            #region Load XML document
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(stream);
            }
            catch (Exception)
            {
                try
                {
                    doc.Load(stream);
                }
                catch (Exception e)
                {
                    StatusForm.statusForm.TrayIcon.ShowBalloonTip(5000, "Unable to parse online response.", "An unexpected error occurred while trying to read themoviedb.org response stream." + Environment.NewLine + "Please check the log file for more details on this error.", ToolTipIcon.Warning);
                    Debugger.LogMessageToFile("An unexpected error occurred while trying to read themoviedb.org response stream. The error was: " + e.ToString());
                    return imdbid;
                }
            }
            #endregion


            //string XmlText = doc.InnerText;
            //MessageBox.Show(XmlText);

            #region Parse XML nodes to get IMDb ID
            XmlNodeList nodelist = doc.ChildNodes;
            bool FoundFirst = false;
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

                            foreach (XmlNode movie in result.ChildNodes)
                            {
                                //MessageBox.Show("Fourth level: " + movie.Name);
                                if (movie.Name == "movie" && !FoundFirst)
                                {
                                    FoundFirst = true;
                                    //MessageBox.Show("Fifth level: " + movie.Name);
                                    XmlNode Tag = movie.SelectSingleNode("imdb_id");

                                    if (Tag == null)
                                        return imdbid;

                                    if (String.IsNullOrEmpty(Tag.InnerText))
                                        return imdbid;

                                    imdbid = Tag.InnerText;
                                    Debugger.LogMessageToFile("The film's IMDb ID " + imdbid + " was succesfully extracted from the TMDb response stream.");

                                }
                            }
                        }
                    }
                }
            }
            #endregion


            #region Save found IMDbID to item's tag
            if (!String.IsNullOrEmpty(imdbid))
            {


                Debugger.LogMessageToFile("Online identification by video fingerprint (using TMDb) was successful!");
                MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress, "Online identification by video fingerprint (using TMDb) was succesfull!");
                Thread.Sleep(2000);
                item.Tags["ImdbID"] = imdbid;
                item.SaveTags();
            }
            else
            {
                Debugger.LogMessageToFile("Online identification by video's fingerprint (using TMDb) was not possible.");
            }
            #endregion

            #endregion


            return imdbid;
        }
    }



}
