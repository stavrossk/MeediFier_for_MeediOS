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
using System.Threading;
using System.Windows.Forms;
using MeediFier.Code.Settings;
using MeediFier.ImportingEngine;
using MeediOS;
//TODO: Add Meedio


namespace MeediFier
{
    public static class MediaSectionsAllocator
    {

        public static IMLSection MoviesSection;
        public static IMLSection MusicSection;
        public static IMLSection TvEpisodesSection;
        public static IMLSection NewsSection;





        internal static void AllocateMediaSections
            (IMLSection section)
        {
            Debugger.LogMessageToFile
                ("Allocating Media Sections..");

            AllocateMoviesSection(section);

            AllocateTvSections(section);

            AllocateMusicSection(section);

            AllocateRssNewsSection(section);

        }


        //private static void AllocateGamesSection(IMLSection section)
        //{
        //    if (Settings.EnableGamesUpdater || Settings.EnableGamesImporter)
        //    {
        //        if (section.Name == Settings.GamesLibrary)
        //            GamesSection = section;
        //        else if (!String.IsNullOrEmpty(Settings.GamesLibrary))
        //            GamesSection = MainImportingEngine.Library.FindSection(Settings.GamesLibrary, true);
        //        else
        //        {
        //            GamesSection = null;
        //            StatusForm.statusForm.TrayIcon.ShowBalloonTip(5000, "Games library section was not specified", "You have chosen to import or update the PC Games section but you have not specified such a section. Importing or updating of this section will not be performed.", ToolTipIcon.Warning);
        //            Thread.Sleep(5000);
        //        }

        //    }
        //    else GamesSection = null;

        //}


        private static void AllocateRssNewsSection(IMLSection section)
        {

            //TODO: Is this already declared correctly?
            //IMLSection NewsSection;

            if (Settings.EnableRssImporter)
            {
                if (section.Name == Settings.NewsLibrary)
                    NewsSection = section;
                else if (!String.IsNullOrEmpty(Settings.NewsLibrary))
                    NewsSection = MainImportingEngine.Library.FindSection(Settings.NewsLibrary, true);
                else
                {
                    NewsSection = null;
                    StatusForm.statusForm.TrayIcon.ShowBalloonTip(5000, "News section was not specified", "You have chosen to import or update the RSS News section but you have not specified such a section. Importing or updating of this section will not be performed.", ToolTipIcon.Warning);
                    Thread.Sleep(5000);
                }
            }
            else NewsSection = null;
        }



        //private static void AllocateFilmReleasesSection(IMLSection section)
        //{
        //    if (!Settings.DownloadFilmReleases) return;

        //    if (section.Name == Settings.ReleasesLibrary)
        //        ReleasesSection = section;
        //    else if (!String.IsNullOrEmpty(Settings.ReleasesLibrary))
        //        ReleasesSection = MainImportingEngine.Library.FindSection(Settings.ReleasesLibrary, true);
        //    else
        //    {
        //        StatusForm.statusForm.TrayIcon.ShowBalloonTip(5000, "Releases section was not specified", "You have chosen to import or update the Releases section but you have not specified such a section. Importing or updating of this section will not be performed.", ToolTipIcon.Warning);
        //        Thread.Sleep(5000);
        //    }

        //}



        //private static void AllocateBooksSection(IMLSection section)
        //{
        //    if (!Settings.EnableBooksImporter && !Settings.EnableBooksSectionUpdater) return;

        //    if (section.Name == Settings.BooksLibrary)
        //        BooksSection = section;
        //    else if (!String.IsNullOrEmpty(Settings.BooksLibrary))
        //        BooksSection = MainImportingEngine.Library.FindSection(Settings.BooksLibrary, true);
        //    else
        //    {
        //        StatusForm.statusForm.TrayIcon.ShowBalloonTip(5000, "Books section was not specified", "You have chosen to import or update the Books section but you have not specified such a section. Importing or updating of this section will not be performed.", ToolTipIcon.Warning);
        //        Thread.Sleep(5000);
        //    }
        //}



        private static void AllocateMusicSection(IMLSection section)
        {
            if (!MediaImportersAndUpdatersCoreSettings.EnableMusicImporter && !Settings.EnableMusicSectionUpdater) return;

            if (section.Name == Settings.MusicLibrary)
                MusicSection = section;
            else if (!String.IsNullOrEmpty(Settings.MusicLibrary))
                MusicSection = MainImportingEngine.Library.FindSection(Settings.MusicLibrary, true);
            else
            {
                StatusForm.statusForm.TrayIcon.ShowBalloonTip(5000, "Music section was not specified", "You have chosen to import or update the Music section but you have not specified such a section. Importing or updating of this section will not be performed.", ToolTipIcon.Warning);
                Thread.Sleep(5000);
            }

        }

        private static void AllocateTvSections(IMLSection section)
        {


            if (!MediaImportersAndUpdatersCoreSettings.EnableTvSeriesImporter
                && !Settings.EnableTvShowsSectionUpdater)
                return;


            if (section.Name == Settings.TvShowsLibrary)
                TvEpisodesSection = section;
            else if (!String.IsNullOrEmpty
                (Settings.TvShowsLibrary))
                TvEpisodesSection = MainImportingEngine
                    .Library.FindSection
                    (Settings.TvShowsLibrary, true);


            else
            {


                StatusForm.statusForm.TrayIcon.ShowBalloonTip
                    (5000, "TV Episodes library section was not specified",
                    "You have chosen to import or update the TV Episodes section " +
                    "but you have not specified such a section." +
                    " Importing or updating of this section" +
                    " will not be performed.", ToolTipIcon.Warning);


                Thread.Sleep(5000);
            }




        }

        private static void AllocateMoviesSection(IMLSection section)
        {
            if (!MediaImportersAndUpdatersCoreSettings.EnableMovieImporter && !Settings.EnableMoviesSectionUpdater) return;

            //MessageBox.Show(FilmsLibrary);

            if (section.Name == Settings.FilmsLibrary)
                MoviesSection = section;
            else if (!String.IsNullOrEmpty(Settings.FilmsLibrary))
                MoviesSection = MainImportingEngine.Library.FindSection(Settings.FilmsLibrary, true);
            else
            {
                StatusForm.statusForm.TrayIcon.ShowBalloonTip(5000, "Films section was not set", "You have chosen to import or update the Films section but you have not specified such a section. Importing or updating of this section will not be performed.", ToolTipIcon.Warning);
                Thread.Sleep(5000);
            }

        }


    }//endof class


}//endof namespace