﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using MediaFairy.Code.User_Interface;
using MeediOS;
using MetaDataFolders;


namespace MediaFairy.Code.Metadata_Scrapers.Cover_Art
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



            if (MediaFairy.Settings.PosterPrimarySource == "TheMovieDatabase")
            {
                if (MediaFairy.Settings.TMDbPostersEnabled)
                {
                    if (imageUrls.Count == 0 || MediaFairy.Settings.AlwaysPromptForImages)
                    {
                        imageUrls.AddRange(TheMovieDb.GetFilmPostersFromTMDb(item, ref previewImages));
                        //tmp = "";
                        //tmp = GetFilmPosterFromTMDb(Item);
                        //if (!String.IsNullOrEmpty(tmp))
                        //    ImageUrls.Add(tmp);
                    }
                }

                if (MediaFairy.Settings.ImpAwardsPostersEnabled)
                {
                    if (imageUrls.Count == 0 || MediaFairy.Settings.AlwaysPromptForImages)
                    {
                        List<string> tmpList = ImpAwards.GetPoster(item, movieName, releaseYear, "Both");
                        imageUrls.AddRange(tmpList);
                        previewImages.AddRange(tmpList);
                    }
                }

            }
            else
            {


                if (MediaFairy.Settings.ImpAwardsPostersEnabled)
                {
                    if (imageUrls.Count == 0 || MediaFairy.Settings.AlwaysPromptForImages)
                    {
                        List<string> tmpList = ImpAwards.GetPoster(item, movieName, releaseYear, "Both");
                        imageUrls.AddRange(tmpList);
                        previewImages.AddRange(tmpList);
                    }
                }

                if (MediaFairy.Settings.TMDbPostersEnabled)
                {
                    if (imageUrls.Count == 0 || MediaFairy.Settings.AlwaysPromptForImages)
                    {
                        imageUrls.AddRange(TheMovieDb.GetFilmPostersFromTMDb(item, ref previewImages));
                        //tmp = "";
                        //tmp = GetFilmPosterFromTMDb(Item);
                        //if (!String.IsNullOrEmpty(tmp))
                        //    ImageUrls.Add(tmp);
                    }
                }

            }

            if (MediaFairy.Settings.IMDbPostersEnabled)
            {
                if (imageUrls.Count == 0 || MediaFairy.Settings.AlwaysPromptForImages)
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


            byte[] imageData = Downloaders
                .TrytoDownloadData(posterUrl, "Movies", "", item);



            if (imageData == null)
                return false;

            #endregion



            #region Save Image
            //if (SaveImage(imageData, Item, Importer.PosterNamingMethod, Importer.WhereToSavePoster, Importer.PosterFolder, true, VideoFilename, filepath, "", "") != "")
            if (ImageFileConstructor.SaveImage(imageData, item,true,"",imageFile) != "")
            {
                if (MediaFairy.Settings.ResizeFilmPosters)
                {
                    Helpers.UpdateProgress("Updating Movies Section", "Resizing image...", item);
                    ImageResizer.ResizeImage(imageFile, imageFile, MediaFairy.Settings.FilmPosterWidth, MediaFairy.Settings.FilmPosterHeight, false);
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

            if (!MediaFairy.Settings.FilmCoverArtDownloaderIsEnabled)
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