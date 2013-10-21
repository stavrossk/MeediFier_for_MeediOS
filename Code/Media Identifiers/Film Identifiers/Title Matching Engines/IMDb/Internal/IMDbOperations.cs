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
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Threading;

#if USE_MEEDIO
using Meedio;
#elif USE_MEEDIOS
using MeediFier.ImportingEngine;
using MeediOS;
#endif

namespace MeediFier.IMDb
{

    public class IMDbOperations
    {

        #region class variables

        public IList<IIMDbSearchResult> ImdbList;
        public IIMDbMovie ImdbMovie;
        public IIMDbSearchResult ImdbResult;

        #region Library Fields
        public string Title = String.Empty;
        //public string _year = String.Empty;
        private string _actorField = String.Empty;
        private string _actorRolesField = String.Empty;
        private string _directorField = String.Empty;
        private string _writerField = String.Empty;
        private string _genreField = String.Empty;
        private string _yearField = String.Empty;
        private string _ratingField = String.Empty;
        private string _ratingDescriptionField = String.Empty;
        private string _releaseDateField = String.Empty;
        private string _reviewField = String.Empty;
        private string _taglineField = String.Empty;
        private string _runtimeField = String.Empty;
        private string _titleField = String.Empty;
        //private string _budgetField = "";
        public const string Country = "USA"; //MAKE THIS AN OPTION
        private string _shortOverviewField = String.Empty;
        private string _longOverviewField = String.Empty;
        private string _studioField = String.Empty;
        //private string _trailerField = "";
        //private string _boxOfficeField = "";
        private string _triviaField = String.Empty;
        private string _goofsField = String.Empty;
        public string ImdbId = String.Empty;

        #endregion

        #region other variables
        public bool IsCustom;
        //private bool _storeExtraInfo = false;
        //private string _imgPath = "";
        //private string _movieFilePath = "";
        //private string _fileRenameMask = "";
        //private string _extraInfoPath = "";
        public bool UseFirst;
        public bool UseLatest;
        public bool UseMostPopular;
        //private bool _renameFiles = false;
        //private bool _renameImages = false;
        public bool NotListed;
        public bool DeleteItem;
        //private IMLSection _section = null;
        //private IMLImportProgress _progress = null;
        //private int _progressPercent = 0;
        // ReSharper disable InconsistentNaming
         public AssistType _assistType = AssistType.NoPerfectMatch;
        // ReSharper restore InconsistentNaming
        
        //private int _itemID = 0;
        //private bool _error = false;
        public enum AssistType { Always = 0, Never, NoPerfectMatch, SkipUnknownItems }
        private string _prefix = string.Empty;
        private string _imdbUrl = string.Empty;
        byte[] _aRequestHtml;
/*
        WebClient objWebClient = new WebClient();
*/
        string _myString = string.Empty;
        readonly UTF8Encoding _utf8 = new UTF8Encoding();
        readonly System.Collections.ArrayList _a = new System.Collections.ArrayList();
        #endregion

        #endregion






        public void SaveIMDbMovieDetails
            (IMLItem item,
            IMLSection section,
            IIMDbMovie movie)
        {

            SetFieldNames();

          

            try
            {

                section.BeginUpdate();

                SavePrimaryIMDbDetails(item, movie);


                SaveAdittionalIMDbDetails(item, movie);

                item.SaveTags();
                section.EndUpdate();


            }
            catch (Exception e)
            {
                Debugger.LogMessageToFile("An unexpected error occured trying to save film details from IMDb to library. The error was: " + e );
                StatusForm.statusForm.TrayIcon.ShowBalloonTip(5000, "Error trying to save film details to library", "MediaFairy encountered an error trying to save the details of a film to library. Please see Debug.log for details.", ToolTipIcon.Error);
                MainImportingEngine.GeneralStatus = "Updating Movies section...";
                MainImportingEngine.SpecialStatus = "An error occured trying to save film details from IMDb to library.";
                Helpers.UpdateProgress(MainImportingEngine.GeneralStatus, MainImportingEngine.SpecialStatus, null);
                Thread.Sleep(5000);
                return;
            }

            return;
        }

