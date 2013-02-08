



using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using MediaFairy.IMDb;
using MediaFairy.Properties;
using ToolBox;
using MeediOS;
#if USE_MEEDIO
using Meedio;
#elif USE_MEEDIOS

#endif


namespace MediaFairy
{
    class DiscIdentifier
    {
        const string PluginName = "MediaFairy";
        const string PluginType = "import";
        string _hexId = String.Empty;
        string _seriesName = String.Empty;
        bool _tvShow;

        string _dvdTitle = String.Empty;
        //string DVDDirector = String.Empty;
        string _dvdDirectors = String.Empty;
        //string DVDFirstActor = String.Empty;
        string _dvdActors = String.Empty;
        string _movieImdbId = String.Empty;

        string _brTitle3 = String.Empty;

        //ENABLE DISABLE ANYDVD
        private static void AnyDvd(bool enable)
        {
            //CHECK IF ANYDVD IS RUNNING
            const string pname = "AnyDVD";
            var procs = Process.GetProcesses();

            foreach (var p in procs.Where(p => p.ProcessName.StartsWith(pname)))
            {
                try
                {
                    var curDir = ToolBox.MeedioUtils.GetPluginPath(PluginName, PluginType);
                    var anyToolPath = (curDir + "\\Anytool.exe");

                    string status = enable ? ("-e") : ("-d");

                    var dAnyDvd = new Process
                                      {
                                          StartInfo =
                                              {
                                                  FileName = anyToolPath,
                                                  Arguments = status,
                                                  CreateNoWindow = true,
                                                  UseShellExecute = false
                                              }
                                      };
                    dAnyDvd.Start();
                    dAnyDvd.WaitForExit();

                }
                catch (Exception)
                {
                    Debugger.LogMessageToFile("Error enabling or disabling AnyDVD: ");
                    
                }
            }
        }


        //RUN MAIN CODE ON NEW THREAD        
        // ReSharper disable UnusedParameter.Global
        public void IdentifyDisc(IMLSection moviesSection, IMLSection tvSeriesSection)
        // ReSharper restore UnusedParameter.Global
        {

                //string PluginDir = Debugger.GetPluginPath();
                //string DataDirectory = ToolBox.MeedioUtils.GetPluginDataDir(PluginName, PluginType);


                ///////////////////////////////////////PROCESS DISC////////////////////////////////

                //GET DRIVE LETTER FROM DISK INSERT NOTIFICATION
                string drive = Settings.DvdDrive;
                string d2 = drive + ":";  //GET DRIVE LETTER PLUS ":" FOR DISK EJECT                    
                Importer.ThisProgress.Progress(Importer.CurrentProgress, "Reading drive " + drive + "...");


            
                IdentifyDvdDisc(moviesSection, drive, d2);


                IdentifyBluRayDisc(drive, moviesSection, d2);
               




                Helpers.UpdateProgress(Importer.GeneralStatus, "Offline media importing complete.", null);
                return;


        }



