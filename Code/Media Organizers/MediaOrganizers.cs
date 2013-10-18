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
using System.IO;
using System.Threading;
using System.Windows.Forms;


#if USE_MEEDIO
using Meedio;
#elif USE_MEEDIOS
using MeediFier.Code;
using MeediFier.ImportingEngine;
using MeediOS;
#endif


namespace MeediFier
{
    class MediaOrganizers
    {



        public static void OrganizeMediaFiles(IMLItem Item, string ItemTitle,
            string[] multipart, ref string location, string SortingDestination, 
            string directoryTagMask, itemUpdateParams updateParams,
            string MediaType)
        {


            if (!updateParams.FileServerIsOnline)
                return;

            if (Settings.OnlyTaggedForMoving &&
                (!Settings.OnlyTaggedForMoving || Helpers.GetTagValueFromItem(Item, "ReadyToSort") != "true"))
                return;


            if (Helpers.GetTagValueFromItem(Item, "FileSorted") == "true") 
                return;

            if (String.IsNullOrEmpty(ItemTitle))
                return;

            #region local varibales
            bool assignedLocationtoFirstItem = false;
            string fileTitle = ItemTitle;
            fileTitle = ToolBox.Utils.StringProcessors.StringBuilderProcessing.NormalizePath(fileTitle);
            string filmTitle = ItemTitle;
            filmTitle = ToolBox.Utils.StringProcessors.StringBuilderProcessing.NormalizePath(filmTitle);
            string multipaTlocation = "";
            bool CancelMoving = false;
            #endregion


            if (!SortingDestination.EndsWith(@"\"))
                SortingDestination = SortingDestination + @"\";


            #region loop files in item's location
            int i = 1;


            foreach (string partLocation in multipart)
            {

                if (File.Exists(partLocation))
                {
                    #region partfile variables

                    FileInfo partfileInfo = new FileInfo(partLocation);

                    string videoextension = partfileInfo.Extension;
                    long filesize = partfileInfo.Length;
                    long secstocopy = filesize / 10000000;
                    string folderToMove = "";

                    #endregion

                    #region synthesise full directory path

                    #region Synthesize folder
                    if (Settings.DirectoryStructure == "Simple- Default directory structures")
                    {
                        #region default structure

                        if (MediaType == "Film")
                        {
                            folderToMove = SortingDestination + filmTitle + @"\";
                        }
                        if (MediaType == "TvShow")
                        {
                            if (String.IsNullOrEmpty(Helpers.GetTagValueFromItem(Item, "SeriesName")) || Item.Tags["SeriesName"] == null)
                                CancelMoving = true;
                            if (String.IsNullOrEmpty(Helpers.GetTagValueFromItem(Item, "SeasonNumber")) || Item.Tags["SeasonNumber"] == null)
                                CancelMoving = true;

                            string seriesNameNorm = Helpers.GetTagValueFromItem(Item, "SeriesName");
                            seriesNameNorm = ToolBox.Utils.StringProcessors.StringBuilderProcessing.NormalizePath(seriesNameNorm);

                            folderToMove = SortingDestination + seriesNameNorm + @"\" + "Season " + Helpers.GetTagValueFromItem(Item, "SeasonNumber") + @"\";

                        }
                        if (MediaType == "Music")
                        {
                            if (String.IsNullOrEmpty(Helpers.GetTagValueFromItem(Item, "Album")))
                                CancelMoving = true;

                            folderToMove = SortingDestination + @"\" + Helpers.GetTagValueFromItem(Item, "Artist") + @"\" + Helpers.GetTagValueFromItem(Item, "Album") + @"\";
                        }


                        if (!Directory.Exists(folderToMove))
                            Directory.CreateDirectory(folderToMove);
                        #endregion
                    }
                    else
                    {
                        #region advanced structure
                        string[] FolderStructure = directoryTagMask.Split('\\');
                        string tagname = "";
                        string tagvalue = "";

                        folderToMove = SortingDestination;

                        #region Loop folder tag mask and construct new path
                        foreach (string folder in FolderStructure)
                        {
                            tagname = folder.Trim('<', '>');
                            tagvalue = Helpers.GetTagValueFromItem(Item, tagname);


                            if (String.IsNullOrEmpty(tagvalue))
                            {
                                CancelMoving = true;
                                MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress, "The item's tag '" + tagname + "' contains an empty value. Sorting operation was cancelled.");
                                Thread.Sleep(2500);
                            }
                            else
                            {
                                tagvalue = ToolBox.Utils.StringProcessors.StringBuilderProcessing.NormalizePath(tagvalue);
                                folderToMove += tagvalue + @"\";

                                //FolderToMove = Helpers.NormalizePath(FolderToMove);

                                //MessageBox.Show(FolderToMove);

                                if (!Directory.Exists(folderToMove))
                                    Directory.CreateDirectory(folderToMove);
                            }
                        }
                        #endregion


                        #endregion
                    }


                    #endregion

                    if (MediaType == "Film")
                    {
                        #region standrand filename synthesizer

                        if (String.IsNullOrEmpty(Helpers.GetTagValueFromItem(Item, "ImdbID")))
                            CancelMoving = true;

                        if (String.IsNullOrEmpty(Helpers.GetTagValueFromItem(Item, "Title")))
                            CancelMoving = true;

                        if (String.IsNullOrEmpty(Helpers.GetTagValueFromItem(Item, "Year") ) )
                            CancelMoving = true;

                        string canonicalTitle = Helpers.GetTagValueFromItem(Item, "Title");
                        canonicalTitle = ToolBox.Utils.StringProcessors.StringBuilderProcessing.NormalizePath(canonicalTitle);

                        fileTitle = Helpers.GetTagValueFromItem(Item, "ImdbID") + " - " + canonicalTitle + "(" + Helpers.GetTagValueFromItem(Item, "Year") + ")";

                        #endregion
                    }
                    if (MediaType == "TvShow")
                    {
                        #region standrand filename synthesizer

                        if (String.IsNullOrEmpty(Helpers.GetTagValueFromItem(Item, "EpisodeNumber") ) )
                            CancelMoving = true;

                        if (String.IsNullOrEmpty(Helpers.GetTagValueFromItem(Item, "EpisodeName") ) )
                            CancelMoving = true;


                        string seriesNameNorm = Helpers.GetTagValueFromItem(Item, "SeriesName");
                        seriesNameNorm = ToolBox.Utils.StringProcessors.StringBuilderProcessing.NormalizePath(seriesNameNorm);

                        string EpisodeNameNorm = Helpers.GetTagValueFromItem(Item, "EpisodeName");
                        EpisodeNameNorm = ToolBox.Utils.StringProcessors.StringBuilderProcessing.NormalizePath(EpisodeNameNorm);

                        string SeasonNumberNorm = Helpers.GetTagValueFromItem(Item, "SeasonNumber");
                        if (SeasonNumberNorm.Length == 1)
                            SeasonNumberNorm = "0" + SeasonNumberNorm;

                        string EpisodeNumberNorm = Helpers.GetTagValueFromItem(Item, "EpisodeNumber");
                        if (EpisodeNumberNorm.Length == 1)
                            EpisodeNumberNorm = "0" + EpisodeNumberNorm;

                        fileTitle = seriesNameNorm + " S" + SeasonNumberNorm + "E" + EpisodeNumberNorm + " - " + EpisodeNameNorm;
                        #endregion
                    }
                    if (MediaType == "Music")
                    {

                    }


                    if (!folderToMove.EndsWith(@"\"))
                        folderToMove = folderToMove + @"\";

                    if (multipart.Length > 1)
                        fileTitle = fileTitle + "[" + i + "]";


                    string locationToMove = folderToMove + fileTitle + videoextension;


                    
                    #endregion




                    #region decide if a file is going to be moved
                    DialogResult result;
                    bool DecideMoving = false;

                    if (locationToMove == partLocation)
                    {
                        DecideMoving = false;
                    }
                    else if (Settings.PromptBeforeMoving)
                    {
                        result = MessageBox.Show(null, "I'm going to move video file to: " + locationToMove + "     Is this ok?", "", MessageBoxButtons.YesNo);
                        if (result == DialogResult.Yes)
                            DecideMoving = true;
                    }
                    else DecideMoving = true;
                    #endregion

                    if (DecideMoving)
                    {


                        if (!CancelMoving)
                        {

                            if (locationToMove != partLocation)
                            {

                                #region move the file and update library

                                if (!updateParams.FileInUse)
                                {
                                                


                                    MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress, "Moving " + ItemTitle + "... " + secstocopy + " seconds remaining.");
                                    Thread.Sleep(500);


                                    try
                                    {

                                        File.Move(partLocation, locationToMove);

                                        #region Move the associated SRT subtitle
                                        string subtitleOldLocation = partLocation.Remove(partLocation.Length - 3, 3);
                                        subtitleOldLocation = subtitleOldLocation + "srt";
                                        string subtitleNewLocation = locationToMove.Remove(locationToMove.Length - 3, 3);
                                        subtitleNewLocation = subtitleNewLocation + "srt";

                                        if (File.Exists(subtitleOldLocation))
                                            File.Move(subtitleOldLocation, subtitleNewLocation);
                                        #endregion

                                        #region Move the associated SUB subtitle
                                        subtitleOldLocation = partLocation.Remove(partLocation.Length - 3, 3);
                                        subtitleOldLocation = subtitleOldLocation + "sub";
                                        subtitleNewLocation = locationToMove.Remove(locationToMove.Length - 3, 3);
                                        subtitleNewLocation = subtitleNewLocation + "sub";

                                        if (File.Exists(subtitleOldLocation))
                                            File.Move(subtitleOldLocation, subtitleNewLocation);
                                        #endregion


                                        #region update item tags
                                        if (updateParams.IsMultipart)
                                        {
                                            multipaTlocation += "|" + locationToMove;

                                            if (partLocation == multipart[multipart.Length - 1])
                                                multipaTlocation += "|";

                                            Item.Location = multipaTlocation;
                                        }
                                        else Item.Location = locationToMove;

                                        Item.Tags["FileSorted"] = "true";
                                        Item.SaveTags();
                                        #endregion

                                        #region Prevent the updating of item's location for the remaining parts
                                        if (!assignedLocationtoFirstItem)
                                        {
                                            location = locationToMove;
                                            assignedLocationtoFirstItem = true;
                                        }
                                        #endregion

                                    }
                                    catch (Exception e)
                                    {
                                        Debugger.LogMessageToFile("An error occurred while trying to move the file " + location + 
                                                                  " to destination " + locationToMove + ". The error was: " + e);

                                        
                                        StatusForm.statusForm.TrayIcon.ShowBalloonTip(10000, "file moving error", 
                                                                                      "An unexpected error occured while MediaFairy was trying to move the media file " + location + 
                                                                                      " to " + locationToMove + ". The error was: " + e.Message, ToolTipIcon.Error);

                                        continue;
                                    }



                                }

                                #endregion

                            }

                        }
                    }

                }

                i++;
            } //endof PartLocation loop
            #endregion


        }


    }


}
