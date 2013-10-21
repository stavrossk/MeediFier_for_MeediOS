using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MeediFier.IMDb;
using MeediOS;

namespace MeediFier.Code.Metadata_Scrapers.Film.IMDb.Title_Matching_Engines.IMDb
{

    class IMDbTitleMatchingEngineInternal
    {


        internal static void IdentifyFilmByIMDbInternalTitleMatchingEngine
            (IMLItem item, ConnectionResult connectionresult)
        {



            #region Return cases


            string imdbid = Helpers.GetTagValueFromItem(item, "ImdbID");

            if (CheckIfFilmIsAlreadyIdentified(imdbid)) return;

            if (CheckIfItemIsNotListed(item)) return;

            if (CheckIfInternetConnectionIsAvailable
                (item, connectionresult)) return;


            Debugger.LogMessageToFile
                ("[IMDb internal film Title matching engine] " +
                 "Starting identification by film's Title...");


            string filmTitle;
            string filmReleaseYear;


            filmTitle = ExtractKeyFieldsFromLibrary
                (item, out filmReleaseYear);



            if (CheckIfFilmTitleIsAvailable(item, filmTitle)) return;

            #endregion



            #region perform search

            Debugger.LogMessageToFile
                ("[IMDb internal film Title matching engine] " +
                 "Searching for films with Title '" + filmTitle + "'...");
            
            Helpers.UpdateProgress
                ("Updating Movies Section...", 
                "Searching IMDb for films with Title '" 
                + filmTitle + "'...", item);

            IMDbOperations imdbOp = new IMDbOperations();

            //TODO: Fix this recursive mess, here should be IMDbConventionalFilmSearchEngine SearchForTitleReturnResults()
            if (!imdbOp.SearchIMDbMovies())
                return;
            

            #endregion



            if ( !IMDbTitleMatchingEngine
                .TitleMatchingEngineTakeAction
                (item, filmReleaseYear, imdbOp))
                return;
            


            if (CheckIfItemIsNotListedAndIgnoreOrDeleteIt(item, imdbOp)) 
                return;


            #region Save IMDbID to library
            imdbid = imdbOp.ImdbId;
            item.Tags["ImdbID"] = imdbid;
            item.SaveTags();
            Debugger.LogMessageToFile("[IMDb video identifier] IMDb ID after matching by Title: " + imdbid);
            #endregion
        
        
        }



        private static bool CheckIfFilmTitleIsAvailable(IMLItem item, string filmTitle)
        {
            #region Return if Title or Name fields are not populated

            if (String.IsNullOrEmpty(filmTitle))
            {
                Debugger.LogMessageToFile
                    ("[IMDb internal film Title matching engine] " +
                     "This item's Title or Name fields are not populated. " +
                     "Online identification by Title cannot proceed. Returning...");

                Helpers.UpdateProgress
                    ("Updating Movies Section...",
                        "This item's Title or Name fields are not populated. " +
                        "Online identification by Title will be skipped.", item);

                Thread.Sleep(5000);

                return true;
            }

            #endregion

            return false;
        }


        private static bool CheckIfItemIsNotListedAndIgnoreOrDeleteIt(IMLItem item, IMDbOperations imdbOp)
        {

            if (!imdbOp.NotListed) return false;
            
            Debugger.LogMessageToFile
                ("[IMDb video identifier] The Item "
                 + item.Name + "was set as 'Not Listed'.");

            item.Tags["NotListed"] = "true";

            item.SaveTags();

            if (!imdbOp.DeleteItem) return true;
            Debugger.LogMessageToFile
                ("[IMDb video identifier] " +
                 "The Item " + item.Name + "was marked for removal from section.");

            item.Tags["ToDelete"] = "true";

            item.SaveTags();

            return true;
        
        }



        private static string ExtractKeyFieldsFromLibrary(IMLItem item, out string filmReleaseYear)
        {

            string filmTitle;

            #region ...extract the film's title or year from library fields

            filmReleaseYear = Helpers.GetTagValueFromItem(item, "Year").Trim();

            filmTitle = Helpers.GetTagValueFromItem(item, "Title").Trim();

            if (String.IsNullOrEmpty(filmTitle))
            {
                filmTitle = item.Name;
            }

            Debugger.LogMessageToFile
                ("[IMDb internal film Title matching engine] " +
                 "Video's Title retrieved from item's tags: " + filmTitle);

            Debugger.LogMessageToFile
                ("[IMDb internal film Title matching engine] " +
                 "Video's Year retrieved from item's tags: " + filmReleaseYear);

            #endregion

            return filmTitle;
        }

        private static bool CheckIfInternetConnectionIsAvailable(IMLItem item, ConnectionResult connectionresult)
        {
            if (connectionresult.InternetConnectionAvailable || !MeediFier.Settings.ConnectionDiagnosticsEnabled)
                return false;
            Debugger.LogMessageToFile
                ("[IMDb internal film Title matching engine] " +
                 "Internet connection is unavailable. " +
                 "MeediFier will skip the online lookup for this film.");

            Helpers.UpdateProgress
                ("Updating Movies Section...",
                    "Internet connection is unavailable. " +
                    "MediaFairy will skip the online lookup for this film.", item);

            Thread.Sleep(2000);

            return true;
        }

        private static bool CheckIfItemIsNotListed(IMLItem item)
        {

            if (Helpers.GetTagValueFromItem
                (item, "NotListed") != "true") 
                return false;
            
            Debugger.LogMessageToFile(
                "[IMDb internal film Title matching engine] " +
                "This Item is not listed on IMDb. Returning...");
            
            return true;
        
        }


        private static bool CheckIfFilmIsAlreadyIdentified(string imdbid)
        {
            #region Return if item is already identified

            if (!String.IsNullOrEmpty(imdbid))
            {
                Debugger.LogMessageToFile
                    ("[IMDb internal film Title matching engine] " +
                     "This film is already identfied. Returning...");

                return true;
            }

            #endregion

            return false;
        }
    }



}
