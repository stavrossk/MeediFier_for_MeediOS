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
using System.Text.RegularExpressions;
using MeediFier.Code.Downloaders;
using MeediFier.Code.Settings;

namespace MeediFier.IMDb
{
    class IMDbFilmDetails
    {

        private readonly IMDbRegEx _imDbRegex = new IMDbRegEx();


        public void GetDirectors(ref IMDbMovie movie, string html)
        {


            if (!ImdbFilmDetailsIndividualChoices
                .GetIMDbMovieDirectors)
                return;


            Debugger.LogMessageToFile
                ("[IMDb film details downloader]" +
                 " Extracting Directors...");



            string directorHtml
                = _imDbRegex.GetRegExString
                (html, _imDbRegex.DirectorPattern);


            if (!String.IsNullOrEmpty(directorHtml.Trim()))
            {

                Match match = _imDbRegex.GetRegExMatch(directorHtml, _imDbRegex.PersonPattern);
                while (match != null && match.Length > 0)
                {
                    string personID = _imDbRegex.GetMatchValue(match, "PersonURL", true);

                    if (String.IsNullOrEmpty(personID.Trim()))
                        continue;

                    IIMDbPerson director = movie.People.GetPersonByID(personID);

                    if (director == null)
                    {
                        director = new IMDbPerson();
                        movie.People.Add(director);
                    }

                    director.URL = personID;
                    director.Name = _imDbRegex.GetMatchValue(match, "PersonName", true);
                    director.IsDirector = true;

                    match = match.NextMatch();
                }
            }

            string directors = movie.People.GetDirectorString();


            Debugger.LogMessageToFile
            ("[IMDb film details downloader] " +
            " IMDb returned Directors: " + directors);

            //MessageBox.Show(@"IMDb returned Directors: " + directors);


        }






        public void GetWriters(ref IMDbMovie movie, string html)
        {


            if (!ImdbFilmDetailsIndividualChoices.GetIMDbMovieWriters)
                return;

            Debugger.LogMessageToFile("[IMDb film details downloader] Extracting Writers...");

            // Grab Writers
            string writerHTML = _imDbRegex.GetRegExString(html, _imDbRegex.WriterPattern);

            if (!String.IsNullOrEmpty(writerHTML.Trim()))
            {
                Match match = _imDbRegex.GetRegExMatch(writerHTML, _imDbRegex.PersonPattern);
                while (match != null && match.Length > 0)
                {
                    string personID = _imDbRegex.GetMatchValue(match, "PersonURL", true);

                    if (String.IsNullOrEmpty(personID.Trim()))
                        continue;
                    
                    IIMDbPerson writer = movie.People.GetPersonByID(personID);
                    
                    if (writer == null)
                    {
                        writer = new IMDbPerson();
                        movie.People.Add(writer);
                    }
                    
                    writer.URL = personID;
                    writer.Name = _imDbRegex.GetMatchValue(match, "PersonName", true);
                    writer.IsWriter = true;

                    match = match.NextMatch();
                }

            }

            string writers = movie.People.GetWriterString();

            Debugger.LogMessageToFile("[IMDb film details downloader]  IMDb returned Writers: " + writers);
            //MessageBox.Show(@"IMDb returned Writers: " + writers);

        }



        public void GetActors(ref IMDbMovie movie, string creditsUrl)
        {


            if (!ImdbFilmDetailsIndividualChoices.GetIMDbMovieActors)
                return;

            // get cast html
            string html = HtmlDownloaders.DownloadHTMLfromURL(creditsUrl);

            // Grab Actors
            string trimmedHtml = _imDbRegex.GetRegExString(html, _imDbRegex.CastPattern);
            
            if (String.IsNullOrEmpty(trimmedHtml.Trim()))
                return;
            
            Match match1 = _imDbRegex.GetRegExMatch(trimmedHtml, _imDbRegex.CastPersonPatternWithCharacterUrLs);
            
            while (match1 != null && match1.Length > 0)
            {

                string personID = _imDbRegex.GetMatchValue(match1, "PersonURL", true);


                if (String.IsNullOrEmpty(personID.Trim())) 
                    continue;
                
                IIMDbPerson actor = movie.People.GetPersonByID(personID);
                
                if (actor == null)
                {
                    actor = new IMDbPerson();
                    movie.People.Add(actor);
                }
                
                
                actor.URL = personID;
                actor.Name = _imDbRegex.GetMatchValue(match1, "PersonName", true);
                actor.IsActor = true;
               
                string characterString = _imDbRegex.GetMatchValue(match1, "CharacterName", true);
                
                ParseCharacters(ref actor, characterString);

                match1 = match1.NextMatch();
            
            }

            Match match2 = _imDbRegex.GetRegExMatch
                (trimmedHtml, _imDbRegex.CastPersonPatternWithNoCharacterUrLs);
            
            while (match2 != null && match2.Length > 0)
            {

                string personID = _imDbRegex.GetMatchValue(match2, "PersonURL", true);

                if (String.IsNullOrEmpty(personID.Trim()))
                    continue;

                IIMDbPerson actor = movie.People.GetPersonByID(personID);

                if (actor == null)
                {
                    actor = new IMDbPerson();
                    movie.People.Add(actor);
                }
                
                actor.URL = personID;
                actor.Name = _imDbRegex.GetMatchValue(match2, "PersonName", true);
                actor.IsActor = true;
                
                string characterString =
                    _imDbRegex.GetMatchValue
                    (match2, "CharacterName", true);
                
                ParseCharacters
                    (ref actor, characterString);

                match2 = match2.NextMatch();

            }


        }






