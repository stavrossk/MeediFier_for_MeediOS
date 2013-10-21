using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using System.Xml;
using MeediFier.Code.Downloaders;
using MeediFier.Code.User_Interface;
using MeediFier.ImportingEngine;
using MeediOS;
using MetaDataFolders;





namespace MeediFier.Code.Metadata_Scrapers.Cover_Art
{




    class TMDbFilmBackdropDownloader
    {





        internal static bool DownloadBackdropFromTheMovieDb
            (IMLItem item, string videofilename, 
             string location, string itemTitle,
             string parent, bool fileServerIsOnline,
             bool isUNC,string mdfSettingsa, 
             IBaseSystem ibs)
        {


            try
            {


                #region return cases



                if (MeediFier.Settings.WantToDisableSources
                    && !MeediFier.Settings.TMDbIsOnline)
                    return false;

                if (!MeediFier.Settings.FilmBackdropArtDownloaderIsEnabled)
                    return false;

                if (isUNC && !fileServerIsOnline)
                    return false;
                #endregion



                #region set item variables
               


                string imdbid = Helpers.GetTagValueFromItem(item, "ImdbID");



                if (imdbid.Length == 8 && imdbid.StartsWith("tt"))
                {
                    imdbid = imdbid.Remove(0,2);
                    imdbid = "0" + imdbid;
                    imdbid = "tt" + imdbid;
                    item.Tags["ImdbID"] = imdbid;
                    item.SaveTags();
                }


                string fanart 
                    = Helpers.GetTagValueFromItem
                    (item, "fanart");

                string lastUpdated
                    = Helpers.GetTagValueFromItem
                    (item, "LastUpdated");



                #endregion


                #region check if image file in item's location exists
                if (!String.IsNullOrEmpty(fanart))
                {
                    if (File.Exists(fanart))
                        return true;
                }

                #endregion


                Application.DoEvents();


                const string ext = ".jpg";

                string imageFile = Movies.CreateFanartFilePath
                    (item, mdfSettingsa, ext, ibs);


                #region look for backdrop in item's location and save to image tag


                if (File.Exists(imageFile))
                {
                    item.Tags["fanart"] = imageFile;
                    item.SaveTags();
                    return true;

                }

                #endregion

                #region Check Last Updated date
                if (!String.IsNullOrEmpty(lastUpdated))
                {

                    DateTime todayDate = DateTime.Today;


                    DateTime updatedDate 
                        = DateTime.Parse
                        (Helpers.GetTagValueFromItem
                        (item, "LastUpdated"));


                    if (todayDate.DayOfYear
                        < (updatedDate.DayOfYear + 2))
                        return false;


                }
                #endregion



                var previewImages = new List<string>();

                List<string> imageUrls
                    = GetFilmBackdropsFromTMDb
                    (item, ref previewImages);


                if (imageUrls.Count == 0 || previewImages.Count == 0)
                {

                    Debugger.LogMessageToFile
                        ("The image URLs list is empty. " +
                         "No online backdrops were found for this film.");
                
                    return false;
                
                }

                string posterUrl
                    = ImageSelectionEngine.UserSelectsImage
                    (item, imageUrls, 
                    previewImages,"backdrop");


                #region Download, save and import backdrop

                #region download the image

                Helpers.UpdateProgress
                    ("Updating Movies Section",
                    "Downloading backdrop " +
                    "from themoviedb.org...", item);



                byte[] imageData
                    = MeediFier.Downloaders.TrytoDownloadData
                    (posterUrl, "Movies",
                    "TheMovieDatabase", item);



                if (imageData == null)
                    return false;




                #endregion

                #region save the image & update tags

                if ( String.IsNullOrEmpty
                    (ImageFileConstructor.SaveImage
                    (imageData, item, false,
                    "fanart", imageFile)))
                    return false;


                if (MeediFier.Settings.ResizeFilmBackdrops)
                {


                    Helpers.UpdateProgress
                        ("Updating Movies Section",
                        "Resizing image...", item);


                    ImageResizer.ResizeImage
                        (imageFile, imageFile, 
                         MeediFier.Settings.FilmBackdropWidth,
                         MeediFier.Settings.FilmBackdropHeight, false);


                }
                return true;

                #endregion


                #endregion

            }
            catch (Exception e)
            {
                Debugger.LogMessageToFile
                    ("The backdrops downloading method" +
                     " returned an unexpected error: " + e);

                return false;
            }


        }


