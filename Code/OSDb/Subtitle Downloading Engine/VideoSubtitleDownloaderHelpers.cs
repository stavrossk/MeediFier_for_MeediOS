using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using CookComputing.XmlRpc;
using ICSharpCode.SharpZipLib.Zip;
using MeediFier;
using MeediFier.ImportingEngine;
using MeediOS;



namespace MeediFier.OSDb
{

    class VideoSubtitleDownloaderHelpers
    {




        internal static void DeleteSubtitleZipFile(string itemName, string parentPath)
        {
            string subtitleZip = parentPath + "\\" + itemName + ".zip";

            try
            {
                File.Delete(subtitleZip);
            }
            catch (Exception e)
            {
                Debugger.LogMessageToFile(e.ToString());
            }

        }


        internal static string ConstructSubtitlePathForOsdbNet(string location,
            out string parentPath, out string subfileSub)
        {

            var videoLocationFileInfo =
                new FileInfo(location);

            //string videoFilenameWithExtension = videoLocationFileInfo.Name;

            parentPath = videoLocationFileInfo.DirectoryName;

            if (Settings.UseSameSubtitlesFolder)
            {
                if (!String.IsNullOrEmpty(Settings.SubtitlesFolder))
                    parentPath = Settings.SubtitlesFolder;
            }

            string subFilenameWithoutExtension = Path.GetFileNameWithoutExtension(location);
            //Old method:
            //string subFilenameWithoutExtension = 
            //    videoFilenameWithExtension.Remove(videoFilename.Length - 3);


            string subfileSrt = subFilenameWithoutExtension + ".srt";
            subfileSub = subFilenameWithoutExtension + ".sub";


            return subfileSrt;
        }





        internal static string ConstructSubtitlePath(string location, bool useSameFolder, string subtitlesFolder,
                                                    out string subfilePathSrt, out string subfilePathSub, out string parentPath)
        {

            var videoLocationFileInfo = 
                new FileInfo(location);

            string videoFilenameWithExtension = videoLocationFileInfo.Name;
           
           
            parentPath = videoLocationFileInfo.DirectoryName;

            if (useSameFolder)
            {
                if (!String.IsNullOrEmpty(subtitlesFolder))
                    parentPath = subtitlesFolder;
            }

            //TODO: Get the subtitle's filename without extension, regardless the length of the video file's extension.
            string subFilenameWithoutExtension = Path.GetFileNameWithoutExtension(location);
            //string subFilenameWithoutExtension = 
            //    videoFilenameWithExtension.Remove(videoFilename.Length - 3);
            
            //string subfileSrt = subFilenameWithoutExtension + "srt";
            string subfileSrt = subFilenameWithoutExtension + ".srt";
            //string subfileSub = subFilenameWithoutExtension + "sub";
            string subfileSub = subFilenameWithoutExtension + ".sub";
            
            subfilePathSrt = parentPath + "\\" + subfileSrt;
            subfilePathSub = parentPath + "\\" + subfileSub;


            return videoFilenameWithExtension;
        }


        internal static bool CheckForExistingSubtitleSetHasSubtitleFlag
            (string itemName, IMLItem item, string parentPath,
             string subfilePathSrt, string subfilePathSub)
        {


            MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress,
                                                      "Determining if subtitle exists for " + item.Name);


            if (File.Exists(subfilePathSrt) || File.Exists(subfilePathSub))
            {
                item.Tags["HasSubtitle"] = "True";
                item.SaveTags();

                DeleteSubtitleZipFile(itemName, parentPath);

                return false; 
            
            }



            return true;
       
        }

        internal static string RecognizeCorrectSubtitle(string sFile)
        {
            try
            {

                ZipFile zip = 
                    new ZipFile(File.OpenRead(sFile));

                string subFilename = String.Empty;

                foreach (ZipEntry entry in zip)
                {

                    if (!entry.IsFile) 
                        continue;

                    FileInfo fi = new FileInfo(entry.Name);
                    
                    string ext = fi.Extension;
                    string name = fi.Name;


                    if (name.Contains("cd") || name.Contains("CD"))
                    {
                        if (name.Contains("1"))
                        {
                            if (ext == ".srt" || ext == ".sub")
                                subFilename = entry.Name;
                        }



                    }
                    else if (ext == ".srt" || ext == ".sub")
                        subFilename = entry.Name;
                }

                return subFilename;

            }
            catch (Exception)
            {

                return String.Empty;
            }


        }

