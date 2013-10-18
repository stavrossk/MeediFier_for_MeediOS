using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using MeediFier.Properties;
using MeediFier.ImportingEngine;
using MeediOS;
using MetaDataFolders;
using TVDBLibrary;

namespace MeediFier.TVDb
{

    class TVdbTvEpisodeDetailsDownloaderHelpers
    {


        internal static bool SearchTVDbForSeries(string seriesName, TVDBLib tvdb, out IList<ITVDBSeries> seriesResults)
        {

            try
            {
                Debugger.LogMessageToFile("Searching TVdb for Series: " + seriesName + "...");
                MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress,
                "Searching for Series " + seriesName + "...");

                seriesResults = tvdb.SearchSeries(seriesName);
            
            }
            catch (Exception)
            {

                MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress,
                "TVdb did not respond. Retrying...");

                try
                {
                    seriesResults = tvdb.SearchSeries(seriesName);
                }
                catch (Exception)
                {

                    Debugger.LogMessageToFile("Communication with TVdb failed. Cannot identify this series.");
                    StatusForm.statusForm.TrayIcon.ShowBalloonTip(2000, "Communication with TVdb failed",
                    "MediaFairy was unable to connect to TVdb." +
                    " Please check your internet connection availability," +
                    " otherwise the online database may be temporarily offline or unreachable.",
                    ToolTipIcon.Warning);

                    Thread.Sleep(3000);

                    seriesResults = null;
                    return true;
                
                }

            }

