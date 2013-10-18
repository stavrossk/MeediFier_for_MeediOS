using System;
using CustomProperties.PropertyData;
using MeediOS.Cache;



namespace MeediFier.Code.Importer_Properties
{



    class LibraryManagementPropertiesGetter
    {






        internal static bool GetLibraryManagementProperties
            (ImporterProperties importerProperties,
             int index, OptionSettings prop, ref int counter)
        {




            if (index == counter++)
            {
                prop.GroupName = "LibraryCleanup";
                //set the internal name
                prop.Name = "ExtractMetadataFromFilenamesProp";
                //set name shown to user
                prop.GroupCaption = "                             Library Management";
                prop.Caption = "Clean and extract metadata from filenames";
                prop.Caption = TranslationProvider.Translate(prop.Caption, importerProperties);
                //set the tool tip
                prop.HelpText = "This option enables the program's ability to recognize and extract key information" +
                                Environment.NewLine +
                                "about media files from their filenames, and clean them by removing unessecarry information." +
                                Environment.NewLine +
                                "Warning: Disabling this option may compromise the performance and accuraccy of the automated film and tv series identifiers.";
                prop.DefaultValue = MeediFier.Settings.ExtractMetadataFromFilenames;
                prop.DataType = "bool";
                return true;
            }
            if (index == counter++)
            {
                prop.GroupName = "LibraryCleanup";
                //set the internal name
                prop.Name = "EnableChainLinkingProp";
                //set name shown to user
                prop.Caption = "Link orphaned video files";
                prop.Caption = TranslationProvider.Translate(prop.Caption, importerProperties);
                //set the tool tip
                prop.HelpText = "If this setting is enabled, the films section updater will scan and link together" +
                                Environment.NewLine +
                                "multiple video files of the same film, using the film's unique id.";
                prop.DefaultValue = MeediFier.Settings.FilmItemChainLinkerIsEnabled;
                prop.DataType = "bool";
                return true;
            }

            if (index == counter++)
            {
                prop.GroupName = "LibraryCleanup";
                //set the internal name
                prop.Name = "DeleteMissingProp";
                //set name shown to user
                prop.Caption = "Delete missing library items";
                prop.Caption = TranslationProvider.Translate(prop.Caption, importerProperties);
                //set the tool tip
                prop.HelpText =
                    "This feature will automatically delete library items which refer to non-existent (deleted) files." +
                    Environment.NewLine +
                    "The media server diagnostic will ensure that the items located in your file server will not be accidentaly deleted if the server is offline. ";
                prop.DefaultValue = MeediFier.Settings.DeleteMissing;
                prop.DataType = "bool";
                prop.Dependencies =
                    "False:HIDE DeleteCoverImageOfMissingMediaFilesProp,DeleteFanartImageOfMissingMediaFilesProp" +
                    ";True:SHOW DeleteCoverImageOfMissingMediaFilesProp,DeleteFanartImageOfMissingMediaFilesProp";
                return true;
            }
            if (index == counter++)
            {
                prop.GroupName = "LibraryCleanup";
                //set the internal name
                prop.Name = "DeleteCoverImageOfMissingMediaFilesProp";
                //set name shown to user
                prop.Caption = "Delete cover images of missing media files";
                prop.Caption = TranslationProvider.Translate(prop.Caption, importerProperties);
                //set the tool tip
                prop.HelpText = "Enable this setting for the plugin to automatically delete" + Environment.NewLine +
                                "any left-over cover images of media files which were deleted. (but their entries are still present in library)";
                prop.DefaultValue = MeediFier.Settings.DeleteCoverImageOfMissingMediaFiles;
                prop.DataType = "bool";
                return true;
            }
            if (index == counter++)
            {
                prop.GroupName = "LibraryCleanup";
                //set the internal name
                prop.Name = "DeleteFanartImageOfMissingMediaFilesProp";
                //set name shown to user
                prop.Caption = "Delete fanart images of missing media files";
                prop.Caption = TranslationProvider.Translate(prop.Caption, importerProperties);
                //set the tool tip
                prop.HelpText = "Enable this setting for the plugin to automatically delete" + Environment.NewLine +
                                "any left-over fanart images of media files which were deleted. (but their entry is still present in library)";
                prop.DefaultValue = MeediFier.Settings.DeleteFanartImageOfMissingMediaFiles;
                prop.DataType = "bool";
                return true;
            }


            if (index == counter++)
            {
                prop.GroupName = "LibraryCleanup";
                //set the internal name
                prop.Name = "ClearEmptyFieldsProp";
                //set name shown to user
                prop.Caption = "Remove empty library fields";
                prop.Caption = TranslationProvider.Translate(prop.Caption, importerProperties);
                //set the tool tip
                prop.HelpText = "If you enable this option, MediaFairy will remove all empty tags in your library." +
                                Environment.NewLine +
                                "Only the tags that have all their values empty will be removed." + Environment.NewLine +
                                "Enabling this option will ensure a cleaner library and faster library accessing operations. ";
                prop.DefaultValue = MeediFier.Settings.CleanEmptyFields;
                prop.DataType = "bool";
                return true;
            }


            if (index == counter++)
            {
                prop.GroupName = "LibraryCleanup";
                //set the internal name
                prop.Name = "CleanSelectedFieldsProp";
                //set name shown to user
                prop.Caption = "Remove selected library fields: ";
                prop.Caption = TranslationProvider.Translate(prop.Caption, importerProperties);
                //set the tool tip
                prop.HelpText = "If you enable this option, MediaFairy will remove the library fields you specify below." +
                                Environment.NewLine +
                                "You can use this option to delete uneeded/unwanted library tags which other importers may have created.";
                prop.DefaultValue = MeediFier.Settings.CleanSelectedFields;
                prop.DataType = "bool";
                prop.Dependencies = "False:HIDE SelectedFieldsToCleanProp" +
                                    ";True:SHOW SelectedFieldsToCleanProp";
                return true;
            }


            if (index == counter++)
            {
                prop.GroupName = "LibraryCleanup";
                //set the internal name
                prop.Name = "SelectedFieldsToCleanProp";
                //set name shown to user
                prop.Caption = "Which tags to delete? ";
                prop.Caption = TranslationProvider.Translate(prop.Caption, importerProperties);
                //set the tool tip
                prop.HelpText = " Please specify here the names of the tags to remove, seperating with a comma (,) ";
                prop.DefaultValue = MeediFier.Settings.SelectedFieldsToClean;
                prop.DataType = "string";
                return true;
            }


            //if (index == counter++)
            //{
            //    prop.GroupName = "LibraryCleanup";
            //    //set the internal name
            //    prop.Name = "MovieNightCompatibleProp";
            //    //set name shown to user
            //    prop.Caption = "MovieNight compatibility (for Meedio version)";
            //    prop.Caption = TranslationProvider.Translate(prop.Caption, this);
            //    //set the tool tip
            //    prop.HelpText = "Enable this setting if you are a user of MovieNight media module for Meedio." + Environment.NewLine +
            //                    "Your film library tags in this case will be named according to this module's definitions.";
            //    prop.DefaultValue = Settings.MovieNightCompatibility;
            //    prop.DataType = "bool";
            //    return true;
            //}


            if (index == counter++)
            {
                prop.GroupName = "LibraryCleanup";
                //set the internal name
                prop.Name = "MeePopupProviderCompatibilityProp";
                //set name shown to user
                prop.Caption = "MeePopupProvider compatibility (for MeediOS version)";
                prop.Caption = TranslationProvider.Translate(prop.Caption, importerProperties);
                //set the tool tip
                prop.HelpText = "Enable this setting if you are a user of MeePopupProvider for MeediOS." + Environment.NewLine +
                                "A 'meePopupProvider' field will be added to all media sections containing each item's ID number.";
                prop.DefaultValue = MeediFier.Settings.MeePopupProviderCompatibility;
                prop.DataType = "bool";
                return true;
            }

            return false;
        }
    }
}
