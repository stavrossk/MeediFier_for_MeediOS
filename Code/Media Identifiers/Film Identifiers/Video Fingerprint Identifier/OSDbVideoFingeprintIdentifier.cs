using System;
using MeediFier.ImportingEngine;
using MeediOS;

namespace MeediFier.VideoFingerprintIdentifier
{
    class OSDbVideoFingeprintIdentifier
    {
        internal static string IdentifyMovieByHashOSDb(IMLItem item, ConnectionResult connectionresult)
        {


            #region function variables

            string moviehash = Helpers.GetTagValueFromItem(item, "Hash");
            string imdbid = Helpers.GetTagValueFromItem(item, "ImdbID");
            #endregion


            if (!Settings.EnableVideoHasher)
                return imdbid;

            if (!connectionresult.InternetConnectionAvailable && Settings.ConnectionDiagnosticsEnabled)
                return imdbid;

            if (!connectionresult.OSDbIsOnline)
                return imdbid;

            if (!String.IsNullOrEmpty(imdbid))
                return imdbid;

            if (String.IsNullOrEmpty(moviehash))
                return imdbid;


            #region Get ImdBID


            imdbid = OSoperations.FindImdbIDbyHash2(moviehash, item, connectionresult.OsDbLoginResult.token, ref connectionresult.OSDbIsOnline);

            #region Construct correct IMDbID in case it was received wrong
            if (!String.IsNullOrEmpty(imdbid))
            {
                if (!imdbid.StartsWith("tt"))
                {
                    if (imdbid.Length == 6)
                        imdbid = "0" + imdbid;

                    imdbid = "tt" + imdbid;
                }
                else
                {
                    if (imdbid.Length == 8)
                    {
                        imdbid = imdbid.Remove(0, 2);
                        imdbid = "0" + imdbid;
                        imdbid = "tt" + imdbid;
                    }
                }
            }
            #endregion

            #region Save found IMDbID to item's tag
            if (!String.IsNullOrEmpty(imdbid))
            {

                Debugger.LogMessageToFile
                    ("Online identification by video fingerprint" +
                     " (using OSDb) was successful!");


                MainImportingEngine.ThisProgress.Progress
                    (MainImportingEngine.CurrentProgress,
                    "Online identification by" +
                    " video fingerprint (using OSDb)" +
                    " was succesfull!");


                //Thread.Sleep(200);

                item.Tags["ImdbID"] = imdbid;
                item.SaveTags();

            }
            else
            {

                Debugger.LogMessageToFile
                    ("Online identification by video's" +
                     " fingerprint (using OSDb)" +
                     " was not possible.");


            }
            #endregion

            #endregion


            return imdbid;
        }
    }



}
