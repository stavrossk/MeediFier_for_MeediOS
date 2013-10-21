using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MeediFier.IMDb;

namespace MeediFier.Code.Metadata_Scrapers.Film.IMDb.Searching_engines.IMDB_Conventional_Film_Search_Engine
{
    class IMDbConventionalFilmSearchEngineHelpers
    {
        internal static void IgnoreIrrelevantResults
            (Match match, IMDbRegEx imDbRegex, IMDbSearchResult result)
        {

            //TODO: Also ignore "Making Of" titles.
            //TODO: Also ignore documentaries (option).

            string extra = imDbRegex.GetMatchValue
                (match, "Extra", true);


            IgnoreVideoGameResult(result, extra);

        }

        internal static void IgnoreVideoGameResult
            (IIMDbSearchResult result, string extra)
        {

            if (!extra.ToLower().Trim().Contains("(vg)")) 
                return;
            
            Debugger.LogMessageToFile
                ("[IMDb Conventional Film Search Engine] " +
                 "This result's 'Extra' tag contains the '(vg)' keyword." +
                 " Result will be ignored.");
            
            result.Ignore = true;
        
        }

        internal static bool LocateTitleIndex(string html)
        {
            int titleIndex = 0;

            titleIndex = html.ToLower().IndexOf("<title>", StringComparison.Ordinal);
            bool titleIndexFound = false;

            if (titleIndex > -1)
            {
                titleIndexFound = true;
                Debugger.LogMessageToFile("[IMDb Conventional Film Search Engine] Title index was found.");
            }
            else Debugger.LogMessageToFile("[IMDb Conventional Film Search Engine] Title index was not found. Returning...");

            return titleIndexFound;
        }

        internal static List<IIMDbSearchResult> SingleMatchAddToSearchResults
            (string html, IMDbRegEx imDbRegex, List<IIMDbSearchResult> results)
        {


            var titleIndexFound = LocateTitleIndex(html);


            if (!titleIndexFound)
                return results;

            Debugger.LogMessageToFile
                ("[IMDb Conventional Film Search Engine] " +
                 "A single result was found.");


            //TODO: Are these RegExes the same as the 'movie result' ones?
            const string titleAndYearPattern = @"<title>(?<Title>.*?)\((?<Year>.*?)\)</title>";
            

            const string ImdbIdPattern = @"<a\shref=""/title/(?<IMDbID>[^/]*)/fullcredits""";
            
            Match match = imDbRegex.GetRegExMatch(html, titleAndYearPattern);

            #region Return if no matches were found
            if (match == null || match.Length == 0)
                return results;
            #endregion


            var result = new IMDbSearchResult
                             {
                                 Title = imDbRegex.GetMatchValue(match, "Title", true),
                                 Year = imDbRegex.GetMatchValue(match, "Year", true)
                             };



            MineImdbIdFromSingleFilmMatch
                (html, imDbRegex, ImdbIdPattern, result);

            AddSingleFilmResultToSearchResults
                (results, result);


            return results;
        }

        private static void AddSingleFilmResultToSearchResults
            (ICollection<IIMDbSearchResult> results, IIMDbSearchResult result)
        {
            if (result.IMDb_ID.Trim() != String.Empty
                && result.Title.Trim() != String.Empty)
                results.Add(result);
        }

        internal static bool DretectMultipleResults(string html)
        {
            #region Detect if multiple results were found

            bool multipleResultsFound
                = !(html.ToLower().IndexOf("<b>popular titles", StringComparison.Ordinal) < 0
                    && html.ToLower().IndexOf("<b>titles", StringComparison.Ordinal) < 0);

            //TODO: Is the first results extraction method ever used?!

            #endregion

            return multipleResultsFound;
        }

        internal static void MineImdbIdFromSingleFilmMatch
            (string html, IMDbRegEx imDbRegex, 
             string imdbIdPattern, IIMDbSearchResult result)
        {
            Match match = imDbRegex.GetRegExMatch(html, imdbIdPattern);

            if (match != null && match.Length > 0)
                result.IMDb_ID =
                    imDbRegex.GetMatchValue
                        (match, "IMDbID", true);
        }

        internal static IMDbSearchResult MultipleMatchesMineDetailsOfSingleFilmResult
            (Match match, IMDbRegEx imDbRegex)
        {


            #region Result's details

            Debugger.LogMessageToFile(
                "[IMDb Conventional Film Search Engine] " +
                "New search result was found. " +
                "Proceeding to add result to the list of total search results...");

            var result = new IMDbSearchResult();


            result.IMDb_ID = imDbRegex.GetMatchValue
                (match, "IMDbID", true);

            Debugger.LogMessageToFile
                ("[IMDb Conventional Film Search Engine] " +
                 "Result IMDb ID: " + result.IMDb_ID);

            result.Title = imDbRegex.GetMatchValue
                (match, "Title", true);

            Debugger.LogMessageToFile
                ("[IMDb Conventional Film Search Engine] " +
                 "Result Title: " + result.Title);

            result.Year = imDbRegex.GetMatchValue
                (match, "Year", true);

            Debugger.LogMessageToFile
                ("[IMDb Conventional Film Search Engine] " +
                 "Result Year: " + result.Year);

            string URL = imDbRegex.GetMatchValue
                (match, "URL", true);

            result.URL = "http://www.imdb.com" + URL;

            Debugger.LogMessageToFile
                ("[IMDb Conventional Film Search Engine] " +
                 "Result URL: " + result.URL);

            #endregion

            return result;
        }
    }
}
