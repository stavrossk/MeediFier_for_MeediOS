using System;
using CustomProperties.PropertyData;
using MeediOS.Cache;

namespace MediaFairy.ImporterPropertiesGroups
{
    internal class MediaSections
    {


        internal static bool MediaSectionsProperties
            (int index, OptionSettings prop, string[] sectionChoices, ref int counter)
        {

            #region Movies Section
            if (index == counter++)
            {
                //set the internal name
                prop.Name = "FilmsLibraryProp";
                //set name shown to user
                prop.Caption = "Please select your Movies library section: ";
                //set the tool tip
                prop.GroupName = "MediaSections";
                prop.GroupCaption = "                                Media Sections Configuration";
                //Prop.IsMandatory = true;
                prop.HelpText =
                    "Please select your Movies library section from the list of available library sections.";
                prop.DefaultValue = Settings.FilmsLibrary;
                prop.DataType = "string";
                //Prop.IsMandatory = true;
                //MessageBox.Show("Sections= " + MeedioLibrary.SectionCount);
                prop.Choices = sectionChoices;
                prop.CanTypeChoices = false;
                return true;
            }
            #endregion

            if (index == counter++)
            {
                //set the internal name
                prop.Name = "TvShowsLibraryProp";
                //set name shown to user
                prop.GroupName = "MediaSections";
                prop.Caption = "Please select your TV Episodes library section: ";
                //Prop.IsMandatory = true;
                prop.HelpText =
                    "Please select your TV Episodes library section from the list of available library sections.";
                prop.DefaultValue = Settings.TvShowsLibrary;
                prop.DataType = "string";
                //MessageBox.Show("Sections= " + MeedioLibrary.SectionCount);
                prop.Choices = sectionChoices;
                prop.CanTypeChoices = false;
                return true;
            }




            if (index == counter++)
            {
                //set the internal name
                prop.Name = "MusicLibraryProp";
                //set name shown to user
                prop.GroupName = "MediaSections";
                //Prop.GroupCaption = "                                Media Sections";
                prop.Caption = "Please select your Music library section: ";
                //Prop.IsMandatory = true;
                //set the tool tip
                prop.HelpText =
                    "Please select your Music library section from the list of available library sections.";
                prop.DefaultValue = Settings.MusicLibrary;
                prop.DataType = "string";
                //Prop.IsMandatory = true;
                //MessageBox.Show("Sections= " + MeedioLibrary.SectionCount);
                prop.Choices = sectionChoices;
                prop.CanTypeChoices = false;
                return true;
            }

            if (index == counter++)
            {
                //set the internal name
                prop.Name = "NewsLibraryProp";
                //set name shown to user
                prop.GroupName = "MediaSections";
                //Prop.GroupCaption = "                                Media Sections";
                prop.Caption = "Please select your RSS News Section: ";
                //Prop.IsMandatory = true;
                //set the tool tip
                prop.HelpText =
                    "Please select your RSS News library section from the list of available library sections.";
                prop.DefaultValue = Settings.NewsLibrary;
                prop.DataType = "string";
                //Prop.IsMandatory = true;
                //MessageBox.Show("Sections= " + MeedioLibrary.SectionCount);
                prop.Choices = sectionChoices;
                prop.CanTypeChoices = false;
                return true;
            }

            #region [DEPRECATED] e-Books Section
            //if (index == counter++)
            //{
            //    //set the internal name
            //    prop.Name = "BooksLibraryProp";
            //    //set name shown to user
            //    prop.GroupName = "MediaSections";
            //    //Prop.GroupCaption = "                                Media Sections";
            //    prop.Caption = "Please select your e-Books Section: ";
            //    //Prop.IsMandatory = true;
            //    //set the tool tip
            //    prop.HelpText =
            //        "Please select your Books Section from the provided list. If you don't want MediaFairy to update this library, just uncheck the 'Update Books Section' option.";
            //    prop.DefaultValue = Settings.BooksLibrary;
            //    prop.DataType = "string";
            //    //Prop.IsMandatory = true;
            //    //MessageBox.Show("Sections= " + MeedioLibrary.SectionCount);
            //    prop.Choices = sectionChoices;
            //    prop.CanTypeChoices = false;
            //    return true;
            //}
            #endregion

            #region [DEPRECATED] Games Section
            //if (index == counter++)
            //{
            //    //set the internal name
            //    prop.Name = "GamesLibraryProp";
            //    //set name shown to user
            //    prop.GroupName = "MediaSections";
            //    //Prop.GroupCaption = "                                Media Sections";
            //    prop.Caption = "Please select your Games Section: ";
            //    //Prop.IsMandatory = true;
            //    //set the tool tip
            //    prop.HelpText =
            //        "Please select your Games Section from the provided list. If you don't want MediaFairy to update this library, just uncheck the 'Import and update Games' option.";
            //    prop.DefaultValue = Settings.GamesLibrary;
            //    prop.DataType = "string";
            //    //Prop.IsMandatory = true;
            //    //MessageBox.Show("Sections= " + MeedioLibrary.SectionCount);
            //    prop.Choices = sectionChoices;
            //    prop.CanTypeChoices = false;
            //    return true;
            //}
            #endregion

            return false;
        }


