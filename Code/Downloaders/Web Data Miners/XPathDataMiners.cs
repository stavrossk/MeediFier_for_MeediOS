using MeediFier.Code.RegEx_Matchers;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;



namespace MeediFier.Code.Metadata_Scrapers
{


	class XPathDataMiners
	{


		internal static string MatchXpathExpressionReturnFirstMatch
			(string html, string xPathExpression)
		{


			var doc = new HtmlDocument();

			doc.LoadHtml(html);

			HtmlAgilityPack.HtmlNode htmlNode
				= doc.DocumentNode.SelectSingleNode
					(xPathExpression);

			string result = htmlNode.InnerText;


			return result;

		}




		internal static string ExtractFilmTitleUsingXPath(string html)
		{


			//TODO: Currently the film's Original Title is used as the default Title. If no Original Title is available, we should get the film's default title.

			var doc = new HtmlDocument();

			doc.LoadHtml(html);

			HtmlAgilityPack.HtmlNode htmlNode
				= doc.DocumentNode.SelectSingleNode
					(@"//span[@class='title-extra']");

		    string originalTitle = htmlNode.InnerText;


			////TODO: The xml data should be loaded and received from memory instead of a file.

			

			originalTitle = RegExDataMiners.MatchRegexExpressionReturnFirstMatchFirstGroup
				(originalTitle, "\"(?<Title>.*?)\"");


			//MessageBox.Show(originalTitle);

			return (originalTitle);


		}



	}



}
