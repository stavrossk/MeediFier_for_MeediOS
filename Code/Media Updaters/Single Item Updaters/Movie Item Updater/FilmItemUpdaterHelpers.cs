using System;
using System.Collections.Generic;
using System.IO;
using MediaFairy.IMDb;
using MediaFairy.ImportingEngine;
using MeediOS;



namespace MediaFairy.Code.Media_Updaters.Single_Item_Updaters.Movie_Item_Updater
{




    class FilmItemUpdaterHelpers
    {




        internal static bool
            RetrievePathLocations
            (string location,
             out string videoFilename,
             out DirectoryInfo parent,
             out DirectoryInfo root)
        {

            videoFilename = String.Empty;
            parent = null;
            root = null;

            Debugger.LogMessageToFile
                ("Creating filesystem instances " +
                 "for media file "
                 + location + "...");

            try
            {
                var fi = new FileInfo(location);

                parent = fi.Directory;

                root = parent != null
                           ? parent.Root : null;

                videoFilename = fi.Name;

                videoFilename
                    = videoFilename.Substring
                        (0, videoFilename.Length - 4);

                return true;


            }
            catch (Exception e)
            {
                Debugger.LogMessageToFile(
                    "Unable to create filesystem instances" +
                    " for this media file. An error occured: " + e);
                return false;
            }


        
        }

        internal static string IMDbDetailer
            (IMLSection moviesSection, 
             IMDbOperations imdbOp,
             IMLItem item,
             string itemTitle,
             string imdbid)
        {


            if (!MediaFairy.Settings.FilmIMDbDetailsDownloaderIsEnabled)
                return imdbid;

            Debugger.LogMessageToFile
                ("IMDb film details downloader is enabled");


            if (String.IsNullOrEmpty(imdbid))
                return imdbid;


            if (!imdbid.StartsWith("tt"))
                imdbid = "tt" + imdbid;


            MainImportingEngine.ThisProgress.Progress
                (MainImportingEngine.CurrentProgress,
                 "Downloading details from IMDb for '"
                 + itemTitle + "'...");


            Debugger.LogMessageToFile
                ("[IMDb film details downloader] " +
                 "Proceeding to phase 1");





            IIMDbMovie movie = IMDbMovieDetailsDownloader.DownloadMovieDetailsFromIMDb
                (imdbid, true);
           


            if (movie == null)
                return imdbid;


            imdbOp.SaveIMDbMovieDetails
                (item, moviesSection, movie);


            return imdbid;
        }

        internal static bool PerformPreupdatingDiagnostics
            (IMLSection moviesSection, ref bool fileServerChecked,
             ref bool fileServerIsOnline, FileSystemInfo root, 
             string location, IMLItem item, ref bool isUNC, 
             out bool updateFilmItem)
        {


            if (Helpers.UserCancels
                (MainImportingEngine.SpecialStatus, item))
            {
                updateFilmItem = false;
                return true;
            }


            FileServerChecker.CheckFileServer
                (MediaFairy.Settings.WantFileserverDiagnostics,
                 location, ref isUNC, ref fileServerChecked,
                 root.FullName, ref fileServerIsOnline);


            if (Helpers.UserCancels
                (MainImportingEngine.SpecialStatus, item))
            {
                updateFilmItem = false;
                return true;
            }


            if (MissingItemRemover.DeleteMissingItem
                (MediaFairy.Settings.DeleteMissing, isUNC,
                 fileServerIsOnline, location,
                 moviesSection, item))
            {
                updateFilmItem = true;
                return true;
            }


            if (Helpers.UserCancels
                (MainImportingEngine.SpecialStatus, item))
            {
                updateFilmItem = false;
                return true;
            }


            Helpers.FileIsInUse
                (location, fileServerIsOnline, item);

            updateFilmItem = false;
            return false;
        }

