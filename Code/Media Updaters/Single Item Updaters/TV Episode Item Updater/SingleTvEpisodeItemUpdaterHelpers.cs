using System;
using System.Threading;
using System.Windows.Forms;
using MediaFairy.ImportingEngine;
using MediaFairy.TVDb;
using MeediOS;
using TVDBLibrary;



 
namespace MediaFairy.SingleItem_Updaters
{


    class SingleTvEpisodeItemUpdaterHelpers
    {




        internal static string PerformMediaOrganizing
            (bool fileServerChecked,
            bool fileServerIsOnline,
            ConnectionResult connectionresult,
            int currentItem,
            string[] multipart, string location,
            IMLItem Item, bool IsMultipart)
        {

            if (!Settings.WantSortingTvShows)
                return location;
            
            string itemTitle
                = Helpers.GetTagValueFromItem
                (Item, "EpisodeName");



            #region update runtime paramaters

            itemUpdateParams updateParams;
            updateParams.FileInUse = Settings.FileInUse;
            updateParams.IsMultipart = IsMultipart;
            updateParams.CanWorkOnline = connectionresult.InternetConnectionAvailable;
            updateParams.CompletedItems = currentItem;
            updateParams.FileServerChecked = fileServerChecked;
            updateParams.FileServerIsOnline = fileServerIsOnline;
            updateParams.OSIsOnline = connectionresult.OSDbIsOnline;

            #endregion



            MediaOrganizers.OrganizeMediaFiles
                (Item, itemTitle, multipart, ref location,
                 Settings.SortingDestinationTvShows,
                 Settings.DirectoryTagMaskTvShows,
                 updateParams,
                 "TvShow");


            return location;
       
        }




        internal static string ExtractKeyFieldsForOnlineMetadataSearch
            (IMLItem item, out string seriesName, out string imdbid)
        {


            string moviehash = Helpers.GetTagValueFromItem(item, "Hash");

            imdbid = Helpers.GetTagValueFromItem(item, "ImdbID");

            seriesName = Helpers.GetTagValueFromItem(item, "SeriesName");

            if (String.IsNullOrEmpty(seriesName))
                seriesName = item.Name;

            return moviehash;
        }




        internal static string DownloadSeriesAndEpisodeDetails
            (bool fileServerIsOnline, string mdfSettingsb,
             IBaseSystem ibs, TVDBLib tvdb, bool isUNC,
             string seriesID, IMLItem item,
             string parentDir, out string imdbid)
        {
            string moviehash = String.Empty;

            if (Helpers.GetTagValueFromItem
                (item, "MediaFairy-processed") != "--processed--")
            {
                string seriesName;

                moviehash = ExtractKeyFieldsForOnlineMetadataSearch
                    (item, out seriesName, out imdbid);



                TVDbTvEpisodeDetailsDownloader
                    .DownloadSeriesInformation
                    (item, parentDir, ref seriesName, 
                     ref seriesID, fileServerIsOnline, 
                     isUNC, tvdb, mdfSettingsb, ibs);



            }

            imdbid = String.Empty;

            return moviehash;

        }






        internal static bool PerfromMediaOrganizingAndDownloadSubtitle
            (bool fileServerChecked,  bool fileServerIsOnline,
             ConnectionResult connectionresult,
             int currentItem, bool isMultipart, string imdbid,
             string moviehash, string location, IMLItem item,
             string[] multipart)
        {

            if (Helpers.UserCancels
                (MainImportingEngine.SpecialStatus, item))
                return false;


            location = PerformMediaOrganizing
                (fileServerChecked, fileServerIsOnline,
                 connectionresult,
                 currentItem, multipart, location, 
                 item, isMultipart);


            return !Helpers.UserCancels
                (MainImportingEngine.SpecialStatus, item);


            //DownloadSubtitle(fileServerIsOnline, connectionresult,
            //            location, item, imdbid, moviehash);
        }