        public static List<string> GetFilmBackdropsFromTMDb(IMLItem item, ref List<string> thumbUrls)
        {
            List<string> urlList = new List<string>();

            if (!MeediFier.Settings.FilmBackdropArtDownloaderIsEnabled)
                return urlList;

            #region Contruct URL
            string tmdbID = Helpers.GetTagValueFromItem(item, "TMDbID");

            //string tmDbUrl = "http://api.themoviedb.org/2.1/Movie.getInfo/en/xml/" + Settings.TMDbApiKey + "/" + tmdbID;

            string tmDbUrl = "http://api.themoviedb.org/3/movie/" + tmdbID + "/images?api_key=" + MeediFier.Settings.TMDbApiKey;
            #endregion


            string resultJson = HtmlDownloaders.DownloadHTMLfromURL(tmDbUrl);

            JavaScriptSerializer serializer = new JavaScriptSerializer();

            var foo = serializer.Deserialize<Foo>(resultJson);


            foreach (var subObject in foo.objects)
            {

                string objName = subObject.attributes.OBJECT_NAME;
                MessageBox.Show(objName);

                string objType = subObject.attributes.OBJECT_TYPE;
                MessageBox.Show(objType);

            }



            Debugger.LogMessageToFile(resultJson);

            #region request data stream
            Debugger.LogMessageToFile("Searching themoviedb.org for film fanart images...");
            Helpers.UpdateProgress(MainImportingEngine.GeneralStatus, "Searcing themoviedb.org for film fanart images...", item);

            WebResponse objResponse = MeediFier.Downloaders.TryDatabaseRequest(tmDbUrl, "Movies", "TheMovieDatabase", item);
            if (objResponse == null)
            {
                Debugger.LogMessageToFile("The web response from TMdb was not valid. Unable to get backdrops for this film.");
                return urlList;
            }
            Debugger.LogMessageToFile("Getting Response stream...");
            Stream stream = objResponse.GetResponseStream();
            #endregion


           

            #region Try to load XML stream
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(stream);
            }
            catch (Exception e)
            {
                Debugger.LogMessageToFile("An error occurred while trying to load TMDb's XML stream: " + e.ToString());
                return urlList;
            }
            #endregion


            XmlNodeList nodelist = doc.ChildNodes;
            bool FoundFirst = false;

            #region Parse node tree and get film details

            foreach (XmlNode node in nodelist)
            {
                //MessageBox.Show("First level: " + node.Name);
                if (node.HasChildNodes)
                {
                    foreach (XmlNode childnode in node.ChildNodes)
                    {
                        //MessageBox.Show("Second level: " + node.Name);
                        foreach (XmlNode result in node.ChildNodes)
                        {
                            //MessageBox.Show("Third level: " + result.Name);
                            foreach (XmlNode unknown in result.ChildNodes)
                            {
                                //MessageBox.Show("Fourth level: " + unknown.Name);
                                if (unknown.Name == "movie" && !FoundFirst)
                                {
                                    FoundFirst = true;
                                    //MessageBox.Show("Fifth level: " + unknown.Name);

                                    foreach (XmlNode tag in unknown.ChildNodes)
                                    {
                                        if (tag.Name == "backdrop")
                                        {
                                            XmlAttributeCollection atrributes = tag.Attributes;
                                            foreach (XmlAttribute attribute in atrributes)
                                            {
                                                if (attribute.Name == "size")
                                                {
                                                    if (attribute.Value == "original")
                                                    {

                                                        //if (Helpers.RemoteFileExists(tag.InnerText))
                                                        //{
                                                        //  MessageBox.Show("Remote image exists!");
                                                        urlList.Add(tag.InnerText);
                                                        //}
                                                        //else
                                                        //{
                                                        //    MessageBox.Show("Remote image does not exist");
                                                        //}
                                               
                                                    }
                                                    if (attribute.Value == "mid")
                                                    {

                                                        //if (Helpers.RemoteFileExists(tag.InnerText))
                                                        //{
                                                        //    MessageBox.Show("Remote image exists!");
                                                        thumbUrls.Add(tag.InnerText);
                                                        //}
                                                        //else
                                                        //{
                                                        //    MessageBox.Show("Remote image does not exist");
                                                        //}
                                                       
                                                    }

                                                }
                                            }
                                        }




                                    }

                                }
                            }
                        }
                    }
                }
            }
            #endregion

            return urlList;

        }
    }




}
