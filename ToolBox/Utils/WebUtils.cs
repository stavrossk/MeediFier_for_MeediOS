using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
using System.Web;

namespace ToolBox
{
    class WebUtils
    {
        static public string GetSiteContents(string url)
        {
            StringBuilder sb = new StringBuilder();
            byte[] buf = new byte[8192];
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            Stream resStream = response.GetResponseStream();
            int count = 0;

            do
            {
                if (resStream != null) 
                    count = resStream.Read(buf, 0, buf.Length);

                if (count == 0)
                    continue;
                
                string tempString = Encoding.ASCII.GetString(buf, 0, count);
                sb.Append(tempString);
            
            }
            while (count > 0);

            return sb.ToString();

        }


        static public String CleanUpHTML(string vString, bool vClearHref)
        {
            int num1 = 0, num2 = 0;
            string sText1 = vString;

            try
            {
                do
                {
                    num1 = sText1.ToLower().IndexOf("<script", num1);
                    if (num1 > -1)
                    {
                        num1++;
                        num2 = sText1.ToLower().IndexOf("</script>", num1);
                        if (num2 > -1)
                        {
                            num2 += 8;
                            sText1 = sText1.Replace(sText1.Substring(num1, (num2 - num1)).Trim(), "").Trim();
                            sText1 += " ";
                        }
                        else
                        {
                            num1 = -1;
                        }
                    }
                } while (num1 > -1);
                num1 = 0;
                do
                {
                    num1 = sText1.ToLower().IndexOf("<style", num1);
                    if (num1 > -1)
                    {
                        num1++;
                        num2 = sText1.ToLower().IndexOf("</style>", num1);
                        if (num2 > -1)
                        {
                            num2 += 7;
                            sText1 = sText1.Replace(sText1.Substring(num1, (num2 - num1)).Trim(), "").Trim();
                            sText1 += " ";
                        }
                        else
                        {
                            num1 = -1;
                        }
                    }
                } while (num1 > -1);
                num1 = 0;
                do
                {
                    num1 = sText1.IndexOf("<", num1);
                    if (num1 > -1)
                    {
                        if (vClearHref)
                        {
                            num2 = sText1.IndexOf(">", num1);
                            if (num2 > -1)
                            {
                                num2++;
                                sText1 = sText1.Replace(sText1.Substring(num1, (num2 - num1)).Trim(), "").Trim();
                                sText1 += " ";
                            }
                            else
                            {
                                num1 = -1;
                            }
                        }
                        else
                        {
                            int num3 = sText1.ToLower().IndexOf("a href", num1);
                            if (num3 == (num1 + 1))
                            {
                                num1++;
                            }
                            else
                            {
                                num3 = sText1.ToLower().IndexOf("/a>", num1);
                                if (num3 == (num1 + 1))
                                {
                                    num1++;
                                }
                                else
                                {
                                    num3 = sText1.ToLower().IndexOf("p class=\"mt-0", num1);
                                    if (num3 == (num1 + 1))
                                    {
                                        num1++;
                                    }
                                    else
                                    {
                                        num2 = sText1.IndexOf(">", num1);
                                        if (num2 > -1)
                                        {
                                            num2++;
                                            sText1 = sText1.Replace(sText1.Substring(num1, (num2 - num1)).Trim(), "").Trim();
                                            sText1 += " ";
                                        }
                                        else
                                        {
                                            num1 = -1;
                                        }
                                    }
                                }
                            }
                        }
                    }
                } while (num1 > -1);
                return sText1.Replace("&nbsp;", " ").Replace("\n", " ").Replace("\r", " ").Replace("\t", " ").Replace("&amp;", "&").Trim();
            }
            catch
            {
                return vString.Trim();
            }
        }

        static public String FixString(string sString)
        {
            int num1 = 0, num2 = 0, num3 = 0;
            string sText = sString;

            try
            {
                for (int i = 1; i < 3; i++)
                {
                    num2 = 0;
                    do
                    {
                        if (num2 > sText.Length)
                        {
                            num2 = 0;
                        }
                        num2 = sText.IndexOf("&#", num2);
                        if (num2 > -1)
                        {
                            num2 += 2;
                            num3 = sText.IndexOf(";", num2);
                            if (num3 > -1 && (num3 - num2) < 4)
                            {
                                num1 = int.Parse(sText.Substring(num2, (num3 - num2)).Trim());
                                sText = ReplaceChar(sText, num1);
                            }
                            else
                            {
                                num2 += 2;
                            }
                        }
                    } while (num2 > -1);
                }
                return sText.Trim();
            }
            catch
            {
                return sString.Trim();
            }
        }

        static public String ReplaceChar(string sString, int iChar)
        {
            try
            {
                string sText1 = "&#" + iChar.ToString() + ";";
                return sString.Replace(sText1, Convert.ToChar(iChar).ToString());
            }
            catch
            {
                return sString.Trim();
            }
        }

        public static string POST(string url, string Params)
        {
            try
            {
                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(url);
                webReq.ContentType = "application/x-www-form-urlencoded";
                webReq.Method = "POST";
                webReq.UserAgent = "Mozilla/4.0 (compatible; MSIE 5.5; Windows NT 5.0)";
                byte[] bytes = Encoding.Default.GetBytes(Params);
                webReq.ContentLength = bytes.Length;
                Stream os = webReq.GetRequestStream();
                os.Write(bytes, 0, bytes.Length);
                os.Close();
                os.Dispose();
                return new StreamReader(webReq.GetResponse().GetResponseStream()).ReadToEnd();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string GET(string URL)
        {
            try
            {
                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(URL);
                webReq.ContentType = "application/x-www-form-urlencoded";
                webReq.Method = "GET";
                webReq.UserAgent = "Mozilla/4.0 (compatible; MSIE 5.5; Windows NT 5.0)";
                return new StreamReader(webReq.GetResponse().GetResponseStream()).ReadToEnd();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string EncodeURL(string url)
        {
            try
            {
                string sReturn = HttpUtility.UrlEncode(url);
                return sReturn.Replace(" & ", "%26");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

             
    }

}