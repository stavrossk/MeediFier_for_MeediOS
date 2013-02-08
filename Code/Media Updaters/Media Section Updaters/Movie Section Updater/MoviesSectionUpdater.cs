using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using MediaFairy.IMDb;
using MediaFairy.ImportingEngine;
using MediaFairy.SingleItemUpdaters;
using MeediOS;




namespace MediaFairy.MediaSectionUpdaters
{



    class MoviesSectionUpdater
    {




        internal static bool UpdateMoviesSection
            (string pluginpath, ref bool FileServerChecked,
             ref bool FileServerIsOnline,
             ref ConnectionResult connectionresult,
             IMDbOperations imdbOP, string MDFSettingsa, 
            IBaseSystem IBS, IEnumerable<string> combinedSceneTags)
        {



            if (String.IsNullOrEmpty(Settings.FilmsLibrary))
                Settings.EnableMoviesSectionUpdater = false;




            if (Settings.EnableMoviesSectionUpdater)
            {


                Debugger.LogMessageToFile
                    (Environment.NewLine 
                    + Environment.NewLine 
                    + "Film Updater is enabled.");


                if (MediaSectionsAllocator.MoviesSection.ItemCount != 0)
                {
                    #region Pre-Updating Operations

                    Debugger.LogMessageToFile("Initializing section variables...");

                    #region Movies Section varibales

                    IMLItemList allFilmItems;
                    int TotalItems = MediaSectionsAllocator.MoviesSection.ItemCount;
                    int CurrentItem = 1;
                    MainImportingEngine.CurrentProgress = 0;
                    MainImportingEngine.GeneralStatus = "Updating Movies Section";
                    MainImportingEngine.SpecialStatus = "";


                    if (Settings.SleepValue > 3000) Settings.SleepValue = 3000;

                    #endregion

                    #region Decide selective updating

                    bool SelectiveUpdatingEnabled = false;
                    Debugger.LogMessageToFile("Scanning section for Selective Updating request...");

                    try
                    {
                        foreach (int id in MediaSectionsAllocator.MoviesSection.GetAllItemIDs())
                        {
                            IMLItem Item = MediaSectionsAllocator.MoviesSection.FindItemByID(id);
                            if (Item.Tags["SelectiveUpdating"] != null &&
                                Helpers.GetTagValueFromItem(Item, "SelectiveUpdating") != "")
                                SelectiveUpdatingEnabled = true;
                        }
                    }
                    catch (NullReferenceException)
                    {
                        StatusForm.statusForm.TrayIcon.ShowBalloonTip(10000, "Films section is corrupt.",
                                                                      "MediaFairy was unable to update your Films media section because it's database file is corrupt. Please restore it using an available backup.",
                                                                      ToolTipIcon.Error);
                        Debugger.LogMessageToFile("The media section " + Settings.FilmsLibrary +
                                                  " is corrupt. This section will not be updated.");
                        return false;
                    }

                    #endregion






                    //FilmReleasesDownloader
                    //    .DownloadFilmReleases
                    //    (pluginpath, connectionresult,
                    //    combinedSceneTags,
                    //    SelectiveUpdatingEnabled);






                    #region Cache films section for the chain linking

                    if (!SelectiveUpdatingEnabled && Settings.FilmItemChainLinkerIsEnabled)
                    {
                        allFilmItems = MediaSectionsAllocator.MoviesSection.SearchAll("*");
                    }
                    else allFilmItems = null;

                    #endregion

                    #region Connect to OSdb

                    //connectionresult = Helpers.CheckInternetConnectionAndConnectToOSDb();

                    #endregion

                    #endregion

                    #region Main Updating Loop

                    Debugger.LogMessageToFile("Beginning to update Films section...");
                    MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress, "Preparing to update Movies section...");
                    Thread.Sleep(1000);
                    MediaSectionsAllocator.MoviesSection.BeginUpdate();



                    foreach (int id in MediaSectionsAllocator.MoviesSection.GetAllItemIDs())
                    {
                        if (
                            !SingleMovieItemUpdater.UpdateFilmItem
                            (id, MediaSectionsAllocator.MoviesSection,
                            allFilmItems, ref FileServerChecked,
                            ref FileServerIsOnline, connectionresult, imdbOP,
                            pluginpath, ref CurrentItem, TotalItems,
                            MDFSettingsa, IBS, combinedSceneTags))
                            return false;
                    }



                    MediaSectionsAllocator.MoviesSection.EndUpdate();
                    Debugger.LogMessageToFile("Completing updating of Films section...");

                    #endregion

                    #region Post-Updating Operations

                    if (Helpers.UserCancels
                        (MainImportingEngine
                        .SpecialStatus, null))
                        return false;

                    LibraryCleaners.CleanupSection
                        (Settings.CleanEmptyFields,
                         Settings.CleanSelectedFields,
                         Settings.SelectedFieldsToClean,
                         MediaSectionsAllocator.MoviesSection);


                    if (Helpers.UserCancels
                        (MainImportingEngine
                        .SpecialStatus, null))
                        return false;


                    MeePopupProvider
                        .ImportMeePopupProviderField
                        (MediaSectionsAllocator.MoviesSection);


                    //if (Settings.EnableXmlBackupFilms)
                    //    MediaLibrarySynchronizerBackup
                    //        .LibrarySynchronizerBackupMediaSection
                    //        (MediaSectionsAllocator.MoviesSection,
                    //        Settings.XmlBackupsDirectory);

                    #endregion
                }




            }



            return true;
        }



    }



}
