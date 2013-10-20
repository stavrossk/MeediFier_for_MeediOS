using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using MeediFier.Code.Metadata_Scrapers;
using MeediFier.ImportingEngine;

namespace MeediFier.IMDb
{
	internal class IMDbMovieDetailsDownloaderHelpers
	{


		internal static readonly string BaseTitleUrl = "http://www.imdb.com/title/";



		internal static IMDbMovie MineFilmDetailsFromAdditionalPages
			(bool showProgress, IMDbFilmDetails filmDetails,
             IMDbMovie movie, string movieUrl)
		{


			if (!showProgress)
				return movie;




            string creditsUrl = movieUrl + "fullcredits";
            string longOverviewUrl = movieUrl + "plotsummary";
            string goofUrl = movieUrl + "goofs";
            string triviaUrl = movieUrl + "trivia";
            string quotesUrl = movieUrl + "quotes";


			MainImportingEngine.ThisProgress.Progress
                (MainImportingEngine.CurrentProgress, "Getting actors...");
			
            filmDetails.GetActors(ref movie, creditsUrl);


			MainImportingEngine.ThisProgress.Progress
                (MainImportingEngine.CurrentProgress, "Getting plot summary...");
			
            filmDetails.GetLongOverview(ref movie, longOverviewUrl);



			//MainImportingEngine.ThisProgress.Progress
			//    (MainImportingEngine.CurrentProgress, "Getting trivia...");

			//filmDetails.GetTrivia(ref movie, triviaUrl);

			//MainImportingEngine.ThisProgress.Progress
			//    (MainImportingEngine.CurrentProgress, "Getting goofs...");

			//filmDetails.GetGoofs(ref movie, goofUrl);

			//MainImportingEngine.ThisProgress.Progress
			//    (MainImportingEngine.CurrentProgress, "Getting quotes...");

			//filmDetails.GetQuotes(ref movie, quotesUrl);

			//TODO: Get these additional IMDb film details

			//Get Stars

			//Get Awards

			//Get Credits

			//Get Plot Keywords

			//Get Official Websites

			//Get Production Country

			//Get Language

			//Get Box Office Budget

			//Get Box Office Gross

			//Get Color

			//Get Aspect Ratio

			//Get Connections

			//Get Soundtracks

			//Get User Review


			return movie;


		}




		internal static bool CheckForValidHtmlData(string html)
		{

			if (String.IsNullOrEmpty(html))
			{
				Debugger.LogMessageToFile("[IMDb film details downloader]" +
										  " Unable to get film data from IMDb." +
										  " The returned film HTML page was empty.");
				return true;
			}


			Debugger.LogMessageToFile("[IMDb film details downloader]" +
									  " The returned IMDb film html page contains vaid data.");
			return false;
		}





		internal static string ConstructMovieUrl(string imdbID)
		{
			Debugger.LogMessageToFile("[IMDb film details downloader] Constructing IMDb movie url...");
			Debugger.LogMessageToFile("[IMDb film details downloader] IMDb base url: " + BaseTitleUrl);

			string movieUrl = BaseTitleUrl + imdbID + "/";
			Debugger.LogMessageToFile("[IMDb film details downloader] movie URL: " + movieUrl);
			return movieUrl;
		}





		internal static string DownloadBaseTitleHtml
			(string movieUrl, WebClient webClient)
		{

			string html;

			Debugger.LogMessageToFile
				("[IMDb film details downloader]" +
				 " Downloading film's main html page...");



			try
			{
				
				html = webClient.DownloadString(movieUrl);


				html = html.Normalize();

			}
			catch
			{
				Debugger.LogMessageToFile("[IMDb film details downloader] IMDb did not respond. Retrying...");
				Helpers.UpdateProgress("Updating Movies Section...", "IMDb did not respond. Retrying...", null);

				try
				{
				   
					html = webClient.DownloadString(movieUrl);

				}
				catch
				{

					Debugger.LogMessageToFile
						("A connection error occured while attempting to download" +
						 " IMDb's film web page. Giving up for this item.");

					Helpers.UpdateProgress
						("Updating Movies Section...", 
						"Unable to connect to IMDb." +
						"Details for this film will not be downloaded.", null);

					StatusForm.statusForm.TrayIcon.ShowBalloonTip
						(5000, "Communication with IMDb failed",
						 "MediaFairy was unable to connect to IMDb in order to download details for a film. " +
						 "Please check your internet connection availability," +
						 " otherwise the online database may be temporarily offline or unreachable.",
						 ToolTipIcon.Warning);

					return null;
				
				}

			}


			return html;

		}



