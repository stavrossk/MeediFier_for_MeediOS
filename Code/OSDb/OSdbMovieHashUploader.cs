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
using System.IO;
using System.Threading;
using CookComputing.XmlRpc;
using MediaFairy.ImportingEngine;
using MeediOS;

namespace MediaFairy
{
    public static class OSdbMovieHashUploader
    {
        public static bool UploadVideoHash
            (IMLItem Item, string location, string imdbid, 
            ConnectionResult connectionresult, bool FileServerIsOnline, bool IsUNC)
        {

            if (String.IsNullOrEmpty(imdbid))
            {
                Debugger.LogMessageToFile("[OSdb movie hash uploader] This video's IMDb ID is not available. Uploading of it's file hash cannot be performed.");
                return false;
            }

            if (!FileServerIsOnline && IsUNC)
            {
                Debugger.LogMessageToFile("[OSdb movie hash uploader] Your file server is offline. ");
                return false;
            }

            if (!File.Exists(location))
                return false;

            string moviehash = Helpers.GetTagValueFromItem(Item, "VideoHash");

            if (String.IsNullOrEmpty(moviehash))
                return false;

           
            #region get file info
            Debugger.LogMessageToFile("[OSdb movie hash uploader] Extracting file information for movie hash uploading...");
            FileInfo file = new FileInfo(location);
            long filesize = file.Length;
            string imdbidNumOnly = imdbid.Remove(0, 2);      
            #endregion

            return PerformUploading(Item, moviehash, connectionresult, filesize, imdbidNumOnly);
        
        }



        private static bool PerformUploading(IMLItem Item, string moviehash, ConnectionResult connectionresult, long filesize, string imdbidNumOnly)
        {
            try
            {
                MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress, "Updating video definitions database...");
                InsertVideoHash(Item, connectionresult.OsDbLoginResult.token, moviehash, filesize,
                                             imdbidNumOnly);
                return true;
            }
            catch
            {
                MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress,
                                               "Osdb database did not respond. Retrying...");
                Thread.Sleep(3000);
                try
                {
                    InsertVideoHash(Item, connectionresult.OsDbLoginResult.token, moviehash, filesize,
                                                 imdbidNumOnly);
                    return true;
                }
                catch
                {
                    Debugger.LogMessageToFile("Unable to communicate with OSdb. Video hash was not uploaded.");
                    MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress,
                                                   "Unable to communicate with OSdb. Video hash was not uploaded.");
                    return false;
                }

            }


        }//endof method


        private static void InsertVideoHash(IMLItem Item, string token, string videohash, double moviebytesize, string imdbid)
        {
            HashUploadParams[] paramS = new HashUploadParams[1];
            paramS[0].moviehash = videohash;
            paramS[0].imdbid = imdbid;
            paramS[0].moviebytesize = moviebytesize;

            Debugger.LogMessageToFile("Attempting to uplad video hash...");
            XmlRpcStruct result = OSoperations.Proxy.InsertMovieHash(token, paramS);

            #region scan entries, extract accepted hashes
            foreach (System.Collections.DictionaryEntry d in result)
            {
                token = Convert.ToString(d.Key);

                if (token == "status")
                    Debugger.LogMessageToFile("Upload status: " + d.Value);

                #region parse the returned 'data' table and return accepted hashes

                if (token != "data") continue;

                object value = d.Value;
                XmlRpcStruct s = (XmlRpcStruct)value;

                foreach (System.Collections.DictionaryEntry e in s)
                {
                    token = Convert.ToString(e.Key);

                    if (token != "accepted_moviehashes") continue;

                    object[] acceptedHases = (object[])e.Value;

                    if (acceptedHases.Length > 0)
                    {
                        string acceptedHash = (string)acceptedHases[0];
                        MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress, "The video fingerprint of \"" + Item.Name + "\" was uploaded succesfully.");
                        Debugger.LogMessageToFile("The video fingerprint '" + acceptedHash + "' of \"" + Helpers.GetTagValueFromItem(Item,"Title") + "\" was uploaded succesfully.");
                        Thread.Sleep(2000);
                    }
                    else
                    {
                        Debugger.LogMessageToFile("This video hash already exists in the online database.");
                        //Importer.thisProgress.Progress(Importer.CurrentProgress, "This video fingerprint already exists in the online database.");
                        //Thread.Sleep(600);
                    }
                }

                #endregion

            }
            #endregion

        }
    }//endof class

}//endof namespace