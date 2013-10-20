using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using MediaInfoLib;
using MeediFier.Code.MediaInfo;
using MeediFier.Code.Settings;
using MeediFier.ImportingEngine;
using MeediFier.MediaAnalyzer;
using MeediOS;

namespace MeediFier.MediaSnapshotEngine
{


    class MediaSectionPopulator
    {

        internal static bool PopulateMediaSection
            (FileInfo file, IMLSection moviesSection, 
             IMLSection tvSection, IMLSection musicSection, 
             IEnumerable<string> videoexts, int audioDuration, 
             string filesize, MediaInfo mi, DirectoryInfo parent,
             string filecodec, bool isVideo, bool isAudio,
             int videoDuration, string fileName, string parentName,
             IEnumerable<string> combinedSceneTags)
        {


            if (String.IsNullOrEmpty(filecodec) && !isVideo && !isAudio)
                return true;

            //Debugger.LogMessageToFile(Environment.NewLine + "The file " + file.Name +
            //" has been detected as a valid media file." + Environment.NewLine + 
            //" Proceeding to media type recognition...");

            
            if (Helpers.UserCancels(MainImportingEngine.SpecialStatus, null))
                return false;


            ImportVideo(file, moviesSection, tvSection, videoexts, filesize, mi, parent,
                filecodec, isVideo, videoDuration, fileName, parentName, combinedSceneTags);

            if (Helpers.UserCancels(MainImportingEngine.SpecialStatus, null))
                return false;

            ImportAudio(file, musicSection, audioDuration, filesize, mi, filecodec, isAudio, fileName);

            return true;
        }






        private static void ImportAudio(FileInfo file, IMLSection musicSection, int audioDuration, string filesize, MediaInfo mi,
                                        string filecodec, bool isAudio, string fileName)
        {

            if (!MediaSectionPopulatorHelpers.ProceedToImportAudioTrack(file, isAudio))
                return;

            AddToMusicSection_ImportMediaInfo(file, musicSection, audioDuration, filesize, mi, filecodec, fileName);
        }



        // ReSharper disable InconsistentNaming
        private static void AddToMusicSection_ImportMediaInfo(FileSystemInfo file, IMLSection musicSection, int audioDuration,
// ReSharper restore InconsistentNaming
                                                              string filesize, MediaInfo mi, string filecodec, string fileName)
        {
            Application.DoEvents();
            //Debugger.LogMessageToFile("Importing audio track " + fileName + "...");
            MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress, "Importing audio track " + fileName + "...");

            IMLItem item;
            AddFileToSection(out item, musicSection, fileName, file.FullName, file.FullName);
            MediaInformation.ImportMediaInfoforAudio(mi, item, filesize, filecodec, audioDuration);
        }


        private static void ImportVideo(FileInfo file,
                                        IMLSection moviesSection, IMLSection tvSection,
                                        IEnumerable<string> videoexts, string filesize,
                                        MediaInfo mi, DirectoryInfo parent,
                                        string filecodec, bool isVideo, int videoDuration,
                                        string fileName, string parentName,
                                        IEnumerable<string> combinedSceneTags)
        {

            if (!isVideo) return;


            Debugger.LogMessageToFile("This file is a video file.");


            if (Helpers.UserCancels(MainImportingEngine.SpecialStatus, null))
                return;

            if ( MediaSectionPopulatorHelpers.SkipTrailer(file) )
                return;

            if (Helpers.UserCancels(MainImportingEngine.SpecialStatus, null))
                return;

            if (AdhereToVideoFilesizeThreshold(file)) 
                return;

            if (Helpers.UserCancels(MainImportingEngine.SpecialStatus, null))
                return;

            ImportMovie(file, moviesSection, videoexts, filesize, mi, parent, filecodec,
                videoDuration, fileName, parentName, combinedSceneTags);

            if (Helpers.UserCancels(MainImportingEngine.SpecialStatus, null))
                return;

            ImportEpisode(file, tvSection, filesize, mi, filecodec, videoDuration, fileName);
        }


        private static bool AdhereToVideoFilesizeThreshold(FileInfo file)
        {
            return file.Length < 52428800
                   &&
                   (!Settings.ImportFilmsInRarArchives || file.Extension != ".r00");
        }


        private static void ImportEpisode(FileSystemInfo file, IMLSection tvSection, string filesize, MediaInfo mi, string filecodec,
                                          int videoDuration, string fileName)
        {

            if (!MediaImportersAndUpdatersCoreSettings.EnableTvSeriesImporter) return;

            Debugger.LogMessageToFile("TV Series importer is enabled.");
            MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress, "Importing " + fileName + "...");

            bool videoIsLocatedInATvShowFolder = MediaSectionPopulatorHelpers.VideoIsLocatedInATvShowFolder(file);


            if ( (videoDuration >= 60 || videoDuration < 18)
                && !videoIsLocatedInATvShowFolder ) return;

   
            Debugger.LogMessageToFile("This video file is considered to be a TV episode.");
            Debugger.LogMessageToFile("Adding file " + file.FullName + " to TV Episodes section...");

            IMLItem item;
            AddFileToSection(out item, tvSection, fileName, file.FullName, file.FullName);

