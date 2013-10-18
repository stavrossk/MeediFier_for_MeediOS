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
using MeediFier.IMDb;
using MeediOS;

namespace MeediFier
{

    public static class MovieFileMetadataExtractor
    {

        //TODO: Test if the overloads for this function are working correctly.
        //Used fy the Film Updater
        public static bool ExtractMetadataFromMovieFilename( IMLItem Item, ref string imdbid,
            ref IMDbOperations imdbOP, IEnumerable<string> combinedSceneTags)
        {

            //TODO: Make seperate options for filename filtering and metadata extraction.
            if (!Settings.ExtractMetadataFromFilenames)
                return false;

            if (String.IsNullOrEmpty(Item.Name))
                return false;

            try
            {

                //Importer.thisProgress.Progress(Importer.CurrentProgress, "Performing meta-information extraction...");
                Helpers.UpdateProgress("Performing diagnostic operations", "Performing meta-information extraction...", Item);
                Debugger.LogMessageToFile("Performing meta-information extraction...");


                imdbid = ExtractImdbIdFromFilename
                    (Item, imdbid, imdbOP, combinedSceneTags);


                #region filter name from release Tags

                Debugger.LogMessageToFile
                    ("[Video filename cleaner] " +
                     "Cleaning filename from common tags...");


                string itemName = Item.Name;

                itemName = VideoFilenameCleaner.CleanVideoFilename
                    (itemName, combinedSceneTags).Trim();

                itemName = VideoFilenameCleaner.CleanVideoFilename
                    (itemName, combinedSceneTags).Trim();

                itemName = VideoFilenameCleaner.CleanVideoFilename
                    (itemName, combinedSceneTags).Trim();

                Item.Name = itemName;
                Item.SaveTags();

                #endregion


                ExtractYearFromFilename(Item);


                //if ( !String.IsNullOrEmpty(location) )
                //Helpers.RecognizeSeries(TvEpisodesSection, Section, RanFromTvSection, Item, location);
                //TODO: This function should not be used!



            }
            catch (Exception e)
            {
                StatusForm.statusForm.TrayIcon.ShowBalloonTip(5000, "Error parsing filename", "An unexpected error ocurred in the filename parser. " + Environment.NewLine + 
                                                                                              "Please see the Debug.log file for details.", ToolTipIcon.Warning);
                Debugger.LogMessageToFile("An unexpected error occured in the filename parsing method. The error was: " + e.ToString() );
            }

            return false;
        }


        //TODO: This function should be changed to use regural expressions. 
        private static string ExtractImdbIdFromFilename(IMLItem item, string imdbid,
            IMDbOperations imdbOP, IEnumerable<string> combinedSceneTags)
        {
            if (item.Name.Contains("tt"))
            {
                //MessageBox.Show("Step 3");

                #region if imdb index is found...

                int imdbid_index = item.Name.IndexOf("tt");

                if ((item.Name.Length - imdbid_index) > 2)
                {

                    //MessageBox.Show("Step 4");
                    Char firstDigit = item.Name[imdbid_index + 2];
                    if (imdbid_index >= 0 && Char.IsNumber(firstDigit))
                    {
                        //MessageBox.Show("Step 5");
                        int imdb_index_start = imdbid_index;
                        int imdb_index_end = 0;
                        imdbid_index = imdbid_index + 2;
                        string substring = item.Name.Substring(imdbid_index, item.Name.Length - imdbid_index - 1);


                        for (int i = imdbid_index; i <= item.Name.Length; i++)
                        {

                            if (Char.IsNumber(item.Name[i]))
                            {
                                imdb_index_end = i;
                            }
                            else break;

                        }

                        //MessageBox.Show("Substring: " + substring); 
                        //MessageBox.Show( Convert.ToString("Index start: " + imdb_index_start) );
                        //MessageBox.Show(Convert.ToString("Index end: " + imdb_index_end));
                        int imdbid_length = imdb_index_end - imdbid_index + 1;
                        //MessageBox.Show("IMDbID length: " + imdbid_length);
                        //MessageBox.Show(Convert.ToString("start index: " + imdb_index_start));
                        imdbid = item.Name.Substring(imdbid_index, imdbid_length);
                        imdbid = "tt" + imdbid;
                        //MessageBox.Show("imdbid after extracting from filename: "+ imdbid);

                        //TODO: IMDb ID should be publicly accessible by all functions!
                        if (imdbOP != null)
                            imdbOP.ImdbId = imdbid;

                        item.Tags["ImdbID"] = imdbid;
                        string left_namepart = item.Name.Substring(0, imdb_index_start);
                        //MessageBox.Show("left part: " + left_namepart);
                        string right_namepart = item.Name.Substring(imdb_index_end + 1,
                                                                    item.Name.Length - imdb_index_end - 1);
                        //MessageBox.Show("Right part: " + right_namepart);
                        item.Name = left_namepart + right_namepart;
                        item.SaveTags();

                    }

                }

                #endregion

                Debugger.LogMessageToFile("ImdbID " + imdbid + " was extracted from item's name...");
            }
            return imdbid;
        }


