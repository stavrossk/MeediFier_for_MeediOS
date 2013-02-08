//'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
//'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
//''    MediaFairy                                                               ''
//''    Copyright (C) 2008-2010  Stavros Skamagkis                               ''
//''                                                                             ''
//''    This program is free software: you can redistribute it and/or modify     ''
//''    it under the terms of the GNU General Public License as published by     ''
//''    the Free Software Foundation, either version 3 of the License, or        ''
//''    (at your option) any later version.                                      ''
//''                                                                             ''
//''    This program is distributed in the hope that it will be useful,          ''
//''    but WITHOUT ANY WARRANTY; without even the implied warranty of           ''
//''    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the            ''
//''    GNU General Public License for more details.                             ''
//''                                                                             ''
//''    You should have received a copy of the GNU General Public License        ''
//''    along with this program.  If not, see <http://www.gnu.org/licenses/>.    ''
//'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
//'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''



using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Collections;
using MediaFairy.Code.Settings;
using MediaFairy.ImportingEngine;
using MediaFolders;
using MeediOS;

namespace MediaFairy
{
    public static class ImportingEngineHelpers
    {

        public static void EndUpdatingSections()
        {
            throw new NotImplementedException();
        }


        internal static bool PerformInitializationTasks(IMLSection section)
        {
            MainImportingEngine.CurrentProgress = 0;
            MainImportingEngine.GeneralStatus = "Performing startup operations...";
            MainImportingEngine.SpecialStatus = String.Empty;


            if (Helpers.UserCancels("MediaFairy is initializing...", null))
                return true;



            #region Write first debug lines for the importing proccess
            Debugger.LogMessageToFile(Environment.NewLine);
            Debugger.LogMessageToFile(Environment.NewLine);
            Debugger.LogMessageToFile(@"----------------------   START OF MAIN IMPORTING PROCCESS   ----------------");
            Debugger.LogMessageToFile("Plugin path: " + Debugger.GetPluginPath());
            #endregion

            if (Helpers.UserCancels("MediaFairy is initializing...", null))
                return true;



            ConfigureDepedentSettings();


            if (Helpers.UserCancels("Allocatiing Media Sections...", null))
                return true;



            MediaSectionsAllocator
                .AllocateMediaSections(section);


            //if (Helpers.UserCancels("Configuring Media Watcher...", null))
            //    return true;

            //MediaWatcherAgents.ConfigureMediaWatcher(section);



            VideoFilenameCleaner.ReadDictionaryReturnAllTags();



            return false;
        }

        internal static void FinishImport()
        {
            StatusForm.statusForm.UpdateProgress(100);
            StatusForm.statusForm.TrayIcon.ShowBalloonTip(500, "MediaFairy completed successfully.", " ", ToolTipIcon.Info);
            MainImportingEngine.CurrentProgress = 100;
            MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress, "MediaFairy completed successfully!");
            Debugger.LogMessageToFile("MediaFairy completed successfully!");
            Thread.Sleep(1000);
            StatusForm.statusForm.TrayIcon.Dispose();
            //StatusForm.statusForm.Close();
            StatusForm.statusForm.Dispose();
        }

