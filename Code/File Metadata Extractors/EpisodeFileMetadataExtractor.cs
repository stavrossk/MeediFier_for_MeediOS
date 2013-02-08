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
using System.IO;
using System.Windows.Forms;
using MediaFairy.Code.RegEx_Matchers;
using MeediOS;

namespace MediaFairy
{
    internal static class EpisodeFileMetadataExtractor
    {


        public static bool ExtractMetadataFromEpisodeFilename(IMLItem item, string location,
            IMLSection Section, IMLSection TvSection, bool RanFromTvSection, 
            IEnumerable<string> combinedSceneTags)
        {
            if (!Settings.ExtractMetadataFromFilenames)
                return false;

            if (String.IsNullOrEmpty(item.Name))
                return false;

            try
            {
                //Importer.thisProgress.Progress(Importer.CurrentProgress, "Performing meta-information extraction...");
                Helpers.UpdateProgress("Performing diagnostic operations", "Performing meta-information extraction...", item);
                Debugger.LogMessageToFile("Performing meta-information extraction...");


                #region Clean Iten Name from common tags
                Debugger.LogMessageToFile("Cleaning Item Name...");
                string itemName = item.Name;
                itemName = VideoFilenameCleaner.CleanVideoFilename(itemName, combinedSceneTags ).Trim();
               

                item.Name = itemName;
                item.SaveTags();
                #endregion

                RecognizeSeries(TvSection, Section, RanFromTvSection, item, location);




            }
            catch (Exception e)
            {
                StatusForm.statusForm.TrayIcon.ShowBalloonTip(5000, "Error parsing filename", "An unexpected error ocurred in the filename parser. " + Environment.NewLine +
                                                                                              "Please see the Debug.log file for details.", ToolTipIcon.Warning);
                Debugger.LogMessageToFile("An unexpected error occured in the filename parsing method. The error was: " + e.ToString());
            }

            return false;
        }


        internal static void ExtractKeyEpisodeDataFromFilename(IMLItem item)
        {


            string itemName = Helpers.GetTagValueFromItem(item, "Name");

            const string regEx =
                @"(.*\\)*
                (?<SeriesName>.*)
                (
                  (s|S)
                  (?<SeasonNumber>[0-9]{1,2})
                  (e|E)
                  (?<EpisodeNumber>[0-9]{1,2})
                )
                (?<EpisodeName>.*)\.
                (?<FileType>...?)";

            string[] results = RegExMatchers.MatchExpressionReturnFirstMatchAllGroups(itemName, regEx );

            if (!String.IsNullOrEmpty(results[2]))
                item.Tags["SeriesName"] = results[2];

            if (!String.IsNullOrEmpty(results[5]))
                item.Tags["SeasonNumber"] = results[5];

            if (!String.IsNullOrEmpty(results[7]))
                item.Tags["EpisodeNumber"] = results[7];

            if (!String.IsNullOrEmpty(results[8]))
                item.Tags["EpisodeName"] = results[8];

            item.SaveTags();


        }


        internal static void RecognizeSeries(IMLSection tvSection, IMLSection section, bool ranFromTvSection, IMLItem Item, string location)
        {

            try
            {

                #region init variables
                int seasonIndex = 0;
                const int eIndex = 0;
                const int nIndex = 0;
                #endregion


                seasonIndex = LocateSeasonIndexFromFilename(Item, seasonIndex);


                ExtractEpisodeNumbersFromIndexes(section, tvSection, ranFromTvSection,
                    Item, location, seasonIndex, eIndex, nIndex);

            }
            catch (Exception e)
            {
                Debugger.LogMessageToFile("An unexpected error ocurred in the Series tags extractor. " +
                                          "The error was: " + e);
            }




        }

        private static int LocateSeasonIndexFromFilename(IMLItem Item, int S_index)
        {
            S_index = Structure_NNxNN(Item);

            S_index = StructureSnnEnn(Item, S_index);

            return S_index;
        }



