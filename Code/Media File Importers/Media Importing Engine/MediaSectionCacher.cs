using System.Linq;
using MeediFier.ImportingEngine;
using MeediOS;



namespace MeediFier.Code.Media_File_Importers.Media_Importing_Engine
{



    class MediaSectionCacher
    {





        internal static string[] CacheMoviesSection()
        {

            MainImportingEngine.CurrentProgress = 0;
            MainImportingEngine.ThisProgress.Progress
                (MainImportingEngine.CurrentProgress, "Caching Movies section...");

            int[] itemIDs = MediaSectionsAllocator.MoviesSection != null
                                ?  MediaSectionsAllocator.MoviesSection.GetAllItemIDs() 
                                :  new int[] { };


            int itemsCount = itemIDs.Length;
            string[] filmLocations = new string[itemsCount];
            int cacheCounter = 0;


            foreach (IMLItem item in itemIDs.Select(id => MediaSectionsAllocator.MoviesSection.FindItemByID(id)))
            {
                filmLocations[cacheCounter] = item.Location;
                cacheCounter++;
            }

            return filmLocations;
        }

        internal static string[] CacheTvShowsSection()
        {
            int[] itemIDs = new int[] {};

            MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress, "Caching Tv shows section...");

            if (MediaSectionsAllocator.TvEpisodesSection != null)
                itemIDs = MediaSectionsAllocator.TvEpisodesSection.GetAllItemIDs();

            int itemsCount = itemIDs.Length;
            string[] tvShowsLocations = new string[itemsCount];
            int cacheCounter = 0;

            foreach (IMLItem item in itemIDs.Select(id => MediaSectionsAllocator.TvEpisodesSection.FindItemByID(id)))
            {
                tvShowsLocations[cacheCounter] = item.Location;
                cacheCounter++;
            }

            return tvShowsLocations;
        }

        internal static string[] CacheMusicSection()
        {

            int[] itemIDs = new int[] {};

            Helpers.UpdateProgress("Importing Media Files..", "Caching Music Section...",  null);

            if (MediaSectionsAllocator.MusicSection != null)
                itemIDs = MediaSectionsAllocator.MusicSection.GetAllItemIDs();

            int itemsCount = itemIDs.Length;
            string[] musicLocations = new string[itemsCount];
            int cacheCounter = 0;

            foreach (IMLItem item in itemIDs.Select(id => MediaSectionsAllocator.MusicSection.FindItemByID(id)))
            {
                //Application.DoEvents();
                //Helpers.UpdateProgress("Importing Media Files..", "[items processed: " + cacheCounter + "] Caching Music Section...", null);
                musicLocations[cacheCounter] = item.Location;
                cacheCounter++;
                
            }

            return musicLocations;
        }
    }
}
