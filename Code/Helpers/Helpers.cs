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
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Net;
//using TheMovieDB;
//using WrapNetflix;


#if USE_MEEDIO

#elif USE_MEEDIOS
using MeediFier.Code.RegEx_Matchers;
using MeediFier.Diagnostics;
using MeediFier.ImportingEngine;
using MeediFier.OSDb;
using MeediOS;

#endif


namespace MeediFier
{

    public static class Helpers
    {


        #region class varibales
        private static WebClient client = new WebClient();
        //private static IWshRuntimeLibrary.WshShell shell = new IWshRuntimeLibrary.WshShell();
        public static IList<IIMDbSearchResult> IMDbResults;
        #endregion



        internal static void UpdateProgress
            (string generalStatus,
            string specialStatus, IMLItem item)
        {

            Application.DoEvents();

            string importerText = String.IsNullOrEmpty(specialStatus)
                ? generalStatus : specialStatus;

            if (MainImportingEngine.ThisProgress == null)
                return;

            if (StatusForm.statusForm == null)
                return;


            MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress, importerText);

            StatusForm.statusForm.UpdateProgress(MainImportingEngine.CurrentProgress);

            StatusForm.statusForm.statusLabel.Text =
                !String.IsNullOrEmpty(generalStatus)
                ? generalStatus 
                : MainImportingEngine.GeneralStatus;
           
            Application.DoEvents();

            if (item != null)
            {
                StatusForm.statusForm.ItemNameLabel.Text = item.Name;

                string locationTmp = item.Location.Trim('|');
                string[] locations = locationTmp.Split('|');

                for (int i = 0; i < locations.Length; i++)
                {
                    if (i < 3)
                    StatusForm.statusForm.ItemLocationlistBox.Items[i] = locations[i];

                }


                StatusForm.statusForm.SpecialStatusLabel.Text = specialStatus;
            }

            Application.DoEvents();

            StatusForm.statusForm.Refresh();


        }
 



        public static void FileIsInUse
            (string location,
            bool fileServerIsOnline,
            IMLItem item)
        {

            if (!fileServerIsOnline) 
                return;
            
            
            try
            {
                Debugger.LogMessageToFile("Checking if file is in use...");
                FileStream filestream = File.OpenRead(location);
                filestream.Close();
                Settings.FileInUse = false;
            }
            catch (Exception)
            {
                Debugger.LogMessageToFile("The file " + location + " is in use by another program.");
                Helpers.UpdateProgress("Performing diagnostics", "This item's media file " + location + " is in use by another program. MediFairy can not perform this operation.",  item);
                StatusForm.statusForm.TrayIcon.ShowBalloonTip(5000, "Media file in use or unavailable", "The item's " + item.Name + " media file is in use or unavailable." + Environment.NewLine + 
                                                                                                        "MediaFairy was unable to perform an operation which required the exclusive use of that file.", ToolTipIcon.Warning);
                Thread.Sleep(1500);
                Settings.FileInUse = true;
            }
        }



        public static bool  UserCancels(string specialStatus, IMLItem item)
        {

            Application.DoEvents();

            if (MainImportingEngine
                .ThisProgress.Progress
                (MainImportingEngine
                .CurrentProgress,
                specialStatus)
                && !Settings.UserCancels)
                return false;


            if (Settings.EnableMoviesSectionUpdater && MediaSectionsAllocator.MoviesSection != null)
                MediaSectionsAllocator.MoviesSection.EndUpdate();

            if (Settings.EnableTvShowsSectionUpdater && MediaSectionsAllocator.TvEpisodesSection != null)
                MediaSectionsAllocator.TvEpisodesSection.EndUpdate();


            if (Settings.EnableMusicSectionUpdater && MediaSectionsAllocator.MusicSection != null)
                MediaSectionsAllocator.MusicSection.EndUpdate();

            Application.DoEvents();

            MainImportingEngine.CurrentProgress = 100;
            UpdateProgress("All operations were cancelled. Completed jobs were saved to library.", "", item);
            //Importer.thisProgress.Progress(Importer.CurrentProgress, "All opeations were cancelled. Completed jobs were saved to library.");

            Application.DoEvents();

            StatusForm.statusForm.TrayIcon.Dispose();
            //StatusForm.statusForm.Dispose();

            Application.DoEvents();

            MainImportingEngine.ThisProgress = null;

            return true;

        }


        public static string GetTagValueFromItem(IMLItem item, string tagName)
       {

           string tagValue = string.Empty;
           try
           {
               tagValue = (string)item.Tags[tagName];
           }
           catch { }

           return tagValue;
       }


        public static string CleanHtml(string dirtyHtml)
        {

            string[] dirtyText = RegExDataMiners.MatchExpressionReturnAllMatchesFirstGroup(dirtyHtml, "<.*?>");
            if (dirtyText != null)
            {

                // ReSharper disable LoopCanBeConvertedToQuery
                foreach (string dirtyTag in dirtyText)
                // ReSharper restore LoopCanBeConvertedToQuery
                {
                    dirtyHtml = dirtyHtml.Replace(dirtyTag, String.Empty);
                }

            }
            string cleanHtml = dirtyHtml;
            return cleanHtml;
        }


        public static ConnectionResult CheckInternetConnectionAndConnectToOSDb()
        {

            ConnectionResult connectionresult =
                InternetConnectionDiagnostic.CheckInternetConnection();

            connectionresult = 
                OsDbConnector.ConnectToOsDb(connectionresult);


            return connectionresult;

        }


        public static void SaveStreamToFile(Byte[] stream, String fileName)
        {
            FileStream file = new FileStream(fileName, FileMode.Create);
            file.Write(stream, 0, stream.Length);
            file.Flush();
            file.Close();
        }




    }




}
