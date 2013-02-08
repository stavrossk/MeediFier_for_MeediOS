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
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MediaFairy
{
    static class ScraperXML
    {

        #region ScraperXML
        //string[] scraperLocations = new string[1];
        //scraperLocations[0]= "scrapers\\video\\imdbid.xml";
        //_imdbList = Helpers.SearchScrapers(_title, scraperLocations);
        #endregion

/*
        public static IList<IMDbSearchResult> SearchScrapers(string Title, string[] ScrapersLocations)
        {

            foreach (string ScraperLocation in ScrapersLocations)
            {
                ScraperXML.ScraperParser scraper = new ScraperXML.ScraperParser();
                scraper.Load(ScraperLocation);
                string searchUrl = scraper.CreateSearchUrl(Title);
                XElement searchResults = scraper.GetSearchResults(searchUrl);
                IEnumerable<XElement> Items = searchResults.Elements();
            
   
                foreach (XElement item in Items )
                {
                    IMDbSearchResult result = new IMDbSearchResult();
                    string title = item.Element("title").Value;
                    result.Title = title;
                    string id = item.Element("id").Value;
                    result.IMDb_ID = id;
                    string year = item.Element("year").Value;
                    result.Year = year;
                    string url = item.Element("url").Value;
                    result.URL = url;

                    result.Ignore = false;

                    Helpers.IMDbResults.Add(result);
                }

                if (Helpers.IMDbResults.Count > 0)
                    return Helpers.IMDbResults;

            }


            return null;
        }
*/
    }
}
