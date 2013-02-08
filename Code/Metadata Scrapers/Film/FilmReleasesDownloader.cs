using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using MediaFairy.IMDb;
using MediaFairy.ImportingEngine;
using MeediOS;





namespace MediaFairy.Code.Metadata_Scrapers.Film
{


    class FilmReleasesDownloader
    {


        private static void DownloadNewReleasesFromSource
            (string RssSource, IMLSection MediaSection,
            IMLSection ReleasesSection,
            ConnectionResult connResult, 
            IEnumerable<string> combinedSceneTags)
        {




            WebClient client = new WebClient();
            XmlDocument xDoc = new XmlDocument();
            //Importer.thisProgress.Progress(Importer.CurrentProgress, "Scanning for new releases...");
            Helpers.UpdateProgress("Updating Downloads library" + MediaSection.Name + "Section", "Scanning for new releases..." , null);
            //WebResponse response = Helpers.TryDatabaseRequest(RssSource,MediaSection.Name, "Mininova",null );

            string xmlData = Downloaders.GetUrl(RssSource, null, true);
            if (String.IsNullOrEmpty(xmlData))
                return;

            StringReader reader = new StringReader(xmlData);

            //if (response == null)
            //{
            //MessageBox.Show("Unable to communicate with online source");
            //  return false;
            //}

            //Stream stream = response.GetResponseStream();

            #region try to load online xml stream
            try
            {
                xDoc.Load(reader);
            }
            catch (Exception e)
            {
                MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress, "An error occured while reading the rss document. Please try later.");
                Thread.Sleep(1500);
                MessageBox.Show(e.ToString());

                try
                {
                    xDoc.Load(reader);
                }
                catch (Exception)
                {
                    MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress, "An error occured while reading the rss document. Please try later.");
                    Thread.Sleep(1500);
                    return;
                }

            }
            #endregion


            //MessageBox.Show(xDoc.InnerXml);

            XmlNodeList torrents = xDoc.GetElementsByTagName("item");

