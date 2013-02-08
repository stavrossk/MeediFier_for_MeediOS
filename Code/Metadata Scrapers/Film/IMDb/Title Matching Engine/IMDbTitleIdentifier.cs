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
using System.Threading;
using System.Diagnostics;
using MeediOS;
#if USE_MEEDIO
using Meedio;
#elif USE_MEEDIOS

#endif

namespace MediaFairy.IMDb
{
    static class IMDbTitleIdentifier
    {

        public static string IdentifyVideoByTitle(IMLItem item, string imdbid, IMDbOperations imdbOp, string year, ConnectionResult connectionresult)
        {

            #region Return Cases
            if (!Settings.EnableNameMatching)
            {
                Debugger.LogMessageToFile("[IMDb Film Title Matcher] IMDb Title matching is disabled. Returning...");
                return imdbid;
            }

            if (!String.IsNullOrEmpty(imdbid))
            {
                Debugger.LogMessageToFile("[IMDb Film Title Matcher] This item is already identified. Returning...");
                return imdbid;
            }
            #endregion

            #region Perform film Title matching

            if (Settings.PrimaryFilmTitleMatchingEngine == "Internal")
            {
                Debugger.LogMessageToFile("[IMDb Film Title Matcher] Primary film Title matching engine was set to 'Internal'.");

                imdbid = IdentifyVideoByIMDbInternal(item, imdbid, imdbOp, year, connectionresult);
                imdbid = IdentifyVideoByIMDbExternal(item, imdbid, connectionresult);

            }
            else
            {
                Debugger.LogMessageToFile("[IMDb Film Title Matcher] Primary film Title matching engine was set to 'External'.");

                imdbid = IdentifyVideoByIMDbExternal(item, imdbid, connectionresult);
                imdbid = IdentifyVideoByIMDbInternal(item, imdbid, imdbOp, year, connectionresult);

            }
            #endregion

            return imdbid;
        }



        private static string IdentifyVideoByIMDbInternal(IMLItem item, string imdbid, IMDbOperations imdbOp, string year, ConnectionResult connectionresult)
        {
            if (year == null) throw new ArgumentNullException("year");

            #region Return cases

            #region Return if item is already identified
            if (!String.IsNullOrEmpty(imdbid))
            {
                Debugger.LogMessageToFile("[IMDb internal film Title matching engine] This film is already identfied. Returning...");
                return imdbid;
            }
            #endregion

            #region check if imdbOP.IMDbID is filled, if yes save it to library and return

            if (!String.IsNullOrEmpty(imdbOp.ImdbId))
            {

                #region Save IMDbID to library
                imdbid = imdbOp.ImdbId;
                item.Tags["ImdbID"] = imdbid;
                item.SaveTags();
                Debugger.LogMessageToFile("[IMDb internal film Title matching engine] IMDb ID was found in imdbOP._imdbid . Will save it to library and return.");
                #endregion

                return imdbid;

            }

            #endregion

            #region return if item is not listed
            if (Helpers.GetTagValueFromItem(item, "NotListed") == "true")
            {
                Debugger.LogMessageToFile("[IMDb internal film Title matching engine] This Item is not listed on IMDb. Returning...");
                return imdbid;
            }

            #endregion

            #region return if internet connection is unavailable
            if (!connectionresult.InternetConnectionAvailable && Settings.ConnectionDiagnosticsEnabled)
            {
                Debugger.LogMessageToFile("[IMDb internal film Title matching engine] Internet connection is unavailable. MediaFairy will skip the online lookup for this film.");
                Helpers.UpdateProgress("Updating Movies Section...", "Internet connection is unavailable. MediaFairy will skip the online lookup for this film.", item);
                Thread.Sleep(2000);
                return imdbid;
            }
            #endregion

            Debugger.LogMessageToFile("[IMDb internal film Title matching engine] Starting identification by film's Title...");

            #region ...extract the film's title or year from library fields

            year = Helpers.GetTagValueFromItem(item, "Year");
            imdbOp.Title = Helpers.GetTagValueFromItem(item, "Title");

            if (String.IsNullOrEmpty(imdbOp.Title))
            {
                    imdbOp.Title = item.Name;
            }

            Debugger.LogMessageToFile("[IMDb internal film Title matching engine] Video's Title retrieved from item's tags: " + imdbOp.Title);
            Debugger.LogMessageToFile("[IMDb internal film Title matching engine] Video's Year retrieved from item's tags: " + year);

            #endregion

            #region Return if Title or Name fields are not populated
            if (String.IsNullOrEmpty(imdbOp.Title.Trim()) )
            {
                Debugger.LogMessageToFile("[IMDb internal film Title matching engine] This item's Title or Name fields are not populated. Online identification by Title cannot proceed. Returning...");
                Helpers.UpdateProgress("Updating Movies Section...", "This item's Title or Name fields are not populated. Online identification by Title will be skipped.", item);
                Thread.Sleep(5000);
                return imdbid;
            }
            #endregion

            #endregion


            #region Find imdbid using item's name or Title

            #region perform search

            Debugger.LogMessageToFile("[IMDb internal film Title matching engine] Searching for films with Title '" + imdbOp.Title + "'...");
            Helpers.UpdateProgress("Updating Movies Section...", "Searching IMDb for films with Title '" + imdbOp.Title + "'...", item);

            //TODO: Fix this recursive mess, here should be IMDbConventionalFilmSearchEngine SearchForTitleReturnResults()
            if (!imdbOp.SearchIMDbMovies())
                return imdbid;
            

            #endregion



            if ( !IMDbTitleMatchingEngine.TitleMatchingEngineTakeAction(item, year, imdbOp))
                return imdbid;
            


            #region check if the item is not listed, and ignore it or delete it
            if (imdbOp.NotListed)
            {
                Debugger.LogMessageToFile("[IMDb video identifier] The Item " + item.Name + "was set as 'Not Listed'.");
                item.Tags["NotListed"] = "true";
                item.SaveTags();

                if (imdbOp.DeleteItem)
                {
                    Debugger.LogMessageToFile("[IMDb video identifier] The Item " + item.Name + "was marked for removal from section.");
                    item.Tags["ToDelete"] = "true";
                    item.SaveTags();
                }
                return imdbid;
            }
            #endregion


            #endregion

            #region Save IMDbID to library
            imdbid = imdbOp.ImdbId;
            item.Tags["ImdbID"] = imdbid;
            item.SaveTags();
            Debugger.LogMessageToFile("[IMDb video identifier] IMDb ID after matching by Title: " + imdbid);
            #endregion


            return imdbid;
        }