        internal static void LogException(Exception e, IMLItem item)
        {
            StatusForm.statusForm.TrayIcon.ShowBalloonTip(10000, "Error updating TV Episode item",
                                                          "MediaFairy encountered an unexpected error while trying to update" +
                                                          " a TV Episode item. Please refer to Debug.log for details on this error.",
                                                          ToolTipIcon.Error);

            Debugger.LogMessageToFile("[TV Episode Item Updater] An unexpected error occured while updating" +
                                      " the TV Episode item " + item.Name + ". " +
                                      "The error was: " + e);

            Thread.Sleep(5000);
        }




        internal static void SetProccesedFlag(IMLItem item)
        {
            if (

                (!String.IsNullOrEmpty(item.ImageFile) || !Settings.WantEpisodeThumbnails) &&

                (!String.IsNullOrEmpty(Helpers.GetTagValueFromItem(item, "Hash")) ||
                 !Settings.EnableVideoHasher) &&

                (!String.IsNullOrEmpty(Helpers.GetTagValueFromItem(item, "Overview")) ||
                 !Settings.WantEpisodeDetails) &&

                (!String.IsNullOrEmpty(Helpers.GetTagValueFromItem(item, "Rating")) ||
                 !Settings.WantEpisodeDetails) &&

                (!String.IsNullOrEmpty(Helpers.GetTagValueFromItem(item, "EpisodeName")) ||
                 !Settings.WantEpisodeDetails) &&

                 //TODO: Get Series Details from IMDb 
                (!String.IsNullOrEmpty(Helpers.GetTagValueFromItem(item, "SeriesIMDbID")) ||
                 !Settings.WantEpisodeDetails) &&

                (!String.IsNullOrEmpty(Helpers.GetTagValueFromItem(item, "SeriesID")) ||
                 !Settings.WantEpisodeDetails) &&

                (!String.IsNullOrEmpty(Helpers.GetTagValueFromItem(item, "SeasonID")) ||
                 !Settings.WantEpisodeDetails) &&

                (!String.IsNullOrEmpty(Helpers.GetTagValueFromItem(item, "TVDbID")) ||
                 !Settings.WantEpisodeDetails) &&

                (!String.IsNullOrEmpty(Helpers.GetTagValueFromItem(item, "Director")) ||
                 !Settings.WantEpisodeDetails) &&

                (!String.IsNullOrEmpty(Helpers.GetTagValueFromItem(item, "Writer")) ||
                 !Settings.WantEpisodeDetails)

                )
            {
                item.Tags["MediaFairy-processed"] = "--processed--";
                item.SaveTags();
            }
            else
            {
                item.Tags["MediaFairy-processed"] = "";
                item.SaveTags();
            }


        }




        internal static void ProceedToNextItem
            (ref int currentItem, IMLItem Item)
        {

            Item.SaveTags();
            currentItem++;

        }





        internal static bool PerfrormOnlineTasks
            (bool fileServerChecked,
            bool fileServerIsOnline,
            ConnectionResult connectionresult,
            string mdfSettingsb, 
            IBaseSystem iBaseSystem,
            TVDBLib tvdb,
            int currentItem, string location,
            bool isMultipart, string[] multipart,
            IMLItem item, bool isUNC, string seriesID,
            string parentDir)
        {

            if (!connectionresult.InternetConnectionAvailable &&
                Settings.ConnectionDiagnosticsEnabled)
                return true;
            
            string imdbid;
            
            string moviehash = DownloadSeriesAndEpisodeDetails
                (fileServerIsOnline, mdfSettingsb, iBaseSystem,
                 tvdb, isUNC, seriesID, item, parentDir, out imdbid);


            return PerfromMediaOrganizingAndDownloadSubtitle
                (fileServerChecked, fileServerIsOnline,
                 connectionresult, currentItem,
                 isMultipart, imdbid, moviehash, location, item, multipart);



        }





        internal static void UpdateProgressTexts(int totalItems, int currentItem, IMLItem item)
        {
            MainImportingEngine.CurrentProgress = ImportingEngineHelpers.ComputeProgress(currentItem, totalItems);
            MainImportingEngine.GeneralStatus = "Updating TV Series section...";
            MainImportingEngine.SpecialStatus = "Updating " + item.Name + "...";
        }
    }



}
