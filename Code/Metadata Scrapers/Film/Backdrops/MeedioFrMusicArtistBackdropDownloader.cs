using System;
using System.IO;
using System.Net;
using System.Windows.Forms;
using MeediOS;
using MetaDataFolders;



namespace MediaFairy.Code.Metadata_Scrapers.Backdrops
{




    class MeedioFrMusicArtistBackdropDownloader
    {









        internal static void DownloadMusicArtistBackdropFromMeedioFr
            (IMLItem Item,string MDFSettingsc,
             IBaseSystem IBS,
             ConnectionResult connectionresult)
        {

            #region Return if disabled
            if (!MediaFairy.Settings.ArtistBackdropsEnabled)
                return;
            Debugger.LogMessageToFile
                ("[Artist fanart downloader]" +
                 " Artist fanart downloader is enabled");
            #endregion

            if (!connectionresult.InternetConnectionAvailable && MediaFairy.Settings.ConnectionDiagnosticsEnabled)
                return;

            #region Retrieve Artist
            string Artist = Helpers.GetTagValueFromItem(Item, "Artist");
            if (String.IsNullOrEmpty(Artist))
                return;
            Debugger.LogMessageToFile("[Artist fanart downloader] Artist: " + Artist);
            #endregion


            string Fanart = Helpers.GetTagValueFromItem(Item, "fanart");
            if (String.IsNullOrEmpty(Fanart))
            {
                Debugger.LogMessageToFile("[Artist fanart downloader] Library tag 'fanart' is empty for this item. Proceeding to download image...");
            }
            else
            {
                Debugger.LogMessageToFile("[Artist fanart downloader] Library tag 'fanart' was found populated for this item. Checking if image file exists...");
                if (File.Exists(Fanart))
                {
                    Debugger.LogMessageToFile("[Artist fanart downloader] Image file exists! Continuing to next item");
                    return;
                }
                else
                {
                    Debugger.LogMessageToFile("[Artist fanart downloader] Image file found in tag 'fanart' does not exist. Proceeding to download image...");
                    Item.Tags["fanart"] = "";
                    Item.SaveTags();
                    Fanart = "";
                }
            }

            Debugger.LogMessageToFile("[Artist fanart downloader] Artist fanart retrieved from library tag: " + Fanart);

            //MessageBox.Show("Step 3");

            #region Normalize Artist name
            string ArtistNorm = Artist.ToLower();
            
            if (ArtistNorm.StartsWith("the"))
                ArtistNorm = ArtistNorm.Remove(0,4);

            ArtistNorm = ArtistNorm.Replace(" ", "%20");
            //MessageBox.Show(ArtistNorm);
            #endregion

            string ImageURL = "http://www.meediofr.org/~dmdocuments/music/artist_backdrops/" + ArtistNorm + ".jpg";

            //MessageBox.Show("Step 5");

            string ImageFile = Music.CreateArtistFanartFilePath(Item, MDFSettingsc, ".jpg", IBS);
            Debugger.LogMessageToFile("[Artist fanart downloader] Fanart file path created by MetadataFolders: " + ImageFile);


            if (File.Exists(ImageFile))
            {
                Debugger.LogMessageToFile("[Artist fanart downloader] Fanart image already exists. Will now save it's path to library tag 'fanart'");
                Item.Tags["fanart"] = ImageFile;
                Item.SaveTags();
                return;
            }
            else
            {
                Debugger.LogMessageToFile("[Artist fanart downloader] Fanart image does not exist for this Artist. Proceeding to download image...");
            }

            //MessageBox.Show("Step 6");

            Helpers.UpdateProgress("Updating Music section...", "Searching meediofr.org for fanart image for artist: " + Artist + "...", Item);
            Debugger.LogMessageToFile("[Artist fanart downloader] Searching meediofr.org for fanart image for artist: " + Artist + "...");
            if (!Helpers.RemoteFileExists(ImageURL))
            {
                Debugger.LogMessageToFile("[Artist fanart downloader] The remote image path constructed was not found on the server. Continuing to next item...");
                return;
            }

            WebClient client = new WebClient();
            try
            {
                Helpers.UpdateProgress("Updating Music Section...", "Downloading fanart image for Artist '" + Artist + "'...", Item);
                Debugger.LogMessageToFile("[Artist fanart downloader] Downloading fanart image for Artist '" + Artist + "'...");
                client.DownloadFile(ImageURL, ImageFile);
                Item.Tags["fanart"] = ImageFile;
                Item.SaveTags();
            }
            catch (Exception e)
            {
                StatusForm.statusForm.TrayIcon.ShowBalloonTip(10000, "Error downloading image", "MediaFairy encountered an error while trying to download an artist fanart image. Please see Debug.log for details on this error.", ToolTipIcon.Warning);
                Debugger.LogMessageToFile("[Artist fanart downloader] An unexpected error occured while trying to download an artist fanart image. The error was: " + e.ToString() );
            }



        }
    }
}
