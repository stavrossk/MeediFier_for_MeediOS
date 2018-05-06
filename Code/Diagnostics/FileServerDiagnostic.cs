using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace MeediFier.Code.Diagnostics
{




    class FileServerDiagnostic
    {


        internal static bool CheckFileServer
            (bool fileServerDiagnosticIsEnabled, 
             string location, ref bool isUNC,
             ref bool fileServerChecked, string root,
             ref bool fileServerIsOnline)
        {

            if (fileServerDiagnosticIsEnabled)
            {

                if (location.StartsWith("\\"))
                {
                    isUNC = true;

                    if ( !fileServerChecked)
                    {
                        Debugger.LogMessageToFile("Checking file server availability..");
                        Helpers.UpdateProgress("Performing Diagnostic operations", "Checking file server availability...", null);
                        //Progress.Progress(Importer.CurrentProgress, "Checking file server availability...");
                        
                        if (!Directory.Exists(root))
                        {
                            Debugger.LogMessageToFile("Your file server is offline. MediaFairy will skip all video files which do not reside locally.");

                            Helpers.UpdateProgress("Performing Diagnostic operations", "Your file server is offline. MediaFairy will skip all video files which do not reside locally.", null);
   
                            StatusForm.statusForm.TrayIcon.ShowBalloonTip(3000, "Media server is offline", "Your media server is offline. MediaFairy will skip file operations for all media files which do not reside locally.", ToolTipIcon.Warning);
                            Thread.Sleep(2000);
                            
                            fileServerIsOnline = false;
                            fileServerChecked = true;

                            return false;
                        }


                        Debugger.LogMessageToFile("Your media server is online!");
                        fileServerIsOnline = true;
                        fileServerChecked = true;
                        return true;
                    }


                }


            }

            return true;
        }
    }
}
