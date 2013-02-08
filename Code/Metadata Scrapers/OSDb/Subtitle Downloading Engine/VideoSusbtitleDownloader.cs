using System;
using System.Net;
using System.Windows.Forms;
using ICSharpCode.SharpZipLib.Zip;
using MeediOS;




namespace MediaFairy.OSDb
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


                if (!VideoSubtitleDownloaderHelpers.SearchForSubtitleByIMDbIdParent(imdbid, language,
                    logintoken, item, isMovie, ref firstsub))
                    return false;

                if (!VideoSubtitleDownloaderHelpers.PerformSubtitleDownload(item, zipfilePath, webClient, firstsub))
                    return false;


            }
            catch (Exception e)
            {
                MessageBox.Show(@"An error occured while trying to download
                the subtitle on online address: " + firstsub + @" to local location: "
                + zipfilePath + @". The error was: " + e);

                Debugger.LogMessageToFile("Error occured in subtitles downloading function: "
                    + e + ".  The intented subtitle zip location was: " + zipfilePath + 
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




        internal static bool DownloadSubtitleForVideoParent(string videoHash, string imdbid, 
            IMLItem item, string location , ConnectionResult connectionresult, 
            bool useSameFolder, string subtitlesFolder, bool isMovie)
        {


            if (!Settings.EnableSubtitleDownloader)
                return false;

            if (!connectionresult.OSDbIsOnline)
                return false;

            string hasSubtitle = Helpers.GetTagValueFromItem(item, "HasSubtitle");

            if (!String.IsNullOrEmpty(hasSubtitle))
                return true;

            //if (String.IsNullOrEmpty(imdbid))
            //    return false;


            if (Settings.SubDloadLimitExceeded)
                return false;


            string subfilePathSrt;
            string subfilePathSub;
            string parentPath;

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
    
    
    }



}
