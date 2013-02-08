using System;
using System.Collections.Generic;
using System.IO;
using MediaFairy.Code;
using MediaFairy.ImportingEngine;
using MediaFairy.SingleItem_Updaters;
using MeediOS;
using TVDBLibrary;

namespace MediaFairy.SingleItemUpdaters
{


    class SingleTvEpisodeItemUpdater
    {



        internal static bool UpdateTvEpisodeItem
            (IMLSection section, ref bool fileServerChecked, 
             ref bool fileServerIsOnline,
             ConnectionResult connectionresult,
             string mdfSettingsb, IBaseSystem iBaseSystem,
             IEnumerable<string> combinedSceneTags, 
             TVDBLib tvdb, int totalItems, int id,
             ref int currentItem)
        {



            #region item variables

            IMLItem item = MediaSectionsAllocator.TvEpisodesSection.FindItemByID(id);
            string seriesID = string.Empty;

            //TODO: This var should be removed!
            const bool isMultipart = false;
            
            string location = item.Location;

            //TODO: This var must be removed!
            string[] multipart = new string[] {};
            
            bool isUNC = false;

            DirectoryInfo root;

            var parentDir = GetParentAndRootDirectories(location, out root);

            SingleTvEpisodeItemUpdaterHelpers.UpdateProgressTexts(totalItems, currentItem, item);

            #endregion

            try
            {


                #region Skip Not Listed item
                if (Helpers.GetTagValueFromItem(item, "NotListed") == "true")
                    return true;

                if (Helpers.UserCancels(MainImportingEngine.SpecialStatus, item))
                    return false;
                #endregion



                FileServerChecker.CheckFileServer
                    (Settings.WantFileserverDiagnostics,
                    location, ref isUNC, ref fileServerChecked,
                    root.FullName, ref fileServerIsOnline);



                if (Helpers.UserCancels(MainImportingEngine.SpecialStatus, item))
                    return false;

                #region Delete Missing Item

                if (MissingItemRemover.DeleteMissingItem(Settings.DeleteMissing, isUNC, fileServerIsOnline, location,
                                                         section, item))
                    return true;

                if (Helpers.UserCancels(MainImportingEngine.SpecialStatus, item))
                    return false;
                
                #endregion


                if (EpisodeFileMetadataExtractor.ExtractMetadataFromEpisodeFilename
                        (item, location, section, MediaSectionsAllocator.TvEpisodesSection,
                        true, combinedSceneTags))
                    return true;
                

                if (Helpers.UserCancels(MainImportingEngine.SpecialStatus, item))
                    return false;



                #region Calculate Video Hash

                TvSeriesOperations.CalculateVideoHash(item, fileServerIsOnline, isUNC, location);
                
                if (Helpers.UserCancels(MainImportingEngine.SpecialStatus, item))
                    return false;

                #endregion


                #region Perform Online Tasks
                
                
                if (!SingleTvEpisodeItemUpdaterHelpers.PerfrormOnlineTasks
                    (fileServerChecked, fileServerIsOnline, 
                    connectionresult, mdfSettingsb, iBaseSystem, tvdb,
                    currentItem, location,
                    isMultipart, multipart, item, isUNC,
                    seriesID, parentDir))
                    return false;

                if (Helpers.UserCancels(MainImportingEngine.SpecialStatus, item))
                    return false;

                #endregion



                SingleTvEpisodeItemUpdaterHelpers.SetProccesedFlag(item);

                SingleTvEpisodeItemUpdaterHelpers.ProceedToNextItem(ref currentItem, item);
            

            }
            catch (Exception e)
            {     
                SingleTvEpisodeItemUpdaterHelpers.LogException(e, item);
            }


            return true;
    
         
        
        }

        private static string GetParentAndRootDirectories(string location, out DirectoryInfo root)
        {
            var fi = new FileInfo(location);
            string parentDir = fi.DirectoryName;
            DirectoryInfo parent = fi.Directory;

            root = parent != null 
                ? parent.Root : null;

           
            return parentDir;

        }
    
    
    }



}