        internal static bool FimImportingOptions
            (ImporterProperties importerProperties, 
            int index, OptionSettings prop,
            string[] twochoices, ref int counter)
        {
            #region Film Importing options

            if (index == counter++)
            {
                prop.GroupName = "MovieTitleLocator";
                //set the internal name
                prop.Name = "ImportDvdImagesProp";
                //set name shown to user
                prop.GroupCaption = "                              Film Importing Options";
                prop.Caption = "Import DVD images containing films";
                prop.Caption = TranslationProvider.Translate(prop.Caption, importerProperties);
                //set the tool tip
                prop.HelpText = "";
                prop.DefaultValue = Settings.ImportDvdImages;
                prop.DataType = "bool";
                return true;
            }
            if (index == counter++)
            {
                prop.GroupName = "MovieTitleLocator";
                //set the internal name
                prop.Name = "ImportBluRayImagesProp";
                //set name shown to user
                prop.GroupCaption = "                              Film Importing Options";
                prop.Caption = "Import BluRay images containing films";
                prop.Caption = TranslationProvider.Translate(prop.Caption, importerProperties);
                //set the tool tip
                prop.HelpText = "";
                prop.DefaultValue = Settings.ImportBluRayImages;
                prop.DataType = "bool";
                return true;
            }

            if (index == counter++)
            {
                prop.GroupName = "MovieTitleLocator";
                //set the internal name
                prop.Name = "ImportDvdFoldersProp";
                //set name shown to user
                prop.Caption = "Import DVD film back-ups (preserved disc structure)";
                prop.Caption = TranslationProvider.Translate(prop.Caption, importerProperties);
                //set the tool tip
                prop.HelpText = "";
                prop.DefaultValue = Settings.ImportDvdFolders;
                prop.DataType = "bool";
                return true;
            }
            if (index == counter++)
            {
                prop.GroupName = "MovieTitleLocator";
                //set the internal name
                prop.Name = "ImportRarFilmsProp";
                //set name shown to user
                prop.Caption = "Import multi-part RAR archives as films";
                prop.Caption = TranslationProvider.Translate(prop.Caption, importerProperties);
                //set the tool tip
                prop.HelpText = "";
                prop.DefaultValue = Settings.ImportFilmsInRarArchives;
                prop.DataType = "bool";
                return true;
            }


            if (index == counter++)
            {
                prop.GroupName = "MovieTitleLocator";
                //set the internal name
                prop.Name = "MovieTitleLocatorProp";
                //set name shown to user
                prop.Caption = "Override automated film title locator";
                prop.Caption = TranslationProvider.Translate(prop.Caption, importerProperties);
                //set the tool tip
                prop.HelpText =
                    " If the Automated film title locator fails to correctly locate the titles of your films in their paths (file name or folder name)," +
                    Environment.NewLine + "please check this setting to manually override it.";
                prop.DefaultValue = Settings.OverrideAutomatedMovieTitleLocator;
                prop.DataType = "bool";
                prop.Dependencies = "False:HIDE MovieTitleLocationProp" +
                                    ";True:SHOW MovieTitleLocationProp";
                return true;
            }
            if (index == counter++)
            {
                prop.GroupName = "MovieTitleLocator";
                //set the internal name
                prop.Name = "MovieTitleLocationProp";
                //set name shown to user
                prop.Caption = "Movie's title location in path: ";
                prop.Caption = TranslationProvider.Translate(prop.Caption, importerProperties);
                //set the tool tip
                prop.HelpText = "Please specify here the location of your movies' titles in their path.";
                prop.DefaultValue = Settings.MovieTitleLocationInPath;
                prop.DataType = "string";
                twochoices[0] = "Parent folder";
                twochoices[1] = "Video filename";
                prop.CanTypeChoices = false;
                prop.Choices = twochoices;
                prop.IsMandatory = false;
                return true;
            }

            #endregion

            return false;
        }
    }
}
