using System;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;


namespace MeediFier.Code.Downloaders
{


    class WebDataMiner
    {


        public static string MineWebDataReturnFirstMatch(string url, string regexPattern)
        {

            var client = new WebClient();
            
            var utf8 = new UTF8Encoding();
            
            //UnicodeEncoding unicode = new UnicodeEncoding();
            
            byte[] responseBytes;
            
            Regex regex = new Regex(regexPattern);
            
            var uri = new Uri(url, true);

            //MessageBox.Show("URL: " + URL);

            #region dload Url Data
            try
            {
                responseBytes = client.DownloadData(uri);
            }
            catch
            {
                try
                {
                    responseBytes = client.DownloadData(uri);
                }
                catch
                {
                    try
                    {
                        responseBytes = client.DownloadData(uri);
                    }
                    catch
                    {
                        return String.Empty;
                    }
                }
            }
            #endregion

            string htmlText = utf8.GetString(responseBytes);
            //htmlText = unicode.GetString(responseBytes);

            MatchCollection matches = regex.Matches(htmlText);

            if (matches.Count == 0)
                return String.Empty;

   

            string result = matches[0].Groups[1].Captures[0].Value;

            return result;
        }


    }



}