        internal static bool DownloadSubtitleForPrimaryOrSecondaryLanguage(string moviehash, string imdbid, IMLItem item,
                                                                          ConnectionResult connectionresult, bool isMovie,
                                                                          string videoFilename, string parentPath)
        {


            MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress,
                                                      "Searching subtitles for " + item.Name + "...");


            bool subsDownloadResult = VideoSusbtitleDownloader.GetSubtitleForVideo
                (moviehash, imdbid, Settings.PrimarySubtitleLanguage,
                 connectionresult.OsDbLoginResult.token, item, 
                 parentPath, videoFilename, isMovie);


            if (!subsDownloadResult)
                subsDownloadResult = VideoSusbtitleDownloader.GetSubtitleForVideo
                    (moviehash, imdbid, Settings.SecondaryLanguage,
                     connectionresult.OsDbLoginResult.token, item,
                     parentPath, videoFilename, isMovie);


            return subsDownloadResult;
        
        
        }

        internal static bool MakeOsDbRequest(string useragent, string searchstring,
                                            out string subtitleData, out WebResponse webResponse)
        {

            HttpWebRequest request = 
                (HttpWebRequest) WebRequest.Create(searchstring);
            
            request.UserAgent = useragent;
            request.Timeout = 10000;

            try
            {
                webResponse = request.GetResponse();
            }
            catch (Exception)
            {

                Helpers.UpdateProgress("", "OSdb did not respond. Retrying...", null);
                Thread.Sleep(2000);

                try
                {
                    webResponse = request.GetResponse();
                }
                catch (Exception)
                {
                    Helpers.UpdateProgress("", "OSdb did not respond. Retrying...", null);
                    Thread.Sleep(2000);

                    try
                    {
                        webResponse = request.GetResponse();
                    }
                    catch (Exception)
                    {

                        Helpers.UpdateProgress("", "Unable to communicate with OSdb.", null);
                        Thread.Sleep(700);
                        {
                            subtitleData = String.Empty;
                            webResponse = null;
                            return true;
                        }

                    }

                }

            }


            subtitleData = null;
            return false;
        
        }

        internal static bool LoadOSDbResponseXmlDocument(XmlDocument xDoc,
                                                        WebResponse webResponse, out string subtitleData)
        {

            Stream stream = webResponse.GetResponseStream();

            try
            {
                if (stream != null)
                    xDoc.Load(stream);
            }
            catch (Exception)
            {
                Helpers.UpdateProgress("", "Unable to download subtitle. " +
                                           "The response from OSdb was invalid.", null);

                Thread.Sleep(2000);

                {
                    subtitleData = String.Empty;
                    return true;
                }
            }


            subtitleData = String.Empty;
            return false;
        
        }

        internal static bool MakeOSDbRequestGetResponse(string searchstring, XmlDocument xDoc, string useragent,
                                                       out string subtitleData)
        {
            WebResponse webResponse;

            string subtitleDataA;

            if (MakeOsDbRequest(useragent, searchstring,
                                                          out subtitleDataA, out webResponse))
            {
                subtitleData = subtitleDataA;
                return true;
            }


            string subtitleDataB;

            if (LoadOSDbResponseXmlDocument(xDoc, webResponse, out subtitleDataB))
            {
                subtitleData = subtitleDataB;
                return true;
            }

            subtitleData = null;
            return false;


        }

        internal static string SearchForSubtitleByImdbID(string imdbid, 
                                                         string language, string logintoken, IMLItem item)
        {

            #region vars

            SearchParams[] paramS = new SearchParams[1];
            paramS[0].sublanguageid = language;
            paramS[0].imdbid = imdbid;
            string[] downloadlink = new string[25];

            #endregion

            XmlRpcStruct moviesubs =
                OSoperations.Proxy.SearchSubtitles(logintoken, paramS);


            foreach (DictionaryEntry d in moviesubs)
            {
                OSoperations.token = Convert.ToString(d.Key);

                if (OSoperations.token != "data") 
                    continue;
                
                object obj = d.Value;

                Type type = obj.GetType();
                string typename = type.Name;


                if (typename == "Boolean")
                    return null;

                object[] subsarray = (object[])obj;

                downloadlink = new string[subsarray.Length];

                for (int i = 0; i < subsarray.Length; i++)
                {

                    object x = subsarray[i];
                    XmlRpcStruct subtitle = (XmlRpcStruct)x;

                    foreach (DictionaryEntry e in subtitle)
                    {
                        OSoperations.token = Convert.ToString(e.Key);

                        if (OSoperations.token != "ZipDownloadLink")
                            continue;
                        
                        object val = e.Value;
                        string str = (string)val;

                        downloadlink[i] = str;
                    
                    }

                }


            }

            string firstSubtitleFound = downloadlink[0];


            return firstSubtitleFound;

        }

        internal static decimal GetSubtitleRating(int i, XmlNodeList subrating)
        {

            decimal rating;
            
            if (!String.IsNullOrEmpty(subrating[i].InnerText))
            {

                try
                {
                    rating = Convert.ToDecimal(subrating[i].InnerText)/10;
                }
                catch (Exception e)
                {
                    rating = 0;
                }

            }
            else rating = 0;

            Debugger.LogMessageToFile("Subtitle rating: " + rating);
            
            return rating;
        }

        internal static XmlNodeList DefineSubtitleXmlNodeElements(XmlDocument xDoc, out XmlNodeList subrating, out XmlNodeList download)
        {
            XmlNodeList subtitle = xDoc.GetElementsByTagName("subtitle");
            download = xDoc.GetElementsByTagName("download");
            subrating = xDoc.GetElementsByTagName("subrating");

            return subtitle;
        }

        internal static void RenameSubtitleAccordingToVideoFilename
            (IMLItem item, string parentPath, string videoFilename,
             string subtitleFilename, string subtitleExtension)
        {

            //Construct subtitle path
            string subtitleFullPath = parentPath + "\\" + subtitleFilename;

            //Rename subtitle according to video file's name
            string source = subtitleFullPath;
            string destination = parentPath + "\\" + videoFilename + subtitleExtension;


            if (!File.Exists(destination))
            {

                try
                {
                    File.Move(source, destination);
                }
                catch (Exception e)
                {

                    MainImportingEngine.ThisProgress.Progress
                        (MainImportingEngine.CurrentProgress,
                         "Error occured while moving/renaming " +
                         source + ": " + e.Message);

                    Thread.Sleep(2000);

                    Debugger.LogMessageToFile("[Video Subtitle Downloader] An unexpecter error occured" +
                                              " while trying to rename subtitle " + source + ". The error was: " + e);
             
                
                }


            }
            else
            {

                MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress,
                                                          "Subtitle for the item " + item.Name + " already exists.");

            }




        }

        internal static bool PerformSubtitleDownload(IMLItem item, 
                                                    string zipfilePath, WebClient client, string firstsub)
        {

            MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress,
                                                      "Downloading Subtitle for " + item.Name + " in 5 seconds...");

            Thread.Sleep(10000);

            try
            {
                client.DownloadFile(firstsub, zipfilePath);
            }
            catch (Exception)
            {
                try
                {
                    client.DownloadFile(firstsub, zipfilePath);
                }
                catch (Exception e)
                {
                    MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress,
                        "Unable to download a subtitle for this video due to a network issue.");

                    Thread.Sleep(1300);

                    Debugger.LogMessageToFile(
                        "MediaFairy was unable to download a subtitle for this video" +
                        " because an unexpected error occurred in the downloading method. " +
                        "The error was: " + e);

                    return false;
                }
            }
            return true;

        }

        internal static string SearchForSubtitleByIMDbId(string imdbid,
                                                        string language, string logintoken, IMLItem item)
        {

            MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress,
                                                      "Synchronized subtitles not found. Searching for unsynchronized subtitles...");

            imdbid = imdbid.TrimStart('t');
            string firstsub = String.Empty;


            try
            {
                firstsub = SearchForSubtitleByImdbID(imdbid, language, logintoken, item);
            }
            catch (Exception)
            {

                MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress,
                                                          "OSdb did not respond. Retrying...");

                Thread.Sleep(2000);

                try
                {
                    firstsub = SearchForSubtitleByImdbID
                        (imdbid, language, logintoken, item);
                }
                catch (Exception)
                {

                    MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress,
                                                              "Unable to communicate with OSdb.");

                    Thread.Sleep(2000);
                }
            }
            return firstsub;
        }


        internal static string ConstructExtractedSubtitleFilename
            (IMLItem item, string zipfilePath, out string subtitleExtension)
        {

            string subtitleFilename =
                RecognizeCorrectSubtitle(zipfilePath);


            if (String.IsNullOrEmpty(subtitleFilename))
            {

                MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress,
                            "Could not extract subtitle. The archive is corrupt.");

                Debugger.LogMessageToFile("Could not extract subtitle for "
                            + item.Name + ". The archive is corrupt.");
                
                Thread.Sleep(Settings.SleepValue);
                
                subtitleExtension = null;
                
                return subtitleFilename;
            
            }


            subtitleExtension = subtitleFilename.Remove(0, subtitleFilename.Length - 4);


            return subtitleFilename;

        }

        internal static string ConstructSubtitleFilenameAndExtractSubtitle
            (IMLItem item, string parentPath, string videoFilename,
             FastZip fz, string zipfilePath, out string subtitleFilename,
             out string subtitleExtension)
        {

            subtitleFilename = ConstructExtractedSubtitleFilename
                (item, zipfilePath, out subtitleExtension);

            videoFilename = videoFilename.Substring(0, videoFilename.Length - 4);


            fz.ExtractZip(zipfilePath, parentPath, @"(?i)^.*(?:(?:.srt)|(?:.sub))$");


            Settings.zipfilepath = zipfilePath;


            return videoFilename;
        }

        internal static bool ValidateDownloadedDataAndRetry(string language, 
                                                           IMLItem item, string firstsub, WebClient client, string zipfilePath)
        {

            StreamReader sr = File.OpenText(zipfilePath);
            string zipfileLine = sr.ReadLine();

            if (zipfileLine != null && zipfileLine.Contains("DOCTYPE html"))
            {
                MainImportingEngine.ThisProgress.Progress
                    (MainImportingEngine.CurrentProgress,
                     "The subtitle archive was corrupt." +
                     " Retrying in 10 seconds...");

                sr.Close();
                Thread.Sleep(10000);

                File.Delete(zipfilePath);

                client.DownloadFile(firstsub, zipfilePath);


                sr = File.OpenText(zipfilePath);

                zipfileLine = sr.ReadLine();

                if (zipfileLine != null && zipfileLine.Contains("DOCTYPE html"))
                {
                    MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress,
                                                              "Unable to extract subtitle. The downloaded archive was corrupt.");

                    sr.Close();
                    Thread.Sleep(1500);
                    File.Delete(zipfilePath);

                    return false;
                }

                MainImportingEngine.ThisProgress.Progress
                    (MainImportingEngine.CurrentProgress,
                     "Subtitle for " + item.Name + " was downloaded succesfully.");

                Thread.Sleep(1500);

                item.Tags["HasSubtitle"] = "True";
                item.Tags["Subtitle language"] = language;
                item.SaveTags();

                sr.Close();
                sr.Dispose();

            }

            return true;

        }

        internal static bool SearchForSubtitleByIMDbIdParent(string imdbid, string language,
                                                            string logintoken, IMLItem item, bool isMovie, ref string firstsub)
        {

            if (!String.IsNullOrEmpty(firstsub))
                return true;
            
            if (!isMovie)
                return false;


            firstsub = SearchForSubtitleByIMDbId
                (imdbid, language, logintoken, item);

            return !String.IsNullOrEmpty(firstsub.Trim());

        }

        internal static string SearchForSubtitleByVideoHash(string moviehash, string language)
        {

            #region local variables

            const string prefix = "http://www.opensubtitles.org/search/sublanguageid-";
            string searchstring = prefix + language + "/moviehash-" + moviehash + "/simplexml";

            const string useragent = "MeediFier for MeediOS v0.7.9";
            XmlDocument xDoc = new XmlDocument();
            #endregion


            string subtitleData;

            if (MakeOSDbRequestGetResponse(searchstring, xDoc, useragent, out subtitleData))
                return subtitleData;

            XmlNodeList subrating;
            XmlNodeList download;

            var subtitle = DefineSubtitleXmlNodeElements
                (xDoc, out subrating, out download);


            #region decide which subtitle to download

            decimal maxRating = 0;
            int preferred = 0;

            Debugger.LogMessageToFile(Environment.NewLine + "We will now loop through all the found subtitles in order to decide which subtitle to download: " + Environment.NewLine);

            for (int i = 0; i < subtitle.Count; i++)
            {
                string sublink = download[i].InnerText;
                Debugger.LogMessageToFile("Subtitle link: " + sublink);

                decimal rating = GetSubtitleRating(i, subrating);

                if (rating <= maxRating)
                    continue;
                
                preferred = i;
                maxRating = rating;
            
            }
            #endregion

            if (download.Count > 0)
            {
                string downloadlink = download[preferred].InnerText;

                if (downloadlink.StartsWith("/download"))
                    downloadlink = "http://www.opensubtitles.org" + downloadlink;

                //TODO: The returned subtitle zip file from OSDb is just an HTML file. I should fix the subtitle downloader in order to download a valid subtitle zip file.
                Debugger.LogMessageToFile(Environment.NewLine + "The prefered subtitle's download link is: "
                    + downloadlink + Environment.NewLine);

                return downloadlink;

            }

            return null;
        
        }





        internal static void ExtractAndRenameSubtitle(string language, IMLItem item,
            string parentPath, string videoFilename, string zipfilePath, FastZip fz)
        {

            MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress,
                                                      "Subtitle for " + item.Name + " was downloaded succesfully.");

            Thread.Sleep(1500);

            item.Tags["HasSubtitle"] = "True";
            item.Tags["Subtitle language"] = language;
            item.SaveTags();


            string subtitleFilename;
            string subtitleExtension;


            videoFilename = ConstructSubtitleFilenameAndExtractSubtitle
                (item, parentPath, videoFilename, fz, zipfilePath,
                 out subtitleFilename, out subtitleExtension);


            RenameSubtitleAccordingToVideoFilename
                (item, parentPath, videoFilename, subtitleFilename, subtitleExtension);


        }

        internal static string SearchForSubtitleByVideoHashParent(string videoHash, string language)
        {

            string firstsub = String.Empty;

            if (!String.IsNullOrEmpty(videoHash))
            {
                MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress,
                                                          "Searching OSdb for subtitle (by video hash)...");

                Debugger.LogMessageToFile(Environment.NewLine + "Searching OSdb for subtitle (by video hash)..." + Environment.NewLine);


                firstsub = SearchForSubtitleByVideoHash
                    (videoHash, language);

            }
            else
            {

                MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress,
                                                          "Unable to search for video subtitle. Video fingerprint is unknown.");

                Debugger.LogMessageToFile(Environment.NewLine 
                    + "Unable to search for video subtitle by video hash. Video fingerprint is unknown." + Environment.NewLine);

                Thread.Sleep(2000);

            }

            return firstsub;


        }

        internal static bool CheckIfSubtitlesWereFound(IMLItem item, IList<OSDBnet.Subtitle> subtitlesResults)
        {
            if (subtitlesResults.Count <= 0)
            {
                MessageBox.Show("No subtitles found in any language for the video: " + item.Name);
                Debugger.LogMessageToFile("No subtitles found in any language for the video: " + item.Name);
                return false;
            }
            return true;
        }

        internal static SearchParams[] ConstructSubtitleSearchParameters(string videoHash, string imdbid)
        {

            var searchParamsArray = new SearchParams[1];
            searchParamsArray[0].moviehash = videoHash;
            searchParamsArray[0].sublanguageid = Settings.PrimarySubtitleLanguage;

            string imdbIdTrimmed = imdbid.Substring(2);
            searchParamsArray[0].imdbid = imdbIdTrimmed;

            return searchParamsArray;
        }
    }



}
