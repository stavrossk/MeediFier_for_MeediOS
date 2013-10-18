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
using System.Linq;
using System.Text.RegularExpressions;
using MeediFier.Code.Settings;
using MeediOS;

namespace MeediFier
{
    public static class NfoFileParser
    {



        internal static string RetrieveImdbidFromNfoFile(IMLItem Item,ref string ImdbID, bool FileServerIsOnline, bool IsUNC, DirectoryInfo parent, string pluginpath)
        {

            if (!Settings.EnableNfoParsing)
                return ImdbID;

            if (!String.IsNullOrEmpty(ImdbID))
                return ImdbID;

            if (!FileServerIsOnline && IsUNC)
                return ImdbID;


            try
            {

                 Debugger.LogMessageToFile("Entered .nfo parser.");

                 #region Detect if parent is Root (contains more than one films)

                 if (DirectoryContainsMultipleNfoFiles(parent))
                     return ImdbID;

                 if (ParentDirectoryIsAMediaSource(parent))
                     return ImdbID;

                 if (ParentDirectoryContainsMultipleVideoFiles(parent, pluginpath))
                     return ImdbID;

                #endregion

                    
                 #region search directory for .nfo files

                Debugger.LogMessageToFile("Searching film's directory for .nfo files...");
                // ReSharper disable LoopCanBeConvertedToQuery
                foreach (FileInfo file in parent.GetFiles("*.nfo", SearchOption.TopDirectoryOnly))
                // ReSharper restore LoopCanBeConvertedToQuery
                    ImdbID = ReadFileExtractImdbId(ImdbID, file, Item);

                #endregion
                        

            }
            catch (Exception e)
            {
                Debugger.LogMessageToFile("The .nfo file parsing method returned an unexpected error: " + e );
            }

            return ImdbID;
        }


        private static bool ParentDirectoryContainsMultipleVideoFiles(DirectoryInfo parent, string pluginpath)
        {
            bool directoryIsRoot = false;

            Debugger.LogMessageToFile("Counting video files in parent to detect root...");

            if (!File.Exists(pluginpath + @"Media file extensions\" + "video_extensions.txt"))
            {
                Debugger.LogMessageToFile("[Nfo File Parser] video_extensions.txt file was not found. Unable to detect if this item's parent folder contains multiple video files.");
                return false;
            }

            string[] videoExtensions = File.ReadAllLines(pluginpath + @"Media file extensions\" + "video_extensions.txt");
            int videoFilesCount = 0;

            foreach (string extension in videoExtensions)
            {
                FileInfo[] files = parent.GetFiles(extension, SearchOption.TopDirectoryOnly);

                videoFilesCount = files.Count(file => !file.Name.Contains("sample"));
            }

            if (videoFilesCount > 2)
            {
                Debugger.LogMessageToFile("Directory " + parent.FullName + " is Root.");
                directoryIsRoot = true;
            }

            return directoryIsRoot;
        }



        private static bool ParentDirectoryIsAMediaSource(FileSystemInfo parent)
        {

            bool directoryIsRoot = false;

            Debugger.LogMessageToFile("Checking if parent directory is root...");
               #pragma warning disable 168
            foreach (string rootFolder in
               #pragma warning restore 168
            MediaImportersAndUpdatersCoreSettings.RootMediaFolders.Where(rootFolder => rootFolder == parent.FullName))
            {
                directoryIsRoot = true;
                Debugger.LogMessageToFile("Parent directory " + parent.FullName +
                                          " is root. The .nfo file parser will not continue for this item.");
            }

            return directoryIsRoot;
        }


        private static bool DirectoryContainsMultipleNfoFiles(DirectoryInfo parent)
        {

            Debugger.LogMessageToFile("Counting .nfo files...");
            FileInfo[] nfoFiles;

            try
            {
                nfoFiles = parent.GetFiles("*.nfo", SearchOption.TopDirectoryOnly);
            }
            catch (Exception e)
            {
                Debugger.LogMessageToFile("[Nfo File Parser] An unexpcted error occured trying to access the parent directory. The error was: " + e);
                return true;
            }

            return nfoFiles.Length > 1;
        }


        private static string ReadFileExtractImdbId(string imdbId, FileInfo file, IMLItem item)
        {
            try
            {
                Debugger.LogMessageToFile("Opening .nfo file for reading...");
                StreamReader reader = file.OpenText();
                string text = reader.ReadToEnd();
                Regex regex = new Regex("title/(.*?)/");
                Debugger.LogMessageToFile("Searching file for matching info...");
                MatchCollection matches = regex.Matches(text);

                //TODO: The .nfo parser should not continue reading additional files when the IMDb ID is succesfully extracted from the first file found!
               
                #region Retrieve first from returned matches
                foreach (Match match in matches)
                {
                    imdbId = match.Groups[1].Value;
                    //MessageBox.Show("imdbid from nfo: " + imdbid);
                    Debugger.LogMessageToFile("Retrieved ImdbID " + imdbId + " from .nfo file.");
                    item.Tags["ImdbID"] = imdbId;
                    item.SaveTags();
                                            
                }
                #endregion

            }
            catch (Exception e)
            {
                //MessageBox.Show("The .nfo reader returned an exception: " + e.ToString());
                Debugger.LogMessageToFile("Reading of .nfo file " + file.Name + " failed due to an unexpected error. The error was: " + e);
            }

            return imdbId;


        }


    }


}