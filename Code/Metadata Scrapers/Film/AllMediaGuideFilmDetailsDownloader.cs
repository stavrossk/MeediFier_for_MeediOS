using System;
using System.Windows.Forms;
using MediaFairy.Code.RegEx_Matchers;
using MeediOS;




namespace MediaFairy.Code.Metadata_Scrapers.Film
{




    class AllMediaGuideFilmDetailsDownloader
    {




        internal static bool DownloadFilmInfoFromAMG
            (IMLItem Item, string location)
        {


            if (!MediaFairy.Settings
                .AllMediaGuideFilmDetailsEnabled)   
                return false;



            if (!String.IsNullOrEmpty
                (Helpers.GetTagValueFromItem
                (Item,"AMGUpdated")))
                return true;



            string itemTitle
                = Helpers.GetTagValueFromItem
                (Item, "Title");




            if (String.IsNullOrEmpty(itemTitle))
            {


                Debugger.LogMessageToFile
                    ("[AMG film details downloader]" +
                     " This item's Title field is not populated." +
                     " Additional film details from AMG" +
                     " cannot be downloaded for this item.");


                return false;


            }




            #region Construct search URL
            string videoid = Helpers.GetTagValueFromItem(Item, "AMGVideoID");
            string BaseUrl = "http://www.allmovie.com/search/work/";
            string ItemName = Item.Name;
            
            string filmPage = "";
            itemTitle = StringFunctions.replaceIllegalCharacters(itemTitle, "-");
            string SearchUrl = itemTitle.Replace(' ', '+');
            SearchUrl = BaseUrl + SearchUrl + "/results";
            #endregion

            try
            {

                #region Download film html page
                Helpers.UpdateProgress("Updating Movies Section...", "Searching AMG for film " + itemTitle + "...",  Item);
                string UrlBody = Downloaders.MineWebDataReturnFirstMatch(SearchUrl, "'http://www.allmovie.com/work/(?<UrlBody>.*?)'\">");

                if (String.IsNullOrEmpty(UrlBody))
                {
                    Debugger.LogMessageToFile("No results were found for this film. Unable to download aditiional film information from AMG.");
                    return false;
                }

                string filmUrl = "http://www.allmovie.com/work/" + UrlBody;

                Debugger.LogMessageToFile(filmPage);
                if (Helpers.GetTagValueFromItem(Item,"Synopsis") == "" || Item.Tags["Synopsis"] == null)
                {
                    Helpers.UpdateProgress("Updating Movies Section...", "Downloading film information from AMG...",  Item);
                    filmPage = HtmlDownloaders.DownloadHTMLfromURL(filmUrl);
                }
                Debugger.LogMessageToFile(filmUrl);
                //Debugger.LogMessageToFile(filmPage);

                if (String.IsNullOrEmpty(filmPage))
                    return false;
                #endregion


                #region Get VideoID
                if (String.IsNullOrEmpty(videoid))
                {
                    videoid = RegExMatchers.MatchExpressionReturnFirstMatchFirstGroup(filmPage, "Work.ID[^$]*?<pre>V(?<VideoID>[^$]*?)</pre");
                    videoid = videoid.Trim();
                    Item.Tags["AMGVideoID"] = videoid;
                    Item.SaveTags();
                }
                #endregion


                #region Download Details

                #region Synopsis
                if (Helpers.GetTagValueFromItem(Item,"Synopsis") == "" || Item.Tags["Synopsis"] == null)
                {
                    string Synopsis = RegExMatchers.MatchExpressionReturnFirstMatchFirstGroup(filmPage, "Synopsis[^$]*<p>(?<Synopsis>[^$]*)</p>");
                    //Debugger.LogMessageToFile("Synopsis: " + Synopsis);
                    string[] dirtyText = RegExMatchers.MatchExpressionReturnAllMatchesFirstGroup(Synopsis, "<.*?>");
                    if (dirtyText != null)
                    {
                        foreach (string dirtyTag in dirtyText)
                        {
                            Synopsis = Synopsis.Replace(dirtyTag, "");
                        }
                    }
                    Item.Tags["Synopsis"] = Synopsis;
                    Item.SaveTags();
                }
                #endregion

                #region Get Flags
                string flagsTmp = RegExMatchers.MatchExpressionReturnFirstMatchFirstGroup(filmPage, "Flags[^$]*?;\">(?<Flags>[^$]*?)</ul>");
                string[] Flags = RegExMatchers.MatchExpressionReturnAllMatchesFirstGroup(flagsTmp, "<li>(?<Flags>[^$]*?)</li>");
                flagsTmp = "|";
                if (Flags != null)
                {
                    foreach (string flag in Flags)
                        flagsTmp = flagsTmp + flag + "|";

                    Item.Tags["Flags"] = flagsTmp;
                    Item.SaveTags();
                }
                #endregion

                #region Get Themes
                string themesTmp = RegExMatchers.MatchExpressionReturnFirstMatchFirstGroup(filmPage, "Themes[^$]*?;\">(?<Themes>[^$]*?)</ul>");
                string[] Themes = RegExMatchers.MatchExpressionReturnAllMatchesFirstGroup(themesTmp, "<li><a.href=[^$]*?\">(?<Themes>[^$]*?)</a></li>");
                themesTmp = "|";
                if (Themes != null)
                {
                    foreach (string theme in Themes)
                        themesTmp = themesTmp + theme + "|";

                    Item.Tags["Themes"] = themesTmp;
                    Item.SaveTags();
                }
                #endregion

                #region Get Types
                string typesTmp = RegExMatchers.MatchExpressionReturnFirstMatchFirstGroup(filmPage, "Types[^$]*?;\">(?<Types>[^$]*?)</ul>");
                string[] Types = RegExMatchers.MatchExpressionReturnAllMatchesFirstGroup(typesTmp, "<li><a.href=[^$]*?\">(?<Types>[^$]*?)</a></li>");
                typesTmp = "|";
                if (Types != null)
                {
                    foreach (string type in Types)
                        typesTmp = typesTmp + type + "|";

                    Item.Tags["Types"] = typesTmp;
                    Item.SaveTags();
                }
                #endregion

                #region Get Tones
                string tonesTmp = RegExMatchers.MatchExpressionReturnFirstMatchFirstGroup(filmPage, "Tones[^$]*?;\">(?<Tones>[^$]*?)</ul>");
                string[] Tones = RegExMatchers.MatchExpressionReturnAllMatchesFirstGroup(tonesTmp, "<li><a.href=[^$]*?\">(?<Tones>[^$]*?)</a></li>");
                tonesTmp = "|";
                if (Tones != null)
                {
                    foreach (string tone in Tones)
                        tonesTmp = tonesTmp + tone + "|";

                    Item.Tags["Tones"] = tonesTmp;
                    Item.SaveTags();
                }
                #endregion


                #region Get Similar Works
                if ( String.IsNullOrEmpty(Helpers.GetTagValueFromItem(Item, "Similar") ) )
                {
                    string similarTmp = RegExMatchers.MatchExpressionReturnFirstMatchFirstGroup(filmPage, "Similar.Works(?<Similar>[^$]*?)</table>");
                    string[] Similar = RegExMatchers.MatchExpressionReturnAllMatchesFirstGroup(similarTmp, "<a.href=[^$]*?\">(?<Similar>[^$]*?)</a>");
                    similarTmp = "|";
                    if (Similar != null)
                    {
                        foreach (string similar in Similar)
                            similarTmp = similarTmp + similar + "|";

                        Item.Tags["Similar"] = similarTmp;
                        Item.SaveTags();
                    }
                }
                #endregion


                string reviewPage = HtmlDownloaders.DownloadHTMLfromURL(filmUrl + "/review");
                #region Get Review
                if (!String.IsNullOrEmpty(reviewPage))
                {
                    if (Helpers.GetTagValueFromItem(Item, "CriticReview") == "" || Item.Tags["CriticReview"] == null)
                    {
                        string AMGReview = RegExMatchers.MatchExpressionReturnFirstMatchFirstGroup(reviewPage, "Review[^$]*<p>(?<Review>[^$]*)</p>");
                        //Debugger.LogMessageToFile("Review: " + AMGReview);
                        string[] dirtyText = RegExMatchers.MatchExpressionReturnAllMatchesFirstGroup(AMGReview, "<.*?>");
                        if (dirtyText != null)
                        {
                            foreach (string dirtyTag in dirtyText)
                            {
                                AMGReview = AMGReview.Replace(dirtyTag, "");
                            }
                        }
                        Item.Tags["CriticReview"] = AMGReview;
                        Item.SaveTags();
                    }
                }
                #endregion


                #endregion


                Item.Tags["AMGUpdated"] = "true";
                Item.SaveTags();

            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            return true;
        }
    }
}
