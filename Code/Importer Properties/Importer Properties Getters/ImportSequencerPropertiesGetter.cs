using CustomProperties.PropertyData;
using MeediOS.Cache;

namespace MediaFairy.Code.Importer_Properties
{


    class ImportSequencerPropertiesGetter
    {





        internal static bool GetImportSequencerProperties
            (ImporterProperties importerProperties,
            int index, OptionSettings prop, ref int counter)
        {





            if (index == counter++)
            {
                prop.GroupName = "ImportSequencer";
                //set the internal name
                prop.Name = "ImportSequencerProp";
                //set name shown to user
                prop.GroupCaption = "                         Import Sequencer";
                prop.Caption = "Enable Import Sequencer";
                prop.Caption = TranslationProvider.Translate(prop.Caption, importerProperties);
                //set the tool tip
                prop.HelpText = "";
                prop.DefaultValue = MediaFairy.Settings.ImportSequencer;
                prop.DataType = "bool";
                prop.Dependencies = "True:SHOW ImportIDsProp" +
                                    ";False:HIDE ImportIDsProp";
                return true;
            }

            if (index == counter++)
            {
                prop.GroupName = "ImportSequencer";
                //set the internal name
                prop.Name = "ImportIDsProp";
                //set name shown to user
                prop.Caption = "Import IDs to run: ";
                prop.Caption = TranslationProvider.Translate(prop.Caption, importerProperties);
                //set the tool tip
                prop.HelpText = "";
                prop.DefaultValue = MediaFairy.Settings.ImportIDs;
                prop.DataType = "stringlist";
                return true;
            }
            return false;
        }






    }




}
