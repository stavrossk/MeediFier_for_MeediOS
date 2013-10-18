using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    }
}
