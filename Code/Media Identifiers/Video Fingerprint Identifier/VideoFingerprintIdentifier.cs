using System;
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




        internal static string IdentifyMovieByVideoFingerprint
            (IMLItem item, ConnectionResult connectionresult,
            bool fileServerIsOnline, bool IsUNC,
            string location, string parent )
        {

            #region function variables

            string moviehash = Helpers.GetTagValueFromItem(item,"Hash");
            string imdbid = Helpers.GetTagValueFromItem(item,"ImdbID");
            #endregion


            if (!String.IsNullOrEmpty(imdbid))
                return imdbid;

            #region Compute Hash
            if (String.IsNullOrEmpty(moviehash))
            {
                //if ( Importer.EnableHashing)
                //{
                if (!IsUNC || fileServerIsOnline)
                {
                    if (File.Exists(location))
                    {
                        Debugger.LogMessageToFile("Computing video fingerprint for " + item.Name + "...");
                        MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress, "Computing video fingerprint for " + item.Name + "...");
                        Thread.Sleep(200);
                        moviehash = Hasher.ComputeHash(location, item);
                        item.Tags["Hash"] = moviehash;
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
            else moviehash = Helpers.GetTagValueFromItem(item,"Hash");

            item.SaveTags();
            #endregion


            MovieDescriptorReader.ReadMovieDescriptor
                (item, moviehash,
                parent, item.Name );


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
            (ref string imdbid, ref string TMDbID, 
             ref IMDbOperations imdbOP, IMLItem Item, 
             bool fileServerIsOnline, bool IsUNC,
             string location, string parent, string Year, 
             ConnectionResult connectionresult,
             IMLSection moviesSection)
        {

            if (!connectionresult.InternetConnectionAvailable && Settings.ConnectionDiagnosticsEnabled)
                return false;


            imdbid = IdentifyFilmByFingerprintOrTitle(Item,imdbid,connectionresult,fileServerIsOnline,IsUNC,location,parent,imdbOP, Year);

          


            TMDbID = TheMovieDb.GetTmdbID(Item);

            return true;
        }

        private static string IdentifyFilmByFingerprintOrTitle
            (IMLItem Item, string imdbid,
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
                     IsUNC, location, parent);
             
                imdbid = IMDbTitleIdentifier.IdentifyVideoByTitle
                    (Item,imdbid,imdbOP,
                     year,connectionresult);
          
           
            }
            else
            {
                IMDbTitleIdentifier.IdentifyVideoByTitle(Item,imdbid,imdbOP,year,connectionresult);
                imdbid = IdentifyMovieByVideoFingerprint(Item, connectionresult, fileServerIsOnline, IsUNC, location, parent);         
            }


            return imdbid;
        }


    }




}
