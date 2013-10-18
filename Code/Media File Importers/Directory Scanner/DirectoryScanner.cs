using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using MediaInfoLib;
using MeediFier.ImportingEngine;
using MeediOS;

namespace MeediFier.MediaSnapshotEngine
{
    internal class DirectoryScanner
    {

        internal static bool ScanMediaDirectories
            (
            string[] mediaFolders, 
            ref string[] extensionsToIgnore, 
            string[] filmLocations, string[] tvShowsLocations,string[] musicLocations,
            string[] videoExtensions, string[] audioExtensions,
            IEnumerable<string> combinedSceneTags,
            IEnumerable<string> videoExtensionsCommon, Importer importer 
            )
        {


            string pluginpath = Debugger.GetPluginPath();

            foreach (string importRootFolder in mediaFolders)

                if (
                    !CalculateExaminationtimeAndScanMediaFolder
                    (MediaSectionsAllocator.MoviesSection,
                    MediaSectionsAllocator.TvEpisodesSection,
                    MediaSectionsAllocator.MusicSection,
                    ref extensionsToIgnore, filmLocations, 
                    tvShowsLocations,  musicLocations,
                    videoExtensions, audioExtensions,
                    importRootFolder, pluginpath, combinedSceneTags,
                    videoExtensionsCommon )
                    )
                    return false;
            

            return true;
        }



        private static bool CalculateExaminationtimeAndScanMediaFolder
            (IMLSection moviesSection, 
            IMLSection tvSection,
            IMLSection musicSection,
            ref string[] extensionsToIgnore,
            string[] filmLocations, 
            string[] tvShowsLocations,
            string[] musicLocations,
            string[] videoExtensions,
            string[] audioExtensions,
            string importRootFolder,
            string pluginpath,
            IEnumerable<string> combinedSceneTags, 
            IEnumerable<string> videoExtensionsCommon)
        {

            if (ValidateRootDirectory(importRootFolder))
                return true;

            #region File Importer varibales

            MainImportingEngine.CurrentProgress = 0;
            MainImportingEngine.GeneralStatus = "Importing Media files";
            MainImportingEngine.SpecialStatus = String.Empty;

            #endregion

            try
            {
                var allfiles = ScanDirectoriesConstructIncludedDirectoriesAndFilesArrays(importRootFolder);

                Application.DoEvents();

                if (
                    !MediaSnapshotEngine
                    .IfMediaSnaphotExists_ScanDirectoriesAndImportNewFiles_ElseCreateNewSnapshotAndImportFilesFromScratch
                            (importRootFolder, pluginpath, allfiles, moviesSection, tvSection,
                             musicSection, ref extensionsToIgnore,
                             filmLocations, tvShowsLocations, musicLocations, videoExtensions,
                             audioExtensions, combinedSceneTags, videoExtensionsCommon))
                    return false;


            }
            catch (Exception e)
            {
                Debugger.LogMessageToFile(e.ToString());
            }

            return true;
        
        }




        private static bool ValidateRootDirectory(string importRootFolder)
        {
            MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress, "Validating root directory...");
            if (!Directory.Exists(importRootFolder))
                return true;
            Debugger.LogMessageToFile("Import root directory was validated.");
            return false;
        }

        private static FileInfo[] ScanDirectoriesConstructIncludedDirectoriesAndFilesArrays(string importRootFolder)
        {
            Application.DoEvents();

            var dirsTotal = CalculateTotalDirectoryCount
                (importRootFolder);

            Helpers.UpdateProgress
                ("Importing Media Files...",
                "Total Directories: "
                + dirsTotal ,null);


            Application.DoEvents();
            //Thread.Sleep(2000);

            int dirCounter = 0;
            FileInfo[] allfiles;

            ExamineDirectoryConstructFilesArray
                (importRootFolder, out allfiles,
                dirsTotal, ref dirCounter);



            Helpers.UpdateProgress
                ("Importing Media Files...",
                "Total files: " 
                + allfiles.Length, null);


            Application.DoEvents();
            //Thread.Sleep(2000);

            return allfiles;
        }


        private static int CalculateTotalDirectoryCount(string importRootFolder)
        {
            MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress, "Calculating remaining time...");
            Debugger.LogMessageToFile("Calculating root media directory examination time...");

            int dirsTotal = 0;
            int currentDir = 0;