            MediaInformation.ImportMediaInfoforVideo(mi, item, filesize, filecodec, videoDuration);


        }


        private static void ImportMovie(FileInfo file, IMLSection moviesSection, IEnumerable<string> videoexts, string filesize,
                                        MediaInfo mi, DirectoryInfo parent, string filecodec, int videoDuration, string fileName,
                                        string parentName, IEnumerable<string> combinedSceneTags)
        {

            if (!MediaImportersAndUpdatersCoreSettings.EnableMovieImporter) return;
            Debugger.LogMessageToFile("[Media File Importers] Movie File Importer is enabled.");


            if (!MediaSectionPopulatorHelpers.ProceedToImportMovie(file, videoDuration)) 
                return;

            LocateTitle_AddToMoviesSection_ImportMediaInfo(file, moviesSection,
                videoexts, filesize, mi, parent,
                filecodec, videoDuration, fileName, parentName, combinedSceneTags );
     
        }



        // ReSharper disable InconsistentNaming
        private static void LocateTitle_AddToMoviesSection_ImportMediaInfo(FileSystemInfo file, IMLSection moviesSection,
        // ReSharper restore InconsistentNaming
                                                                           IEnumerable<string> videoexts, string filesize, MediaInfo mi,
                                                                           DirectoryInfo parent, string filecodec,
                                                                           int videoDuration, string fileName, string parentName,
                                                                           IEnumerable<string> combinedSceneTags)
        {
            Application.DoEvents();

            Debugger.LogMessageToFile("This video file is considered to be a film");

            MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress, "Importing film " + fileName + "...");
            Debugger.LogMessageToFile("Importing film " + fileName + "...");

            Application.DoEvents();

            var useParentFolder = MovieTitleLocatingEngine.MovieTitleLocator(file, videoexts, parent, fileName, 
                ref parentName, combinedSceneTags );

            Application.DoEvents();

            AddToMoviesSectionAndImportMediaInfo(file, moviesSection, filesize, mi, filecodec, videoDuration, fileName,
                                                 parentName, useParentFolder);

        }


        private static void AddToMoviesSectionAndImportMediaInfo(FileSystemInfo file, IMLSection moviesSection, string filesize,
                                                                 MediaInfo mi, string filecodec, int videoDuration,
                                                                 string fileName, string parentName, bool useParentFolder)
        {
        
            Application.DoEvents();

            IMLItem item;

            AddToMoviesSection
                (file, moviesSection, fileName,
                parentName, useParentFolder, out item);

            MediaInformation
                .ImportMediaInfoforVideo
                (mi, item, filesize, filecodec, videoDuration);
        
        }


        private static void AddToMoviesSection(FileSystemInfo file, IMLSection moviesSection, string fileName, string parentName,
                                               bool useParentFolder, out IMLItem item)
        {
            Application.DoEvents();

            if (useParentFolder)
            {
                Debugger.LogMessageToFile("Will use the parent directory's name as Item's name.");
                Debugger.LogMessageToFile(String.Format("Adding file {0} to Films section...", file.FullName));
                AddFileToSection(out item, moviesSection, parentName, file.FullName, file.FullName);
            }
            else
            {
                Debugger.LogMessageToFile("Will use the file's name as Item's name.");
                Debugger.LogMessageToFile(String.Format("Adding file {0} to Films section...", file.FullName));
                AddFileToSection(out item, moviesSection, fileName, file.FullName, file.FullName);
            }

        }


        internal static bool AnalyzeFileAndAddToMediaSection
            (FileInfo file, IMLSection moviesSection, IMLSection tvSection,
             IMLSection musicSection, IList<string> extensionsToIgnore,
             IEnumerable<string> videoExtensions, IEnumerable<string> combinedSceneTags,
             string fileExtension, bool isAudio, bool isVideo,
             DirectoryInfo parent, string parentName, string fileName,
             string pluginPath, MediaInfo mediaInfo)
        {
            int videoDuration;
            string filecodec;
            string audioCodec;
            string videoCodec;
            string filesize;
            int audioDuration;


            MediaTypeDetector.AnalyzeFileDetectMediaType
                (file, fileName, mediaInfo, pluginPath, 
                 fileExtension, ref isAudio, ref isVideo, 
                 out videoDuration, out filecodec,
                 out audioCodec, out videoCodec,
                 out filesize, out audioDuration);

            Application.DoEvents();

            MediaTypeDetector.AddFileToIgnoredExtensions
                (file, extensionsToIgnore, audioCodec,
                 fileName, isAudio, isVideo, videoCodec, fileExtension);

            Application.DoEvents();


            if (!PopulateMediaSection
                (file, moviesSection, tvSection,
                 musicSection, videoExtensions, audioDuration, 
                 filesize, mediaInfo, parent, filecodec,
                 isVideo, isAudio, videoDuration, fileName,
                 parentName, combinedSceneTags))
                return false;


            Application.DoEvents();

            return true;

        }




        internal static void AddFileToSection(out IMLItem item, IMLSection section,
            string itemName, string itemLocation, string externalID)
        {


            //TODO: Implement option to not update the Media Section on each imported item.

            if (Settings.UpdateMediaSectionOnEachImportedItem)
                section.BeginUpdate();


            item = section.AddNewItem(itemName, itemLocation);
            item.ExternalID = externalID;
            item.SaveTags();


            //BUG: Enabling this function makes importer unable to inport files!

            //MediaSectionPopulatorHelpers.ImportDates(item, externalID);


            if (Settings.UpdateMediaSectionOnEachImportedItem)
                section.EndUpdate();


      
        }
    
    
    
    }


}
