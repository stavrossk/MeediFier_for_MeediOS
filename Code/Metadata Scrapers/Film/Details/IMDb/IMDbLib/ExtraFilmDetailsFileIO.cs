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
using System.Collections.Generic;
using System.Text;
using System.IO;

#if USE_MEEDIO 

#elif USE_MEEDIOS

#endif


namespace MediaFairy
{




    public static class ExtraFilmDetailsFileIO
    {



        #region Read and save additional film details from/to XML files

        /// <summary>
        /// Saves movie quotes to an xml file 
        /// </summary>
        /// <param name="movie">IIMDbMovie object</param>
        /// <param name="xmlPath">path to save xml file to</param>
        /// <param name="overwrite">set to true to overwrite existing xml file</param>
        public static void SaveQuotes(
            IIMDbMovie movie, 
            string xmlPath, 
            bool overwrite)
        {
            System.Xml.XmlTextWriter xmlWr;

            try
            {
                if (File.Exists(xmlPath) == false
                    || overwrite)
                    if (movie.Quotes.Count > 0)
                    {
                        xmlWr = new System.Xml.XmlTextWriter(xmlPath, Encoding.Default)
                                    {Formatting = System.Xml.Formatting.Indented};

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
        public static void SaveTrivia(
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
        public static void SaveGoofs(
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
        public static IList<IList<IIMDbQuote>> ReadQuotes(
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
        public static IList<IIMDbGoof> ReadGoofs(
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
        public static IList<string> ReadTrivia(
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
