using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using MeediOS;
using MetaDataFolders;





namespace MediaFairy.Code.Metadata_Scrapers.Cover_Art
{



    class ImdbFilmCoverArtScraperOld
    {







        internal static bool DownloadFilmCoverArtFromImdbOld
            (IMLItem item, string videoFilename,
             string location, string movieName,
             string mdfSettingsa, IBaseSystem ibs)
        {



            string imdbid = Helpers.GetTagValueFromItem
                (item, "ImdbID");




            #region Synthesize the url to parse


            if (!imdbid.StartsWith("tt"))
                imdbid = "tt" + imdbid;


            const string prefix = "http://www.imdb.com/title/";

            string imdbUrl = prefix + imdbid;

            #endregion


            Helpers.UpdateProgress
                ("Updating Movies Section",
                 "Searching IMDb for poster...",
                 item);



            byte[] imageData
                = Downloaders.TrytoDownloadData
                    (imdbUrl, "Movies",
                     "IMDb", item);



            if (imageData == null)
                return false;


            #region Parse imdb_url and return Poster image

            Encoding utf8 = new UTF8Encoding();


            string imageDataString = 
                utf8.GetString(imageData);


            var rb = new Regex
                ("href\\s*=\\s*(?:\"" +
                 "(?<1>[^\"]*)\"|(?<1>\\S+))");


            MatchCollection mcl = rb.Matches(imageDataString);

            CoverDownloaders.a.Clear();

            foreach (Match ml in mcl)
            {
                foreach (Group g in ml.Groups)
                {
                    string b = g.Value + "";
                    // Add the extracted urls to the array list
                    CoverDownloaders.a.Add(b);
                }
            }

            string coverImageLink = String.Empty;

            for (int i = 1; i < CoverDownloaders.a.Count; i = i + 2)
            {

                string tmp = (string)CoverDownloaders.a[i];


                if (!tmp.Contains
                         ("action-box-title" +
                          "/primary-photo"))
                    continue;


                tmp = tmp.Remove(0, 35);

                const string imdbmain
                    = "http://www.imdb.com/";

                coverImageLink 
                    = imdbmain.Insert
                        (imdbmain.Length,
                         tmp);


            }





            if (String.IsNullOrEmpty(coverImageLink))
                return false;


            WebResponse response
                = Downloaders.TryDatabaseRequest
                    (coverImageLink, "Movies",
                     "IMDb", item);




            if (response == null)
                return false;

            string resultStream = String.Empty;

            using (var streamReader = new StreamReader
                (response.GetResponseStream()))
            {


                resultStream = streamReader.ReadToEnd();
                // Close and clean up the StreamReader
                streamReader.Close();


            }





            var regex = new Regex
                ("<img oncontextmenu" +
                 "[^>]*src=\"([^\"]*)\"[^>]*>");


            MatchCollection mclb = regex.Matches
                (resultStream);


            CoverDownloaders.c.Clear();



            foreach (Match ml in mclb)
            {


                foreach (Group g in ml.Groups)
                {
                    string b = g.Value + "";
                    // Add the extracted urls to the array list
                    CoverDownloaders.c.Add(b);

                    Debugger.LogMessageToFile("b: " + b);
                }


            }




            string posterUrl = (string)CoverDownloaders.c[1];

            #endregion


            bool downloadImage;



            downloadImage = 
                Settings.AlwaysPromptForImages
                || UI.PreviewImage
                       (posterUrl, false);


            if (downloadImage)
            {

                Helpers.UpdateProgress
                    ("Updating Movies Section...", 
                     "Downloading Poster from IMDb...", 
                     item);


                item.SaveTags();

                imageData = Downloaders.TrytoDownloadData
                    (posterUrl, "Movies",
                     "IMDb", item);



                if (imageData == null)
                    return false;




                const string imageExtension = ".jpg";

                string imageFile 
                    = Movies.CreatePosterFilePath
                        (item, mdfSettingsa,
                         imageExtension, ibs);



                if (CoverDownloaders.SaveImage
                        (imageData, item, true,
                         "", imageFile) != "")
                {


                    item.Tags["HasPoster"] = "true";
                    item.SaveTags();
                    return true;


                }
                return false;



            }


            return false;
        }
    }
}