        public void GetGenres(ref IMDbMovie movie, string html)
        {


            if (!ImdbFilmDetailsIndividualChoices.GetIMDbMovieGenres)
                return;

            Debugger.LogMessageToFile("[IMDb film details downloader] Extracting Genres...");

            // Grab Genres
            Match match = _imDbRegex.GetRegExMatch(html, _imDbRegex.GenrePattern);
            if (match != null && match.Length > 0)
            {
                match = _imDbRegex.GetRegExMatch(match.Value, _imDbRegex.Genre2Pattern);
                while (match != null && match.Length > 0)
                {
                    string genre = _imDbRegex.GetMatchValue(match, "Genre", true);

                    if (!String.IsNullOrEmpty(genre.Trim()))
                        movie.Genres.Add(genre);
                    
                    match = match.NextMatch();
                
                }
            }

            string genres = movie.GetGenresString();

            Debugger.LogMessageToFile("[IMDb film details downloader]  IMDb returned Genres: " + genres);

            //MessageBox.Show(@"IMDb returned Genres: " + genres);

        }



        public void GetLongOverview(ref IMDbMovie movie, string longOverviewUrl)
        {


            if (!ImdbFilmDetailsIndividualChoices.GetIMDbMovieLongOverview)
                return;

            Debugger.LogMessageToFile("[IMDb film details downloader] Extracting Plot summary...");

            // get long overview html
            string html = HtmlDownloaders.DownloadHTMLfromURL(longOverviewUrl);

            // Grab Long Overview
            Match match = _imDbRegex.GetRegExMatch(html, _imDbRegex.FilmStorylinePattern);
            movie.OverviewLong = _imDbRegex.GetMatchValue(match, "LongOverview", true);

            Debugger.LogMessageToFile("[IMDb film details downloader]  IMDb returned Plot (LongOverview) : " + movie.OverviewLong);
            //MessageBox.Show("IMDb returned Plot Summary: " + movie.OverviewLong);
        }



        public void GetGoofs(ref IMDbMovie movie, string goofUrl)
        {


            if (!ImdbFilmDetailsIndividualChoices.GetIMDbMovieGoofs)
                return;

            Debugger.LogMessageToFile("[IMDb film details downloader] Extracting Goofs...");

            // get goof html
            string html = HtmlDownloaders.DownloadHTMLfromURL(goofUrl);

            // Grab goofs
            Match match = _imDbRegex.GetRegExMatch(html, _imDbRegex.GoofPattern);
            while (match != null && match.Length > 0)
            {
                IMDbGoof goof = new IMDbGoof();
                goof.Category = _imDbRegex.GetMatchValue(match, "Category", true);
                goof.Description = _imDbRegex.GetMatchValue(match, "Goof", true);
                movie.Goofs.Add(goof);
                match = match.NextMatch();
            }

            string goofs = movie.GetGoofsString();

            //Debugger.LogMessageToFile("[IMDb film details downloader]  IMDb returned Goofs: " + goofs);
            //MessageBox.Show("IMDb returned Goofs: " + goofs);

        }



        public void GetTrivia(ref IMDbMovie movie, string triviaUrl)
        {


            if (!ImdbFilmDetailsIndividualChoices.GetIMDbMovieTrivia)
                return;

            Debugger.LogMessageToFile("[IMDb film details downloader] Extracting Trivia...");

            // get trivia html
            string html = HtmlDownloaders.DownloadHTMLfromURL(triviaUrl);

            // Grab trivia
            Match match = _imDbRegex.GetRegExMatch(html, _imDbRegex.TriviaPattern);

            while (match != null && match.Length > 0)
            {

                string trivia = _imDbRegex.GetMatchValue(match, "Trivia", true);

                if (!String.IsNullOrEmpty(trivia.Trim()))
                    movie.Trivia.Add(trivia.Trim());

                match = match.NextMatch();
            
            }


            string triviaTmp = movie.GetTriviaString();

            //Debugger.LogMessageToFile("[IMDb film details downloader]  IMDb returned Trivia: " + triviaTmp);
            //MessageBox.Show("IMDb returned Trivia: " + triviaTmp);

        }



