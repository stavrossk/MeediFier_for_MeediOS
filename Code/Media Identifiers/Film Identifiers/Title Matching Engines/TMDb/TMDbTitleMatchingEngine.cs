using System;
using System.Windows.Forms;
using MeediOS;
using TMDbLib.Client;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Search;

namespace MeediFier.Code.Media_Identifiers.Film_Identifiers.Title_Matching_Engines.TMDb
{


    internal class TMDbTitleMatchingEngine
    {

        internal static string IdentifyFilmByTMDbFilmTitleMatchingEngine
            (IMLItem item)
        {


            string filmTitle = item.Name;


            string filmImdbId = Helpers.GetTagValueFromItem(item, "ImdbID");

            //MessageBox.Show("Film title: " + filmTitle);

            if (String.IsNullOrEmpty(filmTitle))
                return filmImdbId;


            if (!String.IsNullOrEmpty(filmImdbId))
                return filmImdbId;




            int filmReleaseYear = -1;

            string filmReleaseYearString = Helpers.GetTagValueFromItem(item, "Year").Trim();

            if (!String.IsNullOrEmpty(filmReleaseYearString))
            {

                bool yarStringIsValidNumber = true;

                foreach (var digit in filmReleaseYearString)
                {
                    if (!Char.IsNumber(digit))
                        yarStringIsValidNumber = false;
                }

                if (yarStringIsValidNumber)
                {

                    filmReleaseYear = Convert.ToInt16(filmReleaseYearString);

                }

            }


            Helpers.UpdateProgress
                ("Updating Movies Section...", 
                "Attempting to identify film by title using TMDb...", item);


            TMDbClient tmDbClient = new TMDbClient(MeediFier.Settings.TMDbApiKey);


            SearchContainer<SearchMovie> filmSearchResults 
                = tmDbClient.SearchMovie
                (filmTitle, "", -1, false, filmReleaseYear);
            
            //SearchContainer<SearchMovie> filmSearchResults = tmDbClient.SearchMovie(filmTitle);



            if (filmSearchResults.Results.Count <= 0)
                return filmImdbId;
            

            SearchMovie firstMovieInSearchResults = filmSearchResults.Results[0];


            TMDbLib.Objects.Movies.Movie movie = tmDbClient.GetMovie(firstMovieInSearchResults.Id);

            filmImdbId = movie.ImdbId;



            //MessageBox.Show(filmImdbId);


            if (String.IsNullOrEmpty(filmImdbId))
                return filmImdbId;


            item.Tags["ImdbID"] = filmImdbId;
            item.SaveTags();

            return filmImdbId;

        }

    
    }


}
