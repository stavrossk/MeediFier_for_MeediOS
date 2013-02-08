using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MediaFairy.Code.Media_File_Importers.Media_Importing_Engine;
using MediaFairy.ImportingEngine;
using MediaInfoLib;
using MeediOS;



namespace MediaFairy.MediaSnapshotEngine
{


    class MediaImportingEngineHelpers
    {





        internal static string[] CacheMediaExtensionsTables
            (string pluginpath, out string[] audioExtensions,
             out string[] videoExtensions, out string[] videoExtensionsCommon)
        {


            Debugger.LogMessageToFile
                ("Loading media extension tables...");

            #region Cache media extensions tables

            string nonMediaFile = pluginpath 
                + @"Media file extensions\" 
                + "non-media_extensions.txt";


            string[] extensionsToIgnore = File.Exists(nonMediaFile)
                                              ? File.ReadAllLines(nonMediaFile)
                                              : null;
   


            videoExtensions = File.Exists(pluginpath
                                  + @"Media file extensions\" 
                                  + "video_extensions.txt")
                                  ? File.ReadAllLines(pluginpath 
                                  + @"Media file extensions\" 
                                  + "video_extensions.txt")
                                  : null;



            videoExtensionsCommon = File.Exists(pluginpath 
                                  + @"Media file extensions\" 
                                  + "video extensions common.txt")
                                  ? File.ReadAllLines(pluginpath 
                                  + @"Media file extensions\" 
                                  + "video extensions common.txt")
                                  : null;



            audioExtensions = File.Exists(pluginpath
                                  + @"Media file extensions\" 
                                  + "audio_extensions.txt")
                                  ? File.ReadAllLines(pluginpath 
                                  + @"Media file extensions\"
                                  + "audio_extensions.txt")
                                  : null;

            #endregion

            return extensionsToIgnore;
        }



        internal static void DeleteSnapshotsOnEmptySectionsDetection
            (IEnumerable<FileInfo> datfiles, int tvShowsItemcount,
             int moviesItemcount, int musicItemcount)
        {

            if (moviesItemcount != 0
                || musicItemcount != 0 
                || tvShowsItemcount != 0)
                return;

            Debugger.LogMessageToFile
                ("Deleting media snapshots...");

            foreach (FileInfo datfile in datfiles)
                File.Delete(datfile.FullName);


        }






        internal static void CacheMediaSnapshots(IList<FileInfo> datfiles)
        {
            Debugger.LogMessageToFile("Caching media snapshots...");

            ArrayList[] datEntries = new ArrayList[datfiles.Count];

            if (datfiles.Count == 0) return;

            for (int i = 0; i < datfiles.Count; i++)
            {

                datEntries[i] = new ArrayList();

                try
                {
                    string datfile = datfiles[i].FullName;
                    string[] datfileData = File.ReadAllLines(datfile);
                    datEntries[i].Add(datfileData);
                }
                catch (Exception e)
                {
                    StatusForm.statusForm.TrayIcon.ShowBalloonTip(10000,"Unable to locate a media snapshot",
                        "MediaFairy was unable to open a media snapshot file. " +
                        "Please make sure all media snapshot files are located in the corresponding locations and try again." +
                        " The importing session cannot continue.",
                        ToolTipIcon.Error);

                    Debugger.LogMessageToFile("[Media Importing Engine] The Media Importing Engine was unable" +
                                              " to open a media snapshot file because of an unexpected error." +
                                              " The error was: " + e);

                    throw;
                }


            }

        }


        internal static void CountMediaSectionsItems
            (out int musicItemcount, 
            out int moviesItemcount, 
            out int tvShowsItemcount)
        {
            Debugger.LogMessageToFile
                ("Enumerating sections...");


            #region Movies
            try
            {
                moviesItemcount = MediaSectionsAllocator.MoviesSection.ItemCount;
            }
            catch (Exception)
            {
                moviesItemcount = 0;
            }
            #endregion


            #region TV Episodes
            try
            {
                tvShowsItemcount = MediaSectionsAllocator.TvEpisodesSection.ItemCount;
            }
            catch (Exception)
            {
                tvShowsItemcount = 0;
            }
#endregion


            #region Music
            try
            {
                musicItemcount = MediaSectionsAllocator.MusicSection.ItemCount;
            }
            catch (Exception)
            {
                musicItemcount = 0;
            }
            #endregion


            #region Books

            //try
            //{
            //    BooksItemcount = MediaSectionsAllocator.BooksSection.ItemCount;
            //}
            //catch (Exception)
            //{
            //    BooksItemcount = 0;
            //}

            #endregion Books


        }


