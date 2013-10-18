using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using MediaInfoLib;
using MeediFier.ImportingEngine;
using MeediOS;

namespace MeediFier.MediaSnapshotEngine
{

    class RecursiveDirectoryScanner
    {

        internal static bool ScanDirectoryRecursively
            (

            string strDir, IMLSection moviesSection, IMLSection tvSection,
            IMLSection musicSection,
            ref string[] extensionsToIgnore, ref string[] filmLocations,
            ref string[] tvShowsLocations, ref string[] musicLocations,
            string[] videoExtensions, string[] audioExtensions, 
            int totalFiles, ref int currentFile, MediaInfo mediaInfo,
            string pluginPath, IEnumerable<string> combinedSceneTags,
            IEnumerable<string> videoExtensionsCommon              
            
            )
        {


                MainImportingEngine.GeneralStatus = "Performing media importing...";

                if (Helpers.UserCancels("Scanning directory " + strDir + "...", null))
                    return false;


                if (!DirectoryScanner.ImportFilesFromDirectory
                    (strDir, moviesSection, tvSection, musicSection,
                     ref extensionsToIgnore, ref filmLocations,
                     ref tvShowsLocations, ref musicLocations,
                     videoExtensions, audioExtensions, totalFiles,
                     ref currentFile, mediaInfo,
                     pluginPath, combinedSceneTags, videoExtensionsCommon ))

   



                    return false;


                IEnumerable<string> directories = new BindingList<string>();

                try
                {
                    directories = GetSubDirectories(strDir);
                }
                catch (PathTooLongException e)
                {
                    MessageBox.Show(@"Path too long");
                    Debugger.LogMessageToFile("[Directory scanner] An unexpected error occured while the Directory scanner" +
                                              " was trying to retrieve sub-directories of the directory " + strDir + 
                                              ". The error was: " + e);
                }
               

                return
                    directories == null || 
                    ScanSubDirectories(moviesSection, tvSection,
                    musicSection, ref extensionsToIgnore,
                    ref filmLocations, ref tvShowsLocations, ref musicLocations, 
                    videoExtensions, audioExtensions, totalFiles, ref currentFile,
                    mediaInfo, pluginPath, directories, combinedSceneTags,
                    videoExtensionsCommon);


       
        }


        private static IEnumerable<string> GetSubDirectories(string strDir)
        {
            string[] directories;

            try
            {
                directories = Directory.GetDirectories(strDir);
            }
            catch
            {
                return null;
            }

            return directories;
        }


        //TODO: make all these parameters a struct
        private static bool ScanSubDirectories
            (IMLSection moviesSection,
             IMLSection tvSection,
             IMLSection musicSection, 
             ref string[] extensionsToIgnore, 
             ref string[] filmLocations,
             ref string[] tvShowsLocations, 
             ref string[] musicLocations,
             string[] videoExtensions, 
             string[] audioExtensions, int totalFiles,
             ref int currentFile, MediaInfo mediaInfo, 
             string pluginPath, IEnumerable<string> directories,
             IEnumerable<string> combinedSceneTags,
             IEnumerable<string> videoExtensionsCommon)
        {


            foreach (string strDirSub in directories)
            {

                if (!ScanDirectoryRecursively
                    (strDirSub, moviesSection, tvSection, 
                     musicSection,
                    ref extensionsToIgnore, ref filmLocations, ref tvShowsLocations,
                    ref musicLocations, videoExtensions, audioExtensions,
                    totalFiles, ref currentFile, mediaInfo, pluginPath,
                    combinedSceneTags, videoExtensionsCommon))

                    return false;
            }

            return true;
        }


        internal static bool ScanSubDirectoriesAndImportMediaFiles
                   (string importRootFolder,
                    ICollection<FileInfo> allfilesFI,
                    IMLSection moviesSection, 
                    IMLSection tvSection,
                    IMLSection musicSection,
                    ref string[] extensionsToIgnore,
                    string[] filmLocations, string[] tvShowsLocations,
                    string[] musicLocations, string[] videoExtensions,
                    string[] audioExtensions, IEnumerable<string> combinedSceneTags,
                    string pluginPath, MediaInfo mediaInfo,
                    IEnumerable<string> videoExtensionsCommon)
        {
            try
            {
                Application.DoEvents();

                int currentFile = 0;

                if (
                    !ScanDirectoryRecursively
                         (importRootFolder,  moviesSection, tvSection, 
                          musicSection, 
                          ref extensionsToIgnore, ref filmLocations,
                          ref tvShowsLocations, ref musicLocations,
                         videoExtensions, audioExtensions, allfilesFI.Count,
                         ref currentFile, mediaInfo, pluginPath,
                         combinedSceneTags, videoExtensionsCommon
                         )
                    )
                    return false;

                Application.DoEvents();
            }
            catch (Exception e)
            {
                MessageBox.Show(@"The root media directory scanner returned an exception: " + e);
                Debugger.LogMessageToFile("The root media directory scanner returned an exception: " + e);
            }

            return true;
        }
   
    
    }

}
