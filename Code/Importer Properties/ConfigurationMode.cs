using System;
using CustomProperties.PropertyData;
using MeediOS.Cache;




namespace MeediFier.ImporterPropertiesGroups
{

    class ConfigurationMode
    {



        internal static bool ConfigurationModeProperty
            (ImporterProperties importerProperties, 
            int index, OptionSettings prop, ref int counter)
        {






            if (index == counter++)
            {
                prop.Name = "Mode";
                prop.Caption = "Configuration Mode: ";
                prop.Caption = TranslationProvider.Translate(prop.Caption, importerProperties);

                prop.HelpText = "The Configuration Mode menu categorizes the available settings of the plugin" +
                                Environment.NewLine +
                                " according to the following settings groups:" + Environment.NewLine +
                                "Media Sections, Media Importers, Media Importing Settings, " + Environment.NewLine + 
                                "Media Updaters, Media Updaters Settings," + Environment.NewLine + 
                                " Media Organizers, User Interface Settings, Tools, Compatibility";
                
                prop.DataType = "string";


                prop.Choices = new[]
                {"Media Sections", 
                 "Media Importers",
                 "Media Updaters",
                 "Media Organizers",
                 "Advanced"};
                
                
                prop.CanTypeChoices = false;
                prop.Dependencies =

                    #region Media Sections

                    "Media Sections:HIDEGROUP"
                    +
                    " MediaImporter,"
                    +
                    "MediaAnalyzer,"
                    +
                    "MovieTitleLocator,"
                    +
                    "MediaUpdaters,"
                    +
                    "FilmUpdater,"
                    +
                    "TvUpdater,"
                    +
                    "MusicUpdater,"
                    +
                    "VideoIdentifier,"
                    +
                    "FilmDetails,"
                    +
                    "FilmCovers,"
                    +
                    "AlbumCovers,"
                    +
                    "AutoSorter,"
                    +
                    "OrganizingStructures,"
                    +
                    "Diagnostics,"
                    +
                    "LibraryCleanup,"
                    +
                    "UserInterface,"
                    +
                    "RSSimporter"


                    +


                    ";Media Sections:"
                    +
                    "SHOWGROUP "
                    +
                    "MediaSections"

                    #endregion Media Sections




                    +




                    #region Media Importers

                    ";Media Importers:HIDEGROUP"
                    +
                    " MediaSections,"
                    +
                    "MediaAnalyzer,"
                    +
                    "MovieTitleLocator,"
                    +
                    "MediaUpdaters,"
                    +
                    "FilmUpdater,"
                    +
                    "TvUpdater,"
                    +
                    "MusicUpdater,"
                    +
                    "VideoIdentifier,"
                    +
                    "FilmDetails,"
                    +
                    "FilmCovers,"
                    +
                    "AlbumCovers,"
                    +
                    "AutoSorter,"
                    +
                    "OrganizingStructures,"
                    +
                    "Diagnostics,"
                    +
                    "LibraryCleanup,"
                    +
                    "UserInterface,"
                    +
                    "RSSimporter"


                    +


                    ";Media Importers:"
                    +
                    "SHOWGROUP "
                    +
                    "MediaImporter"


                    #endregion Media Importers





                    +




                    #region Media Updaters

                    ";Media Updaters:"
                    +
                    "SHOWGROUP "
                    +
                    "MediaUpdaters"


                    +


                    ";Media Updaters:HIDEGROUP "
                    +
                    "MediaSections,"
                    +
                    "MediaImporter,"
                    +
                    "MediaAnalyzer,"
                    +
                    "MovieTitleLocator,"
                    +
                    "AutoSorter,"
                    +
                    "OrganizingStructures,"
                    +
                    "Diagnostics,"
                    +
                    "LibraryCleanup,"
                    +
                    "UserInterface,"
                    +
                    "RSSimporter"







                    #endregion Media Updaters



                    +











                    #region Media Organizers

                    ";Media Organizers:"
                    +
                    "SHOWGROUP "
                    +
                    "AutoSorter"

                    +

                    ";Media Organizers:"
                    +
                    "HIDEGROUP "
                    +
                    "MediaSections,"
                    +
                    "MediaImporter,"
                    +
                    "MediaAnalyzer,"
                    +
                    "MovieTitleLocator,"
                    +
                    "RSSimporter,"
                    +
                    "MediaUpdaters,"
                    +
                    "FilmUpdater,"
                    +
                    "TvUpdater,"
                    +
                    "MusicUpdater,"
                    +
                    "VideoIdentifier,"
                    +
                    "FilmDetails,"
                    +
                    "FilmCovers,"
                    +
                    "AlbumCovers,"
                    +
                    "Diagnostics,"
                    +
                    "LibraryCleanup,"
                    +
                    "UserInterface"



                    #endregion Media Organizers





                    +




                    #region Advanced

                    ";Advanced:"
                    +
                    "SHOWGROUP "
                    +
                    "AutoSorter,"
                    +
                    "Diagnostics,"
                    +
                    "LibraryCleanup,"
                    +
                    "UserInterface"


                    +


                    ";Advanced:HIDEGROUP "
                    +
                    "MediaSections,"
                    +
                    "MediaImporter,"
                    +
                    "MediaAnalyzer,"
                    +
                    "MovieTitleLocator,"
                    +
                    "RSSimporter,"
                    +
                    "MediaUpdaters,"
                    +
                    "FilmUpdater,"
                    +
                    "TvUpdater,"
                    +
                    "MusicUpdater,"
                    +
                    "VideoIdentifier,"
                    +
                    "FilmDetails,"
                    +
                    "FilmCovers,"
                    +
                    "AlbumCovers";



                    #endregion Advanced








                prop.GroupName = "misc";
                prop.DefaultValue = "Simple";
                return true;


            }
            return false;



        }











    }


}
