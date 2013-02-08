using System;
using System.Threading;
using System.Windows.Forms;
using MediaFairy.ImportingEngine;
using TVDBLibrary;

namespace MediaFairy.MediaSectionUpdaters
{


    class TvSectionUpdaterHelpers
    {




        internal static TVDBLib ConnectToTVDb(ConnectionResult connectionResult)
        {

            if (!connectionResult.InternetConnectionAvailable && Settings.ConnectionDiagnosticsEnabled)
            {

                Helpers.UpdateProgress("Updating TV sections...",
                       "Internet connection was not detected. " +
                       "MediaFairy will not update TV Series libraries.",
                       null);

                Debugger.LogMessageToFile("Internet connection was not detected." +
                                          " MediaFairy will not update TV Series libraries.");
              

                return null;

            }


            Helpers.UpdateProgress("Updating TV Shows Section...", "Connecting to TVDb..", null);
            Debugger.LogMessageToFile("Connecting to TVDb...");


            TVDBLib tvdb;

            try
            {
                tvdb = new TVDBLib();

                Helpers.UpdateProgress("Updating TV Shows Section...", "Connection to TVDb was successful.", null);
                Debugger.LogMessageToFile("Connection to TVDb was succesful!");
            
            }
            catch (Exception)
            {
                try
                {

                    tvdb = new TVDBLib();

                    Helpers.UpdateProgress("Updating TV Shows Section...", "Connection to TVDb was successful.", null);
                    Debugger.LogMessageToFile("Connection to TVDb was succesful!");
                
                }
                catch (Exception)
                {

                    StatusForm.statusForm.TrayIcon.ShowBalloonTip(2000, "Communication with TVdb failed",
                    "MediaFairy was unable to connect to the TVdb database. " +
                    "Please check your internet connection availability," +
                    " otherwise the online database may be temporarily offline or unreachable.",
                    ToolTipIcon.Warning);

                    Debugger.LogMessageToFile("Unable to connect to TVDb. " +
                                              "MediaFairy will not update TV Series libraries.");
                    
                    Thread.Sleep(2000);

                    return null;
                
                }
            
            }

            return tvdb;


        }



        internal static void CheckIfTvShowsSectionIsSet()
        {

            if (!String.IsNullOrEmpty(Settings.TvShowsLibrary))
                return;
            
            Debugger.LogMessageToFile("TV Shows Section was not specified by the user." +
                                      " The TV Shows Section will not be updated.");
            
            Settings.EnableTvShowsSectionUpdater = false;
        
        }

        internal static void PerformPostUpdatingOperations()
        {

            LibraryCleaners.CleanupSection(Settings.CleanEmptyFields, Settings.CleanSelectedFields,
                                           Settings.SelectedFieldsToClean, MediaSectionsAllocator.TvEpisodesSection);


            MeePopupProvider.ImportMeePopupProviderField
                (MediaSectionsAllocator.TvEpisodesSection);


            //BackupTvSections();


        }

        internal static void EndUpdatingSections()
        {

            MediaSectionsAllocator.TvEpisodesSection.EndUpdate();
        }




        internal static void BeginUpdatingSections()
        {

            MediaSectionsAllocator.TvEpisodesSection.BeginUpdate();
      

            MainImportingEngine.ThisProgress.Progress
                (0, "Preparing to update TV Series Section...");
            
            Thread.Sleep(2000);
        
        }







    }


}
