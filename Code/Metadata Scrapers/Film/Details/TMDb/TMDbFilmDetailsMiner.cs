using System;
using System.Collections.Generic;
using MeediFier.ImportingEngine;
using MeediOS;
using TMDbLib.Client;
using TMDbLib.Objects.General;

namespace MeediFier.Code.Metadata_Scrapers.Film.Details.TMDb
{
    class TMDbFilmDetailsMiner
    {








        public static void DownloadFilmDetailsFromTMDb(IMLItem item)
        {


            
            if (!MeediFier.Settings.TMDbFilmDetailsEnabled)
                return;



            string tmdbID = Helpers.GetTagValueFromItem(item, "TMDbID");



            //MessageBox.Show("TMDbID: " + tmdbID);


            if (String.IsNullOrEmpty(tmdbID))
            {

                Debugger.LogMessageToFile
                    ("The TMDb id for this film is unknown. " +
                     "Unable to download adittional film information from themoviedb.org.");
                
                return;
            
            }


            Helpers.UpdateProgress
                (MainImportingEngine.GeneralStatus, 
                "Downloading film details from TMDb...", item);




            TMDbClient client = new TMDbClient(MeediFier.Settings.TMDbApiKey);

            TMDbLib.Objects.Movies.Movie tmdbMovie = client.GetMovie(tmdbID);





            if (!String.IsNullOrEmpty(tmdbMovie.Title))
            {
                if (String.IsNullOrEmpty(Helpers.GetTagValueFromItem(item, "Title")))
                {
                    item.Tags["Title"] = tmdbMovie.Title;
                    item.SaveTags();
                }
            }



            if (!String.IsNullOrEmpty(tmdbMovie.Tagline))
            {
                if (String.IsNullOrEmpty(Helpers.GetTagValueFromItem(item, "Tagline")))
                {
                    item.Tags["Tagline"] = tmdbMovie.Tagline;
                    item.SaveTags();
                }
            }





            if (!String.IsNullOrEmpty(tmdbMovie.Status))
            {
                if (String.IsNullOrEmpty(Helpers.GetTagValueFromItem(item, "ReleaseStatus")))
                {
                    item.Tags["ReleaseStatus"] = tmdbMovie.Status;
                    item.SaveTags();
                }
            }



            if (!String.IsNullOrEmpty(tmdbMovie.Overview))
            {
                if (String.IsNullOrEmpty(Helpers.GetTagValueFromItem(item, "Overview")))
                {
                    item.Tags["Overview"] = tmdbMovie.Overview;
                    item.SaveTags();
                }
            }





            if (String.IsNullOrEmpty(Helpers.GetTagValueFromItem(item, "Genre")))
            {

                List<Genre> filmGenres = tmdbMovie.Genres;

                if (filmGenres.Count > 0)
                {

                    string filmGenresString = String.Empty;

                    foreach (var filmGenre in filmGenres)
                    {

                        filmGenresString = "|" + filmGenre.Name;

                    }

                    filmGenresString = filmGenresString + "|";

                    item.Tags["Genre"] = filmGenresString;

                }



            }








            if (!String.IsNullOrEmpty(Convert.ToString(tmdbMovie.Runtime)))
            {
                if (String.IsNullOrEmpty(Helpers.GetTagValueFromItem(item, "Runtime")))
                {
                    item.Tags["Runtime"] = Convert.ToString(tmdbMovie.Runtime);
                    item.SaveTags();
                }
            }





            if (!String.IsNullOrEmpty(Convert.ToString(tmdbMovie.ReleaseDate)))
            {
                if (String.IsNullOrEmpty(Helpers.GetTagValueFromItem(item, "ReleaseDate")))
                {
                    item.Tags["ReleaseDate"] = Convert.ToString(tmdbMovie.ReleaseDate);
                    item.SaveTags();
                }
            }



            if (!String.IsNullOrEmpty(Convert.ToString(tmdbMovie.VoteAverage)))
            {
                if (String.IsNullOrEmpty(Helpers.GetTagValueFromItem(item, "TMDbReview")))
                {
                    item.Tags["TMDbReview"] = Convert.ToString(tmdbMovie.VoteAverage);
                    item.SaveTags();
                }
            }





            if (!String.IsNullOrEmpty(Convert.ToString(tmdbMovie.Budget)))
            {
                if (String.IsNullOrEmpty(Helpers.GetTagValueFromItem(item, "Budget")))
                {
                    item.Tags["Budget"] = Convert.ToString(tmdbMovie.Budget);
                    item.SaveTags();
                }
            }






            if (!String.IsNullOrEmpty(Convert.ToString(tmdbMovie.Revenue)))
            {
                if (String.IsNullOrEmpty(Helpers.GetTagValueFromItem(item, "Revenue")))
                {
                    item.Tags["Revenue"] = Convert.ToString(tmdbMovie.Revenue);
                    item.SaveTags();
                }
            }





            if (!String.IsNullOrEmpty(tmdbMovie.Homepage))
            {
                if (String.IsNullOrEmpty(Helpers.GetTagValueFromItem(item, "Homepage")))
                {
                    item.Tags["Homepage"] = tmdbMovie.Homepage;
                    item.SaveTags();
                }
            }


            if (String.IsNullOrEmpty(Helpers.GetTagValueFromItem(item, "SpokenLanguages")))
            {

                List<TMDbLib.Objects.Movies.SpokenLanguage> filmSpokenLanguages = tmdbMovie.SpokenLanguages;

                if (filmSpokenLanguages.Count > 0)
                {

                    string spokenLanguagesString = String.Empty;

                    foreach (var filmSpokenLanguage in filmSpokenLanguages)
                    {

                        spokenLanguagesString = "|" + filmSpokenLanguage.Name;

                    }

                    spokenLanguagesString = spokenLanguagesString + "|";

                    item.Tags["SpokenLanguages"] = spokenLanguagesString;

                }



            }
            //item.Tags["Trailer"] = tmdbMovie.Trailers;

            item.SaveTags();

            

        }
    
    







