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
using System.Text.RegularExpressions;

namespace MediaFairy.IMDb
{
    class IMDbRegEx
    {

        #region RegEx Pattern Variables

        public string _trimmedHTMLpattern = "";

        public string _movieResultPattern1 = "";
        public string _movieResultPattern2 = "";

        public string _titlePattern = "";
        public string _yearPattern = "";
        public string _releaseDatePattern = "";
        public string _taglinePattern = "";
        public string _runtimePattern = "";
        public string _ratingPattern = "";
        public string _ratingDescriptionPattern = String.Empty;
        public string _studioPattern = "";
        public string _reviewPattern = "";
        public string _shortOverviewPattern = "";
        public string _longOverviewPattern = "";
        public string _goofPattern = "";
        public string _triviaPattern = "";
        public string _quoteBlockPattern = "";
        public string _quotePattern = "";

        public string _genrePattern = "";
        public string _genre2Pattern = "";

        public string _castPattern = "";
        public string _castPersonPatternWithCharacterURLs = "";
        public string _castPersonPatternWithNOCharacterURLs = "";

        public string _directorPattern = "";
        public string _writerPattern = "";
        public string _personPattern = "";


        //private string _EpisodePattern = "";


        #endregion RegEx Pattern Definitions


        #region Constructor
        public IMDbRegEx()
        {
            SetRegExPatterns();
        }
        #endregion


        #region RegEx Patterns