            try
            {
                dirsTotal = GetDirectoryCount(importRootFolder, ref currentDir);
            }
            catch (Exception e)
            {
                Debugger.LogMessageToFile("The root directory enumerator caused an exception: " + e);
            }

            return dirsTotal;

        }


        internal static int GetDirectoryCount(string dir, ref int dirCount)
        {
            if (!dir.EndsWith("\\"))
                dir = dir + "\\";

            string[] allFiles = Directory.GetFileSystemEntries(dir);

            //loop through all items
            foreach (string file in allFiles.Where(Directory.Exists))
            {
                //recursive call
                dirCount++;

                Helpers.UpdateProgress("Importing Media Files...",
                    "[Total Directories: " + dirCount + "] Counting directories...", null);

                Application.DoEvents();

                GetDirectoryCount(file, ref dirCount);
            }
       


            return dirCount;

        }


        internal static int ExamineDirectoryConstructFilesArray
            (string strDir, out FileInfo[] allfiles, int dirsTotal, ref int i)
        {
            ArrayList directoriesArray = new ArrayList();
            ArrayList filesList = new ArrayList();

            // Make sure we catch exceptions  in looking at this directory.

            // Loop through the list of directories.
            string[] allDirs = Directory.GetDirectories(strDir);

            #region ...for each directory in Root...
            foreach (string strDirName in allDirs)
            {
                try
                {

                    MainImportingEngine.CurrentProgress = ImportingEngineHelpers.ComputeProgress(i, dirsTotal);

                    MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress, "[files found: "
                        + filesList.Count + " ]" +
                        " Examining directory "  + strDirName + "...");

                    DirectoryInfo dirI = new DirectoryInfo(strDirName);

                    try
                    {
                        filesList.AddRange(dirI.GetFiles("*.*", SearchOption.AllDirectories));
                        directoriesArray.AddRange(dirI.GetDirectories("*.*", SearchOption.AllDirectories));
                    }
                    catch
                    {
                        continue;
                    }


                    i++;

                    ExamineDirectoryConstructFilesArray(strDirName, out allfiles, dirsTotal, ref i);



                }
                catch (Exception e)
                {
                    StatusForm.statusForm.TrayIcon.ShowBalloonTip(10000, "An unexpected error occured.", "An error ocurred in the media directories examination process. Please refer to Debug.log file for details.", ToolTipIcon.Error);
                    Debugger.LogMessageToFile("An unexpected error occured in the media directories examination process. The error was: " + e.ToString());
                }
            }
            #endregion

            allfiles = (FileInfo[])filesList.ToArray(typeof(FileInfo));

           
            return allfiles.Length;
        
        }



        internal static bool ImportFilesFromDirectory
            (
            string directoryStr,
            IMLSection moviesSection,
            IMLSection tvSection,
            IMLSection musicSection,
            ref string[] extensionsToIgnore, 
            ref string[] filmLocations, 
            ref string[] tvShowsLocations, 
            ref string[] musicLocations,
            string[] videoExtensions,
            string[] audioExtensions,
            int totalFiles,
            ref int currentFile,
            MediaInfo mediaInfo, 
            string pluginPath,
            IEnumerable<string> combinedSceneTags,
            IEnumerable<string> videoExtensionsCommon 
            )
        {

            DirectoryInfo directory;

            var filesInDirectory =
                MediaImportingEngineHelpers.TestDirectoryAccessGetFileList(directoryStr, totalFiles,
                                                                           currentFile, out directory);

            if (filesInDirectory == null)
                return true;


            const int videosCount = 0;

            string externalID = String.Empty;

            bool isUNC = directoryStr.StartsWith("\\");




            if (MultipartMovieDetectionEngine
                .DetectAndImportMultipartMovie
                (moviesSection, mediaInfo, isUNC,
                 directory, externalID, filesInDirectory,
                 videoExtensions, videosCount)) 
                return true;


            return MediaImportingEngineHelpers
                .ImportEachFileInDirectory
                (moviesSection, tvSection, musicSection, 
                 ref extensionsToIgnore, ref filmLocations, 
                 ref tvShowsLocations,  ref musicLocations, 
                 videoExtensions, audioExtensions,
                 totalFiles, ref currentFile, combinedSceneTags, 
                 filesInDirectory, mediaInfo, pluginPath,
                 videoExtensionsCommon);
      
        
        }
 
    
    
    }



}
