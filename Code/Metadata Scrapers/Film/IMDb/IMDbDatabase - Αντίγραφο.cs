using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

#if USE_MEEDIO 
using Meedio; 
#elif USE_MEEDIOS
using MeediOS;
#endif

using System.Windows.Forms.Design;
using System.Threading;


namespace MediaFairy
{
    internal class IMDbDatabase
    {

        #region Variables
        private string _country = "USA";
        private int _actorCount = 0;
        private int _retries = 0;
        private static readonly string _url = "http://www.imdb.com/title/";
        #endregion

        #region RegEx Pattern Variables

        private string _trimmedHTMLpattern = "";

        private string _movieResultPattern1 = "";
        private string _movieResultPattern2 = "";

        private string _titlePattern = "";
        private string _yearPattern = "";
        private string _releaseDatePattern = "";
        private string _taglinePattern = "";
        private string _runtimePattern = "";
        private string _ratingPattern = "";
        private string _studioPattern = "";
        private string _reviewPattern = "";
        private string _shortOverviewPattern = "";
        private string _longOverviewPattern = "";
        private string _goofPattern = "";
        private string _triviaPattern = "";
        private string _quoteBlockPattern = "";
        private string _quotePattern = "";

        private string _genrePattern = "";
        private string _genre2Pattern = "";

        private string _castPattern = "";
        private string _castPersonPatternWithCharacterURLs = "";
        private string _castPersonPatternWithNOCharacterURLs = "";

        private string _directorPattern = "";
        private string _writerPattern = "";
        private string _personPattern = "";


        //private string _EpisodePattern = "";


        #endregion RegEx Pattern Definitions

        
        #region Constructor
        public IMDbDatabase(string country,
            int actorCount,
            int retries)
        {
            _country = country;
            _actorCount = actorCount;
            _retries = retries;
            SetRegExPatterns();
        }
        #endregion




        #region New Methods
        
        public IList<IIMDbSearchResult> SearchMovies(string title)
        {
            List<IIMDbSearchResult> results = new List<IIMDbSearchResult>();
            string url = "http://www.imdb.com/find?s=title&q=" + JCUtils.WebUtils.EncodeURLstring(title);
            string html;

            try
            {
                html = JCUtils.WebUtils.GET(url);
            }
            catch
            {
                try
                {
                    Thread.Sleep(2000);
                    html = JCUtils.WebUtils.GET(url);
                }
                catch (Exception)
                {
                    //MessageBox.Show(e.ToString());
                    return results;
                }
            }

            if (html.ToLower().IndexOf("<b>popular titles") < 0
                && html.ToLower().IndexOf("<b>titles") < 0)
            {
                int num1 = 0;
                int num2 = 0;
                string text1 = "";
                string text2 = "";
                num1 = html.ToLower().IndexOf("<title>");
                if (num1 > -1)
                {
                    string regExString1 = @"<title>(?<Title>.*?)\((?<Year>.*?)\)</title>";
                    string regExString2 = @"<a\shref=""/title/(?<IMDbID>[^/]*)/fullcredits""";
                    Match match = GetRegExMatch(html, regExString1);
                    if (match != null && match.Length > 0)
                    {
                        IMDbSearchResult result = new IMDbSearchResult();
                        result.Title = GetMatchValue(match, "Title", true);
                        result.Year = GetMatchValue(match, "Year", true);
                        match = GetRegExMatch(html, regExString2);
                        if (match != null && match.Length > 0)
                            result.IMDb_ID = GetMatchValue(match, "IMDbID", true);
                        if (result.IMDb_ID.Trim() != ""
                            && result.Title.Trim() != "")
                            results.Add(result);
                    }
                }
            }
            else
            {
                Match match = GetRegExMatch(html, _movieResultPattern1);
                while (match != null && match.Length > 0)
                {
                    IMDbSearchResult result = new IMDbSearchResult();
                    result.IMDb_ID = GetMatchValue(match, "IMDbID", true);
                    result.Title = GetMatchValue(match, "Title", true);
                    result.Year = GetMatchValue(match, "Year", true);
                    result.URL = "http://www.imdb.com" + GetMatchValue(match, "URL", true);
                    string extra = GetMatchValue(match, "Extra", true);
                    if (extra.ToLower().Trim().Contains("(vg)"))
                        result.Ignore = true;
                    if (!result.Ignore)
                        results.Add(result);
                    match = match.NextMatch();
                }

                match = GetRegExMatch(html, _movieResultPattern2);
                while (match != null && match.Length > 0)
                {
                    IMDbSearchResult result = new IMDbSearchResult();
                    result.IMDb_ID = GetMatchValue(match, "IMDbID", true);
                    result.Title = GetMatchValue(match, "Title", true);
                    result.Year = GetMatchValue(match, "Year", true);
                    result.URL = "http://www.imdb.com" + GetMatchValue(match, "URL", true);
                    string extra = GetMatchValue(match, "Extra", true);
                    if (extra.ToLower().Trim().Contains("(vg)"))
                        result.Ignore = true;
                    results.Add(result);
                    match = match.NextMatch();
                }
            }

            //MessageBox.Show("Results: " + results.Count.ToString() );            

            return results;
        }



