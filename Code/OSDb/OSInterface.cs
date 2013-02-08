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
using CookComputing.XmlRpc;

namespace MediaFairy
{

    public interface IOpenSubtitlesRemoteFunctions : IXmlRpcProxy
    {
        [XmlRpcMethod("LogIn")]
        LoginResult LogIn(string username, string password, string language, string useragent);

        [XmlRpcMethod("ServerInfo")]
        XmlRpcStruct ServerInfo();

        [XmlRpcMethod("CheckMovieHash")]
        XmlRpcStruct CheckMovieHash(string moviehash);

        [XmlRpcMethod("CheckMovieHash2")]
        XmlRpcStruct CheckMovieHash2(string moviehash);

        [XmlRpcMethod("GetIMDBMovieDetails")]
        XmlRpcStruct GetImdbMovieDetails(string token, string imdbid);

        [XmlRpcMethod("SearchSubtitles")]
        XmlRpcStruct SearchSubtitles(string token, SearchParams[] ms);

        [XmlRpcMethod("CheckMovieHash")]
        XmlRpcStruct CheckMovieHash(string token, string[] moviehash);

        [XmlRpcMethod("InsertMovieHash")]
        XmlRpcStruct InsertMovieHash(string token, HashUploadParams[] uploadParams);

    }
 

}
