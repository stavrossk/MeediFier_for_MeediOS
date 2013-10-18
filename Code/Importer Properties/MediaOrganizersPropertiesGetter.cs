using System;
using System.Collections.Generic;
using CustomProperties.PropertyData;
using MeediOS.Cache;




namespace MeediFier.Code.Importer_Properties
{



    class MediaOrganizersPropertiesGetter
    {








        internal static bool GetMediaOrganizersProperties
            (ImporterProperties importerProperties, 
             int index, OptionSettings prop,
             IList<string> twochoices, ref int counter)
        {



            #region Enable / disable media organizers




            if (index == counter++)
            {
                prop.GroupName = "AutoSorter";
                //set the internal name
                prop.Name = "MediaOrganizersProp";
                //set name shown to user
                prop.GroupCaption = "                         Media Organizers";
                prop.Caption = "Enable Media Organizers ";
                prop.Caption = TranslationProvider.Translate(prop.Caption, importerProperties);
                //set the tool tip
                prop.HelpText = "";
                prop.DefaultValue = false;
                prop.DataType = "bool";
                prop.Dependencies =
                    "False:HIDE WantSortingFilmsProp,WantSortingTvShowsProp,WantSortingMusicProp,PromptBeforeMovingProp,SortingDestinationFilmsProp,SortingDestinationTvShowsProp,SortingDestinationMusicProp" +
                    ";False:HIDEGROUP OrganizingStructures" +
                    ";True:SHOW WantSortingFilmsProp,WantSortingTvShowsProp,WantSortingMusicProp,PromptBeforeMovingProp";
                return true;
            }

            if (index == counter++)
            {
                prop.GroupName = "AutoSorter";
                //set the internal name
                prop.Name = "WantSortingFilmsProp";
                //set name shown to user
                //Prop.GroupCaption = "                         Media Organizer";
                prop.Caption = "Automatically organize films ";
                prop.Caption = TranslationProvider.Translate(prop.Caption, importerProperties);
                //set the tool tip
                prop.HelpText = "If this option is enabled, MediaFairy will automatically move or rename the films" +
                                Environment.NewLine +
                                "which it has identified, accroding to your specifications below." + Environment.NewLine +
                                "This feature will ensure a default and well organized structure for your movie files and their related data. ";
                prop.DefaultValue = MeediFier.Settings.WantSortingFilms;
                prop.DataType = "bool";
                prop.Dependencies = "False:HIDE SortingDestinationFilmsProp" +
                                    ";True:SHOW SortingDestinationFilmsProp" +
                                    ";True:SHOWGROUP OrganizingStructures";
                return true;
            }
            if (index == counter++)
            {
                prop.GroupName = "AutoSorter";
                //set the internal name
                prop.Name = "WantSortingTvShowsProp";
                //set name shown to user
                prop.Caption = "Automatically organize tv series ";
                prop.Caption = TranslationProvider.Translate(prop.Caption, importerProperties);
                //set the tool tip
                prop.HelpText =
                    "If this option is enabled, MediaFairy will automatically move or rename the video files of tv series" +
                    Environment.NewLine +
                    "it has already identified, accroding to your specifications below." + Environment.NewLine +
                    "This feature will ensure a default and well organized structure for your TV series and their related files. ";
                prop.DefaultValue = MeediFier.Settings.WantSortingTvShows;
                prop.DataType = "bool";
                prop.Dependencies = "False:HIDE SortingDestinationTvShowsProp" +
                                    ";True:SHOW SortingDestinationTvShowsProp" +
                                    ";True:SHOWGROUP OrganizingStructures";
                return true;
            }
            if (index == counter++)
            {
                prop.GroupName = "AutoSorter";
                //set the internal name
                prop.Name = "WantSortingMusicProp";
                //set name shown to user
                prop.Caption = "Automatically organize music";
                prop.Caption = TranslationProvider.Translate(prop.Caption, importerProperties);
                //set the tool tip
                prop.HelpText = "If this option is enabled, MediaFairy will automatically move or rename the audio tracks" +
                                Environment.NewLine +
                                "it has already identified, accroding to your specifications below." + Environment.NewLine +
                                "This feature will ensure a default and well organized structure for your audio files and their related data. ";
                prop.DefaultValue = MeediFier.Settings.WantSortingMusic;
                prop.DataType = "bool";
                prop.Dependencies = "False:HIDE SortingDestinationMusicProp" +
                                    ";True:SHOW SortingDestinationMusicProp" +
                                    ";True:SHOWGROUP OrganizingStructures";
                return true;
            }
            if (index == counter++)
            {
                prop.GroupName = "AutoSorter";
                //set the internal name
                prop.Name = "PromptBeforeMovingProp";
                //set name shown to user
                prop.Caption = "Prompt me before any file operation";
                prop.Caption = TranslationProvider.Translate(prop.Caption, importerProperties);
                //set the tool tip
                prop.HelpText = "If you enable this option, MediaFairy will ask for your approval before performing" +
                                Environment.NewLine +
                                "any renaming or moving operation on media files. This option exists mainly for debugging purposes. ";
                prop.DefaultValue = MeediFier.Settings.PromptBeforeMoving;
                prop.DataType = "bool";
                return true;
            }

            #endregion

            #region Destination Directories

            if (index == counter++)
            {
                prop.GroupName = "AutoSorter";
                //set the internal name
                prop.Name = "SortingDestinationFilmsProp";
                //set name shown to user
                prop.Caption = "Destination directory for films: ";
                prop.Caption = TranslationProvider.Translate(prop.Caption, importerProperties);
                //set the tool tip
                prop.HelpText = @"Please specify the destination directory in which you want your sorted films" +
                                Environment.NewLine +
                                "and their related files to be moved by MediaFairy." + Environment.NewLine +
                                "Tip: specify the folder where your films are already contained and a blank folder mask for a simple renaming.";
                prop.DefaultValue = MeediFier.Settings.SortingDestinationFilms;
                prop.DataType = "folder";
                return true;
            }
            if (index == counter++)
            {
                prop.GroupName = "AutoSorter";
                //set the internal name
                prop.Name = "SortingDestinationTvShowsProp";
                //set name shown to user
                prop.Caption = "Destination directory for tv series: ";
                prop.Caption = TranslationProvider.Translate(prop.Caption, importerProperties);
                //set the tool tip
                prop.HelpText = @"Please specify the destination directory in which you want your sorted TV series episodes" +
                                Environment.NewLine +
                                "and their related files to be moved by MediaFairy." + Environment.NewLine +
                                "Tip: specify the folder where your TV episodes are already contained and blank folder mask for a simple renaming.";
                prop.DefaultValue = MeediFier.Settings.SortingDestinationTvShows;
                prop.DataType = "folder";
                return true;
            }
            if (index == counter++)
            {
                prop.GroupName = "AutoSorter";
                //set the internal name
                prop.Name = "SortingDestinationMusicProp";
                //set name shown to user
                prop.Caption = "Destination directory for music: ";
                prop.Caption = TranslationProvider.Translate(prop.Caption, importerProperties);
                //set the tool tip
                prop.HelpText = @"Please specify the destination directory where you want your sorted music tracks" +
                                Environment.NewLine +
                                "and their related files to be moved by MediaFairy." + Environment.NewLine +
                                "Tip: specify the folder where your audio tracks are already contained and blank folder mask for a simple renaming.";
                prop.DefaultValue = MeediFier.Settings.SortingDestinationMusic;
                prop.DataType = "folder";
                return true;
            }

            #endregion

            #region Organizing Structures

            if (index == counter++)
            {
                prop.GroupName = "OrganizingStructures";
                //set the internal name
                prop.Name = "DirectoryStructureProp";
                //set name shown to user
                prop.Caption = "Preferred directory structures: ";
                prop.Caption = TranslationProvider.Translate(prop.Caption, importerProperties);
                //set the tool tip
                prop.HelpText = "What is the directory structure in which you want your media files to be organized?" +
                                Environment.NewLine +
                                "MediaFairy will use known information relevant to each media type" + Environment.NewLine +
                                "to give the appropriate naming to folders and media files, according to your selected Media Organizer operating mode:" +
                                Environment.NewLine +
                                "'Simple' mode will use the default directory structures for each media type," +
                                Environment.NewLine +
                                "while 'Advanced' lets you specify your own directory structure for each media type using a tag mask." +
                                Environment.NewLine +
                                "Please refer to the plugin's Wiki web page for more information on directory structures. ";
                prop.DefaultValue = MeediFier.Settings.DirectoryStructure;
                prop.DataType = "string";

                twochoices[0] = "Simple- Default directory structures";
                twochoices[1] = "Advanced- specify directory tag masks below";

                prop.Choices = twochoices;
                prop.CanTypeChoices = false;
                prop.Dependencies =
                    "Simple- Default directory structures:HIDE DirectoryTagMaskFilmsProp,DirectoryTagMaskTvShowsProp,DirectoryTagMaskMusicProp" +
                    ";Advanced- specify directory tag masks below:SHOW DirectoryTagMaskFilmsProp,DirectoryTagMaskTvShowsProp,DirectoryTagMaskMusicProp";

                return true;
            }
            if (index == counter++)
            {
                prop.GroupName = "OrganizingStructures";
                //set the internal name
                prop.Name = "DirectoryTagMaskFilmsProp";
                //set name shown to user
                prop.Caption = "Folder tag mask for films? ";
                prop.Caption = TranslationProvider.Translate(prop.Caption, importerProperties);
                //set the tool tip
                prop.HelpText = " Please type your desired folder structure in the form of a tag mask." + Environment.NewLine +
                                "For each additional directory level you want to be created based on the value of an item's specific tag," +
                                Environment.NewLine +
                                "include this folder name in < and > brackets." + Environment.NewLine +
                                "For example, if you wanted your directory structure to include a folder for Film Genres" +
                                Environment.NewLine +
                                "and below this level each movie to reside in it's own folder named after the film's name," +
                                Environment.NewLine +
                                @"you would use this tag mask: <Genre>\<Title> . ";
                prop.DefaultValue = MeediFier.Settings.DirectoryTagMaskFilms;
                prop.DataType = "string";
                return true;
            }
            if (index == counter++)
            {
                prop.GroupName = "OrganizingStructures";
                //set the internal name
                prop.Name = "DirectoryTagMaskTvShowsProp";
                //set name shown to user
                prop.Caption = "Folder tag mask for tv shows? ";
                prop.Caption = TranslationProvider.Translate(prop.Caption, importerProperties);
                //set the tool tip
                prop.HelpText =
                    @" Please type your desired folder structure in the form of a tag mask. For each additional directory level you want to be created based on the value of an item's specific tag, include this folder name in < and > brackets. For example, if you wanted your directory structure to include a folder for Film Genres and below this level each movie to reside in it's own folder named after the film's name, you would use this tag mask: <Genre>\<Title> . ";
                prop.DefaultValue = MeediFier.Settings.DirectoryTagMaskTvShows;
                prop.DataType = "string";
                return true;
            }
            if (index == counter++)
            {
                prop.GroupName = "OrganizingStructures";
                //set the internal name
                prop.Name = "DirectoryTagMaskMusicProp";
                //set name shown to user
                prop.Caption = "Folder tag mask for music? ";
                prop.Caption = TranslationProvider.Translate(prop.Caption, importerProperties);
                //set the tool tip
                prop.HelpText =
                    @" Please type your desired folder structure in the form of a tag mask. For each additional directory level you want to be created based on the value of an item's specific tag, include this folder name in < and > brackets. For example, if you wanted your directory structure to include a folder for Film Genres and below this level each movie to reside in it's own folder named after the film's name, you would use this tag mask: <Genre>\<Title> . ";
                prop.DefaultValue = MeediFier.Settings.DirectoryTagMaskMusic;
                prop.DataType = "string";
                return true;
            }

            #endregion

            return false;
        }
    }
}
