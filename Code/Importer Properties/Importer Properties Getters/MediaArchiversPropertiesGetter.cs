using System;
using System.Collections.Generic;
using CustomProperties.PropertyData;
using MeediOS.Cache;

namespace MediaFairy.Code.Importer_Properties
{




    class MediaArchiversPropertiesGetter
    {
        internal static bool GetMediaArchiversProperties
            (ImporterProperties importerProperties, 
             int index, OptionSettings prop, 
             IList<string> twochoices, ref int counter)
        {




            if (index == counter++)
            {
                prop.GroupName = "MediaArchiver";
                //set the internal name
                prop.Name = "MediaArchiversProp";
                //set name shown to user
                prop.GroupCaption = "                      Media Archivers (reserved for future version)";
                prop.Caption = "Enable Media Archivers";
                prop.Caption = TranslationProvider.Translate(prop.Caption, importerProperties);
                //set the tool tip
                prop.HelpText = "";
                prop.DefaultValue = false;
                prop.DataType = "bool";
                prop.Dependencies = "True:SHOW FilmArchiverProp" +
                                    ";False:HIDE FilmArchiverProp,ArchiveWatchedFilmsProp,ArchiveOldFilmsProp,ArchivePreferenceFilmsProp,ArchiveLocationProp";
                return true;
            }

            if (index == counter++)
            {
                prop.GroupName = "MediaArchiver";
                //set the internal name
                prop.Name = "FilmArchiverProp";
                //set name shown to user
                prop.Caption = "Archive Films";
                prop.Caption = TranslationProvider.Translate(prop.Caption, importerProperties);
                //set the tool tip
                prop.HelpText =
                    "If you enable this option, the built-in archiver will compress and store (in the directory you provide below)" +
                    Environment.NewLine +
                    "a zip archive for each watched film,  containing the entire film's directory." + Environment.NewLine +
                    "The original directory containing the film will be deleted, but only when the archiving operation is succesfull." +
                    Environment.NewLine +
                    "Please note that in order to determine if a film is watced or not, the plugin will look for a tag named 'Watched' in your film library.";
                prop.DefaultValue = false;
                prop.DataType = "bool";
                prop.Dependencies = "True:SHOW ArchiveWatchedFilmsProp,ArchiveOldFilmsProp" +
                                    ";False:HIDE ArchiveWatchedFilmsProp,ArchiveOldFilmsProp,ArchivePreferenceFilmsProp,ArchiveLocationProp";
                return true;
            }

            if (index == counter++)
            {
                prop.GroupName = "MediaArchiver";
                //set the internal name
                prop.Name = "ArchiveWatchedFilmsProp";
                //set name shown to user
                prop.Caption = "Archive watched films";
                prop.Caption = TranslationProvider.Translate(prop.Caption, importerProperties);
                //set the tool tip
                prop.HelpText = "";
                prop.DefaultValue = Settings.ArchiveWatchedFilms;
                prop.DataType = "bool";
                prop.Dependencies = "True:SHOW ArchivePreferenceFilmsProp";
                return true;
            }

            if (index == counter++)
            {
                prop.GroupName = "MediaArchiver";
                //set the internal name
                prop.Name = "ArchiveOldFilmsProp";
                //set name shown to user
                prop.Caption = "Archive Old Films";
                prop.Caption = TranslationProvider.Translate(prop.Caption, importerProperties);
                //set the tool tip
                prop.HelpText = "";
                prop.DefaultValue = Settings.ArchiveOldFilms;
                prop.DataType = "bool";
                prop.Dependencies = "True:SHOW ArchivePreferenceFilmsProp";
                return true;
            }

            if (index == counter++)
            {
                prop.GroupName = "MediaArchiver";
                //set the internal name
                prop.Name = "ArchivePreferenceFilmsProp";
                //set name shown to user
                prop.Caption = "Archive preference for films: ";
                prop.Caption = TranslationProvider.Translate(prop.Caption, importerProperties);
                //set the tool tip
                prop.HelpText = "";
                prop.DefaultValue = "Delete";
                prop.DataType = "string";
                twochoices[0] = "Archive";
                twochoices[1] = "Delete";
                prop.Choices = twochoices;
                prop.CanTypeChoices = false;
                prop.Dependencies = "Delete:HIDE ArchiveLocationProp" +
                                    ";Archive:SHOW ArchiveLocationProp";
                return true;
            }

            if (index == counter++)
            {
                prop.GroupName = "MediaArchiver";
                //set the internal name
                prop.Name = "ArchiveLocationProp";
                //set name shown to user
                prop.Caption = "Directory to store archives: ";
                prop.Caption = TranslationProvider.Translate(prop.Caption, importerProperties);
                //set the tool tip
                prop.HelpText = "Please specify the directory in which the zip archives of your watched films will be stored: ";
                prop.DefaultValue = Settings.ArchiveLocation;
                prop.DataType = "folder";
                return true;
            }

            return false;
        }
    }



}