        //TODO: This function should be changed to use regural expressions. 
        private static void ExtractYearFromFilename(IMLItem Item)
        {
            int year_index = 0;
            int temp_index = 0;

            #region First Try: Year in parenthesis
            temp_index = Item.Name.IndexOf("[19");
            if (temp_index >= 0) year_index = temp_index + 1;

            temp_index = Item.Name.IndexOf("(19");
            if (temp_index >= 0) year_index = temp_index + 1;

            temp_index = Item.Name.IndexOf(".19");
            if (temp_index >= 0) year_index = temp_index + 1;

            temp_index = Item.Name.IndexOf("[20");
            if (temp_index >= 0) year_index = temp_index + 1;

            temp_index = Item.Name.IndexOf("(20");
            if (temp_index >= 0) year_index = temp_index + 1;

            temp_index = Item.Name.IndexOf(".20");
            if (temp_index >= 0) year_index = temp_index + 1;
            #endregion

            #region Second Try: Year without parenthesis
            if (year_index == 0)
            {
                int LastChar = Item.Name.Length - 1;
                int FirstYearChar = LastChar - 3;
                string NumberWord = Item.Name.Substring(FirstYearChar, 4);

                if (!NumberWord.Contains("x") && !NumberWord.Contains("E") && !NumberWord.Contains("S") ) //the number extracted must Not belong to episode numbers
                {
                    if (Item.Name.Length >= 8)
                    {
                        if (Char.IsDigit(Item.Name[LastChar]))
                        {
                            if (Char.IsDigit(Item.Name[FirstYearChar]))
                            {
                                #region Second Try: Year without parenthesis
                                temp_index = Item.Name.IndexOf("19");
                                if (temp_index >= 0) year_index = temp_index;

                                temp_index = Item.Name.IndexOf("20");
                                if (temp_index >= 0) year_index = temp_index;
                                #endregion
                            }
                        }
                    }
                }
            }
            #endregion

            #region Extract Year and store it in item's tag
            if (year_index > 0)
            {
                //MessageBox.Show(Convert.ToString("year compariosn result: " + year_index));
                string year = Item.Name.Substring(year_index, 4);
                //MessageBox.Show("Year: " + year);
                Item.Tags["Year"] = year;
                Item.Name = Item.Name.Trim();
                       
                Char YearPrevChar = Item.Name[year_index -1];
                if (YearPrevChar == '(' || YearPrevChar == '[' || YearPrevChar == '.')
                    year_index = year_index - 1;

                Item.Name = Item.Name.Substring(0, year_index);
                Item.Name = Item.Name.Trim();
                Item.SaveTags();

                Debugger.LogMessageToFile("The film's Year (" + year + ") was extracted from item's name.");
            }
            #endregion

        }



        //Overload used by the Film Releases Downloader
        public static void ExtractMetadataFromMovieFilename
            (IMLItem item, ref string imdbId, 
            IEnumerable<string> combinedSceneTags)
        {
            IMDbOperations imdbOp = null;

            ExtractMetadataFromMovieFilename
                (item,ref imdbId,
                ref imdbOp, combinedSceneTags );


        }



    }//endof Class

}//endof Namespace