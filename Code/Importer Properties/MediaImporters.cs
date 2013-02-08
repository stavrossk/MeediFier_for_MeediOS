using System;
using CustomProperties.PropertyData;
using MediaFairy.Code.Settings;
using MeediOS.Cache;


namespace MediaFairy.ImporterPropertiesGroups
{


    internal class MediaImporters
    {

        internal static bool MediaImportersProperties
            (ImporterProperties importerProperties, 
            int index, OptionSettings prop, 
            string[] twochoices, ref int counter)
        {

            if (index == counter++)
            {
                //set the internal name
                prop.Name = "WantFilesImportingProp";
                //set name shown to user
                prop.Caption = "Enable Media Importer";

                prop.Caption
                    = TranslationProvider.Translate
                    (prop.Caption, importerProperties);

                prop.GroupName = "MediaImporter";
                prop.GroupCaption = "                                 Media Importer";
                
                prop.HelpText = 
                    "The Automated Media Importer will scan the media directories you provide below" +
                    Environment.NewLine +
                    "and automatically recognize and import the media files of your desired media types" +
                    Environment.NewLine +
                    "in their respective library sections, depending on your preferences.";

                prop.DefaultValue = MediaImportersAndUpdatersCoreSettings.EnableMediaImporters;
                prop.DataType = "bool";
                prop.Dependencies =

                    "False:HIDE "
                    +
                    "WantToImportFilmsProp," +
                    "WantToImportMusicProp," +
                    "WantToImportTvSeriesProp," +
                    "WantMediaInfoProp," +
                    "WantToImportNewsProp"

                    +

                    ";False:HIDEGROUP "
                    +
                    "MediaAnalyzer,"
                    +
                    "MovieTitleLocator,"
                    +
                    "RSSimporter"

                    +

                    ";True:SHOW "
                    +
                    "WantToImportFilmsProp," +
                    "WantToImportMusicProp," +
                    "WantToImportTvSeriesProp," +
                    //"WantToImportBooksProp," +
                    //"WantToImportGamesProp," +
                    "WantMediaInfoProp," +
                    "WantToImportNewsProp"
                    +

                    ";True:SHOWGROUP "
                    +
                    "MediaAnalyzer";

                return true;
            }



            #region [DEPRECATED] Media Locations dialog

            //if (Index == counter++)
            //{
            //    //set the internal name
            //    Prop.Name = "ImportRootFolderProp";
            //    Prop.GroupName = "MediaImporter";
            //    //set name shown to user
            //    Prop.Caption = " Media Sources: ";
            //    //Prop.GroupCaption = "                                 Media Importer";
            //    //set the tool tip
            //    //Prop.IsMandatory = true;
            //    Prop.HelpText = "Please specify the local or network locations which contain the media files you want to import." + Environment.NewLine +
            //    "These directories can contain mixed media types and also any other type of files." + Environment.NewLine +
            //    "The built-in media type recognition engine will take care of importing each media file into its appropriate library section.";
            //    Prop.DefaultValue = Settings.ImportRootFolders;
            //    Prop.DataType = "folderlist";
            //    return true;
            //}

            #endregion



            #region Import Media Types

            #region Movies
            if (index == counter++)
            {
                //set the internal name
                prop.Name = "WantToImportFilmsProp";
                prop.GroupName = "MediaImporter";
                //set name shown to user
                prop.Caption = "Import Films";
                prop.Caption = TranslationProvider.Translate(prop.Caption, importerProperties);
                prop.HelpText = " Would you like the Automated Media Importer to import the films it finds in your root folders? ";
                prop.DefaultValue = MediaImportersAndUpdatersCoreSettings.EnableMovieImporter;
                prop.DataType = "bool";
                prop.Dependencies = "False:HIDEGROUP MovieTitleLocator" +
                                    ";True:SHOWGROUP MovieTitleLocator";
                return true;
            }
            #endregion

            if (MediaSections.FimImportingOptions(importerProperties, index, prop, twochoices, ref counter)) return true;

            #region TV Shows
            if (index == counter++)
            {
                //set the internal name
                prop.Name = "WantToImportTvSeriesProp";
                prop.GroupName = "MediaImporter";
                //set name shown to user
                prop.Caption = "Import Tv Series";
                prop.Caption = TranslationProvider.Translate(prop.Caption, importerProperties);
                //Prop.GroupCaption = "                                 Media Importer";
                //set the tool tip
                prop.HelpText =
                    "Would you like the Automated Media Importer to import the Tv Series episodes it finds in your root folders?";
                prop.DefaultValue = MediaImportersAndUpdatersCoreSettings.EnableTvSeriesImporter;
                prop.DataType = "bool";
                return true;
            }
            #endregion

            #region Music
            if (index == counter++)
            {
                //set the internal name
                prop.Name = "WantToImportMusicProp";
                prop.GroupName = "MediaImporter";
                //set name shown to user
                prop.Caption = "Import Music";
                prop.Caption = TranslationProvider.Translate(prop.Caption, importerProperties);
                //Prop.GroupCaption = "                                 Media Importer";
                //set the tool tip
                prop.HelpText =
                    "Would you like the Automated Media Importer to import the music tracks it finds in your root folders?";
                prop.DefaultValue = MediaImportersAndUpdatersCoreSettings.EnableMusicImporter;
                prop.DataType = "bool";
                return true;
            }
            #endregion

            #region RSS Feeds
            if (index == counter++)
            {
                //set the internal name
                prop.Name = "WantToImportNewsProp";
                prop.GroupName = "MediaImporter";
                //set name shown to user
                prop.Caption = "Import RSS feeds";
                prop.Caption = TranslationProvider.Translate(prop.Caption, importerProperties);
                //Prop.GroupCaption = "                                 Media Importer";
                //set the tool tip
                prop.HelpText = "Would you like to import RSS newsfeeds?";
                prop.DefaultValue = Settings.EnableRssImporter;
                prop.DataType = "bool";
                prop.Dependencies = "False:HIDEGROUP RSSimporter" +
                                    ";True:SHOWGROUP RSSimporter";
                return true;
            }
            #endregion

            //FUTURE: PC Games Importer
            #region PC Games Importer
            //if (index == counter++)
            //{
            //    //set the internal name
            //    prop.Name = "WantToImportGamesProp";
            //    prop.GroupName = "MediaImporter";
            //    //set name shown to user
            //    prop.Caption = "Import PC games ";
            //    prop.Caption = TranslationProvider.Translate(prop.Caption, importerProperties);
            //    //Prop.GroupCaption = "                                 Media Importer";
            //    //set the tool tip
            //    prop.HelpText =
            //        "Would you like the Automated Media Importer to import and download online details for the PC games in your game shortcuts' location?";
            //    prop.DefaultValue = Settings.EnableGamesImporter;
            //    prop.DataType = "bool";
            //    prop.Dependencies = "False:HIDE GameShortcutsProp" +
            //                        ";True:SHOW GameShortcutsProp";
            //    return true;
            //}
            #endregion

            //FUTURE: Books Importer
            #region Books Importer
            //if (index == counter++)
            //{
            //    //set the internal name
            //    prop.Name = "WantToImportBooksProp";
            //    prop.GroupName = "MediaImporter";
            //    //set name shown to user
            //    prop.Caption = "Import Books (reserved for future version) ";
            //    prop.Caption = TranslationProvider.Translate(prop.Caption, importerProperties);
            //    //Prop.GroupCaption = "                                 Media Importer";
            //    //set the tool tip
            //    prop.HelpText =
            //        "Would you like the Automated Media Importer to import the digital books it finds in your root folders?";
            //    prop.DefaultValue = Settings.EnableBooksImporter;
            //    prop.DataType = "bool";
            //    return true;
            //}
            #endregion


            #endregion




            #region Media Analyzer

            if (index == counter++)
            {
                //set the internal name
                prop.Name = "EnableMediaDetectionProp";
                prop.GroupName = "MediaAnalyzer";
                //set name shown to user
                prop.Caption = "Automatically detect media types";

                prop.Caption 
                    = TranslationProvider.Translate
                    (prop.Caption, importerProperties);


                prop.GroupCaption = "                              Media Analyzer";
                //set the tool tip
                prop.HelpText = " This setting will determine if a media type analyzing process will be performed for each imported media file." + Environment.NewLine +
                "This analyzing process provides the media importer the ability to automatically recognize media files and distinguish their media type," + Environment.NewLine +
                "without requiring any user configuration and/or manual media collection organizing." + Environment.NewLine +
                "Please note however that with this feature enabled, the importing time may be significantly increased the first time you run the importer, especially with online files and/or large media collections." + Environment.NewLine +
                "Disabling this feature will improve the importing performance, but you will have to specify the root directories under which each media type is stored (under 'Override Media Analyzer' settings group)," + Environment.NewLine +
                "and those diretories will need to *only* contain media files belonging to the selected media type.";
              
                prop.DefaultValue = Settings.EnableMediaDetection;
                prop.DataType = "bool";
                return true;

            }

            if (index == counter++)
            {
                //set the internal name
                prop.Name = "RescanFilesNotFoundInLibraryProp";
                prop.GroupName = "MediaAnalyzer";
                //set name shown to user
                prop.Caption = "Re-scan media files missing from library";
                prop.Caption = TranslationProvider.Translate(prop.Caption, importerProperties);
                prop.GroupCaption = "                              Media Analyzer";
                //set the tool tip
                prop.HelpText =
                    "This setting determines if media files already scanned (but currently not present in any media section) will be re-scanned and re-imported." +
                    Environment.NewLine +
                    "With a slight importing performance impact, this feature will resolve two problems:" + Environment.NewLine +
                    "1) Items which were removed by mistake will be re-imported." + Environment.NewLine +
                    "2) If any of the root media folders are set to a downloads location and the download is not completed before MediaFairy scans the media file for the first time," +
                    Environment.NewLine +
                    "it may not be detected as a media file and thus not imported." + Environment.NewLine +
                    "Having this option enabled however will force a re-scan of this media file on all subsequent runs until it is finally analyzed correctly and imported." +
                    Environment.NewLine +
                    "If none of the above functionality is needed for your routine usage, you may disable this option for a slight importing performance gain.";
                prop.DefaultValue = Settings.RescanFilesNotFoundInLibrary;
                prop.DataType = "bool";
                return true;
            }





            if (index == counter++)
            {
                //set the internal name
                prop.Name = "WantMediaInfoProp";
                prop.GroupName = "MediaAnalyzer";
                //set name shown to user
                prop.Caption = "Import media information";
                prop.Caption = TranslationProvider.Translate(prop.Caption, importerProperties);
                //Prop.GroupCaption = "                              Media Analyzer";
                //set the tool tip
                prop.HelpText =
                    "Modern themes for Meedio frontend have the capability to graphically display digital media information for films or series,\r for example the video/audio format or quality.\r Would you like the plugin to automatically import this information?";
                prop.DefaultValue = Settings.WantMediaInfo;
                prop.DataType = "bool";
                return true;
            }

            #endregion



            #region RSS Importer

            if (index == counter++)
            {
                //set the internal name
                prop.Name = "RssFeedsProp";
                prop.GroupName = "RSSimporter";
                //set name shown to user
                prop.Caption = "RSS feeds to import: ";
                prop.Caption = TranslationProvider.Translate(prop.Caption, importerProperties);
                prop.GroupCaption = "                              RSS Importer";
                //set the tool tip
                prop.HelpText = "";
                prop.DefaultValue = Settings.RsSfeeds;
                prop.DataType = "stringlist";
                return true;
            }

            if (index == counter++)
            {
                //set the internal name
                prop.Name = "DownloadFeedImagesProp";
                prop.GroupName = "RSSimporter";
                //set name shown to user
                prop.Caption = "Download feed images";
                prop.Caption = TranslationProvider.Translate(prop.Caption, importerProperties);
                //set the tool tip
                prop.HelpText = "";
                prop.DefaultValue = Settings.DownloadRsSimages;
                prop.DataType = "bool";
                prop.Dependencies = "False:HIDE RSSimagesLocationProp" +
                                    ";True:SHOW RSSimagesLocationProp";
                return true;
            }

            if (index == counter++)
            {
                //set the internal name
                prop.Name = "RSSimagesLocationProp";
                prop.GroupName = "RSSimporter";
                //set name shown to user
                prop.Caption = "RSS images location: ";
                prop.Caption = TranslationProvider.Translate(prop.Caption, importerProperties);
                //set the tool tip
                prop.HelpText = "";
                prop.DefaultValue = Settings.RSSimagesLocation;
                prop.DataType = "folder";
                return true;
            }

            #endregion




            //if (FileDatesImporterPropertiesGetter
            //    .GetFileDatesProperties
            //    (importerProperties, index,
            //    prop, ref counter))
            //    return true;


            //if (MediaWatcher.MediaWatcherProperties
            //    (importerProperties, index, prop, 
            //    twochoices, ref counter)) 
            //    return true;



            //if (ArchiveExtractor
            //    .ArchiveExtractorPropRerties
            //    (importerProperties, index,
            //    prop, ref counter)) 
            //    return true;



            return false;

        }
    }


} //endof namespace
