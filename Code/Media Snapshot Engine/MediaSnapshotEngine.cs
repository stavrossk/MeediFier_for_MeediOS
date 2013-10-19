using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using MediaInfoLib;
using MeediFier.ImportingEngine;
using MeediOS;

namespace MeediFier.MediaSnapshotEngine
{


    class MediaSnapshotEngine
    {


        internal static void ConstructAndWriteNewMediaSnapshot(IList<FileInfo> allfilesFI, string datPath)
        {

            StatusForm.statusForm.TrayIcon.ShowBalloonTip(5000, "Constructing media snapshot",
                                                          "MediaFairy is now analyzing your media files in order to create a filesystem snapshot." +
                                                          " This operation needs to be performed only once for each new media root directory, " +
                                                          "and will enable the application to detect and import only new files in future media importing requests.",
                                                          ToolTipIcon.Info);

            MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress, "Scanning directories...");
            Application.DoEvents();

            var filePaths = ConstructFilePathsArray(allfilesFI);


            WriteConstructedFilePathsToMediaSnapshotFile(filePaths, datPath);

        }

        internal static void WriteConstructedFilePathsToMediaSnapshotFile(string[] filePaths, string datPath)
        {
            MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress, "Saving snapshot...");

            Application.DoEvents();
            File.WriteAllLines(datPath, filePaths);
            Application.DoEvents();
        }

        internal static string[] ConstructFilePathsArray(IList<FileInfo> allfiles)
        {

            string[] filePaths = new string[allfiles.Count];

            for (int i = 0; i < allfiles.Count; i++)
            {
                try
                {
                    filePaths[i] = allfiles[i].FullName;
                }
                catch (Exception e)
                {

                    Debugger.LogMessageToFile("[Media Snapshot Constructor] An unexpected error occured while " +
                                              "the Media Snapshot Constructor was trying to create" +
                                              " a file entry for the media snapshot." +
                                              "The error was: " + e);

                }

            }

            return filePaths;

        }


        internal static bool ScanConstructedArrayAndImportMediaFiles
               (IMLSection moviesSection,
                IMLSection tvSection,
                IMLSection musicSection,
                ref IList<string> extensionsToIgnore,
                string[] filmLocations,
                string[] tvShowsLocations,
                string[] musicLocations,
                string[] videoExtensions,
                string[] audioExtensions,
                IEnumerable<string> combinedSceneTags,
                ICollection<FileInfo> filesToImport, MediaInfo mediaInfo,
                string pluginPath, IEnumerable<string> videoExtensionsCommon)
        {



            if (!Settings.UpdateMediaSectionOnEachImportedItem)
                ImportingEngineHelpers.BeginUpdatingSections();

            if ( !ImportFilesInArray(moviesSection, tvSection,
                musicSection, ref extensionsToIgnore, filmLocations,
                tvShowsLocations, musicLocations, videoExtensions, audioExtensions,
                combinedSceneTags, filesToImport, mediaInfo, pluginPath, videoExtensionsCommon)
               ) return false;


            if (!Settings.UpdateMediaSectionOnEachImportedItem)
                ImportingEngineHelpers.EndUpdatingSections();


            return true;
        }


        private static bool ImportFilesInArray
           (IMLSection moviesSection, IMLSection tvSection, 
            IMLSection musicSection, 
            ref IList<string> extensionsToIgnore, string[] filmLocations,
            string[] tvShowsLocations, string[] musicLocations,
            string[] videoExtensions, string[] audioExtensions, 
            IEnumerable<string> combinedSceneTags,
            ICollection<FileInfo> filesToImport, MediaInfo mediaInfo,
            string pluginPath, IEnumerable<string> videoExtensionsCommon)
        {


            if (filesToImport.Count <= 0) return true;


            int currentFile = 0;


            foreach (FileInfo file in filesToImport)
            {
                Application.DoEvents();

                if (SingleMediaFileImporter.ImportMediaFile(file, moviesSection,
                        tvSection, musicSection,
                        ref extensionsToIgnore, ref filmLocations,
                        ref tvShowsLocations, ref musicLocations, videoExtensions,
                        audioExtensions, filesToImport.Count, ref currentFile,
                        combinedSceneTags, mediaInfo, pluginPath, videoExtensionsCommon))
                    continue;


                ImportingEngineHelpers
                    .EndUpdatingSections
                    (moviesSection, tvSection,
                     musicSection);


                MainImportingEngine.CurrentProgress = 100;
                Helpers.UpdateProgress("All operations were cancelled. Completed jobs were save to library.", "", null);
                StatusForm.statusForm.Hide();
                StatusForm.statusForm.TrayIcon.Dispose();
                MainImportingEngine.ThisProgress = null;

                return false;
            }
            return true;


        }




        internal static string ConstructMediaSnapshotLocation(string importRootFolder, string pluginpath)
        {
            DirectoryInfo rootDirectory = new DirectoryInfo(importRootFolder);

            string datName = rootDirectory.Name;
            datName = datName.TrimEnd('\\');
            datName = datName.TrimEnd(':');

            string datPath = Path.Combine(pluginpath, "Media Snapshots\\");
            string mediaSnapshotLocation = String.Format("{0}{1}.dat", datPath, datName);
            return mediaSnapshotLocation;

        }


        // ReSharper disable InconsistentNaming
        internal static bool IfMediaSnaphotExists_ScanDirectoriesAndImportNewFiles_ElseCreateNewSnapshotAndImportFilesFromScratch
            // ReSharper restore InconsistentNaming
            // ReSharper restore InconsistentNaming
            (string importRootFolder,
             string pluginpath, FileInfo[] allfilesFI,
             IMLSection moviesSection, 
             IMLSection tvSection, 
             IMLSection musicSection,
             ref IList<string> extensionsToIgnore,
             string[] filmLocations, 
             string[] tvShowsLocations,
             string[] musicLocations, 
             string[] videoExtensions,
             string[] audioExtensions,
             IEnumerable<string> combinedSceneTags, 
             IEnumerable<string> videoExtensionsCommon)
        {

            var mediaSnapshotLocation = ConstructMediaSnapshotLocation(importRootFolder, pluginpath);

            string pluginPath = Debugger.GetPluginPath();
            MediaInfo mediaInfo = new MediaInfo();

            bool mediaSnapshotExists = File.Exists(mediaSnapshotLocation);

            if (mediaSnapshotExists)
            {

                if (!MediaSnapshotComparingEngine
                    .CompareMediaSnapshotsAndImportNewMediaFiles
                    (allfilesFI, moviesSection,
                    tvSection, musicSection, ref extensionsToIgnore, filmLocations,
                    tvShowsLocations, musicLocations, videoExtensions, audioExtensions,
                    combinedSceneTags, mediaSnapshotLocation, mediaInfo, pluginPath,
                    videoExtensionsCommon ))
                    return false;

            }
            else
            {

                if (!MediaSnapshotComparingEngine.CreateNewMediaSnapshotScanDirectoriesImportMediaFiles(
                    importRootFolder, allfilesFI, moviesSection, tvSection, musicSection,
                    ref extensionsToIgnore, filmLocations, tvShowsLocations, musicLocations,
                    videoExtensions, audioExtensions, combinedSceneTags, mediaInfo,
                    pluginPath, mediaSnapshotLocation, videoExtensionsCommon))
                    return false;

            }


            return true;
        }
    }



}