        public void GetQuotes(ref IMDbMovie movie, string quotesUrl)
        {

            if (!ImdbFilmDetailsIndividualChoices.GetIMDbMovieQuotes)
                return;

            Debugger.LogMessageToFile("[IMDb film details downloader] Extracting Quotes...");

            // get quotes html
            string html = HtmlDownloaders.DownloadHTMLfromURL(quotesUrl);

            // Grab quotes
            Match match1 = _imDbRegex.GetRegExMatch(html, _imDbRegex.QuoteBlockPattern);
            while (match1 != null && match1.Length > 0)
            {

                string quoteBlockHTML = _imDbRegex.GetMatchValue(match1, "QuoteBlock", false);


                if (!String.IsNullOrEmpty(quoteBlockHTML.Trim()))
                {

                    List<IIMDbQuote> quoteBlock = new List<IIMDbQuote>();

                    Match match2 = _imDbRegex.GetRegExMatch(quoteBlockHTML, _imDbRegex.QuotePattern);

                    while (match2 != null && match2.Length > 0)
                    {

                        IMDbQuote quote = new IMDbQuote
                                              {
                                                  Character = _imDbRegex.GetMatchValue(match2, "Character", true),
                                                  Text = _imDbRegex.GetMatchValue(match2, "Quote", true)
                                              };

                        quoteBlock.Add(quote);
                        match2 = match2.NextMatch();
                    }

                    if (quoteBlock.Count > 0)
                        movie.Quotes.Add(quoteBlock);
                }
                match1 = match1.NextMatch();
            }

            string quotes = movie.GetQuotesString();
            //Debugger.LogMessageToFile("[IMDb film details downloader]  IMDb returned Quotes: " + quotes);
            //MessageBox.Show("IMDb returned Quotes: " + quotes);

        }


        private static void ParseCharacters(ref IIMDbPerson actor, string characterString)
        {

            if (characterString.Contains("/"))

                foreach (string character in characterString.Split(new char[] {'/'}))
                    actor.AddRole(character);

            else actor.AddRole(characterString);

        }


        internal static void GetOverview
            (IMDbMovie movie,
            string trimmedHTML,
            IMDbRegEx imDbRegEx )
        {


            if (!ImdbFilmDetailsIndividualChoices.GetIMDbMovieShortOverview)
                return;

            Debugger.LogMessageToFile
                ("[IMDb film details downloader]" +
                 " Extracting Short Overview...");
            

            Match match = imDbRegEx.GetRegExMatch
                (trimmedHTML, imDbRegEx.FilmDescriptionPattern);


            movie.OverviewShort =
                imDbRegEx.GetMatchValue
                (match, "ShortOverview", true);
            
            if (movie.OverviewShort.ToLower().EndsWith("more"))
                movie.OverviewShort =
                    movie.OverviewShort.Substring
                    (0, movie.OverviewShort.Length - 4).Trim();
            
            movie.OverviewShort = movie.OverviewShort.Trim() + "...";

            Debugger.LogMessageToFile("IMDb returned Overview: " + movie.OverviewShort);


        }


        internal static void ExtractRatingDescription
            (IMDbMovie movie,
            string trimmedHTML, IMDbRegEx imDbRegEx)
        {


            if (!ImdbFilmDetailsIndividualChoices.GetIMDbMovieRatingDescription)
                return;

            Debugger.LogMessageToFile("[IMDb film details downloader] Extracting Rating Description...");

            Match match = 
                imDbRegEx.GetRegExMatch
                (trimmedHTML, imDbRegEx.RatingDescriptionPattern);
         
            movie.RatingDescription = 
                imDbRegEx.GetMatchValue
                (match, "RatingDescription", true);

            Debugger.LogMessageToFile("[IMDb film details downloader]  IMDb returned Rating Description: " +
                                      movie.RatingDescription);


        }



        internal static void GetReview
            (IMDbMovie movie,
            string trimmedHTML,
            IMDbRegEx imDbRegEx)
        {


            if (!ImdbFilmDetailsIndividualChoices.GetIMDbMovieReviews)
                return;

            Debugger.LogMessageToFile("[IMDb film details downloader] Extracting Review score...");
          
            Match match = imDbRegEx.GetRegExMatch(trimmedHTML, imDbRegEx.ReviewPattern);
           
            movie.Review = imDbRegEx.GetMatchValue(match, "Review", true);
           
            if (movie.Review.Contains("/"))
                movie.Review = movie.Review.Substring(0, movie.Review.IndexOf("/", System.StringComparison.Ordinal));
          
            Debugger.LogMessageToFile("IMDb returned Review: " + movie.Review);

        }



