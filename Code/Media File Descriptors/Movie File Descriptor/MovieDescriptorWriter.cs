using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using MediaFairy.ImportingEngine;
using MeediOS;

namespace MediaFairy.MediaFileDescriptors
{
    internal static class MovieDescriptorWriter
    {
        internal static bool WriteMovieDescriptor(IMLItem item, string itemTitle, string imdbid, string filmFolder,
                                                  bool fileServerIsOnline, bool isUNC)
        {

            string movieHash = Helpers.GetTagValueFromItem(item, "VideoHash");
            string title = Helpers.GetTagValueFromItem(item, "Title");


            if (!ReturnCases(fileServerIsOnline, isUNC, title, movieHash))
                return false;


            var xmlpath = ConstructXmlDescriptorFilePath(item, filmFolder, movieHash);


            if (DescriptorAlreadyExists(item, xmlpath))
                return true;


            var doc = ConstructMovieXmlDescriptorFromSectionItem(item, itemTitle, imdbid, movieHash);


            return SaveXmlDocument(item, filmFolder, doc, xmlpath);


        }

        internal static bool SaveXmlDocument(IMLItem item, string filmFolder, XmlDocument doc, string xmlpath)
        {
            try
            {
                doc.Save(xmlpath);
                item.Tags["HasDescriptor"] = "true";
                item.SaveTags();
                return true;
            }
            catch (Exception)
            {
                //MessageBox.Show(e.ToString());
                StatusForm.statusForm.TrayIcon.ShowBalloonTip(10000, "Access to a media directory was rectricted",
                                                              "MediaFairy could not write a film descriptor file in the directory " +
                                                              filmFolder +
                                                              " because Windows security privileges do not allow modification for this directory.",
                                                              ToolTipIcon.Warning);
                Thread.Sleep(2000);

                return false;
            }

        }

        internal static bool ReturnCases(bool fileServerIsOnline, bool isUNC, string title, string movieHash)
        {
            if (!Settings.FilmDescriptorsAreEnabled)
                return false;

            if (!fileServerIsOnline && isUNC)
                return false;

            if (String.IsNullOrEmpty(movieHash))
                return false;

            return !String.IsNullOrEmpty(title);
        }

        internal static bool DescriptorAlreadyExists(IMLItem item, string xmlpath)
        {
            if (File.Exists(xmlpath))
            {
                item.Tags["HasDescriptor"] = "true";
                item.SaveTags();
                return true;
            }
            return false;
        }

        internal static XmlDocument ConstructMovieXmlDescriptorFromSectionItem(IMLItem item, string itemTitle, string imdbid,
                                                                              string movieHash)
        {
            string longOverview;
            string tagline;
            string overview;
            string review;
            string watched;
            string year;

            var originalTitle = RetrieveKeyTagsFromSectionItem(item, out longOverview, out tagline, out overview, out review,
                                                               out watched, out year);


            var doc = ConstructMovieXmlDescriptor(item, itemTitle, imdbid, longOverview, review,
                                                  movieHash, tagline, year, watched, overview, originalTitle);

            return doc;
        }

        internal static string ConstructXmlDescriptorFilePath(IMLItem item, string filmFolder, string movieHash)
        {

            string xmlpath;

            if (Settings.FilmDescriptorConvention == "MediaFairy")
                xmlpath = filmFolder + "\\" + movieHash + ".xml";
            else xmlpath = filmFolder + "\\" + item.Name + ".nfo";

            return xmlpath;

        }

        internal static string RetrieveKeyTagsFromSectionItem(IMLItem item, out string longOverview, out string tagline,
                                                             out string overview, out string review, out string watched,
                                                             out string year)
        {
            string originalTitle = Helpers.GetTagValueFromItem(item, "OriginalTitle");
            review = Helpers.GetTagValueFromItem(item, "Review");
            year = Helpers.GetTagValueFromItem(item, "Year");
            watched = Helpers.GetTagValueFromItem(item, "Watched");
            overview = Helpers.GetTagValueFromItem(item, "Overview");
            longOverview = Helpers.GetTagValueFromItem(item, "LongOverview");

            string taglineTag = Settings.MovieNightCompatibility ? "Subtitle" : "Tagline";

            tagline = Helpers.GetTagValueFromItem(item, taglineTag);

            return originalTitle;
        }

        internal static XmlDocument ConstructMovieXmlDescriptor(IMLItem item, string itemTitle, string imdbid,
                                                               string longOverview, string review, string movieHash,
                                                               string Tagline, string Year, string Watched, string overview,
                                                               string originalTitle)
        {
            MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress, "saving film descriptor...");
            Thread.Sleep(600);

            XmlElement film;
            XmlElement root;
            var doc = CreateXmlDocument(out film, out root);

