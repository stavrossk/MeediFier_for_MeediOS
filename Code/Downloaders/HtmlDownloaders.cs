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

namespace MediaFairy
{
    class HtmlDownloaders
    {



        public static string DownloadHTMLfromURL(string URL)
        {
            WebClient client = new WebClient();
            UTF8Encoding utf8 = new UTF8Encoding();
            //UnicodeEncoding unicode = new UnicodeEncoding();
            byte[] responseBytes = null;
            string htmlText = String.Empty;
            //string result = "";


            Debugger.LogMessageToFile("[HTML Downloader (Standard)] Downloading html data from" + URL + "...");
            #region dload Url Data
            try
            {

                responseBytes = client.DownloadData(URL);
            }
            catch
            {
                try
                {
                    Debugger.LogMessageToFile("Online source did not respond. Retrying...(1)");
                    responseBytes = client.DownloadData(URL);
                }
                catch
                {
                    try
                    {
                        Debugger.LogMessageToFile("Online source did not respond. Retrying...(2)");
                        responseBytes = client.DownloadData(URL);
                    }
                    catch
                    {
                        Debugger.LogMessageToFile("Unable to retrieve HTML data from online location " + URL + ".The online source is not connectable.");
                        return String.Empty;
                    }
                }
            }
            #endregion

            htmlText = utf8.GetString(responseBytes);
            //htmlText = unicode.GetString(responseBytes);

            return htmlText;
        }


        public static string DownloadHTMLfromURLUnicode(string URL)
        {

            #region Vars
            WebClient client = new WebClient();
            client.Encoding = Encoding.GetEncoding("iso-8859-7");
            Encoding encoding = Encoding.GetEncoding("iso-8859-7");
            //UTF8Encoding utf8 = new UTF8Encoding();
            //UnicodeEncoding unicode = new UnicodeEncoding();
            byte[] responseBytes = null;
            string htmlText = String.Empty;
            #endregion

            Debugger.LogMessageToFile("[HTML downloader (Unicode)] Downloading html data from" + URL + "...");
            #region Download URL Data
            try
            {

                responseBytes = client.DownloadData(URL);
            }
            catch
            {
                try
                {
                    Debugger.LogMessageToFile("Online source did not respond. 1nd retry...");
                    responseBytes = client.DownloadData(URL);
                }
                catch
                {
                    try
                    {
                        Debugger.LogMessageToFile("Online source did not respond. 2rd retry...");
                        responseBytes = client.DownloadData(URL);
                    }
                    catch
                    {
                        Debugger.LogMessageToFile("Unable to retrieve HTML data from online location " + URL + ".The online source is not connectable.");
                        return "";
                    }
                }
            }
            #endregion

            htmlText = encoding.GetString(responseBytes);
            return htmlText;
        }






    }
}
