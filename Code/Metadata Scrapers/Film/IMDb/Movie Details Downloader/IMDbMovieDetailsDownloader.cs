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
using System.Net;
using MediaFairy.ImportingEngine;

#if USE_MEEDIO 
using Meedio; 
#elif USE_MEEDIOS

#endif



namespace MediaFairy.IMDb
{




    internal static class IMDbMovieDetailsDownloader
    {



        //TODO: Remove Debugger logs for all movie details.
        //TODO: Add indivindual settings for each extra film detail type. 


        internal static IIMDbMovie DownloadMovieDetailsFromIMDb
            (string imdbID, bool showProgress )
        {


            var movie = new IMDbMovie();


            var filmDetails = new IMDbFilmDetails();
            var webClient = new WebClient();

            try
            {

                var movieUrl = IMDbMovieDetailsDownloaderHelpers
                    .ConstructMovieUrl(imdbID);


                var html = IMDbMovieDetailsDownloaderHelpers
                    .DownloadBaseTitleHtml
                    (movieUrl, webClient);


                if (IMDbMovieDetailsDownloaderHelpers
                    .CheckForValidHtmlData(html))
                    return null;


                //TODO: IMDb Movie Details Extraction does not work if the main html page trimming is activated. The html trimming regex should be fixed.
             
                //string trimmedHTML = CleanHtmlPage(html);

                //if (String.IsNullOrEmpty(trimmedHTML))
                //    trimmedHTML = html;

                string trimmedHtml = html;


                movie.Title
                    = IMDbMovieDetailsDownloaderHelpers
                        .ExtractFilmTitle
                        (imdbID, trimmedHtml);
                 


                movie = ExtractDetails
                    (showProgress, trimmedHtml,
                    movieUrl, movie, filmDetails);


            }
            catch (Exception e)
            {
                Debugger.LogMessageToFile
                    ("The GetMovieInfo() method returned an exception: "
                    + Environment.NewLine + e );
            }

            return movie;
        }



        private static IMDbMovie ExtractDetails
            (bool showProgress,
            string trimmedHtml,
            string movieUrl,
            IMDbMovie movie,
            IMDbFilmDetails filmDetails)
        {


            if (showProgress)
            {
                MainImportingEngine.ThisProgress.Progress
                    (MainImportingEngine.CurrentProgress,
                     "Downloading details from IMDb for '"
                     + movie.Title + "'...");
            }


            movie = ExtractDetailsFromMainPage
                (filmDetails, trimmedHtml, movie);


            string creditsUrl = movieUrl + "fullcredits";
            string longOverviewUrl = movieUrl + "plotsummary";
            string goofUrl = movieUrl + "goofs";
            string triviaUrl = movieUrl + "trivia";
            string quotesUrl = movieUrl + "quotes";


            movie = IMDbMovieDetailsDownloaderHelpers
                .ExtractDetailsFromAdditionalPages
                (showProgress, triviaUrl, goofUrl, quotesUrl,
                 longOverviewUrl, filmDetails, movie, creditsUrl);


            return movie;
        
        
        }





        private static IMDbMovie ExtractDetailsFromMainPage
            (IMDbFilmDetails filmDetails,
             string trimmedHtml, IMDbMovie movie)
        {

            IMDbRegEx imDbRegEx = new IMDbRegEx();
            imDbRegEx.SetRegExPatterns();


            IMDbFilmDetails.GetProductionYear(movie, trimmedHtml, imDbRegEx);


            IMDbFilmDetails.GetReleaseDate(movie, trimmedHtml, imDbRegEx);


            IMDbFilmDetails.GetTagline(movie, trimmedHtml, imDbRegEx);


            IMDbFilmDetails.GetRuntime(movie, trimmedHtml, imDbRegEx);


            IMDbFilmDetails.GetRating(movie, trimmedHtml, imDbRegEx);


            IMDbFilmDetails.ExtractRatingDescription(movie, trimmedHtml, imDbRegEx);


            IMDbFilmDetails.GetReview(movie, trimmedHtml, imDbRegEx);


            IMDbFilmDetails.GetStudio(movie, trimmedHtml, imDbRegEx);


            IMDbFilmDetails.GetOverview(movie, trimmedHtml, imDbRegEx);


            filmDetails.GetDirectors(ref movie, trimmedHtml);


            filmDetails.GetWriters(ref movie, trimmedHtml);


            filmDetails.GetGenres(ref movie, trimmedHtml);




            return movie;


        }


        private static string CleanHtmlPage(string html)
        {
            IMDbRegEx imDbRegEx = new IMDbRegEx();
            imDbRegEx.SetRegExPatterns();

            Debugger.LogMessageToFile
                ("Cleaning IMDb result html ");


            string trimmedHtml = imDbRegEx.GetRegExString
                (html, imDbRegEx.TrimmedHTMLpattern);

            return trimmedHtml;
        }



    }





}
