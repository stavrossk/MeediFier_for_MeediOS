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
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Timers;
using System.Xml;
using System.Web;
using System.Web.Services.Protocols;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Net;
using System.Windows.Forms;

#if USE_MEEDIO
using Meedio;
#elif USE_MEEDIOS
using MediaFairy;
using MeediOS;
#endif


namespace MediaFairy
{
    class NetFlixAPI
    {
        public static void getMovieInfo(IMLItem Item)
        {
            string title = Helpers.GetTagValueFromItem(Item, "Title");
            MessageBox.Show("Title: " + title);

            Helpers.UpdateProgress("Updating Movies section", "Downloading film information from NetFlix...", Item);

            try
            {
                string url = "http://odata.netflix.com/Catalog/Titles?$filter=substringof('" + title + "', Name)";
                XmlNodeList xml = getXML(url);
                if (xml == null)
                    return;


                //foreach (XmlNode node in xml)
                //{
                //    result = getMovieResultInfo(node);
                //    if (result != null)
                //        list.Add(result);
                //}

                //XmlNode node = (XmlNodeList)xml;
                //XmlNode node = xml[0];

                //XmlNode[] array = (new System.Collections.Generic.List<XmlNode>(Shim<XmlNode>(xml))).ToArray();
                //XmlNode node = array[0];

                if (xml.Count > 0)
                {
                    foreach (XmlNode node in xml)
                    {
                        if (node.Name == "entry")
                        {
                            getMovieInformation(node, Item);
                            return;
                        }
                    }
                }

                //getMovieInformation(node, Item);

            }
            catch (Exception e)
            {
               Debugger.LogMessageToFile("Error getting xml: " + e);
            }
            return;
        }

        //public static IMovie getMovieById(string id)
        //{
            
        //    string url = "http://odata.netflix.com/Catalog/Titles()?$expand=Awards,Cast,Directors,Genres&$filter=NetflixApiId eq 'http://api.netflix.com/catalog/titles/movies/" + id + "'";
        //    NetFlixDetailSearch.MeedioUtils.LogEntry("Getting movie url: " + url);
        //    XmlNodeList xml = getXML(url);
        //    if (xml == null)
        //        return null;

        //    IMovie result = new MoviesData.Common.BaseMovie(string.Empty);
        //    if (xml.Count > 0)
        //    {
        //        foreach (XmlNode node in xml)
        //        {
        //            if (node.Name == "entry")
        //            {
        //                return getMovieInformation(node);
        //            }
        //        }
        //    }
        //    return null;
        //}

        //private static IMovie getMovieID(XmlNode movieNode)
        //{
        //    if (movieNode == null)
        //        return null;

        //    if (movieNode.ChildNodes.Count < 2 || movieNode.Name != "entry")
        //        return null;

        //    IMovie movie = new MoviesData.Common.BaseMovie(string.Empty);

        //    foreach (XmlNode node in movieNode.ChildNodes)
        //    {
        //        string value = node.InnerText;
        //        switch (node.Name)
        //        {
        //            case "d:NetflixApiId":
        //                string NetflixUrl = value;
        //                int StartIndex = NetflixUrl.LastIndexOf("/");
        //                movie.Id = NetflixUrl.Remove(0, StartIndex + 1);
        //                break;
        //        }
        //    }
        //    return movie;
        //}