        private void SavePrimaryIMDbDetails
            (IMLItem item, IIMDbMovie movie)
        {


            ImdbMovie = movie;

            if (ImdbMovie.IMDb_ID.Trim() != string.Empty)
                item.Tags["ImdbID"] = ImdbMovie.IMDb_ID;

            if (ImdbMovie.Year.Trim() != string.Empty)
                item.Tags[_yearField] = ImdbMovie.Year;


            if (ImdbMovie.Title.Trim() == string.Empty) 
                return;


            if (!ImdbMovie.Title.StartsWith("\"")) //for movies
            {

                item.Name = ImdbMovie.Title;
                item.Tags["Title"] = ImdbMovie.Title;
                item.SaveTags();
            }
            else  //for tv shows
            {

                var closingQuoteIndex = ImdbMovie.Title.IndexOf('"', 1);

                if (closingQuoteIndex > 2)
                {
                    var seriesName = ImdbMovie.Title.Substring(1, closingQuoteIndex - 1);
                    item.Name = seriesName;

                    if (String.IsNullOrEmpty(Helpers.GetTagValueFromItem(item, "SeriesName")))
                        item.Tags["SeriesName"] = seriesName;
                }


            }

            item.SaveTags();
        }




        private void SaveAdittionalIMDbDetails
            (IMLItem item, IIMDbMovie movie)
        {

            ImdbMovie = movie;

            if (!Settings.FilmIMDbDetailsDownloaderIsEnabled) return;

            if ( !String.IsNullOrEmpty(ImdbMovie.People.GetActorsString(false).Trim() ) )
                item.Tags[_actorField] = ImdbMovie.People.GetActorsString(false);


            if ( !String.IsNullOrEmpty(ImdbMovie.People.GetRolesString().Trim() ) )
                item.Tags[_actorRolesField] = ImdbMovie.People.GetRolesString();

            if ( !String.IsNullOrEmpty(ImdbMovie.People.GetDirectorString().Trim() ) )
                item.Tags[_directorField] = ImdbMovie.People.GetDirectorString();

            if ( !String.IsNullOrEmpty(ImdbMovie.GetGenresString().Trim() ) )
                item.Tags[_genreField] = ImdbMovie.GetGenresString();

            if ( !String.IsNullOrEmpty(ImdbMovie.OverviewLong.Trim() ) )
                item.Tags[_longOverviewField] = ImdbMovie.OverviewLong;

            var rating = ImdbMovie.Rating;
            //MessageBox.Show(rating);
            rating = FixRating(rating);
            //MessageBox.Show(rating);
            item.Tags[_ratingField] = rating;

            if (!String.IsNullOrEmpty(ImdbMovie.RatingDescription.Trim()))
                item.Tags[_ratingDescriptionField] = ImdbMovie.RatingDescription;

            if (!String.IsNullOrEmpty(ImdbMovie.Review.Trim()))
            {
                if (System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator == ",")
                    item.Tags[_reviewField] = ImdbMovie.Review.Replace(".", ",");
                else
                    item.Tags[_reviewField] = ImdbMovie.Review;
            }

            if ( !String.IsNullOrEmpty(ImdbMovie.Runtime.Trim() ) )
                item.Tags[_runtimeField] = ImdbMovie.Runtime;

            if ( !String.IsNullOrEmpty(ImdbMovie.OverviewShort.Trim()) )
                item.Tags[_shortOverviewField] = ImdbMovie.OverviewShort;

            if ( !String.IsNullOrEmpty(ImdbMovie.Tagline.Trim()) )
                item.Tags[_taglineField] = ImdbMovie.Tagline;

            if (!String.IsNullOrEmpty(ImdbMovie.Release_Date.Trim()))
                item.Tags[_releaseDateField] = ImdbMovie.Release_Date;

            if (!String.IsNullOrEmpty(ImdbMovie.Studio.Trim()))
                item.Tags[_studioField] = ImdbMovie.Studio;

            if (!String.IsNullOrEmpty(ImdbMovie.GetTriviaString().Trim()))
                item.Tags[_triviaField] = ImdbMovie.GetTriviaString();

            if (!String.IsNullOrEmpty(ImdbMovie.GetGoofsString().Trim()))
                item.Tags[_goofsField] = ImdbMovie.GetGoofsString();

           

            #region custom title
            if (IsCustom)
            {

                IsCustom = false;

                if (!String.IsNullOrEmpty(Title.Trim()))
                {
                    if (_titleField.ToLower().Trim() == "name"
                        | _titleField.ToLower().Trim() == "<name>"
                        | _titleField.ToLower().Trim() == "{name}")
                        item.Name = Title;
                    else
                        item.Tags[_titleField] = Title;
                }
            }
            else
            {
                if (!String.IsNullOrEmpty(ImdbMovie.Title.Trim()))
                    if (_titleField.ToLower().Trim() == "name"
                        | _titleField.ToLower().Trim() == "<name>"
                        | _titleField.ToLower().Trim() == "{name}")
                        item.Name = ImdbMovie.Title;
                    else
                        item.Tags[_titleField] = ImdbMovie.Title;
            }
            #endregion

            if (!String.IsNullOrEmpty(ImdbMovie.People.GetWriterString().Trim()))
                item.Tags[_writerField] = ImdbMovie.People.GetWriterString();

            if (IsCustom)
                item.Tags["SearchTypeUsed"] = "---imdb-custom---";
            else
                item.Tags["SearchTypeUsed"] = "---imdb---";

            item.Tags["UpdateStatus"] = string.Empty;
        }