        public IIMDbMovie GetMovieInfoFromIMDb(string imdbID, bool ShowProgress )
        {
            IMDbMovie movie = new IMDbMovie();


            try
            {

                #region local variables
                Debugger.LogMessageToFile("[IMDb film details downloader] Entered GetMovieInfoFromIMDb()");
                Debugger.LogMessageToFile("[IMDb film details downloader] IMDb base url: " + _url);

                string movieURL = _url + imdbID + "/";
                Debugger.LogMessageToFile("[IMDb film details downloader] movie URL: " + movieURL);

                string html = String.Empty;

                Debugger.LogMessageToFile("[IMDb film details downloader] Downloading film's main html page...");
                try
                {
                    html = JCUtils.WebUtils.GET(movieURL);
                }
                catch
                {
                    Debugger.LogMessageToFile("[IMDb film details downloader] IMDb did not respond. Retrying...");
                    Helpers.UpdateProgress("Updating Movies Section...", "IMDb did not respond. Retrying...", null);
                    try
                    {
                        html = JCUtils.WebUtils.GET(movieURL);
                    }
                    catch
                    {
                        Debugger.LogMessageToFile("A connection error occured while attempting to download IMDb's film web page. Giving up for this item.");
                        Helpers.UpdateProgress("Updating Movies Section...", "Unable to connect to IMDb. Details for this film will not be downloaded.", null);
                        StatusForm.statusForm.TrayIcon.ShowBalloonTip(5000, "Communication with IMDb failed", "MediaFairy was unable to connect to IMDb in order to download details for a film. Please check your internet connection availability, otherwise the online database may be temporarily offline or unreachable.", ToolTipIcon.Warning);
                        return null;
                    }
                }

                //MessageBox.Show("Step 4");

                if (String.IsNullOrEmpty(html))
                {
                    Debugger.LogMessageToFile("[IMDb film details downloader] Unable to get film data from IMDb. The returned film HTML page was empty.");
                    return null;
                }

                Debugger.LogMessageToFile("[IMDb film details downloader] The returned IMDb film html page contains vaid data.");
                //MessageBox.Show("Step 5");

                string trimmedHTML;

                string creditsURL = movieURL + "fullcredits";
                string longOverviewURL = movieURL + "plotsummary";
                string goofURL = movieURL + "goofs";
                string triviaURL = movieURL + "trivia";
                string quotesURL = movieURL + "quotes";
                #endregion


                #region Clean film's html page
                Debugger.LogMessageToFile("Cleaning IMDb result html ");
                trimmedHTML = GetRegExString(html, _trimmedHTMLpattern);
                //Debugger.LogMessageToFile("Trimmed HTML: " + trimmedHTML);
                //MessageBox.Show("Trimmed HTML: " + trimmedHTML);
                //MessageBox.Show("titlePattern: " + _titlePattern);
                #endregion


                #region Get Title
                Debugger.LogMessageToFile("[IMDb film details downloader] Extracting title...");
                Match match = GetRegExMatch(trimmedHTML, _titlePattern);
                //Match match = GetRegExMatch(html, _titlePattern);

                if (match == null)
                {
                    Debugger.LogMessageToFile("Warning! IMDb title matching was unsuccesful. Did IMDb website changed layout?");
                    return null;
                }
                else
                {
                    Debugger.LogMessageToFile("[IMDb film details downloader] The film's title was succesfully located in the IMDb result html.");
                }


                movie.IMDb_ID = imdbID;
                movie.Title = GetMatchValue(match, "Title", true);

                if (String.IsNullOrEmpty(movie.Title))
                {
                    StatusForm.statusForm.TrayIcon.ShowBalloonTip(10000, "Unable to extract film details from IMDb", "MediaFairy's IMDb film details downloader was unable to extract a film's Title from the IMDb database. If the IMDb website changed, please report this issue to the plugin's developer in order for this engine to be updated. Film details downloading for this item will be skipped.", ToolTipIcon.Warning);
                    return movie;
                }

                Debugger.LogMessageToFile("[IMDb film details downloader] IMDb returned title: " + movie.Title);
                MessageBox.Show("IMDb returned title: " + movie.Title);
                #endregion

                
                #region Grab Details
                if (ShowProgress)
                    Importer.thisProgress.Progress(Importer.CurrentProgress, "Downloading details from IMDb for '" + movie.Title + "'...");

                #region Get details from main IMDb page

                #region Extract Year
                Debugger.LogMessageToFile("[IMDb film details downloader] Extracting Year...");
                match = GetRegExMatch(trimmedHTML, _yearPattern);
                //match = GetRegExMatch(html, _yearPattern);
                movie.Year = GetMatchValue(match, "Year", true);
                movie.Year = movie.Year.TrimEnd('/');
                Debugger.LogMessageToFile("[IMDb film details downloader] IMDb returned Year: " + movie.Year);
                MessageBox.Show("IMDb returned Year: " + movie.Year);
                #endregion

                #region Extract Release Date
                Debugger.LogMessageToFile("[IMDb film details downloader] Extracting Release Date...");
                match = GetRegExMatch(trimmedHTML, _releaseDatePattern);
                //match = GetRegExMatch(html, _releaseDatePattern);
                movie.Release_Date = GetMatchValue(match, "ReleaseDate", true);
                Debugger.LogMessageToFile("[IMDb film details downloader] IMDb returned Release Date: " + movie.Release_Date);
                MessageBox.Show("IMDb returned Release Date: " + movie.Release_Date);
                #endregion

                #region Extract Tagline
                Debugger.LogMessageToFile("Extracting Tagline...");
                match = GetRegExMatch(trimmedHTML, _taglinePattern);
                //match = GetRegExMatch(html, _taglinePattern);
                movie.Tagline = GetMatchValue(match, "Tagline", true);
                Debugger.LogMessageToFile("IMDb returned Tagline: " + movie.Tagline);
                MessageBox.Show("IMDb returned Tagline: " + movie.Tagline);
                #endregion

                #region Extract Runtime
                Debugger.LogMessageToFile("[IMDb film details downloader] Extracting Runtime...");
                match = GetRegExMatch(trimmedHTML, _runtimePattern);
                string runtime = GetMatchValue(match, "Runtime", true);
                runtime = FixRuntime(runtime);
                movie.Runtime = runtime;
                Debugger.LogMessageToFile("IMDb returned Runtime: " + movie.Runtime);
                MessageBox.Show("IMDb returned Runtime: " + movie.Runtime);
                #endregion

                #region Extract Rating
                Debugger.LogMessageToFile("[IMDb film details downloader] Extracting Rating...");
                //match = GetRegExMatch(html, _ratingPattern);
                match = GetRegExMatch(trimmedHTML, _ratingPattern);
                movie.Rating = GetMatchValue(match, "Rating", true);
                Debugger.LogMessageToFile("IMDb returned Rating: " + movie.Rating);
                MessageBox.Show("[IMDb film details downloader] IMDb returned Rating: " + movie.Rating);
                #endregion

                #region Extract Rating Description
                Debugger.LogMessageToFile("[IMDb film details downloader] Extracting Rating Description...");
                //match = GetRegExMatch(html, _ratingDescriptionPattern);
                match = GetRegExMatch(trimmedHTML, _ratingDescriptionPattern);
                movie.RatingDescription = GetMatchValue(match, "RatingDescription", true);
                Debugger.LogMessageToFile("[IMDb film details downloader]  IMDb returned Rating Description: " + movie.RatingDescription);
                MessageBox.Show("IMDb returned Rating Description: " + movie.Rating);
                #endregion


                //// Grab Episode Info
                //match = GetRegExMatch(trimmedHTML, _EpisodePattern);
                //movie.Episode = GetMatchValue(match, "Episode", true);
                //Debugger.LogMessageToFile("IMDb returned Episode: " + movie.Episode);

                //Debugger.LogMessageToFile(_trimmedHTMLpattern);
                //Debugger.LogMessageToFile( trimmedHTML );
                //Debugger.LogMessageToFile( _reviewPattern );

                #region Extract Review
                Debugger.LogMessageToFile("[IMDb film details downloader] Extracting Review score...");
                match = GetRegExMatch(trimmedHTML, _reviewPattern);
                movie.Review = GetMatchValue(match, "Review", true);
                if (movie.Review.Contains("/"))
                    movie.Review = movie.Review.Substring(0, movie.Review.IndexOf("/"));
                Debugger.LogMessageToFile("IMDb returned Review: " + movie.Review);
                MessageBox.Show("IMDb returned Review: " + movie.Review);
                #endregion

                #region Extract Studio
                Debugger.LogMessageToFile("[IMDb film details downloader] Extracting Studio...");
                match = GetRegExMatch(trimmedHTML, _studioPattern);
                movie.Studio = GetMatchValue(match, "Studio", true);
                Debugger.LogMessageToFile("[IMDb film details downloader] IMDb returned Studio: " + movie.Studio);
                MessageBox.Show("IMDb returned Studio: " + movie.Studio);
                #endregion


                #region  Grab Short Overview
                Debugger.LogMessageToFile("[IMDb film details downloader] Extracting Short Overview...");
                //match = GetRegExMatch(html, _shortOverviewPattern);
                match = GetRegExMatch(trimmedHTML, _shortOverviewPattern);

                movie.OverviewShort = GetMatchValue(match, "ShortOverview", true);
                if (movie.OverviewShort.ToLower().EndsWith("more"))
                    movie.OverviewShort = movie.OverviewShort.Substring(0, movie.OverviewShort.Length - 4).Trim();
                movie.OverviewShort = movie.OverviewShort.Trim() + "...";

                Debugger.LogMessageToFile("IMDb returned Overview: " + movie.OverviewShort);
                MessageBox.Show("IMDb returned Overview: " + movie.OverviewShort);
                #endregion


                GetDirectors(ref movie, trimmedHTML);

                GetWriters(ref movie, trimmedHTML);

                GetGenres(ref movie, trimmedHTML);

                #endregion

                if (ShowProgress)
                {
                    #region Grab details from additional IMDb pages

                    //if (ShowProgress)
                    //    Importer.thisProgress.Progress(Importer.CurrentProgress, "Getting actors...");
                    GetActors(ref movie, creditsURL);

                    //if (ShowProgress)
                    //    Importer.thisProgress.Progress(Importer.CurrentProgress, "Getting plot...");
                    GetLongOverview(ref movie, longOverviewURL);

                    //if (ShowProgress)
                    //    Importer.thisProgress.Progress(Importer.CurrentProgress, "Getting trivia...");

                    //GetTrivia(ref movie, triviaURL, ShowProgress);

                    //if (ShowProgress)
                    //    Importer.thisProgress.Progress(Importer.CurrentProgress, "Getting goofs...");

                    //GetGoofs(ref movie, goofURL);

                    //if (ShowProgress)
                    //    Importer.thisProgress.Progress(Importer.CurrentProgress, "Getting quotes...");

                    //GetQuotes(ref movie, quotesURL);

                    #endregion
                }

                #endregion

            }
            catch (Exception e)
            {
                Debugger.LogMessageToFile("The GetMovieInfo() method returned an exception: " + Environment.NewLine + e.ToString() );
            }

            return movie;
        }