        private void IdentifyDvdDisc(IMLSection moviesSection, string drive, string d2)
        {
            if (!Settings.DVDMovieOptions && !Settings.DVDTVOptions) return;

            if (!Directory.Exists(drive + ":\\VIDEO_TS")) return;

            //IF DISC IS A REGULAR DVD RUN THE FOLLOWING CODE:
            AnyDvd(false); //Disable AnyDVD

            #region Get DVD disc ID
            try
            {
                Importer.ThisProgress.Progress(Importer.CurrentProgress, "Getting Disc ID...");
                _hexId = DVDUtils.GetDVDID(drive);
            }
            catch (Exception e)
            {
                Debugger.LogMessageToFile("[MAIN CODE] - Error getting DVDID: " + e);
            }
            #endregion

            AnyDvd(true); //Re-enable AnyDVD

            if (DVDUtils.IsAnyDVDRunning())
            {
                Debugger.LogMessageToFile("[MAIN CODE] - Waiting for AnyDVD to read disc...");
                Thread.Sleep(5000); //wait for anydvd to process disc
            }

            #region Check if disc is TV Series
            if (Settings.DVDTVOptions)
            {
                Importer.ThisProgress.Progress(Importer.CurrentProgress, "Recognizing Series...");
                //GET DVD NAME AND EPISODE NAME FOR TVDB SEARCH
                try
                {
                    _seriesName = DVDWebUtils.DVDTitle(_hexId);
                    if (_seriesName.Contains(":"))
                    {
                        int pos = _seriesName.IndexOf(":");
                        String s2 = _seriesName.Remove(pos);
                        _seriesName = s2.Trim();
                    }
                }
                catch (Exception e)
                {
                    //TODO: Handle exception
                }
                //DETERMINE IF DVD MIGHT CONTAIN TV EPISODES
                _tvShow = DVDUtils.IsTVSeries(_seriesName);
            }
            #endregion


            #region TV Series
            /////////////////////////DVD TV CODE//////////////////////
            //if (tvShow)
            //{
            //    String TVServerDirectory = "";
            //    //CONFIRM DVD IS TV SERIES WITH DIALOG BOX      
            //    string Description = (SeriesName + " has been identified as a disc that may contain television episodes.  Please confirm if this is a TV or Movie DVD.");
            //    //ToolBox.MeedioUtils.ShowDialogBox(MeedioSystem, Message, "Please confirm disc type", Description, "yes/no", "TV Series", "Movie");
            //    DialogResult result = MessageBox.Show(Description, "Please confirm disc type", MessageBoxButtons.YesNo);

            //    if (result == DialogResult.Yes)
            //        tvShow = true;
            //    else tvShow = false;

            //    if (tvShow == true)
            //    {
            //        //SHOW ON SCREEN KEYBOARD AND CONFIRM SEASON NUMBER
            //        string SeasonNumber = ToolBox.DVDUtils.GetSeasonNumber(D2);
            //        //ToolBox.MeedioUtils.ShowOnScreenKeyboard(MeedioSystem, Message, "Please confirm season number", SeasonNumber);
            //        SeasonNumber = InputBox.Show("Plase confirm season number", "Confirm season", SeasonNumber);
            //        //mre.WaitOne(); //Pause and wait for input
            //        //mre.Reset();   //Reset thread wait  
            //        string SeasonNumforFileName = DVDTitle.Trim().PadLeft(2, '0');
            //        SeasonNumber = "Season " + DVDTitle.Trim().PadLeft(2, '0');

            //        //SHOW ON SCREEN KEYBOARD AND CONFIRM DISC NUMBER
            //        string DiscNumber = ToolBox.DVDUtils.GetSeasonDiscNumber(D2);
            //        //ToolBox.MeedioUtils.ShowOnScreenKeyboard(MeedioSystem, Message, "Please confirm disc number", DiscNumber);
            //        DiscNumber = InputBox.Show("Plase confirm disc number", "Confirm disc number", DiscNumber);
            //        //mre.WaitOne(); //Pause and wait for input
            //        //mre.Reset();   //Reset thread wait
            //        string DiscNumberforFileName = DVDTitle.Trim().PadLeft(2, '0');
            //        DiscNumber = DVDTitle.Trim();

            //        string SeriesforFileName = ToolBox.StringFunctions.replaceIllegalCharacters(SeriesName, " ");
            //        string FileName = SeriesforFileName + " S" + SeasonNumforFileName + "D" + DiscNumberforFileName;
            //        Debugger.LogMessageToFile("[MAIN CODE] - Series name: " + SeriesName);

            //        //CREATE DIRECTORY ON SERVER
            //        SeriesName = (SeriesName + "\\" + SeasonNumber).Trim();
            //        TVServerDirectory = CreateDirectory(SeriesName, "", "", "TV");
            //        Debugger.LogMessageToFile("[MAIN CODE] - TV server directory: " + TVServerDirectory);


            //    }

            //    //EJECT DVD 
            //    ToolBox.DVDUtils.MCICDROM.Eject(D2);



            //}
            #endregion


                       
            #region Film Identifier
            if (_tvShow == false)
            {
                #region Get DVD film's primary keys
                try
                {
                    Importer.ThisProgress.Progress(Importer.CurrentProgress, "Trying to identify film by Disc ID...");
                    _dvdTitle = DVDWebUtils.DVDTitle(_hexId);
                    _dvdDirectors = DVDWebUtils.DVDDirector(_hexId);
                    _dvdActors = DVDWebUtils.DVDFirstActor(_hexId);

                    if (!String.IsNullOrEmpty(_dvdTitle))
                        MessageBox.Show(Resources.DiscIdentifier_IdentifyDvdDisc_DVD_identified_as_ + _dvdTitle);
                
                }
                catch (Exception e)
                {
                    Debugger.LogMessageToFile("Error retrieving online data for this DVD film: " + e.ToString());
                }
                #endregion

                #region If Title was not recognized, prompt for Title
                bool manualTitle = false;

                if (String.IsNullOrEmpty(_dvdTitle) && Settings.PromptForOfflineMediaName)
                {

                    //ToolBox.MeedioUtils.ShowOnScreenKeyboard(MeedioSystem, Message, "Please enter movie title", "", Log, swWriteLog);
                    _dvdTitle = InputBox.Show("Please enter the Title of this film: ", "Enter film Title");

                    if (!String.IsNullOrEmpty(_dvdTitle))
                        manualTitle = true;

                }
                #endregion


                #region Match MS Director and Actor to IMDb ID
                try
                {
                    Importer.ThisProgress.Progress(Importer.CurrentProgress, "Getting film IMDb ID...");
                    _movieImdbId = "";
                    _movieImdbId = GetIMDBID(_dvdTitle, _dvdDirectors, _dvdActors, "", _hexId);
                    Debugger.LogMessageToFile("IMDb ID: " + _movieImdbId);
                }
                catch (Exception e)
                {
                    Debugger.LogMessageToFile("Error matching MS Director and Actor to IMDb ID: " + e.ToString());
                }

                #endregion

                #region GET IMDB TITLE IF TITLE IS MANUALLY ENTERED
                if (!String.IsNullOrEmpty(_movieImdbId) && manualTitle)
                {
                    Importer.ThisProgress.Progress(Importer.CurrentProgress, "Getting IMDb film Title...");
                    _dvdTitle = GetIMDBTITLE(_movieImdbId);
                    Debugger.LogMessageToFile("Manually entered name adjusted to IMDb title: " + _dvdTitle);
                }
                #endregion


                #region Create new library entry for DVD
                Importer.ThisProgress.Progress(Importer.CurrentProgress, "Adding disc to films section...");
                IMLItem Item = moviesSection.AddNewItem(_dvdTitle, drive + ":\\VIDEO_TS\\video_ts.ifo");
                Item.ExternalID = drive + ":\\VIDEO_TS\\video_ts.ifo";
                Item.Tags["DiscID"] = _hexId;
                Item.Tags["IMDbID"] = _movieImdbId;
                Item.SaveTags();
                #endregion


            }
            #endregion


            EjectDvd(d2, drive);
        }


