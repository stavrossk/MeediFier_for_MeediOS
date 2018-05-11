using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MeediFier.MediaSnapshotEngine;
using MeediFier.RssImporter;
using MeediOS;


namespace MeediFier.ImportingEngine
{

    internal class MainImportingEngine
    {


        internal static void StartMainImportingEngine
            (Importer importer, IMLSection section,
            IEnumerable<string> combinedSceneTags)
        {

            Debugger.LogMessageToFile("----------------------   START OF MEDIA IMPORTERS   ----------------");

            #region Media Importers

            GeneralStatus = "Performing media importing...";
            SpecialStatus = String.Empty;

            //LibrarySynchronizerRestore
            //    .LibrarySynchronizerRestoreFromBackup
            //    (Settings.XmlBackupsDirectory);

            MediaImportingEngine.ImportMediaFilesMAIN(combinedSceneTags, importer);

            //var connectionresult = MediaImportingEngineHelpers.InitConnectionresult();
            var connectionresult = Helpers.CheckInternetConnectionAndConnectToOSDb();

            RssNewsImporter.ImportRssFeeds(connectionresult);

            #endregion

            Debugger.LogMessageToFile("----------------------   END OF MEDIA IMPORTERS   ----------------");

            #region Media Updaters

            if (MediaImportingEngineHelpers.RunMediaSectionsUpdatingEngine(importer, section,
                combinedSceneTags, connectionresult)) 
                return;

            #endregion

            //TODO: Bring back the previously removed Import Sequencer.
            //#region Import Secquencer
            //ImportSequencer.RunImports();
            //#endregion



        }


        internal static bool DoImport
            (Importer importer, IMLSection section, 
            IMLImportProgress progress)
        {
            try
            {
                ThisProgress = progress;

                ImportingEngineHelpers.ConfigureUiStartingState();

                Settings.ImportingStarted = false;
                Settings.ImportingCompleted = false;


                #region Open UI
                //StatusForm.StatusForm.statusForm.TrayIcon.ShowBalloonTip(1000, "MediaFairy is working", "Double clck this tray icon to show or hide the importing status window. ", ToolTipIcon.Info);

                //MessageBox.Show(Convert.ToString(UIenabled));

                //if (UIenabled)
                //{
                //    StatusForm.StatusForm.statusForm.Show();

                //    while (!ImportingStarted)
                //    {
                //        Thread.Sleep(20);
                //        Application.DoEvents();

                //    }

                //}
                #endregion


                ImportingEngineHelpers
                    .RetrieveMediaLocations
                    (importer);


                if (ImportingEngineHelpers
                    .PerformInitializationTasks
                    (section))
                    return true;


                IEnumerable<string> combinedSceneTags
                    = VideoFilenameCleaner
                    .ReadDictionaryReturnAllTags();


                StartMainImportingEngine
                    (importer, section, 
                    combinedSceneTags);


                ImportingEngineHelpers
                    .FinishImport();




                return true;

            }
            catch (Exception e)
            {

                StatusForm.statusForm.TrayIcon.ShowBalloonTip(10000, "Import error",
                                                              "An unexpected error ocurred in the main importing process. Please refer to Debug.log for details.",
                                                              ToolTipIcon.Error);

                Debugger.LogMessageToFile(
                    "An unexpected error ocurred in the main import method DoImport(). The error was: " + e);
            }


            return true;
        }


        internal static bool Initialized;
        internal static IMLImportProgress ThisProgress;
        internal static int CurrentProgress;
        internal static string GeneralStatus = String.Empty;
        internal static string SpecialStatus = String.Empty;
        internal static readonly MediaLibraryClass Library = new MediaLibraryClass();


    }

}
