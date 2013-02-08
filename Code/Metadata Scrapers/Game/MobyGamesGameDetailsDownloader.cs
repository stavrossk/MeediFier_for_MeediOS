using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MediaFairy.Code.RegEx_Matchers;
using MeediOS;

namespace MediaFairy.Code.Metadata_Scrapers.Game
{
    class MobyGamesGameDetailsDownloader
    {
        internal static void DownloadGameDetailsFromMobyGames(IMLItem Item)
        {
            FileInfo fi = new FileInfo(Item.Location);
            string filepath = fi.Directory.FullName;
            string BaseUrl = "http://www.mobygames.com/search/quick?q=";
            string ItemName = Item.Name;
            string ItemTitle = Helpers.GetTagValueFromItem(Item,"Title");
            string SearchUrl = ItemName.Replace(' ', '+');
            string detailsPage = "";
            //string gameid = Helpers.GetTagValueFromItem(Item,"AMG Game ID");
            SearchUrl = BaseUrl + SearchUrl + "&p=3&search=Go";
            string gamePage = Downloaders.MineWebDataReturnFirstMatch(SearchUrl, "game/windows/(?<gamePage>.*?)\"");
            gamePage = "http://www.mobygames.com/game/windows/" + gamePage;
            //MessageBox.Show("SearchUrl: " + SearchUrl);
            //Process p = new Process();
            //p.StartInfo.FileName = SearchUrl;
            //p.Start();

            try
            {
 
                if (Helpers.GetTagValueFromItem(Item,"HasDetails") != "True")
                {
                    Helpers.UpdateProgress("Updating Games Section...", "Downloading game information for " + ItemName + "...",  Item);
                    detailsPage = HtmlDownloaders.DownloadHTMLfromURL(gamePage);
                }

                if (String.IsNullOrEmpty(detailsPage))
                    return;

                if (Helpers.GetTagValueFromItem(Item,"HasDetails") != "True")
                {
                    #region Download Details
                    string Synopsis = RegExMatchers.MatchExpressionReturnFirstMatchFirstGroup(detailsPage, "Description</h2>(?<Description>.*?)<div class=\"sideBarLinks\">");
                    string[] dirtyText = RegExMatchers.MatchExpressionReturnAllMatchesFirstGroup(Synopsis, "<.*?>");
                    if (dirtyText != null)
                    {
                        foreach (string dirtyTag in dirtyText)
                        {
                            Synopsis = Synopsis.Replace(dirtyTag, "");
                        }
                    }
                    Item.Tags["Description"] = Synopsis;
                    Item.SaveTags();


                    string Rating = RegExMatchers.MatchExpressionReturnFirstMatchFirstGroup(detailsPage, "PEGI Rating.*?/\">(?<Rating>.*?)</a>");
                    Item.Tags["Rating"] = Rating;
                    Item.SaveTags();

                    string Developer = RegExMatchers.MatchExpressionReturnFirstMatchFirstGroup(detailsPage, "Developed by.*?/company/.*?\">(?<Developer>.*?)</a>");
                    Item.Tags["Developer"] = Developer;
                    Item.SaveTags();

                    string Publisher = RegExMatchers.MatchExpressionReturnFirstMatchFirstGroup(detailsPage, "Published by.*?/company/.*?\">(?<Publisher>.*?)</a>");
                    Item.Tags["Publisher"] = Publisher;
                    Item.SaveTags();

                    string Review = RegExMatchers.MatchExpressionReturnFirstMatchFirstGroup(detailsPage, "scoreMed\">(?<Score>.*?)</div>");
                    Item.Tags["Review"] = Review;
                    Item.SaveTags();



                    #region Get Genres
                    string[] Genres = RegExMatchers.MatchExpressionReturnAllMatchesFirstGroup(detailsPage, "<a href=\"/genre/sheet/.*?/\">(?<Genre>.*?)</a>");
                    string GenresTmp = "|";
                    if (Genres != null)
                    {
                        foreach (string genre in Genres)
                            GenresTmp = GenresTmp + genre + "|";

                        Item.Tags["Genres"] = GenresTmp;
                        Item.SaveTags();
                    }
                    #endregion


                    Item.Tags["HasDetails"] = "True";
                    Item.SaveTags();
                    #endregion
                }


                if (Item.ImageFile == "")
                {
                    #region Download Cover image
                    string coverID = RegExMatchers.MatchExpressionReturnFirstMatchFirstGroup(detailsPage, "coreGameCover.*?src=\"http://www.mobygames.com/images/covers/small/(?<Cover>.*?)\"");
                    string coverImage = "http://www.mobygames.com/images/covers/large/" + coverID;
                    //MessageBox.Show("coverimage: " + coverImage);
                    byte[] imageData = Downloaders.TrytoDownloadData(coverImage, "Games", "AMG", Item);

                    #region Save Image
                    //MessageBox.Show(Importer.AlbumCoverFolder);
                    string imagefile = ImageFileConstructor.SaveImage(imageData, Item, "folder.jpg", MediaFairy.Settings.WhereToSaveAlbumCover, MediaFairy.Settings.AlbumCoverFolder, true, ItemName, filepath, "", "");

                    if (!String.IsNullOrEmpty(imagefile))
                    {
                        //Helpers.UpdateProgress("Games", "Cover image was downloaded succesfully!", Item);
                        //Thread.Sleep(500);
                        Item.Tags["HasCover"] = "true";
                        Item.SaveTags();
                    }
                    #endregion

                    #endregion
                }

                #region Download Screenshots
                //if (Helpers.GetTagValueFromItem(Item,"ScreenShots") == "" || Item.Tags["ScreenShots"] == null)
                //{
                //    #region Download ScreenShots
                //    string screensPageURL = "http://www.allgame.com/cg/agg.dll?p=agg&sql=1:" + gameid + "~T5";
                //    string screensPage = Downloaders.DownloadHTMLfromURL(screensPageURL);
                //    //MessageBox.Show("screensPage: " + screensPage);
                //    string screensPageCleaned = Helpers.MatchExpressionReturnFirst(screensPage, "Begin.Center.Content-->.*?End.Center.Content");
                //    //MessageBox.Show("screensPageCleaned: " + screensPageCleaned);
                //    string[] screenshots = Helpers.MatchExpressionReturnAll(screensPageCleaned, "<img.src=\"(?<ScreenShots>.*?)\"");
                //    //MessageBox.Show("Step 4");
                //    if (screenshots != null)
                //    {
                //        //MessageBox.Show("found screenshots!");
                //        int i = 1;
                //        string CombinedStr = "|";
                //        foreach (string screenshot in screenshots)
                //        {
                //            Helpers.UpdateProgress("Games", "Downloading screenshot " + Convert.ToString(i) + " for " + ItemTitle + "...",  Item);
                //            //MessageBox.Show("ScreenShot: " + screenshot);
                //            byte[] imageData = TrytoDownloadData(screenshot, "Games", "AMG", Item );

                //            #region Save Image
                //            //MessageBox.Show(Importer.AlbumCoverFolder);

                //            DirectoryInfo di = Directory.CreateDirectory(filepath + "\\" + "ScreenShots");

                //            string imagefile = CoverDownloaders.SaveImage(imageData, Item, Convert.ToString(i) + ".jpg", Importer.WhereToSaveAlbumCover, Importer.AlbumCoverFolder, false, ItemName, filepath + "\\" + "ScreenShots", "ScreenShots", "");

                //            if (!String.IsNullOrEmpty(imagefile))
                //            {
                //                CombinedStr = CombinedStr + imagefile + "|";
                //                Helpers.UpdateProgress("Games", "ScreenShot " + Convert.ToString(i) + " was downloaded.",  Item);
                //                Thread.Sleep(500);

                //            }
                //            #endregion

                //            i++;
                //        }
                //        Item.Tags["ScreenShots"] = CombinedStr;
                //        Item.SaveTags();

                //    }
                //    #endregion
                //}
                #endregion

            }
            catch (Exception e)
            {
                Debugger.LogMessageToFile("An unexpected error occured in the Game details downloader. The error was: " + e.ToString() );
                StatusForm.statusForm.TrayIcon.ShowBalloonTip(10000,"Error downloading Game information","MediaFairy encountered an error while trying to download game information. Please see Debug.log for more details on this error.",ToolTipIcon.Warning);
                //MessageBox.Show(e.ToString());
            }


        }
    }
}
