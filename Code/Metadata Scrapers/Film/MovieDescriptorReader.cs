using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Xml;
using MediaFairy.ImportingEngine;
using MeediOS;

namespace MediaFairy.MediaFileDescriptors
{


    internal static class MovieDescriptorReader
    {


        internal static bool ReadMovieDescriptor(IMLItem item,string moviehash, string filmFolder, string filmTitle )
        {

            if (!Settings.FilmDescriptorsAreEnabled) 
                return false;

            string xmlFile;

            if (!LocateDescriptorFile
                (moviehash, filmFolder,
                filmTitle, out xmlFile)) 
                return false;


            var doc = LoadDescriptorDocument(xmlFile);

            return ReadDescriptorNodesPopulateItemTags
                (item, xmlFile, doc);


        }



        private static bool LocateDescriptorFile(string moviehash, string filmFolder, string filmTitle, out string xmlFile)
        {

            if (Settings.FilmDescriptorConvention == "MediaFairy")
                xmlFile = filmFolder + "\\" + moviehash + ".xml";
            else xmlFile = filmFolder + "\\" + filmTitle + ".nfo";

            return File.Exists(xmlFile);
        }


        private static XmlDocument LoadDescriptorDocument(string xmlFile)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlFile);
            return doc;
        }


        private static bool ReadDescriptorNodesPopulateItemTags(IMLItem item, string xmlFile, XmlDocument doc)
        {
            Debugger.LogMessageToFile("Reading film descriptor " + xmlFile + "...");
            MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress, "Reading film descriptor...");

            try
            {
                XmlNodeList nodelist = doc.ChildNodes;

                string title = Helpers.GetTagValueFromItem(item, "Title");
                string videoHash = Helpers.GetTagValueFromItem(item, "Hash");

                if (!ParseChildNodes(item, title, videoHash, nodelist)) return false;

            }
            catch (Exception e)
            {
                Debugger.LogMessageToFile("An unexpected error occured while parsing a Movie Descriptor. The error was: " +
                                          Environment.NewLine + e);
                return false;
            }
            return true;
        }


        private static bool ParseChildNodes(IMLItem item, string title, string videoHash, IEnumerable nodelist)
        {

            foreach (XmlNode childnode in
                from XmlNode node in nodelist
                where node.HasChildNodes
                from XmlNode childnode
                    in node.ChildNodes
                select childnode)
            {

                if (!ReadVideoHashAndTitle
                    (item, videoHash,
                    title, childnode))
                    return false;


                ReadAdditonalDetails
                    (item, childnode);

                item.SaveTags();
            }

            return true;
        }


        private static void ReadAdditonalDetails(IMLItem item, XmlNode childnode)
        {

            XmlNode tag;

            if (String.IsNullOrEmpty(Helpers.GetTagValueFromItem(item, "OriginalTitle")))
            {
                tag = childnode.SelectSingleNode("originaltitle");
                if (tag != null)
                {
                    item.Tags["OriginalTitle"] = tag.InnerText;
                    item.SaveTags();
                }
            }


            if (String.IsNullOrEmpty(Helpers.GetTagValueFromItem(item, "ImdbID")))
            {
                tag = childnode.SelectSingleNode("id");
                if (tag != null)
                {
                    item.Tags["ImdbID"] = tag.InnerText;
                    item.SaveTags();
                }
            }
            if (String.IsNullOrEmpty(Helpers.GetTagValueFromItem(item, "Review")))
            {
                tag = childnode.SelectSingleNode("rating");
                if (tag != null)
                {
                    item.Tags["Review"] = tag.InnerText;
                    item.SaveTags();
                }
            }
            if (String.IsNullOrEmpty(Helpers.GetTagValueFromItem(item, "Year")))
            {
                tag = childnode.SelectSingleNode("year");
                if (tag != null)
                {
                    item.Tags["Year"] = tag.InnerText;
                    item.SaveTags();
                }
            }
            if (String.IsNullOrEmpty(Helpers.GetTagValueFromItem(item, "Overview")))
            {
                tag = childnode.SelectSingleNode("outline");
                if (tag != null)
                {
                    item.Tags["Overview"] = tag.InnerText;
                    item.SaveTags();
                }
            }
            if (String.IsNullOrEmpty(Helpers.GetTagValueFromItem(item, "LongOverview")))
            {
                tag = childnode.SelectSingleNode("plot");
                if (tag != null)
                {
                    item.Tags["LongOverview"] = tag.InnerText;
                    item.SaveTags();
                }
            }
            if (String.IsNullOrEmpty(Helpers.GetTagValueFromItem(item, "Runtime")))
            {
                tag = childnode.SelectSingleNode("runtime");
                if (tag != null)
                {
                    item.Tags["Runtime"] = tag.InnerText;
                    item.SaveTags();
                }
            }

            string taglineTag = Settings.MovieNightCompatibility ? "Subtitle" : "Tagline";

            if (String.IsNullOrEmpty(Helpers.GetTagValueFromItem(item, taglineTag)))
            {
                tag = childnode.SelectSingleNode("tagline");
                if (tag != null)
                {
                    item.Tags[taglineTag] = tag.InnerText;
                    item.SaveTags();
                }
            }

            if (String.IsNullOrEmpty(Helpers.GetTagValueFromItem(item, "Rating")))
            {
                tag = childnode.SelectSingleNode("mpaa");
                if (tag != null)
                {
                    item.Tags["Rating"] = tag.InnerText;
                    item.SaveTags();
                }
            }
            if (String.IsNullOrEmpty(Helpers.GetTagValueFromItem(item, "Watched")))
            {
                tag = childnode.SelectSingleNode("watched");
                if (tag != null)
                {
                    item.Tags["Watched"] = tag.InnerText;
                    item.SaveTags();
                }
            }
            if (String.IsNullOrEmpty(Helpers.GetTagValueFromItem(item, "Genre")))
            {
                tag = childnode.SelectSingleNode("genre");
                if (tag != null)
                {
                    item.Tags["Genre"] = tag.InnerText;
                    item.SaveTags();
                }
            }
            if (String.IsNullOrEmpty(Helpers.GetTagValueFromItem(item, "Director")))
            {
                tag = childnode.SelectSingleNode("director");
                if (tag != null)
                {
                    item.Tags["Director"] = tag.InnerText;
                    item.SaveTags();
                }
            }
            if (String.IsNullOrEmpty(Helpers.GetTagValueFromItem(item, "Actors")))
            {
                tag = childnode.SelectSingleNode("actors");
                if (tag != null)
                {
                    item.Tags["Actors"] = tag.InnerText;
                    item.SaveTags();
                }
            }
            if (String.IsNullOrEmpty(Helpers.GetTagValueFromItem(item, "ActorRoles")))
            {
                tag = childnode.SelectSingleNode("roles");
                if (tag != null)
                {
                    item.Tags["ActorRoles"] = tag.InnerText;
                    item.SaveTags();
                }
            }
            if (String.IsNullOrEmpty(Helpers.GetTagValueFromItem(item, "ReleaseDate")))
            {
                tag = childnode.SelectSingleNode("ReleaseDate");
                if (tag != null)
                {
                    item.Tags["ReleaseDate"] = tag.InnerText;
                    item.SaveTags();
                }
            }
            if (String.IsNullOrEmpty(Helpers.GetTagValueFromItem(item, "Studio")))
            {
                tag = childnode.SelectSingleNode("studio");
                if (tag != null)
                {
                    item.Tags["Studio"] = tag.InnerText;
                    item.SaveTags();
                }
            }

            if (String.IsNullOrEmpty(Helpers.GetTagValueFromItem(item, "TMDbID")))
            {
                tag = childnode.SelectSingleNode("TMDbID");
                if (tag != null)
                {
                    item.Tags["TMDbID"] = tag.InnerText;
                    item.SaveTags();
                }
            }


            if (String.IsNullOrEmpty(Helpers.GetTagValueFromItem(item, "AMGVideoID")))
            {
                tag = childnode.SelectSingleNode("AMGVideoID");
                if (tag != null)
                {
                    item.Tags["AMGVideoID"] = tag.InnerText;
                    item.SaveTags();
                }
            }

            if (String.IsNullOrEmpty(Helpers.GetTagValueFromItem(item, "CriticReview")))
            {
                tag = childnode.SelectSingleNode("AMGReview");
                if (tag != null)
                {
                    item.Tags["CriticReview"] = tag.InnerText;
                    item.SaveTags();
                }
            }

            if (String.IsNullOrEmpty(Helpers.GetTagValueFromItem(item, "Flags")))
            {
                tag = childnode.SelectSingleNode("Flags");
                if (tag != null)
                {
                    item.Tags["Flags"] = tag.InnerText;
                    item.SaveTags();
                }
            }

            if (String.IsNullOrEmpty(Helpers.GetTagValueFromItem(item, "Similar")))
            {
                tag = childnode.SelectSingleNode("Similar");
                if (tag != null)
                {
                    item.Tags["Similar"] = tag.InnerText;
                    item.SaveTags();
                }
            }

            if (String.IsNullOrEmpty(Helpers.GetTagValueFromItem(item, "Synopsis")))
            {
                tag = childnode.SelectSingleNode("Synopsis");
                if (tag != null)
                {
                    item.Tags["Synopsis"] = tag.InnerText;
                    item.SaveTags();
                }
            }

            if (String.IsNullOrEmpty(Helpers.GetTagValueFromItem(item, "Themes")))
            {
                tag = childnode.SelectSingleNode("Themes");
                if (tag != null)
                {
                    item.Tags["Themes"] = tag.InnerText;
                    item.SaveTags();
                }
            }

            if (String.IsNullOrEmpty(Helpers.GetTagValueFromItem(item, "Tones")))
            {
                tag = childnode.SelectSingleNode("Tones");
                if (tag != null)
                {
                    item.Tags["Tones"] = tag.InnerText;
                    item.SaveTags();
                }
            }

            if (String.IsNullOrEmpty(Helpers.GetTagValueFromItem(item, "Types")))
            {
                tag = childnode.SelectSingleNode("Types");
                if (tag != null)
                {
                    item.Tags["Types"] = tag.InnerText;
                    item.SaveTags();
                }
            }

        }


        private static bool ReadVideoHashAndTitle(IMLItem item, string videoHash, string title, XmlNode childnode)
        {

            XmlNode tag;

            #region Read movie hash

            if (String.IsNullOrEmpty(videoHash))
            {
                tag = childnode.SelectSingleNode("Hash");

                if (tag != null)
                {
                    if (String.IsNullOrEmpty(tag.InnerText))
                        return false;

                    item.Tags["Hash"] = tag.InnerText;
                    item.SaveTags();
                }
            }

            #endregion

            #region Read Title

            Debugger.LogMessageToFile("Setting Item's tags to xml file's values...");

            if (String.IsNullOrEmpty(title))
            {
                tag = childnode.SelectSingleNode("title");

                if (tag != null)
                {
                    if (String.IsNullOrEmpty(tag.InnerText))
                        return false;

                    item.Tags["Title"] = tag.InnerText;
                    item.Name = tag.InnerText;

                    item.SaveTags();
                }
            }

            #endregion

            return true;
        }
    }


}