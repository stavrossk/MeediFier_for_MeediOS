using System;
using System.Diagnostics;
using System.Threading;
using MeediFier.Code.Downloaders;
using MeediOS;

namespace MeediFier.Code.Metadata_Scrapers.Film.IMDb.Title_Matching_Engine
{


    class IMDbTitleIdentifierExternal
    {


        internal static string IdentifyVideoByIMDbExternal
            (IMLItem item, string imdbid, ConnectionResult connectionresult)
        {

            //TODO: Improve external IMDb film search engine, add a Title matching engine to it.


            if (!connectionresult.InternetConnectionAvailable 
                && MeediFier.Settings.ConnectionDiagnosticsEnabled)
            {

                Debugger.LogMessageToFile
                    ("[IMDb video identifier] " +
                     "Internet connection was not detected. " +
                     "Unable to identify this video by Title " +
                     "using the external (web-based) engine.");
                
                return imdbid;
            
            }


            if (!String.IsNullOrEmpty(imdbid))
            {

                Debugger.LogMessageToFile
                    ("[IMDb video identifier] " +
                     "IMDbID is already availabe for this title. " +
                     "Will not need to use the video identifier " +
                     "by Title using the external (web-based) engine.");
                
                return imdbid;
            
            }


            #region return if item is not listed
            
            if (Helpers.GetTagValueFromItem(item, "NotListed") == "true")
            {

                Debugger.LogMessageToFile
                    ("[IMDb video identifier] " +
                     "This Item is not listed on IMDb. Returning...");
                
                return imdbid;
            
            }

            #endregion


            Debugger.LogMessageToFile
                ("[IMDb video identifier] " +
                 "Continuing to identify by IMDb using external web API...");

            string titleNormalized;
            
            var year = ExtractTitleKeysFromLibrary
                (item, out titleNormalized);


            string searchString = ConstructSearchString
                (titleNormalized, year);

            imdbid = ExtractIMDbIdFromExternalEngine
                (item, searchString);

            AssertSuccessfulIdentification
                (item, imdbid);

            #region Save IMDbID to library
            
            item.Tags["ImdbID"] = imdbid;
            
            item.SaveTags();
            
            Debugger.LogMessageToFile
                ("[IMDb video identifier] " +
                 "IMDb ID after matching by Title: " 
                 + imdbid);
            
            #endregion


            return imdbid;
        
        }



        private static string ExtractTitleKeysFromLibrary
            (IMLItem item, out string titleNormalized)
        {

            string year = Helpers.GetTagValueFromItem(item, "Year");
            
            string title = Helpers.GetTagValueFromItem(item, "Title");

            if (String.IsNullOrEmpty(title))
                title = item.Name;

            titleNormalized = title.Replace(" ", "+");


            Debugger.LogMessageToFile
                ("[IMDb video identifier] " +
                 "Video's Title retrieved from item's tags: " + title);

            Debugger.LogMessageToFile
                ("[IMDb video identifier] " +
                 "Video's Year retrieved from item's tags: " + year);

            return year;
        }


        private static string ExtractIMDbIdFromExternalEngine
            (IMLItem item, string searchString)
        {

            Helpers.UpdateProgress
                ("Updating Movies section...",
                 "Trying to dentifying film by Title " +
                 "(using external web engine)... ", item);

            Stopwatch watch = new Stopwatch();

            watch.Start();


            string imdbid = WebDataMiner.MineWebDataReturnFirstMatch
                (searchString,
                 @"""imdburl"":""http:\\/\\/www.imdb.com\\/title\\/(?<IMDbID>.*?)\\/""");


            long millisecondsnow = watch.ElapsedMilliseconds;


            //MessageBox.Show
            //    (@"External search needed " 
            //    + millisecondsnow 
            //    + @" ms.");
            
            return imdbid;
        
        }



        private static void AssertSuccessfulIdentification(IMLItem item, string imdbid)
        {

            if (String.IsNullOrEmpty(imdbid)) return;
            
            Debugger.LogMessageToFile
                ("[IMDb video identifier] " +
                 "IMDb identification by Title " +
                 "(using external engine) was succesful!");

            Helpers.UpdateProgress
                ("Updating Movies section...",
                 "IMDb identification by Title " +
                 "(external engine) was succesful!", item);

            Thread.Sleep(2000);
        }


        private static string ConstructSearchString(string titleNormalized, string year)
        {

            string searchString;
            
            if (String.IsNullOrEmpty(year))
                searchString = "http://www.deanclatworthy.com/imdb/?q=" + titleNormalized;
            else 
                searchString = "http://www.deanclatworthy.com/imdb/?q=" + titleNormalized + "&year=" + year;
            
            return searchString;
        
        }



    }

}
