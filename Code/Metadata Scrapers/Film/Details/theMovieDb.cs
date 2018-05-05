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
using System.Xml;
using System.Windows.Forms;
using TMDbLib.Client;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Search;

#if USE_MEEDIO
using Meedio;
#elif USE_MEEDIOS
using MeediOS;
#endif



namespace MeediFier
{
    class TheMovieDb
    {



        public static int TmdbGetTmdbIdByFilmImdbId(IMLItem item)
        {


            if (!Settings.TMDbFilmDetailsEnabled
                && !Settings.FilmBackdropArtDownloaderIsEnabled
                && !Settings.TMDbPostersEnabled)
                return 0;


            string filmTitle = Helpers.GetTagValueFromItem
                (item, "Title");


            string imdbID = Helpers.GetTagValueFromItem(item, "ImdbID");


            Helpers.UpdateProgress
                ("Updating Movies section...", 
                "Attempting to acquire the film's TMDb ID...", item);

            int tmdbID = 0;

            TMDbClient client = new TMDbClient("c2c73ebd1e25cbc29cf61158c04ad78a");


            try
            {


            SearchContainer<SearchMovie> results = client.SearchMovie(filmTitle);


            if (results.Results.Count <= 0)
                return 0;

            List<SearchMovie> filmSearchResults = results.Results;

            foreach (SearchMovie filmResult in filmSearchResults)
            {


                tmdbID = filmResult.Id;


                //MessageBox.Show(Convert.ToString(tmdbID));


                TMDbLib.Objects.Movies.Movie film 
                    = client.GetMovie(tmdbID);


                if (film.ImdbId != imdbID)
                    continue;


                //MessageBox.Show(film.ImdbId);


                Debugger.LogMessageToFile
                    ("The film's TMDbID " + tmdbID +
                     " was succesfully extracted from the response stream.");

                item.Tags["TMDbID"] = Convert.ToString(tmdbID);

                Debugger.LogMessageToFile
                    ("Saving TMDbID to item..");

                item.SaveTags();





                return tmdbID;

            }



            }
            catch (Exception exception)
            {

                Debugger.LogMessageToFile
                    ("[TMDb Film Title Matching Engine] " +
                     "An error occured while trying to perform TMDb search " +
                     "for the film with Title: " + filmTitle + ". " +
                     "The error was: " + exception);
                
            }

            return tmdbID;
        }












        public static string GetTmdbIdByImdbId(IMLItem item)
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
    }
}
