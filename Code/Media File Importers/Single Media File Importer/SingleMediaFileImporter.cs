using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using MediaInfoLib;
using MeediFier.Code.Media_File_Importers.Media_Analyzer;
using MeediFier.ImportingEngine;
using MeediFier.MediaAnalyzer;
using MeediFier.MediaFileImporters.SingleMediaFileImporter;
using MeediFier.MediaFileImportingEngine;
using MeediOS;

namespace MeediFier.MediaSnapshotEngine
{


    class SingleMediaFileImporter
    {

        internal static bool ImportMediaFile(FileInfo file,
            IMLSection moviesSection, IMLSection tvSection,
            IMLSection musicSection,
            ref IList<string> extensionsToIgnore,
            ref string[] filmLocations,
            ref string[] tvshowsLocations,
            ref string[] musicLocations,
            string[] videoExtensions, 
            string[] audioExtensions,
            int totalFiles, ref int currentFile, 
            IEnumerable<string> combinedSceneTags,
            MediaInfo mediaInfo,
            string pluginPath, 
            IEnumerable<string> videoExtensionsCommon)
        {
            try
            {

                Application.DoEvents();

                #region Init
                
                currentFile++;

                if (file == null)
                    return true;

                #region Set Progress status
                MainImportingEngine.CurrentProgress = ImportingEngineHelpers.ComputeProgress(currentFile, totalFiles);
                MainImportingEngine.GeneralStatus = "Performing media importing...";
                MainImportingEngine.SpecialStatus = String.Format("Checking file {0}...", file.Name);
                #endregion


                if (!File.Exists(file.FullName))
                    return true;



                #region file variables

                string parentName = string.Empty;
                string fileName;
                string fileExtension;
                bool isVideo;
                bool isAudio;

                if (SingleMediaFileImporterHelpers.RemoveExtensionFromFilename
                    (file, out fileName, out fileExtension))
                    return true;

                #region Retrieve Parent Directory
                DirectoryInfo parent = file.Directory;

                if (parent != null && !String.IsNullOrEmpty( parent.Name) )
                parentName = parent.Name;
                #endregion

                #endregion         

                #endregion

                Application.DoEvents();


                //if (SingleMediaFileImporterHelpers.HandleArchive
                //    (file, fileExtension))
                //    return true;


                if (SingleMediaFileImporterHelpers
                    .ImportAdditionalMediaTypes
                    (file, moviesSection, 
                    fileExtension,
                    parent, fileName, parentName))
                    return true;

                Application.DoEvents();

                if (MediaExtensionDictionaryMatcher.FileTypeBelongsToIgnoredExtensions
                    (file, extensionsToIgnore))
                    return true;


                Application.DoEvents();


                if (ExistingMediaItemSeachEngine
                    .SkipAlreadyImportedFiles
                    (file, filmLocations,
                    tvshowsLocations, musicLocations))
                    return true;


                Application.DoEvents();


                MediaTypeDetector
                    .FileTypeIsMediaExtension
                    (videoExtensions,
                    audioExtensions, fileExtension, 
                    out isVideo, out isAudio,
                    fileName, videoExtensionsCommon);


                Application.DoEvents();


                if (!MediaSectionPopulator.AnalyzeFileAndAddToMediaSection
                    (file, moviesSection, tvSection, musicSection,
                    extensionsToIgnore, videoExtensions, combinedSceneTags, 
                    fileExtension, isAudio, isVideo, parent, parentName,
                    fileName, pluginPath, mediaInfo)) 
                    return false;
           
            
            }
            catch (Exception e)
            {

                if (file != null)
                    Debugger.LogMessageToFile("An error occured while attempting to import a media file " + 
                        // + file.Name + 
                        "." + Environment.NewLine + "The error was: " + e);

            }


            return true;
    
        
        }
   
    
    
    }




}