        private static int StructureSnnEnn(IMLItem item, int sIndex)
        {
            #region detect S01E01 structure

            if (sIndex < 1)
            {
                int tmpIndex = item.Name.IndexOf("S0");
                if (tmpIndex > 0)
                    sIndex = tmpIndex;

                tmpIndex = item.Name.IndexOf("s0");
                if (tmpIndex > 0)
                    sIndex = tmpIndex;

                tmpIndex = item.Name.IndexOf("S1");
                if (tmpIndex > 0)
                    sIndex = tmpIndex;

                tmpIndex = item.Name.IndexOf("s1");
                if (tmpIndex > 0)
                    sIndex = tmpIndex;

                tmpIndex = item.Name.IndexOf("S2");
                if (tmpIndex > 0)
                    sIndex = tmpIndex;

                tmpIndex = item.Name.IndexOf("s2");
                if (tmpIndex > 0)
                    sIndex = tmpIndex;

                tmpIndex = item.Name.IndexOf("S3");
                if (tmpIndex > 0)
                    sIndex = tmpIndex;

                tmpIndex = item.Name.IndexOf("s3");
                if (tmpIndex > 0)
                    sIndex = tmpIndex;

                tmpIndex = item.Name.IndexOf("S4");
                if (tmpIndex > 0)
                    sIndex = tmpIndex;

                tmpIndex = item.Name.IndexOf("s4");
                if (tmpIndex > 0)
                    sIndex = tmpIndex;

                tmpIndex = item.Name.IndexOf("S5");
                if (tmpIndex > 0)
                    sIndex = tmpIndex;

                tmpIndex = item.Name.IndexOf("s5");
                if (tmpIndex > 0)
                    sIndex = tmpIndex;

                tmpIndex = item.Name.IndexOf("S6");
                if (tmpIndex > 0)
                    sIndex = tmpIndex;

                tmpIndex = item.Name.IndexOf("s6");
                if (tmpIndex > 0)
                    sIndex = tmpIndex;

                tmpIndex = item.Name.IndexOf("S7");
                if (tmpIndex > 0)
                    sIndex = tmpIndex;

                tmpIndex = item.Name.IndexOf("s7");
                if (tmpIndex > 0)
                    sIndex = tmpIndex;

                tmpIndex = item.Name.IndexOf("S8");
                if (tmpIndex > 0)
                    sIndex = tmpIndex;

                tmpIndex = item.Name.IndexOf("s8");
                if (tmpIndex > 0)
                    sIndex = tmpIndex;

                tmpIndex = item.Name.IndexOf("S9");
                if (tmpIndex > 0)
                    sIndex = tmpIndex;

                tmpIndex = item.Name.IndexOf("s9");
                if (tmpIndex > 0)
                    sIndex = tmpIndex;

                if (sIndex > 1)
                    Debugger.LogMessageToFile("S[SeasonNumber]E[EpisodeNumber] episode naming scheme detected.");
            }

            #endregion

            return sIndex;
        }

        // ReSharper disable InconsistentNaming
        private static int Structure_NNxNN(IMLItem Item)
        // ReSharper restore InconsistentNaming
        {
            int sIndex = 0;

            #region detect 01x01 structure

            int tmpIndex = Item.Name.IndexOf("x0");

            if (tmpIndex < 1)
                tmpIndex = Item.Name.IndexOf("x1");

            if (tmpIndex < 1)
                tmpIndex = Item.Name.IndexOf("x2");

            if (tmpIndex < 1)
                tmpIndex = Item.Name.IndexOf("x3");

            if (tmpIndex < 1)
                tmpIndex = Item.Name.IndexOf("x4");


            if (tmpIndex > 0)
            {
                if (Char.IsDigit(Item.Name[tmpIndex - 1]))
                {
                    Debugger.LogMessageToFile("[SeasonNumber]x[EpisodeNumber] episode naming scheme detected.");
                    if (Char.IsDigit(Item.Name[tmpIndex - 2]))
                    {
                        sIndex = tmpIndex - 3;
                    }
                    else sIndex = tmpIndex - 2;
                }
            }

            #endregion

            return sIndex;
        }





