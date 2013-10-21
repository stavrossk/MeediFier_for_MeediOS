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
using MeediFier.ToolBox.Utils;

namespace MeediFier.IMDb
{
    class IMDbRegEx
    {

        #region RegEx Pattern Variables

        public string TrimmedHTMLpattern = string.Empty;

        public string MovieResultPattern1 = string.Empty;
        public string MovieResultPattern2 = string.Empty;


        internal static string TitlePatternPrimary = String.Empty;
        internal static string TitlePatternOriginal = String.Empty;


        public string YearPattern = string.Empty;
        public string ReleaseDatePattern = string.Empty;
        public string TaglinePattern = string.Empty;


        public string RuntimePattern = string.Empty;

        public string RatingPattern = string.Empty;
        public string RatingDescriptionPattern = String.Empty;

        public string StudioPattern = string.Empty;

        public string ReviewPattern = string.Empty;

        public string FilmDescriptionPattern = string.Empty;
        public string FilmStorylinePattern = string.Empty;


        public string GoofPattern = string.Empty;
        public string TriviaPattern = string.Empty;


        public string QuoteBlockPattern = string.Empty;
        public string QuotePattern = string.Empty;


        public string GenrePattern = string.Empty;
        public string Genre2Pattern = string.Empty;

        public string CastPattern = string.Empty;
        public string CastPersonPatternWithCharacterUrLs = string.Empty;
        public string CastPersonPatternWithNoCharacterUrLs = string.Empty;


        public string DirectorPattern = string.Empty;
        public string WriterPattern = string.Empty;
        public string PersonPattern = string.Empty;



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
            TrimmedHTMLpattern = "(<h1 class=\"header\".*?)" +
                                 "dataslot=\"recommendations\"";



            MovieResultPattern1 = @"<br><a\shref=""(?<URL>/title/(?<IMDbID>.*?))/.*?"">
                                    (?<Title>.*?)</a>((?<Extra>\s*?\((?<Year>.*?)\).*?))?<";
           
            
            MovieResultPattern2 = @"</td><td\svalign=""top""><a\shref=""
                                    (?<URL>/title/(?<IMDbID>.*?))/.*?"">(?<Title>.*?)</a>
                                    ((?<Extra>\s*?\((?<Year>.*?)\).*?))?<";






           

           


