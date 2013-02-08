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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;
using Debugger = MediaFairy.Debugger;


namespace MediaFairy.IMDb
{
    static class IMDbConventionalFilmSearchEngine
    {


        public static IList<IIMDbSearchResult> SearchForTitleReturnResults(string title)
        {

            Stopwatch watch = new Stopwatch();
            watch.Start();

            #region Vars
            Debugger.LogMessageToFile("[IMDb Conventional Film Search Engine] Initializing variables...");
            List<IIMDbSearchResult> results = new List<IIMDbSearchResult>();
            IMDbRegEx _IMDbRegex = new IMDbRegEx(); //TODO: Test if SetPatterns() is correctly called and regexes are succesfully loaded!
            string titleEncoded = JCUtils.WebUtils.EncodeURLstring(title);
            Debugger.LogMessageToFile("[IMDb Conventional Film Search Engine] Title to search for: " + title);
            Debugger.LogMessageToFile("[IMDb Conventional Film Search Engine] Encoded title: " + titleEncoded);
            
            string SearchURL = "http://www.imdb.com/find?s=title&q=" + titleEncoded;
            //string SearchURL = "http://m.imdb.com/find?q=" + titleEncoded + "&button=Search";
            
            Debugger.LogMessageToFile("[IMDb Conventional Film Search Engine] IMDb search URL: " + SearchURL);
            string html = String.Empty;
            #endregion

            #region Get results html
            try
            {
                Debugger.LogMessageToFile("[IMDb Conventional Film Search Engine] Trying to get search results data from IMDb...");
                html = JCUtils.WebUtils.GET(SearchURL);
            }
            catch
            {
                Debugger.LogMessageToFile("[IMDb Conventional Film Search Engine] Communication with IMDb failed. Retrying... (1)");
                Helpers.UpdateProgress("Updating Movies Section...", "Communication with IMDb failed. Retrying... (1)", null);
                //Thread.Sleep(2000);

                try
                {
                    html = JCUtils.WebUtils.GET(SearchURL);
                }
                catch
                {
                    Debugger.LogMessageToFile("[IMDb Conventional Film Search Engine] Communication with IMDb failed. Retrying... (2)");
                    Helpers.UpdateProgress("Updating Movies Section...", "Communication with IMDb failed. Retrying... (2)", null);
                    //Thread.Sleep(2000);
                    try
                    {
                        html = JCUtils.WebUtils.GET(SearchURL);
                    }
                    catch
                    {
                        Debugger.LogMessageToFile("[IMDb Conventional Film Search Engine] Communication with IMDb failed. Unable to perform search for this title. Returning...");
                        Helpers.UpdateProgress("Updating Movies Section...", "Communication with IMDb failed. Unable to perform search for this title.", null);
                        StatusForm.statusForm.TrayIcon.ShowBalloonTip(5000, "Communication with IMDb failed.", "MediaFairy's internal IMDb film title matching engine was unable to communicate with Internet Movie Database (IMDb). If the problem persists, please review your internet connection availability, otherwise the server may be temporarily offline or unreachable.", ToolTipIcon.Warning);
                        Thread.Sleep(5000);
                        return results;
                    }//try 3

                }//try 2


            }//try 1

            long millisecondsnow = watch.ElapsedMilliseconds;


            //MessageBox.Show
            //    ("Unoptimized HTML fetch needed " 
            //    + millisecondsnow + " ms.");
     
            
            watch.Reset();

            Debugger.LogMessageToFile("[IMDb Conventional Film Search Engine] Search results data were obtained sucessfully.");
            #endregion

            #region Detect if multiple results were found
            bool multipleResultsFound = true;
            //TODO: Is the first results extraction method ever used?!
            if (html.ToLower().IndexOf("<b>popular titles") < 0
                && html.ToLower().IndexOf("<b>titles") < 0)
                multipleResultsFound = false;
            #endregion

            if (!multipleResultsFound)
            {

                Debugger.LogMessageToFile("[IMDb Conventional Film Search Engine] Multiple results were not found.");

                results = SingleMatchAddToSearchResults(html, _IMDbRegex, results);


            }
            else
            {

                Debugger.LogMessageToFile("[IMDb Conventional Film Search Engine] Multiple results were found.");

                #region Result Pattern 1
                Match match = _IMDbRegex.GetRegExMatch(html, _IMDbRegex.MovieResultPattern1);

                results = AddMatchesToSearchResults(match, _IMDbRegex, results);
                #endregion

                #region Result Pattern 2
                match = _IMDbRegex.GetRegExMatch(html, _IMDbRegex.MovieResultPattern2);

                results = AddMatchesToSearchResults(match, _IMDbRegex, results);
                #endregion

            }

            //MessageBox.Show("Results: " + results.Count.ToString() );            

            millisecondsnow = watch.ElapsedMilliseconds;

            //MessageBox.Show
            //    ("Extraction of all" +
            //     " results needed " 
            //     + millisecondsnow 
            //     + " ms");

            return results;
        }




