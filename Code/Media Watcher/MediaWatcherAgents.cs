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
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Windows.Forms;
using System.ServiceProcess;
using System.IO;
using System.Threading;

#if USE_MEEDIO 
using Meedio; 
#elif USE_MEEDIOS
using MediaFairy.ImportingEngine;
using MediaFairy.MediaSnapshotEngine;
using MediaInfoLib;
using MeediOS;
#endif


namespace MediaFairy
{

    class MediaWatcherAgents
    {





        public static bool ComplyMediaWatcherRequest(IMLSection moviesSection,
            IMLSection tvSection, IMLSection musicSection,
            IMLSection booksSection, ref ArrayList extensionsToIgnore,
            ref string[] filmLocations, ref string[] tvshowsLocations,
            ref string[] musicLocations, string[] videoexts, 
            string[] audioexts, IEnumerable<string> combinedSceneTags,
            MediaInfo mediaInfo, IEnumerable<string> videoExtensionsCommon, Importer importer)
        {

            try
            {

            string pluginPath = Debugger.GetPluginPath();
            string datFile = pluginPath + "MediaWatcher.snapshot";

                if (File.Exists(datFile))
                {
                    Debugger.LogMessageToFile("New media detected. Running in single-item mode...");

                    string[] files = File.ReadAllLines(datFile);

                    if (files.Length == 0)
                    {
                        File.Delete(datFile);
                        return false;
                    }

                    int currentFile = 0;

                    foreach (string file in files)
                    {
                        FileInfo fileFI = new FileInfo(file);


                        SingleMediaFileImporter.ImportMediaFile(fileFI,
                            moviesSection, tvSection, musicSection, booksSection,
                            ref extensionsToIgnore, ref filmLocations, 
                            ref tvshowsLocations,ref musicLocations, 
                            videoexts, audioexts, files.Length,
                            ref currentFile, combinedSceneTags, mediaInfo, 
                            pluginPath, videoExtensionsCommon);
                   
                    
                    }


                    ImportingEngineHelpers.FinishUpdatingSections();
                    File.Delete(datFile);
                    return true;

                }
                else return false;

            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString() );
                return false;
            }


        }
        


