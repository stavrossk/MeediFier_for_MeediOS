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
using System.Windows.Forms;
using CustomProperties;
using CustomProperties.PropertyData;
using MeediFier.Code.Importer_Properties;
using MeediFier.Code.Settings;
using MeediFier.ImporterPropertiesGroups;
using MeediOS;
using MeediOS.Cache;



namespace MeediFier
{


    internal class ImporterProperties : CustomProperty
    {







        public override bool GetProperty(Int32 index, OptionSettings prop)
        {
            #region init vars
            int counter = 1;

            string[] threechoices = new string[3];
            string[] twochoices = new string[2];
            string[] fourchoices = new string[4];


            #endregion


            #region initialize Sections

            #region Create Media Library class to access media sections
            
                #if USE_MEEDIO
                IMeedioLibrary MeedioLibrary = new MeedioLibrary.MediaLibrary();
                #elif USE_MEEDIOS
                IMeedioLibrary meedioLibrary = new MediaLibraryClass();
                #endif

            #endregion


            #region Create sectionChoices array and populate it with the names of the Media Sections

            string[] sectionChoices = new string[meedioLibrary.SectionCount + 1];
            sectionChoices[0] = string.Empty;

            for (int i = 1; i < meedioLibrary.SectionCount + 1; i++)
            {
                #if USE_MEEDIO
                SectionChoices[i] = MeedioLibrary.get_Sections(i-1);
                #elif USE_MEEDIOS
                sectionChoices[i] = meedioLibrary.Sections(i-1);
                #endif
            }

            #endregion

            #endregion


            try
            {


                if (ConfigurationMode
                    .ConfigurationModeProperty
                    (this, index, 
                    prop, ref counter)) 
                    return true;



                if (MediaSections
                    .MediaSectionsProperties
                    (index, prop, 
                    sectionChoices,
                    ref counter)) 
                    return true;




                //DEPRECATED: Import and Update Media Types Combo Settings
                //if (ImportAndUpdateMediaTypesCombo.ImportAndUpdateMediaTypesComboProperties(index, prop, ref counter)) 
                //    eturn true;





                if (ImporterPropertiesGroups
                    .MediaImporters
                    .MediaImportersProperties
                    (this, index, prop, 
                    twochoices, ref counter))
                    return true;





                #region Media Updaters
                if (index == counter++)
                {
                    //set the internal name
                    prop.Name = "WantMediaUpdatingProp";
                    prop.GroupName = "MediaUpdaters";
                    //set name shown to user
                    prop.Caption = "Enable Media Updaters ";
                    prop.Caption = TranslationProvider.Translate(prop.Caption, this);
                    prop.GroupCaption = "                                Media Updaters ";
                    //set the tool tip
                    prop.HelpText = "The Media Updaters scan the media sections you provide and automatically" + Environment.NewLine + 
                                    "identify, download details, cover images and other online data" + Environment.NewLine + 
                                    "for movies, tv series, audio tracks, books and/or games contained in these sections.";
                    prop.DefaultValue = MediaImportersAndUpdatersCoreSettings.EnableMediaUpdaters;
                    prop.DataType = "bool";
                    prop.Dependencies =

                        "False:HIDE"
                        +
                        " UpdateMoviesProp,"
                        +
                        "UpdateTvShowsProp,"
                        +
                        "UpdateMusicProp"


                        +


                        ";False:HIDEGROUP"
                        +
                        " FilmUpdater,"
                        +
                        "TvUpdater,"
                        +
                        "MusicUpdater,"
                        +
                        "FilmDetails,"
                        +
                        "FilmCovers,"
                        +
                        "AlbumCovers,"
                        +
                        "VideoIdentifier"

                        +

                        ";True:SHOW"
                        +
                        " UpdateMoviesProp,"
                        +
                        "UpdateTvShowsProp,"
                        +
                        "UpdateMusicProp";



                    return true;
                }

                if (index == counter++)
                {
                    //set the internal name
                    prop.Name = "UpdateMoviesProp";
                    prop.GroupName = "MediaUpdaters";
                    //set name shown to user
                    prop.Caption = "Update Films section";
                    prop.Caption = TranslationProvider.Translate(prop.Caption, this);
                    //set the tool tip
                    prop.HelpText = "Do you want MediaFairy to work with the films in your Movies library?";
                    prop.DefaultValue = Settings.EnableMoviesSectionUpdater;
                    prop.DataType = "bool";
                    prop.Dependencies =

                                        "False:HIDEGROUP "
                                        +
                                        "VideoIdentifier,"
                                        +
                                        "FilmUpdater,"
                                        +
                                        "FilmDetails,"
                                        +
                                        "FilmCovers"
                                        +
                                        ";True:SHOWGROUP "
                                        +
                                        "VideoIdentifier,"
                                        +
                                        "FilmUpdater";


                    return true;
                }


                #region Video Identifier




                if (index == counter++)
                {
                    //set the internal name
                    prop.Name = "PrimaryFilmIdentificationMethodProp";
                    //set name shown to user
                    prop.Caption = "Primary film identification method: ";
                    prop.Caption = TranslationProvider.Translate(prop.Caption, this);
                    prop.GroupName = "VideoIdentifier";
                    //set the tool tip
                    prop.HelpText = "Please choose the order of the film identification methods." + Environment.NewLine +
                                    "When/if the primary method of identification fails, MediaFairy will resort to using the secondary method instead.";
                    prop.DefaultValue = Settings.PrimaryFilmIdentificationMethod;
                    prop.DataType = "string";

                    twochoices[0] = "by video fingerprint";
                    twochoices[1] = "by video title";
                    prop.Choices = twochoices;
                    prop.CanTypeChoices = false;
                    return true;
                }




                if (index == counter++)
                {
                    //set the internal name
                    prop.Name = "PrimaryVideoFingerprintMatchingSourceProp";
                    //set name shown to user
                    prop.Caption = "Primary video fingerprint matching source: ";
                    prop.Caption = TranslationProvider.Translate(prop.Caption, this);
                    prop.GroupName = "VideoIdentifier";
                    //set the tool tip
                    prop.HelpText = "Please choose the priority of video fingerprint matching sources MediaFairy should use to identify a video by its fingerprint." + Environment.NewLine +
                                    "If/when your defined primary source fails, MediaFairy will resort to using the second source in the priority.";
                    prop.DefaultValue = Settings.PrimaryVideoFingerprintMatchingSource;
                    prop.DataType = "string";

                    twochoices[0] = "OSDb";
                    twochoices[1] = "TMDb";
                    prop.Choices = twochoices;
                    prop.CanTypeChoices = false;

                    return true;
                }






                #region IMDb Title matching engines priority
                if (index == counter++)
                {
                    //set the internal name
                    prop.Name = "PrimaryFilmTitleMatchingEngineProp";
                    //set name shown to user
                    prop.Caption = "Primary film title matching engine: ";
                    prop.Caption = TranslationProvider.Translate(prop.Caption, this);
                    prop.GroupName = "VideoIdentifier";
                    //set the tool tip

                    prop.HelpText = "Please choose the priority of film Title matching engines" +
                                    " MeediFier should use when identifying your films by Title.";

                    prop.DefaultValue = Settings.PrimaryFilmTitleMatchingEngine;

                    prop.DataType = "string";

                    twochoices[0] = "TMDb";

                    twochoices[1] = "IMDb";

                    prop.Choices = twochoices;

                    prop.CanTypeChoices = false;

                    return true;
                }
                #endregion












                #region IMDb Title matching engines priority
                if (index == counter++)
                {
                    //set the internal name
                    prop.Name = "PrimaryImdbFilmTitleMatchingEngineProp";
                    //set name shown to user
                    prop.Caption = "Primary IMDb Title matching engine: ";
                    prop.Caption = TranslationProvider.Translate(prop.Caption, this);
                    prop.GroupName = "VideoIdentifier";
                    //set the tool tip
                    prop.HelpText = "Please choose the priority of IMDb Title matching engines MediaFairy should use when identifying your films by Title." + Environment.NewLine +
                                    "While the internal engine allows you to choose from multiple IMDb results," + Environment.NewLine +
                                    "the external (web-based) engine will always choose the best match automatically.";
                    prop.DefaultValue = Settings.PrimaryIMDbFilmTitleMatchingEngine;
                    prop.DataType = "string";

                    twochoices[0] = "Internal";
                    twochoices[1] = "External (web-based)";
                    prop.Choices = twochoices;
                    prop.CanTypeChoices = false;
                    return true;
                }
                #endregion




                if (index == counter++)
                {
                    //set the internal name
                    prop.Name = "AssistModeProp";
                    //set name shown to user
                    prop.Caption = "IMDb film title matching assistance: ";
                    prop.Caption = TranslationProvider.Translate(prop.Caption, this);
                    prop.GroupName = "VideoIdentifier";
                    //set the tool tip
                    prop.HelpText = "Please choose the Film Matching Assist mode." + Environment.NewLine +
                                    "This setting defines in which case you 'll be asked to select the correct film from a list of possible IMDb matches." + Environment.NewLine +
                                    "Leave this setting to it's default for unsupervised operation (but with a small probability for incorrect matches when identifying by fingerprint is not possible)  ?";
                    prop.DefaultValue = Settings.AssistType;
                    prop.DataType = "string";

                    fourchoices[0] = "When no exact match found";
                    fourchoices[1] = "Always";
                    fourchoices[2] = "Never";
                    fourchoices[3] = "Skip identifying unknown films";
                    prop.Choices = fourchoices;
                    prop.CanTypeChoices = false;
                    return true;
                }






                if (index == counter++)
                {
                    //set the internal name
                    prop.Name = "WantHashingProp";
                    //set name shown to user
                    prop.GroupCaption = "                      Video Identifier Settings";
                    prop.GroupName = "VideoIdentifier";
                    prop.Caption = "Enable video fingerprint identifier? ";
                    prop.Caption = TranslationProvider.Translate(prop.Caption, this);
                    //set the tool tip
                    prop.HelpText = "Video fingerprint identifying is the method MediaFairy mainly uses in order to identify your films." + Environment.NewLine +
                                    "The Fingerprint Identifying mechanism computes a unique number (fingerprint) for each of your films" + Environment.NewLine + 
                                    "and then uses the available databases to match this fingerprint with the film's unique id (IMDb ID)." + Environment.NewLine +
                                    "When a match is not found using this fingerprint, by default MediaFairy will switch to the IMDb Title matching method below. ";
                    prop.DefaultValue = Settings.EnableVideoHasher;
                    prop.DataType = "bool";
                    prop.Dependencies = "False:HIDE PrimaryVideoFingerprintMatchingSourceProp" +
                                        ";True:SHOW PrimaryVideoFingerprintMatchingSourceProp";
                    return true;
                }










                if (index == counter++)
                {
                    //set the internal name
                    prop.Name = "EnableNfoParsingProp";
                    //set name shown to user
                    prop.Caption = "Enable .nfo file parser";
                    prop.Caption = TranslationProvider.Translate(prop.Caption, this);
                    prop.GroupName = "VideoIdentifier";
                    //set the tool tip
                    prop.HelpText = "When this option is enabled, the video identifier will scan the film's folder for an .nfo file" + Environment.NewLine + 
                                    "which contains useful information about the film for instant and exact film identification. ";
                    prop.DefaultValue = Settings.EnableNfoParsing;
                    prop.DataType = "bool";
                    return true;
                }


                #region IMDb Title matcher
                if (index == counter++)
                {
                    //set the internal name
                    prop.Name = "WantMatchingProp";
                    //set name shown to user
                    prop.Caption = "Enable IMDb Title matcher";
                    prop.Caption = TranslationProvider.Translate(prop.Caption, this);
                    prop.GroupName = "VideoIdentifier";
                    //set the tool tip
                    prop.HelpText = "If the video fingerprinting mechanism is disabled or unable to identify a film by it's fingerprint," + Environment.NewLine + 
                                    "MediaFairy can resort to recognize the film using this method." + Environment.NewLine + 
                                    "The Title matching method will use the film information" + Environment.NewLine +
                                    "already provided in 'name', 'Title' and 'Year' library fields" + Environment.NewLine + 
                                    "to recognize a film and download it's related information. ";
                    prop.DefaultValue = Settings.FilmTitleMatchingEnginesAreEnabled;
                    prop.DataType = "bool";
                    prop.Dependencies = "False:HIDE PrimaryImdbFilmTitleMatchingEngineProp,AssistModeProp,DurationMatchingProp" +
                                        ";True:SHOW PrimaryImdbFilmTitleMatchingEngineProp,AssistModeProp,DurationMatchingProp";

                    return true;
                }








                if (index == counter++)
                {
                    //set the internal name
                    prop.Name = "DurationMatchingProp";
                    //set name shown to user
                    prop.Caption = "Try to match video duration first";
                    prop.Caption = TranslationProvider.Translate(prop.Caption, this);
                    prop.GroupName = "VideoIdentifier";
                    //set the tool tip
                    prop.HelpText = " The video duration matching mechanism will assist the video title matcher in recognizing the correct film result from the returned IMDb results," + Environment.NewLine + 
                                    "by comparing the IMDb result's duration value to the local video file's actual duration. A succesfull video duration matching will result in a perfect match. ";
                    prop.DefaultValue = Settings.EnableDurationMatching;
                    prop.DataType = "bool";
                    return true;
                }
                #endregion

                #endregion


                #region Film Updaters

                #region Film Updater
                if (index == counter++)
                {
                    //set the internal name
                    prop.Name = "SimpleWantFilmDetailsProp";
                    prop.GroupName = "FilmUpdater";
                    //set name shown to user
                    prop.Caption = "Identify and import film information";
                    prop.Caption = TranslationProvider.Translate(prop.Caption, this);
                    prop.GroupCaption = "                                Film Updater";
                    //set the tool tip
                    prop.HelpText = "";
                    prop.DefaultValue = Settings.SimpleFilmDetailsDownloaderIsEnabled;
                    prop.DataType = "bool";
                    prop.Dependencies = "False:HIDEGROUP " 
                                        +
                                        "FilmDetails"
                                        +
                                        ";True:SHOWGROUP " 
                                        +
                                        "FilmDetails"
                                        +
                                        ";False:HIDE "
                                        +
                                        "DownloadPosterChoice," 
                                        +
                                        "DownloadBackdropsChoice," 
                                        +
                                        "HashUploadingProp"
                                        +
                                        ";True:SHOW " 
                                        +
                                        "DownloadPosterChoice,"
                                        +
                                        "DownloadBackdropsChoice," 
                                        +
                                        "HashUploadingProp";


                    return true;
                }

                if (index == counter++)
                {
                    //set the internal name
                    prop.Name = "DownloadPosterChoice";
                    prop.GroupName = "FilmUpdater";
                    //set name shown to user
                    prop.Caption = "Also download cover images ";
                    prop.Caption = TranslationProvider.Translate(prop.Caption, this);
                    //Prop.GroupCaption = "                                Film Updater";
                    //set the tool tip
                    prop.HelpText = "Do you want MediaFairy to download cover images for your films?";
                    prop.DefaultValue = Settings.FilmCoverArtDownloaderIsEnabled;
                    prop.DataType = "bool";
                    prop.Dependencies = "False:HIDEGROUP FilmCovers" +
                                        ";True:SHOWGROUP FilmCovers";
                    return true;
                }
                if (index == counter++)
                {
                    //set the internal name
                    prop.Name = "DownloadBackdropsChoice";
                    prop.GroupName = "FilmUpdater";
                    //set name shown to user
                    prop.Caption = "Also download backdrop images ";
                    prop.Caption = TranslationProvider.Translate(prop.Caption, this);
                    //Prop.GroupCaption = "                                Film Updater";
                    //set the tool tip
                    prop.HelpText = "Do you want MediaFairy to download high-resolution backdrop images for your films?";
                    prop.DefaultValue = Settings.FilmBackdropArtDownloaderIsEnabled;
                    prop.DataType = "bool";
                    prop.Dependencies = "False:HIDEGROUP FilmBackdrops" +
                                        ";True:SHOWGROUP FilmBackdrops";
                    return true;
                }




                #endregion




                if (FilmDetailsPropertiesGetter
                    .GetFilmDetailsProperties
                    (this, index, prop, twochoices, 
                    ref counter)) 
                    return true;




                #region Film Covers


    
                if (index == counter++)
                {
                    //set the internal name
                    prop.Name = "TMDbPostersEnabledProp";
                    //set name shown to user
                    prop.Caption = "Download film coverart from TheMovieDb.org";
                    prop.Caption = TranslationProvider.Translate(prop.Caption, this);
                    //set the tool tip
                    prop.GroupCaption = "                           Film Coverart Settings";
                    prop.GroupName = "FilmCovers";
                    prop.HelpText = "Should TheMovieDb.org be searched for film posters?";
                    prop.DefaultValue = Settings.TMDbPostersEnabled;
                    prop.DataType = "bool";
                    return true;
                }
                if (index == counter++)
                {
                    //set the internal name
                    prop.Name = "ImpAwardsPostersEnabledProp";
                    //set name shown to user
                    prop.Caption = "Download film coverart from ImpAwards.com";
                    prop.Caption = TranslationProvider.Translate(prop.Caption, this);
                    //set the tool tip
                    prop.GroupName = "FilmCovers";
                    prop.HelpText = "Should ImpAwards.com be searched for film coverart?";
                    prop.DefaultValue = Settings.ImpAwardsPostersEnabled;
                    prop.DataType = "bool";
                    return true;
                }
                if (index == counter++)
                {
                    //set the internal name
                    prop.Name = "IMDbPostersEnabledProp";
                    //set name shown to user
                    prop.Caption = "Download film posters from IMDb.com";
                    prop.Caption = TranslationProvider.Translate(prop.Caption, this);
                    //set the tool tip
                    prop.GroupName = "FilmCovers";
                    prop.HelpText = "Should IMDb.com be searched for film posters?";
                    prop.DefaultValue = Settings.IMDbPostersEnabled;
                    prop.DataType = "bool";
                    return true;
                }
 


                if (index == counter++)
                {
                    //set the internal name
                    prop.Name = "PrimaryPosterSourceChoice";
                    //set name shown to user
                    prop.Caption = "Primary coverart source:";
                    prop.Caption = TranslationProvider.Translate(prop.Caption, this);
                    //set the tool tip
                    prop.GroupName = "FilmCovers";
                    prop.HelpText = "From which source do you want MediaFairy to search for film coverart first?";
                    prop.DefaultValue = Settings.PosterPrimarySource;
                    prop.DataType = "string";
                    prop.CanTypeChoices = false;
                    threechoices[0] = "TheMovieDatabase";
                    threechoices[1] = "ImpAwards";
                    threechoices[2] = "IMDb";
                    prop.Choices = threechoices;
                    return true;
                }


                if (index == counter++)
                {
                    //set the internal name
                    prop.Name = "PosterSizeChoice";
                    //set name shown to user
                    prop.Caption = "Preferred coverart size:";
                    prop.Caption = TranslationProvider.Translate(prop.Caption, this);
                    prop.GroupName = "FilmCovers";
                    //set the tool tip
                    prop.HelpText = "When downloading film coverart from TMDB, which poster size do you prefer?";
                    prop.DefaultValue = Settings.PosterSize;
                    prop.DataType = "string";
                    fourchoices[0] = "Extra Large";
                    fourchoices[1] = "Large";
                    fourchoices[2] = "Medium";
                    fourchoices[3] = "Small";
                    prop.CanTypeChoices = false;
                    prop.Choices = fourchoices;
                    return true;
                }


                #region Image Resizer
                if (index == counter++)
                {
                    //set the internal name
                    prop.Name = "ResizeFilmPostersProp";
                    //set name shown to user
                    prop.Caption = "Automatically resize film coverart images";
                    prop.Caption = TranslationProvider.Translate(prop.Caption, this);
                    //set the tool tip
                    prop.GroupName = "FilmCovers";
                    prop.HelpText = "Enable this option if you want the importer to automatically resize film coverart images after download." + Environment.NewLine +
                                    "The aspect ratio of the original image will be mantained.";
                    prop.DefaultValue = Settings.ResizeFilmPosters;
                    prop.DataType = "bool";
                    prop.Dependencies = "False:HIDE FilmPosterWidthProp,FilmPosterHeightProp" +
                                        ";True:SHOW FilmPosterWidthProp,FilmPosterHeightProp";
                    return true;
                }
                if (index == counter++)
                {
                    //set the internal name
                    prop.Name = "FilmPosterWidthProp";
                    //set name shown to user
                    prop.Caption = "Desired coverart width: ";
                    prop.Caption = TranslationProvider.Translate(prop.Caption, this);
                    //set the tool tip
                    prop.GroupName = "FilmCovers";
                    prop.HelpText = "Please set the desired pixel width of the resized coverart image. Default is 800.";
                    prop.DefaultValue = Settings.FilmPosterWidth;
                    prop.DataType = "int";
                    return true;
                }
                if (index == counter++)
                {
                    //set the internal name
                    prop.Name = "FilmPosterHeightProp";
                    //set name shown to user
                    prop.Caption = "Desired coverart height: ";
                    prop.Caption = TranslationProvider.Translate(prop.Caption, this);
                    //set the tool tip
                    prop.GroupName = "FilmCovers";
                    prop.HelpText = "Please set the desired pixel height of the resized coverart image. Default is 1000.";
                    prop.DefaultValue = Settings.FilmPosterHeight;
                    prop.DataType = "int";
                    return true;
                }
                #endregion








                //if (Index == counter++)
                //{
                //    //set the internal name
                //    Prop.Name = "WhereToSavePosterProp";
                //    //set name shown to user
                //    Prop.Caption = "Where to store posters?";
                //    Prop.Caption = TranslationProvider.Translate(Prop.Caption, this);
                //    Prop.GroupName = "FilmCovers";
                //    //set the tool tip
                //    Prop.HelpText = "Do you want the poster/cover image for each movie to be saved in the movie's folder or to a specific folder of your choice?";
                //    Prop.DefaultValue = Importer.WhereToSavePoster;
                //    Prop.DataType = "string";
                //    twochoices[0] = "in the movie's folder";
                //    twochoices[1] = "in the folder i specify below";
                //    Prop.CanTypeChoices = false;
                //    Prop.Choices = twochoices;
                //    Prop.IsMandatory = false;

                //    Prop.Dependencies = "in the movie's folder:HIDE PosterFolderProp" +
                //    ";in the folder i specify below:SHOW PosterFolderProp";
                //    return true;
                //}

                //if (Index == counter++)
                //{
                //    //set the internal name
                //    Prop.Name = "PosterFolderProp";
                //    //set name shown to user
                //    Prop.Caption = "Posters folder: ";
                //    Prop.Caption = TranslationProvider.Translate(Prop.Caption, this);
                //    Prop.GroupName = "FilmCovers";
                //    //set the tool tip
                //    Prop.HelpText = @"Please specify the folder in which you want downloaded film covers to be saved. (Only applicabe is option above is set to 'in the folder i specify below') ";
                //    Prop.DefaultValue = Importer.PosterFolder;
                //    Prop.DataType = "folder";
                //    Prop.CanTypeChoices = false;
                //    return true;
                //}

                //if (Index == counter++)
                //{
                //    //set the internal name
                //    Prop.Name = "PosterNamingChoice";
                //    //set name shown to user
                //    Prop.Caption = "Poster naming convention: ";
                //    Prop.Caption = TranslationProvider.Translate(Prop.Caption, this);
                //    Prop.GroupName = "FilmCovers";
                //    //set the tool tip
                //    Prop.HelpText = "How would you like your movies' posters image files to be named ?";
                //    Prop.DefaultValue = Settings.PosterNamingMethod;
                //    Prop.DataType = "string";
                //    Prop.CanTypeChoices = false;

                //    if (Settings.WhereToSavePoster == "in the movie's folder")
                //    {
                //        threechoices[0] = "folder.jpg";
                //        threechoices[1] = "iMDbID";
                //        threechoices[2] = "video filename";
                //        Prop.Choices = threechoices;
                //    }
                //    else
                //    {
                //        twochoices[0] = "iMDbID";
                //        twochoices[1] = "video filename";
                //        Prop.Choices = twochoices;
                //    }

                //    if (Importer.WhereToSavePoster != "in the movie's folder")
                //    {
                //        if (Importer.PosterNamingMethod == "folder.jpg")
                //            Importer.PosterNamingMethod = "iMDbID";
                //    }

                //    Prop.DefaultValue = Importer.PosterNamingMethod;
                    
                    
 
                //    return true;
                //}

                //if (Index == counter++)
                //{
                //    //set the internal name
                //    Prop.Name = "OverwritePosterChoice";
                //    //set name shown to user
                //    Prop.Caption = "Overwrite existing Posters?";
                //    Prop.Caption = TranslationProvider.Translate(Prop.Caption, this);
                //    Prop.GroupName = "FilmCovers";
                //    //set the tool tip
                //    Prop.HelpText = "If a poster already exists in your specified location, should MediaFairy to download a new poster and overwrite the old one?";
                //    Prop.DefaultValue = Importer.OverwriteExistingPosters;
                //    Prop.DataType = "bool";
                //    return true;
                //}
                #endregion


                #region Film Backdrops



                #region Image Resizer
                if (index == counter++)
                {
                    //set the internal name
                    prop.Name = "ResizeFilmBackdropsProp";
                    //set name shown to user
                    prop.Caption = "Automatically resize film fanart images";
                    prop.Caption = TranslationProvider.Translate(prop.Caption, this);
                    prop.GroupCaption = "                           Film Fanart Settings";
                    //set the tool tip
                    prop.GroupName = "FilmBackdrops";
                    prop.HelpText = "Enable this option if you want the importer to automatically resize film fanart images after download." + Environment.NewLine +
                                    "The aspect ratio of the original image will be mantained.";
                    prop.DefaultValue = Settings.ResizeFilmBackdrops;
                    prop.DataType = "bool";
                    prop.Dependencies = "False:HIDE FilmBackdropWidthProp,FilmBackdropHeightProp" +
                                        ";True:SHOW FilmBackdropWidthProp,FilmBackdropHeightProp";
                    return true;
                }
                if (index == counter++)
                {
                    //set the internal name
                    prop.Name = "FilmBackdropWidthProp";
                    //set name shown to user
                    prop.Caption = "Desired fanart width: ";
                    prop.Caption = TranslationProvider.Translate(prop.Caption, this);
                    //set the tool tip
                    prop.GroupName = "FilmBackdrops";
                    prop.HelpText = "Please set the desired pixel width of the resized fanart image. Default is 1360.";
                    prop.DefaultValue = Settings.FilmBackdropWidth;
                    prop.DataType = "int";
                    return true;
                }
                if (index == counter++)
                {
                    //set the internal name
                    prop.Name = "FilmBackdropHeightProp";
                    //set name shown to user
                    prop.Caption = "Desired fanart height: ";
                    prop.Caption = TranslationProvider.Translate(prop.Caption, this);
                    //set the tool tip
                    prop.GroupName = "FilmBackdrops";
                    prop.HelpText = "Please set the desired pixel height of the resized fanart image. Default is 768.";
                    prop.DefaultValue = Settings.FilmBackdropHeight;
                    prop.DataType = "int";
                    return true;
                }
                #endregion



                //if (FilmBackdropsLocationPropertiesGetter
                //    .GetFilmBackdropsLocationProperties
                //    (this, index, prop, twochoices,
                //    ref counter)) 
                //    return true;



                #endregion



                //TODO: Subtitles should be downloaded for both films and tv episodes with this setting.
                #region Subtitles

                if (index == counter++)
                {
                    //set the internal name
                    prop.Name = "DownloadSubtitlesChoice";
                    prop.GroupName = "Subtitles";
                    //set name shown to user
                    prop.Caption = "Download synchronized subtitles? ";
                    prop.Caption = TranslationProvider.Translate(prop.Caption, this);
                    //set the tool tip
                    prop.HelpText = "Do you want MediaFairy to automatically download subtitles for your movies and TV series?";
                    prop.GroupCaption = "                             Subtitles Settings";
                    prop.DefaultValue = Settings.EnableSubtitleDownloader;
                    prop.Dependencies = "False:HIDE LanguageProp,LanguagePropSecondary" +
                                        ";True:SHOW LanguageProp,LanguagePropSecondary";
                    prop.DataType = "bool";
                    twochoices[0] = "eng";
                    twochoices[1] = "ell";
                    //Prop.Choices = choices;
                    return true;
                }

                if (index == counter++)
                {
                    //set the internal name
                    prop.Name = "LanguageProp";
                    prop.GroupName = "Subtitles";
                    //set name shown to user
                    prop.Caption = "Primary Subtitles language:";
                    prop.Caption = TranslationProvider.Translate(prop.Caption, this);
                    //set the tool tip
                    prop.HelpText = "Please insert the ISO639 3-letter code of your country." + Environment.NewLine +
                                    "MediaFairy will try to download subtitles for this language first." + Environment.NewLine +
                                    "To find your country code use this reference: http://en.wikipedia.org/wiki/List_of_ISO_639-2_codes ";
                    prop.DefaultValue = Settings.PrimaryLanguage;
                    prop.DataType = "string";
                    return true;
                }


                if (index == counter++)
                {
                    //set the internal name
                    prop.Name = "LanguagePropSecondary";
                    //set name shown to user
                    prop.GroupName = "Subtitles";
                    prop.Caption = "Secondary subtitles language:";
                    prop.Caption = TranslationProvider.Translate(prop.Caption, this);
                    //set the tool tip
                    prop.HelpText = "If no subtitles are found for your primary language, MediaFairy will search for subtitles for this language.";
                    prop.DefaultValue = Settings.SecondaryLanguage;
                    prop.DataType = "string";
                    return true;
                }

                if (index == counter++)
                {
                    //set the internal name
                    prop.Name = "WhenToUseSecondaryLanguageProp";
                    //Set the property's group.
                    prop.GroupName = "Subtitles";
                    //set name shown to user
                    prop.Caption = "When to use secondary language?";
                    //set the tool tip
                    prop.HelpText = "Reserved for next version";
                    prop.DefaultValue = Settings.WhenToUseSecondaryLanguage;
                    prop.DataType = "string";
                    twochoices[0] = "If no synchronized subs exist for primary language";
                    twochoices[1] = "If no subtitles at all exist for primary language";
                    prop.Choices = twochoices;
                    prop.IsMandatory = false;
                    return true;
                }

                //TODO: Add "Use the same folder for all subtitles" option.

                #endregion





                //if (FilmReleasesUpdaterProperties
                //    .GetFilmReleasesUpdaterProperties
                //    (this, index, prop, ref counter)) 
                //    return true;



                #endregion










                if (index == counter++)
                {
                    //set the internal name
                    prop.Name = "UpdateTvShowsProp";
                    prop.GroupName = "MediaUpdaters";
                    //set name shown to user
                    prop.Caption = "Update Tv Series sections? ";
                    prop.Caption = TranslationProvider.Translate(prop.Caption, this);
                    //Prop.GroupCaption = "                                Media Updaters ";
                    //set the tool tip
                    prop.HelpText = "Do you want MediaFairy to work with the Tv Series in your Tv Shows library?";
                    prop.DefaultValue = Settings.EnableTvShowsSectionUpdater;
                    prop.DataType = "bool";
                    prop.Dependencies =

                                        "False:HIDEGROUP "
                                        +
                                        "TvUpdater" 
                                        +
                                        ";True:SHOWGROUP " 
                                        +
                                        "TvUpdater";


                    return true;
                }


                #region TV Updaters

                #region TV Updater

                if (index == counter++)
                {
                    //set the internal name
                    prop.Name = "WantEpisodeDetailsProp";
                    prop.GroupName = "TvUpdater";
                    //set name shown to user
                    prop.Caption = "Download episode details";
                    prop.Caption = TranslationProvider.Translate(prop.Caption, this);
                    prop.GroupCaption = "                                   TV Updater";
                    //set the tool tip
                    prop.HelpText = " Would you like the plugin to download online details from TVdb for each episode in your TV library? ";
                    prop.DefaultValue = Settings.WantEpisodeDetails;
                    prop.DataType = "bool";
                    return true;
                }

                if (index == counter++)
                {
                    //set the internal name
                    prop.Name = "WantEpisodeThumbnailsProp";
                    prop.GroupName = "TvUpdater";
                    //set name shown to user
                    prop.Caption = "Download episode thumbnails";
                    prop.Caption = TranslationProvider.Translate(prop.Caption, this);
                    //Prop.GroupCaption = "                                   TV Updater";
                    //set the tool tip
                    prop.HelpText = " Would you like the plugin to download thumbnail images for each episode in your TV library? ";
                    prop.DefaultValue = Settings.WantEpisodeThumbnails;
                    prop.DataType = "bool";
                    return true;
                }

                if (index == counter++)
                {
                    //set the internal name
                    prop.Name = "WantSeriesBannersProp";
                    prop.GroupName = "TvUpdater";
                    //set name shown to user
                    prop.Caption = "Download series banners";
                    prop.Caption = TranslationProvider.Translate(prop.Caption, this);
                    //Prop.GroupCaption = "                                   TV Updater";
                    //set the tool tip
                    prop.HelpText = " Would you like the plugin to download banner images for the series in your TV library? ";
                    prop.DefaultValue = Settings.WantSeriesBanners;
                    prop.DataType = "bool";
                    //Prop.Dependencies = "True:SHOWGROUP TvImages";
                    return true;
                }

                if (index == counter++)
                {
                    //set the internal name
                    prop.Name = "WantSeriesBackdropsProp";
                    prop.GroupName = "TvUpdater";
                    //set name shown to user
                    prop.Caption = "Download series fanart images";
                    prop.Caption = TranslationProvider.Translate(prop.Caption, this);
                    //Prop.GroupCaption = "                                   TV Updater";
                    //set the tool tip
                    prop.HelpText = " Would you like the plugin to download high resolution fanart images for the series in your TV library? ";
                    prop.DefaultValue = Settings.WantSeriesBackdrops;
                    prop.DataType = "bool";
                    //Prop.Dependencies = "True:SHOWGROUP TvImages";
                    return true;
                }

                if (index == counter++)
                {
                    //set the internal name
                    prop.Name = "WantSeasonImagesProp";
                    prop.GroupName = "TvUpdater";
                    //set name shown to user
                    prop.Caption = "Download season covers";
                    prop.Caption = TranslationProvider.Translate(prop.Caption, this);
                    //Prop.GroupCaption = "                                   TV Updater";
                    //set the tool tip
                    prop.HelpText = " Would you like the plugin to download cover images for each season of the tv series in your TV library? ";
                    prop.DefaultValue = Settings.WantSeasonImages;
                    prop.DataType = "bool";
                    //Prop.Dependencies = "True:SHOWGROUP TvImages";
                    return true;
                }
                #endregion


                #region TV Series Images

                //if (Index == counter++)
                //{
                //    Prop.GroupCaption = "                   Tv Series Images Settings";
                //    Prop.GroupName = "TvImages";
                //    //set the internal name
                //    Prop.Name = "SeriesBannersFolderProp";
                //    //set name shown to user
                //    Prop.Caption = "Series Banners location: ";
                //    Prop.Caption = TranslationProvider.Translate(Prop.Caption, this);
                //    //set the tool tip
                //    Prop.HelpText = @"Please specify the folder in which you want downloaded banners for TV series to be saved. ";
                //    Prop.DefaultValue = Importer.SeriesBannersFolder;
                //    Prop.DataType = "folder";
                //    Prop.CanTypeChoices = false;
                //    return true;
                //}

                //if (Index == counter++)
                //{
                //    Prop.GroupName = "TvImages";
                //    //set the internal name
                //    Prop.Name = "SeriesFanartFolderProp";
                //    //set name shown to user
                //    Prop.Caption = "Series fanart images location: ";
                //    Prop.Caption = TranslationProvider.Translate(Prop.Caption, this);
                //    //set the tool tip
                //    Prop.HelpText = @"Please specify the folder in which you want downloaded fanart images for TV series to be stored. ";
                //    Prop.DefaultValue = Importer.SeriesFanartFolder;
                //    Prop.DataType = "folder";
                //    Prop.CanTypeChoices = false;
                //    return true;
                //}

                //if (Index == counter++)
                //{
                //    Prop.GroupName = "TvImages";
                //    //set the internal name
                //    Prop.Name = "SeasonImagesFolderProp";
                //    //set name shown to user
                //    Prop.Caption = "Season images location: ";
                //    Prop.Caption = TranslationProvider.Translate(Prop.Caption, this);
                //    //set the tool tip
                //    Prop.HelpText = @"Please specify the folder in which you want downloaded banners for TV series to be saved. ";
                //    Prop.DefaultValue = Importer.SeasonImagesFolder;
                //    Prop.DataType = "folder";
                //    Prop.CanTypeChoices = false;
                //    return true;
                //}

                #endregion

                #endregion


                if (index == counter++)
                {
                    //set the internal name
                    prop.Name = "UpdateMusicProp";
                    prop.GroupName = "MediaUpdaters";
                    //set name shown to user
                    prop.Caption = "Update Music section? ";
                    prop.Caption = TranslationProvider.Translate(prop.Caption, this);
                    //Prop.GroupCaption = "                                Media Updaters ";
                    //set the tool tip
                    prop.HelpText = "Do you want MediaFairy to work with your Music library";
                    prop.DefaultValue = Settings.EnableMusicSectionUpdater;
                    prop.DataType = "bool";
                    prop.Dependencies = "False:HIDEGROUP MusicUpdater,AlbumCovers" +
                                        ";True:SHOWGROUP MusicUpdater";
                    return true;
                }

                #region Music Updater
                if (index == counter++)
                {
                    //set the internal name
                    prop.Name = "WantAudioTagsProp";
                    prop.GroupName = "MusicUpdater";
                    //set name shown to user
                    prop.Caption = "Import track information from audio file";
                    prop.Caption = TranslationProvider.Translate(prop.Caption, this);
                    prop.GroupCaption = "                              Music Updater";
                    //set the tool tip
                    prop.HelpText = "MediaFairy has the capability to extract the metadata contained in your audio files" + Environment.NewLine + 
                                    "and import this information into your music library. Would you like to enable this feature?";
                    prop.DefaultValue = Settings.WantAudioTags;
                    prop.DataType = "bool";
                    prop.Dependencies = "False:HIDE WantAlbumCoversProp" +
                                        ";True:SHOW WantAlbumCoversProp";
                    return true;
                }



                //if (index == counter++)
                //{
                //    //set the internal name
                //    prop.Name = "WantMusicDNSProp";
                //    prop.GroupName = "MusicUpdater";
                //    //set name shown to user
                //    prop.Caption = "Also use online audio identifier ";
                //    prop.Caption = TranslationProvider.Translate(prop.Caption, this);
                //    //Prop.GroupCaption = "                              Music Updater";
                //    //set the tool tip
                //    prop.HelpText = "MediaFairy contains an automated song recognition engine" + Environment.NewLine + 
                //                    "which uses the audio fingerprinting technology by MusicDNS" + Environment.NewLine + 
                //                    "to uniquely identify your audio tracks by their acoustic properties." + Environment.NewLine + 
                //                    "Would you like this feature enabled?" + Environment.NewLine + 
                //                    "(It is recommended that the option 'import track information from audio file' is also enabled) ";
                //    prop.DefaultValue = Settings.MusicDNSAudioTrackIdentifierIsEnabled;
                //    prop.DataType = "bool";
                //    prop.Dependencies = "False:HIDE SaveMetadataToAudioTagsProp" +
                //                        ";True:SHOW SaveMetadataToAudioTagsProp";
                //    return true;
                //}




                //if (index == counter++)
                //{
                //    //set the internal name
                //    prop.Name = "SaveMetadataToAudioTagsProp";
                //    //set name shown to user
                //    prop.Caption = "Save meta-data in audio file?";
                //    prop.Caption = TranslationProvider.Translate(prop.Caption, this);
                //    //set the tool tip
                //    prop.GroupName = "MusicUpdater";

                //    prop.HelpText = "After succesfull recognition " +
                //                    "by audio fingerprint, " +
                //                    "the newly acquired metadata" + Environment.NewLine + 
                //                    "can be imported into the tags of the audio file," +
                //                    " and not only in the Meedio library.";

                //    prop.DefaultValue = Settings.SaveMetadataToAudioTags;

                //    prop.DataType = "bool";
                //    return true;
                //}





                if (index == counter++)
                {
                    //set the internal name
                    prop.Name = "WantAlbumCoversProp";
                    prop.GroupName = "MusicUpdater";
                    //set name shown to user
                    prop.Caption = "Also download album covers";
                    prop.Caption = TranslationProvider.Translate(prop.Caption, this);
                    //Prop.GroupCaption = "                              Music Updater";
                    //set the tool tip
                    prop.HelpText = "Do you want MediaFairy to download album cover images for your music albums?";
                    prop.DefaultValue = Settings.WantAlbumCovers;
                    prop.DataType = "bool";
                    prop.Dependencies = "False:HIDEGROUP AlbumCovers" +
                                        ";True:SHOWGROUP AlbumCovers";
                    return true;
                }




                //if (index == counter++)
                //{
                //    //set the internal name
                //    prop.Name = "WantArtistBackdropsProp";
                //    prop.GroupName = "MusicUpdater";
                //    //set name shown to user
                //    prop.Caption = "Also download artist fanart images";
                //    prop.Caption = TranslationProvider.Translate(prop.Caption, this);
                //    //Prop.GroupCaption = "                              Music Updater";
                //    //set the tool tip
                //    prop.HelpText = "Would you like to download backdrop images for artists?";
                //    prop.DefaultValue = Settings.ArtistBackdropsEnabled;
                //    prop.DataType = "bool";
                //    //Prop.Dependencies = "False:HIDE ArtistBackdropsLocationProp" +
                //    //";True:SHOW ArtistBackdropsLocationProp";
                //    return true;
                //}

 




                #endregion


                #region Album Covers
                //if (Index == counter++)
                //{
                //    Prop.GroupName = "AlbumCovers";
                //    //set the internal name
                //    Prop.Name = "WhereToSaveAlbumCoverProp";
                //    //set name shown to user
                //    Prop.Caption = "Where to save album covers?";
                //    Prop.Caption = TranslationProvider.Translate(Prop.Caption, this);
                //    //set the tool tip
                //    Prop.GroupCaption = "                       Album Covers Settings";
                //    Prop.HelpText = "Do you want the poster/cover image for each album to be saved in the album's folder or in a specific folder of your choice?";
                //    Prop.DefaultValue = Importer.WhereToSaveAlbumCover;
                //    Prop.DataType = "string";
                //    twochoices[0] = "in the album's folder";
                //    twochoices[1] = "in the folder i specify below";
                //    Prop.CanTypeChoices = false;
                //    Prop.Choices = twochoices;
                //    Prop.IsMandatory = false;

                //    Prop.Dependencies = "in the album's folder:HIDE AlbumCoversFolderProp" +
                //    ";in the folder i specify below:SHOW AlbumCoversFolderProp";
                //    return true;
                //}

                //if (Index == counter++)
                //{
                //    Prop.GroupName = "AlbumCovers";
                //    //set the internal name
                //    Prop.Name = "AlbumCoversFolderProp";
                //    //set name shown to user
                //    Prop.Caption = "Album covers folder: ";
                //    Prop.Caption = TranslationProvider.Translate(Prop.Caption, this);
                //    //set the tool tip
                //    Prop.HelpText = @"Please specify the folder in which you want downloaded album cover images to be saved. (Only applicabe if option above is set to 'Movie's folder') ";
                //    Prop.DefaultValue = Importer.AlbumCoverFolder;
                //    Prop.DataType = "folder";
                //    Prop.CanTypeChoices = false;
                //    return true;
                //}

                //if (Index == counter++)
                //{
                //    //set the internal name
                //    Prop.Name = "AlbumCoverNamingChoice";
                //    //set name shown to user
                //    Prop.Caption = "Album covers naming convention: (tag mask) ";
                //    Prop.Caption = TranslationProvider.Translate(Prop.Caption, this);
                //    Prop.GroupName = "AlbumCovers";
                //    //set the tool tip
                //    Prop.HelpText = "Please enter here your desired album covers naming convention in a form of a tag mask." + Environment.NewLine + "For example, the default tag mask '<IMDbID>_backdrop' will store the backdrop image as the varialbe film's IMDb number followed by the '_backdrop' suffix ." + Environment.NewLine + "Please make sure that any text strings contained in '<>' refer to the names of existing tags of your Movies media section.";
                //    Prop.DefaultValue = Importer.AlbumCoversNamingMethod;
                //    Prop.DataType = "string";
                //    Prop.CanTypeChoices = true;
                //    return true;
                //}


                if (index == counter++)
                {
                    prop.GroupName = "AlbumCovers";
                    //set the internal name
                    prop.Name = "EmbedAlbumCoverProp";
                    //set name shown to user
                    prop.Caption = "Also embed cover image in audio file";
                    prop.Caption = TranslationProvider.Translate(prop.Caption, this);
                    //set the tool tip
                    prop.HelpText = " Certain audio file-types like .mp3 and .wma are capable of storing (embeding) the album's cover image in the file itself. \r This eliminates the need of storing a seperate image file for every music album, thus makes the audio files self-contained and easily transportable. \r Do you want the plugin to embed cover images for imported audio files? ";
                    prop.DefaultValue = Settings.EmbedAlbumCover;
                    prop.DataType = "bool";
                    prop.CanTypeChoices = false;
                    return true;
                }

                //if (Index == counter++)
                //{
                //    //set the internal name
                //    Prop.Name = "OverwriteAlbumCovers";
                //    //set name shown to user
                //    Prop.Caption = "Overwrite existing covers?";
                //    Prop.Caption = TranslationProvider.Translate(Prop.Caption, this);
                //    Prop.GroupName = "AlbumCovers";
                //    //set the tool tip
                //    Prop.HelpText = "If an album cover image already exists in your specified location, should MediaFairy download a new cover image and overwrite the existing one?";
                //    Prop.DefaultValue = Importer.OverwriteExistingAlbumCovers;
                //    Prop.DataType = "bool";
                //    return true;
                //}
                #endregion




                if (MediaOrganizersPropertiesGetter
                    .GetMediaOrganizersProperties
                    (this, index, prop,
                    twochoices, ref counter)) 
                    return true;


                //if (GeneralContentDownloaderProperties
                //    .GetGeneralContentDownloaderProperties
                //    (this, index, prop, ref counter))
                //    return true;




                if (LibraryManagementPropertiesGetter
                    .GetLibraryManagementProperties
                    (this, index, prop, ref counter))
                    return true;

                #endregion






                //if (MediaArchiversPropertiesGetter
                //    .GetMediaArchiversProperties
                //    (this, index, prop,
                //    twochoices, ref counter))
                //    return true;



                //if (LibrarySynchronizersPropertiesGetter
                //    .GetLibrarySynchronizersProperties
                //    (this, index, prop, ref counter))
                //    return true;


                //if (ImportSequencerPropertiesGetter
                //    .GetImportSequencerProperties
                //    (this, index, prop, ref counter)) 
                //    return true;




                #region Diagnostics Properties
                if (index == counter++)
                {
                    prop.GroupName = "Diagnostics";
                    //set the internal name
                    prop.Name = "ConnectionDiagnosticsProp";
                    //set name shown to user
                    prop.Caption = "Enable internet connection diagnostic? ";
                    prop.Caption = TranslationProvider.Translate(prop.Caption, this);
                    prop.GroupCaption = "                               Diagnostics";
                    //set the tool tip
                    prop.HelpText = "Should MediaFairy test for internet connection and online databases availability before attempting to download data?" + Environment.NewLine + 
                                    "This feature will help the importer recognize offline or problematic sources and avoid using them," + Environment.NewLine + 
                                    "preventing failed retries, minimizing delays and improving importing times in those situations.";
                    prop.DefaultValue = Settings.ConnectionDiagnosticsEnabled;
                    prop.DataType = "bool";
                    prop.Dependencies = "False:HIDE DisableSourcesProp" +
                                        ";True:SHOW DisableSourcesProp";
                    return true;
                }
                if (index == counter++)
                {
                    prop.GroupName = "Diagnostics";
                    //set the internal name
                    prop.Name = "DisableSourcesProp";
                    //set name shown to user
                    prop.Caption = "Disable problematic sources? ";
                    prop.Caption = TranslationProvider.Translate(prop.Caption, this);
                    //set the tool tip
                    prop.HelpText = "Should unconnectable online sources be disabled for the rest of an importing session? ";
                    prop.DefaultValue = Settings.WantToDisableSources;
                    prop.DataType = "bool";
                    return true;
                }

                if (index == counter++)
                {
                    prop.GroupName = "Diagnostics";
                    //set the internal name
                    prop.Name = "FileserverDiagnosticsProp";
                    //set name shown to user
                    prop.Caption = "Enable media server diagnostic? ";
                    prop.Caption = TranslationProvider.Translate(prop.Caption, this);
                    //set the tool tip
                    prop.HelpText = "Should MediaFairy check if your network disk is online and accessible before attempting to read or transfer any data?" + Environment.NewLine + 
                                    "This feature will prevent failed attempts to access network disks and improve importing times in those cases.";
                    prop.DefaultValue = Settings.WantFileserverDiagnostics;
                    prop.DataType = "bool";
                    return true;
                }

                if (index == counter++)
                {
                    prop.GroupName = "Diagnostics";
                    //set the internal name
                    prop.Name = "DebugLogProp";
                    //set name shown to user
                    prop.Caption = "Write Debug.log file";
                    prop.Caption = TranslationProvider.Translate(prop.Caption, this);
                    prop.GroupCaption = "                          Diagnostics";
                    //set the tool tip
                    prop.HelpText = "If enabled, MeediFier will write a debug log containing" + Environment.NewLine +
                                    "important information from the last importing session for debugging purposes.";
                    prop.DefaultValue = Settings.WriteDebugLog;
                    prop.DataType = "bool";
                    return true;
                }

                #endregion


                #region User Interface

                // ReSharper disable RedundantAssignment
                if (index == counter++)
                // ReSharper restore RedundantAssignment
                {
                    prop.GroupName = "UserInterface";
                    //set the internal name
                    prop.Name = "EnableUIProp";
                    //set name shown to user
                    prop.Caption = "Always prompt for images";
                    prop.Caption = TranslationProvider.Translate(prop.Caption, this);
                    prop.GroupCaption = "                          User Interface";
                    //set the tool tip
                    prop.HelpText = "The plugin's user interface gives users the option to preview and decide if images" + Environment.NewLine + 
                                    "(film coverart,fanart images and album covers) will be downloaded and which ones." + Environment.NewLine + 
                                    "If you want the plugin to always ask for your confirmation before downloading images, please enable this option.";
                    prop.DefaultValue = Settings.AlwaysPromptForImages;
                    prop.DataType = "bool";
                    return true;
                }


                //TODO: Add "Limit files to import" feature back. 

                //if (Index == counter++)
                //{
                //    //set the internal name
                //    Prop.Name = "LimitFilesToImportProp";
                //    //set name shown to user
                //    Prop.Caption = "How many media files to import in each run? ";
                //    //set the tool tip
                //    Prop.GroupCaption = "                                   Interoperability";

                //    Prop.HelpText = "With this option, you can limit the number of new files MediaFairy imports into your library every time it runs. This setting is useful if you schedule the importer to run frequently and you want each importing proccess to complete quickly. Leave this setting to it's default setting (0) if you want MediaFairy to import all new media files it finds in your media root folders.  ";
                //    Prop.DefaultValue = Importer.importing_limit;
                //    Prop.DataType = "int";
                //    return true;
                //}



                #endregion




            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }


            return false;


        }


        //endof function


    } //endof class


} //endof namespace