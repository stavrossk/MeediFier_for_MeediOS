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
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using MediaFairy.ImportingEngine;
using MeediOS;
using TagLib;

namespace MediaFairy
{
    public static class MusicDnsIdentifier
    {

        internal static void IdentifyAudio(bool musicDnsEnabled, IMLItem Item, bool SkipLongTracks, double AudioDuration, ref int TracksToIdentify, ref int RemaininigTime, bool OnlyIdFilesWithMissingInfo, string location, ConnectionResult connectionresult)
        {
            if (!musicDnsEnabled)
                return;

            if (!connectionresult.InternetConnectionAvailable && Settings.ConnectionDiagnosticsEnabled)
                return;

            if (SkipLongTracks && AudioDuration >= 20)
                return;


            #region function variables

            string puid = String.Empty;

            if (!String.IsNullOrEmpty(Helpers.GetTagValueFromItem(Item, "PUID")))
                puid = Helpers.GetTagValueFromItem(Item, "PUID").Trim();

            #endregion


            #region Generate the known information about the song

            string itemTitle = String.Empty;
            string itemArtist = String.Empty;
            
            //TODO: Add option to NOT overwrite item tags with MusicDNS data if they are already populated.
            string ItemAlbum = String.Empty;

            if (!String.IsNullOrEmpty(Helpers.GetTagValueFromItem(Item, "Title")))
                itemTitle = Helpers.GetTagValueFromItem(Item, "Title").Trim();

            if (!String.IsNullOrEmpty(Helpers.GetTagValueFromItem(Item, "Artist")))
                itemArtist = Helpers.GetTagValueFromItem(Item, "Artist".Trim());

            if (!String.IsNullOrEmpty(Helpers.GetTagValueFromItem(Item, "Album")))
                ItemAlbum = Helpers.GetTagValueFromItem(Item, "Album").Trim();

            #endregion

            bool iDthisItem = DecideIdentification(puid, itemTitle, itemArtist, OnlyIdFilesWithMissingInfo);


            if (!iDthisItem) return;


            
            try
            {
                //TODO: Use the MusicBrainz API instead of calling external process
                Process p = StartMusicDnsProcess(Item, RemaininigTime, TracksToIdentify);
                ParseOutputImportMetadata(Item, p, location);
            }
            catch (Exception e)
            {
                Debugger.LogMessageToFile(
                    "An unexpected error occured in the Audio Identifying process: " + e);
                //MessageBox.Show(e.ToString());
            }


            TracksToIdentify--;
            RemaininigTime = TracksToIdentify*40;

        }


        private static Process StartMusicDnsProcess(IMLItem item, int remaininigTime, int TracksToIdentify)
        {
            string processString = Debugger.GetPluginPath() + "musicDNS.exe";
            string arguments = "\"" + item.Location + "\"";

            Process p = new Process
                            {
                                StartInfo =
                                    {
                                        FileName = processString,
                                        Arguments = arguments,
                                        UseShellExecute = false,
                                        CreateNoWindow = true,
                                        RedirectStandardOutput = true
                                    }
                            };


            string timeText;

            if (remaininigTime > 60)
            {
                timeText = remaininigTime / 60 + " minutes remaining.";
            }
            else
            {
                timeText = remaininigTime + " seconds remaining.";
            }

            MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress, "Identifying " + item.Name + "..." + TracksToIdentify + " items, " + timeText);

            //MessageBox.Show("Step 9");


            #region External process operations
            p.Start();

            int i = 0;
            while (!p.HasExited)
            {
                Thread.Sleep(100);
                Application.DoEvents();


                if (i >= 150)
                {

                    foreach (Process process in
                        Process.GetProcesses().Where(
                            process => process.ProcessName.StartsWith("example")))
                        process.Kill();

                }



                i++;
            }
            #endregion

            Thread.Sleep(1000);
            return p;
        }

        private static bool DecideIdentification(string PUID, string ItemTitle, string ItemArtist, bool OnlyIdFilesWithMissingInfo)
        {
            bool iDthisItem = false;

            if (OnlyIdFilesWithMissingInfo)
            {
                if (String.IsNullOrEmpty(PUID) && (String.IsNullOrEmpty(ItemTitle) || String.IsNullOrEmpty(ItemArtist)))
                    iDthisItem = true;
            }
            else
            {
                if (String.IsNullOrEmpty(PUID))
                    iDthisItem = true;
            }

            return iDthisItem;
        }


        private static void ParseOutputImportMetadata(IMLItem Item, Process p, string location)
        {

            string output = p.StandardOutput.ReadToEnd();

            MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress, "Query result: " + output);
                                       
            //MessageBox.Show(output);

            string[] lines = output.Split(Environment.NewLine.ToCharArray());

            bool hasTitle = false;

            #pragma warning disable 168
            foreach (string line in lines.Where(line => line.Contains("Title:")))
            #pragma warning restore 168
                hasTitle = true;


            if (!hasTitle) return;

            string title = String.Empty;
            string artist = String.Empty;
            string album = String.Empty;
            string year = String.Empty;
            string genre = String.Empty;
            string tracknumber = String.Empty;


            title = SetItemTagsFromOutput(Item, lines, title, ref artist, ref album, ref tracknumber, ref genre, ref year);