        private static void ConfigureDepedentSettings()
        {

            if (!MediaImportersAndUpdatersCoreSettings.EnableMediaImporters)
            {
                Debugger.LogMessageToFile("Media Importing is disabled. All media importers will be disabled.");
                MediaImportersAndUpdatersCoreSettings.EnableMovieImporter = false;
                MediaImportersAndUpdatersCoreSettings.EnableMusicImporter = false;
                MediaImportersAndUpdatersCoreSettings.EnableTvSeriesImporter = false;
            }
          

            #region Disable general Media Importer if all individual media importers are disabled.


            if (!MediaImportersAndUpdatersCoreSettings.EnableMovieImporter
                && !MediaImportersAndUpdatersCoreSettings.EnableTvSeriesImporter
                && !MediaImportersAndUpdatersCoreSettings.EnableMusicImporter)
            {
                Debugger.LogMessageToFile("All media importers are disabled. The general Media Importing setting will also be disabled.");
                MediaImportersAndUpdatersCoreSettings.EnableMediaImporters = false;
            }
            #endregion



            #region Disable all updaters if general media updating is disabled.
            if (!MediaImportersAndUpdatersCoreSettings.EnableMediaUpdaters)
            {
                Debugger.LogMessageToFile("Media Updating is disabled. All media updaters will be disabled.");
                Settings.EnableMoviesSectionUpdater = false;
                Settings.EnableTvShowsSectionUpdater = false;
                Settings.EnableMusicSectionUpdater = false;
            }
            #endregion




            #region Disable general Media Updater if all individual media updaters are disabled.


            if (!Settings.EnableMoviesSectionUpdater 
                && !Settings.EnableTvShowsSectionUpdater
                && !Settings.EnableMusicSectionUpdater)
            {

                Debugger.LogMessageToFile
                    ("All media updaters are disabled." +
                     " The general Media Updating setting" +
                     " will also be disabled.");

                MediaImportersAndUpdatersCoreSettings
                    .EnableMediaImporters = false;


            }
            #endregion





            if (Settings.EnableImportAndUpdateFilmsCombo)
            {
                MediaImportersAndUpdatersCoreSettings.EnableMediaImporters = true;
                MediaImportersAndUpdatersCoreSettings.EnableMovieImporter = true;
                MediaImportersAndUpdatersCoreSettings.EnableMediaUpdaters = true;
                Settings.EnableMoviesSectionUpdater = true;
            }

            if (Settings.EnableImportAndUpdateTvSeriesCombo)
            {
                MediaImportersAndUpdatersCoreSettings.EnableMediaImporters = true;
                MediaImportersAndUpdatersCoreSettings.EnableTvSeriesImporter = true;
                MediaImportersAndUpdatersCoreSettings.EnableMediaUpdaters = true;
                Settings.EnableTvShowsSectionUpdater = true;
            }

            if (Settings.EnabeImportAndUpdateMusicCombo)
            {
                MediaImportersAndUpdatersCoreSettings.EnableMediaImporters = true;
                MediaImportersAndUpdatersCoreSettings.EnableMusicImporter = true;
                MediaImportersAndUpdatersCoreSettings.EnableMediaUpdaters = true;
                Settings.EnableMusicSectionUpdater = true;
            }

            if (Settings.EnableImportAndUpdateBooksCombo)
            {
                MediaImportersAndUpdatersCoreSettings.EnableMediaImporters = true;
                MediaImportersAndUpdatersCoreSettings.EnableMediaUpdaters = true;
            }

            // ReSharper disable InvertIf
            if (Settings.EnableImportAndUpdateGamesCombo)
            // ReSharper restore InvertIf
            {
                MediaImportersAndUpdatersCoreSettings.EnableMediaImporters = true;
                MediaImportersAndUpdatersCoreSettings.EnableMediaUpdaters = true;
            }


        }








        public static void FinishUpdatingSections()
        {
            if ((MediaImportersAndUpdatersCoreSettings.EnableMovieImporter
                || Settings.EnableMoviesSectionUpdater)
                && MediaSectionsAllocator.MoviesSection != null)
                MediaSectionsAllocator.MoviesSection.EndUpdate();



            if ((MediaImportersAndUpdatersCoreSettings.EnableTvSeriesImporter
                || Settings.EnableTvShowsSectionUpdater) 
                && MediaSectionsAllocator.TvEpisodesSection != null)
            {
                MediaSectionsAllocator.TvEpisodesSection.EndUpdate();
            }



            if ((MediaImportersAndUpdatersCoreSettings.EnableMusicImporter
                || Settings.EnableMusicSectionUpdater)
                && MediaSectionsAllocator.MusicSection != null)
                MediaSectionsAllocator.MusicSection.EndUpdate();




            if (Settings.EnableRssImporter && MediaSectionsAllocator.NewsSection != null)
                MediaSectionsAllocator.NewsSection.EndUpdate();
        }

        internal static void ConfigureUiStartingState()
        {
            StatusForm.statusForm.GUIPage.Dispose();
            StatusForm.statusForm.PendingPage.Dispose();
            StatusForm.statusForm.SettingsPage.Dispose();
            StatusForm.statusForm.tabControl1.SelectedTab = StatusForm.statusForm.StatusPage;
        }




        internal static void BeginUpdatingSections()
        {


            if ((MediaImportersAndUpdatersCoreSettings.EnableMovieImporter
                || Settings.EnableMoviesSectionUpdater) &&
                MediaSectionsAllocator.MoviesSection != null)
                MediaSectionsAllocator.MoviesSection.BeginUpdate();


            if ((MediaImportersAndUpdatersCoreSettings.EnableTvSeriesImporter 
                || Settings.EnableTvShowsSectionUpdater) &&
                MediaSectionsAllocator.TvEpisodesSection != null)
            {
                MediaSectionsAllocator.TvEpisodesSection.BeginUpdate();
            }


            if ((MediaImportersAndUpdatersCoreSettings.EnableMusicImporter
                || Settings.EnableMusicSectionUpdater) &&
                MediaSectionsAllocator.MusicSection != null)
                MediaSectionsAllocator.MusicSection.BeginUpdate();
        }





        internal static void EndUpdatingSections
            (IMLSection moviesSection, 
             IMLSection tvSection, 
             IMLSection musicSection)
        {


            if (Settings.EnableMoviesSectionUpdater)
                moviesSection.EndUpdate();


            if (Settings.EnableTvShowsSectionUpdater)
            {
                tvSection.EndUpdate();
            }


            if (Settings.EnableMusicSectionUpdater)
                musicSection.EndUpdate();


        }