		internal static string ExtractFilmTitle
			(string imdbID,
			 string trimmedHtml)
		{


			Debugger.LogMessageToFile
				("[IMDb Movie Details Downloader] " +
				 "Extracting title from movie html page...");


			string filmTitle = XPathDataMiners.ExtractFilmTitleUsingXPath(trimmedHtml);

			//Match match
			//    = GetMovieTitleRegexMatch
			//        (imdbID, trimmedHtml);

			//if (match == null)
			//    return String.Empty;


			//var imDbRegEx = new IMDbRegEx();
			//imDbRegEx.SetRegExPatterns();


			//string filmTitle
			//    = imDbRegEx.GetMatchValue
			//        (match, "Title", true);




			if (String.IsNullOrEmpty
				(filmTitle))
			{


				Debugger.LogMessageToFile
					("[IMDb Movie Details Downloader]" +
					 " Unable to extract Movie Title from " +
					 "Regex match.");

				StatusForm.statusForm.TrayIcon.ShowBalloonTip
					(10000, "Unable to extract film details from IMDb",
					 "MediaFairy's IMDb film details downloader was unable to extract" +
					 " a film's Title from the IMDb database. " +
					 "If the IMDb website changed, please report this issue" +
					 " to the plugin's developer in order for this engine to be updated." +
					 " Film details downloading for this item will be skipped.",
					 ToolTipIcon.Warning);

				Thread.Sleep(2000);

				return String.Empty;
			}


			Debugger.LogMessageToFile("[IMDb film details downloader]" +
									  " IMDb returned title: " + filmTitle);
			//MessageBox.Show(@"IMDb returned title: " + movie.Title);

			return filmTitle;


		}








		internal static Match GetMovieTitleRegexMatch
			(string imdbID, string trimmedHtml)
		{




			Debugger.LogMessageToFile
				("[IMDb Movie Details Downloader]" +
				 " Getting Title regex match...");


			var imDbRegEx = new IMDbRegEx();
			imDbRegEx.SetRegExPatterns();


			Match match = imDbRegEx.GetRegExMatch
				(trimmedHtml,
				 IMDbRegEx.TitlePatternOriginal);



			try
			{
				string filmTitle
					= match.Groups[1].Captures[0].Value;
				return match;

			}
			catch (Exception)
			{


				match = imDbRegEx.GetRegExMatch
					(trimmedHtml,
					 IMDbRegEx.TitlePatternPrimary);


				try
				{
					string filmTitle
						= match.Groups[1].Captures[0].Value;

					return match;

				}
				catch (Exception e)
				{


					Debugger.LogMessageToFile
						("[IMDb Movie Details Downloader] " +
						 "The IMDb Movie Details Downloader was unable" +
						 " to extract the movie title " +
						 "for the movie with IMDb ID: "
						 + imdbID + ".");


					return match;
				}




			}



		}


   






	internal static string FixRuntime(string value)
		{
			try
			{
				var runtime = value.Trim();

				if (runtime.IndexOf(":", StringComparison.Ordinal) > -1
					&& runtime.IndexOf(":", StringComparison.Ordinal) < runtime.Length -1)
					runtime = runtime.Substring(runtime.IndexOf(":", StringComparison.Ordinal) + 1).Trim();
				if (runtime.IndexOf("\\", StringComparison.Ordinal) > -1)
					runtime = runtime.Substring(0, runtime.IndexOf("\\", System.StringComparison.Ordinal)).Trim();
				if (runtime.IndexOf("/", StringComparison.Ordinal) > -1)
					runtime = runtime.Substring(0, runtime.IndexOf("/", System.StringComparison.Ordinal)).Trim();
				if (runtime.IndexOf(" ", System.StringComparison.Ordinal) > -1)
					runtime = runtime.Substring(0, runtime.IndexOf(" ", System.StringComparison.Ordinal)).Trim();
				return runtime.Trim();
			}
			catch
			{
				return value.Trim();
			}
		}
	}



}