        public IIMDbMovieCollection SearchMovieInfo(string title )
       
        {
            IMDbMovieCollection movies = new IMDbMovieCollection();

            try
            {
                IList<IIMDbSearchResult> results = SearchMovies(title);
                if (results != null)
                    foreach (IIMDbSearchResult result in results)
                    {
                        if (result.IMDb_ID.Trim() != "")
                        {
                            IIMDbMovie movie = GetMovieInfoFromIMDb(result.IMDb_ID.Trim(), false);
                            if (movie != null)
                                movies.Add(movie);
                        }
                    }
                return movies;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        private void GetDirectors(ref IMDbMovie movie, string html)
        {
            // Grab Directors
            string directorHTML = GetRegExString(html, _directorPattern);
            if (directorHTML.Trim() != "")
            {
                Match match = GetRegExMatch(directorHTML, _personPattern);
                while (match != null && match.Length > 0)
                {
                    string personID = GetMatchValue(match, "PersonURL", true);
                    if (personID.Trim() != "")
                    {
                        IIMDbPerson director = movie.People.GetPersonByID(personID);
                        if (director == null)
                        {
                            director = new IMDbPerson();
                            movie.People.Add(director);
                        }
                        director.URL = personID;
                        director.Name = GetMatchValue(match, "PersonName", true);
                        director.IsDirector = true;

                        match = match.NextMatch();
                    }
                }
            }
        }


        private void GetWriters(ref IMDbMovie movie, string html)
        {
            // Grab Writers
            string writerHTML = GetRegExString(html, _writerPattern);
            if (writerHTML.Trim() != "")
            {
                Match match = GetRegExMatch(writerHTML, _personPattern);
                while (match != null && match.Length > 0)
                {
                    string personID = GetMatchValue(match, "PersonURL", true);
                    if (personID.Trim() != "")
                    {
                        IIMDbPerson writer = movie.People.GetPersonByID(personID);
                        if (writer == null)
                        {
                            writer = new IMDbPerson();
                            movie.People.Add(writer);
                        }
                        writer.URL = personID;
                        writer.Name = GetMatchValue(match, "PersonName", true);
                        writer.IsWriter = true;

                        match = match.NextMatch();
                    }
                }
            }
        }

        private void GetActors(ref IMDbMovie movie, string creditsURL)
        {
            // get cast html
            string html = MediaFairy.Downloaders.DownloadHTMLfromURL(creditsURL);

            // Grab Actors
            string trimmedHTML = GetRegExString(html, _castPattern);
            if (trimmedHTML.Trim() != "")
            {
                Match match1 = GetRegExMatch(trimmedHTML, _castPersonPatternWithCharacterURLs);
                while (match1 != null && match1.Length > 0)
                {
                    string personID = GetMatchValue(match1, "PersonURL", true);
                    if (personID.Trim() != "")
                    {
                        IIMDbPerson actor = movie.People.GetPersonByID(personID);
                        if (actor == null)
                        {
                            actor = new IMDbPerson();
                            movie.People.Add(actor);
                        }
                        actor.URL = personID;
                        actor.Name = GetMatchValue(match1, "PersonName", true);
                        actor.IsActor = true;
                        string characterString = GetMatchValue(match1, "CharacterName", true);
                        ParseCharacters(ref actor, characterString);

                        match1 = match1.NextMatch();
                    }
                }

                Match match2 = GetRegExMatch(trimmedHTML, _castPersonPatternWithNOCharacterURLs);
                while (match2 != null && match2.Length > 0)
                {
                    string personID = GetMatchValue(match2, "PersonURL", true);
                    if (personID.Trim() != "")
                    {
                        IIMDbPerson actor = movie.People.GetPersonByID(personID);
                        if (actor == null)
                        {
                            actor = new IMDbPerson();
                            movie.People.Add(actor);
                        }
                        actor.URL = personID;
                        actor.Name = GetMatchValue(match2, "PersonName", true);
                        actor.IsActor = true;
                        string characterString = GetMatchValue(match2, "CharacterName", true);
                        ParseCharacters(ref actor, characterString);

                        match2 = match2.NextMatch();
                    }
                }
            }
        }

        private void GetGenres(ref IMDbMovie movie, string html)
        {
            // Grab Genres
            Match match = GetRegExMatch(html, _genrePattern);
            if (match != null && match.Length > 0)
            {
                match = GetRegExMatch(match.Value, _genre2Pattern);
                while (match != null && match.Length > 0)
                {
                    string genre = GetMatchValue(match, "Genre", true);
                    if (genre.Trim() != "")
                        movie.Genres.Add(genre);
                    match = match.NextMatch();
                }
            }
        }

        private void GetLongOverview(ref IMDbMovie movie, string longOverviewURL)
        {
            // get long overview html
            string html = MediaFairy.Downloaders.DownloadHTMLfromURL(longOverviewURL);

            // Grab Long Overview
            Match match = GetRegExMatch(html, _longOverviewPattern);
            movie.OverviewLong = GetMatchValue(match, "LongOverview", true);
        }

        private void GetGoofs(ref IMDbMovie movie, string goofURL)
        {
            // get goof html
            string html = Downloaders.DownloadHTMLfromURL(goofURL);

            // Grab goofs
            Match match = GetRegExMatch(html, _goofPattern);
            while (match != null && match.Length > 0)
            {
                IMDbGoof goof = new IMDbGoof();
                goof.Category = GetMatchValue(match, "Category", true);
                goof.Description = GetMatchValue(match, "Goof", true);
                movie.Goofs.Add(goof);
                match = match.NextMatch();
            }
        }

        private void GetTrivia(ref IMDbMovie movie, string triviaURL , bool ShowProgress)
        {
            // get trivia html
            string html = MediaFairy.Downloaders.DownloadHTMLfromURL(triviaURL);

            // Grab trivia
            Match match = GetRegExMatch(html, _triviaPattern);
            while (match != null && match.Length > 0)
            {
                string trivia = GetMatchValue(match, "Trivia", true);
                if (trivia.Trim() != "")
                    movie.Trivia.Add(trivia.Trim());
                match = match.NextMatch();
            }
        }

        private void GetQuotes(ref IMDbMovie movie, string quotesURL)
        {
            // get quotes html
            string html = MediaFairy.Downloaders.DownloadHTMLfromURL(quotesURL);

            // Grab quotes
            Match match1 = GetRegExMatch(html, _quoteBlockPattern);
            while (match1 != null && match1.Length > 0)
            {
                string quoteBlockHTML = GetMatchValue(match1, "QuoteBlock", false);
                if (quoteBlockHTML.Trim() != "")
                {
                    List<IIMDbQuote> quoteBlock = new List<IIMDbQuote>();
                    Match match2 = GetRegExMatch(quoteBlockHTML, _quotePattern);
                    while (match2 != null && match2.Length > 0)
                    {
                        IMDbQuote quote = new IMDbQuote();
                        quote.Character = GetMatchValue(match2, "Character", true);
                        quote.Text = GetMatchValue(match2, "Quote", true);
                        quoteBlock.Add(quote);
                        match2 = match2.NextMatch();
                    }
                    if (quoteBlock.Count > 0)
                        movie.Quotes.Add(quoteBlock);
                }
                match1 = match1.NextMatch();
            }
        }

        private void ParseCharacters(ref IIMDbPerson actor, string characterString)
        {
            if (characterString.Contains("/"))
                foreach (string character in characterString.Split(new char[] { '/' }))
                {
                    actor.AddRole(character);
                }
            else
                actor.AddRole(characterString);
        }
        
        #endregion


        private string FixRuntime(string value)
        {
            try
            {
                string runtime = value.Trim();
                if (runtime.IndexOf(":") > -1
                    && runtime.IndexOf(":") < runtime.Length -1)
                    runtime = runtime.Substring(runtime.IndexOf(":") + 1).Trim();
                if (runtime.IndexOf("\\") > -1)
                    runtime = runtime.Substring(0, runtime.IndexOf("\\")).Trim();
                if (runtime.IndexOf("/") > -1)
                    runtime = runtime.Substring(0, runtime.IndexOf("/")).Trim();
                if (runtime.IndexOf(" ") > -1)
                    runtime = runtime.Substring(0, runtime.IndexOf(" ")).Trim();
                return runtime.Trim();
            }
            catch
            {
                return value.Trim();
            }
        }

        #region RegEx Stuff
        private void SetRegExPatterns()
        {

            //HTML cleaning pattern, for faster performance
            //_trimmedHTMLpattern = @"<a\shref=""/"">imdb</a>(.*?)show\smore\srecommendations"; /Old html cleaning pattern
            _trimmedHTMLpattern = "(<h1 class=\"header\">.*?)<h2>Recommendations</h2>";

            _movieResultPattern1 = @"<br><a\shref=""(?<URL>/title/(?<IMDbID>.*?))/.*?"">(?<Title>.*?)</a>((?<Extra>\s*?\((?<Year>.*?)\).*?))?<";
            _movieResultPattern2 = @"</td><td\svalign=""top""><a\shref=""(?<URL>/title/(?<IMDbID>.*?))/.*?"">(?<Title>.*?)</a>((?<Extra>\s*?\((?<Year>.*?)\).*?))?<";


            //_titlePattern = @"title=""(?<Title>.*?)"">"; //Even older Title regex
            //_titlePattern = "&gt;.*?<b>(?<Title>.*?)\\("; //Old Title regex
            _titlePattern = @"<h1.class=""header"">.(?<Title>.*?)\s";

            //_yearPattern = @"/years/(?<Year>.*?)"">"; /Old Year pattern
            _yearPattern = @"<a\shref=""/year/.*?/"">(?<Year>.*?)</a>";


            //_releaseDatePattern = @"<h5>Release.Date:</h5>(?<ReleaseDate>[^(]*)";
            _releaseDatePattern = @"Release Date:</h\d>\s(?<ReleaseDate>.*?)\s\(.*?\)";

            //_taglinePattern = @"<h5>tagline:</h5>\s(?<Tagline>[^\[<]*)";
            _taglinePattern = @"Taglines:</h\d>\s*(?<Tagline>.*?)\s<span";

            //_runtimePattern = @"<h5>runtime:</h5>\s?(?<Runtime>.*?)\s?min\s?";
            _runtimePattern = @"Runtime:</h\d>\s*(?<Runtime>.*?)\smin";

            //_ratingPattern = "<h5>Certification:</h5>[^.]*USA:(?<Rating>[^<]*)</a>";
            _ratingPattern = @"Rated (?<Rating>.*?)\s.*?<span";

            _studioPattern = @"<h5>company:</h5>.*?<a\shref="".*?"">(?<Studio>[^<]*)";
            _reviewPattern = ("<div class=\"meta\">.*?<b>(?<Review>.*?)/");



            _shortOverviewPattern = "<h5>Plot:.*?</h5>.*?>(?<ShortOverview>.*?)<a class";
            //Old pattern:
            //_shortOverviewPattern = "<h5>plot.*?</h5>(?<ShortOverview>.*?)<";
            _longOverviewPattern = @"<p\sclass=""plotpar"">\s(?<LongOverview>[^<]*)(?=\s*<i>\s*written)";
            _goofPattern = "<li><b>(?<Category>.*?):</b>(?<Goof>.*?)<br>";
            _triviaPattern = "<li>(?<Trivia>.*?)<br>";
            _quoteBlockPattern = @"<a\sname=""qt.*?></a>(?<QuoteBlock>.*?)<hr";

            _quotePattern = "<b>.*?(?<Character>.*?)</b>:(?<Quote>.*?)<br>";

            _genrePattern = "<h5>genre:</h5>.*?</div>";
            _genre2Pattern = "\">(?!more)(?<Genre>[^<]*)";

            _castPattern = @"<table\sclass=""cast"">.*?</table>";
            _castPersonPatternWithCharacterURLs = "a\\shref=\"(?<PersonURL>/name/[^\"]*?)\"[^>]*?\">(?<PersonName>[A-Za-z][^<]*?)<.*?character/[^\"]*?\">(?<CharacterName>[^<]*?)<";
            _castPersonPatternWithNOCharacterURLs = "((<a\\shref=\"(?<PersonURL>/name/[^\"]*))(?!\"\\s)\">(?<PersonName>[^<]*))(?!</a></td><td\\sclass=\"ddd\">\\s\\.\\.\\.\\s</td><td\\sclass=\"char\"><a\\shref=\"/character/).*?<td\\sclass=\"char\">(?=(?<CharacterName>[^<]*)(?=</td>))";

            _directorPattern = "<h5>director.*?</div>";
            _writerPattern = "<h5>writer.*?</div>";
            _personPattern = @"(?<Person>href=""(?<PersonURL>/name/[^""]*)"".*?>(?<PersonName>[^<]*)<)"; 



            //_EpisodePattern = "<h5>Original Air Date:</h5>.*?(?<Episode>.*?)</div>"; 


        }
      

        private Match GetRegExMatch(string input, string pattern)
        {
            Regex regEx = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            return regEx.Match(input);
        }

        private string GetRegExString(string input, string pattern)
        {
            Match match;
            Regex regEx = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            match = regEx.Match(input);
            if (match != null && match.Value != null)
                return match.Value.Trim();

            return "";
        }

        private string GetMatchValue(Match match, string groupName, bool cleanUpHTML)
        {
            if (match != null
                && match.Groups[groupName] != null
                && match.Groups[groupName].Value != null)
            {
                string value = match.Groups[groupName].Value;
                if (cleanUpHTML)
                {
                    value = JCUtils.WebUtils.CleanUpHTML(value, true);

                    value = System.Web.HttpUtility.HtmlDecode(value);

                    try
                    {
                        value = JCUtils.WebUtils.FixString(value);
                    }
                    catch (Exception e)
                    {
                        Debugger.LogMessageToFile("IMDb parser error: The returned data contains illegal characters. Full error text follows: " + e.ToString() );
                    }

                }
                return value.Trim();
            }

            return "";
        }
        #endregion

    }
}
