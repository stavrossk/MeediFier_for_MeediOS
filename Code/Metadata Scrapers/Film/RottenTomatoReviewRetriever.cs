using System;
using System.Windows.Forms;
using MeediOS;


namespace MediaFairy.Code.Metadata_Scrapers.Film
{




    class RottenTomatoReviewRetriever
    {




        internal static void GetRottenTomatoReview
            (IMLItem Item)
        {


            string rTreview = Helpers.GetTagValueFromItem
                (Item, "RTreview");



            if (!String.IsNullOrEmpty(rTreview))
                return;
            

            string title = Helpers.GetTagValueFromItem
                (Item, "Title");



            string TitleNormalized = title.Replace(" ", "%20");


            MessageBox.Show
                ("Normalized Title: "
                 + TitleNormalized);


            string searchURL 
                = "http://www.rottentomatoes.com/search/full_search.php?search=" 
                  + TitleNormalized;


            const string resultPattern 
                = "class=\"firstCol title\">.*?<p>.*?" +
                  "<a href=\"/m/(?<RTTitle>.*?)/\"";


            MessageBox.Show
                ("SearchURL: " + searchURL);


            Helpers.UpdateProgress
                ("Updating Movies section...",
                 "Searching RottenTomatoes " +
                 "for film review score...", Item);



            string rtTitle 
                = Downloaders.MineWebDataReturnFirstMatch
                    (searchURL, resultPattern);


            string URL = "http://www.rottentomatoes.com/m/" + rtTitle + "/";


            MessageBox.Show("URL: " + URL);


            const string RTReviewPattern = "<ul id=\"tomatometer_nav\".*?" +
                                           "<.*?><a title=\".*?(?<RTreview>.*?)\"";


            Helpers.UpdateProgress
                ("Updating Movies section...",
                 "Downloading Rotten Tomatoes" +
                 " review score...",Item);


            rTreview = Downloaders.MineWebDataReturnFirstMatch
                (URL, RTReviewPattern);


            MessageBox.Show
                ("RTreview: " + rTreview);


            Item.Tags["RTreview"] = rTreview;
            Item.SaveTags();


        }
    }




}