        public void SetRegExPatterns()
        {

            //HTML cleaning pattern, for faster performance
            //_trimmedHTMLpattern = @"<a\shref=""/"">imdb</a>(.*?)show\smore\srecommendations"; /Old html cleaning pattern
            //_trimmedHTMLpattern = "(<h1 class=\"header\">.*?)<h2>Recommendations</h2>";
            _trimmedHTMLpattern = "(<h1 class=\"header\".*?)dataslot=\"recommendations\"";


            _movieResultPattern1 = @"<br><a\shref=""(?<URL>/title/(?<IMDbID>.*?))/.*?"">(?<Title>.*?)</a>((?<Extra>\s*?\((?<Year>.*?)\).*?))?<";
            _movieResultPattern2 = @"</td><td\svalign=""top""><a\shref=""(?<URL>/title/(?<IMDbID>.*?))/.*?"">(?<Title>.*?)</a>((?<Extra>\s*?\((?<Year>.*?)\).*?))?<";


            #region Old Title Patterns
            //_titlePattern = @"title=""(?<Title>.*?)"">"; //Even older Title regex
            //_titlePattern = "&gt;.*?<b>(?<Title>.*?)\\("; //Old Title regex
            //_titlePattern = @"<h1\sclass=""header"">\s*(?<Title>.*?)\s*?<span>";
            #endregion

            _titlePattern = @"<h\d\sclass=""header"".*?>\s(?<Title>.*?)\s*?<span>";



            //_yearPattern = @"/years/(?<Year>.*?)"">"; /Old Year pattern
            _yearPattern = @"<a\shref=""/year/.*?/"">(?<Year>.*?)</a>";


            //_releaseDatePattern = @"<h5>Release.Date:</h5>(?<ReleaseDate>[^(]*)";
            _releaseDatePattern = @"Release Date:</h\d>\s(?<ReleaseDate>.*?)\s\(.*?\)";

            //_taglinePattern = @"<h5>tagline:</h5>\s(?<Tagline>[^\[<]*)";
            _taglinePattern = @"Taglines:</h\d>\s*(?<Tagline>.*?)\s<span";

            //_runtimePattern = @"<h5>runtime:</h5>\s?(?<Runtime>.*?)\s?min\s?";
            _runtimePattern = @"Runtime:</h\d>\s*(?<Runtime>.*?)\smin";

            //_ratingPattern = "<h5>Certification:</h5>[^.]*USA:(?<Rating>[^<]*)</a>";
            //_ratingPattern = @"Rated (?<Rating>.*?)\s.*?<span";
            _ratingPattern = @"Rated (?<Rating>.*?)\s.*?</span>";


            _ratingDescriptionPattern = @"Motion Picture Rating.*?</h\d>\s(?<RatingDescription>.*?)\s<span";


            //Review
            //_reviewPattern = ("<div class=\"meta\">.*?<b>(?<Review>.*?)/");
            _reviewPattern = "<span class=\"rating-rating\">(?<Review>.*?)<span>";

            #region Studio
            //_studioPattern = @"<h5>company:</h5>.*?<a\shref="".*?"">(?<Studio>[^<]*)";
            _studioPattern = @"Production Co:</h\d>\s*?.*?/company/.*?>(?<Studio>.*?)</a>";
            #endregion

            //_shortOverviewPattern = "<h5>plot.*?</h5>(?<ShortOverview>.*?)<";
            //_shortOverviewPattern = "<h5>Plot:.*?</h5>.*?>(?<ShortOverview>.*?)<a class";
            _shortOverviewPattern = @"<p>\s<p>(?<ShortOverview>.*?)\s</p>\s</p>";


            _longOverviewPattern = @"<p\sclass=""plotpar"">\s(?<LongOverview>[^<]*)(?=\s*<i>\s*written)";

            //_goofPattern = "<li><b>(?<Category>.*?):</b>(?<Goof>.*?)<br>";
            //_goofPattern = "<li>.*?<b>(?<Category>.*?):</b>(?<Goof>.*?)<br>";
            _goofPattern = @"<li><a\sname=.*?<b>(?<Category>.*?):</b>(?<Goof>.*?)<br>";


            //_triviaPattern = "<li>(?<Trivia>.*?)<br>";
            _triviaPattern = @"<div\sclass=""sodatext"">\s*?(?<Trivia>.*?)\s*?<br>";


            _quoteBlockPattern = @"<a\sname=""qt.*?></a>(?<QuoteBlock>.*?)<hr";
            _quotePattern = "<b>.*?(?<Character>.*?)</b>:(?<Quote>.*?)<br>";

            #region Genres
            //_genrePattern = "<h5>genre:</h5>.*?</div>";
            _genrePattern = @"<h\d\sclass=""inline"">\s*?Genres??:.*?</div>";

            //_genre2Pattern = "\">(?!more)(?<Genre>[^<]*)";
            _genre2Pattern = "/genre/.*?\">(?!more)(?<Genre>[^<]*)";
            #endregion

            #region Cast
            _castPattern = @"<table\sclass=""cast"">.*?</table>";
            _castPersonPatternWithCharacterURLs = "a\\shref=\"(?<PersonURL>/name/[^\"]*?)\"[^>]*?\">(?<PersonName>[A-Za-z][^<]*?)<.*?character/[^\"]*?\">(?<CharacterName>[^<]*?)<";
            _castPersonPatternWithNOCharacterURLs = "((<a\\shref=\"(?<PersonURL>/name/[^\"]*))(?!\"\\s)\">(?<PersonName>[^<]*))(?!</a></td><td\\sclass=\"ddd\">\\s\\.\\.\\.\\s</td><td\\sclass=\"char\"><a\\shref=\"/character/).*?<td\\sclass=\"char\">(?=(?<CharacterName>[^<]*)(?=</td>))";
            #endregion

            #region Directors
            //_directorPattern = "<h5>director.*?</div>";
            _directorPattern = @"<h\d\sclass=""inline"">\s*?Directors??:.*?</div>";
            #endregion

            #region Writers
            //_writerPattern = "<h5>writer.*?</div>";
            _writerPattern = @"<h\d\sclass=""inline"">\s*?Writers??:.*?</div>";
            #endregion

            #region Person
            _personPattern = @"(?<Person>href=""(?<PersonURL>/name/[^""]*)"".*?>(?<PersonName>[^<]*)<)";
            #endregion


            //_EpisodePattern = "<h5>Original Air Date:</h5>.*?(?<Episode>.*?)</div>"; 


        }


        public Match GetRegExMatch(string input, string pattern)
        {
            Regex regEx = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            return regEx.Match(input);
        }

        public string GetRegExString(string input, string pattern)
        {
            Regex regEx = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            Match match = regEx.Match(input);
            if (match != null && match.Value != null)
                return match.Value.Trim();

            return String.Empty;
        }

        public string GetMatchValue(Match match, string groupName, bool cleanUpHTML)
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
                        Debugger.LogMessageToFile("IMDb parser error: The returned data contains illegal characters. Full error text follows: " + e.ToString());
                    }

                }
                return value.Trim();
            }

            return String.Empty;
        }

        #endregion




    }
}