        private static string IdentifyVideoByIMDbExternal(IMLItem item, string imdbid, ConnectionResult connectionresult)
        {

            //TODO: Improve external IMDb film search engine, add a Title matching engine to it.


            if (!connectionresult.InternetConnectionAvailable && Settings.ConnectionDiagnosticsEnabled)
            {
                Debugger.LogMessageToFile("[IMDb video identifier] Internet connection was not detected. Unable to identify this video by Title using the external (web-based) engine.");
                return imdbid;
            }

            if (!String.IsNullOrEmpty(imdbid))
            {
                Debugger.LogMessageToFile("[IMDb video identifier] IMDbID is already availabe for this title. Will not need to use the video identifier by Title using the external (web-based) engine.");
                return imdbid;
            }


            #region return if item is not listed
            if (Helpers.GetTagValueFromItem(item, "NotListed") == "true")
            {
                Debugger.LogMessageToFile("[IMDb video identifier] This Item is not listed on IMDb. Returning...");
                return imdbid;
            }

            #endregion


            Debugger.LogMessageToFile("[IMDb video identifier] Continuing to identify by IMDb using external web API...");

            #region Find imdbid using item's name or Title

            #region ...extract the film's title or year from library fields

            string year = Helpers.GetTagValueFromItem(item, "Year");
            string title = Helpers.GetTagValueFromItem(item, "Title");

            if (String.IsNullOrEmpty(title))
                title = item.Name;

            string titleNormalized = title.Replace(" ", "+");


            Debugger.LogMessageToFile("[IMDb video identifier] Video's Title retrieved from item's tags: " + title);
            Debugger.LogMessageToFile("[IMDb video identifier] Video's Year retrieved from item's tags: " + year);

            #endregion

            string searchString;

            if (String.IsNullOrEmpty(year))
                searchString = "http://www.deanclatworthy.com/imdb/?q=" + titleNormalized;
            else searchString = "http://www.deanclatworthy.com/imdb/?q=" + titleNormalized + "&year=" + year;

            Helpers.UpdateProgress("Updating Movies section...", "Trying to dentifying film by Title (using external web engine)... ", item);

            Stopwatch watch = new Stopwatch();
            watch.Start();
            imdbid = Downloaders.MineWebDataReturnFirstMatch(searchString, @"""imdburl"":""http:\\/\\/www.imdb.com\\/title\\/(?<IMDbID>.*?)\\/""");

            long millisecondsnow = watch.ElapsedMilliseconds;
          
            
            //MessageBox.Show
            //    (@"External search needed " 
            //    + millisecondsnow 
            //    + @" ms.");
            

            if (!String.IsNullOrEmpty(imdbid))
            {
                Debugger.LogMessageToFile("[IMDb video identifier] IMDb identification by Title (using external engine) was succesful!");
                Helpers.UpdateProgress("Updating Movies section...", "IMDb identification by Title (external engine) was succesful!", item);
                Thread.Sleep(2000);
            }

            #endregion

            #region Save IMDbID to library
            item.Tags["ImdbID"] = imdbid;
            item.SaveTags();
            Debugger.LogMessageToFile("[IMDb video identifier] IMDb ID after matching by Title: " + imdbid);
            #endregion


            return imdbid;
        }







    }
}