            XmlElement studio;
            XmlElement tmdbid;
            XmlElement actors;
            XmlElement roles;
            XmlElement amgid;
            XmlElement releasedate;
            XmlElement themes;
            XmlElement synopsis;
            XmlElement types;
            XmlElement tones;
            XmlElement flags;
            XmlElement amgReview;
            XmlElement similar;
            XmlElement rating;
            XmlElement year;
            XmlElement outline;
            XmlElement moviehash;
            XmlElement originaltitle;
            XmlElement id;
            XmlElement watched;
            XmlElement genre;
            XmlElement director;
            XmlElement mpaa;
            XmlElement plot;
            XmlElement tagline;
            XmlElement runtime;

            var title = CreateSubNodesFromItemTags(item, itemTitle, imdbid, movieHash, review, longOverview,
                                                   Tagline, overview, originalTitle, doc, Year, Watched,
                                                   out studio,      out tmdbid,         out actors,
                                                   out roles,       out amgid,          out releasedate,
                                                   out themes,      out synopsis,       out types, 
                                                   out tones,       out flags,          out amgReview, 
                                                   out similar,     out rating,         out year,   
                                                   out outline,
                                                   out moviehash,   out originaltitle,
                                                   out id,          out watched,        out genre,
                                                   out director,    out mpaa,           out plot,
                                                   out tagline,     out runtime);


            AppendCreatedNodesToRoot(tmdbid, amgid, releasedate, studio, director, actors,
                                     roles, themes, tones, types, synopsis, amgReview, flags, similar,
                                     moviehash, rating, year, id, film, title, originaltitle, mpaa,
                                     watched, genre, tagline, outline, plot, runtime);

            root.AppendChild(film);

            return doc;
        
        }

        internal static XmlDocument CreateXmlDocument(out XmlElement film, out XmlElement root)
        {
            #region Create the XML document

            XmlDocument doc = new XmlDocument();
            XmlDeclaration dec = doc.CreateXmlDeclaration("1.0", null, null);
            doc.AppendChild(dec);
            root = doc.CreateElement("Library");
            doc.AppendChild(root);
            film = doc.CreateElement("movie");

            #endregion

            return doc;
        }

        internal static XmlElement CreateSubNodesFromItemTags(IMLItem item, string itemTitle, string imdbid, string movieHash,
                                                             string review, string longOverview, string taglineTag, string overview,
                                                             string originalTitle, XmlDocument doc, string yearTag, string watchedTag,
                                                          
                                                             out XmlElement studio,        out XmlElement tmdbid, out XmlElement actors,
                                                             out XmlElement roles,         out XmlElement amgid,
                                                             out XmlElement releasedate,   out XmlElement themes,
                                                             out XmlElement synopsis,      out XmlElement types, out XmlElement tones,
                                                             out XmlElement flags,         out XmlElement amgReview,
                                                             out XmlElement similar,       out XmlElement rating, out XmlElement year,
                                                             out XmlElement outline,       out XmlElement moviehash,
                                                             out XmlElement originaltitle, out XmlElement id,
                                                             out XmlElement watched,       out XmlElement genre,
                                                             out XmlElement director,      out XmlElement mpaa, out XmlElement plot,
                                                             out XmlElement tagline,       out XmlElement runtime)
        {

            var title = CreateKeyElements(itemTitle, imdbid, movieHash, originalTitle, doc,
                                          out moviehash, out originaltitle, out id);


            CreateAdditionalElements(item, review, longOverview, taglineTag, overview, doc, yearTag, watchedTag,
                                     out studio, out tmdbid, out actors, out roles, out amgid, out releasedate,
                                     out themes, out synopsis, out types, out tones, out flags,
                                     out amgReview, out similar, out rating, out year,
                                     out outline, out watched, out genre, out director, 
                                     out mpaa, out plot, out tagline, out runtime);


            return title;
        }

        internal static XmlElement CreateKeyElements(string itemTitle, string imdbid, string movieHash, string originalTitle,
                                                    XmlDocument doc,  out XmlElement moviehash, out XmlElement originaltitle,
                                                    out XmlElement id)
        {
            XmlElement title = doc.CreateElement("title");
            title.InnerText = itemTitle;

            originaltitle = doc.CreateElement("originaltitle");
            originaltitle.InnerText = originalTitle;

            id = doc.CreateElement("id");
            id.InnerText = imdbid;

            moviehash = doc.CreateElement("VideoHash");
            moviehash.InnerText = movieHash;
            return title;
        }

