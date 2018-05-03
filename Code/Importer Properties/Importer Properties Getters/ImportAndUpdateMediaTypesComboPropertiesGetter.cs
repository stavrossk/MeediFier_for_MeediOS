using System;
using CustomProperties.PropertyData;
using MeediOS.Cache;

namespace MediaFairy.Importer_Properties
{
    class ImportAndUpdateMediaTypesCombo
    {
        internal bool ImportAndUpdateMediaTypesComboProperties(int index, OptionSettings prop, ref int counter)
        {
            #region [DEPRECATED] Media Types to Import & Update:

            #region Movies

            if (index == counter++)
            {
                //set the internal name
                prop.Name = "WantToImportAndUpdateFilmsProp";
                prop.GroupName = "SimpleMode";
                //set name shown to user
                prop.Caption = "Import and update Movies";
                prop.Caption = TranslationProvider.Translate(prop.Caption, this);
                //Prop.GroupCaption = "                                 Media Importer";
                //set the tool tip
                prop.HelpText = "With this setting enabled, the plugin will automatically import, identify" +
                                Environment.NewLine +
                                "and download media information & images for all films in your Media Sources." +
                                Environment.NewLine +
                                "Important: This setting will enable both media importing *AND* updating for this media type." +
                                Environment.NewLine +
                                "If you need to independently enable the Media Importing or Updating for this media type, please un-tick this checkbox and use Advanced Mode instead. ";
                prop.DefaultValue = Settings.WantToImportAndUpdateFilms;
                prop.DataType = "bool";
                prop.Dependencies = "False:HIDEGROUP MovieTitleLocator,FilmDetails,FilmCovers,AlbumCovers" +
                                    ";True:SHOWGROUP MovieTitleLocator" +
                                    ";True:HIDEGROUP FilmDetails,FilmCovers,AlbumCovers";
                return true;
            }

            #endregion

            #region TV Series

            if (index == counter++)
            {
                //set the internal name
                prop.Name = "WantToImportAndUpdateTvSeriesProp";
                prop.GroupName = "SimpleMode";
                //set name shown to user
                prop.Caption = "Import and update Tv Series";
                prop.Caption = TranslationProvider.Translate(prop.Caption, this);
                prop.HelpText = "With this setting enabled, the plugin will automatically import, identify" +
                                Environment.NewLine +
                                "and download media information & images for all episodes of TV series in your Media Sources. ";
                prop.DefaultValue = Settings.ImportAndUpdateTvSeries;
                prop.DataType = "bool";
                prop.Dependencies = "False:HIDEGROUP AlbumCovers,FilmCovers" +
                                    ";True:HIDEGROUP AlbumCovers,FilmCovers";
                return true;
            }

            #endregion

            #region Music

            if (index == counter++)
            {
                //set the internal name
                prop.Name = "WantToImportAndUpdateMusicProp";
                prop.GroupName = "SimpleMode";
                //set name shown to user
                prop.Caption = "Import and update Music";
                prop.Caption = TranslationProvider.Translate(prop.Caption, this);
                //Prop.GroupCaption = "                                 Media Importer";
                //set the tool tip
                prop.HelpText = "With this setting enabled, the plugin will automatically import, identify" +
                                Environment.NewLine +
                                "and download media information & images for all music in your Media Sources. ";
                prop.DefaultValue = Settings.WantToImportAndUpdateMusic;
                prop.DataType = "bool";
                prop.Dependencies = "False:HIDEGROUP AlbumCovers,FilmCovers" +
                                    ";True:HIDEGROUP AlbumCovers,FilmCovers";
                return true;
            }

            #endregion

            #region Games

            if (index == counter++)
            {
                //set the internal name
                prop.Name = "WantToImportAndUpdateGamesProp";
                prop.GroupName = "SimpleMode";
                //set name shown to user
                prop.Caption = "Import and update Games";
                prop.Caption = TranslationProvider.Translate(prop.Caption, this);
                //Prop.GroupCaption = "                                 Media Importer";
                //set the tool tip
                prop.HelpText = "With this setting enabled, the plugin will automatically import, identify" + Environment.NewLine +
                                "and download media information & images for the PC games in your games shortcuts' location. ";
                prop.DefaultValue = Settings.WantToImportAndUpdateGames;
                prop.DataType = "bool";
                prop.Dependencies = "False:HIDE GameShortcutsProp" +
                                    ";True:SHOW GameShortcutsProp";
                return true;
            }


            if (index == counter++)
            {
                //set the internal name
                prop.Name = "GameShortcutsProp";
                prop.GroupName = "SimpleMode";
                //set name shown to user
                prop.Caption = "Game shortcuts location:";
                prop.Caption = TranslationProvider.Translate(prop.Caption, this);
                //Prop.GroupCaption = "                                 Media Importer";
                //set the tool tip
                prop.HelpText = "Please provide the location of the shortcuts for your PC games." + Environment.NewLine +
                                "MediaFairy will first use the manifest information of the game executable to recognize the game's title." + Environment.NewLine +
                                "If manifest info is not available, MF will use the shortcut's name instead to get the title of the game.";
                prop.DefaultValue = Settings.GameShortcutsLocation;
                prop.DataType = "folder";
                return true;
            }

            #endregion

            #region Books

            if (index == counter++)
            {
                //set the internal name
                prop.Name = "WantToImportAndUpdateBooksProp";
                prop.GroupName = "SimpleMode";
                //set name shown to user
                prop.Caption = "Import and update Books (reserved for future version)";
                prop.Caption = TranslationProvider.Translate(prop.Caption, this);
                //Prop.GroupCaption = "                                 Media Importer";
                //set the tool tip
                prop.HelpText = "With this setting enabled, the plugin will automatically import, identify" +
                                Environment.NewLine +
                                "and download media information & images for all digital books in your Media Sources. ";
                prop.DefaultValue = Settings.WantToImportAndUpdateBooks;
                prop.DataType = "bool";
                prop.Dependencies = "False:HIDEGROUP AlbumCovers,FilmCovers" +
                                    ";True:HIDEGROUP AlbumCovers,FilmCovers";
                return true;
            }

            #endregion

            #endregion Media Types to Import & Update

            return false;
        }
    }
}
