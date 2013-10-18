//'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
//'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
//''    MediaFairy                                                               ''
//''    Copyright (C) 2008-2012  Stavros Skamagkis                               ''
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


namespace MeediFier
{



    internal class Settings
    {





        #region default properties




 




        #region Section Names
        public static string FilmsLibrary = "Movies";
        public static string TvShowsLibrary = "TvEpisodes";
        public static string MusicLibrary = "Music";

        public static string NewsLibrary = "RSS";

        #endregion




        #region Video Identifier

        public static bool EnableNfoParsing = true;
        public static bool EnableVideoHasher = true;


        public static bool WantMediaInfo = true;


        public static bool FilmDescriptorsAreEnabled = true;
        #endregion




        #region Audio Identifier


        //public static bool MusicDNSAudioTrackIdentifierIsEnabled = false;
        


        public static bool WantAudioTags = true;

        //public static bool SaveMetadataToAudioTags = true;


        public static bool OverwriteExistingMusicTags = false;
        
        public static bool SkipLongTracks = true;
        
        public static bool OnlyIdFilesWithMissingInfo = true;


        #endregion




        #region Media Updaters
        public static bool EnableMoviesSectionUpdater;
        public static bool EnableTvShowsSectionUpdater;
        public static bool EnableMusicSectionUpdater;
        #endregion





        #region Film Details
        public static bool FilmOSDbDetailsDownloaderIsEnabled = true;
        public static bool FilmIMDbDetailsDownloaderIsEnabled = true;
        #endregion




        #region Film Posters
        public static bool FilmCoverArtDownloaderIsEnabled = true;
        public static string PosterPrimarySource = "TheMovieDatabase";
        public static string PosterSize = "Medium";
        #endregion




        #region Film Backdrops
        public static bool FilmBackdropArtDownloaderIsEnabled = false;
        //public static string BackdropNamingMethod = "<IMDbID>_backdrop";
        //public static string WhereToSaveBackdrop = "in the movie's folder";
        //public static string BackdropFolder = " ";
        //public static bool OverwriteExistingBackdrops = false;
        #endregion




        #region Album Covers
        public static string WhereToSaveAlbumCover = "in the album's folder";
        public static string AlbumCoverFolder = "";
        public static bool EmbedAlbumCover = false;
        #endregion




        #region Diagnostics
        public static bool ConnectionDiagnosticsEnabled = false;
        public static bool WantOSdbDiagnostics = true;
        public static bool WantFileserverDiagnostics = true;
        #endregion



        #region Interoperability
        public static int SleepValue = 3000;
        #endregion





        #region Media Organizers
        public static bool WantSortingFilms = false;
        public static bool WantSortingTvShows = false;
        public static bool WantSortingMusic = false;
        public static bool OnlyTaggedForMoving = false;
        public static bool PromptBeforeMoving = false;
        public static string SortingDestinationFilms = "";
        public static string SortingDestinationTvShows = "";
        public static string SortingDestinationMusic = "";
        public static string DirectoryStructure = "Simple- Default directory structures";
        public static string DirectoryTagMaskFilms = "<Title>";
        public static string DirectoryTagMaskTvShows = @"<SeriesName>\<SeasonNumber>";
        public static string DirectoryTagMaskMusic = @"<Artist>\<Album>";
        #endregion




        #region Library Cleanup Settings
        public static bool FilmItemChainLinkerIsEnabled;
        public static bool DeleteMissing = true;
        public static bool CleanEmptyFields = true;
        public static bool CleanSelectedFields;
        public static string SelectedFieldsToClean = " ";
        #endregion




        #region IMDb title matcher
        public static string AssistType 
            = "When no exact match found";

        public static bool UseFirst;
        public static bool UseLatest;
        public static bool UseMostPopular = true;
        public static bool EnableNameMatching = true;

        public static bool NotListed;
        public static bool DeleteItem;
        #endregion



        #region Current Session Variables

        public static bool UserCancels;
        public static bool ImportingStarted;
        public static bool ImportingCompleted;
        public static bool AlwaysPromptForImages;
        public static bool WriteDebugLog = true;





        public static bool FileInUse;
        public static int KBytesPerSec;
        public static bool SubDloadLimitExceeded;
        #endregion




        #region Simple Settings
        public static bool SimpleFilmDetailsDownloaderIsEnabled = true;
        public static bool SimpleWantFilmPosters = true;


        public static bool WantAlbumCovers = true;
        #endregion








        #region TV Series Updater
        public static bool WantEpisodeDetails = true;
        public static bool WantEpisodeThumbnails = true;
        public static bool WantSeriesBanners = true;
        public static bool WantSeriesBackdrops = true;
        public static bool WantSeasonImages = true;

