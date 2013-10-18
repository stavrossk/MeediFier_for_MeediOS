using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace MeediFier.Code
{
    class FileServerChecker
    {
        internal static bool CheckFileServer
            (bool EnableFileServerDiagnostics, 
             string location, ref bool IsUNC,
             ref bool FileServerChecked, string root,
             ref bool FileServerIsOnline)
        {

            if (EnableFileServerDiagnostics)
            {

                if (location.StartsWith("\\"))
                {
                    IsUNC = true;

                    if ( !FileServerChecked)
                    {
                        Debugger.LogMessageToFile("Checking file server availability..");
                        Helpers.UpdateProgress("Performing Diagnostic operations", "Checking file server availability...", null);
                        //Progress.Progress(Importer.CurrentProgress, "Checking file server availability...");
                        
                        if (!Directory.Exists(root))
                        {
                            Debugger.LogMessageToFile("Your file server is offline. MediaFairy will skip all video files which do not reside locally.");
                            Helpers.UpdateProgress("Performing Diagnostic operations", "Your file server is offline. MediaFairy will skip all video files which do not reside locally.", null);
                            //Progress.Progress(Importer.CurrentProgress, "Your media server is offline. MediaFairy will skip all video files which do not reside locally.");
                            //StatusForm.statusForm.TrayIcon.ShowBalloonTip(3000, "Media server is offline", "Your media server is offline. MediaFairy will skip file operations for all media files which do not reside locally.", ToolTipIcon.Warning);
                            Thread.Sleep(2000);
                            FileServerIsOnline = false;
                            FileServerChecked = true;

                            return false;
                        }


                        Debugger.LogMessageToFile("Your media server is online!");
                        FileServerIsOnline = true;
                        FileServerChecked = true;
                        return true;
                    }


                }


            }

            return true;
        }
    }
}
