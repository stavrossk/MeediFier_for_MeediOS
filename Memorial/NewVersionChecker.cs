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



using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace MediaFairy
{
    public static class NewVersionChecker
    {

        //DEPRECATED: New Version Checker
        public static bool CheckForNewVersion()
        {

            if (!Settings.AutomaticUpdating)
                return false;

            //string pluginpath = Debugger.GetPluginPath();
            WebClient client = new WebClient();
            string currentVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Importer.ThisProgress.Progress(Importer.CurrentProgress, "Checking for new version...");

            string text = HtmlDownloaders.DownloadHTMLfromURL("http://www.meedios.com/OpenMAIDOS/detail.php?plugin_id=3AE69DCF-B3C6-4DD3-85A8-2744A3B50452&ptype=import&program=meedio");

            string latestVersion = Helpers.MatchExpressionReturnFirst(text,"&plugin_version=(?<Version>.*?)&");
            string latestVersionShort = latestVersion;
            latestVersion = latestVersion + ".0";

            //MessageBox.Show("Current version: " + CurrentVersion);
            //MessageBox.Show("Latest version: " + LatestVersion);


            if (latestVersion == currentVersion)
                return false;

            string tempFolderPath = Path.GetTempPath();


            Importer.ThisProgress.Progress(Importer.CurrentProgress, "Downloading new version...");
            client.DownloadFile("http://www.meedios.com/OpenMAIDOS/dl.php?plugin_id=3AE69DCF-B3C6-4DD3-85A8-2744A3B50452&plugin_version=" + latestVersionShort + "&plugin_state=Beta", tempFolderPath + "MediaFairy.mopp");


           
            Process p = new Process
                            {
                                StartInfo = {FileName = tempFolderPath + "MediaFairy.mopp"}
                            };
            p.Start();


            #region All Done.
            StatusForm.statusForm.UpdateProgress(100);
            StatusForm.statusForm.TrayIcon.ShowBalloonTip(500, "New version downloaded.", "A new version of MediaFairy was downloaded. Please close MeediOS Configuration Editor before running the installer", ToolTipIcon.Info);
            Importer.CurrentProgress = 100;
            Importer.ThisProgress.Progress(Importer.CurrentProgress, "Please close Configuration Editor before running the installer.");
            Debugger.LogMessageToFile("MediaFairy completed successfully!");
            Thread.Sleep(1000);
            StatusForm.statusForm.TrayIcon.Dispose();
            StatusForm.statusForm.Close();
            Application.Exit();
            #endregion



            return true;



        }
    }
}