using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using MediaFairy.Code;
using MediaFairy.Code.Metadata_Scrapers.Cover_Art;
using MediaFairy.ImportingEngine;
using MeediOS;
using TagLib;
using File = System.IO.File;

namespace MediaFairy.SingleItemUpdaters
{


    class MusicTrackItemUpdaterHelpers
    {

        internal static void OrganizeAudioFile(string artist,
            string album, string title, IMLItem item)
        {

            if (ReturnCases(artist, album, title, item))
                return;


            string location = item.Location;
            var partfileInfo = new FileInfo(location);
            string extension = partfileInfo.Extension;
            long filesize = partfileInfo.Length;
            long secstocopy = filesize/10000000;

            string locationToMove;

            if (ConstructMoveLocation(item, extension, out locationToMove))
                return;


            MoveFileUpdateItem(item, locationToMove, location, secstocopy);


        }



        private static bool ConstructMoveLocation(IMLItem item, 
            string extension, out string locationToMove)
        {

            bool cancelMoving;

            if (!Settings.SortingDestinationMusic.EndsWith(@"\"))
                Settings.SortingDestinationMusic = Settings.SortingDestinationMusic + @"\";


            string directoryToMove = ConstructMoveDirectory
                (item, out cancelMoving);


            string newFilename;

            if (ConstructNewFilename(item, out newFilename))
            {
                locationToMove = null;
                return true;
            }


            if (!directoryToMove.EndsWith(@"\"))
                directoryToMove = directoryToMove + @"\";


            locationToMove = directoryToMove +
                             newFilename + extension;


            return cancelMoving;

        }



        private static string ConstructMoveDirectory(IMLItem item, out bool cancelMoving)
        {

            string directoryToMove;

            if (Settings.DirectoryStructure == "Simple- Default directory structures")
            {
                directoryToMove = ConstructNewDirectoryDefaultStructure
                    (item);
            }
            else
            {
                directoryToMove = ConstructNewDirectoryCustomStructure
                    (item, out cancelMoving);
            }

            cancelMoving = false;
            return directoryToMove;
        }


        private static string ConstructNewDirectoryCustomStructure(IMLItem item, out bool cancelMoving)
        {

            string[] folderStructure = Settings.DirectoryTagMaskMusic.Split('\\');
            string directoryToMove = Settings.SortingDestinationMusic;

            foreach (string folder in folderStructure)
            {
                string tagname = folder.Trim('<', '>');
                string tagvalue = Helpers.GetTagValueFromItem(item, tagname);


                if (String.IsNullOrEmpty(tagvalue))
                {

                    MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress,
                        "The item's tag '" + tagname + "' contains an empty value. " +
                        "Sorting operation was cancelled.");

                    Thread.Sleep(2500);
                }
                else
                {
                    StringProcessors.NormalizePath(tagvalue);
                    directoryToMove += tagvalue + @"\";

                    if (!Directory.Exists(directoryToMove))
                        Directory.CreateDirectory(directoryToMove);
                
                }
            
            }

            cancelMoving = false;
            return directoryToMove;


        }


        private static string ConstructNewDirectoryDefaultStructure(IMLItem item)
        {

            string artistNormalized = Helpers.GetTagValueFromItem(item, "Artist");
            string albumNorm = Helpers.GetTagValueFromItem(item, "Album");


            string directoryToMove = Settings.SortingDestinationMusic + artistNormalized + @"\" +
                                     albumNorm + @"\";

            if (!Directory.Exists(directoryToMove))
                Directory.CreateDirectory(directoryToMove);

            return directoryToMove;
        }


        private static bool ReturnCases(string artist, string album
            , string title, IMLItem item)
        {

            if (!Settings.WantSortingMusic)
                return true;

            if (String.IsNullOrEmpty(artist))
                return true;

            if (String.IsNullOrEmpty(album))
                return true;

            if (String.IsNullOrEmpty(title))
                return true;

            string readyToSort = Helpers.GetTagValueFromItem(item, "ReadyToSort");


            if (Settings.OnlyTaggedForMoving &&
                (!Settings.OnlyTaggedForMoving ||
                 readyToSort != "true"))
                return true;

            string fileSorted = Helpers.GetTagValueFromItem(item, "FileSorted");


            return fileSorted == "true";

        }


        private static bool ConstructNewFilename(IMLItem item, out string fileTitle)
        {

            string title =  Helpers.GetTagValueFromItem(item, "Title");
            string track =  Helpers.GetTagValueFromItem(item, "Track");
            string artist = Helpers.GetTagValueFromItem(item, "Artist");
            string album =  Helpers.GetTagValueFromItem(item, "Album");


            if (String.IsNullOrEmpty(title))
            {
                fileTitle = null;
                return true;
            }

            if (String.IsNullOrEmpty(track))
            {
                fileTitle = null;
                return true;
            }

            if (String.IsNullOrEmpty(artist))
            {
                fileTitle = null;
                return true;
            }

            if (String.IsNullOrEmpty(album))
            {
                fileTitle = null;
                return true;
            }

            fileTitle = artist + " - " + album + " - " + track + " - " + title;
   
            fileTitle = StringProcessors.NormalizePath(fileTitle);
            
            return false;
        
        }


        private static void MoveFileUpdateItem
            (IMLItem item, string locationToMove, string location, long secstocopy)
        {


            MainImportingEngine.ThisProgress.Progress
                (MainImportingEngine.CurrentProgress,
                "Moving " + Helpers.GetTagValueFromItem(item, "Title") +
                "... " + secstocopy + " seconds remaining.");



            Thread.Sleep(500);


            File.Move(location, locationToMove);
            item.Location = locationToMove;
            item.ExternalID = locationToMove;
            item.Tags["FileSorted"] = "true";
            item.SaveTags();

        }




        internal static List<string> DownloadAlbumCoverArt
            (bool fileServerIsOnline, ConnectionResult connectionresult,
            string mdfSettingsc, IBaseSystem ibs, List<string> albumsWithoutcover,
            bool trackHasCover, string album, string artist, IMLItem item,
            bool isUNC, ref List<string[]> albumsWithCover)
        {

            if (!Settings.WantAlbumCovers)
                return albumsWithoutcover;

            if (!connectionresult.InternetConnectionAvailable 
                && Settings.ConnectionDiagnosticsEnabled)
                return albumsWithoutcover;

            if (String.IsNullOrEmpty(artist)
                || String.IsNullOrEmpty(album)) 
                return albumsWithoutcover;
            



            //AmazonMusicAlbumCoverArtDownloader
            //    .DownloadMusicAlbumCoverArtFromAmazon
            //    (item, MediaSectionsAllocator.MusicSection, 
            //    artist, album, fileServerIsOnline, isUNC, ref albumsWithoutcover,
            //    ref albumsWithCover, trackHasCover, mdfSettingsc, ibs);



            return albumsWithoutcover;


        }





        internal static uint ExtractAudioDuration(IMLItem item)
        {
            Debugger.LogMessageToFile("Retrieving audio file's duration...");

            string audioDurationTmp = Helpers.GetTagValueFromItem(item, "AudioDuration");


            uint audioDuration = (uint) (!String.IsNullOrEmpty(audioDurationTmp)
                                             ? Convert.ToUInt16(audioDurationTmp)
                                             : 0);
            return audioDuration;
        }

        internal static bool ImportAlbumCoverArt
            (string parentDir, IMLItem item, TagLib.File audiofile)
        {
         

            Debugger.LogMessageToFile("Extracting cover image from track '"
                                      + item.Name + "'...");

            Helpers.UpdateProgress("Updating Music Section",
                                   "Extracting cover image from track '" + item.Name + "'...", item);

            IPicture[] images = audiofile.Tag.Pictures;

            if (!String.IsNullOrEmpty(item.ImageFile))
                return true;

            if (File.Exists(parentDir + "folder.jpg"))
            {
                item.ImageFile = parentDir + "folder.jpg";
                item.SaveTags();
                return true;
            }

            foreach (IPicture image in images)
            {
                ByteVector imagedata = image.Data;
                byte[] imagebytes = imagedata.Data;
                Helpers.SaveStreamToFile(imagebytes, parentDir + "folder.jpg");
                item.ImageFile = parentDir + "folder.jpg";
                item.SaveTags();
                return true;
            }

            return false;

        }

        internal static string ExtractKeyMetatadaFromItem(IMLItem item, out string title, out string album)
        {
            Debugger.LogMessageToFile("Updating key item variables...");

            string artist = Helpers.GetTagValueFromItem(item, "Artist");
            title = Helpers.GetTagValueFromItem(item, "Title");

            album = Helpers.GetTagValueFromItem(item, "Album");

            album = NormalizeAlbumName(item, album);

            
            return artist;
        }

        internal static bool PerformAudioIdentification(ConnectionResult connectionresult, int allTracksToIdentify,
                                                       ref int tracksToIdentify, ref int remainingTime, uint audioDuration,
                                                       IMLItem item, string artist, string title, string location,
                                                       string parentDir, string album)
        {
            bool trackHasCover = TagLibAudioTagPopulator.PerformTagLibAudioTagExtraction
                (title, artist, parentDir, album, item, location);


            //MusicDnsIdentifier.PerformMusicDnsAudioIdentification(connectionresult,
            //            allTracksToIdentify, ref tracksToIdentify,
            //            ref remainingTime, location, item, audioDuration);
          
            
            return trackHasCover;
        
        
        }

        internal static bool PerformAudioIdentificationAndDownloadAlbumCoverArt(bool fileServerIsOnline,
                                                                               ConnectionResult connectionresult,
                                                                               string mdfSettingsc, IBaseSystem iBaseSystem,
                                                                               int allTracksToIdentify, ref List<string[]> albumsWithCover,
                                                                               ref List<string> albumsWithoutcover,
                                                                               ref int tracksToIdentify, ref int remainingTime,
                                                                               string parentDir, bool isUNC, string location,
                                                                               uint audioDuration, IMLItem item,
                                                                               ref string artist, ref string title,
                                                                               ref string album)
        {
            if (fileServerIsOnline || !isUNC)
            {
                var trackHasCover = PerformAudioIdentification(connectionresult,
                                                                                            allTracksToIdentify,
                                                                                            ref tracksToIdentify,
                                                                                            ref remainingTime,
                                                                                            audioDuration, item, artist, title,
                                                                                            location, parentDir, album);

                if (Helpers.UserCancels(MainImportingEngine.SpecialStatus, item))
                    return false;

                artist = ExtractKeyMetatadaFromItem
                    (item, out title, out album);

                albumsWithoutcover = DownloadAlbumCoverArt
                    (fileServerIsOnline, connectionresult,
                     mdfSettingsc, iBaseSystem, albumsWithoutcover,
                     trackHasCover, album, artist, item,
                     isUNC, ref albumsWithCover);
            }
            return true;
        }

        internal static string NormalizeAlbumName(IMLItem item, string album)
        {
            if (string.IsNullOrEmpty(album))
                return string.Empty;

            Debugger.LogMessageToFile("Normalizing Album name...");
            
            album = album.Replace("(disc 1)", "");
            album = album.Replace("(disc 2)", "");
            album = album.Replace("(disc 3)", "");
            
            item.Tags["Album"] = album;        
            item.SaveTags();
            
            return album;
        }


    }



}
