using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net;

#if USE_MEEDIO 
using Meedio; 
#elif USE_MEEDIOS
using MeediOS;
#endif

using Debugger;
using System.Windows.Forms.Design;
using System.Threading;




namespace IMDb
{
    public class IMDbLib
    {

        #region Constants and Variables
        
        private IMDbDatabase _db = null;
        
        #endregion


        #region Constructor
        
        public IMDbLib(string country)
        {
            _db = new IMDbDatabase(country, 100, 10);
        }
        
        #endregion

   
        #region Public Methods 
        
        /// <summary>
        /// Retrieves list of search results matching specified movie title
        /// </summary>
        /// <param name="title">Title of movie to search for</param>
        /// <returns>IList of IIMDbSearchResult objects</returns>
        public IList<IIMDbSearchResult> SearchMovies(string title)
        {

            try
            {
                
                IList<IIMDbSearchResult> list = _db.SearchMovies(title);
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Retrievies list of movies matching specified movie title
        /// </summary>
        /// <param name="title">Title of movie to search for</param>
        /// <returns>IIMDbMovieCollection object</returns>
        public IIMDbMovieCollection SearchMovieDetails(string title, IMLImportProgress ThisProgress, int CurrentProgress)       
        {
        
                IIMDbMovieCollection list = _db.SearchMovieInfo(title, ThisProgress, CurrentProgress );
                return list;
            
           
        }

        /// <summary>
        /// Retrieves movie matching specified id
        /// </summary>
        /// <param name="imdbID">IMDb.com ID</param>
        /// <returns>IIMDbMovie object</returns>
        /// 


        public IIMDbMovie GetMovieDetails(string imdbID, IMLImportProgress ThisProgress, int CurrentProgress, bool ShowProgress)  
        {
            Debugger.Debugger.LogMessageToFile("Entered GetMovieDetails");

            try
            {
                IIMDbMovie movie = _db.GetMovieInfo(imdbID, ThisProgress, CurrentProgress, ShowProgress);
                return movie;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                return null;
            }
        }

        /// <summary>
        /// Saves movie quotes to an xml file 
        /// </summary>
        /// <param name="movie">IIMDbMovie object</param>
        /// <param name="xmlPath">path to save xml file to</param>
        /// <param name="overwrite">set to true to overwrite existing xml file</param>
        public void SaveQuotes(
            IIMDbMovie movie, 
            string xmlPath, 
            bool overwrite)
        {
            System.Xml.XmlTextWriter xmlWr;

            try
            {
                if (System.IO.File.Exists(xmlPath) == false
                    || overwrite)
                    if (movie.Quotes.Count > 0)
                    {
                        xmlWr = new System.Xml.XmlTextWriter(xmlPath, Encoding.Default);
                        xmlWr.Formatting = System.Xml.Formatting.Indented;
                        xmlWr.WriteStartDocument();
                        xmlWr.WriteStartElement("Quotes");
                        foreach (IList<IIMDbQuote> quoteBlock in movie.Quotes)
                        {
                            xmlWr.WriteStartElement("QuoteBlock");
                            foreach (IIMDbQuote quote in quoteBlock)
                            {
                                xmlWr.WriteStartElement("Quote");
                                xmlWr.WriteElementString("Character", quote.Character);
                                xmlWr.WriteElementString("QuoteText", quote.Text);
                                xmlWr.WriteEndElement();
                            }
                            xmlWr.WriteEndElement();
                        }
                        xmlWr.WriteEndElement();
                        xmlWr.WriteEndDocument();
                        xmlWr.Flush();
                        xmlWr.Close();
                        xmlWr = null;
                    }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Saves movie trivia to an xml file 
        /// </summary>
        /// <param name="movie">IIMDbMovie object</param>
        /// <param name="xmlPath">path to save xml file to</param>
        /// <param name="overwrite">set to true to overwrite existing xml file</param>
        public void SaveTrivia(
            IIMDbMovie movie, 
            string xmlPath, 
            bool overwrite)
        {
            System.Xml.XmlTextWriter xmlWr;

            try
            {
                if (System.IO.File.Exists(xmlPath) == false
                    || overwrite)
                    if (movie.Trivia.Count > 0)
                    {
                        xmlWr = new System.Xml.XmlTextWriter(xmlPath, Encoding.Default);
                        xmlWr.Formatting = System.Xml.Formatting.Indented;
                        xmlWr.WriteStartDocument();
                        xmlWr.WriteStartElement("Trivias");
                        foreach (string trivia in movie.Trivia)
                        {
                            xmlWr.WriteElementString("Trivia", trivia);
                        }
                        xmlWr.WriteEndElement();
                        xmlWr.WriteEndDocument();
                        xmlWr.Flush();
                        xmlWr.Close();
                        xmlWr = null;
                    }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Saves movie goofs to an xml file 
        /// </summary>
        /// <param name="movie">IIMDbMovie object</param>
        /// <param name="xmlPath">path to save xml file to</param>
        /// <param name="overwrite">set to true to overwrite existing xml file</param>
        public void SaveGoofs(
            IIMDbMovie movie, 
            string xmlPath, 
            bool overwrite)
        {
            System.Xml.XmlTextWriter xmlWr;

            try
            {
                if (System.IO.File.Exists(xmlPath) == false
                    || overwrite)
                    if (movie.Goofs.Count > 0)
                    {
                        xmlWr = new System.Xml.XmlTextWriter(xmlPath, Encoding.Default);
                        xmlWr.Formatting = System.Xml.Formatting.Indented;
                        xmlWr.WriteStartDocument();
                        xmlWr.WriteStartElement("Goofs");
                        foreach (IIMDbGoof goof in movie.Goofs)
                        {
                            xmlWr.WriteStartElement("Goof");
                            xmlWr.WriteElementString("Category", goof.Category);
                            xmlWr.WriteElementString("Description", goof.Description);
                            xmlWr.WriteEndElement();
                        }
                        xmlWr.WriteEndElement();
                        xmlWr.WriteEndDocument();
                        xmlWr.Flush();
                        xmlWr.Close();
                        xmlWr = null;
                    }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Retrieves list of quotes from specified xml file
        /// </summary>
        /// <param name="xmlPath">path of xml file to read from</param>
        public IList<IList<IIMDbQuote>> ReadQuotes(
            string xmlPath)
        {
            try
            {
                List<IList<IIMDbQuote>> quotes = new List<IList<IIMDbQuote>>();
                if (System.IO.File.Exists(xmlPath))
                {
                    System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                    doc.Load(xmlPath);
                    System.Xml.XmlNodeList quoteBlockList = doc.SelectNodes("/Quotes/QuoteBlock");
                    if (quoteBlockList != null)
                        foreach (System.Xml.XmlNode quoteBlockNode in quoteBlockList)
                        {
                            List<IIMDbQuote> quoteBlock = new List<IIMDbQuote>();
                            foreach (System.Xml.XmlNode quoteNode in quoteBlockNode.ChildNodes)
                            {
                                if (quoteNode["Character"] != null
                                    && quoteNode["Character"].InnerText != null
                                    && quoteNode["Character"].InnerText.Trim() != ""
                                    && quoteNode["QuoteText"] != null
                                    && quoteNode["QuoteText"].InnerText != null
                                    && quoteNode["QuoteText"].InnerText.Trim() != "")
                                {
                                    IMDbQuote quote = new IMDbQuote();
                                    quote.Character = quoteNode["Character"].InnerText;
                                    quote.Text = quoteNode["QuoteText"].InnerText;
                                    quoteBlock.Add(quote);
                                }
                            }
                            if (quoteBlock.Count > 0)
                                quotes.Add(quoteBlock);
                        }
                }
                return quotes;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Retrieves list of goofs from specified xml file
        /// </summary>
        /// <param name="xmlPath">path of xml file to read from</param>
        public IList<IIMDbGoof> ReadGoofs(
            string xmlPath)
        {
            try
            {
                List<IIMDbGoof> goofs = new List<IIMDbGoof>();
                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                doc.Load(xmlPath);
                System.Xml.XmlNodeList goofList = doc.SelectNodes("/Goofs/Goof");
                if (goofList != null)
                    foreach (System.Xml.XmlNode goofNode in goofList)
                    {
                        if (goofNode["Category"] != null
                            && goofNode["Category"].InnerText != null
                            && goofNode["Category"].InnerText.Trim() != ""
                            && goofNode["Description"] != null
                            && goofNode["Description"].InnerText != null
                            && goofNode["Description"].InnerText.Trim() != "")
                        {
                            IMDbGoof goof = new IMDbGoof();
                            goof.Category = goofNode["Category"].InnerText;
                            goof.Description = goofNode["Description"].InnerText;
                            goofs.Add(goof);
                        }
                    }
                return goofs;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Retrieves list of trivia from specified xml file
        /// </summary>
        /// <param name="xmlPath">path of xml file to read from</param>
        public IList<string> ReadTrivia(
            string xmlPath)
        {
            try
            {
                List<string> trivia = new List<string>();
                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                doc.Load(xmlPath);
                System.Xml.XmlNodeList triviaList = doc.SelectNodes("/Trivias/Trivia");
                if (triviaList != null)
                    foreach (System.Xml.XmlNode triviaNode in triviaList)
                    {
                        if (triviaNode.InnerText != null
                            && triviaNode.InnerText.Trim() != "")
                            trivia.Add(triviaNode.InnerText);
                    }
                return trivia;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

    }
}