        internal static void ExtractEpisodeNumbersFromIndexes(IMLSection Section, IMLSection TvSection, bool RanFromTvSection, IMLItem Item, string location, int S_index, int E_index, int N_index)
        {
            string seasonNumber = Helpers.GetTagValueFromItem(Item, "SeasonNumber");
            string episodeNumber = Helpers.GetTagValueFromItem(Item, "EpisodeNumber");
            string seriesName = Helpers.GetTagValueFromItem(Item, "SeriesName");

            if (!String.IsNullOrEmpty(seasonNumber) && !String.IsNullOrEmpty(episodeNumber) && !String.IsNullOrEmpty(seriesName))
                return;

            #region if season index is found, extract season & episode numbers from name and save to item's tags
            if (S_index > 0)
            {

                ExtractSeasonAndEpisodeNumbersFromFilename(Section, TvSection, RanFromTvSection, Item, S_index, N_index);
            }
                #endregion
            else
                #region but if season index is not found, try to extract the season number from the parent folder
            {

                ExtractSeasonAndEpisodeNumbersFromParentDirectory(Section, TvSection, RanFromTvSection, Item, location, S_index, seasonNumber, episodeNumber);
            }
            #endregion



        }



        private static void ExtractSeasonAndEpisodeNumbersFromParentDirectory(IMLSection Section, IMLSection TvSection,
                                                                              bool RanFromTvSection, IMLItem Item,
                                                                              string location, int S_index, string seasonNumber,
                                                                              string episodeNumber)
        {
            Debugger.LogMessageToFile("Season index was not found. Trying to retrieve SeasonNumber from parent directory...");

            string parentDirectoryName;

            if (RetrieveParentDirectory(location, out parentDirectoryName)) 
                return;


            if (!parentDirectoryName.Contains("Season") && !parentDirectoryName.Contains("season")) 
                return;

            S_index = LocateSeasonIndexFromParentDirectoryName(parentDirectoryName);


            string seriesName;

            if (ExtractSeasonNumberFromParentDirectoryName(S_index, out seasonNumber, parentDirectoryName, out seriesName)) 
                return;


            SaveSeasonNumberToItemTag(Item, seasonNumber);


            episodeNumber = ExtractEpisodeNumberFromParentDirectoryName(Item, episodeNumber);


            if (!String.IsNullOrEmpty(seriesName.Trim()))
            {
                seriesName = seriesName.TrimStart('.');
                seriesName = seriesName.TrimEnd('.');
                seriesName = seriesName.TrimStart('-');
                seriesName = seriesName.TrimEnd('-');

                if (String.IsNullOrEmpty(Helpers.GetTagValueFromItem(Item, "SeriesName")))
                {
                    Debugger.LogMessageToFile("SeriesName: " + seriesName);
                    Item.Tags["SeriesName"] = seriesName;
                }

                Item.Name = seriesName;
                Item.SaveTags();
            }

            MoveToTvSection(Section, TvSection, Item, RanFromTvSection, seasonNumber, episodeNumber, seriesName);

           
        }

