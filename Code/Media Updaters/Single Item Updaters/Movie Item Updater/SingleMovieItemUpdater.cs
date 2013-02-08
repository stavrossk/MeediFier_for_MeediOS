using System;
using System.Collections.Generic;
using System.IO;
using MediaFairy.Code.Media_Updaters.Single_Item_Updaters.Movie_Item_Updater;
using MediaFairy.Code.Metadata_Scrapers.Cover_Art;
using MediaFairy.IMDb;
using MediaFairy.ImportingEngine;
using MediaFairy.MediaFileDescriptors;
using MeediOS;




namespace MediaFairy.SingleItemUpdaters
{

    class SingleMovieItemUpdater
    {






        internal static bool UpdateFilmItem
            (int itemID, IMLSection moviesSection,
             IMLItemList allFilmItems, 
             ref bool fileServerChecked,
             ref bool fileServerIsOnline, 
             ConnectionResult connectionresult,
             IMDbOperations imdbOp, string pluginpath,
             ref int currentItem, int totalItems,
             string mdfSettingsa, IBaseSystem ibs,
             IEnumerable<string> combinedSceneTags) 
        {


            MainImportingEngine.CurrentProgress 
                = ImportingEngineHelpers
                .ComputeProgress
                (currentItem, totalItems);




            #region Pre-updaters

            IMLItem item
                = moviesSection
                .FindItemByID(itemID);

            if (item == null)
                return true;

            MainImportingEngine.SpecialStatus
                = "Updating " + item.Name + "...";

            Debugger.LogMessageToFile
                (Environment.NewLine +
                "Starting to work with library item:" +
                 " " + item.Name +" with ID: " + itemID);


            Debugger.LogMessageToFile
                ("Initializing item variables...");


            string imdbid = Helpers.GetTagValueFromItem(item, "ImdbID");
            string tmdbID = Helpers.GetTagValueFromItem(item, "TMDbID");

            string filmYear = Helpers.GetTagValueFromItem(item, "Year");
            string itemTitle = Helpers.GetTagValueFromItem(item, "Title");
            string sortTitle = Helpers.GetTagValueFromItem(item, "SortTitle");


            bool isMultipart = false;
            string location = item.Location;
            string[] multipart = null;
            bool isUNC = false;

            DirectoryInfo parent;
            DirectoryInfo root;
            string videoFilename;


            MultipartFilmLibraryItemDetector
                .DetectMultipartItemRetrieveFirstItemLocation
                (ref location, ref isMultipart, ref multipart);


            if (!FilmItemUpdaterHelpers
                .RetrievePathLocations
                (location, out videoFilename,
                out parent, out root))
                return false;


            bool updateFilmItem;
            if (FilmItemUpdaterHelpers
                .PerformPreupdatingDiagnostics
                (moviesSection, ref fileServerChecked, 
                ref fileServerIsOnline, root, location, 
                item, ref isUNC, out updateFilmItem))
                return updateFilmItem;

            #endregion




            #region Primary Updating

            Debugger.LogMessageToFile
                ("Identification of film '"
                + itemTitle + "' is complete." +
                 " Continuing to download online data.");


            if (Helpers.GetTagValueFromItem
                (item, "MediaFairy-processed") != "--processed--")
            {

                #region Identifiers

                bool b;
                if (FilmItemUpdaterHelpers.ExtractLocalMetadataForFilmItem
                    (fileServerIsOnline, ref imdbOp,
                    pluginpath, combinedSceneTags,
                    parent, isUNC, item,
                    ref imdbid, out b))
                    return b;


                imdbOp.ImdbMovie = null;
                imdbOp.ImdbId = imdbid;


                //#region Set IsExactMatched flag for the video fingerprint uploader

                //try
                //{
                //    Year = Helpers.GetTagValueFromItem(Item, "Year");
                //}
                //catch
                //{
                //}

                //if (!String.IsNullOrEmpty(Year))
                //    isExactMatched = true;

                //if (!String.IsNullOrEmpty(imdbid))
                //    isExactMatched = true;

                //#endregion

                if (Helpers.UserCancels
                    (MainImportingEngine
                    .SpecialStatus, item))
                    return false;


                if (
                    !VideoFingerprintIdentifier
                    .VideoFingerprintIdentifier
                    .IdentifyVideo(ref imdbid, 
                    ref tmdbID, ref imdbOp,
                    item, fileServerIsOnline,
                    isUNC, location, parent.FullName,
                    filmYear, connectionresult, moviesSection)
                    )
                {

                    Debugger.LogMessageToFile
                        ("Unable to identify this video. Continuing to next item.");
                    return true;

                }
                Debugger.LogMessageToFile
                    ("Video identification was succesfull!");


                if (Helpers.UserCancels
                    (MainImportingEngine.SpecialStatus, item))
                    return false;


                

                #endregion


                #region Set Updating Flags




                #endregion


                #region extract important fields for online searches

                string imdbidTmp = Helpers.GetTagValueFromItem(item, "ImdbID");

                #region Get IMDbID

                if (!String.IsNullOrEmpty(imdbidTmp))
                {
                    imdbid = Helpers.GetTagValueFromItem(item, "ImdbID");
                    imdbOp.ImdbId = Helpers.GetTagValueFromItem(item, "ImdbID");
                }
                else
                {
                    if (!String.IsNullOrEmpty(imdbOp.ImdbId))
                        imdbid = imdbOp.ImdbId;
                    else imdbOp.ImdbId = imdbid;

                    item.Tags["ImdbID"] = imdbOp.ImdbId;
                }

                //MessageBox.Show("Item tag imdbid: " + Helpers.GetTagValueFromItem(Item,"ImdbID"]);

                #endregion

                #region Get ItemTitle

                itemTitle = Helpers.GetTagValueFromItem(item, "Title");

                if (String.IsNullOrEmpty(itemTitle))
                {
                    if (!String.IsNullOrEmpty(item.Name))
                    {
                        itemTitle = item.Name;
                    }
                    else return true;
                }

                #endregion

                #endregion

                #region Metadata Downloaders

                //MessageBox.Show("Can work online:" + connectionresult.CanWorkOnline);

                if (connectionresult.InternetConnectionAvailable
                    || !Settings.ConnectionDiagnosticsEnabled)
                {



                        #region download details


                       imdbid = FilmItemUpdaterHelpers.IMDbDetailer
                           (moviesSection, imdbOp,
                           item, itemTitle, imdbid);
                        


                    OSoperations.GetDetailsFromOSdb
                        (imdbid, item, connectionresult.OSDbIsOnline,
                        !imdbOp.NotListed);
    

                    #endregion


                        if (Helpers.UserCancels
                            (MainImportingEngine
                            .SpecialStatus, item))
                            return false;


                        #region Get ItemTitle

                        if (!String.IsNullOrEmpty(Helpers.GetTagValueFromItem(item, "Title")))
                        {
                            itemTitle = Helpers.GetTagValueFromItem(item, "Title");
                        }
                        else if (!String.IsNullOrEmpty(item.Name))
                        {
                            itemTitle = item.Name;
                        }
                        else return true;

                        #endregion

                        #region Set SortTitle

                        if (!String.IsNullOrEmpty
                            (itemTitle) 
                            && String.IsNullOrEmpty
                            (sortTitle))
                        {
                            if (itemTitle.StartsWith("The"))
                            {
                                sortTitle = itemTitle.Remove(0, 4) + ", The ";
                                Debugger.LogMessageToFile
                                    ("Set item's SortTitle to: '" + sortTitle + "'.");
                            }
                            else sortTitle = itemTitle;

                            item.Tags["SortTitle"] = sortTitle;
                            item.SaveTags();
                        }

                        #endregion



                    if (!FilmItemUpdaterHelpers
                            .DownloadFilmDetailsExtra(item))
                            return false;


                    location = FilmItemUpdaterHelpers
                            .PerformFilmOrganizing
                            (fileServerChecked, fileServerIsOnline,
                            connectionresult, currentItem, itemTitle,
                            multipart, location, isMultipart, item);


                    if (Helpers.UserCancels
                            (MainImportingEngine
                            .SpecialStatus, item))
                            return false;


                        MovieDescriptorWriter
                            .WriteMovieDescriptor
                            (item, itemTitle, imdbid, 
                            parent.FullName,
                            fileServerIsOnline, isUNC);



                        if (Helpers.UserCancels
                            (MainImportingEngine
                            .SpecialStatus, item))
                            return false;



                        if (!DownloadFilmCoverAndBakdropArt
                            (fileServerIsOnline, mdfSettingsa, 
                            ibs, item, isUNC, itemTitle, location,
                            parent, videoFilename))
                            return false;


                }

                #endregion
            }

            #endregion




            if (!FilmItemSecondaryUpdater
                .PerformSecondaryFilmItemUpdating
                (moviesSection, allFilmItems, item))
                return false;



            item.SaveTags();
            currentItem++;

            return true;

        }

        private static bool DownloadFilmCoverAndBakdropArt
            (bool fileServerIsOnline, string mdfSettingsa,
             IBaseSystem ibs, IMLItem item, bool isUNC,
             string itemTitle, string location,
            FileSystemInfo parent, string videoFilename)
        {


            FilmCoverArtDownloadingEngine
                .LocateOrDownloadFimCoverArt
                (item, location, itemTitle,
                 videoFilename, parent.FullName,
                 fileServerIsOnline, isUNC,
                 mdfSettingsa, ibs);

            if (Helpers.UserCancels
                (MainImportingEngine
                     .SpecialStatus, item))
                return false;

            //TheMovieDbFilmBackdropDownloader
            //    .DownloadBackdropFromTheMovieDb
            //    (Item, VideoFilename, location,
            //     ItemTitle, parent.FullName,
            //     fileServerIsOnline, IsUNC,
            //     mdfSettingsa, IBS);

            return true;
        }


    }





}
