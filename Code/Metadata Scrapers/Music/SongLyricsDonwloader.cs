using System;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MediaFairy.Code.RegEx_Matchers;
using MediaFairy.ImportingEngine;
using MeediOS;





namespace MediaFairy.Code.Metadata_Scrapers.Music
{






    class SongLyricsDonwloader
    {





        internal static void DownloadSongLyrics
            (IMLItem Item, ConnectionResult connectionresult)
        {



            if (!Settings.LyricsEnabled)
                return;


            if (!connectionresult.InternetConnectionAvailable
                && Settings.ConnectionDiagnosticsEnabled)
                return;

            try
            {


                Debugger.LogMessageToFile
                    ("Retrieving LyricsSearched value...");

                string LyricsSearched 
                    = Helpers.GetTagValueFromItem
                    (Item, "---LyricsSearched---");



                if (Settings.SkipSearchedLyrics)
                {
                    if (LyricsSearched == "true")
                        return;
                }

                Debugger.LogMessageToFile("Retrieving Lyrics value...");
                string Lyrics = Helpers.GetTagValueFromItem(Item, "Lyrics");
                Debugger.LogMessageToFile("Retrieving Artist value...");
                string Artist = Helpers.GetTagValueFromItem(Item, "Artist");
                Debugger.LogMessageToFile("Retrieving Title value...");
                string Title = Helpers.GetTagValueFromItem(Item, "Title");

                if (!String.IsNullOrEmpty(Lyrics))
                    return;

                if (String.IsNullOrEmpty(Artist))
                    return;

                if (String.IsNullOrEmpty(Title))
                    return;

                Debugger.LogMessageToFile("Creating LyricFinder class...");
                Lyrics myLyrics = new Lyrics();
                string lyricsStr;

                Debugger.LogMessageToFile("Searching lyrics for " + Title + "...");
                MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress, "Searching lyrics for  " + Title + "...");
                lyricsStr = myLyrics.ReturnLyrics(Artist, Title);

                string[] dirtyText = RegExMatchers.MatchExpressionReturnAllMatchesFirstGroup(lyricsStr, "<br />");
                if (dirtyText != null)
                    lyricsStr = dirtyText.Aggregate(lyricsStr, (current, dirtyTag) => current.Replace(dirtyTag, Environment.NewLine));


                string[] dirtyTextb = RegExMatchers.MatchExpressionReturnAllMatchesFirstGroup(lyricsStr, "<.*?>");
                if (dirtyTextb != null)
                    lyricsStr = dirtyTextb.Aggregate(lyricsStr, (current, dirtyTag) => current.Replace(dirtyTag, ""));


                string dirtyTextc = RegExMatchers.MatchExpressionReturnFirstMatchFirstGroup(lyricsStr, "Send.*?Ringtone to your Cell ");

                if (!String.IsNullOrEmpty(dirtyTextc))
                    lyricsStr = lyricsStr.Replace(dirtyTextc, "");

                Debugger.LogMessageToFile("Saving to 'Lyrics' tag...");
                Item.Tags["Lyrics"] = lyricsStr;
                Item.Tags["---LyricsSearched---"] = "true";
                Item.SaveTags();
            }
            catch (Exception e)
            {
                Debugger.LogMessageToFile("An uncexpected error occurred in the Lyrics downloader method. The error was: " + e.ToString() );
                StatusForm.statusForm.TrayIcon.ShowBalloonTip(10000, "An error ocurred", "An uncexpected error occurred in the Lyrics downloader method." + Environment.NewLine + "Please see Debug.log for details.",ToolTipIcon.Error);
            }
        }




        internal static void GetLyricsFromLyricWiki(IMLItem Item)
        {

            string Artist = Helpers.GetTagValueFromItem(Item, "Artist");
            string Title = Helpers.GetTagValueFromItem(Item,"Title");
            string Lyrics = Helpers.GetTagValueFromItem(Item, "Lyrics");

            if (!String.IsNullOrEmpty(Lyrics))
                return;

            var wiki = new LyricWiki();
            LyricsResult result;



            MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress, "Searching LyricWiki for lyrics...");
            if(wiki.checkSongExists(Artist,Title))
            {
                MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress, "Downloading lyrics for song " + Title + "...");
                result = wiki.getSong(Artist, Title);
                Encoding iso8859 = Encoding.GetEncoding("ISO-8859-1");
                Item.Tags["Lyrics"] = Encoding.UTF8.GetString(iso8859.GetBytes(result.lyrics));
                Item.SaveTags();
            }
        }
    }



}