        public static void StartService(string serviceName, int timeoutMilliseconds)
        {
            try
            {
                Debugger.LogMessageToFile("Locating Media Watcher service...");
                ServiceController service = new ServiceController(serviceName);

                if (service.Status == ServiceControllerStatus.Running)
                    return;

                try
                {
                    TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

                    MainImportingEngine.CurrentProgress++;
                    Debugger.LogMessageToFile("Starting service...");
                    MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress, "Starting Media Watcher service...");
                    service.Start();
                    service.WaitForStatus(ServiceControllerStatus.Running, timeout);


                    MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress,
                                                   service.Status == ServiceControllerStatus.Running
                                                       ? "Media Watcher Service was started."
                                                       : "Media Watcher service could not be started. The request timed out.");
                }
                catch (InvalidOperationException e)
                {
                    StatusForm.statusForm.TrayIcon.ShowBalloonTip(5000, "MediaWatcher Service could not be started", "The access to Windows Services is restrictred for this account." + Environment.NewLine + "Please run MediaFairy with administrative privileges or start the service manually.", ToolTipIcon.Warning);
                    Thread.Sleep(10000);
                    Debugger.LogMessageToFile("Unable to start MediaWatcher service. Access was restricted. The error was: " + Environment.NewLine + e);
                    //throw e;
                }


            }
            catch (Exception e)
            {
                Debugger.LogMessageToFile("Media Watcher service was not found.");
                StatusForm.statusForm.TrayIcon.ShowBalloonTip(10000, "Media Watcher service not installed", "The MediaFairy Watcher service was not found on this system." + Environment.NewLine + "Your specified directories will not be watched for new media.", ToolTipIcon.Warning);
                Thread.Sleep(5000);
                return;
            }




        }


        public static void StopService(string serviceName, int timeoutMilliseconds)
        {
            try
            {
                ServiceController service = new ServiceController(serviceName);

                if (service.Status == ServiceControllerStatus.Stopped)
                    return;

                try
                {
                    TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);

                    MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress,
                                                   service.Status == ServiceControllerStatus.Stopped
                                                       ? "Media Watcher service stopped."
                                                       : "Media Watcher service could not be stopped. The request timed out.");
                }
                catch (InvalidOperationException e)
                {
                    StatusForm.statusForm.TrayIcon.ShowBalloonTip(5000, "MediaWatcher Service could not be stopped.", "The access to Windows Services is restrictred for this account." + Environment.NewLine + "Please run MediaFairy with administrative privileges or stop the service manually.", ToolTipIcon.Warning);
                    Debugger.LogMessageToFile("Unable to stop MediaWatcher service. Access was restricted. The error was:" + Environment.NewLine + e );
                }
            }
            catch (Exception)
            {
                Debugger.LogMessageToFile("Media Watcher service was not found.");
                //StatusForm.statusForm.TrayIcon.ShowBalloonTip(10000, "Media Watcher service not installed", "The MediaFairy Watcher service was not found on this system." + Environment.NewLine + "Your specified directories will not be watched for new media.", ToolTipIcon.Warning);
                //System.Threading.Thread.Sleep(5000);
                return;
            }


        }



        private static void SetMediaWatcherSettings(IMLSection section)
        {

            #region construct WatchedDirectories setting

            string watchedDirectories = string.Empty;
            int i = 1;

            if (Settings.RootMediaFolders != null)
            {
                foreach (string watchedDir in Settings.RootMediaFolders)
                {
                    watchedDirectories += watchedDir;

                    if (i != Settings.RootMediaFolders.Length)
                        watchedDirectories += ";";

                    i++;

                }
            }
            #endregion

            #region construct IncludeSubDirs setting

            string includeSubDirs = Settings.IncludeSubdirectories ? "true" : "false";
            #endregion

            string detectVideo;

            if (Settings.EnableMovieImporter || Settings.EnableTvSeriesImporter)
                detectVideo = "true";
            else detectVideo = "false";

            string detectAudio = Settings.EnableMusicImporter ? "true" : "false";


            string pluginDir = Debugger.GetPluginPath();


            #region Open Media Watcher settings file and set it's values

            string sys32Dir = Environment.SystemDirectory + @"\";
            string mediaWatcherSettingsFile = sys32Dir + "MediaFairyWatcher.config";

            if (File.Exists(mediaWatcherSettingsFile))
            {
                XmlDocument myXmlDocument = new XmlDocument();
                myXmlDocument.Load(mediaWatcherSettingsFile);
                XmlNode node = myXmlDocument.DocumentElement;

                #region Loop settings and set their values
                foreach (XmlNode node1 in node.ChildNodes)
                {
                    foreach (XmlNode node2 in node1.ChildNodes)
                    {
                        XmlAttributeCollection attributes = node2.Attributes;

                        if (attributes[0].Value == "WatchedDirectories")
                        {
                            Console.WriteLine(attributes[1].Value);
                            attributes[1].Value = watchedDirectories;
                        }

                        if (attributes[0].Value == "IncludeSubDirs")
                        {
                            Console.WriteLine(attributes[1].Value);
                            attributes[1].Value = includeSubDirs;
                        }

                        if (attributes[0].Value == "PluginDir")
                        {
                            Console.WriteLine(attributes[1].Value);
                            attributes[1].Value = pluginDir;
                        }

                        if (attributes[0].Value == "DetectVideo")
                        {
                            attributes[1].Value = detectVideo;
                        }

                        if (attributes[0].Value == "DetectAudio")
                        {
                            attributes[1].Value = detectAudio;
                        }

                    }
                }
                #endregion

                myXmlDocument.Save(mediaWatcherSettingsFile);
            }
            #endregion



            #region Set Current Section to RunSectionImports.xml

            string xmlFilename = Debugger.GetPluginPath() + @"MediaWatcher\RunSectionImports.xml";

            if (File.Exists(xmlFilename))
            {
                string fileContents = File.ReadAllText(xmlFilename);
                fileContents = fileContents.Replace("<plugin-property name=\"section\"></plugin-property>", "<plugin-property name=\"section\">" + section.Name + "</plugin-property>");

                File.WriteAllText(xmlFilename, fileContents);
            }
            else Debugger.LogMessageToFile("RunSectionImports.xml was not found. The current import section cannot be set and MediaFairy will not be able to run in single-item import mode.");

            #endregion





        }


        internal static void ConfigureMediaWatcher(IMLSection section)
        {

            SetMediaWatcherSettings(section);

            if (Settings.MediaWatcherEnabled)
                StartService("MediaWatcher", 10000);
            else StopService("MediaWatcher", 10000);
        }

    }//endof class


}//endof namespace
