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


namespace MeediFier
{
    public class IMDbSearchResult : IIMDbSearchResult
    {

        private string _title = "";
        
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        private string _url = "";
        
        public string URL
        {
            get { return _url; }
            set { _url = value; }
        }

        private string _imdbId = "";
        
        public string IMDb_ID
        {
            get { return _imdbId; }
            set { _imdbId = value; }
        }

        private string _year = "";
        
        public string Year
        {
            get { return _year; }
            set { _year = value; }
        }

        public IMDbSearchResult()
        {
            Ignore = false;
        }

        public bool Ignore { get; set; }
    }
}