        private void IdentifyBluRayDisc(string drive, IMLSection moviesSection, string d2)
        {
            if (!Settings.BRMovieOptions && !Settings.BRTVOptions) return;

            if (!Directory.Exists(drive + ":\\BDMV")) return;


            #region Get Movie name
            //IF BLURAY, GET MOVIE NAME FROM META XML FILE ON DISC
            _brTitle3 = DVDUtils.GetBluRayTitle(drive);

            if (!String.IsNullOrEmpty(_brTitle3))
                Helpers.UpdateProgress(Importer.GeneralStatus, "Blu-Ray identified as " + _brTitle3, null);

            //IF NAME IS NOT FOUND, PROMPT USER FOR MOVIE NAME
            bool manualTitle = false;
            if (String.IsNullOrEmpty(_brTitle3) && Settings.PromptForOfflineMediaName)
            {
                _brTitle3 = InputBox.Show("Please enter the film's title:", "MediaFairy offline media importer");

                if (!String.IsNullOrEmpty(_brTitle3))
                    manualTitle = true;

            }
            #endregion


            #region Tv Series
            /////////////////////////BLU-RAY TV CODE//////////////////////
            //if (Importer.BRTVOptions)
            //{
            //    //DETERMINE IF BLU-RAY MIGHT CONTAIN TV EPISODES
            //    String TVServerDirectory = "";
            //    SeriesName = BRTitle3;
            //    tvShow = ToolBox.DVDUtils.IsTVSeries(SeriesName);

            //    if (tvShow == true)
            //    {
            //        //CONFIRM DVD IS TV SERIES WITH DIALOG BOX      
            //        string Description = (SeriesName + " has been identified as a disc that may contain television episodes.  Please confirm if this is a TV or Movie DVD.");
            //        //ToolBox.MeedioUtils.ShowDialogBox(MeedioSystem, Message, "Please confirm disc type", Description, "yes/no", "TV Series", "Movie", Log, swWriteLog);
            //        DialogResult result =  MessageBox.Show(Description, "Please confirm disc type", MessageBoxButtons.YesNo);

            //        //SHOW ON SCREEN KEYBOARD AND CONFIRM SEASON NUMBER
            //        string SeasonNumber = ToolBox.DVDUtils.GetSeasonNumber(D2);
            //        ToolBox.MeedioUtils.ShowOnScreenKeyboard(MeedioSystem, Message, "Please confirm season number", SeasonNumber, Log, swWriteLog);
            //        SeasonNumber = InputBox.Show("Please confirm season number", "Confirm season number", SeasonNumber);
            //        //mre.WaitOne(); //Pause and wait
            //        //mre.Reset();   //Reset thread wait  
            //        string SeasonNumforFileName = BRTitle3.Trim().PadLeft(2, '0');
            //        SeasonNumber = "Season " + BRTitle3.Trim().PadLeft(2, '0');

            //        //SHOW ON SCREEN KEYBOARD AND CONFIRM DISC NUMBER
            //        string DiscNumber = ToolBox.DVDUtils.GetSeasonDiscNumber(D2);
            //        ToolBox.MeedioUtils.ShowOnScreenKeyboard(MeedioSystem, Message, "Please confirm disc number", DiscNumber, Log, swWriteLog);
            //        mre.WaitOne(); //Pause and wait
            //        mre.Reset();   //Reset thread wait
            //        string DiscNumberforFileName = BRTitle3.Trim().PadLeft(2, '0');
            //        DiscNumber = BRTitle3.Trim();

            //        string SeriesforFileName = ToolBox.StringFunctions.replaceIllegalCharacters(SeriesName, " ");
            //        string FileName = SeriesforFileName + " S" + SeasonNumforFileName + "D" + DiscNumberforFileName;
            //        Debugger.LogMessageToFile("[MAIN CODE] - Series name: " + SeriesName);

            //        //CREATE DIRECTORY ON SERVER
            //        SeriesName = (SeriesName + "\\" + SeasonNumber).Trim();
            //        TVServerDirectory = CreateDirectory(SeriesName, "", "", "TV");
            //        Debugger.LogMessageToFile("[MAIN CODE] - TV server directory: " + TVServerDirectory);



            //        #region EJECT DVD
            //        // EJECT DVD
            //        if (Importer.EjectDvd)
            //        {
            //            try
            //            {
            //                ToolBox.DVDUtils.MCICDROM.Eject(D2);
            //            }
            //            catch (Exception e)
            //            {
            //                Debugger.LogMessageToFile("[MAIN CODE] - Error on OnMessage: " + e.ToString());
            //            }
            //        }
            //        #endregion



            //    }
            //}
            #endregion


            #region Movies
            if (_tvShow == false)
            {
                #region GET YEAR FROM BRDISC FILE CREATION DATE
                String brPlaylistDirectory = (drive + ":\\BDMV\\PLAYLIST");
                String[] brPlaylistFiles = Directory.GetFiles(brPlaylistDirectory, "*.MPLS", SearchOption.AllDirectories);
                String brYear = string.Empty;
                foreach (string f in brPlaylistFiles)
                {
                    try
                    {
                        DateTime BRDate = File.GetCreationTime(Path.GetFullPath(f));
                        int intYear = BRDate.Year;
                        brYear = Convert.ToString(intYear);
                        Debugger.LogMessageToFile("Year from file: " + brYear);
                        break;
                    }
                    catch (Exception)
                    {
                        //TODO: Handle exception
                    }

                }
                #endregion

                #region GET IMDb BASED ON NAME & YEAR
                try
                {
                    _movieImdbId = "";
                    _movieImdbId = GetIMDBID(_brTitle3, "", "", brYear, "");
                    Debugger.LogMessageToFile("IMDb ID: " + _movieImdbId);

                }
                catch (Exception e)
                {
                    Debugger.LogMessageToFile("An error occurred in GetIMDBID function: " + e.ToString());
                }
                #endregion

                //GET IMDB TITLE IF TITLE WAS MANUALLY ENTERED
                if (_movieImdbId != "" && manualTitle == true)
                {
                    _brTitle3 = GetIMDBTITLE(_movieImdbId);
                    Debugger.LogMessageToFile("Manually entered name adjusted to IMDb title: " + _brTitle3);
                                
                }


                #region Create new library entry for Bluray
                IMLItem Item = moviesSection.AddNewItem(_brTitle3,"");
                Item.ExternalID = drive + "";
                Item.Tags["DiscID"] = _hexId;
                Item.Tags["IMDbID"] = _movieImdbId;
                Item.SaveTags();
                #endregion


    
            }
            #endregion


            EjectDvd(d2, drive);
        }


