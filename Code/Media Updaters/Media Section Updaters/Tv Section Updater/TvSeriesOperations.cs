//'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
//'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
//''    MediaFairy                                                               ''
//''    Copyright (C) 2008-2010  Stavros Skamagkis                               ''
//''                                                                             ''
//''    This program is free software: you can redistribute it and/or modify     ''
//''    it under the terms of the GNU General Public License as published by     ''
//''    the Free Software Foundation, either version 3 of the License, or        ''
//''    (at your option) any later version.                                      ''
//''                                                                             ''
//''    This program is distributed in the hope that it will be useful,          ''
//''    but WITHOUT ANY WARRANTY; without even the implied warranty of           ''
//''    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the            ''
//''    GNU General Public License for more details.                             ''
//''                                                                             ''
//''    You should have received a copy of the GNU General Public License        ''
//''    along with this program.  If not, see <http://www.gnu.org/licenses/>.    ''
//'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
//'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''



using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using MediaFairy.ImportingEngine;
using TVDBLibrary;
using System.Threading;

#if USE_MEEDIO 
using Meedio; 
#elif USE_MEEDIOS
using MeediOS;
#endif


namespace MediaFairy
{
    class TvSeriesOperations
    {


        public static string IdentifyEpisode(IMLItem item, ConnectionResult connectionresult,
            bool fileServerIsOnline, bool isUNC, string location, string parent)
        {

            CalculateVideoHash(item, fileServerIsOnline, isUNC, location);

            //Helpers.ReadXmlForEpisode(Item, moviehash, parent );

            string videoHash = Helpers.GetTagValueFromItem(item, "Hash");
            string imdbID = Helpers.GetTagValueFromItem(item, "ImdbID");

            //imdbID = GetIMDbIdByVideoHash(item, connectionresult, imdbID, videoHash);

            return imdbID;

        }



        private static string GetIMDbIdByVideoHash(IMLItem item,
            ConnectionResult connectionresult, 
            string imdbid, string moviehash)
        {
            if (!connectionresult.OSDbIsOnline)
                return imdbid;

            if (String.IsNullOrEmpty(moviehash))
                return imdbid;

            if (!String.IsNullOrEmpty(imdbid))
                return imdbid;
            
            imdbid = OSoperations.FindImdbIDbyHash2
                (moviehash, item, connectionresult.OsDbLoginResult.token,
                 ref connectionresult.OSDbIsOnline);

            item.Tags["ImdbID"] = imdbid;
            item.SaveTags();

            return imdbid;
        }



        internal static void CalculateVideoHash(IMLItem item,
            bool fileServerIsOnline, bool isUNC, string location)
        {

            string videoHash = 
                Helpers.GetTagValueFromItem(item, "Hash");


            if (!Settings.EnableVideoHasher)
                return;

            if (!String.IsNullOrEmpty(videoHash))
                return;

            if (isUNC && !fileServerIsOnline)
                return;

            if (!File.Exists(location))
            {
                MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress,
                "Unable to identify video. The video file in location field was not found.");
                Thread.Sleep(2000);

                return;
            }


            MainImportingEngine.ThisProgress.Progress
                (MainImportingEngine.CurrentProgress,
            "Computing video fingerprint for " + item.Name + "...");


            videoHash = Hasher.ComputeHash(location, item);


            item.Tags["Hash"] = videoHash;
            item.SaveTags();


            return;
        }



        public static bool DownloadSeriesBanner(IMLItem Item, string BannerType, int SeasonNumber, string SeasonID, string ImageFile)
        {
            string posterUrl = "";
            byte[] imageData;


            try
            {

                #region define local variables
                TVDBLib tvdb = new TVDBLib();

                string SeriesID = Helpers.GetTagValueFromItem(Item,"SeriesID");

                #endregion


                string banner = Helpers.GetTagValueFromItem(Item, BannerType);
                #region check if image file in item's location exists
                if (!String.IsNullOrEmpty(banner))
                {
                    if (File.Exists(banner))
                        return true;
                }
                #endregion

                Application.DoEvents();


                #region look for backdrop in item's location and save to image tag
                if (File.Exists(ImageFile))
                {
                    Item.Tags[BannerType] = ImageFile;
                    Item.SaveTags();
                    return true;
                }
                #endregion


                //MessageBox.Show("Step 6");

                //MessageBox.Show(SeriesID);
                #region request and return image URLs

                IList<TVDBLibrary.ITVDBBanner> banners;
               

                try
                {
                    Helpers.UpdateProgress("Updating TV Section", "Searching TVdb for " + BannerType + " for '" + Item.Name + "'...", Item);
                    //MessageBox.Show("Step 7");

                    if (BannerType == "SeriesBanner")
                        banners = tvdb.GetSeriesBanners(SeriesID);
                    else if (BannerType == "Fanart")
                        banners = tvdb.GetSeriesFanart(SeriesID);
                    else banners = tvdb.GetSeasonImagesPoster(SeriesID, SeasonNumber);

                    //if (BannerType == "SeriesPoster")
                                           

                }
                catch (Exception)
                {
                    try
                    {
                        Helpers.UpdateProgress("Updating TV Section", "TVdb did not respond. Retrying...",  Item);

                        if (BannerType == "SeriesBanner")
                            banners = tvdb.GetSeriesBanners(SeriesID);
                        else if (BannerType == "Fanart")
                            banners = tvdb.GetSeriesFanart(SeriesID);
                        else banners = tvdb.GetSeasonImagesPoster(SeriesID, SeasonNumber);

                        //if (BannerType == "SeriesPoster")

                    }
                    catch
                    {
                        return false;
                    }

                }

                //MessageBox.Show("Step 8");

                #region cancel operation if no backdrop found
                if (banners.Count == 0)
                {
                    Item.Tags["LastUpdated"] = DateTime.Today.Date.ToString();
                    Item.SaveTags();
                    //Importer.thisProgress.Progress(Importer.CurrentProgress, "backdrop not found." + MovieName + ".");
                    //Thread.Sleep(500);
                    return false;
                }
                #endregion

                #endregion


                posterUrl = UI.UserSelectsSeriesBanner(Item, banners, BannerType );


                #region Download, save and import backdrop

                #region download the image
                Helpers.UpdateProgress("Updating TV Section", "Downloading Series " + BannerType + " from TVdb...",  Item);
                imageData = Downloaders.TrytoDownloadData(posterUrl, "TV", "TVdb", Item );

                if (imageData == null)
                    return false;

                #endregion

                #region save the image & update tags
                //MessageBox.Show("filepath: " + filepath);

                bool IsPoster = false;
                if (BannerType == "SeriesBanner")
                    IsPoster = true;

                if (BannerType == "backdrop")
                    BannerType = "Fanart";

                if ( ImageFileConstructor.SaveImage(imageData, Item, IsPoster, BannerType,ImageFile) != "")
                {
                    return true;
                }
                else return false;
                #endregion

                #endregion


            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                return false;
            }


        }



    }
}
