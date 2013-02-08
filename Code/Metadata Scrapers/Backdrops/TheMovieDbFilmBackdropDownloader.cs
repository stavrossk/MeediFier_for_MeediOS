using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using MediaFairy.Code.User_Interface;
using MeediOS;
using MetaDataFolders;





namespace MediaFairy.Code.Metadata_Scrapers.Cover_Art
{




    class TheMovieDbFilmBackdropDownloader
    {





        internal static bool DownloadBackdropFromTheMovieDb
            (IMLItem item, string videofilename, 
             string location, string itemTitle,
             string parent, bool fileServerIsOnline,
             bool isUNC,string mdfSettingsa, 
             IBaseSystem ibs)
        {


            try
            {


                #region return cases



                if (MediaFairy.Settings.WantToDisableSources
                    && !MediaFairy.Settings.TMDbIsOnline)
                    return false;

                if (!MediaFairy.Settings.FilmBackdropArtDownloaderIsEnabled)
                    return false;

                if (isUNC && !fileServerIsOnline)
                    return false;
                #endregion



                #region set item variables
               


                string imdbid = Helpers
                    .GetTagValueFromItem(item, "ImdbID");



                if (imdbid.Length == 8 && imdbid.StartsWith("tt"))
                {
                    imdbid = imdbid.Remove(0,2);
                    imdbid = "0" + imdbid;
                    imdbid = "tt" + imdbid;
                    item.Tags["ImdbID"] = imdbid;
                    item.SaveTags();
                }


                string fanart 
                    = Helpers.GetTagValueFromItem
                    (item, "fanart");

                string lastUpdated
                    = Helpers.GetTagValueFromItem
                    (item, "LastUpdated");



                #endregion


                #region check if image file in item's location exists
                if (!String.IsNullOrEmpty(fanart))
                {
                    if (File.Exists(fanart))
                        return true;
                }

                #endregion


                Application.DoEvents();


                const string ext = ".jpg";

                string imageFile = Movies.CreateFanartFilePath
                    (item, mdfSettingsa, ext, ibs);


                #region look for backdrop in item's location and save to image tag


                if (File.Exists(imageFile))
                {
                    item.Tags["fanart"] = imageFile;
                    item.SaveTags();
                    return true;

                }

                #endregion

                #region Check Last Updated date
                if (!String.IsNullOrEmpty(lastUpdated))
                {

                    DateTime todayDate = DateTime.Today;


                    DateTime updatedDate 
                        = DateTime.Parse
                        (Helpers.GetTagValueFromItem
                        (item, "LastUpdated"));


                    if (todayDate.DayOfYear
                        < (updatedDate.DayOfYear + 2))
                        return false;


                }
                #endregion



                var previewImages = new List<string>();

                List<string> imageUrls
                    = TheMovieDb.GetFilmBackdropsFromTMDb
                    (item, ref previewImages);

                if (imageUrls.Count == 0 || previewImages.Count == 0)
                {
                    Debugger.LogMessageToFile("The image URLs list is empty. No online backdrops were found for this film.");
                    return false;
                }

                string posterUrl
                    = ImageSelectionEngine.UserSelectsImage
                    (item, imageUrls, 
                    previewImages,"backdrop");


                #region Download, save and import backdrop

                #region download the image

                Helpers.UpdateProgress
                    ("Updating Movies Section",
                    "Downloading backdrop " +
                    "from themoviedb.org...", item);



                byte[] imageData
                    = Downloaders
                    .TrytoDownloadData
                    (posterUrl, "Movies",
                    "TheMovieDatabase", item);



                if (imageData == null)
                    return false;




                #endregion

                #region save the image & update tags

                if ( String.IsNullOrEmpty
                    (ImageFileConstructor.SaveImage
                    (imageData, item, false,
                    "fanart", imageFile)))
                    return false;


                if (MediaFairy.Settings
                    .ResizeFilmBackdrops)
                {


                    Helpers.UpdateProgress
                        ("Updating Movies Section",
                        "Resizing image...", item);


                    ImageResizer.ResizeImage
                        (imageFile, imageFile, 
                         MediaFairy.Settings.FilmBackdropWidth,
                         MediaFairy.Settings.FilmBackdropHeight, false);


                }
                return true;

                #endregion


                #endregion

            }
            catch (Exception e)
            {
                Debugger.LogMessageToFile
                    ("The backdrops downloading method" +
                     " returned an unexpected error: " + e);

                return false;
            }


        }








    }




}
