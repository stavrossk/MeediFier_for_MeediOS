using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CustomProperties.PropertyData;
using MeediOS.Cache;

namespace MediaFairy.Code.Importer_Properties
{
    class FilmBackdropsLocationPropertiesGetter
    {
        internal static bool GetFilmBackdropsLocationProperties
            (ImporterProperties importerProperties, int index, OptionSettings prop,
             string[] twochoices, ref int counter)
        {




            if (index == counter++)
            {
                //set the internal name
                prop.Name = "WhereToSaveBackdropProp";
                //set name shown to user
                prop.Caption = "Where to store fanart?";

                prop.Caption
                    = TranslationProvider.Translate
                        (prop.Caption, importerProperties);


                //set the tool tip
                prop.GroupName = "FilmBackdrops";
                prop.GroupCaption = "                     Film fanart Settings";


                prop.HelpText = "Do you want the backdrop/fanart image " +
                                "for each movie to be saved in the movie's folder" +
                                " or to a specific folder of your choice?";

                prop.DefaultValue = Importer.WhereToSaveBackdrop;
                Prop.DataType = "string";
                twochoices[0] = "in the movie's folder";
                twochoices[1] = "in the folder i specify below";
                Prop.Choices = twochoices;
                Prop.CanTypeChoices = false;

                Prop.Dependencies = "in the movie's folder:HIDE BackdropFolderProp" +
                                    ";in the folder i specify below:SHOW BackdropFolderProp";
                return true;
            }

            if (Index == counter++)
            {
                //set the internal name
                Prop.Name = "BackdropFolderProp";
                //set name shown to user
                Prop.Caption = "Fanart folder: ";
                Prop.Caption = TranslationProvider.Translate(Prop.Caption, importerProperties);
                Prop.GroupName = "FilmBackdrops";
                //set the tool tip
                Prop.HelpText =
                    @"Please specify the folder in which you want downloaded Fanart to be saved. (Only applicabe if option above is *not* set to 'Movie's folder') ";
                Prop.DefaultValue = Importer.BackdropFolder;
                Prop.DataType = "folder";
                Prop.CanTypeChoices = false;
                return true;
            }

            if (Index == counter++)
            {
                //set the internal name
                Prop.Name = "BackdropNamingChoice";
                //set name shown to user
                Prop.Caption = "Fanart naming convention: (tag mask) ";
                Prop.Caption = TranslationProvider.Translate(Prop.Caption, importerProperties);
                Prop.GroupName = "FilmBackdrops";
                //set the tool tip
                Prop.HelpText = "Please enter here your desired fanart naming convention in a form of a tag mask." +
                                Environment.NewLine +
                                "For example, the default tag mask '<IMDbID>_backdrop' will store the backdrop image as the varialbe film's IMDb number followed by the '_backdrop' suffix ." +
                                Environment.NewLine +
                                "Please make sure that any text strings contained in '<>' refer to the names of existing tags of your Movies media section.";
                Prop.DefaultValue = Importer.BackdropNamingMethod;
                Prop.DataType = "string";
                Prop.CanTypeChoices = true;
                return true;
            }

            if (index == counter++)
            {
                //set the internal name
                prop.Name = "OverwriteBackdropChoice";
                //set name shown to user
                prop.Caption = "Overwrite existing fanart? ";

                prop.Caption = TranslationProvider.Translate(prop.Caption, importerProperties);

                prop.GroupName = "FilmBackdrops";
                //set the tool tip

                prop.HelpText = "If a backdrop already exists" +
                                " in your specified location, " +
                                "should MediaFairy to download" +
                                " a new backdrop and overwrite the original?";


                prop.DefaultValue = Importer.OverwriteExistingBackdrops;
                prop.DataType = "bool";
                return true;
            }


            return false;
        }
    }
}
