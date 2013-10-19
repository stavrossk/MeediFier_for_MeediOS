using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MediaInfoLib;
using MeediFier.ImportingEngine;
using MeediOS;


namespace MeediFier.MediaSnapshotEngine
{


    internal class MediaSnapshotComparingEngine
    {



        internal static string[] LoadAndUpdateStoredMediaSnapshot(string datPath, string[] filePaths)
        {
            #region read .dat file and store to oldfilePaths

            MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress, "Loading previously stored snapshot...");
            string[] oldfilePaths = File.ReadAllLines(datPath);

            #endregion

            MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress, "Updating media snapshot...");

            #region write new .dat file with the new paths.

            File.WriteAllLines(datPath, filePaths);

            #endregion

            return oldfilePaths;

        }


        internal static FileInfo[] CompareMediaSnapshotsAddFilesToScanningList(string[] filmLocations, string[] tvShowsLocations,
                                                                              string[] musicLocations, string[] oldfilePaths,
                                                                              string[] filePaths)
        {
            MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress, "Comparing media snapshots...");


            FileInfo[] filesInDir = new FileInfo[filePaths.Length];

            for (int i = 0; i < filePaths.Length; i++)
            {
                var matchfound = FileHasAlreadyBeenScanned(i, filePaths, oldfilePaths);

                var foundInLibrary = FileExistsInMediaSections(filmLocations, tvShowsLocations, musicLocations, filePaths, i);

                AddFileToScanningList(i, filesInDir, filePaths, matchfound, foundInLibrary);
            }
            return filesInDir;
        }


        private static void AddFileToScanningList(int i, IList<FileInfo> filesInDir, IList<string> filePaths, bool matchfound,
                                                  bool foundInLibrary)
        {
            #region If no match has been found, add this file to array to scan

            if (
                matchfound &&
                (foundInLibrary || !Settings.RescanFilesNotFoundInLibrary)
                ) return;


            if (File.Exists(filePaths[i]))
                filesInDir[i] = new FileInfo(filePaths[i]);

            #endregion
        }



        private static bool FileHasAlreadyBeenScanned(int i, IList<string> filePaths, IEnumerable<string> oldfilePaths)
        {
            #region Compare with old snapshot to detect if the file has already been scanned 

            bool matchfound = false;

            foreach (string t in oldfilePaths)
                if (filePaths[i] == t)
                    matchfound = true;

            #endregion

            return matchfound;
        }


        internal static string[] CacheCurrentFileEntries(FileInfo[] allfiles)
        {
            MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress, "Caching current file entries...");

            string[] filePaths = new string[allfiles.Length];

            for (int i = 0; i < allfiles.Length; i++)
                filePaths[i] = allfiles[i].FullName;
            return filePaths;
        }


        private static bool FileExistsInMediaSections(IEnumerable<string> filmLocations, IEnumerable<string> tvShowsLocations, IEnumerable<string> musicLocations,
                                                      IList<string> filePaths, int i)
        {

            if (filmLocations != null)
            {
                if (filmLocations.Any(t => filePaths[i] == t))
                    return true;
            }

  
            if (tvShowsLocations != null)
            {
                if (tvShowsLocations.Any(t => filePaths[i] == t))
                    return true;
            }
            

    
            return musicLocations != null && musicLocations.Any(t => filePaths[i] == t);
        
        }

        internal static bool CompareMediaSnapshotsAndImportNewMediaFiles
                    (FileInfo[] allfilesFI,
                     IMLSection moviesSection,
                     IMLSection tvSection,
                     IMLSection musicSection,
                     ref IList<string> extensionsToIgnore, 
                     string[] filmLocations,
                     string[] tvShowsLocations, string[] musicLocations,
                     string[] videoExtensions, string[] audioExtensions,
                     IEnumerable<string> combinedSceneTags,
                     string mediaSnapshotLocation, MediaInfo mediaInfo,
                     string pluginPath, IEnumerable<string> videoExtensionsCommon)
        {


            Application.DoEvents();

            var filesToImport = CompareSnapshotsAndConstructArrayOfFilesToImport
                (allfilesFI, filmLocations, tvShowsLocations,
                musicLocations, mediaSnapshotLocation);

            Application.DoEvents();


            return MediaSnapshotEngine.ScanConstructedArrayAndImportMediaFiles(
                moviesSection, tvSection, musicSection,
                ref extensionsToIgnore, filmLocations, tvShowsLocations, musicLocations,
                videoExtensions, audioExtensions, combinedSceneTags, filesToImport,
                mediaInfo, pluginPath, videoExtensionsCommon);


        }

        internal static bool CreateNewMediaSnapshotScanDirectoriesImportMediaFiles
                    (string importRootFolder,
                     FileInfo[] allfilesFI,
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
                     MediaInfo mediaInfo, string pluginPath,
                     string mediaSnapshotLocation, 
                     IEnumerable<string> videoExtensionsCommon)
        {

            if (Settings.EnableMediaSnapshots)
            MediaSnapshotEngine
                .ConstructAndWriteNewMediaSnapshot
                (allfilesFI, mediaSnapshotLocation);


            return RecursiveDirectoryScanner
                .ScanSubDirectoriesAndImportMediaFiles
                    (importRootFolder, allfilesFI, moviesSection,
                    tvSection, musicSection,
                    ref extensionsToIgnore,
                    filmLocations, tvShowsLocations, musicLocations,
                    videoExtensions, audioExtensions,
                    combinedSceneTags, pluginPath, mediaInfo,
                    videoExtensionsCommon);

        
        }


        internal static FileInfo[] CompareSnapshotsAndConstructArrayOfFilesToImport(FileInfo[] allfiles, string[] filmLocations,
                                                                                    string[] tvShowsLocations,
                                                                                    string[] musicLocations, string datPath)
        {
            MainImportingEngine.CurrentProgress = 0;

            var filePaths = CacheCurrentFileEntries(allfiles);

            var oldfilePaths = LoadAndUpdateStoredMediaSnapshot(datPath, filePaths);

            FileInfo[] filesInDir = CompareMediaSnapshotsAddFilesToScanningList(filmLocations, tvShowsLocations, musicLocations, oldfilePaths, filePaths);

            return filesInDir;    
        }
    }


}
