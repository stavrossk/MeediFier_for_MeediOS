using System;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace MeediFier.Code.RegEx_Matchers
{
    class RegExDataMiners
    {


        internal static string MatchRegexExpressionReturnFirstMatchFirstGroup(
            string textData, string regexPattern)
        {


            Regex regex = new Regex(regexPattern);
            
            string result = String.Empty;
            
            MatchCollection matches = regex.Matches(textData);

            if (matches.Count == 0)
                return String.Empty;

            try
            {

                result = matches[0].Groups[1].Value;
                if (String.IsNullOrEmpty(result))
                    result = matches[0].Value;

            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }



            return result;
        }








        internal static string[] MatchExpressionReturnFirstMatchAllGroups(string textData, string regexPattern)
        {
            Regex regex = new Regex(regexPattern);
            string[] results = new[] {String.Empty};
            MatchCollection matches = regex.Matches(textData);

            if (matches.Count == 0)
                return null;

            try
            {

                for (int i = 0; i < matches[0].Groups.Count; i++)
                    results[i] = matches[0].Groups[i].Value;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }



            return results;
        }



        internal static string[] MatchExpressionReturnAllMatchesFirstGroup(string textData, string regexPattern)
        {
            Regex regex = new Regex(regexPattern);
            MatchCollection matches = regex.Matches(textData);
            string[] result = new string[matches.Count];

            if (matches.Count == 0)
                return null;

            try
            {

                for (int i = 0; i < matches.Count; i++)
                {
                    result[i] = matches[i].Groups[1].Value;
                    if (String.IsNullOrEmpty(result[i]))
                        result[i] = matches[i].Value;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }

            return result;
        }


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
