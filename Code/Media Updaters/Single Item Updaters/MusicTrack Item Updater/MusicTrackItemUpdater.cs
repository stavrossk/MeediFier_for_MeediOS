using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MediaFairy.ImportingEngine;
using MeediOS;


namespace MediaFairy.SingleItemUpdaters
{




    class MusicTrackItemUpdater
    {
        internal static bool UpdateMusicTrackItem(ref bool fileServerChecked,
            ref bool fileServerIsOnline, ConnectionResult connectionresult,
            string mdfSettingsc, IBaseSystem iBaseSystem, int allTracksToIdentify,
            bool connectedToiTunes, int totalItems, int id, 
            ref List<string[]> albumsWithCover,
            ref List<string> albumsWithoutcover,
            ref int tracksToIdentify, 
            ref int remainingTime, 
            ref int currentItem)
        {


            #region item varibales
            Debugger.LogMessageToFile("Retrieving item's key tags...");
            IMLItem item = MediaSectionsAllocator.MusicSection.FindItemByID(id);
            string artist = Helpers.GetTagValueFromItem(item, "Artist");
            string album = Helpers.GetTagValueFromItem(item, "Album");
            album = MusicTrackItemUpdaterHelpers.NormalizeAlbumName(item, album);
            string title = Helpers.GetTagValueFromItem(item, "Title");
            string location = item.Location;
            bool isUNC = false;

            int i = location.LastIndexOf('\\');
            string parent = location.Substring(0, i);
            parent = parent + "\\";
            i = parent.LastIndexOf('\\');
            string root = parent.Substring(0, i);
            root = root + "\\";

            string parentDirectory = parent;

            #endregion

            MainImportingEngine.CurrentProgress =
                ImportingEngineHelpers.ComputeProgress(currentItem, totalItems);

 

            #region validate filename

            //try
            //{
               
            //    Debugger.LogMessageToFile("Validating filename...");
            //    Helpers.CheckFileServer(Settings.WantFileserverDiagnostics, location, ref isUNC,
            //            ref fileServerChecked, root, ref fileServerIsOnline);
            //}
            //catch (Exception)
            //{
            //    return true;
            //}

            #endregion


            if (MissingItemRemover.DeleteMissingItem(Settings.DeleteMissing,
               isUNC, fileServerIsOnline, location,
               MediaSectionsAllocator.MusicSection, item))
               return true;


            try
            {

                var audioDuration = MusicTrackItemUpdaterHelpers.ExtractAudioDuration(item);

                if (Helpers.UserCancels(MainImportingEngine.SpecialStatus, item))
                    return false;
      
                if (!MusicTrackItemUpdaterHelpers.PerformAudioIdentificationAndDownloadAlbumCoverArt
                      (fileServerIsOnline, connectionresult, mdfSettingsc, 
                      iBaseSystem, allTracksToIdentify, ref albumsWithCover, 
                      ref albumsWithoutcover, ref tracksToIdentify,
                      ref remainingTime, parentDirectory, isUNC, location, 
                      audioDuration, item, ref artist, ref title, ref album)) 
                      return false;
      

                if (Helpers.UserCancels
                    (MainImportingEngine.SpecialStatus, item))
                    return false;


                MusicTrackItemUpdaterHelpers
                    .OrganizeAudioFile
                    (artist, album, title, item);


                if (Helpers.UserCancels
                    (MainImportingEngine.SpecialStatus, item))
                    return false;


                //SongLyricsDonwloader
                //    .DownloadSongLyrics
                //    (item, connectionresult);


                //if (Helpers.UserCancels
                //    (MainImportingEngine.SpecialStatus, item))
                //    return false;

                //MeedioFrMusicArtistBackdropDownloader
                //    .DownloadMusicArtistBackdropFromMeedioFr
                //    (item, mdfSettingsc,
                //    iBaseSystem, connectionresult);



                //if (Helpers.UserCancels
                //    (MainImportingEngine.SpecialStatus, item))
                //    return false;






                #region update iTunes library

                //if (Settings.WantItunesSync)
                //{
                //    if (connectedToiTunes)
                //    {

                //        IiTunes iTunesApp = new iTunesAppClass();
                //        IITLibraryPlaylist playlist = iTunesApp.LibraryPlaylist;

                //        MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress,
                //                                                    "Adding audio track " + item.Name +
                //                                                    " to iTunes library...");


                //        try
                //        {
                //            playlist.AddFile(item.Location);
                //        }
                //        catch (Exception e)
                //        {
                //            MessageBox.Show(e.ToString());
                //        }
                //    }
                //}

                #endregion





                item.SaveTags();
            
            }
            catch (Exception e)
            {
                Debugger.LogMessageToFile(
                "An uncexpected error occurred in the Music Updater method. The error was: " + e);
                StatusForm.statusForm.TrayIcon.ShowBalloonTip(10000, "Error updating Music Section",
                "An uncexpected error occurred in the Music Updater method." +
                Environment.NewLine + "Please see Debug.log for details.",
                ToolTipIcon.Error);

                return true;
            }

            currentItem++;
        
            
            
            
            
            return true;
    
        
        }
    
    
    
    }



}