        #endregion


        public static bool AnalyzeAudio = true;



        #region Miscelaneous
        public static bool EstimateAudioIdentification = true;




        public static bool UIenabled;
        public static bool WantToDisableSources = true;
        public static string[] ImporterIDs = new string[1];
        public static int importing_limit;
        public static string username = string.Empty;
        public static string Password = string.Empty;


        #endregion



        public static bool EnableDurationMatching;


        public static bool OverrideAutomatedMovieTitleLocator;

        public static string MovieTitleLocationInPath = "Parent folder";






        public static bool ImportDateCreated;
        public static bool ImportDateModified;
        public static bool ImportDateImported;
        public static bool TimeInCollectionEnabled;


        public static bool TMDbPostersEnabled = true;
        public static bool IMDbPostersEnabled = true;
        public static bool ImpAwardsPostersEnabled;


        public static bool TMDbFilmDetailsEnabled = true;
        //public static bool AllMediaGuideFilmDetailsEnabled;
        public static bool NetFlixDetailsEnabled;



        #region NetFlix film metadata
        public static bool NetFlixTitleEnabled = true;
        public static bool NetFlixReleaseYearEnabled = true;
        public static bool NetFlixDirectorsEnabled = true;
        public static bool NetFlixActorsEnabled = true;
        public static bool NetFlixGenresEnabled = true;
        public static bool NetFlixRatingEnabled = true;
        public static bool NetFlixRuntimeEnabled = true;
        public static bool NetFlixHomepageEnabled = true;
        public static bool NetFlixLongOverviewEnabled = true;
        public static bool NetFlixAwardsEnabled = true;
        #endregion

        public static string TMDbApiKey = "1fbf47739c264837f6b73b6431e92ae0";

        // ReSharper disable InconsistentNaming
        public static bool IMDbIsOnline = true;
        public static bool TMDbIsOnline = true;
        // ReSharper restore InconsistentNaming
        public static bool ImpAwardsIsOnline = true;

        public static bool ExtractMetadataFromFilenames = true;

        public static bool RescanFilesNotFoundInLibrary = true;



        public static bool MovieNightCompatibility;


        public static bool EnableImportAndUpdateFilmsCombo;
        public static bool EnableImportAndUpdateTvSeriesCombo;
        public static bool EnabeImportAndUpdateMusicCombo;
        public static bool EnableImportAndUpdateGamesCombo;
        public static bool EnableImportAndUpdateBooksCombo;
        





        #region Disc Images
        public static bool ImportDvdFolders = true;
        public static bool ImportDvdImages = true;
        public static bool ImportBluRayImages = true;
        #endregion



        internal static bool ImportFilmsInRarArchives;
        //public static bool OverwriteExistingAlbumCovers = false;


        public static string FilmDescriptorConvention = "MediaFairy";

        public static bool TVDbIsOnline;





        #region RSS News
        public static bool EnableRssImporter;
        public static bool DownloadRsSimages = true;
        public static string[] RsSfeeds;
        public static string RSSimagesLocation;
        #endregion


        //public static bool SevenFilmDetailsEnabled;

        public static string GameShortcutsLocation = string.Empty;




        //public static bool ArtistBackdropsEnabled = true;
        //public static string ArtistBackdropsLocation = "";

        #region Image Resizer
        public static bool ResizeFilmPosters;
        public static bool ResizeFilmBackdrops;

        public static int FilmPosterWidth = 800;
        public static int FilmPosterHeight = 1000;

        public static int FilmBackdropWidth = 1360;
        public static int FilmBackdropHeight = 768;

        #endregion


        public static bool MeePopupProviderCompatibility = true;


        public static string PrimaryFilmIdentificationMethod = "by video fingerprint";
        public static string PrimaryFilmTitleMatchingEngine = "Internal";
        public static string PrimaryVideoFingerprintMatchingSource = "TMDb";
        
        #endregion


        #region Metadata deleter of missing media files
        public static bool DeleteCoverImageOfMissingMediaFiles;
        public static bool DeleteFanartImageOfMissingMediaFiles;
        #endregion






        #region Media Type Detector

        public static bool EnableMediaDetection = true;
        public static bool EnableMediaSnapshots = true;

        public static string[] FilmsFolders = new string[1];
        public static string[] TvShowsFolders = new string[1];
        public static string[] MusicFolders = new string[1];


        public static string NowImportingMediaType = string.Empty;
        #endregion


        public static bool UpdateMediaSectionOnEachImportedItem = true;




    }



}
