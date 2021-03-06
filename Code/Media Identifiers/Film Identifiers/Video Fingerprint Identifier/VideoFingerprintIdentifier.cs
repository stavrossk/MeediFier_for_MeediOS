﻿using System;
using System.IO;
using System.Threading;
using MeediFier.IMDb;
using MeediFier.ImportingEngine;
using MeediFier.MediaFileDescriptors;
using MeediOS;

namespace MeediFier.VideoFingerprintIdentifier
{


    class VideoFingerprintIdentifier
    {



        //TODO: This function should also be used for identifying for TV episodes with change to the ReadMovieDescripton code.
        internal static string IdentifyMovieByVideoFingerprint
            (IMLItem item, ConnectionResult connectionresult,
            bool fileServerIsOnline, bool isUNC,
            string location, string parent, ref string videoHash )
        {

            #region function variables

            if (String.IsNullOrEmpty(videoHash))
            {
                videoHash = Helpers.GetTagValueFromItem(item, "Hash");
            }

            string imdbid = Helpers.GetTagValueFromItem(item,"ImdbID");
            
            #endregion


            if (!String.IsNullOrEmpty(imdbid))
                return imdbid;

            #region Compute Hash
            if (String.IsNullOrEmpty(videoHash))
            {
                //if ( Importer.EnableHashing)
                //{
                if (!isUNC || fileServerIsOnline)
                {
                    if (File.Exists(location))
                    {
                        Debugger.LogMessageToFile("Computing video fingerprint for " + item.Name + "...");
                        MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress, "Computing video fingerprint for " + item.Name + "...");
                        Thread.Sleep(200);
                        videoHash = Hasher.ComputeHash(location, item);
                        item.Tags["Hash"] = videoHash;
                        item.SaveTags();
                    }
                    else
                    {
                        MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress, "The video file in location field was not found. MediaFairy cannot identify this film.");
                        Thread.Sleep(Settings.SleepValue);
                    }
                }
                //}
            }
            else videoHash = Helpers.GetTagValueFromItem(item,"Hash");

            item.SaveTags();
            #endregion


            //MovieDescriptorReader.ReadMovieDescriptor
            //    (item, moviehash,
            //    parent, item.Name );


            if (Settings.PrimaryVideoFingerprintMatchingSource == "OSDb")
            {
                OSDbVideoFingeprintIdentifier.IdentifyMovieByHashOSDb(item, connectionresult);
                imdbid = TMDbVideoFingerprintIdentifier.IdentifyMovieByHashTMDb(item, connectionresult);
            }
            else
            {
                TMDbVideoFingerprintIdentifier.IdentifyMovieByHashTMDb(item, connectionresult);
                imdbid = OSDbVideoFingeprintIdentifier.IdentifyMovieByHashOSDb(item, connectionresult);
            }

            return imdbid;
        }


        internal static bool IdentifyVideo
            (ref string moviehash, ref string imdbid, ref string tmdbID, 
             ref IMDbOperations imdbOp, IMLItem item, 
             bool fileServerIsOnline, bool isUNC,
             string location, string parent, string year, 
             ConnectionResult connectionresult,
             IMLSection moviesSection)
        {

            if (!connectionresult.InternetConnectionAvailable && Settings.ConnectionDiagnosticsEnabled)
                return false;


            imdbid = IdentifyFilmByFingerprintOrTitle
                (item, ref moviehash, imdbid, connectionresult,
                 fileServerIsOnline, isUNC,
                 location, parent, imdbOp, year);

          
            //TMDbID = TheMovieDb.GetTmdbIdByImdbId(item);

            TheMovieDb.TmdbGetTmdbIdByFilmImdbId(item);

            return true;
        
        }





        private static string IdentifyFilmByFingerprintOrTitle
            (IMLItem Item, ref string moviehash, string imdbid,
             ConnectionResult connectionresult,
             bool fileServerIsOnline,
             bool IsUNC, string location, 
             string parent, IMDbOperations imdbOP, 
             string year)
        {

            if (Settings.PrimaryFilmIdentificationMethod == "by video fingerprint")
            {


                imdbid = IdentifyMovieByVideoFingerprint
                    (Item, connectionresult, fileServerIsOnline,
                     IsUNC, location, parent, ref moviehash);
             
                imdbid = FilmTitleMatchingEngine.IdentifyFilmByFilmTitleMatchingEngines
                    (Item,imdbid,imdbOP,
                     year,connectionresult);
          
           
            }
            else
            {

                FilmTitleMatchingEngine.IdentifyFilmByFilmTitleMatchingEngines
                    (Item,imdbid,imdbOP,year,connectionresult);
                
                imdbid = IdentifyMovieByVideoFingerprint
                    (Item, connectionresult, fileServerIsOnline, 
                    IsUNC, location, parent, ref moviehash);         
            
            }


            return imdbid;
        }


    }




}
