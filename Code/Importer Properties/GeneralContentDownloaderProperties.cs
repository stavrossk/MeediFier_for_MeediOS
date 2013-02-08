using System;
using CustomProperties.PropertyData;
using MeediOS.Cache;





namespace MediaFairy.Code.Importer_Properties
{



    class GeneralContentDownloaderProperties
    {
        internal static bool GetGeneralContentDownloaderProperties
            (ImporterProperties importerProperties,
             int index, OptionSettings prop, ref int counter)
        {




            if (index == counter++)
            {
                prop.GroupName = "OnlineMediaDownloader";
                //set the internal name
                prop.Name = "DownloadOnlineContentProp";
                prop.GroupCaption = "                     Online Media Downloader";
                //set name shown to user
                prop.Caption = "Download online files ";
                prop.Caption = TranslationProvider.Translate(prop.Caption, importerProperties);
                //set the tool tip
                prop.HelpText = "MediaFairy can automatically download any online media file (from the web or ftp address)" +
                                Environment.NewLine +
                                "which is contained in the field you specify in the 'Online content library field' option below,  and save it to the folder you specify." +
                                Environment.NewLine +
                                "Please refer to the plugin's documentation for more information and usage paradigms.";
                prop.DefaultValue = Settings.DownloadOnlineContent;
                prop.DataType = "bool";
                prop.Dependencies = "False:HIDE OnlineContentFieldProp,DownloadFolderProp" +
                                    ";True:SHOW OnlineContentFieldProp,DownloadFolderProp";
                return true;
            }

            if (index == counter++)
            {
                prop.GroupName = "OnlineMediaDownloader";
                //set the internal name
                prop.Name = "OnlineContentFieldProp";
                //set name shown to user
                prop.Caption = "Online media library fields: ";
                prop.Caption = TranslationProvider.Translate(prop.Caption, importerProperties);
                //set the tool tip
                prop.HelpText =
                    " Please specify here the library fields (Tags) in which the online files you desire to store locally are contained." +
                    Environment.NewLine +
                    "MediaFairy will download the online files to which the links contained in these tags are referring,  and replace them with the disk paths of the local files those links were downloaded to." +
                    Environment.NewLine +
                    "Please input those tag names by either clicking the '...' button and typing a tag name *or* by typing the tag names directly in the input box, seperating with a comma (,)." +
                    Environment.NewLine +
                    "MediaFairy will search for those tags in all currently supported media libraries and save the online files they contain in the local locations you specify below. ";
                prop.DefaultValue = Settings.OnlineContentFields;
                prop.DataType = "stringlist";
                return true;
            }


            if (index == counter++)
            {
                prop.GroupName = "OnlineMediaDownloader";
                //set the internal name
                prop.Name = "DownloadFolderProp";
                //set name shown to user
                prop.Caption = "Save online media in: ";
                prop.Caption = TranslationProvider.Translate(prop.Caption, importerProperties);
                //set the tool tip
                prop.HelpText = @"Please specify the folder in which you want downloaded online content to be saved. ";
                prop.DefaultValue = Settings.DownloadFolder;
                prop.DataType = "folder";
                return true;
            }


            return false;
        }
    }




}
