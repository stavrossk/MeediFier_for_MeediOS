
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

namespace MediaFairy.IMDb
{
    class IMDbAdvancedFilmSearchEngine
    {

        public static void SearchForFilmReturnResults(string Title, string Year)
        {

            string searchURL = ConstructSearchURL();

            string html = HtmlDownloaders.DownloadHTMLfromURL(searchURL);


        }



        private static string ConstructSearchURL()
        {

           string baseURL           =   "http://www.imdb.com/search/title?";
           string commonCountries   =   "countries=%C2%B7%C2%B7%C2%B7%C2%A0Common%20Countries%C2%A0%C2%B7%C2%B7%C2%B7";
           string productionStatus  =   "&production_status=released";
           string releaseDate       =   "&release_date=2008,2008";
           string title             =   "&title=The%20day%20the%20earth%20stood%20still";
           string titleType         =   "&title_type=feature,tv_movie,documentary";

           string searchURL = baseURL + commonCountries + productionStatus + releaseDate + title + titleType;

           return searchURL;

        }









    }
}
