
using MeediFier.OSDb;
using MeediFier.ImportingEngine;
using MeediOS;


namespace MeediFier.Code.Media_Updaters.Single_Item_Updaters.Movie_Item_Updater
{


    class FilmItemSecondaryUpdater
    {



        internal static bool PerformSecondaryFilmItemUpdating
            (IMLSection moviesSection,
             IMLItemList allFilmItems, IMLItem item, string location, string moviehash, string imdbid, bool fileServerIsOnline,
            ConnectionResult connectionresult)
        {

            if (Helpers.UserCancels
                (MainImportingEngine
                .SpecialStatus, item))
                return false;

            VideoSusbtitleDownloader.DownloadSubtitleForVideoUsingOsdbNet("Movies", item, location, moviehash, imdbid, connectionresult,
                MeediFier.Settings.UseSameSubtitlesFolder, MeediFier.Settings.SubtitlesFolder, true, fileServerIsOnline);

            //DEPRECATED: DownloadSubtitleForVideoParent.
            //VideoSusbtitleDownloader
            //    .DownloadSubtitleForVideoParent
            //    (moviehash, imdbid, item,
            //     location, connectionresult,
            //    MeediFier.Settings.UseSameSubtitlesFolder,
            //    MeediFier.Settings.SubtitlesFolder, true);
             

            //MediaArchivers
            //    .ArchiveFilm
            //    (Item, parent,
            //    Settings.ArchiveLocation);


            if (Helpers.UserCancels
                (MainImportingEngine
                .SpecialStatus, item))
                return false;


            FilmItemChainLinker
                .LinkFilmItemChains
                (moviesSection,
                 allFilmItems, item);


            if (Helpers.UserCancels
                (MainImportingEngine
                .SpecialStatus, item))
                return false;


            FilmItemProccessSigner
                .SignFilmItemProcess(item);



            return true;

        }



    }


}