        //GET IMDBID
        private static string GetIMDBID(string movieName, string amgDirector, string amgActor, string year, string dvdid)
        {
            var id = string.Empty;
            var newSearch = new IMDbLib("USA");  //TODO: Make this a module option
            IIMDbMovie imdblist;
            var titleList = new List<String>();
            var movieList = IMDbConventionalFilmSearchEngine.SearchForTitleReturnResults(movieName);
            
            
            try
            {

                for (var i = 0; i < movieList.Count; i++)
                {
                    if (i >= movieList.Count) continue;

                    titleList.Add(movieList[i].IMDb_ID);
                    Debugger.LogMessageToFile("IMDb Search Results: " + movieList[i].Title.ToString());
                
                }

                if (String.IsNullOrEmpty(year))
                {
                    year = DVDWebUtils.GetReleaseYear(dvdid);

                    if (titleList.Count > 1)
                    {
                        foreach (string t in titleList)
                        {
                            imdblist = newSearch.GetMovieDetails(t,false);
                            String iDir = imdblist.People.GetDirectorString();
                            Debugger.LogMessageToFile("Directors: " + iDir);

                            if (!iDir.ToLower().Contains(amgDirector.ToLower())) continue;

                            Debugger.LogMessageToFile(imdblist.Title + ": " + "Directors Matched");
                            String iActor = imdblist.People.GetActorsString(false); //true to get roles
                            Debugger.LogMessageToFile("Actors: " + iActor);

                            if (!iActor.ToLower().Contains(amgActor.ToLower())) continue;

                            Debugger.LogMessageToFile(imdblist.Title + ": " + "Actors Matched");
                            id = imdblist.IMDb_ID;
                            break;

                        }//endof foreach

                    }
                    else
                    {
                        foreach (string t in titleList)
                        {

                            imdblist = newSearch.GetMovieDetails(t,false);

                            if (imdblist.Title.ToLower() == movieName.ToLower() ||
                                imdblist.Title.ToLower().Contains(movieName.ToLower()) ||
                                movieName.ToLower().Contains(imdblist.Title.ToLower()))
                                id = t;

                        }
                    }

                }


                if (!String.IsNullOrEmpty(year) && String.IsNullOrEmpty(id))
                {

                    foreach (string t in titleList)
                    {
                        imdblist = newSearch.GetMovieDetails(t,false);
                        String sYear2 = imdblist.Year;
                        String sYear = sYear2.Replace("/", "").Trim();
                        int iYear = Convert.ToInt16(year);
                        int iYearMinusOne = iYear - 1;
                        int iYearMinusTwo = iYear - 2;
                        String sYearMinusTwo = Convert.ToString(iYearMinusTwo);
                        String sYearMinusOne = Convert.ToString(iYearMinusOne);
                        Debugger.LogMessageToFile("Search Year: " + year);
                        Debugger.LogMessageToFile("Search Year: " + sYearMinusOne);
                        Debugger.LogMessageToFile("Search Year: " + sYearMinusTwo);
                        Debugger.LogMessageToFile("IMdb Year: " + sYear);

                        if ((imdblist.Title.ToLower() != movieName.ToLower() &&
                             !imdblist.Title.ToLower().Contains(movieName.ToLower())) &&
                            !movieName.ToLower().Contains(imdblist.Title.ToLower())) continue;

                        if ((sYear != year && sYear != sYearMinusOne) && sYear != sYearMinusTwo) continue;

                        Debugger.LogMessageToFile(imdblist.Title + ": " + "Years Matched");
                        id = imdblist.IMDb_ID;
                        break;

                    }
                }
            }
            catch (Exception e)
            {
                Debugger.LogMessageToFile("Get IMDb 2 - Error on OnMessage: " + e.ToString());
            }
            return id.ToString();
        }


        //GET IMDB TITLE
        private static string GetIMDBTITLE(string IMDBID)
        {
            IMDbLib newSearch = new IMDbLib("USA");  //Make this a module option
            IIMDbMovie MovieSearch = newSearch.GetMovieDetails(IMDBID,true);
            return MovieSearch.Title;
        }


        private static void EjectDvd(string driveLetter, string drive)
        {

            if (!Settings.EjectDvd)
                return;

                try
                {
                    DVDUtils.MCICDROM.Eject(driveLetter);
                    Debugger.LogMessageToFile("Ejecting DVD: " + drive);
                }
                catch (Exception e)
                {
                    Debugger.LogMessageToFile("Error ejecting DVD: " + e);
                }


        }




    }
}
