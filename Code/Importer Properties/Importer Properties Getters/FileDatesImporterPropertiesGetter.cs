using CustomProperties.PropertyData;
using MeediOS.Cache;

namespace MediaFairy.Code.Importer_Properties
{
    class FileDatesImporterPropertiesGetter
    {
        internal static bool GetFileDatesProperties
            (ImporterProperties importerProperties,
             int index, OptionSettings prop, ref int counter)
        {



            if (index == counter++)
            {
                //set the internal name
                prop.Name = "ImportDateCreatedProp";
                prop.GroupName = "AdditionalFileTags";
                //set name shown to user
                prop.Caption = "Import file Creation date?";
                prop.Caption = TranslationProvider.Translate(prop.Caption, importerProperties);
                prop.GroupCaption = "                                 Additional Tags to Import";
                //set the tool tip
                prop.HelpText = "";
                prop.DefaultValue = Settings.ImportDateCreated;
                prop.DataType = "bool";
                return true;
            }

            if (index == counter++)
            {
                //set the internal name
                prop.Name = "ImportDateModifiedProp";
                prop.GroupName = "AdditionalFileTags";
                //set name shown to user
                prop.Caption = "Import file Modification date?";
                prop.Caption = TranslationProvider.Translate(prop.Caption, importerProperties);
                //Prop.GroupCaption = "                                 Additional Tags to Import";
                //set the tool tip
                prop.HelpText = "";
                prop.DefaultValue = Settings.ImportDateModified;
                prop.DataType = "bool";
                return true;
            }

            if (index == counter++)
            {
                //set the internal name
                prop.Name = "ImportDateImportedProp";
                prop.GroupName = "AdditionalFileTags";
                //set name shown to user
                prop.Caption = "Import Addition to library date?";
                prop.Caption = TranslationProvider.Translate(prop.Caption, importerProperties);
                //Prop.GroupCaption = "                                 Additional Tags to Import";
                //set the tool tip
                prop.HelpText = "";
                prop.DefaultValue = Settings.ImportDateImported;
                prop.DataType = "bool";
                return true;
            }

            return false;
        }
    }
}