        internal static void PerformPreImportCaching
            (
            IList<FileInfo> datfiles, string pluginpath,
            out string[] filmLocations, out string[] musicLocations,
            out string[] tvShowsLocations, out string[] extensionsToIgnore,
            out string[] videoExtensions, out string[] audioExtensions,
            out string[] videoExtensionsCommon
            )
        {
            int moviesItemcount;
            int musicItemcount;
            int tvShowsItemcount;

            CountMediaSectionsItems(out musicItemcount, out moviesItemcount, out tvShowsItemcount);

            DeleteSnapshotsOnEmptySectionsDetection(datfiles, tvShowsItemcount, moviesItemcount,
                                                                                musicItemcount);

            CacheMediaSnapshots(datfiles);


            extensionsToIgnore = CacheMediaExtensionsTables
                (pluginpath, out audioExtensions,
                out videoExtensions, out videoExtensionsCommon);



            #region Cache Media Sections

            Debugger.LogMessageToFile("Caching media sections...");

            filmLocations = MediaSectionCacher.CacheMoviesSection();
            tvShowsLocations = MediaSectionCacher.CacheTvShowsSection();
            musicLocations = MediaSectionCacher.CacheMusicSection();

            #endregion
        }




        internal static ArrayList TestDirectoryAccessGetFileList
            (string directoryStr, int totalFiles, 
            int currentFile, out DirectoryInfo directory)
        {

            if (TestDirectoryAccess(directoryStr, out directory))
                return null;

            Application.DoEvents();

            var filesInDirectory = ImportingEngineHelpers.GetDirectoryFileList
                (directoryStr, totalFiles, currentFile, directory);

            Application.DoEvents();

            return filesInDirectory;

        }



        internal static bool TestDirectoryAccess
            (string directoryStr, out DirectoryInfo directory)
        {
            Application.DoEvents();

            directory = ImportingEngineHelpers.TestDirectoryAccess(directoryStr);

            return directory == null;
        }


        internal static void WriteNonMediaExtensionsToFile
            (string[] extensionsToIgnore, string pluginpath)
        {
            File.WriteAllLines(pluginpath
                + @"Media file extensions\"
                + "non-media_extensions.txt",
                extensionsToIgnore);
        }


        internal static bool ImportEachFileInDirectory
            (IMLSection moviesSection,
             IMLSection tvSection,
             IMLSection musicSection,
             ref string[] extensionsToIgnore,
             ref string[] filmLocations,
             ref string[] tvShowsLocations,
             ref string[] musicLocations, 
             string[] videoExtensions,
             string[] audioExtensions, 
             int totalFiles, ref int currentFile,
             IEnumerable<string> combinedSceneTags,
             ArrayList filesInDirectory,
             MediaInfo mediaInfo, string pluginPath,
             IEnumerable<string> videoExtensionsCommon)
        {

            foreach (FileInfo file in filesInDirectory)
            {
                Application.DoEvents();

                if (!SingleMediaFileImporter
                    .ImportMediaFile
                    (file, moviesSection, tvSection,
                    musicSection, ref extensionsToIgnore, ref filmLocations,
                            ref tvShowsLocations, ref musicLocations,
                            videoExtensions, audioExtensions, totalFiles,
                            ref currentFile, combinedSceneTags, mediaInfo,
                            pluginPath, videoExtensionsCommon))
                    return false;
            }


            return true;


        }




        internal static FileInfo[] LoadMediaSnapshots
            (out string pluginpath)
        {
            pluginpath = Debugger.GetPluginPath();
            string snapshotsPath = Path.Combine(pluginpath, "Media Snapshots\\" );

            DirectoryInfo snapshotsPathFI = new DirectoryInfo(snapshotsPath);
            FileInfo[] datfiles = snapshotsPathFI.GetFiles("*.dat");

            return datfiles;
        }




        internal static bool RunMediaSectionsUpdatingEngine
            (Importer importer, IMLSection section,
            IEnumerable<string> combinedSceneTags, 
            ConnectionResult connectionresult)
        {
            var updatemediaSectionParams =
                InitUpdatemediaSectionParams(importer, section, combinedSceneTags);


            return !MediaUpdaters.UpdateMediaSections
                        (updatemediaSectionParams, ref connectionresult);

        }




        internal static UpdateMediaSectionsParams 
            InitUpdatemediaSectionParams
            (Importer importer, IMLSection section, 
            IEnumerable<string> combinedSceneTags)
        {
            UpdateMediaSectionsParams updatemediaSectionParams = new
                UpdateMediaSectionsParams(section, importer.MdfSettingsa,
                                          importer.Ibs, importer.MdfSettingsb,
                                          importer.MdfSettingsc, combinedSceneTags);

            return updatemediaSectionParams;
        }



        internal static ConnectionResult InitConnectionresult()
        {
            var connectionresult =
                new ConnectionResult
                    {InternetConnectionAvailable = true, OSDbIsOnline = true};

            return connectionresult;
        }








    }

}
