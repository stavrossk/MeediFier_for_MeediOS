using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MediaInfoLib;
using MeediFier.Code.Media_File_Importers.Media_Analyzer;
using MeediFier.Code.Settings;
using MeediFier.MediaSnapshotEngine.MediaAnalyzer;

namespace MeediFier.MediaAnalyzer
{
    class MediaTypeDetector
    {


        internal static void AddFileToIgnoredExtensions
            (FileSystemInfo file, IList extensionsToIgnore,
            string audioCodec,
                                                       string fileName, bool isAudio, bool isVideo, string videoCodec,
                                                       string ext)
        {

            if (isVideo || isAudio)
                return;

            if ((ext == ".iso" || ext == ".ISO") || (ext == ".ifo" || ext == ".IFO") || (ext == ".pdf" || ext == ".PDF") ||
                (ext == ".djvu" || ext == ".DJVU")) 
                return;

            if (!Settings.EnableMediaDetection || !String.IsNullOrEmpty(videoCodec) || !String.IsNullOrEmpty(audioCodec))
                return;


            Debugger.LogMessageToFile("The MediaInfo process detected that the file " + fileName +
                                      " does not contain video nor audio. The file's extension " + ext +
                                      " will be added to the ignored extensions list.");

            extensionsToIgnore.Add(file.Extension);

        }



        internal static void AnalyzeFileDetectMediaType(FileInfo file, string fileName, MediaInfo mediaInfo, string pluginPath,
                                                       string ext, ref bool isAudio, ref bool isVideo, out int videoDuration,
                                                       out string filecodec, out string audioCodec, out string videoCodec,
                                                       out string filesize, out int audioDuration)
        {

            Application.DoEvents();

            UseMediaAnalyzerToDetectMediaDurations(file, mediaInfo,
                                                   isAudio, isVideo, out filesize, out filecodec,
                                                   out audioDuration, out videoDuration,
                                                   out videoCodec, out audioCodec);

            Application.DoEvents();

            DetectMediaTypeByUsingMediaDurations(fileName,
                                                 ref isVideo, audioDuration, videoDuration,
                                                 pluginPath, ext, ref isAudio);
        }



        internal static void DetectMediaTypeByUsingMediaDurations
            (string fileName, ref bool isVideo,
             int audioDuration, int videoDuration,
             string pluginPath, string ext, ref bool isAudio)
        {

            if (videoDuration >= 15)
            {

                AddFileTypeToKnownVideoExtensions
                    (pluginPath, ext, fileName, isVideo);


                isVideo = true;
            }


            if (videoDuration != 0
                || audioDuration < 2 
                || audioDuration > 17 
                || ext == ".wav"
                || ext == ".WAV") 
                return;


            AddFileTypeToKnownAudioExtensions
                (pluginPath, fileName, isAudio, ext);


            isAudio = true;


        }






        internal static void FileTypeIsMediaExtension
            (IEnumerable<string> videoExtensions,
                        IEnumerable<string> audioExtensions, string ext,
                        out bool isVideo, out bool isAudio, string fileName,
            IEnumerable<string> videoExtensionsCommon  )
        {

            Helpers.UpdateProgress("Importing Media Files...", 
                String.Format("Detecting media type of file {0}...", fileName) ,null);

            isVideo = MediaExtensionDictionaryMatcher
                .FileTypeIsVideoExtension
                (videoExtensionsCommon, ext);

            if (isVideo)
            {
                isAudio = false;
                return;
            }


            isVideo = MediaExtensionDictionaryMatcher
                .FileTypeIsVideoExtension
                (videoExtensions, ext);



            if (isVideo)
            {
                isAudio = false;
                return;
            }

            isAudio = MediaExtensionDictionaryMatcher
                .FileTypeIsAudioExtension
                (audioExtensions, ext);



        }




        internal static void UseMediaAnalyzerToDetectMediaDurations(FileInfo file, MediaInfo mediaInfo, bool isAudio,
                                                                   bool isVideo, out string filesize, out string filecodec,
                                                                   out int audioDuration, out int videoDuration,
                                                                   out string videoCodec, out string audioCodec)
        {

            #region init vars
            filesize = String.Empty;
            filecodec = String.Empty;

            videoCodec = String.Empty;
            audioCodec = String.Empty;

            audioDuration = 0;
            videoDuration = 0;
            #endregion

            if (!Settings.EnableMediaDetection)
                return;


            if (file.Extension == ".ISO" || file.Extension == ".iso" ||
                file.Extension == ".bin" || file.Extension == ".BIN" || 
                file.Extension == ".img" || file.Extension == ".IMG" ||
                file.Extension == ".vob" || file.Extension == ".VOB" ||
                file.Extension == ".ifo" || file.Extension == ".IFO"||
                file.Extension == ".bup" || file.Extension == ".BUP"
                ) 
                return;

            if
                (
          
                    (!isVideo && !isAudio)
                    ||
                    ( isVideo && file.Length >= 157286400 ) 
                    ||
                    (MediaImportersAndUpdatersCoreSettings
                    .EnableMusicImporter
                    && isAudio
                    && Settings.AnalyzeAudio
                    && file.Length >= 1048576)
                    
                )

            {

                //MessageBox.Show("Step 1");
                MediaInfoExtractor.AnalyzeMediaFileGetMediaInformation(file, mediaInfo,
                                                                                     out videoDuration,
                                                                                     out videoCodec, out audioCodec,
                                                                                     out audioDuration, out filesize,
                                                                                     out filecodec);

            }


        }


        internal static void AddFileTypeToKnownAudioExtensions(string pluginPath, string fileName, bool isAudio, string ext)
        {
            if (!isAudio)
            {
                Debugger.LogMessageToFile(String.Format("The file's extension {0} will be added to the known audio extensions list.", ext));
                var sw = File.AppendText(pluginPath + @"Media file extensions\" + "audio_extensions.txt");
                sw.WriteLine(ext);
                sw.Close();
            }

            Debugger.LogMessageToFile(String.Format("The MediaInfo process detected that the file {0} is a music track.", fileName));
        }





        internal static void AddFileTypeToKnownVideoExtensions(string pluginPath, string ext, string fileName, bool isVideo)
        {
            Debugger.LogMessageToFile(String.Format("The MediaInfo process detected that the file {0} is video.", fileName));

            if (!isVideo)
            {
                Debugger.LogMessageToFile(String.Format("The file's extension {0} will be added to the known video extensions list.", ext));
                var sw = File.AppendText(pluginPath + @"Media file extensions\" + "video_extensions.txt");
                sw.WriteLine(ext);
                sw.Close();
            }
        }


        internal static bool FileIsRarPart(string ext)
        {

            if (ext[1] == 'r' || ext[1] == 'R')
            {
                if (Char.IsNumber(ext[2]) && Char.IsNumber(ext[3]))
                {
                    //Debugger.LogMessageToFile("Ignoring multi-part rar archive " + file.Name);
                    return true;
                }
            }

            return false;
        }


    }



}