            SaveTagsToAudioFile(location, title, artist, album, tracknumber, genre, year);

        }



        private static string SetItemTagsFromOutput(IMLItem item, IEnumerable<string> lines, string title, ref string artist, ref string album, ref string tracknumber, ref string genre, ref string year)
        {
            string puid;
            foreach (string line in lines)
            {
                //MessageBox.Show(line);
                #region Set library tags

                #region Set Title
                if (line.Contains("Title:"))
                {
                    //MessageBox.Show("line contains Title!");

                    title = line.Remove(0, 7);

                    if (!String.IsNullOrEmpty(title.Trim()))
                    {
                        //MessageBox.Show(Title);
                        item.Tags["Title"] = title;
                        item.Name = title;
                        item.SaveTags();


                    }

                }
                #endregion

                #region Set Artist
                if (line.Contains("Artist:"))
                {
                    artist = line.Remove(0, 8);
                    //MessageBox.Show(artist);

                    if (!String.IsNullOrEmpty(artist))
                    {
                        item.Tags["Artist"] = artist;
                        item.SaveTags();

 
                    }
                    MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress, item.Name + " was succesfully identified!");
                    Thread.Sleep(1000);
                }
                #endregion

                #region Set Album
                if (line.Contains("Album:"))
                {
                    album = line.Remove(0, 7);

                    if (!String.IsNullOrEmpty(album))
                    {
                        //MessageBox.Show(Album);
                        item.Tags["Album"] = album;
                        item.SaveTags();
                    }

                }
                #endregion

                #region Set track number
                if (line.Contains("Track number:"))
                {
                    tracknumber = line.Remove(0, 14);

                    if (tracknumber.Trim() != "(null)")
                    {
                        //MessageBox.Show(tracknumber);
                        item.Tags["Track No."] = tracknumber;
                        item.SaveTags();
                    }

                }
                #endregion

                #region Set Genre
                if (line.Contains("Genre:"))
                {
                    genre = line.Remove(0, 7);

                    if (!String.IsNullOrEmpty(genre.Trim()))
                    {
                        //MessageBox.Show(Title);
                        item.Tags["Genre"] = genre;
                        item.SaveTags();
                    }

                }
                #endregion

                #region Set Year
                if (line.Contains("Year:"))
                {
                    year = line.Remove(0, 6);

                    if (!String.IsNullOrEmpty(year.Trim()))
                    {
                        //MessageBox.Show(Title);
                        item.Tags["Year"] = year;
                        item.SaveTags();
                    }

                }
                #endregion

                #region Set PUID

                if (!line.Contains("PUID:")) continue;

                puid = line.Remove(0, 8);

                if (String.IsNullOrEmpty(puid.Trim())) continue;

                //MessageBox.Show(PUID);
                item.Tags["PUID"] = puid;
                item.SaveTags();

                #endregion

                #endregion

            }
            return title;
        }


        private static void SaveTagsToAudioFile(string location, string title, string artist, string album, string tracknumber, string genre, string year)
        {
            try
            {

                File audiofile = File.Create(location);


                if (!String.IsNullOrEmpty(title) )
                {
                    audiofile.Tag.Title = title;
                    //TODO: Saving of the tags to the actual audio file should be performed only once!
                    audiofile.Save();
                }

                if (!String.IsNullOrEmpty(artist) )
                {
                    audiofile.Tag.AlbumArtists = new[] { artist };
                    audiofile.Save();
                }

                if (!String.IsNullOrEmpty(album))
                {
                    audiofile.Tag.Album = album;
                    audiofile.Save();
                }

                if (!String.IsNullOrEmpty(tracknumber))
                {
                    audiofile.Tag.Track = Convert.ToUInt16(tracknumber);
                    audiofile.Save();
                }

                if (!String.IsNullOrEmpty(year))
                {
                    audiofile.Tag.Year = Convert.ToUInt16(year);
                    audiofile.Save();
                }


                if (!String.IsNullOrEmpty(genre))
                {
                    audiofile.Tag.Genres = new[] { genre };
                    audiofile.Save();
                }


            }
            catch (UnauthorizedAccessException)
            {
                Debugger.LogMessageToFile("MediaFairy was unable to update the tags of the file " + location + ". Access to this file was restricted.");
            }

        }


        internal static void PerformMusicDnsAudioIdentification(ConnectionResult connectionresult, int allTracksToIdentify,
                                                               ref int tracksToIdentify, ref int remainingTime, string location,
                                                               IMLItem item, uint audioDuration)
        {
            if (Settings.WantMusicDNS)
            {
                if (tracksToIdentify >= 1)
                {
                    MainImportingEngine.CurrentProgress =
                        (allTracksToIdentify - tracksToIdentify)*100/
                        allTracksToIdentify;

                    Debugger.LogMessageToFile(
                        "Identifying audio track online using MusicDNS...");

                    IdentifyAudio(
                        Settings.WantMusicDNS, item, Settings.SkipLongTracks, audioDuration,
                        ref tracksToIdentify, ref remainingTime,
                        Settings.OnlyIdFilesWithMissingInfo, location, connectionresult);
                }
            }
        }
    }//endof Class


}//endof Namespace