             TitlePatternOriginal
            = @"""title.extra"">(?<Title>.*?).<i>\(original title\)</i>";



             TitlePatternPrimary
            = @"<h\d\sclass=""header"" itemprop=""name"">\n(?<Title>.*?)\n";










            //_yearPattern = @"/years/(?<Year>.*?)"">"; /Old Year pattern
            //YearPattern =  @"<a\shref=""/year/.*?/"">(?<Year>.*?)</a>";
            YearPattern =    @"<div id=""ratingWidget"">.*?Title:.*?\((?<Title>.*?)\).*?</p>";


            ReleaseDatePattern = @"Release Date:</h\d>\s(?<ReleaseDate>.*?)\s\(.*?\)";

            TaglinePattern = @"Taglines:</h\d>\s*(?<Tagline>.*?)\s<span";

            RuntimePattern = @"Runtime:</h\d>\s*(?<Runtime>.*?)\smin";






            RatingPattern = @"op=""contentRating"">Rated (?<Rating>.*?)\s.*?</span>";


            RatingDescriptionPattern 
                = @"Motion Picture Rating.*?</h\d>\s(?<RatingDescription>.*?)\s<span";




            ReviewPattern = "ratingValue.*?>(?<Review>.*?)</span>";







            #region Studio
            StudioPattern = @"Production Co:</h\d>\s*?.*?/company/.*?>(?<Studio>.*?)</a>";
            #endregion









            FilmDescriptionPattern 
                = @"<p itemprop=""description"">\n(?<ShortOverview>.*?)\n</p>";


            FilmStorylinePattern
                = @"<p\sclass=""plotpar"">\s(?<LongOverview>[^<]*)(?=\s*<i>\s*written)";





            GoofPattern = @"<li><a\sname=.*?<b>(?<Category>.*?):</b>(?<Goof>.*?)<br>";

            TriviaPattern = @"<div\sclass=""sodatext"">\s*?(?<Trivia>.*?)\s*?<br>";

            QuoteBlockPattern = @"<a\sname=""qt.*?></a>(?<QuoteBlock>.*?)<hr";
            QuotePattern = "<b>.*?(?<Character>.*?)</b>:(?<Quote>.*?)<br>";




            #region Genres
            //_genrePattern = "<h5>genre:</h5>.*?</div>";
            GenrePattern = @"<h\d\sclass=""inline"">\s*?Genres??:.*?</div>";

            //_genre2Pattern = "\">(?!more)(?<Genre>[^<]*)";
            Genre2Pattern = "/genre/.*?\">(?!more)(?<Genre>[^<]*)";
            #endregion



            #region Cast
            CastPattern = @"<table\sclass=""cast"">.*?</table>";
            CastPersonPatternWithCharacterUrLs = "a\\shref=\"(?<PersonURL>/name/[^\"]*?)\"[^>]*?\">(?<PersonName>[A-Za-z][^<]*?)<.*?character/[^\"]*?\">(?<CharacterName>[^<]*?)<";
            CastPersonPatternWithNoCharacterUrLs = "((<a\\shref=\"(?<PersonURL>/name/[^\"]*))(?!\"\\s)\">(?<PersonName>[^<]*))(?!</a></td><td\\sclass=\"ddd\">\\s\\.\\.\\.\\s</td><td\\sclass=\"char\"><a\\shref=\"/character/).*?<td\\sclass=\"char\">(?=(?<CharacterName>[^<]*)(?=</td>))";
            #endregion



            #region Directors
            DirectorPattern = @"<h\d\sclass=""inline"">\s*?Directors??:.*?</div>";
            #endregion



            #region Writers
            WriterPattern = @"<h\d\sclass=""inline"">\s*?Writers??:.*?</div>";
            #endregion



            #region Person
            PersonPattern = @"(?<Person>href=""(?<PersonURL>/name/[^""]*)"".*?>(?<PersonName>[^<]*)<)";
            #endregion



        }


        public Match GetRegExMatch(string input, string pattern)
        {
            Regex regEx = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            return regEx.Match(input);
        }



        public string GetRegExString
            (string input, string pattern)
        {


            var regEx = new Regex
                (pattern,
                RegexOptions.IgnoreCase
                | RegexOptions.Singleline);


            Match match = regEx.Match(input);



            return match.Value.Trim();
        }




        public string GetMatchValue
            (Match match, string groupName,
            bool cleanUpHtml)
        {


            if (match == null)
                return String.Empty;


            //MessageBox.Show("Groups[0].Captures[0].Value: "
            //    +  match.Groups[0].Captures[0].Value);


            //try
            //{
            //    MessageBox.Show("Groups[1].Captures[0].Value: "
            //               + match.Groups[1].Captures[0].Value);
            //}
            //catch (Exception e)
            //{
               
               
            //}
           


            string value;

            try
            {
                value = match.Groups[1].Captures[0].Value;
            }
            catch (Exception e)
            {
               
                Debugger.LogMessageToFile(e.ToString());
                return String.Empty;
               
            }
            
           



            if (!cleanUpHtml)
                return value;

            //TODO: Replace this HTML Cleaner with RegEx, xPath or external HtmlAgilityPack cleaner.
            value = HtmlCleaners.HtmlCleanerConventional(value, true);

            value = System.Web.HttpUtility.HtmlDecode(value);

            try
            {
                value = ToolBox.Utils.WebUtils.FixString(value);
            }
            catch (Exception e)
            {

                Debugger.LogMessageToFile(
                    "IMDb parser error:" +
                    " The returned data contains" +
                    " illegal characters." +
                    " Full error text follows: " +
                    e);


            }

            return value != null
                ? value.Trim() 
                : String.Empty;
        }


        #endregion




    }
}