        private static string ExtractEpisodeNumberFromParentDirectoryName(IMLItem Item, string episodeNumber)
        {
            if (Item.Name.Trim().Contains("-"))
            {
                int episodeSepIndex = Item.Name.LastIndexOf('-');

                if (Char.IsNumber(Item.Name[episodeSepIndex - 1]))
                {
                    episodeNumber = Char.IsNumber(Item.Name[episodeSepIndex - 2])
                        ? Item.Name.Substring(episodeSepIndex - 2, 2) 
                        : Item.Name.Substring(episodeSepIndex - 1, 1);
                }
                else if (Char.IsNumber(Item.Name[episodeSepIndex - 2]))
                {
                    episodeNumber = Char.IsNumber(Item.Name[episodeSepIndex - 3])
                        ? Item.Name.Substring(episodeSepIndex - 3, 2)
                        : Item.Name.Substring(episodeSepIndex - 2, 1);
                }
                else if (Char.IsNumber(Item.Name[episodeSepIndex - 3]))
                {
                    episodeNumber = Char.IsNumber(Item.Name[episodeSepIndex - 4]) 
                        ? Item.Name.Substring(episodeSepIndex - 4, 2) 
                        : Item.Name.Substring(episodeSepIndex - 3, 1);
                }

                #region ...and save episode number to tag

                if (!String.IsNullOrEmpty(episodeNumber.Trim()))
                {
                    Debugger.LogMessageToFile("EpisodeNumber: " + episodeNumber);
                    Item.Tags["EpisodeNumber"] = episodeNumber;
                    Item.SaveTags();
                }

                #endregion
            }
            return episodeNumber;
        }

        private static void SaveSeasonNumberToItemTag(IMLItem item, string seasonNumber)
        {
            #region ...and save SeasonNumber to Item's tag.

            if (seasonNumber == null || String.IsNullOrEmpty(seasonNumber.Trim()))
                return;

            Debugger.LogMessageToFile("SeasonNumber: " + seasonNumber);
            item.Tags["SeasonNumber"] = seasonNumber;
            item.SaveTags();

            #endregion
        }

        private static bool ExtractSeasonNumberFromParentDirectoryName(int seasonIndex, out string seasonNumber,
                                                                      string parentDirectoryName, out string seriesName)
        {
            if (seasonIndex <= 0)
            {
                seasonNumber = null;
                seriesName = null;
                return true;
            }

            if (!Char.IsDigit(parentDirectoryName[seasonIndex + 7]))
            {
                seasonNumber = null;
                seriesName = null;
                return true;
            }

            Debugger.LogMessageToFile("Season index was succesfully retrived from parent.");

            #region Extract Series Name from parent directory name
            seriesName = parentDirectoryName.Substring(0, seasonIndex - 1);
            Debugger.LogMessageToFile("Series Name: " + seriesName);
            #endregion


            #region remove any exesssive characters afer Season Number
            string rightPart = parentDirectoryName.Substring(seasonIndex, parentDirectoryName.Length - seasonIndex);
            string seasonNumberString = rightPart.Remove(0, 6);
            #endregion


            #region detect Season Number

            for (int i = 0; i < seasonNumberString.Length; i++)
            {
                if (!Char.IsNumber(seasonNumberString[i])) continue;

                if (SeasonNumberDoubleDigitTestA(out seasonNumber, i, seasonNumberString))
                    continue;

                seasonNumber = seasonNumberString.Substring(i, 1);
                continue;
            }

            #endregion

            seasonNumber = null;
            return false;

        }

        private static bool SeasonNumberDoubleDigitTestA(out string seasonNumber, int i, string seasonNumberString)
        {
            if (seasonNumberString.Length >= i + 2)
            {
                if (Char.IsNumber(seasonNumberString[i + 1]))
                {
                    seasonNumber = seasonNumberString.Substring(i, 2);
                    return true;
                }

                seasonNumber = seasonNumberString.Substring(i, 1);
                return true;
            }

            seasonNumber = null;
            return false;
        }

        private static int LocateSeasonIndexFromParentDirectoryName(string parentDirectoryName)
        {
            int sIndex = 0;

            #region detect Season Index

            int tmpIndex = parentDirectoryName.IndexOf("Season");

            if (tmpIndex > 0)
                sIndex = tmpIndex;

            tmpIndex = parentDirectoryName.IndexOf("season");
            if (tmpIndex > 0)
                sIndex = tmpIndex;

            #endregion

            return sIndex;
        }