        internal static void CleanNameSetOriginalTitle(IMLItem item)
        {


            string itemName = item.Name;

            if (String.IsNullOrEmpty(itemName))
                return;

            itemName = itemName.Replace('_', ' ');
            itemName = itemName.Replace('.', ' ');

            item.Name = itemName;
            item.SaveTags();


            string originalTitle = Helpers.GetTagValueFromItem
                (item, "OriginalTitle");


            if (!String.IsNullOrEmpty
                     (originalTitle)) 
                return;

            item.Tags["OriginalTitle"] = item.Name;
            item.SaveTags();

            Debugger.LogMessageToFile
                ("The item's original name " + item.Name +
                 " was stored to OrginalTitle tag.");


        }

        internal static string PerformFilmOrganizing
            (bool fileServerChecked, bool fileServerIsOnline,
             ConnectionResult connectionresult, int currentItem, 
             string itemTitle,  string[] multipart, 
             string location, bool isMultipart, IMLItem item)
        {

            if (!MediaFairy.Settings.WantSortingFilms) 
                return location;

            #region update runtime paramaters

            itemUpdateParams updateParams;
            updateParams.FileInUse = MediaFairy.Settings.FileInUse;
            updateParams.IsMultipart = isMultipart;
            updateParams.CanWorkOnline = connectionresult.InternetConnectionAvailable;
            updateParams.CompletedItems = currentItem;
            updateParams.FileServerChecked = fileServerChecked;
            updateParams.FileServerIsOnline = fileServerIsOnline;
            updateParams.OSIsOnline = connectionresult.OSDbIsOnline;

            #endregion

            MediaOrganizers.OrganizeMediaFiles
                (item, itemTitle,
                 multipart, ref location,
                 MediaFairy.Settings.SortingDestinationFilms,
                 MediaFairy.Settings.DirectoryTagMaskFilms,
                 updateParams, "Film");


            return location;
        }

        internal static bool DownloadFilmDetailsExtra(IMLItem item)
        {
//AllMediaGuideFilmDetailsDownloader
            //    .DownloadFilmInfoFromAMG(Item, location);


            //if (Helpers.UserCancels(MainImportingEngine.SpecialStatus, Item))
            //    return false;

            TheMovieDb.DownloadFilmDetailsFromTMDb(item);

            return !Helpers.UserCancels
                        (MainImportingEngine.SpecialStatus, item);


            //NetFlixAPI.getMovieInfo(Item);

            //if (Helpers.UserCancels(Importer.SpecialStatus, Item))
            //    return false;


            //Downloaders.GetRottenTomatoReview(Item);

            //if (Helpers.UserCancels(Importer.SpecialStatus, Item))
            //    return false;


            //SevenVideoNetFilmDetailsDownloader
            //    .DownloadFilmInfoFromSevenVideoNet
            //    (Item, location);

            //if (Helpers.UserCancels
            //    (MainImportingEngine.SpecialStatus, Item))
            //    return false;
        }

        internal static bool ExtractLocalMetadataForFilmItem
            (bool fileServerIsOnline, ref IMDbOperations imdbOp,
             string pluginpath, IEnumerable<string> combinedSceneTags,
             DirectoryInfo parent, bool isUNC, IMLItem item,
             ref string imdbid, out bool b)
        {


            if (MovieFileMetadataExtractor.ExtractMetadataFromMovieFilename
                (item, ref imdbid,
                 ref imdbOp, combinedSceneTags))
            {
                b = true;
                return true;
            }


            if (Helpers.UserCancels
                (MainImportingEngine.SpecialStatus, item))
            {
                b = false;
                return true;
            }


            CleanNameSetOriginalTitle(item);


            if (Helpers.UserCancels
                (MainImportingEngine.SpecialStatus,
                 item))
            {
                b = false;
                return true;
            }

            imdbid = NfoFileParser.RetrieveImdbidFromNfoFile
                (item, ref imdbid,
                 fileServerIsOnline,
                 isUNC, parent, pluginpath);


            b = false;
            return false;
        }
    }
}
