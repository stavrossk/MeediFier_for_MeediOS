using System;
using System.IO;
using System.Threading;
using MediaFairy.ImportingEngine;
using MediaFairy.MediaFileDescriptors;
using MeediOS;

namespace MediaFairy.VideoFingerprintIdentifier
{
    class VideoFingerprintIdentifier
    {
        internal static string IdentifyMovieByVideoFingerprint(IMLItem item, ConnectionResult connectionresult, bool FileServerIsOnline, bool IsUNC, string location, string parent )
        {

            #region function variables

            string moviehash = Helpers.GetTagValueFromItem(item,"VideoHash");
            string imdbid = Helpers.GetTagValueFromItem(item,"ImdbID");
            #endregion


            if (!String.IsNullOrEmpty(imdbid))
                return imdbid;

            #region Compute Hash
            if (String.IsNullOrEmpty(moviehash))
            {
                //if ( Importer.EnableHashing)
                //{
                if (!IsUNC || FileServerIsOnline)
                {
                    if (File.Exists(location))
                    {
                        Debugger.LogMessageToFile("Computing video fingerprint for " + item.Name + "...");
                        MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress, "Computing video fingerprint for " + item.Name + "...");
                        Thread.Sleep(200);
                        moviehash = Hasher.ComputeHash(location, item);
                        item.Tags["VideoHash"] = moviehash;
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
            else moviehash = Helpers.GetTagValueFromItem(item,"VideoHash");

            item.SaveTags();
            #endregion


            MovieDescriptorReader.ReadMovieDescriptor(item, moviehash, parent, item.Name );


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
    }




}