        internal static void GetStudio
            (IMDbMovie movie,
            string trimmedHtml,
            IMDbRegEx imDbRegEx)
        {


            if (!ImdbFilmDetailsIndividualChoices
                .GetIMDbMovieProductionStudio)
                return;

            Debugger.LogMessageToFile
                ("[IMDb film details downloader] " +
                 "Extracting Studio...");


            Match match
                = imDbRegEx
                .GetRegExMatch
                (trimmedHtml, 
                imDbRegEx.StudioPattern);


            movie.Studio = imDbRegEx.GetMatchValue
                (match, "Studio", true);


            Debugger.LogMessageToFile
                ("[IMDb film details downloader] " +
                 "IMDb returned Studio: " 
                 + movie.Studio);


        }


        internal static void GetReleaseDate
            (IIMDbMovie movie,
            string trimmedHTML, IMDbRegEx imDbRegEx)
        {


            if (!ImdbFilmDetailsIndividualChoices.GetIMDbMovieReleaseDate)
                return;

            Debugger.LogMessageToFile("[IMDb film details downloader]" +
                                      " Extracting Release Date...");

            Match match = imDbRegEx.GetRegExMatch
                (trimmedHTML, imDbRegEx.ReleaseDatePattern);

            movie.Release_Date = imDbRegEx.GetMatchValue
                (match, "ReleaseDate", true);

            Debugger.LogMessageToFile("[IMDb film details downloader]" +
                                      " IMDb returned Release Date: " + movie.Release_Date);

            //MessageBox.Show("IMDb returned Release Date: " + movie.Release_Date);

        }





        internal static void GetProductionYear
            (IIMDbMovie movie, string trimmedHTML, IMDbRegEx imDbRegEx)
        {


            Debugger.LogMessageToFile("[IMDb film details downloader]" +
                                      " Extracting Year...");

            Match match = imDbRegEx.GetRegExMatch
                (trimmedHTML, imDbRegEx.YearPattern);

            movie.Year = imDbRegEx.GetMatchValue
                (match, "Year", true);
            
            movie.Year = movie.Year.TrimEnd('/');

            Debugger.LogMessageToFile("[IMDb film details downloader] IMDb returned Year: " + movie.Year);
            //MessageBox.Show("IMDb returned Year: " + movie.Year);
        
        }


        internal static void GetTagline
            (IIMDbMovie movie,
            string trimmedHTML, IMDbRegEx imDbRegEx)
        {


            if (!ImdbFilmDetailsIndividualChoices.GetIMDbMovieTaglines)
                return;

            Debugger.LogMessageToFile("Extracting Tagline...");

            Match match = imDbRegEx.GetRegExMatch
                (trimmedHTML, imDbRegEx.TaglinePattern);

            movie.Tagline = imDbRegEx.GetMatchValue
                (match, "Tagline", true);

            Debugger.LogMessageToFile("IMDb returned Tagline: " + movie.Tagline);
            //MessageBox.Show("IMDb returned Tagline: " + movie.Tagline);

        }


        internal static void GetRating
            (IIMDbMovie movie, 
            string trimmedHtml, IMDbRegEx imDbRegEx)
        {


            if (!ImdbFilmDetailsIndividualChoices.GetIMDbMovieRatings)
                return;

            Debugger.LogMessageToFile
                ("[IMDb film details downloader]" +
                                      " Extracting Rating...");

            Match match = imDbRegEx.GetRegExMatch
                (trimmedHtml, imDbRegEx.RatingPattern);

            movie.Rating = imDbRegEx.GetMatchValue
                (match, "Rating", true);

            Debugger.LogMessageToFile("IMDb returned Rating: "
                                      + movie.Rating);

        }


        internal static void GetRuntime
            (IIMDbMovie movie,
            string trimmedHTML, 
            IMDbRegEx imDbRegEx)
        {

            Debugger.LogMessageToFile
                ("[IMDb film details downloader]" +
                " Extracting Runtime...");

            Match match = imDbRegEx.GetRegExMatch
                (trimmedHTML, imDbRegEx.RuntimePattern);
            
            string runtime = imDbRegEx.GetMatchValue
                (match, "Runtime", true);
            
            runtime = IMDbMovieDetailsDownloaderHelpers.FixRuntime(runtime);
            
            movie.Runtime = runtime;
            
            Debugger.LogMessageToFile("IMDb returned Runtime: " + movie.Runtime);
            //MessageBox.Show("IMDb returned Runtime: " + movie.Runtime);
        
        }

    


    
    }


}
