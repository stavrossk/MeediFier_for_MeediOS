using System;
using System.Collections.Generic;
using MediaFairy.ImportingEngine;
using MediaFairy.SingleItemUpdaters;
using MeediOS;
using TVDBLibrary;

namespace MediaFairy.MediaSectionUpdaters
{

    class TvSectionUpdater
    {



        internal static bool UpdateTvSections
            (IMLSection section, ref bool fileServerChecked,
            ref bool fileServerIsOnline,
            ConnectionResult connectionresult, string mdfSettingsb,
            IBaseSystem ibs, IEnumerable<string> combinedSceneTags)
        {

            TvSectionUpdaterHelpers.CheckIfTvShowsSectionIsSet();


            if (!Settings.EnableTvShowsSectionUpdater ||
                MediaSectionsAllocator.TvEpisodesSection.ItemCount == 0)
                return true;


            #region TV Shows Section varibales

            var currentItem = 1;

            var tvEpisodeItems = 
                MediaSectionsAllocator.TvEpisodesSection.GetAllItemIDs();
                
            var totalItems = tvEpisodeItems.Length;

            MainImportingEngine.CurrentProgress = 0;
            MainImportingEngine.GeneralStatus = "Updating Tv Section";
            MainImportingEngine.SpecialStatus = "";

            if (Settings.SleepValue > 3000) Settings.SleepValue = 3000;

            #endregion



                

            TVDBLib tvdb = TvSectionUpdaterHelpers
                .ConnectToTVDb(connectionresult);

            if (tvdb == null)
                return true;


            TvSectionUpdaterHelpers
                .BeginUpdatingSections();


            foreach (int id in tvEpisodeItems)
            {

                if (!SingleTvEpisodeItemUpdater.UpdateTvEpisodeItem
                    (section, ref fileServerChecked, ref fileServerIsOnline, 
                    connectionresult, mdfSettingsb,ibs, combinedSceneTags,
                    tvdb, totalItems, id, ref currentItem))
                    return false;

            }


            TvSectionUpdaterHelpers.EndUpdatingSections();


            TvSectionUpdaterHelpers.PerformPostUpdatingOperations();


            return true;


        }
    
    
    
    }



}