        private static void getMovieInformation(XmlNode movieNode, IMLItem Item)
        {
            Debugger.LogMessageToFile("Downloading film information from NetFlix...");
            if (movieNode == null)
            {
                MessageBox.Show("movieNode = null");
                return;
            }

            //if (movieNode.ChildNodes.Count < 2)
            //{
            //    MessageBox.Show("movieNode.ChildNodes < 2");
            //    return;
            //}

            //if (movieNode.Name != "entry")
            //{
            //    MessageBox.Show("movieNode.Name != entry");
            //    return;
            //}


            foreach (XmlNode node in movieNode.ChildNodes)
            {
                string value = node.InnerText;
                switch (node.Name)
                {
                    case "title":
                        MessageBox.Show("title case found");

                        if (Settings.NetFlixTitleEnabled && Settings.NetFlixDetailsEnabled)
                        {
                            Item.Tags["Title"] = value;
                            Item.SaveTags();
                            
                        }
                        break;

                    #region Link
                    case "link":
                        MessageBox.Show("link case found");

                        if (Settings.NetFlixActorsEnabled && Settings.NetFlixDetailsEnabled)
                        {
                            MessageBox.Show("Actors step 1");
                            #region Actors
                            if (node.Attributes["title"].Value == "Cast")
                            {
                                MessageBox.Show("Actors step 2");
                                XmlNode CastNode = node.FirstChild.FirstChild;
                                foreach (XmlNode cnode in CastNode)
                                {
                                    switch (cnode.Name)
                                    {
                                        case "entry":
                                            MessageBox.Show("Actors step 3");

                                            foreach (XmlNode actorNode in cnode)
                                            {
                                                switch (actorNode.Name)
                                                {
                                                    case "title":
                                                        MessageBox.Show("Actors step 4");

                                                        if (!string.IsNullOrEmpty((string)Item.Tags["Actors"]))
                                                        {
                                                            if (!Item.Tags["Actors"].ToString().StartsWith("|"))
                                                                Item.Tags["Actors"] = "|" + Item.Tags["Actors"] + "|" + actorNode.InnerText + "|";
                                                            else
                                                                Item.Tags["Actors"] = Item.Tags["Actors"] + actorNode.InnerText + "|";
                                                        }
                                                        else
                                                        {
                                                            Item.Tags["Actors"] = actorNode.InnerText;
                                                        }
                                                        Item.SaveTags();
                                                        break;
                                                }
                                            }
                                            break;
                                    }
                                }
                                Debugger.LogMessageToFile("Movie Cast: " + Item.Tags["Actors"]);
                            }
                            #endregion
                        }

                        if (Settings.NetFlixDirectorsEnabled && Settings.NetFlixDetailsEnabled)
                        {
                            #region Directors
                            if (node.Attributes["title"].Value == "Directors")
                            {
                                XmlNode CastNode = node.FirstChild.FirstChild;
                                foreach (XmlNode cnode in CastNode)
                                {
                                    switch (cnode.Name)
                                    {
                                        case "entry":
                                            foreach (XmlNode directorNode in cnode)
                                            {
                                                switch (directorNode.Name)
                                                {
                                                    case "title":
                                                        if (!string.IsNullOrEmpty((string)Item.Tags["Director"]))
                                                        {
                                                            if (!Item.Tags["Director"].ToString().StartsWith("|"))
                                                                Item.Tags["Director"] = "|" + Item.Tags["Director"] + "|" + directorNode.InnerText + "|";
                                                            else
                                                                Item.Tags["Director"] = Item.Tags["Director"] + directorNode.InnerText + "|";
                                                        }
                                                        else
                                                        {
                                                            Item.Tags["Director"] = directorNode.InnerText;
                                                        }
                                                        Item.SaveTags();
                                                        break;
                                                }
                                            }
                                            break;
                                    }
                                }
                                
                            }
                            #endregion
                        }

                        if (Settings.NetFlixAwardsEnabled && Settings.NetFlixDetailsEnabled)
                        {
                            #region Awards
                            if (node.Attributes["title"].Value == "Awards")
                            {
                                string tempAward = "";
                                XmlNode A1Node = node.FirstChild.FirstChild;
                                foreach (XmlNode cnode in A1Node)
                                {
                                    switch (cnode.Name)
                                    {
                                        case "entry":
                                            foreach (XmlNode awardsNode in cnode)
                                            {
                                                string awardYear = "";
                                                string awardType = "";
                                                string awardLabel = "";
                                                string awardWon = "";
                                                bool AwardSet = false;
                                                switch (awardsNode.Name)
                                                {
                                                    case "content":

                                                        foreach (XmlNode anode in awardsNode)
                                                        {
                                                            switch (anode.Name)
                                                            {
                                                                case "m:properties":
                                                                    foreach (XmlNode a2node in anode.ChildNodes)
                                                                    {
                                                                        switch (a2node.Name)
                                                                        {
                                                                            case "d:Type":
                                                                                awardType = a2node.InnerText;
                                                                                Debugger.LogMessageToFile("Award Type: " + awardType);
                                                                                AwardSet = true;
                                                                                break;
                                                                            case "d:Year":
                                                                                awardYear = a2node.InnerText;
                                                                                Debugger.LogMessageToFile("Award Year: " + awardYear);
                                                                                break;
                                                                            case "d:Category":
                                                                                awardLabel = a2node.InnerText;
                                                                                Debugger.LogMessageToFile("Award Category: " + awardLabel);
                                                                                break;
                                                                            case "d:Won":
                                                                                if (a2node.InnerText == "true")
                                                                                {
                                                                                    awardWon = "(Won)";
                                                                                }
                                                                                else
                                                                                {
                                                                                    awardWon = "(Nominated)";
                                                                                }
                                                                                Debugger.LogMessageToFile("Award Won: " + awardWon);
                                                                                break;
                                                                        }
                                                                    }
                                                                    break;
                                                            }
                                                        }
                                                        break;
                                                }
                                                if (AwardSet == true)
                                                {
                                                    tempAward = tempAward.Trim() + "|" + awardYear.Trim() + " " + awardType.Trim() + ": " + awardLabel.Trim() + " " + awardWon;
                                                }
                                            }
                                            break;
                                    }
                                }
                                if (tempAward.StartsWith("|") && !string.IsNullOrEmpty(tempAward))
                                {
                                    Item.Tags["Awards"] = tempAward + "|";
                                }
                                else if (!string.IsNullOrEmpty(tempAward))
                                {
                                    Item.Tags["Awards"] = tempAward;
                                }
                                Item.SaveTags();

                                MessageBox.Show("Awards: " + tempAward);
                                MessageBox.Show("Awards: " + Item.Tags["Awards"]);
                                Debugger.LogMessageToFile("Movie Awards: " + Item.Tags["Awards"]);
                            }
                            #endregion
                        }

                        if (Settings.NetFlixGenresEnabled && Settings.NetFlixDetailsEnabled)
                        {
                            #region Genres
                            if (node.Attributes["title"].Value == "Genres")
                            {
                                string tempGenre = "";
                                XmlNode A1Node = node.FirstChild.FirstChild;
                                foreach (XmlNode cnode in A1Node)
                                {
                                    switch (cnode.Name)
                                    {
                                        case "entry":
                                            foreach (XmlNode genreNode in cnode)
                                            {

                                                string genre = "";

                                                bool GenreSet = false;
                                                switch (genreNode.Name)
                                                {
                                                    case "content":
                                                        foreach (XmlNode anode in genreNode)
                                                        {
                                                            switch (anode.Name)
                                                            {
                                                                case "m:properties":
                                                                    foreach (XmlNode a2node in anode.ChildNodes)
                                                                    {
                                                                        switch (a2node.Name)
                                                                        {
                                                                            case "d:Name":
                                                                                genre = a2node.InnerText;
                                                                                Debugger.LogMessageToFile("NetFlix Genre: " + genre);
                                                                                GenreSet = true;
                                                                                break;
                                                                        }
                                                                    }
                                                                    break;
                                                            }
                                                        }
                                                        break;
                                                }
                                                if (GenreSet == true)
                                                {
                                                    tempGenre = tempGenre.Trim() + "|" + genre.Trim();
                                                }
                                            }
                                            break;
                                    }
                                }
                                if (tempGenre.StartsWith("|") && !string.IsNullOrEmpty(tempGenre))
                                {
                                    Item.Tags["Genre"] = tempGenre + "|";
                                }
                                else if (!string.IsNullOrEmpty(tempGenre))
                                {
                                    Item.Tags["Genre"] = tempGenre;
                                }
                                Item.SaveTags();
                                Debugger.LogMessageToFile("NetFlix Movie Genres: " + Item.Tags["Genre"]);
                            }
                            #endregion

                        }
                        break;

                    #endregion

                    #region Properties
                    case "m:properties":
                        foreach (XmlNode node2 in node)
                        {
                            switch (node2.Name)
                            {
                                #region NetFlix Movie ID
                                case "d:NetflixApiId":
                                    if (Settings.NetFlixDetailsEnabled)
                                    {
                                        MessageBox.Show("case NetFlix ID found");
                                        string NetflixUrl = node2.InnerText;
                                        int StartIndex = NetflixUrl.LastIndexOf("/");
                                        Item.Tags["NetFlixMovieID"] = NetflixUrl.Remove(0, StartIndex + 1);
                                        Item.SaveTags();
                                        Debugger.LogMessageToFile("Netflix Movie ID: " + Item.Tags["NetFlixMovieID"]);
                                    }
                                    break;

                                #endregion

                                #region Rating
                                case "d:Rating":
                                    if (Settings.NetFlixRatingEnabled && Settings.NetFlixDetailsEnabled)
                                    {
                                        if (!string.IsNullOrEmpty(node2.InnerText))
                                        {
                                            Item.Tags["Rating"] = "USA-" + node2.InnerText;
                                            Item.SaveTags();
                                            Debugger.LogMessageToFile("NetFlix Movie rating: " + Item.Tags["Rating"]);
                                        }
                                    }
                                    break;
                                #endregion

                                #region Release Year
                                case "d:ReleaseYear":
                                    if (Settings.NetFlixReleaseYearEnabled)
                                    {
                                        Item.Tags["Year"] = node2.InnerText;
                                        Item.SaveTags();
                                        Debugger.LogMessageToFile("NetFlix Movie year: " + Item.Tags["Year"]);
                                    }
                                    break;
                                #endregion

                                #region Runtime
                                case "d:Runtime":
                                    if (Settings.NetFlixRuntimeEnabled && Settings.NetFlixDetailsEnabled)
                                    {
                                        Item.Tags["Runtime"] = (Convert.ToInt16(node2.InnerText) / 60);
                                        Item.SaveTags();
                                        Debugger.LogMessageToFile("NetFlix Movie runtime: " + Item.Tags["Runtime"]);
                                    }
                                    break;
                                #endregion

                                #region Film HomePage
                                case "d:WebsiteUrl":
                                    if (Settings.NetFlixHomepageEnabled && Settings.NetFlixDetailsEnabled)
                                    {
                                        Item.Tags["FilmHomepage"] = node2.InnerText;
                                        Item.SaveTags();
                                        Debugger.LogMessageToFile("NetFlix Movie home page: " + Item.Tags["FilmHomepage"]);
                                    }
                                    break;
                                #endregion

                                #region LongOverview 
                                case "d:Synopsis":
                                    MessageBox.Show("Synopsis case found");
                                    if (Settings.NetFlixLongOverviewEnabled && Settings.NetFlixDetailsEnabled)
                                    {
                                        MessageBox.Show("Synopsis step 2");
                                        MessageBox.Show("node2.InnerText: " + node2.InnerText);
                                        Item.Tags["LongOverview"] = MediaFairy.Helpers.CleanHTML(node2.InnerText);
                                        Item.SaveTags();
                                        Debugger.LogMessageToFile("NetFlix Movie synopsis: " + Item.Tags["LongOverview"]);
                                        MessageBox.Show("LongOverview: " + Item.Tags["LongOverview"]);
                                    }
                                    break;
                                #endregion
                            }
                        }
                        break;
                    #endregion

                }
            }
            return;
        }


