using CustomProperties.PropertyData;
using MeediOS.Cache;




namespace MediaFairy.Code.Importer_Properties
{




    class LibrarySynchronizersPropertiesGetter
    {





        internal static bool GetLibrarySynchronizersProperties
            (ImporterProperties importerProperties,
            int index, OptionSettings prop, ref int counter)
        {



            if (index == counter++)
            {
                prop.GroupName = "LibraryBackup";
                //set the internal name
                prop.Name = "LibraryBackupProp";
                //set name shown to user
                prop.GroupCaption = "                             Library Backup & Restore";
                prop.Caption = "Enable Library Backup and Restore";
                prop.Caption = TranslationProvider.Translate(prop.Caption, importerProperties);
                //set the tool tip
                prop.HelpText = "";
                prop.DefaultValue = false;
                prop.DataType = "bool";
                prop.Dependencies =
                    "True:SHOW BackupMoviesSectionProp,BackupTvSeriesSectionsProp,BackupMusicSectionProp,RestoreSectionsProp,XmlBackupDirectoryProp" +
                    ";False:HIDE BackupMoviesSectionProp,BackupTvSeriesSectionsProp,BackupMusicSectionProp,RestoreSectionsProp,XmlBackupDirectoryProp";
                return true;
            }

            if (index == counter++)
            {
                prop.GroupName = "LibraryBackup";
                //set the internal name
                prop.Name = "BackupMoviesSectionProp";
                //set name shown to user
                prop.Caption = "Backup Movies Section";
                prop.Caption = TranslationProvider.Translate(prop.Caption, importerProperties);
                //set the tool tip
                prop.HelpText = "";
                prop.DefaultValue = MediaFairy.Settings.EnableXmlBackupFilms;
                prop.DataType = "bool";
                return true;
            }
            if (index == counter++)
            {
                prop.GroupName = "LibraryBackup";
                //set the internal name
                prop.Name = "BackupTvSeriesSectionsProp";
                //set name shown to user
                prop.Caption = "Backup TV Series Sections";
                prop.Caption = TranslationProvider.Translate(prop.Caption, importerProperties);
                //set the tool tip
                prop.HelpText = "";
                prop.DefaultValue = MediaFairy.Settings.EnableXmlBackupTvSeries;
                prop.DataType = "bool";
                return true;
            }
            if (index == counter++)
            {
                prop.GroupName = "LibraryBackup";
                //set the internal name
                prop.Name = "BackupMusicSectionProp";
                //set name shown to user
                prop.Caption = "Backup Music Section";
                prop.Caption = TranslationProvider.Translate(prop.Caption, importerProperties);
                //set the tool tip
                prop.HelpText = "";
                prop.DefaultValue = MediaFairy.Settings.EnableXmlBackupMusic;
                prop.DataType = "bool";
                return true;
            }
            if (index == counter++)
            {
                prop.GroupName = "LibraryBackup";
                //set the internal name
                prop.Name = "RestoreSectionsProp";
                //set name shown to user
                prop.Caption = "Restore Sections from backup";
                prop.Caption = TranslationProvider.Translate(prop.Caption, importerProperties);
                //set the tool tip
                prop.HelpText = "";
                prop.DefaultValue = MediaFairy.Settings.EnableXmlRestore;
                prop.DataType = "bool";
                return true;
            }
            if (index == counter++)
            {
                prop.GroupName = "LibraryBackup";
                //set the internal name
                prop.Name = "XmlBackupDirectoryProp";
                //set name shown to user
                prop.Caption = "XML backups directory";
                prop.Caption = TranslationProvider.Translate(prop.Caption, importerProperties);
                //set the tool tip
                prop.HelpText = "";
                prop.DefaultValue = MediaFairy.Settings.XmlBackupsDirectory;
                prop.DataType = "folder";
                return true;
            }

            return false;
        }
    }
}