        private static List<IIMDbSearchResult> AddMatchesToSearchResults(Match match, IMDbRegEx _IMDbRegex, List<IIMDbSearchResult> results)
        {

            while (match != null && match.Length > 0)
            {

                #region Result's details
                Debugger.LogMessageToFile("[IMDb Conventional Film Search Engine] New search result was found. Proceeding to add result to the list of total search results...");
                IMDbSearchResult result = new IMDbSearchResult();

                result.IMDb_ID = _IMDbRegex.GetMatchValue(match, "IMDbID", true);
                Debugger.LogMessageToFile("[IMDb Conventional Film Search Engine] Result IMDb ID: " + result.IMDb_ID);

                result.Title = _IMDbRegex.GetMatchValue(match, "Title", true); 
                Debugger.LogMessageToFile("[IMDb Conventional Film Search Engine] Result Title: " + result.Title);

                result.Year = _IMDbRegex.GetMatchValue(match, "Year", true);
                Debugger.LogMessageToFile("[IMDb Conventional Film Search Engine] Result Year: " + result.Year);

                string URL = _IMDbRegex.GetMatchValue(match, "URL", true);
                result.URL = "http://www.imdb.com" + URL;
                Debugger.LogMessageToFile("[IMDb Conventional Film Search Engine] Result URL: " + result.URL);
                #endregion

                //TODO: Also ignore "Making Of" titles.
                //TODO: Also ignore documentaries (option).
                string extra = _IMDbRegex.GetMatchValue(match, "Extra", true);
                if (extra.ToLower().Trim().Contains("(vg)"))
                {
                    Debugger.LogMessageToFile("[IMDb Conventional Film Search Engine] This result's 'Extra' tag contains the '(vg)' keyword. Result will be ignored.");
                    result.Ignore = true;
                }

                results.Add(result);
                Debugger.LogMessageToFile("[IMDb Conventional Film Search Engine] Result was added to list.");
                match = match.NextMatch();
                Debugger.LogMessageToFile("[IMDb Conventional Film Search Engine] Proceeding to next result...");
            }



          return results;
        }



        private static List<IIMDbSearchResult> SingleMatchAddToSearchResults(string html, IMDbRegEx _IMDbRegex, List<IIMDbSearchResult> results)
        {

            #region Find title index
            int titleIndex = 0;
            titleIndex = html.ToLower().IndexOf("<title>");
            bool titleIndexFound = false;

            if (titleIndex > -1)
            {
                titleIndexFound = true;
                Debugger.LogMessageToFile("[IMDb Conventional Film Search Engine] Title index was found.");
            }
            else Debugger.LogMessageToFile("[IMDb Conventional Film Search Engine] Title index was not found. Returning...");
            #endregion

            if (!titleIndexFound)
                return results;

            Debugger.LogMessageToFile("[IMDb Conventional Film Search Engine] A single result was found.");

            //TODO: Are these RegExes the same as the 'movie result' ones?
            string TitleAndYearPattern = @"<title>(?<Title>.*?)\((?<Year>.*?)\)</title>";
            string ImdbIdPattern = @"<a\shref=""/title/(?<IMDbID>[^/]*)/fullcredits""";
            Match match = _IMDbRegex.GetRegExMatch(html, TitleAndYearPattern);

            #region Return if no matches were found
            if (match == null || match.Length == 0)
                return results;
            #endregion

            IMDbSearchResult result = new IMDbSearchResult();

            #region Get Title and Year
            result.Title = _IMDbRegex.GetMatchValue(match, "Title", true);
            result.Year = _IMDbRegex.GetMatchValue(match, "Year", true);
            #endregion

            #region Get IMDb ID
            match = _IMDbRegex.GetRegExMatch(html, ImdbIdPattern);

            if (match != null && match.Length > 0)
                result.IMDb_ID = _IMDbRegex.GetMatchValue(match, "IMDbID", true);
            #endregion

            #region Add to results
            if (result.IMDb_ID.Trim() != String.Empty
                && result.Title.Trim() != String.Empty)
                results.Add(result);
            #endregion


            return results;
        }









    }//endof class


}//endof namespace