            foreach (XmlNode torrent in torrents)
            {

                XmlNodeList tags = torrent.ChildNodes;
                string title = "";
                string year = "";
                string seeds = "";
                string leechers = "";
                string link = "";

                #region loop torrent tags and retrieve their values
                foreach (XmlNode tag in tags)
                {
                    #region retrieve url
                    if (tag.Name == "enclosure")
                    {
                        XmlAttributeCollection attributes = tag.Attributes;
                        foreach (XmlAttribute attribute in attributes)
                        {
                            if (attribute.Name == "url")
                            {
                                //MessageBox.Show("attrib innertext: " + attribute.InnerText);
                                link = attribute.InnerText;
                            }


                        }

                    }
                    #endregion

                    if (tag.Name == "title")
                    {
                        //MessageBox.Show(tag.InnerText);

                        #region parse release name and return film information
                        Regex regex = new Regex(".*?(?<Title>.*?)\\[(?<Year>.*?)\\].*?\\((?<Seeds>.*?)S/(?<Leechers>.*?)L");
                        MatchCollection matches = regex.Matches(tag.InnerText);

                        if (matches.Count == 0)
                        {
                            regex = new Regex("(?<Title>.*?)\\z");
                            matches = regex.Matches(tag.InnerText);

                            if (matches.Count == 0)
                                continue;
                        }


                        Match match = matches[0];

                        #region mine data from match

                        int i = 0;
                        foreach (Group group in match.Groups)
                        {
                            //Console.WriteLine(i);

                            #region retrieve item tags from regex captures
                            foreach (Capture captrue in @group.Captures)
                            {


                                #region extract title
                                if (i == 1) //this is the image's url
                                {
                                    title = captrue.Value;
                                    //MessageBox.Show("Title: " + captrue.Value);
                                }
                                #endregion


                                #region extract year
                                if (i == 2)
                                {
                                    year = captrue.Value;
                                    //MessageBox.Show("Year: " + captrue.Value);
                                }
                                #endregion

                                #region extract Seeds
                                if (i == 3)
                                {
                                    seeds = captrue.Value;
                                    //MessageBox.Show("Seeds: " + captrue.Value);
                                }
                                #endregion

                                #region extract Leechers
                                if (i == 4)
                                {
                                    leechers = captrue.Value;
                                    //MessageBox.Show("Leechers: " + captrue.Value);
                                }
                                #endregion


                            }
                            #endregion

                            i++;
                        }
                        #endregion


                        #endregion

                    }

                }
                #endregion

                #region check if torrent already exists
                IMLItemList items = ReleasesSection.SearchByLocation(link);
                if (items.Count != 0)
                    continue;
                #endregion

                #region add & identify films
                ReleasesSection.BeginUpdate();
                #region create new torrent item
                //MessageBox.Show(link);
                IMLItem Item = ReleasesSection.AddNewItem(title, link);
                Item.Tags["Title"] = title;
                Item.Tags["Year"] = year;
                Item.Tags["Seeds"] = seeds;
                Item.Tags["Peers"] = leechers;
                string imdbid = Helpers.GetTagValueFromItem(Item,"IMDbID");
                Item.SaveTags();
                #endregion

                //MessageBox.Show("Before: " + Item.Name);
                MovieFileMetadataExtractor.ExtractMetadataFromMovieFilename(Item, ref imdbid, combinedSceneTags);
                //MessageBox.Show("After: " + Item.Name);
                Item.Name = Item.Name.Replace('.', ' ');
                Item.Tags["Title"] = Item.Name;
                Item.SaveTags();


                if (Settings.FilmRatingMin != 0)
                {
                    IMDbOperations imdbOP = new IMDbOperations();
                    imdbOP._assistType = IMDbOperations.AssistType.Never;
                    connResult.OSDbIsOnline = false;
                    string tmp = "";
                    Helpers.IdentifyVideo(ref imdbid, ref tmp, ref imdbOP, Item, true, false, "", "", year , connResult, ReleasesSection);
                    tmp = null;


                    #region IMDb Detailer

                    string HasDetails = Helpers.GetTagValueFromItem(Item,"HasDetails");
                    if (HasDetails != "True")
                    {
                        ////MessageBox.Show("About to d/load film details from IMDB...");



                        #region ...now get film details using this iMDBiD...
                        //MessageBox.Show("imdbid before d/loading details is: " + imdbid);
                        //MessageBox.Show("imdbOP._imdbID before d/loading details is: " + imdbOP._imdbID);
                        if (imdbOP.ImdbId != "") imdbid = imdbOP.ImdbId;
                        if (imdbid != "") imdbOP.ImdbId = imdbid;


                        imdbOP.ImdbMovie = null;
                        if (!String.IsNullOrEmpty(imdbOP.ImdbId))
                        {
                            imdbid = imdbOP.ImdbId;
                            //imdbOP._imdbID = imdbid;
                            //////MessageBox.Show("imdbID before getting details is: " + imdbid );
                            //////MessageBox.Show("IMDbOP.imdbid is: " + imdbOP._imdbID);
                            #region ...get the film's details.
                            if (!imdbid.StartsWith("tt"))
                                imdbid = "tt" + imdbid;

                            // _progressText = "Grabbing IMDb ID: " + imdbOP._imdbID;
                            MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress, "Downloading details from IMDb for " + imdbOP.ImdbId + "...");
                            imdbOP.GetIMDbMovieDetails(imdbid, true);
                            #endregion
                        }
                        #endregion


                        #region ...finally if the movie struct is filled, save the film details in the item's fields.
                        if (!String.IsNullOrEmpty(imdbid))
                        {
                            //string Episode = imdbOP._imdbMovie.Episode;

                            //MessageBox.Show(Episode);

                            //MessageBox.Show("title before saving: " + imdbOP._imdbMovie.Title);



                            //MessageBox.Show("... movie struct is filled, about to save details in tags...");
                            imdbOP.SaveIMDbMovieDetails(Item, ReleasesSection , imdbOP);


                            if (Item.Name.StartsWith("\""))
                            {
                                MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress, "This item is a TV episode.");
                                Thread.Sleep(1000);

                                MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress, "Searching for episode information...");


                                imdbOP.SaveEpisodeInfo(imdbid, Item );


                            }



                            //////MessageBox.Show("Saved movie details. continuing to cover art...");
                            //Item.Tags["HasDetails"] = "True";
                            //Item.SaveTags();
                        }
                        #endregion

                    }

                    #endregion
                }
                ReleasesSection.EndUpdate();


                #endregion

                #region Get torrent destination
                string location = Item.Location;
                string destination = "";

                if (String.IsNullOrEmpty(Settings.TorrentsFolder))
                    destination = Debugger.GetPluginPath() + "test.torrent";
                else
                {
                    destination = Settings.TorrentsFolder + title + ".torrent";
                }
                #endregion

                #region check if Title already exists in Releases section
                string Title = Helpers.GetTagValueFromItem(Item,"Title");
                IMLItemList itemsb = ReleasesSection.SearchByTag("Title", Title);
                if (itemsb.Count > 1)
                    continue;
                #endregion

                #region check if Title already exists in Movies section
                IMLItemList itemsc = MediaSection.SearchByTag("Title", Title);
                if (itemsc.Count != 0)
                    continue;
                #endregion

                #region Donwload the torrent
                double review = Convert.ToDouble(Helpers.GetTagValueFromItem(Item,"Review"));

                if (review > Settings.FilmRatingMin || Settings.FilmRatingMin == 0)
                {
                    //MessageBox.Show("destination: " + destination);
                    //MessageBox.Show("location: " + location);
                    #region Download torrent
                    try
                    {
                        client.DownloadFile(location, destination);
                    }
                    catch (Exception e)
                    {
                        Debugger.LogMessageToFile("[RSS torrent downloader] The torrent '" + location + "' could not be downloaded. The erros was: " + e.ToString() );
                    }
                    #endregion

                    #region Start torrent
                    //if (String.IsNullOrEmpty(Importer.TorrentsFolder))
                    //{
                    #region start torrent directly
                    Process p = new Process();
                    p.StartInfo.FileName = destination;

                    p.Start();

                    //while (!p.HasExited)
                    //{
                    //    Application.DoEvents();
                    //    Thread.Sleep(50);
                    //}
                    Thread.Sleep(5000);

                    try
                    {
                        File.Delete(destination);
                    }
                    catch { }
                    #endregion
                    //}
                    #endregion

                }
                #endregion

            }


        }






        internal static void DownloadFilmReleases
            (string pluginpath, ConnectionResult connectionresult,
             IEnumerable<string> combinedSceneTags,
             bool SelectiveUpdatingEnabled)
        {


            if (!Settings.DownloadFilmReleases 
                || SelectiveUpdatingEnabled 
                ||
                (!connectionresult.InternetConnectionAvailable 
                 && Settings.ConnectionDiagnosticsEnabled))
                return;


            Debugger.LogMessageToFile("RSS film releases downloader is enabled.");
            Debugger.LogMessageToFile("Allocating Releases section...");
            IMLSection ReleasesSection = MainImportingEngine.Library.FindSection(Settings.ReleasesLibrary, true);
            Debugger.LogMessageToFile("Reading RSS sources file...");
            Settings.FilmsRssFeeds = File.ReadAllLines(pluginpath + "RSS Sources.txt");

            foreach (string rssSource in Settings.FilmsRssFeeds)
            {
                Debugger.LogMessageToFile("Starting to download film releases from source " + rssSource +
                                          "...");
                if (!String.IsNullOrEmpty(rssSource))
                    DownloadNewReleasesFromSource(rssSource, MediaSectionsAllocator.MoviesSection,
                                                                         ReleasesSection,
                                                                         connectionresult, combinedSceneTags);
            }


        }









    }




}
