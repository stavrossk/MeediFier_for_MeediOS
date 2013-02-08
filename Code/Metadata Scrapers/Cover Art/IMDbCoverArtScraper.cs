using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using MeediOS;

namespace MediaFairy.Code.Metadata_Scrapers.Cover_Art
{


    class IMDbCoverArtScraper
    {





        internal static string GetFilmPosterFromIMDb(IMLItem item)
        {


            var imdbUrl = SynthesizeUrlToParse(item);

      
            Helpers.UpdateProgress
                ("Updating Movies Section",
                "Searching IMDb for poster...", item);


            try
            {




                byte[] htmlData = Downloaders.TrytoDownloadData
                    (imdbUrl, "Movies", "IMDb", item);


                if (htmlData == null)
                    return String.Empty;


                var coverImageLink
                    = ParseMainPageReturnCoverImagePage
                    (htmlData);


                if (String.IsNullOrEmpty
                    (coverImageLink))
                    return String.Empty;






                WebResponse response
                    = Downloaders
                    .TryDatabaseRequest
                    (coverImageLink, "Movies", 
                    "IMDb", item);



                if (response == null)
                    return String.Empty;


                string resultStream 
                    = String.Empty;

                using (var streamReader
                    = new StreamReader
                    (response.GetResponseStream()))
                {


                    resultStream
                        = streamReader.ReadToEnd();

                    // Close and clean up the StreamReader
                    streamReader.Close();

                }




                var posterUrl = ParseCoverImagePageReturnCoverImageUrl
                    (resultStream);


                return posterUrl;



            }
            catch (Exception e)
            {


                Debugger.LogMessageToFile
                    ("An unexpected error occured " +
                     "in the IMDb film poster downloader." +
                     " The error was: " 
                     + Environment.NewLine 
                     + e);


                return String.Empty;


            }


        }
















        private static string ParseCoverImagePageReturnCoverImageUrl
            (string resultStream)
        {



            var r = new Regex
                ("<img oncontextmenu[^>]*src=" +
                 "\"([^\"]*)\"[^>]*>");


            MatchCollection mclb
                = r.Matches
                    (resultStream);


            var coverImageList
                = new ArrayList();


            foreach (Match ml in mclb)
            {
                foreach (Group g in ml.Groups)
                {
                    string b = g.Value + "";
                    // Add the extracted urls to the array list
                    coverImageList.Add(b);

                    Debugger.LogMessageToFile
                        ("b: " + b);
                }
            }



            var posterUrl
                = (string) coverImageList[1];


            return posterUrl;
        }






        private static string ParseMainPageReturnCoverImagePage
            (byte[] htmlData)
        {


            Encoding utf8 = new UTF8Encoding();

            string htmlDataString
                = utf8.GetString
                    (htmlData);


            var rb = new Regex
                ("href\\s*=\\s*(?:\"" +
                 "(?<1>[^\"]*)\"|(?<1>\\S+))");


            MatchCollection mcl
                = rb.Matches(htmlDataString);


            ImageFileConstructor.a.Clear();

            foreach (Match ml in mcl)
            {
                foreach (Group g in ml.Groups)
                {
                    string b = g.Value + "";
                    // Add the extracted urls to the array list
                    ImageFileConstructor.a.Add(b);
                }
            }


            string coverImageLink
                = string.Empty;


            for (int i = 1; i < ImageFileConstructor.a.Count; i = i + 2)
            {
                var tmp = (string) ImageFileConstructor.a[i];


                if (!tmp.Contains
                         ("action-box-title/primary-photo"))
                    continue;


                tmp = tmp.Remove(0, 35);

                const string imdbmain
                    = "http://www.imdb.com/";


                coverImageLink
                    = imdbmain.Insert
                        (imdbmain.Length, tmp);
            }



            return coverImageLink;
        }






        private static string SynthesizeUrlToParse(IMLItem item)
        {


            string imdbid
                = Helpers.GetTagValueFromItem
                    (item, "ImdbID");

            if (!imdbid.StartsWith("tt"))
                imdbid = "tt" + imdbid;

            const string prefix
                = "http://www.imdb.com/title/";


            string imdbUrl = prefix + imdbid;

            return imdbUrl;


        }










    }



}
