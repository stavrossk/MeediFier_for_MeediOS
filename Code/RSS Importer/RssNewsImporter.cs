using System;
using System.Collections;
using System.Net;
using System.Windows.Forms;
using MeediFier.Code.RegEx_Matchers;
using MeediFier.ImportingEngine;
using MeediOS;
using Sloppycode.net;


namespace MeediFier.RssImporter
{


    class RssNewsImporter
    {


        internal static void ImportRssFeeds(ConnectionResult connectionresult)
        {


            if (ReturnCases(connectionresult))
                return;

            //TODO: Provide choice to not delete existing RSS items, but instead check for existing items and skip re-importing them. ("Only Update New Items" feature)
           
            Debugger.LogMessageToFile("[RSS news downloader]" +
            " Deleting all RSS Section items in order to import new items...");
            
            MediaSectionsAllocator.NewsSection.DeleteAllItems();

            using (WebClient webClient = new WebClient())
            {

                DownloadRssFeedsAndPopulateFeedEntries
                    (MediaSectionsAllocator.NewsSection, webClient);
            
            }


        }


        private static bool ReturnCases(ConnectionResult connectionresult)
        {

            if (!Settings.EnableRssImporter)
                return true;
            Debugger.LogMessageToFile("[RSS news downloader] RSS news downloader is enabled");


            if (Settings.RsSfeeds == null)
                return true;

            if (Settings.RsSfeeds.Length == 0)
                return true;

            Debugger.LogMessageToFile("[RSS news downloader] RSS feeds were provided by the user."
                                      + Environment.NewLine +
                                      "Proceeding to download and import " + Settings.RsSfeeds.Length + " RSS feeds...");


            return !CheckInternetConnection( connectionresult);

        }


        private static void DownloadRssFeedsAndPopulateFeedEntries
            (IMLSection newsSection, WebClient webClient)
        {

            newsSection.BeginUpdate();

            foreach (string url in Settings.RsSfeeds)
                DownloadRssFeedAndPopulateFeedEntries(newsSection,
                    url, webClient);

            newsSection.EndUpdate();
        
        }


        private static void DownloadRssFeedAndPopulateFeedEntries
            (IMLSection newsSection, string url, WebClient webClient)
        {

            try
            {

                RssFeed feed;
                
                var feedEntries = DownloadFeedGetFeedEntries(url, out feed);

                PopulateFeedEntries(newsSection, feed, feedEntries, webClient);
            
            }
            catch (Exception e)
            {

                StatusForm.statusForm.TrayIcon.ShowBalloonTip(5000, "Error downloading RSS newsfeed",
                                                              "MediaFairy encountered an error trying to download an RSS newsfeed you provided. Please see Debug.log for details on this error.",
                                                              ToolTipIcon.Warning);

                Debugger.LogMessageToFile(
                    "[RSS news downloader] An unexpected error occured while trying to download an RSS newsfeed." +
                    " The error was: " + e);

            }


        }


        private static RssItems DownloadFeedGetFeedEntries
            (string url, out RssFeed feed)
        {
            Debugger.LogMessageToFile(String.Format(
                "[RSS news downloader] Downloading feed items" +
                " for RSS feed {0}...", url));

            Helpers.UpdateProgress("Importing RSS News...",
                String.Format("Downloading RSS feed" +
                              " {0}...", url), null);

            feed = RssReader.GetFeed(url);

            RssItems items = feed.Items;

            return items;
        }


        private static void PopulateFeedEntries(IMLSection newsSection, 
            RssFeed feed, IEnumerable items, WebClient webClient)
        {

            foreach (RssItem rssItem in items)
            {
                var item = CreateFeedEntry(newsSection, rssItem);
                PopulateFeedEntry(webClient, rssItem, item, feed);
            }

        }


        private static void PopulateFeedEntry(WebClient webClient, 
            RssItem rssItem, IMLItem item, RssFeed feed)
        {

            var description = PopulateFeedEntryDescription(rssItem, item, feed);

            DownloadFeedImage(webClient, item, description);

            PopulateAdditionalFeedEntryDetails(rssItem, item, feed);

        }


        private static void PopulateAdditionalFeedEntryDetails
            (RssItem rssItem, IMLItem item, RssFeed feed)
        {
            item.Tags["FeedDescription"] = feed.Description;

            item.Tags["FeedTitle"] = feed.Title;
            item.Tags["Author"] = rssItem.Author;
            item.Tags["Channel"] = feed.Category;
            item.Tags["pubDate"] = rssItem.Pubdate;
            item.Tags["guid"] = rssItem.Guid;

            item.SaveTags();
        }


        private static string PopulateFeedEntryDescription
            (RssItem rssItem, IMLItem item, RssFeed feed)
        {
            Debugger.LogMessageToFile("[RSS news downloader] Populating RSS feed entry...");
            MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress, "Populating feed entry " + feed.Title + "...");

            string description = rssItem.Description;
            string cleanDescription = Helpers.CleanHtml(description);
            item.Tags["description"] = cleanDescription;
            return description;
        }


        private static IMLItem CreateFeedEntry
            (IMLSection newsSection, RssItem rssItem)
        {
            Debugger.LogMessageToFile("[RSS news downloader] Creating RSS feed entry...");
            IMLItem item = newsSection.AddNewItem(rssItem.Title, rssItem.Link);
            item.ExternalID = rssItem.Link;
            return item;
        }


        private static void DownloadFeedImage(WebClient webClient,
            IMLItem item, string description)
        {
            string image;

            if (!GetImageUrl(description, out image))
                return;

            if (!Settings.DownloadRsSimages)
            {
                item.ImageFile = image;
                item.SaveTags();
                return;
            }

            var imagePath = ConstructImageNameAndPath(image);

            DownloadImageAndPopulateImageTag(webClient, item, imagePath, image);

        }


        private static bool GetImageUrl(string description, out string image)
        {
            image = RegExDataMiners.MatchRegexExpressionReturnFirstMatchFirstGroup(description, "<img src=\"(?<Image>.*?)\"");

            return !String.IsNullOrEmpty(image);
        }


        private static string ConstructImageNameAndPath(string image)
        {
            var imageName = ConstructImageName(image);

            var imagePath = ConstructImagePath(imageName);
            return imagePath;
        }


        private static string ConstructImageName(string image)
        {
            string imageName = RegExDataMiners.MatchRegexExpressionReturnFirstMatchFirstGroup(image, ".*/(?<Image>.*?).jpg");
            imageName = imageName + ".jpg";
            return imageName;
        }


        private static void DownloadImageAndPopulateImageTag
            (WebClient webClient, IMLItem item,
            string imagePath, string image)
        {
            webClient.DownloadFile(image, imagePath);
            item.ImageFile = imagePath;
            item.SaveTags();
        }


        private static string ConstructImagePath(string imageName)
        {
            string imagePath;

            if (Settings.RSSimagesLocation.EndsWith("\\"))
                imagePath = Settings.RSSimagesLocation + imageName;
            else imagePath = String.Format("{0}\\{1}", Settings.RSSimagesLocation, imageName);

            return imagePath;
        }


        private static bool CheckInternetConnection
            (ConnectionResult connectionresult)
        {
            return connectionresult.InternetConnectionAvailable || 
                !Settings.ConnectionDiagnosticsEnabled;
        }


    }


}
