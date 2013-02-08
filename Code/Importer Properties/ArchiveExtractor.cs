using System;
using CustomProperties.PropertyData;
using MeediOS.Cache;

namespace MediaFairy.ImporterPropertiesGroups
{

    class ArchiveExtractor
    {

        internal static bool ArchiveExtractorPropRerties(ImporterProperties importerProperties, int index, OptionSettings prop,
                                                         ref int counter)
        {


            if (index == counter++)
            {
                prop.GroupName = "ArchiveExtractor";
                //set the internal name
                prop.Name = "EnableArchiveExtractorProp";
                //set name shown to user
                prop.GroupCaption = "                              Archive Extractor";
                prop.Caption = "Auto-extract compressed archives";
                prop.Caption = TranslationProvider.Translate(prop.Caption, importerProperties);
                //set the tool tip
                prop.HelpText =
                    "The built-in media importer has the capability to automatically extract zip and/or rar archives" +
                    Environment.NewLine +
                    "contained in your specified media root directories and import the media files they may contain." +
                    Environment.NewLine +
                    "Multi-part rar archives are also supported.";
                prop.DefaultValue = Settings.EnableArchiveExtractor;
                prop.DataType = "bool";

                prop.Dependencies = "False:HIDE DeleteArchiveAfterExtractionProp,freespaceThresholdProp" +
                                    ";True:SHOW DeleteArchiveAfterExtractionProp,freespaceThresholdProp";
                return true;
            }
            if (index == counter++)
            {
                prop.GroupName = "ArchiveExtractor";
                //set the internal name
                prop.Name = "DeleteArchiveAfterExtractionProp";
                //set name shown to user
                prop.Caption = "Delete archive after extraction ";
                prop.Caption = TranslationProvider.Translate(prop.Caption, importerProperties);
                //set the tool tip
                prop.HelpText = "When an archive extraction process is succesfull, the media importer can delete" +
                                Environment.NewLine +
                                "the original compressed archive in order to free any disk space used by that archive.";
                prop.DefaultValue = Settings.DeleteArchiveAfterExtraction;
                prop.DataType = "bool";
                return true;
            }
            if (index == counter++)
            {
                prop.GroupName = "ArchiveExtractor";
                //set the internal name
                prop.Name = "freespaceThresholdProp";
                //set name shown to user
                prop.Caption = "Maintain at least this amount of disk space: (MB)";
                prop.Caption = TranslationProvider.Translate(prop.Caption, importerProperties);
                //set the tool tip
                prop.HelpText =
                    "Please specify here the amount of free disk space (in Megabytes, MB) you would like the plugin to always maintain." +
                    Environment.NewLine +
                    "When this limit is calculated to be exceeded, the archive extraction process in question will be skipped.";
                prop.DefaultValue = Settings.freespaceThreshold;
                prop.DataType = "int";
                return true;
            }

            return false;
        }

    }

}
