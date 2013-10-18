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
using System.Text;
using System.Net;
using System.IO.Compression;
using System.Text.RegularExpressions;


#if USE_MEEDIO
using MeediOS;
using Meedio; 
#elif USE_MEEDIOS
using MeediOS;
#endif

using System.IO;
using System.Threading;
using System.Windows.Forms;





namespace MeediFier
{

    public class Downloaders
    {





        public static string GetUrl(string Url, string PostData, bool GZip)
        {


            var http = (HttpWebRequest)WebRequest.Create(Url);

            if (GZip)
                http.Headers.Add
                    (HttpRequestHeader.AcceptEncoding, "gzip,deflate");


            if (!string.IsNullOrEmpty(PostData))
            {
                http.Method = "POST";
                byte[] lbPostBuffer = Encoding.Default.GetBytes(PostData);

                http.ContentLength = lbPostBuffer.Length;

                
                Stream PostStream = http.GetRequestStream();
                
                PostStream.Write(lbPostBuffer, 0, lbPostBuffer.Length);
            
                PostStream.Close();
            
            }


            var webResponse = (HttpWebResponse)http.GetResponse();

            Stream responseStream = responseStream = webResponse.GetResponseStream();

            if (webResponse.ContentEncoding.ToLower().Contains("gzip"))
            {

                responseStream = new GZipStream
                    (responseStream, CompressionMode.Decompress);
            
            }
            else if (webResponse.ContentEncoding.ToLower().Contains("deflate"))
            {

                responseStream = new DeflateStream
                    (responseStream, CompressionMode.Decompress);

            }

            StreamReader streamReader = new StreamReader
                (responseStream, Encoding.Default);

            string Html = streamReader.ReadToEnd();

            webResponse.Close();

            if (responseStream != null) responseStream.Close();

            return Html;
        }





        public static WebResponse TryDatabaseRequest
            (string url, string sectionName,
             string databaseName, IMLItem item)
        {


            HttpWebRequest request
                = (HttpWebRequest)WebRequest.Create(url);

            WebResponse response;
            request.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.8.1.4) Gecko/20070515 Firefox/2.0.0.4";


            try
            {
                response = request.GetResponse();
            }
            catch
            {
                Helpers.UpdateProgress("Updating " + sectionName + " Section", databaseName + " did not respond. Retrying...", item);
                Thread.Sleep(2000);
                Helpers.UpdateProgress( "Updating " + sectionName + " Section", "Retrying connection attempt to " + databaseName + "...", item);

                try
                {
                    response = request.GetResponse();
                }
                catch
                {
                    Helpers.UpdateProgress("Updating " + sectionName + " Section", "MediaFairy was unable to connect to " + databaseName + ". The requested online data will not be downloaded for this item.", item);
                    StatusForm.statusForm.TrayIcon.ShowBalloonTip(5000, "Communication with " + databaseName + " failed", "MediaFairy was unable to connect to " + databaseName + " in order to download the requested data. Please check your internet connection availability, otherwise the online database may be temporarily offline or unreachable.", ToolTipIcon.Warning);
                    Thread.Sleep(3000);
                    return null;
                }


            }


            return response;
        }




        public static byte[] TrytoDownloadData
            (string url, string sectionName,
            string databaseName, IMLItem item)
        {



            byte[] data;
            
            var client = new WebClient();

            try
            {
                data = client.DownloadData(url);
                return data;
            }
            catch (Exception)
            {

                Helpers.UpdateProgress("Updating " + sectionName 
                    + " Section", "The connection request to "
                    + databaseName + " timed out. Waiting 5 seconds to retry...", item);

                Thread.Sleep(3000);
                
                Helpers.UpdateProgress("Updating " + sectionName
                    + " Section", "Retrying connection attempt to "
                    + databaseName + "...", item);


                try
                {
                    data = client.DownloadData(url);
                    return data;
                }
                catch
                {


                    Helpers.UpdateProgress
                        ("Updating " + sectionName +
                        " Section", "MediaFairy was unable to connect to "
                        + databaseName + ". The requested online data" +
                        " will not be downloaded for this item.",item);


                    StatusForm.statusForm.TrayIcon.ShowBalloonTip
                        (5000, "Connection to " + databaseName + " failed",
                        "MediaFairy was unable to connect to " + databaseName 
                        + " in order to download the requested data. " +
                        "Please check your internet connection availability, " +
                        "otherwise the online database may be" +
                        " temporarily offline or unreachable.", ToolTipIcon.Warning);


                    Thread.Sleep(3000);
                    return null;
                
                
                }


            }


        }






        public static int TestConnectionSpeed(string url)
        {

            WebClient wc = new WebClient();
            
            DateTime dt1 = DateTime.Now;
            
            byte[] data = wc.DownloadData(url);
            
            DateTime dt2 = DateTime.Now;
            
            int Kbytes = data.Length / 1024;
            
            int KbytesPerSec = Convert.ToInt32(Kbytes / (dt2 - dt1).TotalSeconds);
            
            return KbytesPerSec;
        
        }
    
    }




}