        internal static int ComputeProgress(int currentItemInt, int totalItemsInt)
        {
            double divider = 0;
            double currentItem = Convert.ToDouble(currentItemInt);
            double totalItems = Convert.ToDouble(totalItemsInt);
            //double Remaining = 100 - Importer.CurrentProgress;
            double currentProgress;

            if (currentItem != 0)
                divider = totalItems / currentItem; ;

            if (currentItem == 0 || totalItems == 0 || divider == 0)
                currentProgress = 0;
            else currentProgress = 100 / divider;

            int currentProgressInt = Convert.ToInt32(currentProgress);
            return currentProgressInt;

        }

        internal static ArrayList GetDirectoryFileList(string directoryStr, int totalFiles, int currentFile,
                                                       DirectoryInfo directory)
        {
            MainImportingEngine.CurrentProgress = ComputeProgress(currentFile, totalFiles);
            
            Debugger.LogMessageToFile("Retrieving the files contained in directory "
                + directoryStr + "...");
            
            Helpers.UpdateProgress("Importing media files...",
                "Getting file list from directory " + directoryStr + "...", null);
            
            
            ArrayList filesInDirectory = new ArrayList();
               filesInDirectory.AddRange(directory.GetFiles("*.*"));



            return filesInDirectory;
        }

        internal static DirectoryInfo TestDirectoryAccess(string dirStr)
        {

            Debugger.LogMessageToFile("Testing access for directory " + dirStr + "...");
            
            Helpers.UpdateProgress("Importing media files...",
                "Testing access for directory " + dirStr + "...",null);
           
            DirectoryInfo dir;
            try
            {
                dir = new DirectoryInfo(dirStr);
            }
            catch (Exception e)
            {
                Debugger.LogMessageToFile("Unable to access directory. The following error occured: " + e);
                return null;
            }


            return dir;
        }

        internal static void RetrieveMediaLocations(Importer importer)
        {


            string importRootFoldersStr = API.Folders(importer.Ibs, importer.MfSettingsMovies) + '|'
                                          + API.Folders(importer.Ibs, importer.MfSettingsTvShows) + '|'
                                          + API.Folders(importer.Ibs, importer.MfSettingsMusic);

            MediaImportersAndUpdatersCoreSettings.RootMediaFolders = importRootFoldersStr.Split(new[] { '|' }, StringSplitOptions.None);

            try
            {


                Settings.FilmsFolders = MediaFolders.API.Folders
                    (importer.Ibs, importer.MfSettingsMovies)
                    .Split(new[] { '|' }, StringSplitOptions.None);
               

                //TODO: Also retrieve and use Music folders!

            }
            catch (Exception e)
            {
                //TODO: This message should be a Balloon Tip.

                // ReSharper disable LocalizableElement
                MessageBox.Show("The media importing engine was unable to retrieve the directories containing your Movies media files. " +
                                "Please make sure that you have provided your media locations" +
                                "for this type of your media files in the plugin's 'Media Folders' settings group and try again.",
                                "Movies Media Folders were not specified", MessageBoxButtons.OK);
                // ReSharper restore LocalizableElement
                          

                Debugger.LogMessageToFile("[Media Importing Engine] Unable to retrieve user's media files locations." +
                                          "The error was: " + Environment.NewLine + e);


            }

            try
            {

                Settings.TvShowsFolders = MediaFolders.API.Folders
                    (importer.Ibs, importer.MfSettingsTvShows)
                    .Split(new[] { '|' }, StringSplitOptions.None);

            }
            catch (Exception e)
            {

                //TODO: This message should be a balloon tip.

                // ReSharper disable LocalizableElement
                MessageBox.Show("The media importing engine was unable to retrieve the directories containing your TV Shows media files. " +
                                "Please make sure that you have provided your media locations" +
                                "for this type of your media files in the plugin's 'Media Folders' settings group and try again.",
                                "TV Shows Media Folders were not specified", MessageBoxButtons.OK);
                // ReSharper restore LocalizableElement


                Debugger.LogMessageToFile("[Media Importing Engine] Unable to retrieve user's media files locations." +
                                          "The error was: " + Environment.NewLine + e);



            }


            try
            {
                Settings.MusicFolders = 
                    API.Folders(importer.Ibs, importer.MfSettingsMusic)
                    .Split(new[] { '|' }, StringSplitOptions.None);

            }
            catch (Exception e)
            {
                //TODO: This message should be a balloon tip.
                // ReSharper disable LocalizableElement
                MessageBox.Show("The media importing engine was unable to retrieve the directories containing your Music media files. " +
                                "Please make sure that you have provided your media locations" +
                                "for this type of your media files in the plugin's 'Media Folders' settings group and try again.",
                                "Music Media Folders were not specified", MessageBoxButtons.OK);
                // ReSharper restore LocalizableElement


                Debugger.LogMessageToFile("[Media Importing Engine] Unable to retrieve user's media files locations." +
                                          "The error was: " + Environment.NewLine + e);



            }



        }
    }
}