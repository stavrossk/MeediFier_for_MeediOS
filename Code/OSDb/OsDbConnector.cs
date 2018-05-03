using System;
using System.Threading;
using System.Windows.Forms;
using MeediFier.ImportingEngine;

namespace MeediFier.OSDb
{

    internal static class OsDbConnector
    {


        internal static ConnectionResult ConnectToOsDb
            (ConnectionResult connectionResult)
        {

            if (!connectionResult.InternetConnectionAvailable
                &&
                Settings.ConnectionDiagnosticsEnabled)
            {
                connectionResult.OSDbIsOnline = false;
                return connectionResult;
            }



            if (!Settings.EnableVideoHasher &&
                !Settings.FilmOSDbDetailsDownloaderIsEnabled)
            {
                connectionResult.OSDbIsOnline = false;
                return connectionResult;
            }



            OSoperations osDbOperations = new OSoperations();

            connectionResult = 
                TryConnection(osDbOperations, connectionResult);


            return connectionResult;


        }



        private static ConnectionResult TryConnection(OSoperations osDbOperations, ConnectionResult connectionResult)
        {
            Helpers.UpdateProgress("Performing Diagnostic Operations... ",
                                   "Connecting to OSDb...", null);

            Debugger.LogMessageToFile("Connecting to OSDb...");
            Thread.Sleep(2000);

            try
            {
                connectionResult =
                    OsDbConnectionFirstTry
                        (osDbOperations, connectionResult);

                Debugger.LogMessageToFile("Connection to OSDb was sucessfull!");
                Thread.Sleep(2000);

            }
            catch (Exception)
            {
                Debugger.LogMessageToFile("Connection to OSDb failed. Retrying...");

                connectionResult =
                    OsDbConnectionRetry
                        (osDbOperations, connectionResult);
            }

            return connectionResult;

        }


        internal static ConnectionResult OsDbConnectionRetry
            (OSoperations osDbOperations,
             ConnectionResult connectionResult)
        {
            MainImportingEngine.ThisProgress.Progress
                (MainImportingEngine.CurrentProgress,
                 "OSdb did not respond. Retrying... ");

            Thread.Sleep(2000);

            try
            {
                connectionResult.OsDbLoginResult =
                    osDbOperations.SiteLogin();

                MainImportingEngine.ThisProgress.Progress
                    (MainImportingEngine.CurrentProgress,
                     "login status: " +
                     connectionResult.OsDbLoginResult.status);

                Thread.Sleep(500);

                connectionResult.OSDbIsOnline = true;

            }
            catch (Exception)
            {
                connectionResult =
                    DisableOsDbConnection(connectionResult);
            }

            return connectionResult;


        }


        internal static ConnectionResult OsDbConnectionFirstTry
            (OSoperations osDbOperations,
             ConnectionResult connectionResult)
        {
          
            connectionResult.OsDbLoginResult =
                osDbOperations.SiteLogin();

            MainImportingEngine.ThisProgress.Progress
                (MainImportingEngine.CurrentProgress,
                 "login status: " +
                 connectionResult.OsDbLoginResult.status);

            Thread.Sleep(500);

            connectionResult.OSDbIsOnline = true;

            return connectionResult;

        }


        internal static ConnectionResult DisableOsDbConnection
            (ConnectionResult connectionResult)
        {

            MainImportingEngine.ThisProgress.Progress
                (MainImportingEngine.CurrentProgress,
                 "Unable to connect to OSdb online database. ");

            StatusForm.statusForm.TrayIcon.ShowBalloonTip
                (5000, "Unable to connect to OSDb",
                 "MediaFairy was unable to connect to " +
                 "OpenSubtitles.org databse." +
                 "Video Fingerprint Identifying and Subtitle Downloading" +
                 "will not be performed in this session using this source.",
                 ToolTipIcon.Warning);

            Thread.Sleep(2000);

            connectionResult.OSDbIsOnline = false;

            return connectionResult;
        
        }
   
    
    }

}
