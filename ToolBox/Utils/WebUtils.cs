using System;
using System.Globalization;
using System.Text;
using System.Net;
using System.IO;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;


namespace MeediFier.ToolBox.Utils
{

	class WebUtils
	{


		static public string GetSiteContents(string url)
		{

			var sb = new StringBuilder();
			
			byte[] buf = new byte[8192];
			
			var request = (HttpWebRequest)WebRequest.Create(url);
			
			var response = (HttpWebResponse)request.GetResponse();

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
						if (num2 <= -1) continue;
						
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
				string sText1 = "&#" + iChar.ToString(CultureInfo.InvariantCulture) + ";";
				return sString.Replace(sText1, Convert.ToChar(iChar).ToString(CultureInfo.InvariantCulture));
			}
			catch
			{
				return sString.Trim();
			}
		}

        // ReSharper disable InconsistentNaming
		public static string POST(string url, string Params)
        // ReSharper restore InconsistentNaming
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

        // ReSharper disable InconsistentNaming
		public static string GET(string URL)
        // ReSharper restore InconsistentNaming
		{
			try
			{

				var webReq = (HttpWebRequest)WebRequest.Create(URL);
				
				webReq.ContentType = "application/x-www-form-urlencoded";
				
				webReq.Method = "GET";
				
				webReq.UserAgent = "Mozilla/4.0 (compatible; MSIE 5.5; Windows NT 5.0)";
				
				return new StreamReader
					(webReq.GetResponse()
					.GetResponseStream())
					.ReadToEnd();
			
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public static string EncodeURL(string url)
		{
			string sReturn = HttpUtility.UrlEncode(url);
			return sReturn.Replace(" & ", "%26");
		}

			 
	}

}