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


using MeediFier.Code.Settings;
using MeediOS;



namespace MeediFier
{




    public class PropertiesSetter
    {





        internal static void SetProperties(IMeedioItem properties)
        {

            #region [DEPRECATED] Media Importers and Updaters combo

            if (properties["WantToImportAndUpdateFilmsProp"] != null)
                Settings.EnableImportAndUpdateFilmsCombo = (bool)properties["WantToImportAndUpdateFilmsProp"];

            if (properties["WantToImportAndUpdateMusicProp"] != null)
                Settings.EnabeImportAndUpdateMusicCombo = (bool)properties["WantToImportAndUpdateMusicProp"];

            if (properties["WantToImportAndUpdateTvSeriesProp"] != null)
                Settings.EnableImportAndUpdateTvSeriesCombo = (bool)properties["WantToImportAndUpdateTvSeriesProp"];

            if (properties["WantToImportAndUpdateBooksProp"] != null)
                Settings.EnableImportAndUpdateBooksCombo = (bool)properties["WantToImportAndUpdateBooksProp"];

            if (properties["WantToImportAndUpdateGamesProp"] != null)
                Settings.EnableImportAndUpdateGamesCombo = (bool)properties["WantToImportAndUpdateGamesProp"];
            
            #endregion

            if (properties["GameShortcutsProp"] != null)
                Settings.GameShortcutsLocation = (string) properties["GameShortcutsProp"];

            if (properties["WantFilesImportingProp"] != null)
                MediaImportersAndUpdatersCoreSettings.EnableMediaImporters = (bool) properties["WantFilesImportingProp"];
            if (properties["WantMediaUpdatingProp"] != null)
                MediaImportersAndUpdatersCoreSettings.EnableMediaUpdaters = (bool) properties["WantMediaUpdatingProp"];


            #region Media Sources
            //if (Properties["ImportRootFolderProp"] != null)
            //{
            //    Type type = Properties["ImportRootFolderProp"].GetType();

            //    if (type.Name != "DBNull")
            //        ImportRootFolders = (string[])Properties["ImportRootFolderProp"];
            //    else
            //    {
            //        ImportRootFolders = new string[1];
            //        ImportRootFolders[0] = "";
            //    }
            //}
            #endregion


            if (properties["MovieTitleLocatorProp"] != null)
                Settings.OverrideAutomatedMovieTitleLocator = (bool) properties["MovieTitleLocatorProp"];
            if (properties["MovieTitleLocationProp"] != null)
                Settings.MovieTitleLocationInPath = (string) properties["MovieTitleLocationProp"];

            #region Media Importing

            if (properties["WantToImportFilmsProp"] != null)
                MediaImportersAndUpdatersCoreSettings.EnableMovieImporter = (bool) properties["WantToImportFilmsProp"];
            if (properties["WantToImportMusicProp"] != null)
                MediaImportersAndUpdatersCoreSettings.EnableMusicImporter = (bool) properties["WantToImportMusicProp"];
            if (properties["WantToImportTvSeriesProp"] != null)
                MediaImportersAndUpdatersCoreSettings.EnableTvSeriesImporter = (bool) properties["WantToImportTvSeriesProp"];



            #endregion

            if (properties["WantMediaInfoProp"] != null)
                Settings.MediaInfoIsEnabled = (bool) properties["WantMediaInfoProp"];

            #region Film Details

            if (properties["PopulateIMDbDetailsProp"] != null)
                Settings.FilmIMDbDetailsDownloaderIsEnabled = (bool) properties["PopulateIMDbDetailsProp"];


            if (properties["PopulateOSdbDetailsProp"] != null)
                Settings.FilmOSDbDetailsDownloaderIsEnabled = (bool) properties["PopulateOSdbDetailsProp"];

            //if (properties["PopulateAMGDetailsProp"] != null)
            //    Settings.AllMediaGuideFilmDetailsEnabled 
            //        = (bool) properties["PopulateAMGDetailsProp"];


            if (properties["PopulateTMDbDetailsProp"] != null)
                Settings.TMDbFilmDetailsEnabled = (bool) properties["PopulateTMDbDetailsProp"];

            //if (properties["PopulateSevenVideoNetDetailsProp"] != null)
            //    Settings.SevenFilmDetailsEnabled = (bool) properties["PopulateSevenVideoNetDetailsProp"];

            #endregion

            #region Film Posters

            if (properties["DownloadPosterChoice"] != null)
                Settings.FilmCoverArtDownloaderIsEnabled = (bool) properties["DownloadPosterChoice"];
            if (properties["PrimaryPosterSourceChoice"] != null)
                Settings.PosterPrimarySource = (string) properties["PrimaryPosterSourceChoice"];
            //if (Properties["OverwritePosterChoice"] != null) OverwriteExistingPosters = (bool)Properties["OverwritePosterChoice"];
            if (properties["PosterSizeChoice"] != null)
                Settings.PosterSize = (string) properties["PosterSizeChoice"];
            //if (Properties["WhereToSavePosterProp"] != null) WhereToSavePoster = (string)Properties["WhereToSavePosterProp"];
            //if (Properties["PosterFolderProp"] != null) PosterFolder = (string)Properties["PosterFolderProp"];

            //if (Properties["PosterNamingChoice"] != null)
            //{
            //    if (WhereToSavePoster == "in the folder i specify below")
            //    {
            //        PosterNamingMethod = "iMDbID";
            //    }
            //    else
            //    PosterNamingMethod = (string)Properties["PosterNamingChoice"];

            //}


            if (properties["ResizeFilmPostersProp"] != null)
                Settings.ResizeFilmPosters = (bool) properties["ResizeFilmPostersProp"];
            if (properties["FilmPosterWidthProp"] != null)
                Settings.FilmPosterWidth = (int) properties["FilmPosterWidthProp"];
            if (properties["FilmPosterHeightProp"] != null)
                Settings.FilmPosterHeight = (int) properties["FilmPosterHeightProp"];

            #endregion

            #region Film Backdrops

            if (properties["DownloadBackdropsChoice"] != null)
                Settings.FilmBackdropArtDownloaderIsEnabled = (bool) properties["DownloadBackdropsChoice"];
            //if (Properties["WhereToSaveBackdropProp"] != null) WhereToSaveBackdrop = (string)Properties["WhereToSaveBackdropProp"];
            //if (Properties["BackdropFolderProp"] != null) BackdropFolder = (string)Properties["BackdropFolderProp"];
            //if (Properties["BackdropNamingChoice"] != null) BackdropNamingMethod = (string)Properties["BackdropNamingChoice"];

            if (properties["ResizeFilmBackdropsProp"] != null)
                Settings.ResizeFilmBackdrops = (bool) properties["ResizeFilmBackdropsProp"];
            if (properties["FilmBackdropWidthProp"] != null)
                Settings.FilmBackdropWidth = (int) properties["FilmBackdropWidthProp"];
            if (properties["FilmBackdropHeightProp"] != null)
                Settings.FilmBackdropHeight = (int) properties["FilmBackdropHeightProp"];

            #endregion


            #region Subtitles

            if (properties["DownloadSubtitlesChoice"] != null)
                Settings.EnableSubtitleDownloader = (bool)properties["DownloadSubtitlesChoice"];

            if (properties["LanguageProp"] != null) Settings.PrimarySubtitleLanguage = (string)properties["LanguageProp"];
            
            if (properties["LanguagePropSecondary"] != null)
                Settings.SecondaryLanguage = (string)properties["LanguagePropSecondary"];

            if (properties["WhenToUseSecondaryLanguageProp"] != null)
                Settings.WhenToUseSecondaryLanguage = (string)properties["WhenToUseSecondaryLanguageProp"];

            if (properties["SameSubsFolderProp"] != null)
                Settings.UseSameSubtitlesFolder = (bool)properties["SameSubsFolderProp"];

            #endregion


            if (properties["ConnectionDiagnosticsProp"] != null)
                Settings.ConnectionDiagnosticsEnabled = (bool) properties["ConnectionDiagnosticsProp"];
            if (properties["DisableSourcesProp"] != null)
                Settings.WantToDisableSources = (bool) properties["DisableSourcesProp"];

            if (properties["FileserverDiagnosticsProp"] != null)
                Settings.WantFileserverDiagnostics = (bool) properties["FileserverDiagnosticsProp"];


            if (properties["WantHashingProp"] != null)
                Settings.EnableVideoHasher = (bool) properties["WantHashingProp"];
            if (properties["PrimaryVideoFingerprintMatchingSourceProp"] != null)
                Settings.PrimaryVideoFingerprintMatchingSource =
                    (string) properties["PrimaryVideoFingerprintMatchingSourceProp"];








            #region Library Cleaners

            if (properties["DeleteMissingProp"] != null)
                Settings.DeleteMissing = (bool) properties["DeleteMissingProp"];
            if (properties["DeleteCoverImageOfMissingMediaFilesProp"] != null)
                Settings.DeleteCoverImageOfMissingMediaFiles =
                    (bool) properties["DeleteCoverImageOfMissingMediaFilesProp"];
            if (properties["DeleteFanartImageOfMissingMediaFilesProp"] != null)
                Settings.DeleteFanartImageOfMissingMediaFiles =
                    (bool) properties["DeleteFanartImageOfMissingMediaFilesProp"];


            if (properties["ClearEmptyFieldsProp"] != null)
                Settings.CleanEmptyFields = (bool) properties["ClearEmptyFieldsProp"];
            if (properties["CleanSelectedFieldsProp"] != null)
                Settings.CleanSelectedFields = (bool) properties["CleanSelectedFieldsProp"];
            if (properties["SelectedFieldsToCleanProp"] != null)
                Settings.SelectedFieldsToClean = (string) properties["SelectedFieldsToCleanProp"];

            #endregion




            if (properties["EnableChainLinkingProp"] != null)
                Settings.FilmItemChainLinkerIsEnabled = (bool) properties["EnableChainLinkingProp"];
            if (properties["ExtractMetadataFromFilenamesProp"] != null)
                Settings.ExtractMetadataFromFilenames = (bool) properties["ExtractMetadataFromFilenamesProp"];





            #region Media Organizers

            if (properties["WantSortingFilmsProp"] != null)
                Settings.WantSortingFilms = (bool) properties["WantSortingFilmsProp"];
            if (properties["WantSortingTvShowsProp"] != null)
                Settings.WantSortingTvShows = (bool) properties["WantSortingTvShowsProp"];
            if (properties["WantSortingMusicProp"] != null)
                Settings.WantSortingMusic = (bool) properties["WantSortingMusicProp"];

            if (properties["PromptBeforeMovingProp"] != null)
                Settings.PromptBeforeMoving = (bool) properties["PromptBeforeMovingProp"];
            if (properties["OnlyTaggedForMovingProp"] != null)
                Settings.OnlyTaggedForMoving = (bool) properties["OnlyTaggedForMovingProp"];

            if (properties["SortingDestinationFilmsProp"] != null)
                Settings.SortingDestinationFilms = (string) properties["SortingDestinationFilmsProp"];
            if (properties["SortingDestinationTvShowsProp"] != null)
                Settings.SortingDestinationTvShows = (string) properties["SortingDestinationTvShowsProp"];
            if (properties["SortingDestinationMusicProp"] != null)
                Settings.SortingDestinationMusic = (string) properties["SortingDestinationMusicProp"];

            if (properties["DirectoryTagMaskFilmsProp"] != null)
                Settings.DirectoryTagMaskFilms = (string) properties["DirectoryTagMaskFilmsProp"];
            if (properties["DirectoryTagMaskTvShowsProp"] != null)
                Settings.DirectoryTagMaskTvShows = (string) properties["DirectoryTagMaskTvShowsProp"];
            if (properties["DirectoryTagMaskMusicProp"] != null)
                Settings.DirectoryTagMaskMusic = (string) properties["DirectoryTagMaskMusicProp"];

            if (properties["DirectoryStructureProp"] != null)
                Settings.DirectoryStructure = (string) properties["DirectoryStructureProp"];

            #endregion




            if (properties["AssistModeProp"] != null) Settings.AssistType = (string) properties["AssistModeProp"];
            if (properties["DurationMatchingProp"] != null)
                Settings.EnableDurationMatching = (bool) properties["DurationMatchingProp"];


            //if (Properties["LimitFilesToImportProp"] != null) importing_limit = (int)Properties["LimitFilesToImportProp"];

            #region Media Sections

            if (properties["FilmsLibraryProp"] != null)
                Settings.FilmsLibrary = (string) properties["FilmsLibraryProp"];


            if (properties["TvShowsLibraryProp"] != null)
                Settings.TvShowsLibrary = (string) properties["TvShowsLibraryProp"];


            if (properties["TvSeriesLibraryProp"] != null)
                Settings.MusicLibrary = (string) properties["MusicLibraryProp"];


            if (properties["NewsLibraryProp"] != null)
                Settings.NewsLibrary = (string) properties["NewsLibraryProp"];



            #endregion





            //if (properties["WantMusicDNSProp"] != null)
            //    Settings.MusicDNSAudioTrackIdentifierIsEnabled
            //        = (bool) properties["WantMusicDNSProp"];




            if (properties["WantAudioTagsProp"] != null)
                Settings.WantAudioTags 
                    = (bool) properties["WantAudioTagsProp"];



            //if (properties["SaveMetadataToAudioTagsProp"] != null)
            //    Settings.SaveMetadataToAudioTags
            //        = (bool) properties["SaveMetadataToAudioTagsProp"];



            #region Media Updaters

            if (properties["UpdateMoviesProp"] != null)
                Settings.EnableMoviesSectionUpdater = (bool) properties["UpdateMoviesProp"];

            if (properties["UpdateTvShowsProp"] != null)
                Settings.EnableTvShowsSectionUpdater = (bool) properties["UpdateTvShowsProp"];

            if (properties["UpdateMusicProp"] != null)
                Settings.EnableMusicSectionUpdater = (bool) properties["UpdateMusicProp"];


            #endregion



            if (properties["EnableUIProp"] != null)
                Settings.AlwaysPromptForImages
                    = (bool) properties["EnableUIProp"];

            if (properties["DebugLogProp"] != null) Settings.WriteDebugLog
                = (bool) properties["DebugLogProp"];



            if (properties["EnableNfoParsingProp"] != null)
                Settings.EnableNfoParsing = (bool) properties["EnableNfoParsingProp"];
            if (properties["WantMatchingProp"] != null)
                Settings.FilmTitleMatchingEnginesAreEnabled = (bool) properties["WantMatchingProp"];



            if (properties["PrimaryFilmIdentificationMethodProp"] != null)
                Settings.PrimaryFilmIdentificationMethod =
                    (string) properties["PrimaryFilmIdentificationMethodProp"];


            #region Film Title Matching Engines

            if (properties["PrimaryFilmTitleMatchingEngineProp"] != null)
                Settings.PrimaryFilmTitleMatchingEngine
                    = (string)properties["PrimaryFilmTitleMatchingEngineProp"];


            if (properties["PrimaryImdbFilmTitleMatchingEngineProp"] != null)
                Settings.PrimaryIMDbFilmTitleMatchingEngine 
                    = (string) properties["PrimaryImdbFilmTitleMatchingEngineProp"];

            #endregion


            if (properties["WriteFilmDescriptorsProp"] != null)
                Settings.FilmDescriptorsAreEnabled = (bool) properties["WriteFilmDescriptorsProp"];
            if (properties["FilmDescriptorConventionProp"] != null)
                Settings.FilmDescriptorConvention = (string) properties["FilmDescriptorConventionProp"];







            if (properties["SimpleWantFilmDetailsProp"] != null)
                Settings.SimpleFilmDetailsDownloaderIsEnabled = (bool) properties["SimpleWantFilmDetailsProp"];

            if (Settings.SimpleFilmDetailsDownloaderIsEnabled)
            {
                if (!Settings.FilmIMDbDetailsDownloaderIsEnabled && !Settings.FilmOSDbDetailsDownloaderIsEnabled)
                {
                    Settings.FilmIMDbDetailsDownloaderIsEnabled = true;
                    Settings.FilmOSDbDetailsDownloaderIsEnabled = true;
                }
            }
            if (!Settings.SimpleFilmDetailsDownloaderIsEnabled)
            {
                if (Settings.FilmIMDbDetailsDownloaderIsEnabled || Settings.FilmOSDbDetailsDownloaderIsEnabled)
                {
                    Settings.FilmIMDbDetailsDownloaderIsEnabled = false;
                    Settings.FilmOSDbDetailsDownloaderIsEnabled = false;
                }
            }


            if (properties["WantAlbumCoversProp"] != null)
                Settings.WantAlbumCovers = (bool) properties["WantAlbumCoversProp"];



            //if (properties["WantArtistBackdropsProp"] != null)
            //    Settings.ArtistBackdropsEnabled = (bool) properties["WantArtistBackdropsProp"];
           


            //if (Properties["EnableMediaDetectionProp"] != null) EnableMediaDetection = (bool)Properties["EnableMediaDetectionProp"];
            //if (Properties["FilmsFolderProp"] != null) FilmsFolders = (string[])Properties["FilmsFolderProp"];
            //if (Properties["TvShowsFolderProp"] != null) TvShowsFolders = (string[])Properties["TvShowsFolderProp"];



            if (properties["RescanFilesNotFoundInLibraryProp"] != null)
                Settings.RescanFilesNotFoundInLibrary = (bool) properties["RescanFilesNotFoundInLibraryProp"];




            if (properties["EmbedAlbumCoverProp"] != null)
                Settings.EmbedAlbumCover = (bool) properties["EmbedAlbumCoverProp"];



            #region TV Series Details and Images

            if (properties["WantEpisodeDetailsProp"] != null)
                Settings.WantEpisodeDetails = (bool) properties["WantEpisodeDetailsProp"];

            if (properties["WantEpisodeThumbnailsProp"] != null)
                Settings.WantEpisodeThumbnails = (bool) properties["WantEpisodeThumbnailsProp"];
            
            if (properties["WantSeriesBannersProp"] != null)
                Settings.WantSeriesBanners = (bool) properties["WantSeriesBannersProp"];
            
            if (properties["WantSeriesBackdropsProp"] != null)
                Settings.WantSeriesBackdrops = (bool) properties["WantSeriesBackdropsProp"];
            
            if (properties["WantSeasonImagesProp"] != null)
                Settings.WantSeasonImages = (bool) properties["WantSeasonImagesProp"];

            #endregion



            if (properties["ImportDateCreatedProp"] != null)
                Settings.ImportDateCreated = (bool) properties["ImportDateCreatedProp"];
            if (properties["ImportDateModifiedProp"] != null)
                Settings.ImportDateModified = (bool) properties["ImportDateModifiedProp"];
            if (properties["ImportDateImportedProp"] != null)
                Settings.ImportDateImported = (bool) properties["ImportDateImportedProp"];



            if (properties["TMDbPostersEnabledProp"] != null)
                Settings.TMDbPostersEnabled = (bool) properties["TMDbPostersEnabledProp"];
            if (properties["IMDbPostersEnabledProp"] != null)
                Settings.IMDbPostersEnabled = (bool) properties["IMDbPostersEnabledProp"];
            if (properties["ImpAwardsPostersEnabledProp"] != null)
                Settings.ImpAwardsPostersEnabled = (bool) properties["ImpAwardsPostersEnabledProp"];

            //Compatibility setting for MovieNight for Meedio.
            if (properties["MovieNightCompatibleProp"] != null)
                Settings.MovieNightCompatibility 
                    = (bool) properties["MovieNightCompatibleProp"];

            #region Film Optical Discs Importers

            if (properties["ImportDvdFoldersProp"] != null)
                Settings.ImportDvdFolders = (bool) properties["ImportDvdFoldersProp"];
            if (properties["ImportDvdImagesProp"] != null)
                Settings.ImportDvdImages = (bool) properties["ImportDvdImagesProp"];
            if (properties["ImportBluRayImagesProp"] != null)
                Settings.ImportBluRayImages = (bool) properties["ImportBluRayImagesProp"];

            #endregion
            if (properties["ImportRarFilmsProp"] != null)
                Settings.ImportFilmsInRarArchives = (bool) properties["ImportRarFilmsProp"];






            if (properties["WantToImportNewsProp"] != null)
                Settings.EnableRssImporter = (bool) properties["WantToImportNewsProp"];
            if (properties["RssFeedsProp"] != null) Settings.RsSfeeds = (string[]) properties["RssFeedsProp"];
            if (properties["DownloadFeedImagesProp"] != null)
                Settings.DownloadRsSimages = (bool) properties["DownloadFeedImagesProp"];
            if (properties["RSSimagesLocationProp"] != null)
                Settings.RSSimagesLocation = (string) properties["RSSimagesLocationProp"];


            #region Offline Media Mode
            //if (properties["OfflineModeProp"] != null)
            //    Settings.OfflineMediaMode = (bool)properties["OfflineModeProp"];

            //if (properties["IdentifyDVDfilmsProp"] != null)
            //    Settings.DvdMovieOptions = (bool)properties["IdentifyDVDfilmsProp"];

            //if (properties["IdentifyDVDseriesProp"] != null)
            //    Settings.DVDTVOptions = (bool)properties["IdentifyDVDseriesProp"];

            //if (properties["IdentifyBRfilmsProp"] != null)
            //    Settings.BRMovieOptions = (bool)properties["IdentifyBRfilmsProp"];

            //if (properties["IdentifyBRseriesProp"] != null)
            //    Settings.BRTVOptions = (bool)properties["IdentifyBRseriesProp"];

            //if (properties["DriveProp"] != null) Settings.DvdDrive = (string)properties["DriveProp"];

            //if (properties["EjectDriveProp"] != null) Settings.EjectDvd = (bool)properties["EjectDriveProp"];
            #endregion


            if (properties["MeePopupProviderCompatibilityProp"] != null)
                Settings.MeePopupProviderCompatibility = (bool) properties["MeePopupProviderCompatibilityProp"];
        }
    }
}