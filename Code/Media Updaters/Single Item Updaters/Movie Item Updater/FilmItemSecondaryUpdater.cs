using MediaFairy.ImportingEngine;
using MeediOS;


namespace MediaFairy.Code.Media_Updaters.Single_Item_Updaters.Movie_Item_Updater
{


    class FilmItemSecondaryUpdater
    {



        internal static bool PerformSecondaryFilmItemUpdating
            (IMLSection moviesSection,
             IMLItemList allFilmItems, IMLItem item)
        {

            if (Helpers.UserCancels
                (MainImportingEngine
                .SpecialStatus, item))
                return false;


            //VideoSusbtitleDownloader
            //    .DownloadSubtitleForVideoParent
            //    (moviehash, imdbid, Item,
            //     location, connectionresult,
            //    Settings.UseSameSubtitlesFolder,
            //    Settings.SubtitlesFolder, true);
             

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