        public static XmlNodeList getXML(string url)
        {
            try
            {
                HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(url);
                myRequest.AllowAutoRedirect = false;
                HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
                if (myResponse.StatusCode.ToString() == "OK")
                {
                    Stream stream = myResponse.GetResponseStream();
                    StreamReader reader = new StreamReader(stream, Encoding.Default, true);
                    string data = reader.ReadToEnd();
                    reader.Close();
                    stream.Close();
                    myResponse.Close();
                    XmlDocument xml = new XmlDocument();
                    xml.LoadXml(data);
                    XmlNode xmlRoot = xml.FirstChild.NextSibling;
                    return xmlRoot.ChildNodes;
                }
            }
            catch (Exception e)
            {
                Debugger.LogMessageToFile("Error getting node list: " + e);
            }
            return null;
        }

        public static string TwoLetterToFullLanguage(string TwoLetterLanguage)
        {
            foreach (CultureInfo ci in CultureInfo.GetCultures(CultureTypes.NeutralCultures))
            {
                if (ci.TwoLetterISOLanguageName.ToLower() == TwoLetterLanguage.ToLower())
                    return ci.EnglishName;
            }
            return "";
        }





        public static IEnumerable<T> Shim<T>(System.Collections.IEnumerable enumerable)
        {
            foreach (object current in enumerable)
            {
                yield return (T)current;
            }
        }


    }
}