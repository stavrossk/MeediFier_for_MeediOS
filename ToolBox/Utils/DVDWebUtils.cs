using System;
using System.IO;
using System.Net;
using System.Xml;

namespace MediaFairy.ToolBox.Utils
{
    internal class DvdWebUtils
    {
        public static string DvdTitle(string DVDID)
        {
            string dvdTitle;
            String dvdName = @"http://movie.metaservices.microsoft.com/pas_movie_B/template/GetMDRDVDByCRC.xml?CRC=" +
                             DVDID;
            var request = WebRequest.Create(dvdName)
                as HttpWebRequest;


            request.AllowAutoRedirect = false;


            using (var response = request.GetResponse() as HttpWebResponse)
            {
                Stream content = response.GetResponseStream();
                XmlReader reader = XmlReader.Create(new StreamReader(content));
                var doc = new XmlDocument();
                reader.Read();
                doc.Load(reader);
                dvdTitle = doc.SelectSingleNode("/METADATA/MDR-DVD/dvdTitle").InnerText;
                {
                    if (dvdTitle.Contains("["))
                    {
                        int Pos = dvdTitle.IndexOf("[");
                        String S2 = dvdTitle.Remove(Pos);
                        dvdTitle = S2.Trim();
                    }
                    Debugger.LogMessageToFile("[DVDWebUtils] - DVD title: " + dvdTitle);
                }
            }
            return dvdTitle.Trim();
        }

        public static string DVDFirstChapter(string DVDID, bool Log, StreamWriter swWriteLog)
        {
            string EpisodeName = "";
            String dvdName = @"http://movie.metaservices.microsoft.com/pas_movie_B/template/GetMDRDVDByCRC.xml?CRC=" +
                             DVDID;
            var request = WebRequest.Create(dvdName) as HttpWebRequest;
            request.AllowAutoRedirect = false;
            using (var response = request.GetResponse() as HttpWebResponse)
            {
                Stream content = response.GetResponseStream();
                XmlReader reader = XmlReader.Create(new StreamReader(content));
                var doc = new XmlDocument();
                reader.Read();
                doc.Load(reader);
                XmlNodeList nodeList = doc.SelectNodes("/METADATA/MDR-DVD/title/chapter");
                foreach (XmlNode nd in nodeList)
                {

                    if (nd.Name != "chapter")
                        continue;

                    XmlNode xmLchapterNum = nd.FirstChild;
                    XmlNode xmLchapterTitle = xmLchapterNum.NextSibling;
                    EpisodeName = xmLchapterTitle.InnerText;
                    break;

                }
                reader.Close();
                content.Close();
            }
            return EpisodeName.Trim();
        }



        public static string DVDDirector(string DVDID)
        {
            string DVDDirectors = "";
            String DVDName = @"http://movie.metaservices.microsoft.com/pas_movie_B/template/GetMDRDVDByCRC.xml?CRC=" +
                             DVDID;
            var request = WebRequest.Create(DVDName) as HttpWebRequest;
            request.AllowAutoRedirect = false;
            using (var response = request.GetResponse() as HttpWebResponse)
            {
                Stream content = response.GetResponseStream();
                XmlReader reader = XmlReader.Create(new StreamReader(content));
                var doc = new XmlDocument();
                reader.Read();
                doc.Load(reader);
                DVDDirectors = doc.SelectSingleNode("/METADATA/MDR-DVD/director").InnerText;
                {
                    if (DVDDirectors.Contains(";"))
                    {
                        int Pos4 = DVDDirectors.IndexOf(";");
                        String Directors3 = DVDDirectors.Remove(Pos4);
                        DVDDirectors = Directors3.Trim();
                        Debugger.LogMessageToFile("[DVDWebUtils] - First Director from MS Site: " + DVDDirectors);
                    }
                    else
                    {
                        Debugger.LogMessageToFile("[DVDWebUtils] - First Director from MS Site: " + DVDDirectors);
                    }
                }
                reader.Close();
                content.Close();
            }
            return DVDDirectors.Trim();
        }



        public static string DvdFirstActor(string dvdid)
        {
            string dvdActors = "";
            String DVDName = @"http://movie.metaservices.microsoft.com/pas_movie_B/template/GetMDRDVDByCRC.xml?CRC=" +
                             dvdid;
            var request = WebRequest.Create(DVDName) as HttpWebRequest;
            request.AllowAutoRedirect = false;
            using (var response = request.GetResponse() as HttpWebResponse)
            {
                Stream content = response.GetResponseStream();
                XmlReader reader = XmlReader.Create(new StreamReader(content));
                var doc = new XmlDocument();
                reader.Read();
                doc.Load(reader);
                dvdActors = doc.SelectSingleNode("/METADATA/MDR-DVD/actors").InnerText;
                {
                    if (dvdActors.Contains(";"))
                    {
                        int Pos5 = dvdActors.IndexOf(";");
                        String Actors3 = dvdActors.Remove(Pos5);
                        dvdActors = Actors3.Trim();
                        Debugger.LogMessageToFile("[DVDWebUtils] - First Actor from MS Site: " + dvdActors);
                    }
                    else
                    {
                        Debugger.LogMessageToFile("[DVDWebUtils] - First Actor from MS Site: " + dvdActors);
                    }
                }
                reader.Close();
                content.Close();
            }
            return dvdActors.Trim();
        }

        public static string GetReleaseYear(string DVDID)
        {
            String ReleaseYear = "";
            try
            {
                String DVDID2 = DVDID.Insert(8, "|");
                String DVDDetails_url = @"http://www.e-home.no/metaservices/XML/GetDVDInfo.aspx?Extended=True&DVDID=" +
                                        DVDID2; //999b71fa|d438d6f5
                var request = WebRequest.Create(DVDDetails_url) as HttpWebRequest;
                
                if (request != null) 
                    request.AllowAutoRedirect = false;

                String DVDYear = "";

                if (request != null)
                    using (var response = request.GetResponse() as HttpWebResponse)
                    {
                        if (response != null)
                        {
                            Stream content = response.GetResponseStream();

                            if (content != null)
                            {
                                XmlReader reader = XmlReader.Create(new StreamReader(content));
                                var doc = new XmlDocument();
                                reader.Read();
                                doc.Load(reader);
                                var selectSingleNode = doc.SelectSingleNode("/METADATA/MDR-DVD/movieReleaseYear");
                   
                                if (selectSingleNode != null)
                                    DVDYear = selectSingleNode.InnerText;

                                reader.Close();
                            }

                            if (content != null) 
                                content.Close();
                        }
                    }
                ReleaseYear = DVDYear;
            }
            catch (Exception e)
            {
                Debugger.LogMessageToFile("[DVDWebUtils] - Get Release Year - Error on OnMessage: " + e);
            }
            Debugger.LogMessageToFile("[DVDWebUtils] - ReleaseYear Year: " + ReleaseYear);
            return ReleaseYear;
        }
    }
}