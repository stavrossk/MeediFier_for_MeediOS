using System;
using System.IO;
using System.Windows.Forms;
using MediaInfoLib;
using MeediFier.ImportingEngine;

namespace MeediFier.MediaSnapshotEngine.MediaAnalyzer
{



    class MediaInfoExtractor
    {


        internal static void AnalyzeMediaFileGetMediaInformation(FileSystemInfo file, MediaInfo mediaInfo, out int videoDuration,
                                                                out string videoCodec, out string audioCodec, out int audioDuration,
                                                                out string filesize, out string filecodec)
        {

            OpenMediaFileForProcessing(file, mediaInfo);

            filesize = GetFileSize(mediaInfo);

            GetCodecs(mediaInfo, out videoCodec, out audioCodec, out filecodec);

            GetDurations(mediaInfo, out videoDuration, out audioDuration);

        }


        private static void GetDurations(MediaInfo mediaInfo, out int videoDuration, out int audioDuration)
        {
            videoDuration = GetVideoDuration(mediaInfo);

            audioDuration = GetAudioDuration(mediaInfo);
        }


        private static void GetCodecs(MediaInfo mediaInfo, out string videoCodec, out string audioCodec, out string filecodec)
        {
            filecodec = GetFileCodec(mediaInfo);

            videoCodec = GetVideoCodec(mediaInfo);

            audioCodec = GetAudioCodec(mediaInfo);
        }


        private static string GetAudioCodec(MediaInfo mediaInfo)
        {

            Application.DoEvents();

            string audioCodec;
            Debugger.LogMessageToFile("Getting audio codec...");

            try
            {
                audioCodec = mediaInfo.Get(StreamKind.Audio, 0, "Codec");
            }
            catch
            {
                audioCodec = string.Empty;
            }
            return audioCodec;
        }


        private static int GetAudioDuration(MediaInfo mediaInfo)
        {

            Application.DoEvents();

            int audioDuration = 0;
            string audioDurationStr;

            Debugger.LogMessageToFile("Getting audio duration...");

            try
            {
                audioDurationStr = mediaInfo.Get(StreamKind.Audio, 0, "Duration");
            }
            catch (Exception e)
            {
                Debugger.LogMessageToFile("[Media File Analyzer] An unexpected error occured while" +
                                          " the Media Analyzer was trying to obtain a media file's Audio Duration." +
                                          " The error was: " + e);

                audioDurationStr = string.Empty;
            }

            try
            {

                if (!String.IsNullOrEmpty(audioDurationStr))
                {
                    Debugger.LogMessageToFile("Converting audio duration...");
                    audioDuration = Convert.ToInt32(audioDurationStr);
                    audioDuration = audioDuration/60/1000;
                    Debugger.LogMessageToFile("Audio duration: " + audioDuration);
                }

            }
            catch (Exception exception)
            {
                Debugger.LogMessageToFile("An error occured while trying to convert audio duration. The error was: " + exception);
            }


            return audioDuration;

        }


        private static int GetVideoDuration(MediaInfo mediaInfo)
        {

            Application.DoEvents();

            int videoDuration = 0;

            Debugger.LogMessageToFile("Getting video duration...");
            string videoDurationStr = mediaInfo.Get(StreamKind.Video, 0, "Duration");

            if (!String.IsNullOrEmpty(videoDurationStr))
            {
                Debugger.LogMessageToFile("Converting video duration...");
                videoDuration = Convert.ToInt32(videoDurationStr);
                videoDuration = videoDuration/60/1000;
                Debugger.LogMessageToFile("Video duration: " + videoDuration);
            }


            return videoDuration;
        }


        private static string GetVideoCodec(MediaInfo mediaInfo)
        {
            Application.DoEvents();

            string videoCodec;

            #region Get video codec

            Debugger.LogMessageToFile("Getting video codec...");
            try
            {
                videoCodec = (mediaInfo.Get(StreamKind.Video, 0, "Codec"));
            }
            catch (Exception e)
            {
                Debugger.LogMessageToFile("[Media File Analyzer] An unexpected error occured while" +
                                          " the Media Analyzer was trying to obtain a media file's size." +
                                          " The error was: " + e);

                videoCodec = String.Empty;
            }

            #endregion

            return videoCodec;

        }


        private static string GetFileCodec(MediaInfo mediaInfo)
        {
            Application.DoEvents();

            string filecodec;

            #region Get file codec

            Debugger.LogMessageToFile("Getting file codec...");

            try
            {
                filecodec = (mediaInfo.Get(0, 0, "Codec"));
            }
            catch (Exception e)
            {
                Debugger.LogMessageToFile("[Media File Analyzer] An unexpected error occured while" +
                                          " the Media Analyzer was trying to obtain a media file's size." +
                                          " The error was: " + e);

                throw;
            }

            #endregion

            return filecodec;
        }


        private static string GetFileSize(MediaInfo mediaInfo)
        {
            Application.DoEvents();

            string filesize;

            Debugger.LogMessageToFile("Getting filesize...");

            try
            {
                filesize = mediaInfo.Get(0, 0, "FileSize");
            }
            catch (Exception e)
            {
                Debugger.LogMessageToFile("[Media File Analyzer] An unexpected error occured while" +
                                          " the Media Analyzer was trying to obtain a media file's size." +
                                          " The error was: " + e);
                throw;
            }

            return filesize;
        }


        private static void OpenMediaFileForProcessing(FileSystemInfo file, MediaInfo mediaInfo)
        {
            Application.DoEvents();

            MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress, "Analyzing file " + file.Name + "...");
            Debugger.LogMessageToFile(Environment.NewLine + "Creating MediaInfo instance for file " + file.Name + "...");

            try
            {
                int openResult = mediaInfo.Open(file.FullName);
                Debugger.LogMessageToFile("The media file's open result is: " + openResult);
            }
            catch (Exception e)
            {
                Debugger.LogMessageToFile("[Madia File Analyzer] An unexpected error occured while" +
                                          " the Media File Analyzer was trying to open a media file for Media Info extraction." +
                                          "The error was: " + e);
            }



            Application.DoEvents();
        }



    }


}
