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
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Windows.Forms;

#if USE_MEEDIO
using Meedio;
#elif USE_MEEDIOS
using MeediOS;
#endif


namespace MediaFairy
{
    class iTunesSynchronizer
    {


        public static void SynchronizeITunesIpod(IMLSection MusicSection)
        {


            Importer.CurrentProgress = 0;
            Importer.GeneralStatus = "Synchronizing iTunes...";
            Importer.SpecialStatus = String.Empty;

            #region iTunes % iPod


            if (Settings.WantItunesSync && Settings.WantToUpdateMusicSection && MusicSection != null)
            {
                try
                {

                    iTunesLib.IiTunes iTunesApp;
                    iTunesLib.IITLibraryPlaylist playlist;
                    iTunesLib.IITTrackCollection TrackCollection;

                    #region Launch itunes
                    //Importer.CurrentProgress++;
                    Importer.ThisProgress.Progress(Importer.CurrentProgress, "Launching iTunes...");
                    iTunesApp = new iTunesLib.iTunesAppClass();

                    Thread.Sleep(5000);

                    iTunesLib.IITWindowCollection windows = iTunesApp.Windows;
                    foreach (iTunesLib.IITWindow window in windows)
                    {

                        if (window.Name == "iTunes")
                        {
                            window.Minimized = true;
                        }

                    }

                    #endregion


                    #region iTunes Synchronizer




                    #region sync libraries


                    playlist = iTunesApp.LibraryPlaylist;
                    TrackCollection = playlist.Tracks;



                    MusicSection.BeginUpdate();
                    try
                    {

                        foreach (iTunesLib.IITTrack track in TrackCollection)
                        {

                            if (track.Kind == iTunesLib.ITTrackKind.ITTrackKindFile)
                            {
                                iTunesLib.IITPlaylist trackPlaylist = track.Playlist;
                                //MessageBox.Show(trackPlaylist.Name);
                                //iTunesLib.IITPlaylist track_playlist = track.Playlist;
                                //MessageBox.Show("Track playlist: " + track_playlist.Name);
                                iTunesLib.IITFileOrCDTrack file = (iTunesLib.IITFileOrCDTrack)track;

                                if (file.Location != "" && file.Location != null)
                                {

                                    FileInfo fiB = new FileInfo(file.Location);

                                    //MessageBox.Show(fiB.Extension);

                                    #region check to make sure that the file is not video...
                                    if (fiB.Extension != ".m4v")
                                    {

                                        IMLItem Item = MusicSection.FindItemByLocation(file.Location);

                                        if (Item == null && file.Podcast == false)
                                        {

                                            Importer.ThisProgress.Progress(Importer.CurrentProgress, "Adding audio track " + track.Name + " to music library...");
                                            Item = MusicSection.AddNewItem(track.Name, file.Location);


                                            #region write item tags

                                            Item.Tags["Artist"] = track.Artist;
                                            Item.Tags["Album"] = track.Album;
                                            Item.Tags["Title"] = track.Name;

                                            Item.Tags["Genre"] = track.Genre;
                                            if (track.TrackNumber != 0)
                                                Item.Tags["Track"] = track.TrackNumber;
                                            if (track.Year != 0)
                                                Item.Tags["Year"] = track.Year;

                                            try
                                            {
                                                if (file.Lyrics != null && file.Lyrics != "")
                                                    Item.Tags["Lyrics"] = file.Lyrics;
                                            }
                                            catch (Exception)
                                            { }


                                            if (track.Rating != 0)
                                                Item.Tags["Rating"] = Convert.ToString(track.Rating);
                                            if (track.SampleRate != 0)
                                                Item.Tags["SampleRate"] = track.SampleRate;
                                            if (track.BitRate != 0)
                                                Item.Tags["BitRate"] = track.BitRate;
                                            if (track.Duration != 0)
                                                Item.Tags["Duration"] = track.Duration;

                                            #endregion


                                            #region save artwork to file and add to imagefield

                                            try
                                            {
                                                iTunesLib.IITArtworkCollection artworks = track.Artwork;
                                                foreach (iTunesLib.IITArtwork artwork in artworks)
                                                {
                                                    FileInfo fi = new FileInfo(file.Location);
                                                    DirectoryInfo di = fi.Directory;
                                                    string trackDirectory = di.FullName;

                                                    string artworkPath = trackDirectory + @"\" + "folder.jpg";
                                                    //MessageBox.Show(artworkPath);
                                                    artwork.SaveArtworkToFile(artworkPath);
                                                    Item.ImageFile = artworkPath;
                                                }
                                            }
                                            catch (Exception)
                                            { }

                                            #endregion

                                            Item.SaveTags();
                                        }

                                    }
                                    #endregion

                                }

                            }

                        }


                    }
                    catch (Exception e)
                    {
                        MusicSection.EndUpdate();
                        MessageBox.Show(e.ToString());
                    }

                    MusicSection.EndUpdate();

                    #endregion


                    #endregion



                    #region manage and Update iPod

                    //iTunesApp.UpdateIPod();



                    iTunesLib.IITSourceCollection sources = iTunesApp.Sources;

                    foreach (iTunesLib.IITSource source in sources)
                    {

                        //MessageBox.Show(source.Name);


                        if (source.Kind == iTunesLib.ITSourceKind.ITSourceKindIPod)
                        {
                            object iSource = source;

                            //iTunesLib.IITLibraryPlaylist MyPlaylist =(iTunesLib.IITLibraryPlaylist) iTunesApp.CreatePlaylistInSource("MyPlaylist", ref iSource);


                            //MyPlaylist.AddFile("D:\test.mp3");



                            //iTunesLib.IITPlaylistCollection iPodPlaylists = source.Playlists;

                            //foreach (iTunesLib.IITPlaylist ipodPlaylist in iPodPlaylists)
                            //{
                            //    //MessageBox.Show("iPod library name:" + ipodlibrary.Name);
                            //    //MessageBox.Show("Duration: " + Convert.ToString(ipodlibrary.Duration));

                            //    if (ipodPlaylist.Kind == iTunesLib.ITPlaylistKind.ITPlaylistKindLibrary)
                            //    {
                            //        iTunesLib.IITLibraryPlaylist iPodLibrary = (iTunesLib.IITLibraryPlaylist)ipodPlaylist;


                            //        iPodLibrary.AddFile("D:\test.mp3");

                            //    }

                            //}


                        }


                        //foreach (iTunesLib.IITLibraryPlaylist ipodlibrary in ipodPlaylists)
                        //{
                        //    MessageBox.Show("Passed 4");
                        //    MessageBox.Show(Convert.ToString(ipodlibrary.Duration));
                        //}


                    }

                    #endregion

                }
                catch (Exception)
                {

                }

            }


            #endregion





        }




    }
}
