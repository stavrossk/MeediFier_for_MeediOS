using System;
using System.IO;
using System.Net;
using System.Windows.Forms;
using MediaFairy.ImportingEngine;
using MeediOS;


namespace MediaFairy
{


    class GeneralContentDownloader
    {


        internal static bool DownloadAnything(IMLItem item, string downloadFolder)
        {

            WebClient client = new WebClient();


            foreach (string onlineContentField in Settings.OnlineContentFields)
            {

                string content = Helpers.GetTagValueFromItem(item,onlineContentField);
                string[] contentFilenameTemp = content.Split('/');
                string contentFilename = contentFilenameTemp[(contentFilenameTemp.Length - 1)];
                string downloadPath = downloadFolder + contentFilename;


                #region try to download

                try
                {

                    MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress, "MediaFairy is downloading the "
                        + onlineContentField + " " + contentFilename + "...");

                    client.DownloadFile(content, downloadPath);

                    item.Tags[onlineContentField] = downloadPath;
                    item.SaveTags();

                    return true;

                }
                catch (Exception e)
                {

                    #region Retry

                    Debugger.LogMessageToFile("Error downloading online content from web address: " + content + "  to local file " + downloadPath + ". The error was: " + e.Message);
                    MainImportingEngine.SpecialStatus = "An error occurred while downloading " + content + ". Retrying...";
                    Helpers.UpdateProgress(MainImportingEngine.GeneralStatus, MainImportingEngine.SpecialStatus, item);

                    try
                    {
                        MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress, "MediaFairy is downloading " + contentFilename + " from " + content + "...");
                        client.DownloadFile(content, downloadPath);
                        item.Tags[onlineContentField] = downloadPath;
                        item.SaveTags();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Debugger.LogMessageToFile("Error downloading online content from web address: " + content + "  to local file " + downloadPath + ". The error was: " + ex.Message);
                        StatusForm.statusForm.TrayIcon.ShowBalloonTip(5000, "A file was not downloaded", "MediaFairy was not able to download the online file " + contentFilename + " because a netwrok error occure during the process. If this problem persists, please check your internet connectivity and the online source's availability. ", ToolTipIcon.Warning);
                        MainImportingEngine.SpecialStatus = "An error occurred while downloading " + contentFilename;
                        Helpers.UpdateProgress(MainImportingEngine.GeneralStatus, MainImportingEngine.SpecialStatus, item);
                        return false;
                    }

                    #endregion

                }
                #endregion


            }

            return true;
    
        
        }
    
    
    }




}
