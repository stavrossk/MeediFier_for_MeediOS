using System;
using System.Net;
using System.Threading;
using MediaFairy.ImportingEngine;
using MeediOS;
using TVDBLibrary;

// ReSharper disable CheckNamespace
namespace MediaFairy.TVDb
// ReSharper restore CheckNamespace
{
    class TVDbTvEpisodeDetailsDownloader
    {


        internal static void DownloadSeriesInformation(IMLItem item, string parentDir,
            ref string seriesName, ref string seriesID, bool fileServerIsOnline,
            bool isUNC, TVDBLib tvdb,string mdfSettingsb, IBaseSystem iBaseSystem )
        {


            if (String.IsNullOrEmpty(seriesName))
                return;

            WebClient client = new WebClient();
            bool foundSeries;

            TVdbTvEpisodeDetailsDownloaderHelpers.NormalizeSeriesName(ref seriesName);


            if (TVdbTvEpisodeDetailsDownloaderHelpers.GetSeriesMatchFromSectionOrTVDb(item, ref seriesName, out seriesID,
                tvdb, mdfSettingsb, iBaseSystem, out foundSeries))
                return;


            GetEpisodeKeysFromItemTagsAndPopulateEpisodeMetadata(item, seriesName, 
                seriesID, fileServerIsOnline, isUNC, tvdb, mdfSettingsb,
                iBaseSystem, client, foundSeries);
     
        
        }




        private static void GetEpisodeKeysFromItemTagsAndPopulateEpisodeMetadata(IMLItem item, string seriesName,
                            string seriesID, bool fileServerIsOnline,
                            bool isUNC, TVDBLib tvdb, string mdfSettingsb,
                            IBaseSystem iBaseSystem, WebClient client,
                            bool foundSeries)
        {

            if (!foundSeries)
            {
                MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress,
                "Series with name " + seriesName + " was not found.");

                Thread.Sleep(2000);
                
                return;
            
            }

            string episodeID = Helpers.GetTagValueFromItem(item, "EpisodeID");

            if (!String.IsNullOrEmpty(episodeID))
                return;
            
            int episodeNumber;
            int seasonNumber;

            if (TVdbTvEpisodeDetailsDownloaderHelpers.GetEpisodeKeysFromItemTags
                (item, out episodeNumber, out seasonNumber))
                return;

            if (seasonNumber == 0 || episodeNumber == 0)
                return;

            ITVDBEpisode episode;
            ITVDBSeries series;


            if (TVdbTvEpisodeDetailsDownloaderHelpers.GetSeriesAndEpisodeFromTVDb(seriesName, seriesID, tvdb,
                        episodeNumber, seasonNumber,
                        out episode, out series))
                return;


            TVdbTvEpisodeDetailsDownloaderHelpers.PopulateEpisodeMetadata(item, fileServerIsOnline, isUNC,
                        mdfSettingsb,
                        iBaseSystem, seasonNumber, client,
                        series,
                        episode);



        }
   
    
    }


}
