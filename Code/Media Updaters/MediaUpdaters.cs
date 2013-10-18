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
using System.Collections.Generic;
using System.Windows.Forms;
using MeediFier.IMDb;
using MeediFier.ImportingEngine;
using MeediFier.MediaSectionUpdaters;
using MeediOS;



#if USE_MEEDIO
using Meedio;
#elif USE_MEEDIOS

#endif



namespace MeediFier
{


    internal class UpdateMediaSectionsParams
    {


        public UpdateMediaSectionsParams(IMLSection section,
            string mdfSettingsa, IBaseSystem iBaseSystem, 
            string mdfSettingsb, string mdfSettingsc, 
            IEnumerable<string> combinedSceneTags)
        {
            Section = section;
            MdfSettingsa = mdfSettingsa;
            Ibs = iBaseSystem;
            MdfSettingsb = mdfSettingsb;
            MdfSettingsc = mdfSettingsc;
            CombinedSceneTags = combinedSceneTags;
        }

        public IMLSection Section { get; private set; }

        public string MdfSettingsa { get; private set; }

        public IBaseSystem Ibs { get; private set; }

        public string MdfSettingsb { get; private set; }

        public string MdfSettingsc { get; private set; }

        public IEnumerable<string> CombinedSceneTags 
        { get; private set; }

    }



    internal static class MediaUpdaters
    {




        public static bool UpdateMediaSections
            (UpdateMediaSectionsParams updateMediaSectionsParams, 
            ref ConnectionResult connectionresult)
        {

            string pluginpath = Debugger.GetPluginPath();

            var imdbOp = new IMDbOperations();
            SetIMDbInternalTitleMatchingEngineProperties(imdbOp);

            bool fileServerIsOnline = true;
            bool fileServerChecked = false;

            MainImportingEngine.CurrentProgress = 0;
            MainImportingEngine.GeneralStatus = "Starting media updating process...";
            MainImportingEngine.SpecialStatus = "";


            try
            {
  



                if (
                    !MoviesSectionUpdater.UpdateMoviesSection
                    (pluginpath, ref fileServerChecked, ref fileServerIsOnline,
                    ref connectionresult, imdbOp,
                    updateMediaSectionsParams.MdfSettingsa,
                    updateMediaSectionsParams.Ibs,
                    updateMediaSectionsParams.CombinedSceneTags))
                    return true;

                if (
                    !TvSectionUpdater.UpdateTvSections
                    (updateMediaSectionsParams.Section,
                    ref fileServerChecked, ref fileServerIsOnline,
                    connectionresult, updateMediaSectionsParams.MdfSettingsb,
                    updateMediaSectionsParams.Ibs,
                    updateMediaSectionsParams.CombinedSceneTags))
                    return true;


                if (
                    !MusicSectionUpdater.UpdateMusicSection
                    (ref fileServerChecked, ref fileServerIsOnline,
                    connectionresult, updateMediaSectionsParams.MdfSettingsc,
                    updateMediaSectionsParams.Ibs))
                    return true;


            }
            catch (Exception e)
            {

                StatusForm.statusForm.TrayIcon.ShowBalloonTip(10000,
                        "Error updating Media Sections",
                        "The main Media Updating process returned an unexpected error." +
                        Environment.NewLine +
                        "Please refer to debug.log file (in the plugin's directory)" +
                        " for debugging information.",
                        ToolTipIcon.Error);


                Debugger.LogMessageToFile(Environment.NewLine +
                        "An unexpected error occured in DoImport() method. The error was: " +
                        Environment.NewLine + e + Environment.NewLine);


                MainImportingEngine.CurrentProgress = 100;

                MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress,
                        "The import process terminated unexpectidly due to an error.");


            }


            return true;
            
            
       
        }



        private static void SetIMDbInternalTitleMatchingEngineProperties(IMDbOperations imdbOp)
        {

            if (Settings.AssistType == "When no exact match found")
                imdbOp._assistType = IMDbOperations.AssistType.NoPerfectMatch;

            if (Settings.AssistType == "Never")
                imdbOp._assistType = IMDbOperations.AssistType.Never;

            if (Settings.AssistType == "Skip identifying unknown films")
                imdbOp._assistType = IMDbOperations.AssistType.SkipUnknownItems;

            if (Settings.AssistType == "Always")
                imdbOp._assistType = IMDbOperations.AssistType.Always;

            imdbOp.UseFirst = Settings.UseFirst;
            imdbOp.UseLatest = Settings.UseLatest;
            imdbOp.UseMostPopular = Settings.UseMostPopular;


        }


    }



}