        //public static void DownloadFilmDetailsFromTMDb(IMLItem item)
        //{


        //    if (!MeediFier.Settings.TMDbFilmDetailsEnabled)
        //        return;
            

        //    string tmDbRating = Helpers.GetTagValueFromItem(item, "TMDbRating");

        //    if (!String.IsNullOrEmpty(tmDbRating))
        //        return;

        //    string tmdbID = Helpers.GetTagValueFromItem(item, "TMDbID");

        //    if (String.IsNullOrEmpty(tmdbID))
        //    {
        //        Debugger.LogMessageToFile("The TMDb id for this film is unknown. Unable to download adittional film information from themoviedb.org.");
        //        return;
        //    }


        //    string tmDbUrl = "http://api.themoviedb.org/2.1/Movie.getInfo/en/xml/" + MeediFier.Settings.TMDbApiKey + "/" + tmdbID;
            
            
        //    Debugger.LogMessageToFile("Attempting to download film information from url " + tmDbUrl + "...");
        //    Helpers.UpdateProgress(MainImportingEngine.GeneralStatus, "Downloading film details from TMDb...", item);

        //    #region request film info
        //    WebResponse objResponse = MeediFier.Downloaders.TryDatabaseRequest(tmDbUrl, "Movies", "TheMovieDatabase", item);
        //    if (objResponse == null)
        //    {
        //        Debugger.LogMessageToFile("The web response from TMdb was not valid. Unable to download film info.");
        //        return;
        //    }
        //    Debugger.LogMessageToFile("Getting Response stream...");
        //    Stream stream = objResponse.GetResponseStream();
        //    #endregion


        //    #region Try to load XML stream
        //    XmlDocument doc = new XmlDocument();
        //    try
        //    {
        //        doc.Load(stream);
        //    }
        //    catch (Exception e)
        //    {
        //        Debugger.LogMessageToFile("An error occurred while trying to load TMDb's XML stream: " + e.ToString());
        //        return;
        //    }
        //    #endregion



        //    XmlNodeList nodelist = doc.ChildNodes;
        //    bool foundFirst = false;

        //    #region Parse node tree and get film details

        //    foreach (XmlNode node in nodelist)
        //    {
        //        //MessageBox.Show("First level: " + node.Name);
        //        if (node.HasChildNodes)
        //        {
        //            foreach (XmlNode childnode in node.ChildNodes)
        //            {
        //                //MessageBox.Show("Second level: " + node.Name);
        //                foreach (XmlNode result in node.ChildNodes)
        //                {
        //                    //MessageBox.Show("Third level: " + result.Name);
        //                    foreach (XmlNode unknown in result.ChildNodes)
        //                    {

        //                        if (unknown.Name != "movie" || foundFirst)
        //                            continue;

        //                        foundFirst = true;

        //                        //MessageBox.Show("Fifth level: " + unknown.Name);
        //                        XmlNode Tag = unknown.SelectSingleNode("id");

        //                        foreach (XmlNode tag in unknown.ChildNodes)
        //                        {

        //                            if (tag.Name == "rating")
        //                            {
        //                                if (!String.IsNullOrEmpty(tag.InnerText))
        //                                {
        //                                    if (tag.InnerText != "0")
        //                                    {
        //                                        if (tag.InnerText != "0.0")
        //                                        {
        //                                            item.Tags["TMDbRating"] = tag.InnerText;
        //                                            item.SaveTags();
        //                                        }
        //                                    }
        //                                }
        //                            }

        //                            if (tag.Name == "budget")
        //                            {
        //                                if (!String.IsNullOrEmpty(tag.InnerText))
        //                                {
        //                                    if (tag.InnerText != "0")
        //                                    {
        //                                        item.Tags["Budget"] = tag.InnerText;
        //                                        item.SaveTags();
        //                                    }
        //                                }
        //                            }

        //                            if (tag.Name == "revenue")
        //                            {
        //                                if (!String.IsNullOrEmpty(tag.InnerText))
        //                                {
        //                                    if (tag.InnerText != "0")
        //                                    {
        //                                        item.Tags["Revenue"] = tag.InnerText;
        //                                        item.SaveTags();
        //                                    }
        //                                }
        //                            }
        //                            if (tag.Name == "homepage")
        //                            {
        //                                if (!String.IsNullOrEmpty(tag.InnerText))
        //                                {
        //                                    item.Tags["FilmHomepage"] = tag.InnerText;
        //                                    item.SaveTags();
        //                                }
        //                            }

        //                            if (tag.Name == "production_countries")
        //                            {
        //                                string productionCountries = "|";

        //                                foreach (XmlNode country in tag.ChildNodes)
        //                                {
        //                                    foreach (XmlNode countryName in country.ChildNodes)
        //                                    {
        //                                        if (!String.IsNullOrEmpty(countryName.InnerText))
        //                                        {
        //                                            productionCountries = productionCountries + countryName.InnerText + "|";

        //                                        }
        //                                    }
        //                                }

        //                                item.Tags["ProductionCountries"] = tag.InnerText;
        //                                item.SaveTags();

        //                            }

        //                            if (tag.Name != "trailer") 
        //                                continue;

        //                            if (String.IsNullOrEmpty(tag.InnerText)) 
        //                                continue;

        //                            item.Tags["Trailer"] = tag.InnerText;

        //                            item.SaveTags();

        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    #endregion

        //    item.SaveTags();


        //}
    
    
    
    
    }


}