            return false;

        }



        internal static bool GetSeriesKeysFromLocalMatch
            (string seriesName, out string seriesID, 
            IMLItemList seriesItems)
        {


            Debugger.LogMessageToFile
                ("The Series with name "
                + seriesName +
                "was found in media section.");

            IMLItem seriesItem = seriesItems[0];



            seriesID = (string)seriesItem.Tags["SeriesID"];
            //seriesID = seriesItem.Location;
            
            Debugger.LogMessageToFile
                ("SeriesID: " + seriesID);
            

            var notListed
                = (string) seriesItem
                .Tags["NotListed"];


            bool foundSeries = notListed != "true";

            return foundSeries;
        
        }


        internal static IMLItemList SearchForSeriesInSection(string seriesName)
        {

            MainImportingEngine.ThisProgress.Progress
                (MainImportingEngine.CurrentProgress,
                "Searching Series section for Series " + seriesName + "...");


            IMLItemList seriesItems 
                = MediaSectionsAllocator
                .TvEpisodesSection
                .SearchByTag
                ("SeriesName",
                seriesName);



            return seriesItems;
        
        }


        internal static void NormalizeSeriesName(ref string seriesName)
        {

            Debugger.LogMessageToFile("Normalizing SeriesName...");

            Char[] trimChars = {'.', '-', '_', ' '};

            seriesName = seriesName.Trim();
            seriesName = seriesName.TrimStart(trimChars);
            seriesName = seriesName.TrimEnd(trimChars);
            seriesName = seriesName.Trim();
            seriesName = seriesName.TrimStart(trimChars);
            seriesName = seriesName.TrimEnd(trimChars);
            seriesName = seriesName.Trim();
            seriesName = seriesName.Replace('.', ' ');


            Debugger.LogMessageToFile("Normalization result: " + seriesName);

        }




        internal static void PopulateSeriesDetails(IMLItem item,
                string mdfSettingsb, IBaseSystem ibs, ITVDBSeries series)
        {

            Debugger.LogMessageToFile
                ("Populating Series Details...");


            GetMainDetails(item, series);


            GetSeriesBanner(item, mdfSettingsb, ibs);


            //TODO: Add Series Posters downloading feature
            //if (Importer.WantSeriesPosters)
            //{
            //    try
            //    {

            //        string ImageFile = MetaDataFolders.TVShows.CreateSeasonPostersFilePath(Item, MDFSettingsb, ".jpg", IBS);
            //        //MessageBox.Show(ImageFile);
            //        Debugger.LogMessageToFile("Downloading Series Poster...");
            //        TvSeriesOperations.DownloadSeriesBanner(SeriesItem, "SeriesPoster", 0, "", ImageFile);
            //    }
            //    catch (Exception e)
            //    {
            //        Debugger.LogMessageToFile("[Series metadata downloader] An unexpected error occured while trying to create a Series Poster file path. The error was: " + e.ToString());
            //        StatusForm.statusForm.TrayIcon.ShowBalloonTip(5000, "Error creating image file path", "MediaFairy encountered an error while trying to create a Series Poster file path. Please see Debug.log for details on this error.", ToolTipIcon.Warning);
            //    }

            //}


            GetSeriesFanart(item, mdfSettingsb, ibs);


        }




        private static void GetSeriesFanart(IMLItem item, string mdfSettingsb, IBaseSystem ibs)
        {
            if (!Settings.WantSeriesBackdrops)
                return;
            
            try
            {
                string imageFile = TVShows.CreateSeriesFanartFilePath(item, mdfSettingsb, ".jpg", ibs);

                Debugger.LogMessageToFile("Downloading Series fanart...");

                TvSeriesOperations.DownloadSeriesBanner(item, "Fanart", 0, "", imageFile);
            
            }
            catch (Exception e)
            {

                Debugger.LogMessageToFile("[Series metadata downloader]" +
                " An unexpected error occured while trying to create a Series backdrop file path." +
                " The error was: " + e);

                StatusForm.statusForm.TrayIcon.ShowBalloonTip(10000, "Error creating image file path",
                "MediaFairy encountered an error while trying to create a Series backdrop file path. " +
                "Please see Debug.log for details on this error.",
                ToolTipIcon.Warning);
            
            }
        
        
        }


        private static void GetSeriesBanner(IMLItem item, string mdfSettingsb, IBaseSystem ibs)
        {

            if (!Settings.WantSeriesBanners) return;
            
            try
            {


                string imageFile = TVShows.CreateSeriesBannersFilePath
                    (item, mdfSettingsb, ".jpg", ibs);

                Debugger.LogMessageToFile("Downloading Series banner...");

                TvSeriesOperations.DownloadSeriesBanner(item, "SeriesBanner", 0, "", imageFile);
            
            }
            catch (Exception e)
            {
                Debugger.LogMessageToFile(
                "[Series metadata downloader] An unexpected error occured while" +
                " trying to create a Series banner file path. The error was: " + e);

                StatusForm.statusForm.TrayIcon.ShowBalloonTip(5000, "Error creating image file path",
                "MediaFairy encountered an error while trying to create a Series banner file path." +
                " Please see Debug.log for details on this error.",
                ToolTipIcon.Warning);

            }

        }


        private static void GetMainDetails
            (IMLItem item, ITVDBSeries series)
        {


            //if (!String.IsNullOrEmpty(series.ID))
            //{

            //    MediaSectionsAllocator
            //        .TvSeriesSection
            //        .BeginUpdate();

            //    IMLItem seriesItem
            //        = MediaSectionsAllocator
            //        .TvSeriesSection
            //        .AddNewItem
            //        (series.SeriesName,
            //        series.ID);


            //    seriesItem.ExternalID = series.ID;
            //    seriesItem.SaveTags();

            //    MediaSectionsAllocator
            //        .TvSeriesSection
            //        .EndUpdate();

            //}


            if (!String.IsNullOrEmpty(series.Actors))
                item.Tags["SeriesActors"] = series.Actors;

            if (!String.IsNullOrEmpty(series.Airs_DayOfWeek) && !String.IsNullOrEmpty(series.Airs_TimeOfDay))
                item.Tags["SeriesAirs"] = series.Airs_DayOfWeek + " @ " + series.Airs_TimeOfDay;

            if (!String.IsNullOrEmpty(series.FirstAired.ToShortDateString()))
                item.Tags["SeriesFirstAired"] = series.FirstAired;

            if (!String.IsNullOrEmpty(series.Genre))
                item.Tags["SeriesGenre"] = series.Genre;

            if (!String.IsNullOrEmpty(series.IMDb_ID))
                item.Tags["SeriesIMDbID"] = series.IMDb_ID;

            if (!String.IsNullOrEmpty(series.Language))
            item.Tags["SeriesLanguage"] = series.Language;

            if (series.LastUpdated != 0)
            item.Tags["SeriesLastUpdated"] = series.LastUpdated;

            if (!String.IsNullOrEmpty(series.Network))
            item.Tags["SeriesNetwork"] = series.Network;

            if (!String.IsNullOrEmpty(series.Overview))
            item.Tags["SeriesOverview"] = series.Overview;

            if (series.Rating > 0)
                item.Tags["SeriesRating"] = series.Rating;


            if (!String.IsNullOrEmpty(series.Runtime))
                item.Tags["SeriesRuntime"] = series.Runtime;

            if (!String.IsNullOrEmpty(series.SeriesName))
            item.Tags["SeriesName"] = series.SeriesName;

            if (!String.IsNullOrEmpty(series.Status))
            item.Tags["SeriesStatus"] = series.Status;

            if (!String.IsNullOrEmpty(series.Zap2It_ID))
            item.Tags["Zap2ItID"] = series.Zap2It_ID;

            if (!String.IsNullOrEmpty(series.ID))
            item.Tags["SeriesID"] = series.ID;
            


            item.SaveTags();
        

        }


        internal static bool IfSeriesWasFoundPopulateSeriesDetailsElseMarkAsNotListed
            (IMLItem item, ref string seriesName,
             out string seriesID, string mdfSettingsb,
             IBaseSystem ibs, IMLItemList seriesItems,
             IList<ITVDBSeries> seriesResults)
        {

            bool foundSeries = false;


            if (seriesResults.Count > 0)
            {

                foundSeries = true;

                ITVDBSeries series = seriesResults[0];

                seriesID = seriesResults[0].ID;

                Debugger.LogMessageToFile("SeriesID found : " + seriesID);

                seriesName = series.SeriesName;

                Debugger.LogMessageToFile("SeriesName found : " + seriesName);

                if (seriesItems.Count == 0)
                    PopulateSeriesDetails
                        (item, mdfSettingsb, ibs, series);


            }
            else
            {
                MarkSeriesAsNotListed(item, seriesName);
                seriesID = null;
            }


            
            return foundSeries;
        
        
        
        }

        internal static void MarkSeriesAsNotListed(IMLItem Item, string SeriesName)
        {
            Debugger.LogMessageToFile("The Series named " + SeriesName + " was not found in TVdb.");
            Item.Tags["NotListed"] = "true";
            Item.SaveTags();
        }




        internal static bool GetSeriesKeysFromTVDb
            (IMLItem item, ref string seriesName,
            out string seriesID, TVDBLib tvdb, string mdfSettingsb,
            IBaseSystem iBaseSystem, IMLItemList seriesItems,
            out bool foundSeries)
        {


            IList<ITVDBSeries> seriesResults;



            if (SearchTVDbForSeries
                (seriesName, tvdb, out seriesResults))
            {
                seriesID = null;
                foundSeries = false;
                return true;
            }


            foundSeries = IfSeriesWasFoundPopulateSeriesDetailsElseMarkAsNotListed
                (item, ref seriesName, out seriesID, mdfSettingsb, iBaseSystem, seriesItems, seriesResults);

            return false;
        }




        internal static bool GetSeriesMatchFromSectionOrTVDb
            (IMLItem item, ref string seriesName, out string SeriesID,
            TVDBLib tvdb, string mdfSettingsb, IBaseSystem IBS,
            out bool foundSeries)
        {


            var seriesItems = SearchForSeriesInSection(seriesName);


            if (seriesItems.Count > 0)
            {

                foundSeries = GetSeriesKeysFromLocalMatch
                    (seriesName, out SeriesID, seriesItems);



                if (string.IsNullOrEmpty(SeriesID))
                {
                    if (GetSeriesKeysFromTVDb(item, ref seriesName, out SeriesID, tvdb,
                            mdfSettingsb, IBS, seriesItems, out foundSeries))
                        return true;
                }

            }
            else
            {
                if (GetSeriesKeysFromTVDb(item, ref seriesName, out SeriesID, tvdb,
                                            mdfSettingsb, IBS, seriesItems, out foundSeries))
                    return true;
            }

            return false;
    
        
        }

        internal static void PopulateEpisodeDetails(IMLItem item, ITVDBEpisode episode, ITVDBSeries series)
        {

            Debugger.LogMessageToFile("Populating episode details...");

            item.Name = series.SeriesName + " " +
                        episode.SeasonNumber + "x" +
                        episode.EpisodeNumber + " - " +
                        episode.EpisodeName;


            item.Tags["SeasonID"] = episode.SeasonID;
            item.Tags["SeriesID"] = episode.SeriesID;
            item.Tags["TVDbID"] = episode.ID;

            item.Tags["EpisodeNumber"] = Convert.ToString(episode.EpisodeNumber);
            item.Tags["SeasonNumber"] = Convert.ToString(episode.SeasonNumber);
            item.Tags["EpisodeName"] = episode.EpisodeName;

            if (!Settings.WantEpisodeDetails)
                return;

            item.Tags["Director"] = episode.Director;
            item.Tags["Overview"] = episode.Overview;
            item.Tags["SeriesOverview"] = series.Overview;
            item.Tags["SeriesActors"] = series.Actors;
            item.Tags["Rating"] = episode.Rating;
            item.Tags["Writer"] = episode.Writer;
            item.Tags["GuestStars"] = episode.GuestStars;
            item.Tags["DVD_Chapter"] = episode.DVD_Chapter;
            item.Tags["DVD_Disc_ID"] = episode.DVD_Disc_ID;
            item.Tags["DVD_Episode_Number"] = episode.DVD_Episode_Number;


        }

        internal static void GetSeasonPoster(IMLItem item, string mdfSettingsb, IBaseSystem iBaseSystem, ITVDBEpisode episode,
                                            int seasonNumber)
        {

            if (!Settings.WantSeasonImages) 
                return;

            string imageFile = TVShows.CreateSeasonPostersFilePath
                (item, mdfSettingsb, ".jpg", iBaseSystem);
            
            TvSeriesOperations.DownloadSeriesBanner
                (item, "SeasonImage", seasonNumber, episode.SeasonID, imageFile);
        
        }

        internal static bool GetEpisodeKeysFromItemTags(IMLItem item, out int episodeNumber, out int seasonNumber)
        {

            string seasonNumberTmp = Helpers.GetTagValueFromItem(item, "SeasonNumber");
            string episodeNumberTmp = Helpers.GetTagValueFromItem(item, "EpisodeNumber");


            try
            {
                seasonNumber = Convert.ToInt16(seasonNumberTmp);
                episodeNumber = Convert.ToInt16(episodeNumberTmp);
            }
            catch (Exception)
            {
                MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress,
                                                          "The Season or Episode number was not recognized.");
                StatusForm.statusForm.TrayIcon.ShowBalloonTip(10000, "Unrecognised Series episode number",
                                                              "The Season or Episode numbers for the episode " + item.Name +
                                                              " could not be recognized by MediaFairy." +
                                                              " Please click here to open the folder containing the Unrecognized" +
                                                              " Episodes.txt file and send it to the application's developer to modify" +
                                                              " the Series Recognition engine in order to recognize this video file's naming scheme. ",
                                                              ToolTipIcon.Warning);

                Thread.Sleep(5000);

                episodeNumber = 0;
                seasonNumber = 0;
                
                return true;
            }

            return false;

        }

        internal static void GetEpisodeDetailsAndImages(IMLItem item, bool fileServerIsOnline, bool isUNC, string mdfSettingsb,
                                                       IBaseSystem iBaseSystem, WebClient client, int seasonNumber,
                                                       ITVDBEpisode episode, ITVDBSeries series)
        {

            PopulateEpisodeDetails
                (item, episode, series);

            GetEpisodeThumbnail(item, fileServerIsOnline, isUNC, mdfSettingsb,
                                iBaseSystem, episode, client);

            GetSeasonPoster
                (item, mdfSettingsb, iBaseSystem, episode, seasonNumber);

        }

        internal static bool GetSeriesFromTVDb(string seriesName, string seriesID,
                                              TVDBLib tvdb, out ITVDBSeries series)
        {

            Debugger.LogMessageToFile("Getting information for Series " + seriesName + "...");
            MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress,
                                                      "Getting information for Series " + seriesName + "...");

            try
            {
                series = tvdb.GetSeries(seriesID);
            }
            catch
            {
                MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress,
                                                          "TVdb did not respond. Retrying...");

                try
                {
                    series = tvdb.GetSeries(seriesID);
                }
                catch (Exception)
                {
                    Debugger.LogMessageToFile("Unable to connect to TVdb." +
                                              " Cannot get information for this Series.");
                    
                    StatusForm.statusForm.TrayIcon.ShowBalloonTip(2000, "Unable to connect to TVdb",
                                                                  "MediaFairy was unable to contact TVdb database in order to download episode information. Please check your internet connection availability, otherwise the online database may be temporarily unavailable.",
                                                                  ToolTipIcon.Info);

                    Thread.Sleep(2000);
                    series = null;
                    return true;

                }

            }

            return false;
        
        }

        internal static void GetEpisodeThumbnail(IMLItem item, bool fileServerIsOnline, bool isUNC, string mdfSettingsb,
                                                 IBaseSystem iBaseSystem, ITVDBEpisode episode, WebClient client)
        {

            string imageFile = TVShows.CreateEpisodeThumbsFilePath
                (item, mdfSettingsb, ".jpg", iBaseSystem);


            string imagelocation = imageFile;

            if (File.Exists(imagelocation))
            {
                item.ImageFile = imagelocation;
                item.SaveTags();
                return;
            }

            if (!Settings.WantEpisodeThumbnails)
                return;

            if (String.IsNullOrEmpty(episode.ImageFile))
                return;

            if (!fileServerIsOnline && isUNC)
                return;


            MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress,
                                                      "Downloading episode thumbnail...");

            try
            {
                client.DownloadFile(episode.ImageFile, imagelocation);
                item.ImageFile = imagelocation;
                item.SaveTags();
            }
            catch
            {
                try
                {
                    client.DownloadFile(episode.ImageFile, imagelocation);
                    item.ImageFile = imagelocation;
                    item.SaveTags();
                }
                catch
                {
                }

            }

        }

        internal static bool GetSeriesAndEpisodeFromTVDb(string seriesName, string seriesID, TVDBLib tvdb, int episodeNumber,
                                                        int seasonNumber, out ITVDBEpisode episode, out ITVDBSeries series)
        {
            if (GetSeriesFromTVDb(seriesName, seriesID, tvdb, out series))
            {
                episode = null;
                return true;
            }

            return GetEpisodeFromTvDb(seriesName, seriesID, tvdb,
                                      seasonNumber, episodeNumber, out episode);

        }

        internal static bool GetEpisodeFromTvDb(string seriesName, string seriesID, TVDBLib tvdb, int seasonNumber,
                                               int episodeNumber, out ITVDBEpisode episode)
        {
            try
            {
                Debugger.LogMessageToFile("Getting information for Season " + seasonNumber + ", Episode " +
                                          Convert.ToString(episodeNumber) + Resources.ImageSelectionEngine_UserSelectsImage__of_ + seriesName + "...");
                
                MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress,
                                                          "Getting information for Season " + seasonNumber + ", Episode " +
                                                          Convert.ToString(episodeNumber) + Resources.ImageSelectionEngine_UserSelectsImage__of_ + seriesName + "...");
            
                episode = tvdb.GetEpisode(seriesID, seasonNumber, episodeNumber, TVDBLib.EpisodeOrderEnum.Default);
           
            }
            catch
            {
                try
                {
                    MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress,
                                                              "TVdb did not respond. Retrying...");

                    episode = tvdb.GetEpisode(seriesID, seasonNumber,
                                              episodeNumber, TVDBLib.EpisodeOrderEnum.Default);
             
                }
                catch (Exception e)
                {

                    Debugger.LogMessageToFile("Unable to connect to TVdb. Cannot download Episode information.");
                    
                    StatusForm.statusForm.TrayIcon.ShowBalloonTip(2000, "Unable to connect to TVdb",
                                                                  "MediaFairy was unable to contact TVdb database in order to download episode information. " +
                                                                  "Please check your internet connection availability, " +
                                                                  "otherwise the online database may be temporarily unavailable.",
                                                                  ToolTipIcon.Info);

                    Thread.Sleep(2000);

                    episode = null;
                    
                    return true;
                
                }

            }

            return false;
        
        }

        internal static void PopulateEpisodeMetadata(IMLItem item, bool fileServerIsOnline, bool isUNC, string mdfSettingsb,
                                                    IBaseSystem iBaseSystem, int seasonNumber, WebClient client,
                                                    ITVDBSeries series, ITVDBEpisode episode)
        {
            if (episode == null)
                return;

            if (String.IsNullOrEmpty(episode.ID.Trim()) ||
                episode.ID == null) 
                return;

            Debugger.LogMessageToFile("Found episode named " +
                                      episode.EpisodeName + "with ID " +
                                      episode.ID + ".");

            Debugger.LogMessageToFile("Writing series tags...");

            item.Tags["SeriesName"] = series.SeriesName;
            item.Tags["SeriesIMDbID"] = series.IMDb_ID;
            item.Tags["SeriesID"] = series.ID;


            GetEpisodeDetailsAndImages(item, fileServerIsOnline, isUNC,
                                                                             mdfSettingsb, iBaseSystem, client,
                                                                             seasonNumber, episode, series);


        }
    }


}
