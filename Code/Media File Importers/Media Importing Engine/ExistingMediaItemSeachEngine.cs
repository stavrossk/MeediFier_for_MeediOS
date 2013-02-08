using System.Collections.Generic;
using System.IO;
using System.Linq;
using MeediOS;

namespace MediaFairy.MediaFileImportingEngine
{



    class ExistingMediaItemSeachEngine
    {


        internal static bool SkipAlreadyImportedFiles
            (FileSystemInfo file, IEnumerable<string> filmLocations, 
             IEnumerable<string> tvshowsLocations, IEnumerable<string> musicLocations)
        {

            //TODO: Make existing item search faster by specifying the media type to be scanned.

            Helpers.UpdateProgress("Importing Media Files...",
                "Searching media library for existing file entry...", null);


            if (SearchForExistingItemInMoviesSection(file, filmLocations))
                return true;

            if (SearchForExistingItemInTvShowsSection(file, tvshowsLocations))
                return true;

            // ReSharper disable ConvertIfStatementToReturnStatement
            if (SearchForExistingItemInMusicSection(file, musicLocations))
            // ReSharper restore ConvertIfStatementToReturnStatement
                return true;




            return false;


        }





        private static bool SearchForExistingItemInMoviesSection(FileSystemInfo file, IEnumerable<string> filmLocations)
        {

            if (Settings.NowImportingMediaType == "Movies")
            {

                if (filmLocations != null)
                {

                    // ReSharper disable LoopCanBeConvertedToQuery
                    foreach (var movieFolder in Settings.FilmsFolders)
                    // ReSharper restore LoopCanBeConvertedToQuery
                    {


                        if (!file.FullName.Contains(movieFolder))
                            continue;

                        //if (filmLocations.Any(location => location == file.FullName))
                        //    return true;

                        IMLItem item =
                        MediaSectionsAllocator.MoviesSection.FindItemByExternalID
                        (file.FullName);


                        if (item != null)
                            return true;

                    }

                }

            }
            return false;

        }



        private static bool SearchForExistingItemInTvShowsSection(FileSystemInfo file, IEnumerable<string> tvshowsLocations)
        {

            if (Settings.NowImportingMediaType == "TvShows")
            {

                if (tvshowsLocations != null)
                {

                    // ReSharper disable LoopCanBeConvertedToQuery
                    foreach (var tvShowsFolder in Settings.TvShowsFolders)
                        // ReSharper restore LoopCanBeConvertedToQuery
                    {

                        if (!file.FullName.Contains(tvShowsFolder))
                            continue;


                        //if (tvshowsLocations.Any(location => location == file.FullName))
                        //    return true;

                        IMLItem item =
                          MediaSectionsAllocator.TvEpisodesSection.FindItemByExternalID
                          (file.FullName);

                        if (item != null)
                            return true;

                    }
            
                }
        
            }
            return false;
        
        }



        private static bool SearchForExistingItemInMusicSection(FileSystemInfo file, IEnumerable<string> musicLocations)
        {

            if (Settings.NowImportingMediaType == "Music")
            {

                if (musicLocations != null)
                {

                    // ReSharper disable LoopCanBeConvertedToQuery
                    foreach (var musicFolder in Settings.MusicFolders)
                        // ReSharper restore LoopCanBeConvertedToQuery
                    {

                        if (!file.FullName.Contains(musicFolder))
                            continue;

                        //if (musicLocations.Any(location => location == file.FullName))
                        //    return true;

                        IMLItem item =
                            MediaSectionsAllocator.MusicSection.FindItemByExternalID(file.FullName);

                        if (item != null)
                            return true;
                    
                    }

                }

            }
            return false;

        }




    }



}
