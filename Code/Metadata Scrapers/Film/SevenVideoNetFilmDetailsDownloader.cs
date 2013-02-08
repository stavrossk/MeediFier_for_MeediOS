using System;
using System.Windows.Forms;
using MediaFairy.Code.RegEx_Matchers;
using MeediOS;




namespace MediaFairy.Code.Metadata_Scrapers.Film
{



    class SevenVideoNetFilmDetailsDownloader
    {



        internal static bool DownloadFilmInfoFromSevenVideoNet
            (IMLItem Item, string location)
        {

            if (!MediaFairy.Settings.SevenFilmDetailsEnabled)
                return false;

            if (!String.IsNullOrEmpty(Helpers.GetTagValueFromItem(Item, "SevenUpdated")))
                return true;

        
            #region Construct search URL
            string BaseUrl = "http://www.videoseven.gr/site.php?file=searchsimple.xml&searchstr=";
            string ItemName = Item.Name;
            string ItemTitle = Helpers.GetTagValueFromItem(Item, "Title");
            string filmPage = "";
            ItemTitle = StringFunctions.replaceIllegalCharacters(ItemTitle, "-");
            string SearchUrl = ItemTitle.Replace(' ', '+');
            string Suffix = "&imageField22.x=0&imageField22.y=0";
            SearchUrl = BaseUrl + SearchUrl + Suffix;
            #endregion

            try
            {

                #region Download film html page
                Helpers.UpdateProgress("Updating Movies Section...", "Searching Seven Video Net for film " + ItemTitle + "...", Item);
                string MovieID = Downloaders.MineWebDataReturnFirstMatch(SearchUrl, "showOneMovieImage.xml&id=(?<MovieID>.*?)\"");
                //MessageBox.Show("MovieID: " + MovieID);

                if (String.IsNullOrEmpty(MovieID))
                {
                    Debugger.LogMessageToFile("No results were found for this film. Unable to download aditiional film information from AMG.");
                    return false;
                }

                string filmUrl = "http://www.videoseven.gr/site.php?&file=moviedetails.xml&id=" + MovieID;

                Debugger.LogMessageToFile("filmUrl: " + filmUrl);

                Helpers.UpdateProgress("Updating Movies Section...", "Downloading film information from Seven Video Net...", Item);
                filmPage = HtmlDownloaders.DownloadHTMLfromURLUnicode(filmUrl);
                
                //Debugger.LogMessageToFile(filmPage);
                //MessageBox.Show("filmPage: " + filmPage);

                if (String.IsNullOrEmpty(filmPage))
                    return false;
                #endregion



                #region Download Details

                #region Synopsis


                string Synopsis = RegExMatchers.MatchExpressionReturnFirstMatchFirstGroup(filmPage, "<td align=\"justified\"><p>(?<Synopsis>[^$]*?)<br>");
                //Debugger.LogMessageToFile("Synopsis: " + Synopsis);
                string[] dirtyText = RegExMatchers.MatchExpressionReturnAllMatchesFirstGroup(Synopsis, "<.*?>");
                if (dirtyText != null)
                {
                    foreach (string dirtyTag in dirtyText)
                    {
                        Synopsis = Synopsis.Replace(dirtyTag, "");
                    }
                }

                //MessageBox.Show("Synopsis: " + Synopsis);
                if (!String.IsNullOrEmpty(Synopsis))
                {
                    Item.Tags["LongOverview"] = Synopsis;
                    Item.SaveTags();
                }

                string Tagline = RegExMatchers.MatchExpressionReturnFirstMatchFirstGroup(filmPage, "<span class=\"comment\">(?<Tagline>.*?)</span>");
                if (!String.IsNullOrEmpty(Tagline))
                {
                    Item.Tags["Tagline"] = Tagline;
                    Item.SaveTags();
                }

                string TitleGR = RegExMatchers.MatchExpressionReturnFirstMatchFirstGroup(filmPage, "<b>(?<TitleGR>.*?)</b><br/>");
                Item.Tags["TitleGR"] = TitleGR;
                Item.SaveTags();
                    
                
                #endregion


      

                #endregion


                Item.Tags["SevenUpdated"] = "true";
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
