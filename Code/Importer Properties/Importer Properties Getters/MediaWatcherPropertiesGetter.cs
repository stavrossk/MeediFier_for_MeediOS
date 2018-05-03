using CustomProperties.PropertyData;
using MeediOS.Cache;

namespace MediaFairy.ImporterPropertiesGroups
{

    internal class MediaWatcher
    {

        internal static bool MediaWatcherProperties
            (ImporterProperties importerProperties,
             int index, OptionSettings prop,
             string[] twochoices, ref int counter)
        {



            #region Media Watcher

            if (index == counter++)
            {
                //set the internal name
                prop.Name = "EnableMediaWatcherProp";
                prop.GroupName = "MediaWatcher";
                //set name shown to user
                prop.Caption = "Enable Media Watcher?";
                prop.Caption = TranslationProvider.Translate(prop.Caption, importerProperties);
                prop.GroupCaption = "                  Media Watcher";
                //set the tool tip
                //TODO: Add help tooltip
                prop.HelpText = "";
                prop.DefaultValue = Settings.MediaWatcherEnabled;
                prop.DataType = "bool";
                prop.Dependencies =
                    "False:HIDE UseRootFoldersAsDirsToWatchProp,DirectoriesToWatchProp,IncludeSubdirectoriesProp,ItemsAdditionModeProp,UseAutoSorterForMediaWatcherProp" +
                    ";True:SHOW UseRootFoldersAsDirsToWatchProp,IncludeSubdirectoriesProp,ItemsAdditionModeProp,UseAutoSorterForMediaWatcherProp";
                return true;
            }

            if (index == counter++)
            {
                //set the internal name
                prop.Name = "UseRootFoldersAsDirsToWatchProp";
                prop.GroupName = "MediaWatcher";
                //set name shown to user
                prop.Caption = "Use Root media folders as directories to watch";
                prop.Caption = TranslationProvider.Translate(prop.Caption, importerProperties);
                //set the tool tip
                //TODO: Add help tooltip
                prop.HelpText = "";
                prop.DefaultValue = Settings.UseRootFoldersAsDirsToWatch;
                prop.DataType = "bool";
                prop.Dependencies = "False:SHOW DirectoriesToWatchProp" +
                                    ";True:HIDE DirectoriesToWatchProp";
                return true;
            }

            if (index == counter++)
            {
                //set the internal name
                prop.Name = "DirectoriesToWatchProp";
                prop.GroupName = "MediaWatcher";
                //set name shown to user
                prop.Caption = "Directories to Watch: ";
                prop.Caption = TranslationProvider.Translate(prop.Caption, importerProperties);
                //set the tool tip
                //TODO: Add help tooltip
                prop.HelpText = "";
                prop.DefaultValue = Settings.DirectoriesToWatch;
                prop.DataType = "folder";
                //Prop.Dependencies = "False:SHOW AnalyzerOverrides" +
                //";True:HIDE AnalyzerOverrides";
                return true;
            }

            if (index == counter++)
            {
                //set the internal name
                prop.Name = "IncludeSubdirectoriesProp";
                prop.GroupName = "MediaWatcher";
                //set name shown to user
                prop.Caption = "Include subdirectories";
                prop.Caption = TranslationProvider.Translate(prop.Caption, importerProperties);
                //set the tool tip
                //TODO: Add help tooltip
                prop.HelpText = "";
                prop.DefaultValue = Settings.IncludeSubdirectories;
                prop.DataType = "bool";
                return true;
            }

            if (index == counter++)
            {
                //set the internal name
                prop.Name = "ItemsAdditionModeProp";
                prop.GroupName = "MediaWatcher";
                //set name shown to user
                prop.Caption = "Addition Mode for new items: ";
                prop.Caption = TranslationProvider.Translate(prop.Caption, importerProperties);
                //set the tool tip
                //TODO: Add help tooltip
                prop.HelpText = "";
                prop.DefaultValue = Settings.ItemsAdditionMode;
                prop.DataType = "string";
                twochoices[0] = "Import Only";
                twochoices[1] = "Import and Update";
                prop.CanTypeChoices = false;
                prop.Choices = twochoices;
                prop.IsMandatory = false;
                //Prop.Dependencies = "in the movie's folder:HIDE PosterFolderProp" +
                //";in the folder i specify below:SHOW PosterFolderProp";
                return true;
            }

            if (index == counter++)
            {
                //set the internal name
                prop.Name = "UseAutoSorterForMediaWatcherProp";
                prop.GroupName = "MediaWatcher";
                //set name shown to user
                prop.Caption = "Organize imported files using Media Organizer settings";
                prop.Caption = TranslationProvider.Translate(prop.Caption, importerProperties);
                //set the tool tip
                //TODO: Add help tooltip
                prop.HelpText = "";
                prop.DefaultValue = Settings.UseAutoSorterForMediaWatcher;
                prop.DataType = "bool";
                return true;
            }

            #endregion


            return false;


        }//endof function


    }//endof class


}//endof namespace
