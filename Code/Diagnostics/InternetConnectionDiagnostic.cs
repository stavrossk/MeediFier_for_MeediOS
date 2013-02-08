using System;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using MediaFairy.ImportingEngine;


// ReSharper disable CheckNamespace
namespace MediaFairy.Diagnostics
// ReSharper restore CheckNamespace
{


    class InternetConnectionDiagnostic
    {


        internal static ConnectionResult CheckInternetConnection()
        {


            ConnectionResult connectionresult = new ConnectionResult();

            if (!Settings.ConnectionDiagnosticsEnabled)
                return connectionresult;

            
            Helpers.UpdateProgress("Performing Diagnostic Operations...",
                                   "Checking Internet Connection...", null);

            Thread.Sleep(1000);

            try
            {

                bool pingResult = Ping("www.google.com");

                connectionresult =
                    SetConnectionFlagBasedOnPingResult(pingResult);

                

            }
            catch (Exception e)
            {
                connectionresult = InternetConnectionWasNotDetected();
                Debugger.LogMessageToFile(e.ToString());
            }


            return connectionresult;


        }



        private static ConnectionResult SetConnectionFlagBasedOnPingResult(bool pingResult)
        {
            ConnectionResult connectionresult =
                pingResult
                ? InternetConnectionTestWasSuccessful()
                : InternetConnectionWasNotDetected();

            return connectionresult;
        }


        private static ConnectionResult InternetConnectionTestWasSuccessful()
        {
            ConnectionResult connectionresult = new ConnectionResult();

            Helpers.UpdateProgress("Performing Diagnostic Operations...",
                                   "Internet connection test was successful.", null);

            Thread.Sleep(3000);

            connectionresult.InternetConnectionAvailable = true;
            return connectionresult;
        }


        private static ConnectionResult InternetConnectionWasNotDetected()
        {
            ConnectionResult connectionresult = new ConnectionResult();

            StatusForm.statusForm.TrayIcon.ShowBalloonTip(1000,
                        "Internet connection was not detected.",
                        "Internet Connection diagnostic was unable" +
                        " to etabilish a test connection. " +
                        "All online features will be disabled." +
                        "", ToolTipIcon.Info);

            MainImportingEngine.ThisProgress.
                Progress(MainImportingEngine.CurrentProgress,
                         "Internet Connection was not detected." +
                         " All online features will be disabled. ");

            Thread.Sleep(3000);

            connectionresult.InternetConnectionAvailable
                = false;

            return connectionresult;
        
        }


        internal static bool Ping(string hostname)
        {
            Ping pingSender = new Ping();
            PingOptions options = new PingOptions();


            // Create a buffer of 32 bytes of data to be transmitted.
            const string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            byte[] buffer = Encoding.ASCII.GetBytes(data);

            //TODO: Move the Ping timeout to the settings file 
            const int timeout = 10000;

            PingReply reply = pingSender.Send(hostname, timeout, buffer, options);


            if (reply.Status == IPStatus.Success)
            {
                pingSender.Dispose();
                return true;
            }
            reply = pingSender.Send(hostname, timeout, buffer, options);
            if (reply.Status == IPStatus.Success)
            {
                pingSender.Dispose();
                return true;
            }
            pingSender.Dispose();
            return false;
        }
    }


}
