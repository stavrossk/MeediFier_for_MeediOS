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
using System.Threading;
using System.Windows.Forms;
using MeediFier.ImportingEngine;
using MeediFier.ToolBox.Utils;


namespace MeediFier.IMDb.Title_Matching_Engine
{


    internal static class ExactMatchDetector
    {



        public static IIMDbSearchResult FindExactMatch
            (int max, string title, string year, 
            IList<IIMDbSearchResult> list, string fileDuration)
        {

            try
            {

                for (var i = 0; i < max; i++)
                {

                    if (i >= list.Count) 
                        continue;


                    if (!String.IsNullOrEmpty(year))
                    {

                        #region match using year

                        #region Match exactly same strings

                        if (ToolBox.Utils.StringProcessors.StringsMatchNormalized(list[i].Title, title) &&
                            ToolBox.Utils.StringProcessors.StringsMatchNormalized(list[i].Year, year))
                            return list[i];

                        #endregion

                        #region Match AND, Ending THE and Colon

                        if (StringProcessors.MatchAND(title, list[i].Title) &&
                            StringProcessors.StringsMatchNormalized(list[i].Year, year))
                            return list[i];

                        if (StringProcessors.MatchEndingTHE(title, list[i].Title) &&
                            StringProcessors.StringsMatchNormalized(list[i].Year, year))
                            return list[i];

                        if (StringProcessors.MatchColon(title, list[i].Title) &&
                            StringProcessors.StringsMatchNormalized(list[i].Year, year))
                            return list[i];

                        #endregion

                        #endregion
                    
                    }
                    else
                    {

                        if (list.Count == 1)
                            return list[i];

                        #region match without year

                        if (StringProcessors.StringsMatchNormalized(list[i].Title, title))
                            return list[i];

                        if (StringProcessors.MatchAND(title, list[i].Title))
                            return list[i];

                        if (StringProcessors.MatchEndingTHE(title, list[i].Title))
                            return list[i];

                        if (StringProcessors.MatchColon(title, list[i].Title))
                            return list[i];

                        if (DetectExactMatchUsingVideoDuration
                            (fileDuration, list[i].IMDb_ID))
                            return list[i];

                        #endregion
                    
                    }


                }

            }
            catch (Exception ex)
            {

                Debugger.LogMessageToFile
                    ("An unexpected error occured trying" +
                     " to find an exact film match from IMDb. " +
                     "The error was: " + ex);
                
                StatusForm.statusForm.TrayIcon.ShowBalloonTip
                    (5000, "Error searching for an exact film match",
                     "MediaFairy encountered an error trying" +
                     " to find an exact film match from IMDb. " +
                     "Please see Debug.log for details.",
                     ToolTipIcon.Error);
                
                MainImportingEngine.GeneralStatus 
                    = "Updating Movies section...";
                
                MainImportingEngine.SpecialStatus 
                    = "An error occured trying " +
                      "to find an exact film match from IMDb.";
                
                Helpers.UpdateProgress
                    (MainImportingEngine.GeneralStatus, 
                    MainImportingEngine.SpecialStatus, null);
                
                Thread.Sleep(5000);
            
            }

            return null;
        }


        private static bool DetectExactMatchUsingVideoDuration
            (string fileDuration, string imdbId)
        {



            if (!Settings.EnableDurationMatching)
                return false;

            if (String.IsNullOrEmpty(fileDuration))
                return false;

            #region match Duration

            //string imdbid = list[i].IMDb_ID;
            int filedurationNum = Convert.ToInt16(fileDuration);

            //MessageBox.Show("fileduration: " + fileDuration);
            Helpers.UpdateProgress("Updating Films Section", "Attempting duration matching...", null);
            //Importer.thisProgress.Progress(Importer.CurrentProgress, "Attempting duration matching...");


            IIMDbMovie tmpMovie 
                = IMDbMovieDetailsDownloader
                .DownloadMovieDetailsFromIMDb
                (imdbId, false);
          

            if (tmpMovie == null)
                return false;

            var runtime = tmpMovie.Runtime;
            var runtimeNum = Convert.ToInt16(runtime);
            //MessageBox.Show("Runtime: " + Runtime);

            return (filedurationNum <= runtimeNum + 5) && (filedurationNum >= runtimeNum - 5);

            #endregion
        }

//endof function
    }

//endof Class
}

//endof namespace