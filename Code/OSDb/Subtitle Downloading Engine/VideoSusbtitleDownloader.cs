using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using ICSharpCode.SharpZipLib.Zip;
using MeediOS;
using OSDBnet;


namespace MeediFier.OSDb
{


    class VideoSusbtitleDownloader
    {





        internal static bool GetSubtitleForVideo(string videoHash, string imdbid,
            string language, string logintoken, IMLItem item,
            string parentPath, string videoFilename, bool isMovie)
        {


            #region  vars
            WebClient webClient = new WebClient();
            webClient.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
           
            string firstsub = string.Empty;
            
            FastZip fz = new FastZip();
            #endregion


            if (string.IsNullOrEmpty(parentPath))
                return false;

            if (!parentPath.EndsWith(@"\"))
                parentPath += @"\";

            string zipfilePath = parentPath + videoHash + ".zip";


            try
            {


                firstsub = VideoSubtitleDownloaderHelpers.SearchForSubtitleByVideoHashParent(videoHash, language);

                if (String.IsNullOrEmpty(firstsub))
                {
                    Debugger.LogMessageToFile("The fetching of the online address of the first subtitle by using the video hash failed. Trying to search for subtitle by IMDbID...");
                    firstsub = String.Empty;
                }

                Debugger.LogMessageToFile("The first subtitle's online address is: " + firstsub);

                if (!VideoSubtitleDownloaderHelpers.SearchForSubtitleByIMDbIdParent(imdbid, language,
                    logintoken, item, isMovie, ref firstsub))
                {
                    Debugger.LogMessageToFile("The fetching of the online address of the first subtitle by using the video IMDbID failed. Subtitle downloading cannot continue.");
                    return false;
                }

                if (!VideoSubtitleDownloaderHelpers.PerformSubtitleDownload(item, zipfilePath, webClient, firstsub))
                    return false;


            }
            catch (Exception error)
            {
                MessageBox.Show(@"An error occured while trying to download
                the subtitle on online address: " + firstsub + @" to local location: "
                + zipfilePath + @". The error was: " + error);

                Debugger.LogMessageToFile("An error occured in subtitles downloading function: "
                    + error + ".  The intented subtitle zip location was: " + zipfilePath + 
                    "  and oline subtitle location was: " + firstsub);

                return false;
            
            }


            if (!VideoSubtitleDownloaderHelpers.ValidateDownloadedDataAndRetry(language, item, 
                firstsub, webClient, zipfilePath)) 
                return false;


            VideoSubtitleDownloaderHelpers.ExtractAndRenameSubtitle(language, item, parentPath,
                videoFilename, zipfilePath, fz);


            return true;


        }




        internal static bool DownloadSubtitleForVideoUsingOsdbNet(string mediaSectionFriendlyName, IMLItem item,
            string location, string videoHash, string imdbid, ConnectionResult connectionResult, bool useSameFolder,
            string subtitlesFolder, bool isMovie, bool fileServerIsOnline)
        {

            MessageBox.Show("Entered DownloadSubtitleForVideoUsingOsdbNet method.");
            Debugger.LogMessageToFile("Entered DownloadSubtitleForVideoUsingOsdbNet method.");

            if (!Settings.EnableSubtitleDownloader)
                return false;

            if (Settings.WantFileserverDiagnostics && !fileServerIsOnline)
            {
                //MessageBox.Show("Your file server is offline. Cannot proceed to search for and download a video subtitle.");

                Helpers.UpdateProgress("Updating " + mediaSectionFriendlyName + " Section.",
                    "Your file server is offline. Cannot proceed to search for and download a subtitle for the video: " + item.Name, item);
                Thread.Sleep(1500);

                Debugger.LogMessageToFile("Your file server is offline. Cannot proceed to search for and download a video subtitle.");
                
                return false;
            }

            if (!connectionResult.OSDbIsOnline)
                return false;

            #region Check if the HasSubtitle tag is populated.
            string hasSubtitle = Helpers.GetTagValueFromItem(item, "HasSubtitle");

            if (!String.IsNullOrEmpty(hasSubtitle))
                return true;
            #endregion

            if (Settings.SubDloadLimitExceeded)
                return false;

            MessageBox.Show("Method DownloadSubtitleForVideoUsingOsdbNet passed initial checks. Proceeding to search for and download subtitle...");
            Debugger.LogMessageToFile("Method DownloadSubtitleForVideoUsingOsdbNet passed initial checks. Proceeding to search for and download subtitle...");


            try
            {

                var osdbClient = Osdb.Login("en", Settings.OsdbUserAgent);

                //TODO: Check if video IsMovie and if not change the general progress text to "Series", "Documentary" or other.
                Helpers.UpdateProgress("Updating " + mediaSectionFriendlyName + " Section.", "Searching for subtitles for video: " + item.Name, item);
                var subtitlesResults = osdbClient.SearchSubtitlesFromFile(Settings.PrimarySubtitleLanguage, location);

                if (!CheckIfSubtitlesWereFound(item, subtitlesResults))
                    return false;

                foreach (OSDBnet.Subtitle subtitlesResult in subtitlesResults)
                {
                    //MessageBox.Show("Subtitle language id: " + subtitlesResult.LanguageId);

                    // ReSharper disable InvertIf
                    if (subtitlesResult.LanguageId == Settings.PrimarySubtitleLanguage)
                    // ReSharper restore InvertIf
                    {
                        OSDBnet.Subtitle subtitle = subtitlesResult;
                        Helpers.UpdateProgress("Updating " + mediaSectionFriendlyName + " Section.", "Downloading subtitle for video: " + item.Name, item);

                        //TODO: Download subtitle to the video file's path.
                        osdbClient.DownloadSubtitleToPath(@"M:\", subtitle);
                        return true;
                    }
                }

                return false;
            }
            catch (Exception exception)
            {
                MessageBox.Show("An error occured in the DownloadSubtitleForVideoUsingOsdbNet method. The error was: " + exception);
                Helpers.UpdateProgress("Updating " + mediaSectionFriendlyName + " Section.", "An error occured while trying to search or download a subtitle. Plase see Debug.log for deatails.", item);

                Debugger.LogMessageToFile("An error occured in the DownloadSubtitleForVideoUsingOsdbNet method. The error was: " + exception);

                StatusForm.statusForm.TrayIcon.ShowBalloonTip(3000, "An error occured trying to search for or download a subtitle.",
                    "An error occured while MeediFier was trying to search for or download a video subtitle. Please see Debug.log for details on this error.", ToolTipIcon.Warning);
                Thread.Sleep(2000);
                
                return false;
            }

        }



        
   
        private static bool CheckIfSubtitlesWereFound(IMLItem item, IList<OSDBnet.Subtitle> subtitlesResults)
        {
            if (subtitlesResults.Count <= 0)
            {
                MessageBox.Show("No subtitles found in any language for the video: " + item.Name);
                Debugger.LogMessageToFile("No subtitles found in any language for the video: " + item.Name);
                return false;
            }
            return true;
        }


        internal static bool DownloadSubtitleForVideoParent(string videoHash, string imdbid, 
            IMLItem item, string location , ConnectionResult connectionresult, 
            bool useSameFolder, string subtitlesFolder, bool isMovie)
        {

            MessageBox.Show("Entered DownloadSubtitleForVideoParent method.");

            if (!Settings.EnableSubtitleDownloader)
                return false;

            if (!connectionresult.OSDbIsOnline)
                return false;

            string hasSubtitle = Helpers.GetTagValueFromItem(item, "HasSubtitle");

            if (!String.IsNullOrEmpty(hasSubtitle))
                return true;

            if (Settings.SubDloadLimitExceeded)
                return false;

            MessageBox.Show("Method DownloadSubtitleForVideoParent passed initial checks. Proceeding to download subtitle...");

            string subfilePathSrt;
            string subfilePathSub;
            string parentPath;


            //try
            //{

                //var searchParamsArray = ConstructSubtitleSearchParameters(videoHash, imdbid);

                //XmlRpcStruct searchSubtitlesStruct = OSoperations.Proxy.SearchSubtitles
                //    (connectionresult.OsDbLoginResult.token, searchParamsArray);

                //string statusValue = Convert.ToString(searchSubtitlesStruct["status"]);
                //MessageBox.Show("Status: " + statusValue );

                //string dataValue = Convert.ToString(searchSubtitlesStruct["data"]);
                //MessageBox.Show("Data: " + dataValue);

                //object dataObject = searchSubtitlesStruct["data"];
                //object[] dataArray = dataObject as object[];
                //object subtitleInfoObject = dataArray[0];
                //object[] subtitleInfoArray = subtitleInfoObject as object[];
                //string matchedBy = Convert.ToString(subtitleInfoArray[0]);
                //MessageBox.Show("MatchedBy: " + matchedBy);


                //#region Scan entries to find data token.
                //foreach (DictionaryEntry rootDictionaryEntry in searchSubtitlesStruct)
                //{
                //    string rootDictionaryEntryKey = Convert.ToString(rootDictionaryEntry.Key);
                //    MessageBox.Show("Root Dictionary Entry Key: " + rootDictionaryEntryKey);

                //    if (rootDictionaryEntryKey == "data")
                //    {
                //        MessageBox.Show("Data obhect found!");
                //        Debugger.LogMessageToFile("Data object found!");
                //    }
                //    else
                //    {
                //        //Data object not found, continue to next Root Dictionary Entry.
                //        continue;
                //    }

                //    object subtitlesArrayObject = rootDictionaryEntry.Value;
                //    var subtitlesArrayStruct = (XmlRpcStruct[])subtitlesArrayObject; //unboxing

                //    foreach (DictionaryEntry subtitleDictionaryEntry in subtitlesArrayStruct[0])
                //    {
                //        string subtitleDictionaryEntryKey = Convert.ToString(subtitleDictionaryEntry.Key);
                //        MessageBox.Show("Subtitle Dictionary Entry Key: " + subtitleDictionaryEntryKey);
                //
                //        if (subtitleDictionaryEntryKey == "0")
                //        {
                //            MessageBox.Show("First subtitle obhect found!");
                //            Debugger.LogMessageToFile("First subtitle object found!");
                //        }
                //        else
                //        {
                //            //First subtitle object not found, continue to next Subtitle Dictionary Entry.
                //            continue;
                //        }

                //    }

                    

                //}
                //#endregion

            //}
            //catch (Exception exception)
            //{
            //    MessageBox.Show("The error was: " + exception);
            //}



            var videoFilename = VideoSubtitleDownloaderHelpers.ConstructSubtitlePath(location, useSameFolder,
                subtitlesFolder, out subfilePathSrt, out subfilePathSub, out parentPath);


            if (!VideoSubtitleDownloaderHelpers.CheckForExistingSubtitleSetHasSubtitleFlag
                (videoHash, item, parentPath, subfilePathSrt, subfilePathSub))
                return false;


            var subsDownloadResult = VideoSubtitleDownloaderHelpers.DownloadSubtitleForPrimaryOrSecondaryLanguage
                (videoHash, imdbid, item, connectionresult, isMovie, videoFilename, parentPath);


            VideoSubtitleDownloaderHelpers.DeleteSubtitleZipFile(videoHash, parentPath);



            return subsDownloadResult;


        }

        private static SearchParams[] ConstructSubtitleSearchParameters(string videoHash, string imdbid)
        {
            #region Construct subtitles search parameters.

            var searchParamsArray = new SearchParams[1];
            searchParamsArray[0].moviehash = videoHash;
            searchParamsArray[0].sublanguageid = Settings.PrimarySubtitleLanguage;

            string imdbIdTrimmed = imdbid.Substring(2);
            searchParamsArray[0].imdbid = imdbIdTrimmed;

            #endregion

            return searchParamsArray;
        }
    }



}