        private void SetFieldNames()
        {
            _titleField = "Title";
            _actorField = "Actors";
            _actorRolesField = "ActorRoles";
            _directorField = "Director";
            _genreField = "Genre";
            _shortOverviewField = "Overview";
            _longOverviewField = "LongOverview";
            _ratingField = "Rating";
            _ratingDescriptionField = "RatingDescription";
            _releaseDateField = "ReleaseDate";
            _reviewField = "Review";
            _runtimeField = "Runtime";

            #region Tagline

            var taglineTag = Settings.MovieNightCompatibility ? "Subtitle" : "Tagline";

            #endregion

            _taglineField = taglineTag;


            _studioField = "Studio";
            _writerField = "Writer";
            _yearField = "Year";
            _triviaField = "Trivia";
            _goofsField = "Goofs";
        }



        //TODO: Should remove this function!
        public bool SearchIMDbMovies()
        {

            #region Item variables
            ImdbResult = null;
            ImdbMovie = null;
            ImdbId = String.Empty;
            IsCustom = false;
            NotListed = false;

            #endregion


            try
            {

                
                ImdbList = IMDbConventionalFilmSearchEngine.SearchForTitleReturnResults(Title);
                Debugger.LogMessageToFile("IMDb results found: " + ImdbList.Count);

                return ImdbList.Count > 0;

            }
            catch (Exception e)
            {
                Debugger.LogMessageToFile("An error occured while performing IMDb search by title. The error was: " + e);
                return false;
            }


        }





        public void SaveEpisodeInfo(string imdbid, IMLItem item)
        {


            _prefix = "http://www.imdb.com/title/";

            if (!imdbid.StartsWith("tt"))
                imdbid = "tt" + imdbid;

            _imdbUrl = _prefix + imdbid;


            _a.Clear();

            try
            {
                _aRequestHtml = Downloaders.TrytoDownloadData(_imdbUrl, "Tv Series", "IMDb", item );
                _myString = _utf8.GetString(_aRequestHtml);

                var regex = new Regex("h5>Original.*\\s*.*\\s*.*\\((?<Episode>.*?)\\)");
                var matches = regex.Matches(_myString);

                if (matches.Count == 0)
                    return;

                foreach (Match match in matches)
                {
                    foreach (Group group in match.Groups)
                    {
                        var b = group.Value + string.Empty;
                        // Add the extracted urls to the array list
                        _a.Add(b);
                    }
                }

                var seasonAndEpisode = (string)_a[1];

                var infoslice = seasonAndEpisode.Split(',');

                var season = infoslice[0].Replace("Season", "").Trim();
                var episode = infoslice[1].Replace("Episode", "").Trim();

                if (season.Length == 1)
                    season = "0" + season;

                if (episode.Length == 1)
                    episode = "0" + episode;


                item.Tags["SeasonNumber"] = season;
                item.Tags["EpisodeNumber"] = episode;
                item.SaveTags();

            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }


        #region Miscellaneous



        private static string FixRating(string rating)
        {
            string tmp;

            if (rating.Trim() == string.Empty)
                tmp = "Unrated";
            else switch (rating.ToLower().Trim())
            {
                case "not rated":
                    tmp = "Unrated";
                    break;
                case "approved":
                    tmp = "G";
                    break;
                case "passed":
                    tmp = "G";
                    break;
                default:
                    tmp = rating;
                    break;
            }

            return tmp;
        }



 

        #endregion

    }
 

}
