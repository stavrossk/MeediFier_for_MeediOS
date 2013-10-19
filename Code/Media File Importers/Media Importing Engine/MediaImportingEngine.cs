//'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
//'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
//''    MediaFairy                                                               ''
//''    Copyright (C) 2008-2011  Stavros Skamagkis                               ''
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


using System.Collections.Generic;
using System.Collections;
using System.Threading;
using MeediFier.Code.Settings;
using MeediFier.MediaSnapshotEngine;

#if USE_MEEDIO
using MeediFier.Code.Settings;
using MeediFier.MediaSnapshotEngine;
using Meedio; 
#elif USE_MEEDIOS

#endif



namespace MeediFier
{
    class MediaImportingEngine
    {
   
// ReSharper disable InconsistentNaming


        public static void ImportMediaFilesMAIN
            (IEnumerable<string> combinedSceneTags, Importer importer)
        {


            string pluginpath;

            var mediaSnapshotsFI =
                MediaImportingEngineHelpers.LoadMediaSnapshots(out pluginpath);


            #region Set EnableMediaImporter flag
            if (
            (MediaImportersAndUpdatersCoreSettings.EnableMovieImporter
            && MediaSectionsAllocator.MoviesSection != null) 
            ||
            (MediaImportersAndUpdatersCoreSettings.EnableTvSeriesImporter
            && MediaSectionsAllocator.TvEpisodesSection != null)
            ||
            (MediaImportersAndUpdatersCoreSettings.EnableMusicImporter 
            && MediaSectionsAllocator.MusicSection != null)
           )
            { MediaImportersAndUpdatersCoreSettings.EnableMediaImporters = true; }
            else MediaImportersAndUpdatersCoreSettings.EnableMediaImporters = false;
            #endregion


            #region Return Cases
            if (!MediaImportersAndUpdatersCoreSettings.EnableMediaImporters)
                return;

            Debugger.LogMessageToFile("Media File Importer is enabled.");
            if (MediaImportersAndUpdatersCoreSettings.RootMediaFolders == null || MediaImportersAndUpdatersCoreSettings.RootMediaFolders.Length == 0)
                return;

#endregion


            #region Declare Vars
            string[] filmLocations;
            string[] musicLocations;
            string[] tvShowsLocations;
            
            IList<string> extensionsToIgnore;
            string[] videoExtensions;
            string[] videoExtensionsCommon;
            string[] audioExtensions;
            #endregion



            MediaImportingEngineHelpers.PerformPreImportCaching
                (mediaSnapshotsFI, pluginpath, out filmLocations,
                out musicLocations, out tvShowsLocations, out extensionsToIgnore,
                out videoExtensions, out audioExtensions, out videoExtensionsCommon);



            ImportMoviesSeriesMusic(combinedSceneTags, videoExtensions, 
                audioExtensions, pluginpath, filmLocations,
                ref extensionsToIgnore, musicLocations, tvShowsLocations,
                videoExtensionsCommon, importer);
       
        
        
        }



        private static void ImportMoviesSeriesMusic
            (IEnumerable<string> combinedSceneTags, string[] videoExtensions,
            string[] audioExtensions, string pluginpath, string[] filmLocations,
            ref IList<string> extensionsToIgnore, string[] musicLocations,
            string[] tvShowsLocations, IEnumerable<string> videoExtensionsCommon, Importer importer)
        {
            ImportingEngineHelpers.BeginUpdatingSections();


            if (MediaImportersAndUpdatersCoreSettings.EnableMovieImporter)
            {


                Helpers.UpdateProgress
                    ("Importing Media Files...",
                    "Preparing to Import Movies...",null);
              
                Thread.Sleep(200);

                Settings.NowImportingMediaType = "Movies";

                DirectoryScanner.ScanMediaDirectories(Settings.FilmsFolders,
                    ref extensionsToIgnore, filmLocations, tvShowsLocations,
                    musicLocations, videoExtensions, audioExtensions,
                    combinedSceneTags, videoExtensionsCommon, importer);
            }


            if (MediaImportersAndUpdatersCoreSettings.EnableTvSeriesImporter)
            {
                Helpers.UpdateProgress("Importing Media Files...",
                    "Preparing to Import TV Shows...", null);
                Thread.Sleep(200);

                Settings.NowImportingMediaType = "TvShows";

                DirectoryScanner.ScanMediaDirectories(Settings.TvShowsFolders,
                    ref extensionsToIgnore, filmLocations, tvShowsLocations,
                    musicLocations, videoExtensions, audioExtensions,
                    combinedSceneTags, videoExtensionsCommon, importer);
            }

            if (MediaImportersAndUpdatersCoreSettings.EnableMusicImporter)
            {

                Helpers.UpdateProgress("Importing Media Files...",
                    "Preparing to Import Music...", null);
                Thread.Sleep(200);

                Settings.NowImportingMediaType = "Music";

                DirectoryScanner.ScanMediaDirectories(Settings.MusicFolders,
                        ref extensionsToIgnore, filmLocations, tvShowsLocations,
                        musicLocations, videoExtensions, audioExtensions,
                        combinedSceneTags, videoExtensionsCommon, importer);
            }



            MediaImportingEngineHelpers
                .WriteNonMediaExtensionsToFile
                (ref extensionsToIgnore, pluginpath);


            ImportingEngineHelpers.FinishUpdatingSections();


        }
   
    
    }



} //endof namespace
