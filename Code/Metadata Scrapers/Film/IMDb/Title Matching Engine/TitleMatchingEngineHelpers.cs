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
using System.Text;
using System.Threading;
using System.Windows.Forms;
using MediaFairy;
using MediaFairy.ImportingEngine;


namespace MediaFairy.IMDb.Title_Matching_Engine
{
    internal static class TitleMatchingEngineHelpers
    {
        internal static bool DuplicatesExist(string title, string year, IEnumerable<IIMDbSearchResult> list)
        {
            try
            {
                var sbTitles = new StringBuilder();
                sbTitles.Append("---");
                foreach (IIMDbSearchResult result in list)
                {
                    #region Continue to next result if result's title doesn't match with the current title

                    if ((((!JCUtils.Utils.StringsMatchNormalized(result.Title, title)) &&
                          (!JCUtils.Utils.MatchAND(title, result.Title))) &&
                         (!JCUtils.Utils.MatchEndingTHE(title, result.Title))) &&
                        (!JCUtils.Utils.MatchColon(title, result.Title))) continue;

                    #endregion

                    if (year.Trim() != String.Empty
                        && JCUtils.Utils.StringsMatchNormalized(result.Year, year))
                    {
                        if (
                            sbTitles.ToString().ToLower().IndexOf("--" + result.Title.ToLower().Trim() + "-" +
                                                                  result.Year.Trim() + "--") > -1)
                            return true;

                        sbTitles.Append(result.Title.ToLower().Trim());
                        sbTitles.Append("-");
                        sbTitles.Append(result.Year.Trim());
                        sbTitles.Append("---");
                    }
                    else if (year.Trim() == String.Empty)
                    {
                        if (sbTitles.ToString().ToLower().IndexOf("--" + result.Title.ToLower().Trim() + "-noyear---") >
                            -1)
                            return true;

                        sbTitles.Append(result.Title.ToLower().Trim());
                        sbTitles.Append("-noyear---");
                    }
                } //endof foreach
            } //endof try
            catch (Exception ex)
            {
                Debugger.LogMessageToFile(
                    "An unexpected error occured trying to decide if a film duplicate exists on IMDb. The error was: " +
                    ex);
                StatusForm.statusForm.TrayIcon.ShowBalloonTip(5000, "Error trying to detect a film duplicate",
                                                              "MediaFairy encountered an error trying to detect if duplicates exists for a film on IMDb. Please see Debug.log for details.",
                                                              ToolTipIcon.Error);
                MainImportingEngine.GeneralStatus = "Updating Movies section...";
                MainImportingEngine.SpecialStatus = "An error occured trying to detect duplicates for a film.";
                Helpers.UpdateProgress(MainImportingEngine.GeneralStatus, MainImportingEngine.SpecialStatus, null);
                Thread.Sleep(5000);
            }

            return false;
        }


        internal static IIMDbSearchResult GetFirstMovie(IList<IIMDbSearchResult> results)
        {
            if (results != null
                && results.Count > 0)
                return results[0];

            return null;
        }


        internal static IIMDbSearchResult GetLatestMovie(IEnumerable<IIMDbSearchResult> results, string title)
        {
            IIMDbSearchResult result = null;
            var year = -1;
            year.ToString();

            if (results == null)
                return result;

            foreach (var t in results)
            {
                if (((!JCUtils.Utils.StringsMatchNormalized(title, t.Title) &&
                      !JCUtils.Utils.MatchAND(title, t.Title)) && !JCUtils.Utils.MatchEndingTHE(title, t.Title)) &&
                    !JCUtils.Utils.MatchColon(title, t.Title)) continue;

                if (t.Year.Length < 4) continue;

                var newYear = t.Year.Substring(0, 4);
                int iNewYear;

                if (!int.TryParse(newYear, out iNewYear)) continue;

                if (iNewYear <= year) continue;

                year = iNewYear;
                result = t;
            }

            return result;
        }


        private static void RemoveVideoGameTitles(ref IIMDbMovieCollection list)
        {
            try
            {
                if (list == null)
                    return;
                
                for (var i = list.Count - 1; i >= 0; i--)
                {
                    if (list[i].Year.ToLower().Trim().IndexOf("vg") > -1)
                        list.RemoveAt(i);
                }

            }
            catch (Exception ex)
            {
                Helpers.UpdateProgress("Updating Movies Section...",
                                       "An error occured. Please refer to Debug.log for details.", null);
                Debugger.LogMessageToFile(
                    "[IMDb Film Title Matching Engine] An unexpected error occured while trying to find and remove IMDb video game titles. The error was: " +
                    ex);
            }
        }

//endof function
    }
}