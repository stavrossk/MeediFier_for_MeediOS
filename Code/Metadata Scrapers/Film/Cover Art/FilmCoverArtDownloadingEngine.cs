using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using MeediFier.Code.Metadata_Scrapers.Film.Cover_Art;
using MeediFier.Code.User_Interface;
using MeediOS;
using MetaDataFolders;


namespace MeediFier.Code.Metadata_Scrapers.Cover_Art
{




    class FilmCoverArtDownloadingEngine
    {






        internal static bool SearchForAndDownloadFilmCoverArtFromMultipleSources
            (IMLItem item, string location,
             string videoFilename, 
             string mdfSettingsa,
             IBaseSystem ibs,
             string imageFile)
        {


            #region initialize variables


            string movieName = Helpers.GetTagValueFromItem(item, "Title");
            string releaseYear = Helpers.GetTagValueFromItem(item, "Year");
            #endregion

            string title = Helpers.GetTagValueFromItem(item, "Title");


            var imageUrls = new List<string>();
            var previewImages = new List<string>();
            //PreviewImages = ImageUrls;





            //TODO: Skip searching secondary sources if cover art is found from the first source.

            #region Add the image URLs from all sources



            if (MeediFier.Settings.PosterPrimarySource == "TheMovieDatabase")
            {

                if (MeediFier.Settings.TMDbPostersEnabled)
                {

                    if (imageUrls.Count == 0 || MeediFier.Settings.AlwaysPromptForImages)
                    {

                        //imageUrls.AddRange(TMDbFilmCoverArtScraper.GetFilmPostersFromTMDb(item, ref previewImages));
                        
                        //tmp = "";
                        //tmp = GetFilmPosterFromTMDb(Item);
                        //if (!String.IsNullOrEmpty(tmp))
                        //    ImageUrls.Add(tmp);
                    
                    }
                
                }


                if (MeediFier.Settings.ImpAwardsPostersEnabled)
                {
                    if (imageUrls.Count == 0 || MeediFier.Settings.AlwaysPromptForImages)
                    {
                        List<string> tmpList = ImpAwardsFilmCoverArtDownloader.GetPoster(item, movieName, releaseYear, "Both");
                        imageUrls.AddRange(tmpList);
                        previewImages.AddRange(tmpList);
                    }
                }

            }
            else
            {


                if (MeediFier.Settings.ImpAwardsPostersEnabled)
                {
                    if (imageUrls.Count == 0 || MeediFier.Settings.AlwaysPromptForImages)
                    {
                        List<string> tmpList = ImpAwardsFilmCoverArtDownloader.GetPoster(item, movieName, releaseYear, "Both");
                        imageUrls.AddRange(tmpList);
                        previewImages.AddRange(tmpList);
                    }
                }

                if (MeediFier.Settings.TMDbPostersEnabled)
                {
                    if (imageUrls.Count == 0 || MeediFier.Settings.AlwaysPromptForImages)
                    {

                        //imageUrls.AddRange(TMDbFilmCoverArtScraper.GetFilmPostersFromTMDb(item, ref previewImages));
                        //tmp = "";
                        //tmp = GetFilmPosterFromTMDb(Item);
                        //if (!String.IsNullOrEmpty(tmp))
                        //    ImageUrls.Add(tmp);
                    }
                }

            }

            if (MeediFier.Settings.IMDbPostersEnabled)
            {
                if (imageUrls.Count == 0 || MeediFier.Settings.AlwaysPromptForImages)
                {
                    string tmp = IMDbCoverArtScraper.GetFilmPosterFromIMDb
                        (item);


                    if (!String.IsNullOrEmpty(tmp))
                    {
                        imageUrls.Add(tmp);
                        previewImages.Add(tmp);
                    }
                }
            }

            #endregion



            if (imageUrls.Count == 0 || previewImages.Count == 0)
            {
                Debugger.LogMessageToFile("The image URLs list is empty. No online posters were found for this film.");
                return false;
            }



            string posterUrl 
                = ImageSelectionEngine
                .UserSelectsImage
                (item, imageUrls,
                previewImages,
                "poster");



            #region Download Image

            Helpers.UpdateProgress
                ("Updating Films Section...", 
                "Downloading film poster for "
                + title + "...", item);


            byte[] imageData = MeediFier.Downloaders
                .TrytoDownloadData(posterUrl, "Movies", "", item);



            if (imageData == null)
                return false;

            #endregion



            #region Save Image
            //if (SaveImage(imageData, Item, Importer.PosterNamingMethod, Importer.WhereToSavePoster, Importer.PosterFolder, true, VideoFilename, filepath, "", "") != "")
            if (ImageFileConstructor.SaveImage(imageData, item,true,"",imageFile) != "")
            {
                if (MeediFier.Settings.ResizeFilmPosters)
                {

                    Helpers.UpdateProgress
                        ("Updating Movies Section", 
                         "Resizing image...", item);
                    
                    ImageResizer.ResizeImage
                        (imageFile, imageFile, MeediFier.Settings.FilmPosterWidth, 
                         MeediFier.Settings.FilmPosterHeight, false);
                
                }

                item.SaveTags();
                return true;
            }


            return false;

            #endregion


        }


        internal static bool LocateOrDownloadFimCoverArt
            (IMLItem item, string location, string itemTitle,
             string videoFilename, string parentDirectory,
             bool fileServerIsOnline, bool isUNC,
             string metadataFoldersSettingsA, IBaseSystem ibs)
        {



            Application.DoEvents();

            if (!MeediFier.Settings.FilmCoverArtDownloaderIsEnabled)
                return false;

            if (isUNC && !fileServerIsOnline)
                return false;



            Application.DoEvents();

            if (ImageFileExistsInItemLocatiohn
                (item, fileServerIsOnline, isUNC))
                return true;

            Application.DoEvents();


            var imageFile = ConstructMoviePosterFilePath
                (item, metadataFoldersSettingsA, ibs);

            if (String.IsNullOrEmpty(imageFile))
                return false;

            Application.DoEvents();


           

            if (LookForPosterInItemLocation
                (item, fileServerIsOnline, isUNC, imageFile))
                return true;


            return SearchForAndDownloadFilmCoverArtFromMultipleSources
                (item, location, videoFilename,
                 metadataFoldersSettingsA, ibs, imageFile);


        }

        private static bool LookForPosterInItemLocation(IMLItem item, bool fileServerIsOnline, bool isUNC, string imageFile)
        {

            if (!fileServerIsOnline && isUNC)
                return false;

            if (!File.Exists(imageFile))
                return false;

            item.ImageFile = imageFile;
            item.SaveTags();

            return true;
        }

        private static string ConstructMoviePosterFilePath(IMLItem item, string metadataFoldersSettingsA, IBaseSystem ibs)
        {
            const string ext = ".jpg";
            string imageFile;

            try
            {
                imageFile = Movies.CreatePosterFilePath(item, metadataFoldersSettingsA, ext, ibs);
            }
            catch (Exception)
            {
                Debugger.LogMessageToFile("[Media Image Downloader] " +
                                          "The Media Image Downloader was unable to construct the file path" +
                                          " for a movie poster image. Movie Poster Metadata Folders were not set.");
                return String.Empty;
            }

            return imageFile;
        }

        private static bool ImageFileExistsInItemLocatiohn
            (IMLItem item, bool fileServerIsOnline, bool isUNC)
        {

            if (String.IsNullOrEmpty(item.ImageFile))
                return false;


            if (!fileServerIsOnline && isUNC)
                return false;


            if (!File.Exists(item.ImageFile))
                return false;

            item.SaveTags();

            return true;
        }
    }




}