        private static bool RetrieveParentDirectory(string location, out string parentDirectoryName)
        {
            FileInfo fi = new FileInfo(location);
            DirectoryInfo di = fi.Directory;

            if (di == null || String.IsNullOrEmpty(di.Name))
            {
                parentDirectoryName = null;
                return true;
            }

            parentDirectoryName = di.Name;
            Debugger.LogMessageToFile("Parent directory name: " + parentDirectoryName);
            return false;
        }

        private static void ExtractSeasonAndEpisodeNumbersFromFilename(IMLSection section, IMLSection TvSection,
                                                                       bool RanFromTvSection, IMLItem Item, int S_index,
                                                                       int N_index)
        {
            int eIndex;
            string seasonNumber = ExtractSeasonNumberFromFilename(Item, S_index, out eIndex);
            Debugger.LogMessageToFile("Extracted SeasonNumber: " + seasonNumber);
            string episodeNumber = ExtractEpisodeNumberFromFilename(Item, N_index, eIndex);
            Debugger.LogMessageToFile("Extracted EpisodeNumber: " + episodeNumber);

            if (Item.Name[eIndex] != 'E'
                && 
                Item.Name[eIndex] != 'e'
                && 
                Item.Name[eIndex] != 'x')
                return;

            string seriesName = ExtractSeriesName(Item, S_index, N_index);
            Debugger.LogMessageToFile("Extracted SeriesName: " + seriesName);
            MoveToTvSection(section, TvSection, Item, RanFromTvSection, seasonNumber, episodeNumber, seriesName);
        }

        internal static string ExtractSeasonNumberFromFilename(IMLItem item, int seasonIndex, out int episodeIndex)
        {
            string seasonNumber;
 
            if (Char.IsNumber(item.Name[seasonIndex + 2]))
            {
                episodeIndex = seasonIndex + 3;
                seasonNumber = item.Name.Substring(seasonIndex + 1, 2);
            }
            else
            {
                episodeIndex = seasonIndex + 2;
                seasonNumber = item.Name.Substring(seasonIndex + 1, 1);
            }

            item.Tags["SeasonNumber"] = seasonNumber;
            item.SaveTags();
            return seasonNumber;

        }

        internal static string ExtractEpisodeNumberFromFilename(IMLItem item, int N_index, int episodeIndex)
        {
            string episodeNumber;


            if (Char.IsNumber(item.Name[episodeIndex + 2]))
                episodeNumber = item.Name.Substring(episodeIndex + 1, 2);
            else
                episodeNumber = item.Name.Substring(episodeIndex + 1, 1);


            item.Tags["EpisodeNumber"] = episodeNumber;
            item.SaveTags();
            return episodeNumber;
       

        }

        internal static string ExtractSeriesName(IMLItem item, int seasonIndex, int nIndex)
        {
            string leftpart = item.Name.Substring(0, seasonIndex);
            leftpart = leftpart.TrimStart('.');
            leftpart = leftpart.TrimEnd('.');

            string seriesName = leftpart;
            item.Tags["SeriesName"] = leftpart;
            item.SaveTags();

            return seriesName;

        }


        internal static void MoveToTvSection(IMLSection section, IMLSection tvSection,
            IMLItem item, bool ranFromTvSection, string seasonNumber,
            string episodeNumber, string seriesName)
        {

            if (tvSection == null) return;

            bool moveSeriesToTvSection = !ranFromTvSection;

            if (!moveSeriesToTvSection) return;

            Debugger.LogMessageToFile("Moving episode " + item.Name + " to TvEpisodes section...");

            tvSection.BeginUpdate();

            IMLItem tvItem = tvSection.AddNewItem(item.Name, item.Location);
            tvItem.Tags["SeasonNumber"] = seasonNumber;
            tvItem.Tags["EpisodeNumber"] = episodeNumber;
            tvItem.Tags["SeriesName"] = seriesName;
            tvItem.SaveTags();

            tvSection.EndUpdate();

            section.DeleteItem(item);

        }

    
    }


}