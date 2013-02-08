using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using MediaFairy.ImportingEngine;
using MeediOS;
using MetaDataFolders;



namespace MediaFairy.Code.Metadata_Scrapers.Cover_Art
{


    class AmazonMusicAlbumCoverArtDownloader
    {





        internal static bool DownloadMusicAlbumCoverArtFromAmazon
            (IMLItem item, IMLSection musicSection,
             string artist, string Album, 
             bool fileServerIsOnline, bool IsUnc, 
             ref List<string> albumsWithoutcover, 
             ref List<string[]> albumsWithCover, 
             bool trackHasCover,string mdfSettingsc,
             IBaseSystem ibs)
        {
            #region method vars
            
            byte[] aRequestHTML;
            if (String.IsNullOrEmpty(artist) || String.IsNullOrEmpty(Album))
                return false;
            #endregion

            Application.DoEvents();


            #region check if image file in item's location exists
            if (!String.IsNullOrEmpty(item.ImageFile))
            {

                if (fileServerIsOnline || !IsUnc)
                {
                    if (File.Exists(item.ImageFile))
                    {
                        Application.DoEvents();
                        Helpers.EmbedCoverArt(item, item.ImageFile , trackHasCover);
                        return true;
                    }
                }
            }
            #endregion

            Application.DoEvents();

            string ImageFile = Music.CreateAlbumCoversFilePath(item, mdfSettingsc, ".jpg", ibs);

            #region look for album cover in item's location and save to image tag
            if (fileServerIsOnline || !IsUnc)
            {
                if (File.Exists(ImageFile))
                {
                    item.ImageFile = ImageFile;
                    item.SaveTags();
                    Application.DoEvents();
                    Helpers.EmbedCoverArt(item, ImageFile , trackHasCover);
                    return true;

                }
            }
           
            #endregion

            Application.DoEvents();

            #region skip albums for which Amazon doesn't have album cover.
            foreach (string album in albumsWithoutcover)
            {
                //MessageBox.Show(album);
                if (album == Album)
                    return false;
            }
            #endregion


            #region Locate existing album cover and save to item's image tag.
            foreach (string[] album in albumsWithCover)
            {
                if (album[0] == Album)
                {
                    item.ImageFile = album[1];
                    item.SaveTags();
                    Helpers.EmbedCoverArt(item, album[1] , trackHasCover);
                    return true;
                }
            }

            if (Helpers.GetTagValueFromItem(item,"HasAlbumCover") == "false")
                return false;

            #endregion

            Application.DoEvents();
            MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress, "Searching album cover for " + Album + "...");

            #region construct search url
            string ArtistNormalized = artist.Replace(' ', '+');
            string AlbumNormalized = Album.Replace(' ', '+');
            AlbumNormalized = AlbumNormalized.Replace("(disc 1", "");
            AlbumNormalized = AlbumNormalized.Replace("(disc 2", "");
            AlbumNormalized = AlbumNormalized.Replace("(disc 3", "");
            string SearchUrl = @"http://www.amazon.com/gp/search/ref=sr_adv_m_pop/?search-alias=popular&unfiltered=1&field-keywords=&field-artist=" + ArtistNormalized + @"&field-title=" + AlbumNormalized + @"&field-label=&field-binding=&sort=relevancerank&Adv-Srch-Music-Album-Submit.x=41&Adv-Srch-Music-Album-Submit.y=4";
            #endregion

            //MessageBox.Show(SearchUrl);
            string firstItemUrl = Downloaders.MineWebDataReturnFirstMatch(SearchUrl, "e\"><a href=\"(?<Cover>.*?)\">");
            //MessageBox.Show(firstItemUrl);

            #region add to list of albums without cover
            if (String.IsNullOrEmpty(firstItemUrl))
            {
                //Application.DoEvents();
                item.Tags["HasAlbumCover"] = "false";
                item.SaveTags();
                albumsWithoutcover.Add(new string(Album.ToCharArray()));
                return false;
            }
            #endregion

            #region get CoverImage
            string CoverImage = Downloaders.MineWebDataReturnFirstMatch(firstItemUrl, "registerImage.*?original_image\",.\"(?<Image>.*?)\"");
            if (String.IsNullOrEmpty(CoverImage))
            {
                item.Tags["HasAlbumCover"] = "false";
                item.SaveTags();
                albumsWithoutcover.Add(new string(Album.ToCharArray()));

                return false;
            }
            #endregion

            Application.DoEvents();

            bool downloadImage
                = !MediaFairy.Settings.AlwaysPromptForImages 
                  || UI.PreviewImage(CoverImage, false);


            if (downloadImage)
            {
                #region Download Image
                MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress, "Downloading album cover for " + Album + "...");
                aRequestHTML = Downloaders.TrytoDownloadData(CoverImage, "Updating Music Section", "Amazon.com", item );
                Application.DoEvents();
                #endregion

                #region Save Image
                //MessageBox.Show(Importer.AlbumCoverFolder);
                string imagefile = ImageFileConstructor.SaveImage(aRequestHTML, item, true,"",ImageFile);
                if (imagefile != "")
                {
                    Helpers.UpdateProgress("Music", "Album cover was downloaded succefully!",  item);
                    Thread.Sleep(500);
                    item.Tags["HasAlbumCover"] = "true";
                    item.SaveTags();
                    albumsWithCover.Add(new string[2] { Album, imagefile });

                    Helpers.EmbedCoverArt(item, imagefile , trackHasCover);
                    return true;
                }
                else return false;
                #endregion
            }


            return false;
        }
    }
}
