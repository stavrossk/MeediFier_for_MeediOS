using System;
using System.Threading;
using System.Windows.Forms;
using MediaFairy.ImportingEngine;
using MeediOS;



namespace MediaFairy.MediaSectionUpdaters
{


    class MusicDnsIdentificationTimeCalculator
    {


        internal static int CalculateMusicDnsIdentificationTime(ConnectionResult connectionresult,
                            out int tracksToIdentify, ref int totalTracksToIdentifyCount)
        {

            int remainingTime = 0;
            tracksToIdentify = 0;

            if (!Settings.EnableMusicDnsAudioTrackIdentification)
                return remainingTime;

            if (
                !Settings.EstimateAudioIdentification ||
                (
                    !connectionresult.InternetConnectionAvailable && 
                    Settings.ConnectionDiagnosticsEnabled
                )
                )
                return remainingTime;


            Debugger.LogMessageToFile("Calculating audio identification time...");
            MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress,
                                                      "Calculating audio identification time...");
            Thread.Sleep(500);


            CountItemsWhichNeedMusicDnsIdentification
                (ref totalTracksToIdentifyCount);


            remainingTime = totalTracksToIdentifyCount*10;
            tracksToIdentify = totalTracksToIdentifyCount;


            if (totalTracksToIdentifyCount > 12)
            {
                StatusForm.statusForm.TrayIcon.ShowBalloonTip(5000,
                    "Music Track Identification has started.",
                    "This operation will need approximately " +
                    remainingTime/60 + " minutes to complete. ",
                    ToolTipIcon.Info);
            }

            return remainingTime;
        
        }



        internal static void CountItemsWhichNeedMusicDnsIdentification
            (ref int totalTracksToIdentifyCount)
        {

            int[] allItemsIDs = MediaSectionsAllocator.MusicSection.GetAllItemIDs();

            foreach (int id in allItemsIDs)
            {
                IncrementTotalTracksToIdentifyCount(ref totalTracksToIdentifyCount, id);
            }


        }

        internal static void IncrementTotalTracksToIdentifyCount
            (ref int totalTracksToIdentifyCount, int id)
        {
            Application.DoEvents();

            IMLItem item = MediaSectionsAllocator.MusicSection.FindItemByID(id);

            string puid = Helpers.GetTagValueFromItem(item, "PUID");
            string artist = Helpers.GetTagValueFromItem(item, "Artist");
            string title = Helpers.GetTagValueFromItem(item, "Title");


            if (Settings.MusicDnsOnlyIdentifyFilesWithMissingKeyMetadata)
            {
                if (String.IsNullOrEmpty(puid))
                {
                    if (String.IsNullOrEmpty(artist) || String.IsNullOrEmpty(title))
                        totalTracksToIdentifyCount++;
                }
            }
            else
            {
                if (String.IsNullOrEmpty(puid))
                    totalTracksToIdentifyCount++;
            }

        
        }
    }


}
