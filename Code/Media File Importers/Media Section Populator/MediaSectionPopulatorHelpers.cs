using System;
using System.IO;
using System.Windows.Forms;
using MeediFier.Code.Settings;
using MeediOS;

namespace MeediFier.MediaSnapshotEngine
{

    internal class MediaSectionPopulatorHelpers
    {
        internal static bool ProceedToImportAudioTrack(FileInfo file, bool isAudio)
        {


            if (!MediaImportersAndUpdatersCoreSettings.EnableMusicImporter) return false;

            if (!isAudio) return false;
            //Debugger.LogMessageToFile("File " + file.Name + " has been detected to contain audio.");

            return file.Length >= 1048576;

            //Debugger.LogMessageToFile("This file was considered to be a music track.");
        }

        internal static bool VideoIsLocatedInATvShowFolder(FileSystemInfo file)
        {
            bool isTvShow = false;

            foreach (string tvshowFolder in Settings.TvShowsFolders)
            {
                if (String.IsNullOrEmpty(tvshowFolder)) continue;

                if (!file.FullName.Contains(tvshowFolder)) continue;

                Debugger.LogMessageToFile(
                    "This video file is contained in the specified TV Series root directory and will be considered to be a TV episode.");

                isTvShow = true;
            }

            return isTvShow;
        }

        internal static bool ProceedToImportMovie(FileInfo file, int videoDuration)
        {
            Application.DoEvents();

            bool videoIsLocatedInAMovieFolder = VideoIsLocatedInAMovieFolder(file);

            Application.DoEvents();

            bool proceedToImport = videoDuration > 60 || videoIsLocatedInAMovieFolder;

            if (IsDvdImage(file))
                proceedToImport = true;

            if (IsBluRayImage(file))
                proceedToImport = true;

            if (IsRarMovie(file))
                proceedToImport = true;

            return proceedToImport;
            
        }

        internal static bool IsRarMovie(FileSystemInfo file)
        {
            return (Settings.ImportFilmsInRarArchives &&
                    file.Extension == ".r00");
        }

        internal static bool IsDvdImage(FileInfo file)
        {
            return (

                       Settings.ImportDvdImages
                       &&
                       //TODO: Retrieve disc image extensions from a text file.
                       //TODO: Implement seperate folder setting for DVD and BluRay images.
                       (
                           file.Extension == ".ISO" || file.Extension == ".iso" || 
                           file.Extension == ".bin" || file.Extension == ".BIN" ||
                           file.Extension == ".img" || file.Extension == ".IMG"
                       )
                       &&
                       //TODO: Size check should be removed for disc images
                       (file.Length > 4294967296 && file.Length < 4724464025)

                   );
        }

        internal static bool IsBluRayImage(FileInfo file)
        {
            return (

                       Settings.ImportBluRayImages
                       &&
                       (
                           file.Extension == ".ISO" || file.Extension == ".iso" || 
                           file.Extension == ".bin" || file.Extension == ".BIN" ||
                           file.Extension == ".img" || file.Extension == ".IMG"
                       )
                       &&
                       //TODO: Size check should be removed for disc images
                       ( file.Length > 10737418240 && file.Length < 42949672960 )

                   );
        }

        internal static bool VideoIsLocatedInAMovieFolder(FileSystemInfo file)
        {

            Application.DoEvents();

            bool isFilm = false;

            foreach (string filmsFolder in Settings.FilmsFolders)
            {
                if (String.IsNullOrEmpty(filmsFolder)) continue;

                if (!file.FullName.Contains(filmsFolder)) continue;

                Debugger.LogMessageToFile(
                    "This video file is contained in the specified films root directory and will be considered to be a film.");
                isFilm = true;
            }

            Application.DoEvents();

            return isFilm;
        }

        internal static bool SkipTrailer(FileSystemInfo file)
        {
            string fileNameWoExt = file.Name.Replace(file.Extension, "");

            return fileNameWoExt.EndsWith("-trailer");
        }




        internal static void ImportDates(IMLItem item, string externalID)
        {


                //try
                //{

                    FileInfo file = new FileInfo(externalID);

                    if (Settings.ImportDateCreated)
                    {

                            DateTime dateCreated = file.CreationTime;
                            item.Tags["DateCreated"] = dateCreated.ToShortDateString();
                            item.Tags["DateCreatedTicks"] = dateCreated.Ticks;
                        
                    }

                    if (Settings.ImportDateModified)
                    {
                            DateTime dateModified = file.LastWriteTime;
                            item.Tags["DateModified"] = dateModified.ToShortDateString();
                            item.Tags["DateModifiedTicks"] = dateModified.Ticks;     
                    }

                    if (Settings.ImportDateImported)
                    {
                        item.Tags["DateImported"] = DateTime.Today.ToShortDateString();
                        item.Tags["DateImportedTicks"] = DateTime.Today.Ticks;
                    }



                //}
                //catch (Exception e)
                //{

                //    Debugger.LogMessageToFile("[File Dates Importer] An unexpected error occured while" +
                //                              " the File Dates Importer was trying to create an instance of the file " 
                //                              + item.ExternalID + ". The error was: " + e);   

                //}
                


                item.SaveTags();

       
        }
    


    
    }


}
