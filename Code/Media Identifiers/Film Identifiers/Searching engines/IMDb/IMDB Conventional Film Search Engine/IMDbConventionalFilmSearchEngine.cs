//'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
//'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
//''    MediaFairy                                                               ''
//''    Copyright (C) 2008-2013  Stavros Skamagkis                               ''
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
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;
using MeediFier.Code.Metadata_Scrapers.Film.IMDb.Searching_engines.IMDB_Conventional_Film_Search_Engine;


namespace MeediFier.IMDb
{


    static class IMDbConventionalFilmSearchEngine
    {


        public static IList<IIMDbSearchResult> SearchForTitleReturnResults(string title)
        {



            #region Vars
            

            Debugger.LogMessageToFile
                ("[IMDb Conventional Film Search Engine] " +
                 "Initializing variables...");
            
            var searchResults = new List<IIMDbSearchResult>();
            

            var imDbRegex = new IMDbRegEx(); 
            //TODO: Test if SetPatterns() is correctly called and regexes are succesfully loaded!
            

            string titleEncoded = ToolBox.Utils.WebUtils.EncodeURL(title);
            
            Debugger.LogMessageToFile
                ("[IMDb Conventional Film Search Engine] " +
                 "Title to search for: " + title);
            
            Debugger.LogMessageToFile
                ("[IMDb Conventional Film Search Engine] " +
                 "Encoded title: " + titleEncoded);
            

            string SearchURL = "http://www.imdb.com/find?s=title&q=" + titleEncoded;
            //string SearchURL = "http://m.imdb.com/find?q=" + titleEncoded + "&button=Search";
            
            Debugger.LogMessageToFile
                ("[IMDb Conventional Film Search Engine] " +
                 "IMDb search URL: " + SearchURL);
            
            #endregion



            var watch = new Stopwatch();
            watch.Start();


            long millisecondsnow;



            string searchResultsHtml
                = DownloadSearchResultsHtml
                (watch, SearchURL);
            

            if ( String.IsNullOrEmpty
                ( searchResultsHtml ))
                return searchResults;


            var multipleResultsFound 
                = IMDbConventionalFilmSearchEngineHelpers.DretectMultipleResults
                (searchResultsHtml);


            searchResults = AddSingleOrMultipleMatchesToSearchResults
                (imDbRegex, searchResultsHtml, 
                 multipleResultsFound, searchResults);



            //MessageBox.Show("Results: " + results.Count.ToString() );            

            millisecondsnow = watch.ElapsedMilliseconds;

            //MessageBox.Show
            //    ("Extraction of all" +
            //     " results needed " 
            //     + millisecondsnow 
            //     + " ms");

            return searchResults;
        }



        private static string DownloadSearchResultsHtml
            (Stopwatch watch, string searchURL)
        {

            string searchResultsHtml = String.Empty;

            try
            {
                Debugger.LogMessageToFile
                    ("[IMDb Conventional Film Search Engine] " +
                     "Trying to get search results data from IMDb...");

                searchResultsHtml = Downloaders.GetUrl
                    (searchURL, String.Empty, false);
            }
            catch
            {
                Debugger.LogMessageToFile
                    ("[IMDb Conventional Film Search Engine] " +
                     "Communication with IMDb failed. Retrying... (1)");

                Helpers.UpdateProgress
                    ("Updating Movies Section...",
                     "Communication with IMDb failed. Retrying... (1)", null);

                //Thread.Sleep(2000);

                try
                {
                    searchResultsHtml = Downloaders.GetUrl
                        (searchURL, String.Empty, false);
                }
                catch
                {
                    Debugger.LogMessageToFile
                        ("[IMDb Conventional Film Search Engine] " +
                         "Communication with IMDb failed. Retrying... (2)");

                    Helpers.UpdateProgress
                        ("Updating Movies Section...",
                         "Communication with IMDb failed. Retrying... (2)", null);
                    //Thread.Sleep(2000);

                    try
                    {
                        searchResultsHtml = Downloaders.GetUrl
                            (searchURL, String.Empty, false);
                    }
                    catch
                    {
                        Debugger.LogMessageToFile
                            ("[IMDb Conventional Film Search Engine] " +
                             "Communication with IMDb failed. " +
                             "Unable to perform search for this title. Returning...");

                        Helpers.UpdateProgress
                            ("Updating Movies Section...",
                             "Communication with IMDb failed. " +
                             "Unable to perform search for this title.", null);

                        StatusForm.statusForm.TrayIcon.ShowBalloonTip
                            (5000, "Communication with IMDb failed.",
                             "MediaFairy's internal IMDb film title matching engine " +
                             "was unable to communicate with Internet Movie Database (IMDb). " +
                             "If the problem persists, please review your internet connection availability, " +
                             "otherwise the server may be temporarily offline or unreachable.", ToolTipIcon.Warning);

                        Thread.Sleep(5000);

                        return searchResultsHtml;

                    } //try 3
                } //try 2
            } //try 1


            long millisecondsnow = watch.ElapsedMilliseconds;


            //MessageBox.Show
            //    ("Unoptimized HTML fetch needed " 
            //    + millisecondsnow + " ms.");


            watch.Reset();

            Debugger.LogMessageToFile
                ("[IMDb Conventional Film Search Engine] " +
                 "Search results data were obtained sucessfully.");

            return searchResultsHtml;

        }


        private static List<IIMDbSearchResult> 
            AddSingleOrMultipleMatchesToSearchResults
            (IMDbRegEx imDbRegex, string html,
             bool multipleResultsFound, 
             List<IIMDbSearchResult> results)
        {


            if (!multipleResultsFound)
            {
                Debugger.LogMessageToFile
                    ("[IMDb Conventional Film Search Engine] " +
                     "Multiple results were not found.");

                results = IMDbConventionalFilmSearchEngineHelpers.SingleMatchAddToSearchResults
                    (html, imDbRegex, results);
            }
            else
            {
                Debugger.LogMessageToFile
                    ("[IMDb Conventional Film Search Engine] " +
                     "Multiple results were found.");

                #region Result Pattern 1

                Match match = imDbRegex.GetRegExMatch(html, imDbRegex.MovieResultPattern1);

                results = MultipleMatchesAddToSearchResults(match, imDbRegex, results);

                #endregion

                #region Result Pattern 2

                match = imDbRegex.GetRegExMatch(html, imDbRegex.MovieResultPattern2);

                results = MultipleMatchesAddToSearchResults(match, imDbRegex, results);

                #endregion
            }

            return results;
        }




        private static List<IIMDbSearchResult> MultipleMatchesAddToSearchResults
            (Match match, IMDbRegEx imDbRegex, List<IIMDbSearchResult> results)
        {

            while (match != null && match.Length > 0)
            {


                var result = IMDbConventionalFilmSearchEngineHelpers
                    .MultipleMatchesMineDetailsOfSingleFilmResult(match, imDbRegex);


                IMDbConventionalFilmSearchEngineHelpers
                    .IgnoreIrrelevantResults
                    (match, imDbRegex, result);


                results.Add(result);
                
                Debugger.LogMessageToFile
                    ("[IMDb Conventional Film Search Engine] " +
                     "Result was added to list.");
                
                match = match.NextMatch();
                
                Debugger.LogMessageToFile
                    ("[IMDb Conventional Film Search Engine]" +
                     " Proceeding to next result...");
            
            }




          return results;
        }
    }//endof class




}//endof namespace