        internal static void CreateAdditionalElements(IMLItem item, string reviewTag, string longOverviewTag, string taglineTag,
                                                     string overviewTag, XmlDocument doc, string yearTag, string watchedTag,

                                                     out XmlElement studio,    out XmlElement tmdbid,   out XmlElement actors,
                                                     out XmlElement roles,     out XmlElement amgid,    out XmlElement releasedate,
                                                     out XmlElement themes,    out XmlElement synopsis, out XmlElement types,
                                                     out XmlElement tones,     out XmlElement flags,    out XmlElement amgReview,
                                                     out XmlElement similar,   out XmlElement rating,   out XmlElement year,
                                                     out XmlElement outline,   out XmlElement watched,  out XmlElement genre,
                                                     out XmlElement director,  out XmlElement mpaa,     out XmlElement plot,
                                                     out XmlElement tagline,   out XmlElement runtime)
        {
            rating = doc.CreateElement("rating");
            rating.InnerText = reviewTag;

            year = doc.CreateElement("year");
            year.InnerText = yearTag;

            outline = doc.CreateElement("outline");
            outline.InnerText = overviewTag;

            plot = doc.CreateElement("plot");
            plot.InnerText = longOverviewTag;

            tagline = doc.CreateElement("tagline");
            tagline.InnerText = taglineTag;

            runtime = doc.CreateElement("runtime");
            runtime.InnerText = Helpers.GetTagValueFromItem(item, "Runtime");

            mpaa = doc.CreateElement("mpaa");
            mpaa.InnerText = Helpers.GetTagValueFromItem(item, "Rating");

            watched = doc.CreateElement("watched");
            watched.InnerText = watchedTag;

            genre = doc.CreateElement("genre");
            genre.InnerText = Helpers.GetTagValueFromItem(item, "Genre");

            director = doc.CreateElement("director");
            director.InnerText = Helpers.GetTagValueFromItem(item, "Director");

            actors = doc.CreateElement("actors");
            actors.InnerText = Helpers.GetTagValueFromItem(item, "Actors");

            roles = doc.CreateElement("roles");
            roles.InnerText = Helpers.GetTagValueFromItem(item, "ActorRoles");

            studio = doc.CreateElement("studio");
            studio.InnerText = Helpers.GetTagValueFromItem(item, "Studio");

            tmdbid = doc.CreateElement("TMDbID");
            tmdbid.InnerText = Helpers.GetTagValueFromItem(item, "TMDbID");

            amgid = doc.CreateElement("AMGVideoID");
            amgid.InnerText = Helpers.GetTagValueFromItem(item, "AMGVideoID");

            releasedate = doc.CreateElement("ReleaseDate");
            releasedate.InnerText = Helpers.GetTagValueFromItem(item, "ReleaseDate");

            amgReview = doc.CreateElement("AMGReview");
            amgReview.InnerText = Helpers.GetTagValueFromItem(item, "CriticReview");

            flags = doc.CreateElement("Flags");
            flags.InnerText = Helpers.GetTagValueFromItem(item, "Flags");

            similar = doc.CreateElement("Similar");
            similar.InnerText = Helpers.GetTagValueFromItem(item, "Similar");

            synopsis = doc.CreateElement("Synopsis");
            synopsis.InnerText = Helpers.GetTagValueFromItem(item, "Synopsis");

            themes = doc.CreateElement("Themes");
            themes.InnerText = Helpers.GetTagValueFromItem(item, "Themes");

            tones = doc.CreateElement("Tones");
            tones.InnerText = Helpers.GetTagValueFromItem(item, "Tones");

            types = doc.CreateElement("Types");
            types.InnerText = Helpers.GetTagValueFromItem(item, "Types");

        }

        internal static void AppendCreatedNodesToRoot(XmlNode tmdbid,     XmlNode amgid,      XmlNode releasedate,
                                                     XmlNode studio,     XmlNode director,   XmlNode actors,        XmlNode roles,
                                                     XmlNode themes,     XmlNode tones,      XmlNode types,         XmlNode synopsis,
                                                     XmlNode amgReview,  XmlNode flags,      XmlNode similar,
                                                     XmlNode moviehash,  XmlNode rating,     XmlNode year,          XmlNode id,
                                                     XmlNode film,       XmlNode title,      XmlNode originaltitle,
                                                     XmlNode mpaa,       XmlNode watched,    XmlNode genre,         XmlNode tagline,
                                                     XmlNode outline,    XmlNode plot,       XmlNode runtime)
        {
            film.AppendChild(title);
            film.AppendChild(originaltitle);
            film.AppendChild(id);
            film.AppendChild(moviehash);
            film.AppendChild(rating);
            film.AppendChild(year);
            film.AppendChild(outline);
            film.AppendChild(plot);
            film.AppendChild(runtime);
            film.AppendChild(tagline);
            film.AppendChild(mpaa);
            film.AppendChild(watched);
            film.AppendChild(genre);
            film.AppendChild(director);
            film.AppendChild(actors);
            film.AppendChild(roles);
            film.AppendChild(studio);
            film.AppendChild(tmdbid);
            film.AppendChild(amgid);
            film.AppendChild(releasedate);
            film.AppendChild(amgReview);
            film.AppendChild(flags);
            film.AppendChild(similar);
            film.AppendChild(synopsis);
            film.AppendChild(themes);
            film.AppendChild(tones);
            film.AppendChild(types);
        }
    }


}
