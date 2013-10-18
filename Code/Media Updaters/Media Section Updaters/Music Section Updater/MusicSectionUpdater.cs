using System;
using System.Collections.Generic;
using System.Threading;
using MeediFier.ImportingEngine;
using MeediFier.SingleItemUpdaters;
using MeediOS;

namespace MeediFier.MediaSectionUpdaters
{


    class MusicSectionUpdater
    {
        internal static bool UpdateMusicSection(ref bool fileServerIsChecked,
            ref bool fileServerIsOnline, ConnectionResult connectionresult,
            string mediaDataFoldersSettingsMusic, IBaseSystem iBaseSystem)
        {


            Debugger.LogMessageToFile("Checking for valid Music Section...");

            if (String.IsNullOrEmpty(Settings.MusicLibrary))
                Settings.EnableMusicSectionUpdater = false;

            if (!Settings.EnableMusicSectionUpdater)
                return true;

            if (MediaSectionsAllocator.MusicSection.ItemCount == 0)
                return true;

            #region Music Section varibales

            Debugger.LogMessageToFile("Creating album cover lists...");
            var albumsWithoutCoverArt = new List<string>();
            var albumsWithCoverArt = new List<string[]>();

            int allTracksToIdentifyCount = 0;
            int tracksRemainingToIdentifyCount = 0;

            MainImportingEngine.CurrentProgress = 0;
            MainImportingEngine.GeneralStatus = "Updating Music Section";
            MainImportingEngine.SpecialStatus = "";
            //Debugger.LogMessageToFile("Creating iTunes instance...");
            //IiTunes iTunesApp;
            if (Settings.SleepValue > 3000) Settings.SleepValue = 3000;

            #endregion

            int musicDnsIdentificationTime = 0;
            //    MusicDnsIdentificationTimeCalculator.CalculateMusicDnsIdentificationTime
            //        (connectionresult, out tracksRemainingToIdentifyCount,
            //         ref allTracksToIdentifyCount);

            #region Synchronize with ITunes

            const bool connectedToiTunes = false;

            //if (Settings.WantItunesSync)
            //{
            //    Debugger.LogMessageToFile("Launching iTunes...");

            //    #region Launch itunes

            //    try
            //    {
            //        //Importer.CurrentProgress++;
            //        MainImportingEngine.ThisProgress.Progress
            //            (MainImportingEngine.CurrentProgress, 
            //            "Launching iTunes...");

            //        connectedToiTunes = true;
            //    }
            //    catch (Exception)
            //    {
            //        StatusForm.statusForm.TrayIcon.ShowBalloonTip(10000, "Unable to connect to iTunes",
            //        "MediaFairy was unable to detect a valid iTunes installation. " +
            //        "If you don't have iTunes installed, please disable the iTunes synchronization feature.",
            //        ToolTipIcon.Warning);
            //        connectedToiTunes = false;
            //    }
            //} 

            #endregion

            if (!PerformMusicSectionUpdate(ref fileServerIsChecked,
                ref fileServerIsOnline, connectionresult,
                mediaDataFoldersSettingsMusic, iBaseSystem,
                albumsWithCoverArt, connectedToiTunes,
                tracksRemainingToIdentifyCount, albumsWithoutCoverArt,
                allTracksToIdentifyCount, musicDnsIdentificationTime))
                return false;


            LibraryCleaners.CleanupSection(Settings.CleanEmptyFields, Settings.CleanSelectedFields,
                                           Settings.SelectedFieldsToClean, MediaSectionsAllocator.MusicSection);


            MeePopupProvider.ImportMeePopupProviderField(MediaSectionsAllocator.MusicSection);


            //if (Settings.EnableXmlBackupMusic)
            //    MediaLibrarySynchronizerBackup
            //        .LibrarySynchronizerBackupMediaSection
            //        (MediaSectionsAllocator.MusicSection,
            //         Settings.XmlBackupsDirectory);



            return true;
    
        }



        private static bool PerformMusicSectionUpdate(ref bool fileServerIsChecked, ref bool fileServerIsOnline,
                            ConnectionResult connectionresult, string mediaDataFoldersSettingsMusic,
                            IBaseSystem iBaseSystem, List<string[]> albumsWithCoverArt, bool connectedToiTunes,
                            int tracksRemainingToIdentifyCount, List<string> albumsWithoutCoverArt,
                            int allTracksToIdentifyCount, int musicDnsIdentificationTime)
        {


            MediaSectionsAllocator.MusicSection.BeginUpdate();
            
            MainImportingEngine.ThisProgress.Progress
                (0, "Preparing to update Music Section...");
            
            Thread.Sleep(1000);

            #region Section variables

            Debugger.LogMessageToFile("Creating section variables...");
            int currentItem = 1;
            int[] allItems = MediaSectionsAllocator.MusicSection.GetAllItemIDs();
            int totalItemCount = allItems.Length;

            #endregion

            MediaSectionsAllocator.MusicSection.BeginUpdate();

            foreach (int id in allItems)
            {
                if (!MusicTrackItemUpdater.UpdateMusicTrackItem(ref fileServerIsChecked, ref fileServerIsOnline,
                                                                connectionresult, mediaDataFoldersSettingsMusic, iBaseSystem,
                                                                allTracksToIdentifyCount,
                                                                connectedToiTunes, totalItemCount, id, ref albumsWithCoverArt,
                                                                ref albumsWithoutCoverArt, ref tracksRemainingToIdentifyCount,
                                                                ref musicDnsIdentificationTime, ref currentItem))
                    return false;
            }


            MediaSectionsAllocator.MusicSection.EndUpdate();

            return true;
        
        
        }
    
    
    
    }




}
