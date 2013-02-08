using System;
using System.Collections.Generic;
using CustomProperties.PropertyData;
using MeediOS.Cache;

namespace MediaFairy.Code.Importer_Properties
{



    class FilmDetailsPropertiesGetter
    {




        internal static bool GetFilmDetailsProperties
            (ImporterProperties importerProperties, int index, OptionSettings prop,
             IList<string> twochoices, ref int counter)
        {



            if (index == counter++)
            {
                //set the internal name
                prop.Name = "PopulateIMDbDetailsProp";
                //set name shown to user
                prop.Caption = "Download Film Details from IMDb";
                prop.Caption = TranslationProvider.Translate(prop.Caption, importerProperties);
                prop.GroupCaption = "                              Film Details";
                prop.GroupName = "FilmDetails";
                //set the tool tip
                prop.HelpText = "Would you like to use IMDb (International Movie Database) to download details for your films?";
                prop.DefaultValue = MediaFairy.Settings.FilmIMDbDetailsDownloaderIsEnabled;
                prop.DataType = "bool";
                return true;
            }
            if (index == counter++)
            {
                //set the internal name
                prop.Name = "PopulateOSdbDetailsProp";
                //set name shown to user
                prop.Caption = "Download Film Details from OSdb";
                prop.Caption = TranslationProvider.Translate(prop.Caption, importerProperties);
                prop.GroupName = "FilmDetails";
                //set the tool tip
                prop.HelpText = "Would you like to use OSdb (opensubtitles.org) as one of the film details online sources?" +
                                Environment.NewLine +
                                "MediaFairy will download film details from OSdb only if IMDb is offline or unreachable";
                prop.DefaultValue = MediaFairy.Settings.FilmOSDbDetailsDownloaderIsEnabled;
                prop.DataType = "bool";
                return true;
            }
            if (index == counter++)
            {
                //set the internal name
                prop.Name = "PopulateTMDbDetailsProp";
                //set name shown to user
                prop.Caption = "Download additional film details from TMDb";
                prop.Caption = TranslationProvider.Translate(prop.Caption, importerProperties);
                prop.GroupName = "FilmDetails";
                //set the tool tip
                prop.HelpText = " Would you like additional film details to be downloaded from TMDb (www.themoviedb.org)?" +
                                Environment.NewLine +
                                "(TMDb Rating, Media Type, Budget, Revenue, film HomePage, Production Countries, Trailer)";
                prop.DefaultValue = MediaFairy.Settings.TMDbFilmDetailsEnabled;
                prop.DataType = "bool";
                return true;
            }


            //if (index == counter++)
            //{
            //    //set the internal name
            //    prop.Name = "PopulateAMGDetailsProp";
            //    //set name shown to user
            //    prop.Caption = "Download additional film details from AMG";
            //    prop.Caption = TranslationProvider.Translate(prop.Caption, importerProperties);
            //    prop.GroupName = "FilmDetails";
            //    //set the tool tip
            //    prop.HelpText = " Would you like additional film details to be downloaded from AMG (allmovie.com)?" +
            //                    Environment.NewLine +
            //                    "(Plot synopsis, Editorial review, Category, Types, Flags, Keywords, Themes, Tones)";
            //    prop.DefaultValue = MediaFairy.Settings.AllMediaGuideFilmDetailsEnabled;
            //    prop.DataType = "bool";
            //    return true;
            //}



            //if (index == counter++)
            //{
            //    //set the internal name
            //    prop.Name = "PopulateSevenVideoNetDetailsProp";
            //    //set name shown to user
            //    prop.Caption = "Download Greek film details from Seven Video Net";
            //    prop.Caption = TranslationProvider.Translate(prop.Caption, importerProperties);
            //    prop.GroupName = "FilmDetails";
            //    //set the tool tip
            //    prop.HelpText =
            //        " Would you like to automatically download Greek film details from Seven Video Net (videoseven.gr)?";
            //    prop.DefaultValue = MediaFairy.Settings.SevenFilmDetailsEnabled;
            //    prop.DataType = "bool";
            //    return true;
            //}




            if (index == counter++)
            {
                //set the internal name
                prop.Name = "WriteFilmDescriptorsProp";
                //set name shown to user
                prop.Caption = "Save film descriptors ";
                prop.Caption = TranslationProvider.Translate(prop.Caption, importerProperties);
                prop.GroupName = "FilmDetails";
                //set the tool tip
                prop.HelpText =
                    "Film descriptors are standrard .xml files which contain a film's identifying information and details." +
                    Environment.NewLine +
                    "If this feature is enabled, a film descriptor will be saved (in the film's directory) for each film the plugin successfully identifies." +
                    Environment.NewLine +
                    "This procedure offers greatly increased updating performance if/when the film needs to be imported again," +
                    Environment.NewLine +
                    "because if a film descriptor is found, the plugin will use this file to obtain the film's details, thus skipping the video identifying phase. ";
                prop.DefaultValue = MediaFairy.Settings.FilmDescriptorsAreEnabled;
                prop.DataType = "bool";
                return true;
            }


            if (index == counter++)
            {
                //set the internal name
                prop.Name = "FilmDescriptorConventionProp";
                //set name shown to user
                prop.Caption = "Film descriptors naming convention ";
                prop.Caption = TranslationProvider.Translate(prop.Caption, importerProperties);
                prop.GroupName = "FilmDetails";
                //set the tool tip
                prop.HelpText = "Which naming convention would you like to use for your film descriptors?" + Environment.NewLine +
                                "MediaFairy is compatible with XBMC's .nfo files." + Environment.NewLine +
                                "MediaFairy's film descriptors are named <VideoHash>.xml while XBMC's are named <MovieName>.nfo";
                prop.DefaultValue = MediaFairy.Settings.FilmDescriptorConvention;
                prop.DataType = "string";
                prop.CanTypeChoices = false;
                twochoices[0] = "MediaFairy";
                twochoices[1] = "XBMC";
                prop.Choices = twochoices;
                return true;
            }

            return false;
        }
    }
}
