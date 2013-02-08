using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MediaInfoLib;
using System.Windows.Forms;
using System.Threading;
//using System.Net;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using MediaFairy.Code;
using CustomProperties;
using CustomProperties.PropertiesData;

#if USE_MEEDIO
using Meedio;
#elif USE_MEEDIOS
using MeediOS;
#endif




#region Structures

public struct Subtitle
{

    //string IDSubMovieFile;
    //string MovieHash;
    //string MovieByteSize;
    //string MovieTimeMS;
    //string IDSubtitleFile;
    //string SubFileName;
    //string SubActualCD;
    //string SubSize;
    //string SubHash;
    //string IDSubtitle;
    //string UserID;
    //string SubLanguageID;
    //string SubFormat;
    //string SubSumCD;
    //string SubAuthorComment;
    //string SubAddDate;
    //string SubBad;
    //string SubRating;
    //string SubDownloadsCnt;
    //string MovieReleaseName;
    //string IDMovie;
    //string IDMovieImdb;
    //string MovieName;
    //string MovieNameEng;
    //string MovieYear;
    //string MovieImdbRating;
    //string UserNickName;
    //string ISO639;
    //string LanguageName;
    //string SubDownloadLink;
    //string ZipDownloadLink;
}

public struct LoginResult
{
    public string token;
    public string status;
    public double seconds;
}

public struct SearchParams
{
    public string sublanguageid;
    //public string moviehash;
    //public double moviesize;
    public string imdbid;
}

public struct ConnectionResult
{
    public bool CanWorkOnline;
    public bool OsIsOnline;
    public LoginResult loginresult;
}

public struct ImportProgress
{
    public IMLImportProgress Progress;
    public int CurrentProgress;
    public int ProgressStep;

    public string GeneralOperation;
    public string SpecialOperation;
}

public struct Movie
{
    public string Name;
    public string Year;
    public string ID;
    public string Hash;
    public string imdbid;
    public string Tagline;
    public string Duration;
    public string Plot;
    public string Directors;
    public string Cast;
    public string Writers;
    public string Genres;

    public string Trivia;
    public string Goofs;

    public string Cover;

}

public struct CheckMovieHashOutput
{
    //public Movie[] data;
    //public Movie[] data;
    public Movie data;
    public double seconds;
}

public struct SearchSubtitlesOutput
{
    //Subtitle[] data;
    //double seconds;
}

public struct targetVideo
{
    public IMLItem Item;
    public string Videofilename;
    public string targetPath;

    public ImportProgress ThisProgress;

}

public struct itemUpdateParams
{
    public bool FileServerChecked;
    public bool FileServerIsOnline;
    public bool FileInUse;
    public bool CanWorkOnline;
    public bool OSIsOnline;
    public int CompletedItems;
    public int ItemTimeframe;
    public bool SelectiveUpdating;
    public bool IsMultipart;

}

public struct HashUploadParams
{
    public string moviehash;
    public double moviebytesize;
    public string imdbid;
}



#endregion





namespace MediaFairy
{



    public partial class StatusForm : Form
    {

        public StatusForm()
        {
            InitializeComponent();
        }

        #region UI Methods


        private void StatusForm_Load(object sender, EventArgs e)
        {

            #region initialize form components
            //this.MaximizeBox = false;
            //this.MinimizeBox = false;
            this.ShowInTaskbar = true;
            this.ControlBox = false;
            //this.Width = 780;
            //this.Height = 500;

            #region Set DataGrid Preferences
            this.dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.Image.DefaultCellStyle.NullValue = null;
            this.Fanart.DefaultCellStyle.NullValue = null;
            DataGridViewRowCollection rowsMovies = this.dataGridView1.Rows;

            this.datagridMusic.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewImageColumn1.DefaultCellStyle.NullValue = null;
            DataGridViewRowCollection rowsMusic = this.datagridMusic.Rows;

            this.DatagridTvSeries.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewImageColumn2.DefaultCellStyle.NullValue = null;
            DataGridViewRowCollection rowsTvSeries = this.DatagridTvSeries.Rows;
            #endregion

            #endregion


            if (Importer.UIenabled)
            {
                #region Fill Sections Datagrids

                if (Importer.UpdateMoviesSection)
                {
                    UpdateMoviescheckBox.Checked = true;

                    #region fill Movies Datagrid

                    foreach (int id in (int[])Importer.MoviesSection.GetAllItemIDs())
                    {
                        IMLItem Item = Importer.MoviesSection.FindItemByID(id);
                        System.Drawing.Image ItemImage;
                        System.Drawing.Image ItemFanart;

                        object[] ItemValues = new object[5];
                        ItemValues[0] = Item.Name;
                        ItemValues[1] = Item.Location;
                        if ((string)Item.ImageFile != "" && Item.ImageFile != null)
                        {
                            if (File.Exists(Item.ImageFile))
                            {
                                ItemImage = System.Drawing.Image.FromFile(Item.ImageFile);
                                ItemValues[2] = ItemImage;
                            }

                        }

                        if ((string)Item.Tags["fanart"] != "" && Item.Tags["fanart"] != null)
                        {
                            if (File.Exists((string)Item.Tags["fanart"]))
                            {
                                ItemFanart = System.Drawing.Image.FromFile((string)Item.Tags["fanart"]);
                                ItemValues[3] = ItemFanart;
                            }

                        }

                        if ((string)Item.Tags["ImdbID"] != "" && Item.Tags["ImdbID"] != null)
                        {
                            ItemValues[4] = (string)Item.Tags["ImdbID"];
                        }


                        rowsMovies.Add(ItemValues);



                    }

                    #endregion

                }
                else
                {
                    UpdateMoviescheckBox.Checked = false;
                    dataGridView1.Visible = false;
                }

                if (Importer.UpdateMusicSection)
                {
                    UpdateMusicCheckbox.Checked = true;

                    #region fill Music Datagrid

                    foreach (int id in (int[])Importer.MusicSection.GetAllItemIDs())
                    {
                        IMLItem Item = Importer.MusicSection.FindItemByID(id);
                        System.Drawing.Image ItemImage;

                        object[] ItemValues = new object[6];
                        ItemValues[0] = Item.Name;
                        ItemValues[1] = Item.Location;
                        if ((string)Item.ImageFile != "" && Item.ImageFile != null)
                        {
                            if (File.Exists(Item.ImageFile))
                            {
                                ItemImage = System.Drawing.Image.FromFile(Item.ImageFile);
                                ItemValues[2] = ItemImage;
                            }

                        }


                        if ((string)Item.Tags["Artist"] != "" && Item.Tags["Artist"] != null)
                        {
                            ItemValues[3] = (string)Item.Tags["Artist"];
                        }
                        if ((string)Item.Tags["Album"] != "" && Item.Tags["Album"] != null)
                        {
                            ItemValues[4] = (string)Item.Tags["Album"];
                        }
                        if ((string)Item.Tags["Title"] != "" && Item.Tags["Title"] != null)
                        {
                            ItemValues[5] = (string)Item.Tags["Title"];
                        }




                        rowsMusic.Add(ItemValues);



                    }

                    #endregion

                }
                else
                {
                    datagridMusic.Visible = false;
                    UpdateMusicCheckbox.Checked = false;
                }
                #endregion
            }
            else
            {


                Application.DoEvents();
            }

            #region update checkboxes for libraries updating


            //if (Importer.UpdateTvShowsSection)
            //    .Checked = true;
            //else UpdateMoviescheckBox.Checked = false;


            #endregion

        }

        public void UpdateProgress(int Progress)
        {
            progressBar1.Value = Progress;
            //Refresh();
            progressBar1.Refresh();
        }


        private void TrayIcon_DoubleClick(object sender, EventArgs e)
        {

            if (this.Visible)
                this.Hide();
            else
                this.Show();

        }

        private void TrayIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }

        private void PauseButton_Click(object sender, EventArgs e)
        {
            if (ImportingPaused)
            {
                ImportingPaused = false;
                Importer.statusForm.PauseButton.Text = "Pause";
            }
            else
            {
                ImportingPaused = true;
                Importer.statusForm.PauseButton.Text = "Continue";
            }


            if (ImportingPaused)
                PauseLoop();



        }

        private void PauseLoop()
        {

            while (ImportingPaused)
            {
                Application.DoEvents();
                Thread.Sleep(50);
                //this.Refresh();
            }

        }

        private void ConfirmImageCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (ConfirmImageCheckBox.Checked)
                ConfirmImage = true;
            else
                ConfirmImage = false;

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            ConfirmImageDialogResult = DialogResult.Yes;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ConfirmImageDialogResult = DialogResult.No;
        }

        private void imageSelect_Click(object sender, EventArgs e)
        {
            SelectBackropDialogResult = DialogResult.OK;

        }

        private bool ImportingPaused = false;
        public bool ConfirmImage = false;
        public DialogResult ConfirmImageDialogResult = DialogResult.Cancel;
        public DialogResult SelectBackropDialogResult = DialogResult.Cancel;

        private void imagePrev_Click(object sender, EventArgs e)
        {
            SelectBackropDialogResult = DialogResult.No;
        }

        private void imageNext_Click(object sender, EventArgs e)
        {
            SelectBackropDialogResult = DialogResult.Yes;
        }

        private void cancelAll_Click(object sender, EventArgs e)
        {
            Importer.userCancels = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (this.Width == 642)
                Helpers.HideControls();
            else Helpers.ShowControls();




            this.Refresh();
        }

        private void tabPage3_Enter(object sender, EventArgs e)
        {
            Helpers.ShowControls();
            this.Width = 780;
            this.Height = 500;















        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int ColumnIndex = e.ColumnIndex;
            int RowIndex = e.RowIndex;
            DataGridViewRowCollection rows = this.dataGridView1.Rows;

            if (ColumnIndex == 4)
            {
                string CellValue = (string)rows[RowIndex].Cells[ColumnIndex].Value;
                //MessageBox.Show(CellValue);
                string targetURL = @"http://www.imdb.com/title/" + CellValue + @"/";
                System.Diagnostics.Process.Start(targetURL);

            }


        }

        private void statusStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void tabControl1_Enter(object sender, EventArgs e)
        {

        }

        private void tabPage1_Enter(object sender, EventArgs e)
        {
            Helpers.HideControls();
            this.Height = 375;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                //this.dataGridView1.Enabled = false;
                this.dataGridView1.SelectionMode = DataGridViewSelectionMode.CellSelect;

            }
            else
            {
                //this.dataGridView1.Enabled = true;
                this.dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            }




        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            Importer.statusForm.tabControl1.SelectedTab = Importer.statusForm.tabPage1;
            Importer.ImportingStarted = true;
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void UpdateMoviescheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (UpdateMoviescheckBox.Checked)
                Importer.UpdateMoviesSection = true;
            else Importer.UpdateMoviesSection = false;
        }

        #endregion


    }





    #region Supporting Classes


    internal class MyProperties : CustomProperty
    {

        public override bool GetProperty(Int32 Index, OptionSettings Prop)
        {
            #region init vars
            int counter = 1;
            
            string[] threechoices = new string[3];
            string[] twochoices = new string[2];
            string[] fourchoices = new string[4];

            
            #endregion

            try
            {

                if (Index == counter++)
                {
                    Prop.Name = "Mode";
                    Prop.Caption = "Settings Mode: ";
                    Prop.HelpText = "Advanced mode shows all options except those dependent on other options." + Environment.NewLine +
                                    "Simple mode hides advanced and experimental settings";
                    Prop.DataType = "string";
                    Prop.Choices = new[] { "Simple", "Advanced" };
                    Prop.Dependencies = "Simple:HIDEGROUP AudioIdentifier,FilmCovers,FilmBackdrops,TvImages,AlbumCovers,ArchiveExtractor,FilmArchiver,RssUpdater,OnlineMediaDownloader,AutoSorter,Diagnostics,LibraryCleanup,AnalyzerOverrides" +
                                        //";Simple:HIDE ImageTagName,DeleteNonExistentFiles,AlwaysUpdate," +
                                           //"IncludeHiddenFiles,ExcludeFileMasks,MinFileSize," +
                                           //"ChainingOptions,AlwaysUpdateImages" +
                                        ";Advanced:SHOWGROUP AudioIdentifier,FilmCovers,FilmBackdrops,TvImages,AlbumCovers,ArchiveExtractor,FilmArchiver,RssUpdater,OnlineMediaDownloader,AutoSorter,Diagnostics,LibraryCleanup,AnalyzerOverrides" +
                                        ";Advanced:SHOWALL "
                                        ;

                    Prop.GroupName = "misc";
                    Prop.DefaultValue = "Simple";
                    return true;
                }




                #region Video Identifier

                if (Index == counter++)
                {
                    //set the internal name
                    Prop.Name = "WantHashingProp";
                    //set name shown to user
                    Prop.GroupCaption = "                      Video Identifier Settings";
                    Prop.GroupName = "VideoIdentifier";
                    Prop.Caption = "Enable video fingerprint identifier? ";
                    //set the tool tip
                    Prop.HelpText = "Video fingerprint identifying is the method MediaFairy mainly uses in order to identify your films. The Fingerprint Identifying mechanism computes a unique number (fingerprint) for each of your films and then uses the OSdb database to match this fingerpint with the film's unique id (IMDb ID). When a match is not found using this fingerprint, MediaFairy will switch to the name matching method below. ";
                    Prop.DefaultValue = Importer.EnableHashing;
                    Prop.DataType = "bool";
                    return true;
                }

                if (Index == counter++)
                {
                    //set the internal name
                    Prop.Name = "EnableNfoParsingProp";
                    //set name shown to user
                    Prop.Caption = "Enable .nfo parser?";
                    Prop.GroupName = "VideoIdentifier";
                    //set the tool tip
                    Prop.HelpText = " When this option is enabled, the video identifier will scan the film's folder for an .nfo file which contains useful information about the film for instant and exact film identification. ";
                    Prop.DefaultValue = Importer.EnableNfoParsing;
                    Prop.DataType = "bool";
                    return true;
                }

                if (Index == counter++)
                {
                    //set the internal name
                    Prop.Name = "WantMatchingProp";
                    //set name shown to user
                    Prop.Caption = "Enable Title matcher?";
                   
                    Prop.GroupName = "VideoIdentifier";
                    //set the tool tip
                    Prop.HelpText = "If the video fingerprinting mechanism is disabled or unable to identify a film by it's fingerprint, MediaFairy can resort to recognize the film using this method. The Name Matching method will use the film information already provided in 'name', 'Title' and 'Year' section fields to recognize a film and download it's related information. ";
                    Prop.DefaultValue = Importer.EnableNameMatching;
                    Prop.DataType = "bool";
                    Prop.Dependencies = "False:HIDE AssistModeProp" +
                    ";True:SHOW AssistModeProp";

                    return true;
                }

                if (Index == counter++)
                {
                    //set the internal name
                    Prop.Name = "AssistModeProp";
                    //set name shown to user
                    Prop.Caption = "When to ask for film matching assistance? ";
                    Prop.GroupName = "VideoIdentifier";
                    //set the tool tip
                    Prop.HelpText = "Please choose the Film Matching Assist mode. This setting defines in which case you 'll be asked to select the correct film from a list of possible matches. Leave this setting to it's default for unsupervised operation (but with a small probability for incorrect matches when identifying by fingerprint is not possible)  ?";
                    Prop.DefaultValue = Importer.AssistType;
                    Prop.DataType = "string";

                    fourchoices[0] = "When no exact match found";
                    fourchoices[1] = "Always";
                    fourchoices[2] = "Never";
                    fourchoices[3] = "Skip identifying unknown films";
                    Prop.Choices = fourchoices;
                    Prop.CanTypeChoices = false;
                    return true;
                }
                #endregion

                #region Audio Identifier


                if (Index == counter++)
                {
                    //set the internal name
                    Prop.Name = "SaveMetadataToAudioTagsProp";
                    //set name shown to user
                    Prop.Caption = "Save meta-data in audio file?";
                    //set the tool tip
                    Prop.GroupCaption = "                     Audio Identifier Settings";
                    Prop.GroupName = "AudioIdentifier";
                    Prop.HelpText = "After succesfull recognition by audio fingerprint, the newly acquired metadata can be imported into the tags of the audio file, and not only in the Meedio library.";
                    Prop.DefaultValue = Importer.SaveMetadataToAudioTags;
                    
                    Prop.DataType = "bool";
                    return true;
                }
                if (Index == counter++)
                {
                    //set the internal name
                    Prop.Name = "WantItunesSyncProp";
                    //set name shown to user
                    Prop.Caption = "Synchronize with iTunes library? ";
                    Prop.GroupName = "AudioIdentifier";
                    //set the tool tip
                    Prop.HelpText = "MediaFairy offers automated 2-way synchronization with the iTunes music library. Any new audio tracks are contained in the Meedio music library can be imported into the iTunes lbrary and vice versa.";
                    Prop.DefaultValue = Importer.WantItunesSync;
                    Prop.DataType = "bool";
                    return true;
                }

                #endregion

                #region Film Details
                if (Index == counter++)
                {
                    //set the internal name
                    Prop.Name = "PopulateIMDbDetailsProp";
                    //set name shown to user
                    Prop.Caption = "Download Film Details from IMDb? ";
                    Prop.GroupCaption = "                              Film Details";
                    Prop.GroupName = "FilmDetails";
                    //set the tool tip
                    Prop.HelpText = " Do you want MediaFairy to download details for your films and import them into your library?";
                    Prop.DefaultValue = Importer.WantIMDbDetails;
                    Prop.DataType = "bool";
                    return true;
                }
                if (Index == counter++)
                {
                    //set the internal name
                    Prop.Name = "PopulateOSdbDetailsProp";
                    //set name shown to user
                    Prop.Caption = "Download Film Details from OSdb? ";
                    Prop.GroupName = "FilmDetails";
                    //set the tool tip
                    Prop.HelpText = " Do you want MediaFairy to use OSdb as one of the film details online sources? MediaFairy will download film details from OSdb only if IMDb is offline or unreachable";
                    Prop.DefaultValue = Importer.WantOSdbDetails;
                    Prop.DataType = "bool";
                    return true;
                }
                if (Index == counter++)
                {
                    //set the internal name
                    Prop.Name = "WriteFilmDescriptorsProp";
                    //set name shown to user
                    Prop.Caption = "Save film descriptors ";
                    Prop.GroupName = "FilmDetails";
                    //set the tool tip
                    Prop.HelpText = "Film descriptors are standrard .xml files which contain a film's identifying information and details. If this feature is enabled, a film descriptor will be saved (in the film's directory) for each film the plugin successfully identifies. This procedure offers greatly increased updating performance if/when the film needs to be imported again, because if a film descriptor is found, the plugin will use this file to obtain the film's details, thus skipping the video identifying phase. ";
                    Prop.DefaultValue = Importer.WriteFilmDescriptors;
                    Prop.DataType = "bool";
                    return true;
                }
                #endregion

                #region Media Images

                #region Film Posters
                if (Index == counter++)
                {
                    //set the internal name
                    Prop.Name = "PrimaryPosterSourceChoice";
                    //set name shown to user
                    Prop.Caption = "Primary Posters Source? ";
                    //set the tool tip
                    Prop.GroupCaption = "                           Film Covers Settings";
                    Prop.GroupName = "FilmCovers";
                    Prop.HelpText = "From which source do you want MediaFairy to search for covers first?";
                    Prop.DefaultValue = Importer.PosterPrimarySource;
                    Prop.DataType = "string";
                    Prop.CanTypeChoices = false;
                    threechoices[0] = "TheMovieDatabase";
                    threechoices[1] = "NiceCovers";
                    threechoices[2] = "IMDb";
                    Prop.Choices = threechoices;
                    return true;
                }


                if (Index == counter++)
                {
                    //set the internal name
                    Prop.Name = "PosterSizeChoice";
                    //set name shown to user
                    Prop.Caption = "Preferred Cover Size? ";
                    Prop.GroupName = "FilmCovers";
                    //set the tool tip
                    Prop.HelpText = "When downloading covers from TMDB, which poster size do you prefer?";
                    Prop.DefaultValue = Importer.PosterSize;
                    Prop.DataType = "string";
                    fourchoices[0] = "Extra Large";
                    fourchoices[1] = "Large";
                    fourchoices[2] = "Medium";
                    fourchoices[3] = "Small";
                    Prop.CanTypeChoices = false;
                    Prop.Choices = fourchoices;
                    return true;
                }

                if (Index == counter++)
                {
                    //set the internal name
                    Prop.Name = "PosterNamingChoice";
                    //set name shown to user
                    Prop.Caption = "Poster Naming Convention? ";
                    Prop.GroupName = "FilmCovers";
                    //set the tool tip
                    Prop.HelpText = "How would you like your movies' posters image files to be named ?";
                    Prop.DefaultValue = Importer.PosterNamingMethod;
                    Prop.DataType = "string";

                    threechoices[0] = "folder.jpg";
                    threechoices[1] = "iMDbID";
                    threechoices[2] = "video filename";
                    Prop.CanTypeChoices = false;
                    Prop.Choices = threechoices;
                    return true;
                }

                if (Index == counter++)
                {
                    //set the internal name
                    Prop.Name = "WhereToSavePosterProp";
                    //set name shown to user
                    Prop.Caption = "Where to save Poster?";
                    Prop.GroupName = "FilmCovers";
                    //set the tool tip
                    Prop.HelpText = "Do you want the poster/cover image for each movie to be saved in the movie's folder or to a specific folder of your choosing?";
                    Prop.DefaultValue = Importer.WhereToSavePoster;
                    Prop.DataType = "string";
                    twochoices[0] = "in the movie's folder";
                    twochoices[1] = "in the folder i specify below";
                    Prop.CanTypeChoices = false;
                    Prop.Choices = twochoices;
                    Prop.IsMandatory = false;

                    Prop.Dependencies = "in the movie's folder:HIDE PosterFolderProp" +
                    ";in the folder i specify below:SHOW PosterFolderProp";
                    return true;
                }

                if (Index == counter++)
                {
                    //set the internal name
                    Prop.Name = "PosterFolderProp";
                    //set name shown to user
                    Prop.Caption = "Posters folder: ";
                    Prop.GroupName = "FilmCovers";
                    //set the tool tip
                    Prop.HelpText = @"Please specify the folder in which you want downloaded film covers to be saved. (Only applicabe is option above is set to 'in the folder i specify below') ";
                    Prop.DefaultValue = Importer.PosterFolder;
                    Prop.DataType = "folder";
                    Prop.CanTypeChoices = false;
                    return true;
                }
                if (Index == counter++)
                {
                    //set the internal name
                    Prop.Name = "OverwritePosterChoice";
                    //set name shown to user
                    Prop.Caption = "Overwrite existing Posters? ";
                    Prop.GroupName = "FilmCovers";
                    //set the tool tip
                    Prop.HelpText = "If the image field is already filled in, do you want MediaFairy to download a poster and overwrite this image field?";
                    Prop.DefaultValue = Importer.OverwriteExistingPoster;
                    Prop.DataType = "bool";
                    return true;
                }
                #endregion

                #region Backdrops
                if (Index == counter++)
                {
                    //set the internal name
                    Prop.Name = "BackdropNamingChoice";
                    //set name shown to user
                    Prop.Caption = "Backdrop Naming Convention? ";
                    //set the tool tip
                    Prop.GroupName = "FilmBackdrops";
                    Prop.GroupCaption = "                     Film Backdrops Settings";
                    Prop.HelpText = "How would you like your movies' backdrop/fanart image files to be named ?";
                    Prop.DefaultValue = Importer.BackdropNamingMethod;
                    Prop.DataType = "string";

                    threechoices[0] = "folder.jpg";
                    threechoices[1] = "iMDbID";
                    threechoices[2] = "video filename";
                    Prop.CanTypeChoices = false;
                    Prop.Choices = threechoices;
                    return true;
                }

                if (Index == counter++)
                {
                    //set the internal name
                    Prop.Name = "WhereToSaveBackdropProp";
                    //set name shown to user
                    Prop.Caption = "Where to save Backdrop?";
                    //set the tool tip
                    Prop.GroupName = "FilmBackdrops";
                    Prop.HelpText = "Do you want the backdrop/fanart image for each movie to be saved in the movie's folder or to a specific folder of your choosing?";
                    Prop.DefaultValue = Importer.WhereToSaveBackdrop;
                    Prop.DataType = "string";
                    twochoices[0] = "in the movie's folder";
                    twochoices[1] = "in the folder i specify below";
                    Prop.Choices = twochoices;
                    Prop.CanTypeChoices = false;

                    Prop.Dependencies = "in the movie's folder:HIDE BackdropFolderProp" +
                    ";in the folder i specify below:SHOW BackdropFolderProp";
                    return true;
                }

                if (Index == counter++)
                {
                    //set the internal name
                    Prop.Name = "BackdropFolderProp";
                    //set name shown to user
                    Prop.Caption = "Backdrops folder: ";
                    Prop.GroupName = "FilmBackdrops";
                    //set the tool tip
                    Prop.HelpText = @"Please specify the folder in which you want downloaded Backdrops/Fanart to be saved. (Only applicabe if option above is *not* set to 'Movie's folder') ";
                    Prop.DefaultValue = Importer.BackdropFolder;
                    Prop.DataType = "folder";
                    Prop.CanTypeChoices = false;
                    return true;
                }
                #endregion

                #region TV Series Images

                if (Index == counter++)
                {
                    Prop.GroupCaption = "                   Tv Series Images Settings";
                    Prop.GroupName = "TvImages";
                    //set the internal name
                    Prop.Name = "SeriesBannersFolderProp";
                    //set name shown to user
                    Prop.Caption = "Series Banners location: ";
                    //set the tool tip
                    Prop.HelpText = @"Please specify the folder in which you want downloaded banners for TV series to be saved. ";
                    Prop.DefaultValue = Importer.SeriesBannersFolder;
                    Prop.DataType = "folder";
                    Prop.CanTypeChoices = false;
                    return true;
                }

                if (Index == counter++)
                {
                    Prop.GroupName = "TvImages";
                    //set the internal name
                    Prop.Name = "SeriesFanartFolderProp";
                    //set name shown to user
                    Prop.Caption = "Series Backdrops location: ";
                    //set the tool tip
                    Prop.HelpText = @"Please specify the folder in which you want downloaded backdrops for TV series to be stored. ";
                    Prop.DefaultValue = Importer.SeriesFanartFolder;
                    Prop.DataType = "folder";
                    Prop.CanTypeChoices = false;
                    return true;
                }

                if (Index == counter++)
                {
                    Prop.GroupName = "TvImages";
                    //set the internal name
                    Prop.Name = "SeasonImagesFolderProp";
                    //set name shown to user
                    Prop.Caption = "Season images location: ";
                    //set the tool tip
                    Prop.HelpText = @"Please specify the folder in which you want downloaded banners for TV series to be saved. ";
                    Prop.DefaultValue = Importer.SeasonImagesFolder;
                    Prop.DataType = "folder";
                    Prop.CanTypeChoices = false;
                    return true;
                }

                #endregion


                #region Album Covers
                if (Index == counter++)
                {
                    Prop.GroupName = "AlbumCovers";
                    //set the internal name
                    Prop.Name = "WhereToSaveAlbumCoverProp";
                    //set name shown to user
                    Prop.Caption = "Where to save album covers?";
                    //set the tool tip
                    Prop.GroupCaption = "                       Album Covers Settings";
                    Prop.HelpText = "Do you want the poster/cover image for each album to be saved in the album's folder or in a specific folder of your choice?";
                    Prop.DefaultValue = Importer.WhereToSaveAlbumCover;
                    Prop.DataType = "string";
                    twochoices[0] = "in the album's folder";
                    twochoices[1] = "in the folder i specify below";
                    Prop.CanTypeChoices = false;
                    Prop.Choices = twochoices;
                    Prop.IsMandatory = false;

                    Prop.Dependencies = "in the album's folder:HIDE AlbumCoversFolderProp" +
                    ";in the folder i specify below:SHOW AlbumCoversFolderProp";
                    return true;
                }

                if (Index == counter++)
                {
                    Prop.GroupName = "AlbumCovers";
                    //set the internal name
                    Prop.Name = "AlbumCoversFolderProp";
                    //set name shown to user
                    Prop.Caption = "Album covers folder: ";
                    //set the tool tip
                    Prop.HelpText = @"Please specify the folder in which you want downloaded album cover images to be saved. (Only applicabe if option above is set to 'Movie's folder') ";
                    Prop.DefaultValue = Importer.AlbumCoverFolder;
                    Prop.DataType = "folder";
                    Prop.CanTypeChoices = false;
                    return true;
                }

                if (Index == counter++)
                {
                    Prop.GroupName = "AlbumCovers";
                    //set the internal name
                    Prop.Name = "EmbedAlbumCoverProp";
                    //set name shown to user
                    Prop.Caption = "Also embed cover image in audio file";
                    //set the tool tip
                    Prop.HelpText = " Certain audio file-types like .mp3 and .wma are capable of storing (embeding) the album's cover image in the file itself. \r This eliminates the need of storing a seperate image file for every music album, thus makes the audio files self-contained and easily transportable. \r Do you want the plugin to embed cover images for imported audio files? ";
                    Prop.DefaultValue = Importer.EmbedAlbumCover;
                    Prop.DataType = "bool";
                    Prop.CanTypeChoices = false;
                    return true;
                }
                #endregion

                #endregion

                #region Subtitles Properties

                if (Index == counter++)
                {
                    //set the internal name
                    Prop.Name = "DownloadSubtitlesChoice";
                    //set name shown to user
                    Prop.Caption = "Download synchronized subtitles? ";
                    //set the tool tip
                    Prop.HelpText = "Do you want MediaFairy to automatically download subtitles for your movies?";
                    Prop.GroupCaption = "                             Subtitles Settings";
                    Prop.DefaultValue = Importer.WantSubtitles;
                    Prop.Dependencies = "False:HIDE LanguageProp" +
                                        ";True:SHOW LanguageProp";
                    Prop.DataType = "bool";
                    twochoices[0] = "eng";
                    twochoices[1] = "ell";
                    //Prop.Choices = choices;
                    return true;
                }





                if (Index == counter++)
                {
                    //set the internal name
                    Prop.Name = "LanguageProp";
                    //set name shown to user
                    Prop.Caption = "Primary Subtitles language:";
                    //set the tool tip
                    Prop.HelpText = "Please insert the ISO639 3-letter code of your country. MediaFairy will try to download subtitles for this language first. To find your country code use this reference: http://en.wikipedia.org/wiki/List_of_ISO_639-2_codes ";
                    Prop.DefaultValue = Importer.PrimaryLanguage;

                    Prop.DataType = "string";
                    twochoices[0] = "eng";
                    twochoices[1] = "ell";
                    //Prop.Choices = choices;
                    return true;
                }


                //if (Index == counter++)
                //{
                //    //set the internal name
                //    Prop.Name = "LanguagePropSecondary";
                //    //set name shown to user
                //    Prop.Caption = "Secondary subtitles language:";
                //    //set the tool tip
                //    Prop.HelpText = "If no subtitles are found for your primary language, MediaFairy will search for subtitles for this language.";
                //    Prop.DefaultValue = SecondaryLanguage;
                //    Prop.DataType = "string";
                //    twochoices[0] = "to be implemented soon";
                //    twochoices[1] = "";
                //    Prop.Choices = twochoices;
                //    return true;
                //}




                //if (Index == counter++)
                //{
                //    //set the internal name
                //    Prop.Name = "WhenToUseSecondaryLanguageProp";
                //    //set name shown to user
                //    Prop.Caption = "When to use Secondary?";
                //    //set the tool tip
                //    Prop.HelpText = "Reserved for next version";
                //    Prop.DefaultValue = WhenToUseSecondaryLanguage;
                //    Prop.DataType = "string";
                //    twochoices[0] = "If no synchronized subs exist for primary language";
                //    twochoices[1] = "If no subtitles at all exist for primary language";
                //    Prop.Choices = twochoices;
                //    Prop.IsMandatory = false;
                //    return true;
                //}
                #endregion


                #region Archive Extractor
                if (Index == counter++)
                {
                    Prop.GroupName = "ArchiveExtractor";
                    //set the internal name
                    Prop.Name = "EnableArchiveExtractorProp";
                    //set name shown to user
                    Prop.GroupCaption = "                              Archive Extractor";
                    Prop.Caption = "Auto-extract compressed archives";
                    //set the tool tip
                    Prop.HelpText = "The built-in media importer has the capability to automatically extract zip and/or rar archives contained in your specified media root directories and import the media files they may contain. Multi-part rar archives are also supported.";
                    Prop.DefaultValue = Importer.EnableArchiveExtractor;
                    Prop.DataType = "bool";

                    Prop.Dependencies = "False:HIDE DeleteArchiveAfterExtractionProp,freespaceThresholdProp" +
                    ";True:SHOW DeleteArchiveAfterExtractionProp,freespaceThresholdProp";
                    return true;
                }
                if (Index == counter++)
                {
                    Prop.GroupName = "ArchiveExtractor";
                    //set the internal name
                    Prop.Name = "DeleteArchiveAfterExtractionProp";
                    //set name shown to user
                    Prop.Caption = "Delete archive after extraction ";
                    //set the tool tip
                    Prop.HelpText = "When (and only) an archive extraction process is succesfull, the media importer can delete the original compressed archive in order to free the disk space used by that archive.";
                    Prop.DefaultValue = Importer.DeleteArchiveAfterExtraction;
                    Prop.DataType = "bool";
                    return true;
                }
                if (Index == counter++)
                {
                    Prop.GroupName = "ArchiveExtractor";
                    //set the internal name
                    Prop.Name = "freespaceThresholdProp";
                    //set name shown to user
                    Prop.Caption = "Maintain at least X MB of disk space: ";
                    //set the tool tip
                    Prop.HelpText = "Please specify here the amount of free disk space (in Megabytes, MB) you would like the plugin to always mantain. When this limit is calculated to be exceeded, the archive extraction process in question will be skipped.";
                    Prop.DefaultValue = Importer.freespaceThreshold;
                    Prop.DataType = "int";
                    return true;
                }
                #endregion


                #region Watched Film Archiver
                if (Index == counter++)
                {
                    Prop.GroupName = "FilmArchiver";
                    //set the internal name
                    Prop.Name = "ArchiveWatchedFilmsProp";
                    //set name shown to user
                    Prop.GroupCaption = "                      Watched films archiver";
                    Prop.Caption = "Archive watched films";
                    //set the tool tip
                    Prop.HelpText = "If you enable this option, the built-in archiver will compress and store (in the directory you provide below) a zip archive for each watched film,  containing the entire film's directory.\r The original directory containing the film will be deleted, but only when the archiving operation is succesfull.\r Note that in order to determine if a film is watced or not, the plugin will look for a tag named 'Watched' or 'watched' in your film library.";
                    Prop.DefaultValue = Importer.ArchiveWatchedFilms;
                    Prop.DataType = "bool";
                    return true;
                }
                if (Index == counter++)
                {
                    Prop.GroupName = "FilmArchiver";
                    //set the internal name
                    Prop.Name = "ArchiveLocationProp";
                    //set name shown to user
                    Prop.Caption = "Directory to store archives: ";
                    //set the tool tip
                    Prop.HelpText = "Please specify the directory in which the zip archives of your watched films will be stored: ";
                    Prop.DefaultValue = Importer.ArchiveLocation;
                    Prop.DataType = "folder";
                    return true;
                }
                #endregion


                #region Releases Updater

                if (Index == counter++)
                {
                    Prop.GroupName = "RssUpdater";
                    //set the internal name
                    Prop.Name = "EnableFilmReleasesProp";
                    //set name shown to user
                    Prop.GroupCaption = "                        RSS Releases Updater";
                    Prop.Caption = "Download film releases via BitTorrent";
                    //set the tool tip
                    Prop.HelpText = "The Film Releases Updater will scan for new film releases the rss feeds you provide in the RSS Sources.txt file (in the plugin's direcotry), \n  and download those films using your default BitTorrent client. Adittional filters can be provided like the film's production year or IMDb rating.  ";
                    Prop.DefaultValue = Importer.DownloadFilmReleases;
                    Prop.DataType = "bool";
                    return true;
                }

                //if (Index == counter++)
                //{
                //    //set the internal name
                //    Prop.Name = "FilmReleasesRssProp";
                //    //set name shown to user
                //    Prop.Caption = " RSS feeds for films releases: ";
                //    //set the tool tip
                //    Prop.HelpText = "Please provide here the rss feed URLs which will be scanned for new film releases. Important Notice: Please make sure that the sources you provide contain public domain media, in order to comply with copyright regulations.  ";
                //    Prop.DefaultValue = Importer.FilmsRssFeeds;
                //    Prop.DataType = "stringlist";
                //    return true;
                //}


                if (Index == counter++)
                {
                    Prop.GroupName = "RssUpdater";
                    //set the internal name
                    Prop.Name = "FilmRatingMinProp";
                    Prop.Caption = "Minimum film IMDb rating: ";
                    //set the tool tip
                    Prop.HelpText = "This value represents the minimum IMDb rating that a film must have, in order to be downloaded. Set this value to zero (0) to disable the rating filter.";
                    Prop.DefaultValue = Importer.FilmRatingMin2;
                    Prop.DataType = "string";

                    return true;
                }
                #endregion


                #region Online Media Downloader

                if (Index == counter++)
                {
                    Prop.GroupName = "OnlineMediaDownloader";
                    //set the internal name
                    Prop.Name = "DownloadOnlineContentProp";

                    Prop.GroupCaption = "                     Online Media Downloader";

                    //set name shown to user
                    Prop.Caption = "Download Online media? ";
                    //set the tool tip
                    Prop.HelpText = "MediaFairy can automatically download any online media file (from the web or ftp address) which is contained in the field you specify in the 'Online content library field' option below,  and save it to the folder you specify below. Refer to the plugin's documentation for more information and usage paradigms.";
                    Prop.DefaultValue = Importer.DownloadOnlineContent;
                    Prop.DataType = "bool";
                    return true;
                }

                if (Index == counter++)
                {
                    Prop.GroupName = "OnlineMediaDownloader";
                    //set the internal name
                    Prop.Name = "OnlineContentFieldProp";
                    //set name shown to user
                    Prop.Caption = "Online media library fields: ";
                    //set the tool tip
                    Prop.HelpText = " Please specify here the library fields (Tags) in which the online files you desire to store locally are contained. MediaFairy will download the online links contained in these tags and replace them with the disk paths of the local files those links were downloaded to. Please input those tag names by either clicking the '...' button and typing a tag name *or* by typing the tag names directly in the input box, seperating with a comma (,). MediaFairy will search for those tags in all currently supported media libraries and save the online files they contain in the local locations you specify below. ";
                    Prop.DefaultValue = Importer.OnlineContentFields;
                    Prop.DataType = "stringlist";
                    return true;
                }


                if (Index == counter++)
                {
                    Prop.GroupName = "OnlineMediaDownloader";
                    //set the internal name
                    Prop.Name = "DownloadFolderProp";
                    //set name shown to user
                    Prop.Caption = "Save online media in: ";
                    //set the tool tip
                    Prop.HelpText = @"Please specify the folder in which you want downloaded online content to be saved. ";
                    Prop.DefaultValue = Importer.DownloadFolder;
                    Prop.DataType = "folder";
                    return true;
                }

                #endregion


                #region Sorting / Archiving Properties

                #region Enable / disable media organizers
                if (Index == counter++)
                {
                    Prop.GroupName = "AutoSorter";
                    //set the internal name
                    Prop.Name = "WantSortingFilmsProp";
                    //set name shown to user
                    Prop.GroupCaption = "                         Media Organizer";
                    Prop.Caption = "Automatically organize films ";
                    //set the tool tip
                    Prop.HelpText = "If this option is enabled, MediaFairy will automatically move or rename the films which it has identified, accroding to your specifications below. This feature will ensure a default and well organized structure for your digital films and their related files. ";
                    Prop.DefaultValue = Importer.WantSortingFilms;
                    Prop.DataType = "bool";
                    return true;
                }
                if (Index == counter++)
                {
                    Prop.GroupName = "AutoSorter";
                    //set the internal name
                    Prop.Name = "WantSortingTvShowsProp";
                    //set name shown to user
                    Prop.Caption = "Automatically organize tv series ";
                    //set the tool tip
                    Prop.HelpText = "If this option is enabled, MediaFairy will automatically move or rename the films which it has identified, accroding to your specifications below. This feature will ensure a default and well organized structure for your digital films and their related files. ";
                    Prop.DefaultValue = Importer.WantSortingTvShows;
                    Prop.DataType = "bool";
                    return true;
                }
                if (Index == counter++)
                {
                    Prop.GroupName = "AutoSorter";
                    //set the internal name
                    Prop.Name = "WantSortingMusicProp";
                    //set name shown to user
                    Prop.Caption = "Automatically organize music";
                    //set the tool tip
                    Prop.HelpText = "If this option is enabled, MediaFairy will automatically move or rename the films which it has identified, accroding to your specifications below. This feature will ensure a default and well organized structure for your digital films and their related files. ";
                    Prop.DefaultValue = Importer.WantSortingMusic;
                    Prop.DataType = "bool";
                    return true;
                }
                if (Index == counter++)
                {
                    Prop.GroupName = "AutoSorter";
                    //set the internal name
                    Prop.Name = "PromptBeforeMovingProp";
                    //set name shown to user
                    Prop.Caption = "Prompt me before any file operation";
                    //set the tool tip
                    Prop.HelpText = "If you enable this option, MediaFairy will ask for your approval before performing any renaming or moving operation on media files. This option is here for debugging purposes. ";
                    Prop.DefaultValue = Importer.PromptBeforeMoving;
                    Prop.DataType = "bool";
                    return true;
                }
                #endregion

                #region Destination Directories
                if (Index == counter++)
                {
                    Prop.GroupName = "AutoSorter";
                    //set the internal name
                    Prop.Name = "SortingDestinationFilmsProp";
                    //set name shown to user
                    Prop.Caption = "Destination directory for films: ";
                    //set the tool tip
                    Prop.HelpText = @"Please specify the destination directory in which you want your sorted films and their related files to be moved by MediaFairy. Tip: specify the folder which your films are already contained and blank folder mask for a simple renaming.";
                    Prop.DefaultValue = Importer.SortingDestinationFilms;
                    Prop.DataType = "folder";
                    return true;
                }
                if (Index == counter++)
                {
                    Prop.GroupName = "AutoSorter";
                    //set the internal name
                    Prop.Name = "SortingDestinationTvShowsProp";
                    //set name shown to user
                    Prop.Caption = "Destination directory for tv series: ";
                    //set the tool tip
                    Prop.HelpText = @"Please specify the destination directory in which you want your sorted Tv Shows and their related files to be moved by MediaFairy. Tip: specify the folder which your films are already contained and blank folder mask for a simple renaming.";
                    Prop.DefaultValue = Importer.SortingDestinationTvShows;
                    Prop.DataType = "folder";
                    return true;
                }
                if (Index == counter++)
                {
                    Prop.GroupName = "AutoSorter";
                    //set the internal name
                    Prop.Name = "SortingDestinationMusicProp";
                    //set name shown to user
                    Prop.Caption = "Destination directory for music: ";
                    //set the tool tip
                    Prop.HelpText = @"Please specify the destination directory in which you want your sorted music tracks and their related files to be moved by MediaFairy. Tip: specify the folder which your films are already contained and blank folder mask for a simple renaming.";
                    Prop.DefaultValue = Importer.SortingDestinationMusic;
                    Prop.DataType = "folder";
                    return true;
                }
                #endregion

                if (Index == counter++)
                {
                    Prop.GroupName = "AutoSorter";
                    //set the internal name
                    Prop.Name = "DirectoryStructureProp";
                    //set name shown to user
                    Prop.Caption = "Preferred directory structures: ";
                    //set the tool tip
                    Prop.HelpText = " What is the directory structure in which you want your media files to be organized? MediaFairy will use known information relevant to each media type to give the appropriate naming to folders and media files, according to your selected AutoSorting mode. 'Simple' mode will use the default directory structures for each media type, while 'advanced' lets you specify your own directory structure for each media type using a tag mask. Please refer to plugin's manual for more information on directory structures. ";
                    Prop.DefaultValue = Importer.DirectoryStructure;
                    Prop.DataType = "string";

                    twochoices[0] = "Simple: default organising structures for each media type.";
                    twochoices[1] = "Advanced: specify directory tag masks below.";

                    Prop.Choices = twochoices;
                    return true;
                }
                if (Index == counter++)
                {
                    Prop.GroupName = "AutoSorter";
                    //set the internal name
                    Prop.Name = "DirectoryTagMaskFilmsProp";
                    //set name shown to user
                    Prop.Caption = "Folder tag mask for films? ";
                    //set the tool tip
                    Prop.HelpText = @" Please type your desired folder structure in the form of a tag mask. For each additional directory level you want to be created based on the value of an item's specific tag, include this folder name in < and > brackets. For example, if you wanted your directory structure to include a folder for Film Genres and below this level each movie to reside in it's own folder named after the film's name, you would use this tag mask: <Genre>\<Title> . ";
                    Prop.DefaultValue = Importer.DirectoryTagMaskFilms;
                    Prop.DataType = "string";
                    return true;
                }
                if (Index == counter++)
                {
                    Prop.GroupName = "AutoSorter";
                    //set the internal name
                    Prop.Name = "DirectoryTagMaskTvShowsProp";
                    //set name shown to user
                    Prop.Caption = "Folder tag mask for tv shows? ";
                    //set the tool tip
                    Prop.HelpText = @" Please type your desired folder structure in the form of a tag mask. For each additional directory level you want to be created based on the value of an item's specific tag, include this folder name in < and > brackets. For example, if you wanted your directory structure to include a folder for Film Genres and below this level each movie to reside in it's own folder named after the film's name, you would use this tag mask: <Genre>\<Title> . ";
                    Prop.DefaultValue = Importer.DirectoryTagMaskTvShows;
                    Prop.DataType = "string";
                    return true;
                }
                if (Index == counter++)
                {
                    Prop.GroupName = "AutoSorter";
                    //set the internal name
                    Prop.Name = "DirectoryTagMaskMusicProp";
                    //set name shown to user
                    Prop.Caption = "Folder tag mask for music? ";
                    //set the tool tip
                    Prop.HelpText = @" Please type your desired folder structure in the form of a tag mask. For each additional directory level you want to be created based on the value of an item's specific tag, include this folder name in < and > brackets. For example, if you wanted your directory structure to include a folder for Film Genres and below this level each movie to reside in it's own folder named after the film's name, you would use this tag mask: <Genre>\<Title> . ";
                    Prop.DefaultValue = Importer.DirectoryTagMaskMusic;
                    Prop.DataType = "string";
                    return true;
                }

                #endregion


                #region Diagnostics Properties
                if (Index == counter++)
                {
                    Prop.GroupName = "Diagnostics";
                    //set the internal name
                    Prop.Name = "ConnectionDiagnosticsProp";
                    //set name shown to user
                    Prop.Caption = "Enable internet connection diagnostic? ";
                    Prop.GroupCaption = "                               Diagnostics";
                    //set the tool tip
                    Prop.HelpText = " Do you want MediaFairy to test for internet connection and online databases availability before attempting to download data? (This feature will help the importer recognize offline or problematic sources and avoid using them, preventing failed retries, minimizing delays and improving importing times in those situations.) ";
                    Prop.DefaultValue = Importer.WantConnectionDiagnostics;
                    Prop.DataType = "bool";
                    return true;
                }
                if (Index == counter++)
                {
                    Prop.GroupName = "Diagnostics";
                    //set the internal name
                    Prop.Name = "FileserverDiagnosticsProp";
                    //set name shown to user
                    Prop.Caption = "Enable media server diagnostic? ";
                    //set the tool tip
                    Prop.HelpText = " Do you want MediaFairy to check if your network disk is online and accessible before attempting to transfer any data? This feature will prevent failed attempts to access network disks and improve importing times in those cases.";
                    Prop.DefaultValue = Importer.WantFileserverDiagnostics;
                    Prop.DataType = "bool";
                    return true;
                }

                #endregion

                #region User Interface

                if (Index == counter++)
                {
                    Prop.GroupName = "UserInterface";
                    //set the internal name
                    Prop.Name = "EnableUIProp";
                    //set name shown to user
                    Prop.Caption = "Always prompt for images";
                    Prop.GroupCaption = "                          User Interface";
                    //set the tool tip
                    Prop.HelpText = "The plugin's user interface gives users the option to preview and decide if images (film posters,backdrops and album covers) will be downloaded and which ones.\r If you want the plugin to always ask for your confirmation before downloading images, please check this option.";
                    Prop.DefaultValue = Importer.AlwaysPromptForImages;
                    Prop.DataType = "bool";
                    return true;
                }



                //if (Index == counter++)
                //{
                //    //set the internal name
                //    Prop.Name = "SleepDelayProp";
                //    //set name shown to user
                //    Prop.Caption = "Messages display interval (ms): ";
                //    //set the tool tip
                //    Prop.HelpText = "MediaFairy displays delayed informative messages which update you about important events and import progress. The importing will pause with these messagess on-screen (for the interval you specify here) for you to see them and then the importer will continue it's work as normally. Chnage the value to 0 to disable informative messages completely. (useful if you schedule the importer to run automatically without your supervision). ";
                //    Prop.DefaultValue = Importer.SleepValue;
                //    Prop.DataType = "int";
                //    return true;
                //}
                //if (Index == counter++)
                //{
                //    //set the internal name
                //    Prop.Name = "LimitFilesToImportProp";
                //    //set name shown to user
                //    Prop.Caption = "How many media files to import in each run? ";
                //    //set the tool tip
                //    Prop.GroupCaption = "                                   Interoperability";

                //    Prop.HelpText = "With this option, you can limit the number of new files MediaFairy imports into your library every time it runs. This setting is useful if you schedule the importer to run frequently and you want each importing proccess to complete quickly. Leave this setting to it's default setting (0) if you want MediaFairy to import all new media files it finds in your media root folders.  ";
                //    Prop.DefaultValue = Importer.importing_limit;
                //    Prop.DataType = "int";
                //    return true;
                //}

                //if (Index == counter++)
                //{
                //    //set the internal name
                //    Prop.Name = "RunImportersProp";
                //    //set name shown to user
                //    Prop.Caption = "Run importers upon completion: ";
                //    //set the tool tip
                //    Prop.HelpText = "MediaFairy has the capability to run consequently the importers you specify here when all importing jobs are finished. Please input the IDs of the importers you want to run, by typing one ID in each line. ";
                //    Prop.DefaultValue = Importer.ImporterIDs;
                //    Prop.DataType = "stringlist";
                //    return true;
                //}



                #endregion

                #region Library Cleanup

                if (Index == counter++)
                {
                    Prop.GroupName = "LibraryCleanup";
                    //set the internal name
                    Prop.Name = "EnableChainLinkingProp";
                    //set name shown to user
                    Prop.GroupCaption = "                             Library Cleanup";
                    Prop.Caption = "Link orphaned video files";
                    //set the tool tip
                    Prop.HelpText = "If this setting is enabled, the films section updater will scan and link together multiple video files of the same film, using the film's unique id.\r This process is not done on the file importing phase but the updating phase which means that if your already imported film library contains unlinked video files of the same film,\r this process will find and link them together. \r Another application of this feature is for cases where conventional video file linking methods are unsuccesfull, for example if your films are not contained in seperate directories.";
                    Prop.DefaultValue = Importer.EnableChainLinking;
                    Prop.DataType = "bool";
                    return true;
                }
                if (Index == counter++)
                {
                    Prop.GroupName = "LibraryCleanup";
                    //set the internal name
                    Prop.Name = "DeleteMissingProp";
                    //set name shown to user
                    Prop.Caption = "Delete missing library items";
                    //set the tool tip
                    Prop.HelpText = "This feature will automatically delete library items which refer to non-existent (deleted) files. The NAS (file server) diagnostic will ensure that the items located in your file server will not be accidentaly deleted if the file server is offline. ";
                    Prop.DefaultValue = Importer.DeleteMissing;
                    Prop.DataType = "bool";
                    return true;
                }


                if (Index == counter++)
                {
                    Prop.GroupName = "LibraryCleanup";
                    //set the internal name
                    Prop.Name = "ClearEmptyFieldsProp";
                    //set name shown to user
                    Prop.Caption = "Remove Empty library fields";
                    //set the tool tip
                    Prop.HelpText = "If you enable this option, MediaFairy will remove all empty tags in your library. Only the tags that have all their values empty will be removed. Enabling this option will ensure a cleaner library and faster library accessing operations. ";
                    Prop.DefaultValue = Importer.CleanEmptyFields;
                    Prop.DataType = "bool";
                    return true;
                }


                if (Index == counter++)
                {
                    Prop.GroupName = "LibraryCleanup";
                    //set the internal name
                    Prop.Name = "CleanSelectedFieldsProp";
                    //set name shown to user
                    Prop.Caption = "Remove Selected library fields: ";
                    //set the tool tip
                    Prop.HelpText = "If you enable this option, MediaFairy will remove the library tags you specify below. You can use this option to delete uneeded/unwanted library tags which other importers may have created.  ";
                    Prop.DefaultValue = Importer.CleanSelectedFields;
                    Prop.DataType = "bool";
                    return true;
                }


                if (Index == counter++)
                {
                    Prop.GroupName = "LibraryCleanup";
                    //set the internal name
                    Prop.Name = "SelectedFieldsToCleanProp";
                    //set name shown to user
                    Prop.Caption = "Which tags to delete? ";
                    //set the tool tip
                    Prop.HelpText = " Please specify here the names of the tags to remove, seperating with a comma (,) ";
                    Prop.DefaultValue = Importer.SelectedFieldsToClean;
                    Prop.DataType = "string";
                    return true;
                }
                #endregion

                #region Media Analyzer overrides
                if (Index == counter++)
                {
                    Prop.GroupName = "AnalyzerOverrides";
                    //set the internal name
                    Prop.Name = "FilmsFolderProp";
                    //set name shown to user
                    Prop.Caption = "(optional) specify films directories: ";
                    Prop.GroupCaption = "                     Media analyzer Overrides: ";
                    //set the tool tip
                    Prop.HelpText = "If the automated media detection setting under 'Media Analyzer' settings group is disabled,\r you will have to specify the root directory under which your films are stored, in order for the plugin to distinguise and import video files of this media type.";
                    Prop.DefaultValue = Importer.FilmsFolders;
                    Prop.DataType = "folderlist";
                    return true;
                }
                if (Index == counter++)
                {
                    Prop.GroupName = "AnalyzerOverrides";
                    //set the internal name
                    Prop.Name = "TvShowsFolderProp";
                    //set name shown to user
                    Prop.Caption = "(optional) specify tv series directories: ";
                    //set the tool tip
                    Prop.HelpText = "If the automated media detection setting (under 'Media Analyzer' settings group) is disabled,\r you will have to specify the root directory under which your tv episodes are stored, in order for the plugin to distinguise and import video files of this media type.";
                    Prop.DefaultValue = Importer.TvShowsFolders;
                    Prop.DataType = "folderlist";
                    return true;
                }
                #endregion


            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }


            return false;
        }

    }
 

    #endregion


    #region Main Importer Class

    public class Importer : IMLImportPlugin, IDisposable
    {
       

        #region IMLImportPlugin Members



        #region Supporting Functions


        public static bool Ping(string hostname)
        {
            Ping pingSender = new Ping();
            PingOptions options = new PingOptions();


            // Create a buffer of 32 bytes of data to be transmitted.
            string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            int timeout = 10000;
            PingReply reply = pingSender.Send(hostname, timeout, buffer, options);


            if (reply.Status == IPStatus.Success)
            {
                pingSender.Dispose();
                return true;
            }
            else
            {
                reply = pingSender.Send(hostname, timeout, buffer, options);
                if (reply.Status == IPStatus.Success)
                {
                    pingSender.Dispose();
                    return true;
                }
                else
                {
                    pingSender.Dispose();
                    return false;
                }
            }

        }


        public ConnectionResult Connect(IMLImportProgress Progress)
        {
            OSoperations os = new OSoperations();
            ConnectionResult connectionresult;

            connectionresult.CanWorkOnline = true;
            connectionresult.OsIsOnline = true;
            connectionresult.loginresult.seconds = 0;
            connectionresult.loginresult.status = "";
            connectionresult.loginresult.token = "";



            #region Internet connection diagnostic

            if (WantConnectionDiagnostics == true)
            {
                Progress.Progress(3, "Detecting internet connection... ");
                Thread.Sleep(1000);
                bool PingResult = false;

                try
                {

                    PingResult = Ping("www.google.com");

                    if (PingResult == false)
                    {
                        Progress.Progress(0, "No internet connection detected. All online featurs disabled. ");
                        Thread.Sleep(1000);
                        connectionresult.CanWorkOnline = false;
                    }
                    else connectionresult.CanWorkOnline = true;


                }
                catch (Exception e)
                {
                    //statusForm.TrayIcon.ShowBalloonTip(1000, "No internet connection detected", "Online information retrieval operations will be postponed.  ", ToolTipIcon.Info);
                    Progress.Progress(0, "No internet connection detected. All online featurs disabled. ");
                    Thread.Sleep(1000);
                    Debugger.Debugger.LogMessageToFile(e.ToString());
                    connectionresult.CanWorkOnline = false;
                }



            }

            #endregion



            #region OSdb Connect

            //Helpers.UpdateProgress(Progress, "Connecting to OSdb... ", "", 5, null);
            Progress.Progress(5, "Connecting to OSdb...");

            try
            {

                if (connectionresult.CanWorkOnline == true)
                {
                    connectionresult.loginresult = os.SiteLogin();

                    Progress.Progress(5, "login status: " + connectionresult.loginresult.status);
                    Thread.Sleep(SleepValue);
                }
                else
                {
                    connectionresult.OsIsOnline = false;
                }

            }
            catch (Exception)
            {
                Progress.Progress(5, "The remote database did not respond. Retrying... ");
                Thread.Sleep(SleepValue);
                try
                {
                    connectionresult.loginresult = os.SiteLogin();
                    Progress.Progress(5, "login status: " + connectionresult.loginresult.status);
                    Thread.Sleep(SleepValue);
                }
                catch (Exception)
                {
                    Progress.Progress(5, "The remote database did not respond. Retrying... ");
                    Thread.Sleep(SleepValue);
                    try
                    {
                        connectionresult.loginresult = os.SiteLogin();
                        Progress.Progress(5, "login status: " + connectionresult.loginresult.status);
                        Thread.Sleep(SleepValue);
                    }
                    catch (Exception)
                    {
                        Progress.Progress(5, "Unable to connect to Opensubtitles.org. ");
                        Thread.Sleep(2000);
                        connectionresult.OsIsOnline = false;
                    }

                }

            }

            #endregion

            return connectionresult;
        }



        public static string IdentifyMovie(IMLItem Item, ConnectionResult connectionresult, ref bool OSisOnline, bool CanWorkOnline, bool FileServerIsOnline, bool IsUNC, string location, string parent, ImportProgress ThisProgress)
        {

            #region function variables
            string name = Item.Name;
            string moviehash = "";
            string imdbid = "";
            try
            {
                moviehash = (string)Item.Tags["VideoHash"];
            }
            catch { }
            try
            {
                imdbid = (string)Item.Tags["ImdbID"];
            }
            catch { }
            #endregion


            //ThisProgress.Progress.Progress(1, "Passed inside IdentifyMovie function... ");
            //Thread.Sleep(1000);


            #region Compute Hash
            if (moviehash == "" || moviehash == null)
            {

                if (EnableHashing == true)
                {
                    if (IsUNC == false || FileServerIsOnline == true)
                    {
                        //ThisProgress.Progress.Progress(100, "why passed here? IsUnc:"+IsUNC +"FileServerOnline: "+ FileServerIsOnline);
                        //Thread.Sleep(2000);

                        //ThisProgress.Progress.Progress(1, "Passed before ComputeHash. ");
                        //Thread.Sleep(1000);

                        if (File.Exists(location))
                        {
                            ThisProgress.Progress.Progress(ThisProgress.CurrentProgress += ThisProgress.ProgressStep, "Computing video fingerprint for " + Item.Name + "...");
                            //Thread.Sleep(200);
                            moviehash = Hasher.ComputeHash(location, Item);
                            Item.Tags["VideoHash"] = moviehash;
                        }
                        else
                        {
                            //ThisProgress.Progress.Progress(ThisProgress.CurrentProgress, location);
                            //Thread.Sleep(SleepValue);

                            ThisProgress.Progress.Progress(ThisProgress.CurrentProgress, "The video file in location field was not found. MediaFairy cannot identify this film.");
                            Thread.Sleep(SleepValue);

                        }


                    }


                }



            }
            else moviehash = (string)Item.Tags["VideoHash"];

            Item.SaveTags();
            #endregion

            Helpers.ReadXmlForFilm(Item, moviehash, parent, ThisProgress);

            #region function variables
            name = Item.Name;
            //string location = Item.Location;
            moviehash = (string)Item.Tags["VideoHash"];
            imdbid = (string)Item.Tags["ImdbID"];
            #endregion


            #region Get ImdBID
            if (OSisOnline == true)
            {
                if (moviehash != null && moviehash != "")
                {
                    if (imdbid == "" || imdbid == null)
                    {

                        #region try to get the imdbid from OSdb
                        try
                        {
                            //Helpers.UpdateProgress(ThisProgress, "Updating Movies Section...", "Identifying Video...", ThisProgress.CurrentProgress += ThisProgress.ProgressStep, Item);
                            ThisProgress.Progress.Progress(ThisProgress.CurrentProgress += ThisProgress.ProgressStep, "Identifying Video..." + name + "...");
                            Thread.Sleep(500);
                            imdbid = OSoperations.FindImdbIDbyHash2(moviehash, Item, connectionresult.loginresult.token, ThisProgress);
                        }
                        catch (Exception)
                        {
                            //MessageBox.Show( e.ToString() );

                            try
                            {
                                ThisProgress.Progress.Progress(ThisProgress.CurrentProgress, "OSdb did not respond. Waiting 5 seconds to retry...");
                                Thread.Sleep(5000);

                                ThisProgress.Progress.Progress(ThisProgress.CurrentProgress, "Retrying...");
                                imdbid = OSoperations.FindImdbIDbyHash2(moviehash, Item, connectionresult.loginresult.token, ThisProgress);
                            }
                            catch (Exception)
                            {
                                //statusForm.TrayIcon.ShowBalloonTip(5000, "Connection to OSdb not possible", "MediaFairy is experiencing difficulties communicating with OpenSubtitles.org database. Please check your connection availability, otherwise the server may be temporarily unreachable.", ToolTipIcon.Info);
                                if (WantToDisableSources)
                                    OSisOnline = false;

                                return null;
                            }
                            //finally
                            //{
                            //    Progress.Progress(cur_progress += prog_step, "Connection error: The Opensubtitles.org server did not respond in a timely fashion");
                            //}

                        }
                        #endregion

                        if (imdbid != "" && imdbid != null)
                        {
                            //ThisProgress.Progress.Progress(ThisProgress.CurrentProgress, "the video was succesfully identified by fingerprint!");
                            //Thread.Sleep(1000);
                        }
                        else return "";


                        Item.Tags["ImdbID"] = imdbid;
                        Item.SaveTags();
                    }
                    else return imdbid;

                }
                else return "";
            }
            #endregion

            return imdbid;
        }
 



        #endregion


        #region Main Importing Functions

        public bool Import(IMLSection Section, IMLImportProgress Progress)
        {

            #region method vars
            string pluginpath = Debugger.Debugger.GetPluginPath();
            //MessageBox.Show(pluginpath);
            DirectoryInfo homePath = new DirectoryInfo(pluginpath);
            FileInfo[] datfiles = homePath.GetFiles("*.dat");

            #endregion 


            #region Allocate Library Sections

            if (WantToImportFilms || UpdateMoviesSection)
            {
                if (Section.Name == FilmsLibrary)
                    MoviesSection = Section;
                else
                    MoviesSection = library.FindSection(FilmsLibrary, true);
            }

            if (WantToImportTvSeries || UpdateTvShowsSection)
            {
                if (Section.Name == TvShowsLibrary)
                    TvSection = Section;
                else
                    TvSection = library.FindSection(TvShowsLibrary, true);

                if (Section.Name == TvSeriesLibrary)
                    TvSeriesSection = Section;
                else
                    TvSeriesSection = library.FindSection(TvSeriesLibrary, true);
            }

            if (WantToImportMusic || UpdateMusicSection)
            {
                if (Section.Name == MusicLibrary)
                    MusicSection = Section;
                else
                    MusicSection = library.FindSection(MusicLibrary, true);
            }

            if (WantToImportBooks || UpdateBooksSection)
            {
                if (Section.Name == BooksLibrary)
                    BooksSection = Section;
                else
                    BooksSection = library.FindSection(BooksLibrary, true);
            }

            IMLSection ReleasesSection;
            if (DownloadFilmReleases)
            {
                if (Section.Name == ReleasesLibrary)
                    ReleasesSection = Section;
                else
                    ReleasesSection = library.FindSection(ReleasesLibrary, true);
            }
            #endregion


            #region General Importer variables

            ImportProgress ThisProgress;
            //string _progressText = "";
            ConnectionResult connectionresult = new ConnectionResult();
            bool OSisOnline = true;
            bool CanWorkOnline = true;
            bool FileServerIsOnline = true;
            bool FileServerChecked = false;
            string[] videoExtensions;
            string[] audioExtensions;
            #endregion


            #region Count Sections Items
            int MoviesItemcount = 0;
            int TvShowsItemcount = 0;
            int MusicItemcount = 0;
            int BooksItemcount = 0;

            try
            {
                MoviesItemcount = MoviesSection.ItemCount;
            }
            catch (Exception)
            {
                MoviesItemcount = 0;
            }
            try
            {
                TvShowsItemcount = TvSection.ItemCount;
            }
            catch (Exception)
            {
                TvShowsItemcount = 0;
            }
            try
            {
                MusicItemcount = MusicSection.ItemCount;
            }
            catch (Exception)
            {
                MusicItemcount = 0;
            }

            try
            {
                BooksItemcount = BooksSection.ItemCount;
            }
            catch (Exception)
            {
                BooksItemcount = 0;
            }

            #endregion


            #region Delete all .dat files if all sections are empty
            if (MoviesItemcount == 0 && MusicItemcount == 0 && TvShowsItemcount == 0)
            {
                foreach (FileInfo datfile in datfiles)
                    File.Delete(datfile.FullName);
            }
            #endregion



            #region Auto File Importer

            if (WantFilesImporting)
            {

                try
                {
                    #region Cache media *.dat files for later manipulation

                    datfiles = homePath.GetFiles("*.dat");

                    System.Collections.ArrayList[] datEntries = new System.Collections.ArrayList[datfiles.Length];

                    if (datfiles.Length != 0)
                    {
                        for (int i = 0; i < datfiles.Length; i++)
                        {
                            datEntries[i] = new System.Collections.ArrayList();
                            //MessageBox.Show(datfiles[i].FullName);
                            string datfile = datfiles[i].FullName;
                            string[] datfileData = File.ReadAllLines(datfile);
                            //string tmp = "";
                            //foreach (string line in datfileData)
                            //{
                            //    tmp = tmp + "" + line;
                            //}
                            //MessageBox.Show(tmp);
                            datEntries[i].Add(datfileData);
                        }
                    }
                    #endregion

                    #region Import films, tv series, and music
                    if (ImportRootFolders != null && ImportRootFolders.Length >= 1)
                    {
                        #region Begin updating Sections
                        if (WantToImportFilms || UpdateMoviesSection)
                            MoviesSection.BeginUpdate();

                        if (WantToImportTvSeries || UpdateTvShowsSection)
                        {
                            TvSection.BeginUpdate();
                            TvSeriesSection.BeginUpdate();
                        }

                        if (WantToImportMusic || UpdateMusicSection)
                            MusicSection.BeginUpdate();

                        if (WantToImportBooks || UpdateBooksSection)
                            BooksSection.BeginUpdate();

                        #endregion


                        #region initialize media extensions arrays
                        System.Collections.ArrayList extensionsToIgnore = new System.Collections.ArrayList();
                        string non_mediaFile = pluginpath + "non-media_extensions.txt";
                        if (!File.Exists(non_mediaFile))
                        {
                            FileStream filestream = File.Create(non_mediaFile);
                            filestream.Close();
                        }

                        extensionsToIgnore.AddRange(File.ReadAllLines(non_mediaFile));


                        if (File.Exists(pluginpath + "video_extensions.txt"))
                            videoExtensions = File.ReadAllLines(pluginpath + "video_extensions.txt");
                        else videoExtensions = null;


                        if (File.Exists(pluginpath + "audio_extensions.txt"))
                            audioExtensions = File.ReadAllLines(pluginpath + "audio_extensions.txt");
                        else audioExtensions = null;
                        #endregion


                        #region Cache Sections for later scanning
                        ThisProgress.CurrentProgress = 0;
                        Progress.Progress(ThisProgress.CurrentProgress, "Caching Films section...");
                        int[] ItemIDs;
                        int itemsCount;
                        string[] filmLocations;
                        string[] TvShowsLocations;
                        string[] MusicLocations;
                        int i = 0;

                        try
                        {
                            ItemIDs = (int[])MoviesSection.GetAllItemIDs();
                        }
                        catch
                        {
                            ItemIDs = null;
                        }
                        if (ItemIDs != null)
                        {
                            itemsCount = ItemIDs.Length;
                            filmLocations = new string[itemsCount];
                            i = 0;
                            foreach (int id in ItemIDs)
                            {
                                IMLItem item = MoviesSection.FindItemByID(id);
                                filmLocations[i] = item.Location;
                                i++;
                            }
                        }
                        else filmLocations = null;

                        Progress.Progress(ThisProgress.CurrentProgress, "Caching Tv shows section...");
                        try
                        {
                            ItemIDs = (int[])TvSection.GetAllItemIDs();
                        }
                        catch
                        {
                            ItemIDs = null;
                        }
                        if (ItemIDs != null)
                        {
                            itemsCount = ItemIDs.Length;
                            TvShowsLocations = new string[itemsCount];
                            i = 0;
                            foreach (int id in ItemIDs)
                            {
                                IMLItem item = TvSection.FindItemByID(id);
                                TvShowsLocations[i] = item.Location;
                                i++;
                            }
                        }
                        else TvShowsLocations = null;

                        //MessageBox.Show("Step 0");

                        #region Cache Music Section
                        Progress.Progress(ThisProgress.CurrentProgress, "Caching Music section...");
                        try
                        {
                            ItemIDs = (int[])MusicSection.GetAllItemIDs();
                        }
                        catch
                        {
                            ItemIDs = null;
                        }
                        if (ItemIDs != null)
                        {
                            itemsCount = ItemIDs.Length;
                            MusicLocations = new string[itemsCount];
                            i = 0;
                            foreach (int id in ItemIDs)
                            {
                                IMLItem item = MusicSection.FindItemByID(id);
                                MusicLocations[i] = item.Location;
                                i++;
                            }
                        }
                        else MusicLocations = null;
                        #endregion

                        #endregion


                        #region Read Sections Snapshots
                        //Progress.Progress(ThisProgress.CurrentProgress, "Reading media sections snapshots...");
                        //string[] MoviesSnapshot = File.ReadAllLines(pluginpath + "Movies.section");
                        //string[] SeriesSnapshot = File.ReadAllLines(pluginpath + "Series.section");
                        //string[] MusicSnapshot = File.ReadAllLines(pluginpath + "Music.section");
                        #endregion


                        #region Compare Sections snapshot to detect items to re-import
                        //    if (filmLocations != null && MoviesSnapshot != null)
                        //    {
                        //    Progress.Progress(ThisProgress.CurrentProgress, "Updating Media Snaphots...");
                        //    foreach (string oldEntry in MoviesSnapshot)
                        //    {
                        //        bool filefound = false;
                        //         foreach (string newEntry in filmLocations)
                        //         {
                        //             if (newEntry == oldEntry)
                        //                 filefound = true;
                        //         }

                        #region if entry was not found in the new section, delete the missing entry from .dat files.
                        //         if (!filefound)
                        //         {
                        //             //MessageBox.Show(Convert.ToString(datEntries.Length));
                        #region Scan and remove missing entries from .dat files
                        //             for (int x=0; x< datEntries.Length; x++)
                        //             {
                        //                 //MessageBox.Show(Convert.ToString(datentries.Count));
                        //                  int y = 0;
                        //                 foreach (string line in (string[])datEntries[x][0])
                        //                 {
                        //                     if ( line == oldEntry)
                        //                     {
                        //                         datEntries[x].RemoveAt(y);
                        //                     }
                        //                   y++;
                        //                 }


                        //             }
                        #endregion


                        //             #region Re-write the .dat files with the updated snapshot
                        //             for (i = 0; i < datfiles.Length; i++)
                        //             {
                        //                 string[] tmp = (string[])datEntries[i][0];
                        //                 File.WriteAllLines(datfiles[i].FullName, tmp );
                        //             }
                        #endregion



                        //         }
                        #endregion

                        //    }
                        //}



                        //    #endregion


                        #region loop root folders and Import Files
                        foreach (string ImportRootFolder in ImportRootFolders)
                        {
                            Progress.Progress(ThisProgress.CurrentProgress, "Validating root directory...");
                            if (Directory.Exists(ImportRootFolder))
                            {
                                //MessageBox.Show("Step 2");
                                //DirectoryInfo ImportRootFolderDI = new DirectoryInfo(ImportRootFolder);
                                //ImportRootFolderDI.
                                int CurrentProgress = 0;
                                int ImportedItems = 0;
                                #region File Importer varibales
                                FileInfo[] Allfiles;
                                int item_timeframe = 0;
                                ThisProgress.CurrentProgress = 0;
                                ThisProgress.ProgressStep = item_timeframe / 5;
                                ThisProgress.Progress = Progress;
                                ThisProgress.GeneralOperation = "Importing Media files";
                                ThisProgress.SpecialOperation = "";
                                int HowManyItemsBeforeUpdatingProgress = 0;
                                int ProgressStep = 0;
                                #endregion

                                try
                                {
                                    Application.DoEvents();

                                    #region scan directories in root and build subDirectories and AllFiles arrays.
                                    DirectoryInfo rootDirectory = new DirectoryInfo(ImportRootFolder);

                                    Progress.Progress(ThisProgress.CurrentProgress, "Calculating remaining time...");
                                    DirectoryInfo[] subDirectories = rootDirectory.GetDirectories("*.*", SearchOption.AllDirectories);
                                    int dirsTotal = subDirectories.Length;

                                    int dirCounter = 0;
                                    Helpers.ExamineDir(ImportRootFolder, out Allfiles, Progress, dirsTotal, ref dirCounter);
                                    #endregion

                                    Application.DoEvents();


                                    #region new files differentiator

                                    string datName = rootDirectory.Name;
                                    datName = datName.TrimEnd('\\');
                                    datName = datName.TrimEnd(':');
                                    string datPath = pluginpath + datName + ".dat";

                                    if (File.Exists(datPath))
                                    {

                                        #region compare snapshot and create Files list.
                                        Progress.Progress(0, "Loading filesystem image...");
                                        #region scan folders and construct filepaths array
                                        //FileInfo[] Allfiles = rootDirectory.GetFiles("*.*", SearchOption.AllDirectories);
                                        string[] filePaths = new string[Allfiles.Length];
                                        for (i = 0; i < Allfiles.Length; i++)
                                        {
                                            try
                                            {
                                                filePaths[i] = Allfiles[i].FullName;
                                            }
                                            catch (Exception)
                                            {
                                            }

                                        }
                                        #endregion

                                        Progress.Progress(0, "Reading filesystem snapshot...");
                                        #region read .dat file and store to oldfilePaths
                                        string[] oldfilePaths = File.ReadAllLines(datPath);
                                        #endregion

                                        Progress.Progress(0, "Refreshing snapshot...");
                                        #region write new .dat file with the new paths.
                                        //MessageBox.Show(@"D:\" + rootDirectory.Name + ".dat");
                                        File.WriteAllLines(datPath, filePaths);
                                        #endregion

                                        Progress.Progress(0, "Comparing media snapshots...");
                                        #region compare old filepaths with new
                                        FileInfo[] FilesInDir = new FileInfo[filePaths.Length];
                                        for (i = 0; i < filePaths.Length; i++)
                                        {
                                            bool matchfound = false;
                                            for (int j = 0; j < oldfilePaths.Length; j++)
                                            {
                                                if (filePaths[i] == oldfilePaths[j])
                                                {
                                                    matchfound = true;
                                                }
                                            }

                                            if (!matchfound)
                                            {
                                                try
                                                {
                                                    if (File.Exists(filePaths[i]))
                                                    {
                                                        FilesInDir[i] = new FileInfo(filePaths[i]);
                                                    }
                                                }
                                                catch (Exception e)
                                                {
                                                    MessageBox.Show(e.ToString());
                                                }
                                            }

                                        }
                                        #endregion
                                        #endregion

                                        Application.DoEvents();

                                        #region files importing limiter

                                        int importing_count = 0;
                                        bool LimitFilesToImport = false;

                                        if (importing_limit == 0)
                                        {
                                            LimitFilesToImport = false;
                                        }
                                        else LimitFilesToImport = true;

                                        #endregion


                                        #region compute ProgressStep
                                        if (Allfiles.Length >= 100)
                                        {
                                            HowManyItemsBeforeUpdatingProgress = (Allfiles.Length / 100);
                                            //MessageBox.Show(Convert.ToString( HowManyItemsBeforeUpdatingProgress) );

                                            ProgressStep = 1;
                                        }
                                        else
                                        {
                                            if (Allfiles.Length > 0)
                                                ProgressStep = 100 / Allfiles.Length;
                                        }
                                        #endregion

                                        if (FilesInDir.Length > 0)
                                        {
                                            int j = 0;
                                            foreach (FileInfo file in FilesInDir)
                                            {
                                                Application.DoEvents();
                                                if (!Helpers.ImportMediaFile(file, MoviesSection, TvSection, MusicSection, BooksSection, Progress, 5, ref ImportedItems, ref  importing_count, ref importing_limit, ref ThisProgress.ProgressStep, ref HowManyItemsBeforeUpdatingProgress, ref extensionsToIgnore, ref LimitFilesToImport, ref filmLocations, ref TvShowsLocations, ref MusicLocations, videoExtensions, audioExtensions, FilesInDir.Length, j))
                                                {
                                                    if (Importer.UpdateMoviesSection)
                                                        MoviesSection.EndUpdate();
                                                    if (Importer.UpdateTvShowsSection)
                                                    {
                                                        TvSection.EndUpdate();
                                                        TvSeriesSection.EndUpdate();
                                                    }
                                                    if (Importer.UpdateMusicSection)
                                                        MusicSection.EndUpdate();

                                                    if (Importer.UpdateBooksSection)
                                                        BooksSection.EndUpdate();

                                                    Progress.Progress(100, "All operations were cancelled. Completed jobs were save to library.");
                                                    //Helpers.UpdateProgress(ThisProgress, "All operations were cancelled. Completed jobs were save to library.", "", 100, null);
                                                    //Progress.Progress(ThisProgress.CurrentProgress, "You canceled MediaFairy. Completed jobs were saved to library.");
                                                    //Importer.statusForm.Hide();
                                                    //Importer.statusForm.TrayIcon.Dispose();
                                                    Application.Exit();
                                                    Progress = null;
                                                    return true;
                                                }

                                                i++;
                                            }
                                        }


                                    }
                                    else
                                    {
                                        //statusForm.TrayIcon.ShowBalloonTip(10000, "Constructing media snapshot", "MediaFairy is now analyzing your media files in order to create a filesystem snapshot. This operation needs to be performed only once for each new media root directory, and will enable the application to detect and import only new files in all future media importing requests.", ToolTipIcon.Info);
                                        Progress.Progress(0, "Scanning folders...");
                                        Application.DoEvents();

                                        #region scan folders and construct filepaths array
                                        //FileInfo[] Allfiles = FilesInDir;
                                        //FileInfo[] Allfiles = rootDirectory.GetFiles("*.*", SearchOption.AllDirectories);
                                        string[] filePaths = new string[Allfiles.Length];
                                        for (i = 0; i < Allfiles.Length; i++)
                                        {
                                            try
                                            {
                                                filePaths[i] = Allfiles[i].FullName;
                                            }
                                            catch (Exception)
                                            {
                                            }

                                        }
                                        #endregion

                                        Progress.Progress(0, "Saving snapshot...");
                                        Application.DoEvents();
                                        #region write new .dat file with the new paths.
                                        //MessageBox.Show(datPath);
                                        File.WriteAllLines(datPath, filePaths);
                                        #endregion


                                        #region files importing limiter

                                        int importing_count = 0;
                                        bool LimitFilesToImport = false;

                                        if (importing_limit == 0)
                                        {
                                            LimitFilesToImport = false;
                                        }
                                        else LimitFilesToImport = true;

                                        #endregion

                                        #region compute ProgressStep
                                        if (Allfiles.Length >= 100)
                                        {
                                            HowManyItemsBeforeUpdatingProgress = (Allfiles.Length / 100);
                                            //MessageBox.Show(Convert.ToString( HowManyItemsBeforeUpdatingProgress) );

                                            ProgressStep = 1;
                                        }
                                        else
                                        {
                                            if (Allfiles.Length != 0)
                                                ProgressStep = 100 / Allfiles.Length;
                                        }
                                        #endregion


                                        #region loop through sub-directories and import media files
                                        try
                                        {
                                            Application.DoEvents();
                                            int CurrentFile = 0;

                                            if (!Helpers.ImportDir(ImportRootFolder, Progress, CurrentProgress, MoviesSection, TvSection, MusicSection, BooksSection, ref ImportedItems, ref importing_count, ref importing_limit, ref ProgressStep, ref HowManyItemsBeforeUpdatingProgress, extensionsToIgnore, ref LimitFilesToImport, ref filmLocations, ref TvShowsLocations, ref MusicLocations, videoExtensions, audioExtensions, Allfiles.Length, ref CurrentFile))
                                            {
                                                if (Importer.UpdateMoviesSection)
                                                    MoviesSection.EndUpdate();
                                                if (Importer.UpdateTvShowsSection)
                                                {
                                                    TvSection.EndUpdate();
                                                    TvSeriesSection.EndUpdate();
                                                }
                                                if (Importer.UpdateMusicSection)
                                                    MusicSection.EndUpdate();

                                                if (Importer.UpdateBooksSection)
                                                    BooksSection.EndUpdate();

                                                //Helpers.UpdateProgress(ThisProgress, "All operations were cancelled. Completed jobs were save to library.", "", 100, null);
                                                Progress.Progress(100, "All operations were cancelled. Completed jobs were saved to library.");
                                                //Importer.statusForm.Hide();
                                                //Importer.statusForm.TrayIcon.Dispose();
                                                Application.Exit();
                                                Progress = null;
                                                return true;
                                            }

                                        }
                                        catch (Exception e)
                                        {
                                            Progress.Progress(0, e.Message);
                                            Thread.Sleep(2000);
                                            MessageBox.Show(e.ToString());
                                            Debugger.Debugger.LogMessageToFile(e.ToString());
                                        }
                                        #endregion

                                    }
                                    #endregion


                                }
                                catch (Exception e)
                                {
                                    MessageBox.Show(e.ToString());

                                    Debugger.Debugger.LogMessageToFile(e.ToString());

                                }

                            }

                        }
                        #endregion


                        #region Re-cache sections for snapshots updating
                        //ThisProgress.CurrentProgress = 0;
                        //Progress.Progress(ThisProgress.CurrentProgress, "Caching Films section...");
                        //i = 0;

                        //try
                        //{
                        //    ItemIDs = (int[])MoviesSection.GetAllItemIDs();
                        //}
                        //catch
                        //{
                        //    ItemIDs = null;
                        //}
                        //if (ItemIDs != null)
                        //{
                        //    itemsCount = ItemIDs.Length;
                        //    filmLocations = new string[itemsCount];
                        //    i = 0;
                        //    foreach (int id in ItemIDs)
                        //    {
                        //        IMLItem item = MoviesSection.FindItemByID(id);
                        //        filmLocations[i] = item.Location;
                        //        i++;
                        //    }
                        //}
                        //else filmLocations = null;

                        //Progress.Progress(ThisProgress.CurrentProgress, "Caching Tv shows section...");
                        //try
                        //{
                        //    ItemIDs = (int[])TvSection.GetAllItemIDs();
                        //}
                        //catch
                        //{
                        //    ItemIDs = null;
                        //}
                        //if (ItemIDs != null)
                        //{
                        //    itemsCount = ItemIDs.Length;
                        //    TvShowsLocations = new string[itemsCount];
                        //    i = 0;
                        //    foreach (int id in ItemIDs)
                        //    {
                        //        IMLItem item = TvSection.FindItemByID(id);
                        //        TvShowsLocations[i] = item.Location;
                        //        i++;
                        //    }
                        //}
                        //else TvShowsLocations = null;

                        ////MessageBox.Show("Step 0");

                        //#region Cache Music Section
                        //Progress.Progress(ThisProgress.CurrentProgress, "Caching Music section...");
                        //try
                        //{
                        //    ItemIDs = (int[])MusicSection.GetAllItemIDs();
                        //}
                        //catch
                        //{
                        //    ItemIDs = null;
                        //}
                        //if (ItemIDs != null)
                        //{
                        //    itemsCount = ItemIDs.Length;
                        //    MusicLocations = new string[itemsCount];
                        //    i = 0;
                        //    foreach (int id in ItemIDs)
                        //    {
                        //        IMLItem item = MusicSection.FindItemByID(id);
                        //        MusicLocations[i] = item.Location;
                        //        i++;
                        //    }
                        //}
                        //else MusicLocations = null;
                        //#endregion

                        #endregion


                        #region Write Sections Snapshots
                        //Progress.Progress(ThisProgress.CurrentProgress, "Updating Sections Snapshots...");

                        //if (filmLocations != null)
                        //File.WriteAllLines(pluginpath + "Movies.section", filmLocations);
                        //if (TvShowsLocations != null)
                        //File.WriteAllLines(pluginpath + "Series.section", TvShowsLocations);
                        //if (MusicLocations != null)
                        //File.WriteAllLines(pluginpath + "Music.section", MusicLocations);
                        #endregion


                        #region Finish Upating Sections
                        if (WantToImportFilms || UpdateMoviesSection)
                            MoviesSection.EndUpdate();

                        if (WantToImportTvSeries || UpdateTvShowsSection)
                        {
                            TvSection.EndUpdate();
                            TvSeriesSection.EndUpdate();
                        }

                        if (WantToImportMusic || UpdateMusicSection)
                            MusicSection.EndUpdate();

                        if (WantToImportBooks || UpdateBooksSection)
                            BooksSection.EndUpdate();
                        #endregion


                        File.WriteAllLines(pluginpath + "non-media_extensions.txt", (string[])extensionsToIgnore.ToArray(typeof(string)));

                    }
                    #endregion
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                }




                //#region import PC Games
                //if (WantToImportPcGames && !String.IsNullOrEmpty(PcGamesFolderStr))
                //{
                //    ThisProgress.Progress = Progress;
                //    ThisProgress.GeneralOperation = "Updating Games Section";
                //    ThisProgress.SpecialOperation = "";

                //    DirectoryInfo PcGamesFolder = new DirectoryInfo(PcGamesFolderStr);
                //    FileInfo[] shortcuts = PcGamesFolder.GetFiles("*.lnk", SearchOption.AllDirectories);
                //    IMLSection GamesSection = library.FindSection("PcGames", true);

                //    GamesSection.BeginUpdate();
                //    int itemCounter = 0;
                //    foreach (FileInfo shortcut in shortcuts)
                //    {
                //        ThisProgress.ProgressStep = 100 / shortcuts.Length;
                //        ThisProgress.CurrentProgress = 100 - (ThisProgress.ProgressStep * (shortcuts.Length - itemCounter));

                //        Helpers.ImportPcGame(shortcut.FullName, GamesSection, ThisProgress);
                //        itemCounter++;
                //    }
                //    GamesSection.EndUpdate();



                //    GamesSection.BeginUpdate();
                //    itemCounter = 0;
                //    int[] AllItemIDs = (int[])GamesSection.GetAllItemIDs();
                //    foreach (int id in AllItemIDs)
                //    {
                //        IMLItem item = GamesSection.FindItemByID(id);
                //        ThisProgress.ProgressStep = 100 / AllItemIDs.Length;
                //        ThisProgress.CurrentProgress = 100 - (ThisProgress.ProgressStep * (AllItemIDs.Length - itemCounter));
                //        Helpers.DownloadGameInfo(item, ThisProgress);
                //        Helpers.UserCancels(Progress, ThisProgress, MoviesSection, TvSection, TvSeriesSection, MusicSection, BooksSection, item);

                //        itemCounter++;
                //    }
                //    GamesSection.EndUpdate();



                //}
                //#endregion


            }

            #endregion



            #region iTunes % iPod

            iTunesLib.IiTunes iTunesApp;
            iTunesLib.IITLibraryPlaylist playlist;
            iTunesLib.IITTrackCollection TrackCollection;


            if (WantItunesSync && UpdateMusicSection)
            {
                try
                {
                    #region Launch itunes

                    Progress.Progress(10, "Launching iTunes...");
                    iTunesApp = new iTunesLib.iTunesAppClass();

                    Thread.Sleep(5000);

                    iTunesLib.IITWindowCollection windows = iTunesApp.Windows;
                    foreach (iTunesLib.IITWindow window in windows)
                    {

                        if (window.Name == "iTunes")
                        {
                            window.Minimized = true;
                        }

                    }

                    #endregion


                    #region iTunes Synchronizer




                    #region sync libraries


                    playlist = iTunesApp.LibraryPlaylist;
                    TrackCollection = playlist.Tracks;



                    MusicSection.BeginUpdate();
                    try
                    {

                        foreach (iTunesLib.IITTrack track in TrackCollection)
                        {

                            if (track.Kind == iTunesLib.ITTrackKind.ITTrackKindFile)
                            {
                                iTunesLib.IITPlaylist trackPlaylist = track.Playlist;
                                //MessageBox.Show(trackPlaylist.Name);
                                //iTunesLib.IITPlaylist track_playlist = track.Playlist;
                                //MessageBox.Show("Track playlist: " + track_playlist.Name);
                                iTunesLib.IITFileOrCDTrack file = (iTunesLib.IITFileOrCDTrack)track;

                                if (file.Location != "" && file.Location != null)
                                {

                                    FileInfo fiB = new FileInfo(file.Location);

                                    //MessageBox.Show(fiB.Extension);

                                    #region check to make sure that the file is not video...
                                    if (fiB.Extension != ".m4v")
                                    {

                                        IMLItem Item = MusicSection.FindItemByLocation(file.Location);

                                        if (Item == null && file.Podcast == false)
                                        {
                                            Progress.Progress(10, "Adding audio track " + track.Name + "to Meedio music library...");
                                            Item = MusicSection.AddNewItem(track.Name, file.Location);


                                            #region write item tags

                                            Item.Tags["Artist"] = track.Artist;
                                            Item.Tags["Album"] = track.Album;
                                            Item.Tags["Title"] = track.Name;
                                            Item.Tags["Genre"] = track.Genre;
                                            if (track.TrackNumber != 0)
                                                Item.Tags["Track"] = track.TrackNumber;
                                            if (track.Year != 0)
                                                Item.Tags["Year"] = track.Year;

                                            try
                                            {
                                                if (file.Lyrics != null && file.Lyrics != "")
                                                    Item.Tags["Lyrics"] = file.Lyrics;
                                            }
                                            catch (Exception)
                                            { }

                                            if (track.Rating != 0)
                                                Item.Tags["Rating"] = Convert.ToString(track.Rating);
                                            if (track.SampleRate != 0)
                                                Item.Tags["SampleRate"] = track.SampleRate;
                                            if (track.BitRate != 0)
                                                Item.Tags["BitRate"] = track.BitRate;
                                            if (track.Duration != 0)
                                                Item.Tags["Duration"] = track.Duration;

                                            #endregion


                                            #region save artwork to file and add to imagefield

                                            try
                                            {
                                                iTunesLib.IITArtworkCollection artworks = track.Artwork;
                                                foreach (iTunesLib.IITArtwork artwork in artworks)
                                                {
                                                    FileInfo fi = new FileInfo(file.Location);
                                                    DirectoryInfo di = fi.Directory;
                                                    string trackDirectory = di.FullName;

                                                    string artworkPath = trackDirectory + @"\" + "folder.jpg";
                                                    //MessageBox.Show(artworkPath);
                                                    artwork.SaveArtworkToFile(artworkPath);
                                                    Item.ImageFile = artworkPath;
                                                }
                                            }
                                            catch (Exception)
                                            { }

                                            #endregion

                                            Item.SaveTags();
                                        }

                                    }
                                    #endregion

                                }

                            }

                        }


                    }
                    catch (Exception e)
                    {
                        MusicSection.EndUpdate();
                        MessageBox.Show(e.ToString());
                    }

                    MusicSection.EndUpdate();

                    #endregion


                    #endregion



                    #region manage and Update iPod

                    //iTunesApp.UpdateIPod();



                    iTunesLib.IITSourceCollection sources = iTunesApp.Sources;

                    foreach (iTunesLib.IITSource source in sources)
                    {

                        //MessageBox.Show(source.Name);


                        if (source.Kind == iTunesLib.ITSourceKind.ITSourceKindIPod)
                        {
                            object iSource = source;

                            //iTunesLib.IITLibraryPlaylist MyPlaylist =(iTunesLib.IITLibraryPlaylist) iTunesApp.CreatePlaylistInSource("MyPlaylist", ref iSource);


                            //MyPlaylist.AddFile("D:\test.mp3");



                            //iTunesLib.IITPlaylistCollection iPodPlaylists = source.Playlists;

                            //foreach (iTunesLib.IITPlaylist ipodPlaylist in iPodPlaylists)
                            //{
                            //    //MessageBox.Show("iPod library name:" + ipodlibrary.Name);
                            //    //MessageBox.Show("Duration: " + Convert.ToString(ipodlibrary.Duration));

                            //    if (ipodPlaylist.Kind == iTunesLib.ITPlaylistKind.ITPlaylistKindLibrary)
                            //    {
                            //        iTunesLib.IITLibraryPlaylist iPodLibrary = (iTunesLib.IITLibraryPlaylist)ipodPlaylist;


                            //        iPodLibrary.AddFile("D:\test.mp3");

                            //    }

                            //}


                        }


                        //foreach (iTunesLib.IITLibraryPlaylist ipodlibrary in ipodPlaylists)
                        //{
                        //    MessageBox.Show("Passed 4");
                        //    MessageBox.Show(Convert.ToString(ipodlibrary.Duration));
                        //}


                    }

                    #endregion

                }
                catch (Exception)
                {

                }

            }


            #endregion



            #region Main Thread

            try
            {

                #region IMDb grabber variables
                IMDbOperations imdbOP = new IMDbOperations();
                if (AssistType == "When no exact match found") imdbOP._assistType = IMDbOperations.AssistType.NoPerfectMatch;
                if (AssistType == "Never") imdbOP._assistType = IMDbOperations.AssistType.Never;
                if (AssistType == "Skip identifying unknown films") imdbOP._assistType = IMDbOperations.AssistType.SkipUnknownItems;
                if (AssistType == "Always") imdbOP._assistType = IMDbOperations.AssistType.Always;
                imdbOP._useFirst = UseFirst;
                imdbOP._useLatest = UseLatest;
                imdbOP._useMostPopular = UseMostPopular;
                //IMDbLib lib = new IMDbLib("USA");
                #endregion


                #region Connect
                if ((MoviesItemcount != 0 && UpdateMoviesSection) || (TvShowsItemcount != 0 && UpdateTvShowsSection))
                {
                    connectionresult = Connect(Progress);

                    if (connectionresult.CanWorkOnline == false)
                        CanWorkOnline = false;

                    if (connectionresult.OsIsOnline == false)
                        OSisOnline = false;



                }
                #endregion


                #region Sections Updaters



                #region Movies Section Updater

                if (FilmsLibrary == "")
                    UpdateMoviesSection = false;

                if (UpdateMoviesSection)
                {

                    if (MoviesSection.ItemCount != 0)
                    {

                        #region Movies Section varibales
                        IMLItemList AllFilmItems;
                        int ItemCount = MoviesSection.ItemCount;
                        int CompletedCount = 0;
                        int item_timeframe = 100 / ItemCount;
                        ThisProgress.CurrentProgress = 0;
                        ThisProgress.ProgressStep = item_timeframe / 5;
                        ThisProgress.Progress = Progress;
                        ThisProgress.GeneralOperation = "Updating Movies Section";
                        ThisProgress.SpecialOperation = "";

                        if (SleepValue > 3000) SleepValue = 3000;

                        #endregion


                        //Helpers.UpdateProgress(ThisProgress, "Preparing to update films Section", "", 0, null);
                        ThisProgress.Progress.Progress(0, "Preparing to update films Section...");
                        Thread.Sleep(1000);

                        #region Decide selective updating
                        bool SelectiveUpdating = false;
                        foreach (int id in (int[])MoviesSection.GetAllItemIDs())
                        {
                            IMLItem Item = MoviesSection.FindItemByID(id);
                            if (Item.Tags["SelectiveUpdating"] != null && (string)Item.Tags["SelectiveUpdating"] != "")
                                SelectiveUpdating = true;
                        }
                        #endregion


                        #region Download film releases
                        //MessageBox.Show("DownloadFilmReleases: " + Convert.ToString(DownloadFilmReleases));
                        //MessageBox.Show("SelectiveUpdating: " + Convert.ToString(SelectiveUpdating));
                        if (DownloadFilmReleases && !SelectiveUpdating)
                        {

                            //MessageBox.Show("Passed releases cases");
                            ReleasesSection = library.FindSection(ReleasesLibrary, true);


                            FilmsRssFeeds = File.ReadAllLines(pluginpath + "RSS Sources.txt");

                            foreach (string rssSource in FilmsRssFeeds)
                            {
                                //MessageBox.Show("Found rss source!");
                                if (!String.IsNullOrEmpty(rssSource))
                                    Helpers.DownloadNewReleases(rssSource, MoviesSection, ReleasesSection, connectionresult, ThisProgress);
                            }

                        }
                        #endregion


                        if (!SelectiveUpdating && EnableChainLinking)
                        {
                            AllFilmItems = MoviesSection.SearchAll("*");
                        }
                        else AllFilmItems = null;



                        #region Loop Movies Section items

                        MoviesSection.BeginUpdate();
                        foreach (int id in (int[])MoviesSection.GetAllItemIDs())
                        {

                            #region item variables
                            IMLItem Item = MoviesSection.FindItemByID(id);

                            if (Item == null)
                                continue;

                            string moviehash = "";
                            string imdbid = "";

                            try
                            {
                                moviehash = (string)Item.Tags["VideoHash"];
                            }
                            catch { }

                            try
                            {
                                imdbid = (string)Item.Tags["ImdbID"];
                            }
                            catch {}

                            string Year = "";
                            string itemTitle = "";
                            string SortTitle = "";


                            bool IsMultipart = false;
                            string location = Item.Location;
                            string[] multipart = null;
                            bool IsUNC = false;
                            string HasDetails = "";
                            FileInfo fi;
                            DirectoryInfo parent;
                            DirectoryInfo root;
                            string VideoFilename;
                            bool IsExactMatched = false;

                            Helpers.DetectMultipart(ref location, ref IsMultipart, ref multipart);


                            //MessageBox.Show(location);
                            #region store path locations

                            try
                            {
                                fi = new FileInfo(location);
                                parent = fi.Directory;
                                root = parent.Root;
                                VideoFilename = fi.Name;
                                VideoFilename = VideoFilename.Substring(0, VideoFilename.Length - 4);
                            }
                            catch (Exception)
                            {
                                //statusForm.TrayIcon.ShowBalloonTip(5000, "Invalid item location", "The item's " + Item.Name + " location path is invalid. Please consider correcting it. MediaFairy will skip this item to avoid unexpected operations.", ToolTipIcon.Warning);
                                //Thread.Sleep(5000);
                                continue;
                            }
                            #endregion

                            ThisProgress.CurrentProgress = 6 + (item_timeframe * CompletedCount);

                            #endregion


                            #region Pre-updating diagnostics
                            if (!SelectiveUpdating)
                            {

                                Debugger.Debugger.LogMessageToFile("Srarting to work with library item: " + Item.Name + " with ID: " + id);

                                Application.DoEvents();

                                Helpers.CheckFileServer(WantFileserverDiagnostics, location, ref IsUNC, ref FileServerChecked, Progress, ThisProgress, root.FullName, ref FileServerIsOnline);

                                Application.DoEvents();

                                if (Helpers.DeleteMissingItem(DeleteMissing, IsUNC, FileServerIsOnline, location, MoviesSection, Item, ThisProgress))
                                    continue;

                                Application.DoEvents();

                                if (Helpers.UserCancels(Progress, ThisProgress, MoviesSection, TvSection, TvSeriesSection, MusicSection, null, Item))
                                    return true;

                                Helpers.FileIsInUse(location, FileServerIsOnline, Item, ThisProgress);

                            }
                            #endregion


                            #region Identifiers

                            if (!SelectiveUpdating)
                            {
                                if (Helpers.ExtractMetadataDetectEpisode(ThisProgress, Item, location, ref imdbid, ref imdbOP, MoviesSection, TvSection, false))
                                    continue;

                                #region Clean name and set OriginalTitle
                                string ItemName = Item.Name;
                                if (ItemName != null)
                                {
                                    ItemName = ItemName.Replace('_', ' ');
                                    ItemName = ItemName.Replace('.', ' ');
                                    Item.Name = ItemName;
                                    string tmp = "";
                                    try
                                    {
                                       tmp = (string)Item.Tags["OriginalTitle"];
                                    }
                                    catch { }

                                    if (String.IsNullOrEmpty(tmp))
                                        Item.Tags["OriginalTitle"] = Item.Name;

                                    //Item.Tags["Title"] = Item.Name;
                                    Item.SaveTags();
                                }
                                #endregion

                                if (Helpers.UserCancels(Progress, ThisProgress, MoviesSection, TvSection, TvSeriesSection, MusicSection, null, Item))
                                    return true;


                                #region retrieve imdbid from .nfo file
                                if (Item.Tags["ImdbID"] == null || (string)Item.Tags["ImdbID"] == "")
                                {
                                    if (FileServerIsOnline || !IsUNC)
                                    {
                                        bool DirectoryIsRoot = false;
                                        foreach (string rootFolder in Importer.ImportRootFolders)
                                        {
                                            if (rootFolder == parent.FullName)
                                                DirectoryIsRoot = true;
                                        }


                                        FileInfo[] nfoFiles = parent.GetFiles("*.nfo", SearchOption.TopDirectoryOnly);
                                        if (nfoFiles.Length > 1)
                                            DirectoryIsRoot = true;

                                        if (File.Exists(pluginpath + "video_extensions.txt"))
                                        {
                                            videoExtensions = File.ReadAllLines(pluginpath + "video_extensions.txt");
                                            int videoFilesCount = 0;
                                            foreach (string extension in videoExtensions)
                                            {
                                                FileInfo[] files = parent.GetFiles(extension, SearchOption.TopDirectoryOnly);
                                                foreach (FileInfo file in files)
                                                {
                                                    if (!file.Name.Contains("sample"))
                                                        videoFilesCount++;
                                                }
                                            }

                                            if (videoFilesCount > 2)
                                                DirectoryIsRoot = true;
                                        }


                                        if (!DirectoryIsRoot)
                                        {
                                            #region search directory for .nfo files
                                            foreach (FileInfo file in parent.GetFiles("*.nfo", SearchOption.TopDirectoryOnly))
                                            {
                                                try
                                                {
                                                    StreamReader reader;
                                                    reader = file.OpenText();
                                                    string text = reader.ReadToEnd();
                                                    Regex regex = new Regex("title/(.*)/");
                                                    MatchCollection matches = regex.Matches(text);
                                                    foreach (Match match in matches)
                                                    {
                                                        imdbid = match.Groups[1].Value;
                                                        //MessageBox.Show("imdbid from nfo: " + imdbid);
                                                        Item.Tags["ImdbID"] = imdbid;
                                                        Item.SaveTags();
                                                    }

                                                }
                                                catch (Exception e)
                                                {
                                                    MessageBox.Show(e.ToString());
                                                }
                                            }
                                            #endregion
                                        }

                                    }
                                }
                                #endregion

                                Application.DoEvents();

                                imdbOP._imdbMovie = null;
                                imdbOP._imdbID = imdbid;

                                try
                                {
                                    Year = (string)Item.Tags["Year"];
                                }
                                catch { }

                                if (!String.IsNullOrEmpty( Year) || !String.IsNullOrEmpty(imdbOP._imdbID))
                                    IsExactMatched = true;


                                if (!Helpers.IdentifyVideo(ref imdbid, ref imdbOP, Item, OSisOnline, CanWorkOnline, FileServerIsOnline, IsUNC, location, parent.FullName, Year, ThisProgress, connectionresult, MoviesSection))
                                    continue;

                                #region Upload video hash to OSdb
                                if (EnableHashUploading)
                                {
                                    if (Item.Tags["OSdbUpdated"] == null || (string)Item.Tags["OSdbUpdated"] != "true")
                                    {
                                        if (IsExactMatched || imdbOP._assistType != IMDbOperations.AssistType.Never)
                                        {
                                            if (Helpers.UploadVideoHash(Item, location, imdbid, connectionresult, FileServerIsOnline, IsUNC, ThisProgress))
                                            {
                                                Item.Tags["OSdbUpdated"] = "true";
                                                Item.SaveTags();
                                            }
                                        }
                                    }
                                }
                                #endregion


                                try
                                {
                                    itemTitle = (string)Item.Tags["Title"];
                                }
                                catch { }


                                #region Set SortTitle
                                if (!String.IsNullOrEmpty(itemTitle))
                                {
                                    if (itemTitle.StartsWith("The"))
                                        SortTitle = itemTitle.Remove(0, 4) + ", The ";
                                    else SortTitle = itemTitle;

                                    Item.Tags["SortTitle"] = SortTitle;
                                    Item.SaveTags();
                                }
                                #endregion

                                if (Helpers.UserCancels(Progress, ThisProgress, MoviesSection, TvSection, TvSeriesSection, MusicSection, null, Item))
                                    return true;
                            }
                            #endregion


                            #region extract important fields for online searches


                            try
                            {
                                moviehash = (string)Item.Tags["VideoHash"];
                            }
                            catch { }

                            string imdbidTmp = "";
                            try
                            {
                                imdbidTmp = (string)Item.Tags["ImdbID"];
                            }
                            catch { }


                            if ( !String.IsNullOrEmpty(imdbidTmp) )
                            {
                                imdbid = (string)Item.Tags["ImdbID"];
                                imdbOP._imdbID = (string)Item.Tags["ImdbID"];
                            }
                            else
                            {

                                if (!String.IsNullOrEmpty(imdbOP._imdbID))
                                    imdbid = imdbOP._imdbID;
                                else imdbOP._imdbID = imdbid;

                                Item.Tags["ImdbID"] = imdbOP._imdbID;
                            }

                            //MessageBox.Show("Item tag imdbid: " + (string)Item.Tags["ImdbID"]);

                            string ItemTitle = "";
                            string ItemTitleTmp = "";
                            try
                            {
                                ItemTitleTmp = (string)Item.Tags["Title"];
                            }
                            catch { }
                            
                            if ( !String.IsNullOrEmpty(ItemTitleTmp) )
                                ItemTitle = (string)Item.Tags["Title"];
                            else if (!String.IsNullOrEmpty(Item.Name))
                            {
                                ItemTitle = Item.Name;
                            }
                            else continue;
                            #endregion

                            #region online jobs
                            Debugger.Debugger.LogMessageToFile("Identified film " + ItemTitle + ". Continuing to download data for this film.");
                            ////MessageBox.Show("Identified film " + MovieTitle + ". Continuing to download data for this film.");
                            Thread.Sleep(500);

                            if (CanWorkOnline == true)
                            {

                                if (!SelectiveUpdating)
                                {

                                    #region download details


                                    #region IMDb Detailer

                                    try
                                    {
                                        HasDetails = (string)Item.Tags["HasDetails"];
                                    }
                                    catch { }

                                    if (HasDetails != "True")
                                    {
                                        if (WantIMDbDetails)
                                        {
                                            #region ...now get film details using this iMDBiD...
                                            //MessageBox.Show("imdbid before d/loading details is: " + imdbid);
                                            //MessageBox.Show("imdbOP._imdbID before d/loading details is: " + imdbOP._imdbID);
                                            //if (imdbOP._imdbID != "") imdbid = imdbOP._imdbID;
                                            //if (imdbid != "") imdbOP._imdbID = imdbid;                             
                                            if (imdbid != "")
                                            {

                                                #region ...get the film's details.
                                                if (!imdbid.StartsWith("tt"))
                                                    imdbid = "tt" + imdbid;
                                                // _progressText = "Grabbing IMDb ID: " + imdbOP._imdbID;
                                                ThisProgress.Progress.Progress(ThisProgress.CurrentProgress, "Downloading details from IMDb for '" + ItemTitle + "'...");
                                                Progress = ThisProgress.Progress;
                                                imdbOP.GetIMDbMovieDetails(imdbid, Progress, ThisProgress.CurrentProgress, true);
                                                #endregion
                                            }
                                            #endregion


                                            #region ...finally if the movie struct is filled, save the film details in the item's fields.
                                            if (imdbOP._imdbID.Trim() != "" && imdbOP._imdbID != null)
                                            {
                                                //string Episode = imdbOP._imdbMovie.Episode;
                                                //MessageBox.Show(Episode);
                                                //MessageBox.Show("title before saving: " + imdbOP._imdbMovie.Title);
                                                //MessageBox.Show("... movie struct is filled, about to save details in tags...");
                                                imdbOP.SaveIMDbMovieDetails(Item, MoviesSection, ThisProgress, imdbOP, parent.FullName, FileServerIsOnline);

                                                string titleTag = (string)Item.Tags["Title"];
                                                if (titleTag.StartsWith("\""))
                                                {
                                                    ThisProgress.Progress.Progress(ThisProgress.CurrentProgress, "This item is a TV episode.");
                                                    Thread.Sleep(1000);
                                                    ThisProgress.Progress.Progress(ThisProgress.CurrentProgress, "Searching for episode information...");
                                                    imdbOP.SaveEpisodeInfo(imdbid, Item, ThisProgress);
                                                }


                                                //////MessageBox.Show("Saved movie details. continuing to cover art...");
                                                //Item.Tags["HasDetails"] = "True";
                                                //Item.SaveTags();
                                            }
                                            #endregion
                                        }
                                    }

                                    #endregion


                                    #region OSdb Detailer

                                    HasDetails = (string)Item.Tags["HasDetails"];
                                    if (HasDetails != "True")
                                    {
                                        if (WantOSdbDetails)
                                        {
                                            if (OSisOnline)
                                            {

                                                if ( !imdbOP._notListed )
                                                {


                                                    if ( !String.IsNullOrEmpty(imdbid) )
                                                    {

                                                        try
                                                        {
                                                            ThisProgress.Progress.Progress(ThisProgress.CurrentProgress += ThisProgress.ProgressStep, "Downloading film details for " + ItemTitle + " from OSdb ...");
                                                            Thread.Sleep(2000);
                                                            Movie m = OSoperations.GetDetails(imdbid, Item);

                                                        }
                                                        catch (Exception)
                                                        {
                                                            try
                                                            {
                                                                ThisProgress.Progress.Progress(ThisProgress.CurrentProgress, "OSdb did not respond. Retrying... ");
                                                                Movie m = OSoperations.GetDetails(imdbid, Item);
                                                            }
                                                            catch (Exception)
                                                            {
                                                                //ThisProgress.Progress.Progress(ThisProgress.CurrentProgress += ThisProgress.ProgressStep, "MediaFairy could not download info for " + ItemTitle + ". Will give up for this item");
                                                                //Thread.Sleep(SleepValue);
                                                            }
                                                        }


                                                    }
                                                }
                                            }
                                        }
                                    #endregion



                                    }
                                    #endregion

                                    #region user cancels
                                    if (!Progress.Progress(ThisProgress.CurrentProgress, "Updating " + Item.Name + "..."))
                                    {
                                        MoviesSection.EndUpdate();
                                        Progress.Progress(ThisProgress.CurrentProgress, "You canceled MediaFairy. Completed jobs were saved to library.");
                                        return true;
                                    }
                                    #endregion

                                    //MessageBox.Show("ItemTitle before updating: " + ItemTitle);
                                    //MessageBox.Show("Title: " + (string)Item.Tags["Title"] );
                                    //MessageBox.Show("Name: " + Item.Name);

                                    #region save ItemTitle
                                    if ((string)Item.Tags["Title"] != "" && Item.Tags["Title"] != null)
                                    {
                                        ItemTitle = (string)Item.Tags["Title"];
                                    }
                                    else if (!String.IsNullOrEmpty(Item.Name))
                                    {
                                        ItemTitle = Item.Name;
                                    }
                                    else continue;
                                    #endregion

                                    //Helpers.DownloadFilmInfoFromAMG(Item, location, ThisProgress);

                                    //MessageBox.Show("ItemTitle after updating: " + ItemTitle);

                                    Helpers.WriteXmlForFilm(Item, ItemTitle, imdbid, parent.FullName, ThisProgress, FileServerIsOnline, IsUNC);


                                    #region Sort/move files
                                    if (WantSortingFilms == true)
                                    {
                                        #region update runtime paramaters
                                        itemUpdateParams updateParams;
                                        updateParams.FileInUse = FileInUse;
                                        updateParams.IsMultipart = IsMultipart;
                                        updateParams.CanWorkOnline = CanWorkOnline;
                                        updateParams.CompletedItems = CompletedCount;
                                        updateParams.FileServerChecked = FileServerChecked;
                                        updateParams.FileServerIsOnline = FileServerIsOnline;
                                        updateParams.ItemTimeframe = item_timeframe;
                                        updateParams.OSIsOnline = OSisOnline;
                                        updateParams.SelectiveUpdating = SelectiveUpdating;
                                        #endregion

                                        Helpers.AutoSort(Item, ItemTitle, multipart, ref location, SortingDestinationFilms, DirectoryTagMaskFilms, updateParams, "Film", ThisProgress);

                                    }
                                    #endregion

                                    #region user cancels
                                    if (!Progress.Progress(ThisProgress.CurrentProgress, "Updating " + Item.Name + "..."))
                                    {
                                        MoviesSection.EndUpdate();
                                        Progress.Progress(ThisProgress.CurrentProgress, "You canceled MediaFairy. Completed jobs were saved to library.");
                                        return true;
                                    }
                                    #endregion

                                    #region download Posters

                                    //Helpers.DownloadFilmPoster(Item, location, ItemTitle, VideoFilename, parent.FullName, ThisProgress, FileServerIsOnline, IsUNC);

                                    #endregion

                                    #region user cancels
                                    if (!Progress.Progress(ThisProgress.CurrentProgress, "Updating " + Item.Name + "..."))
                                    {
                                        MoviesSection.EndUpdate();
                                        Progress.Progress(ThisProgress.CurrentProgress, "You canceled MediaFairy. Completed jobs were saved to library.");
                                        return true;
                                    }
                                    #endregion

                                    #region download backdrops


                                    if ((string)Item.Tags["HasBackdrop"] != "True")
                                    {
                                        if (WantBackdrops)
                                        {
                                            if (WhereToSaveBackdrop != "in the movie's folder" || !IsUNC || FileServerIsOnline)
                                            {

                                                if (!File.Exists((string)Item.Tags["Backdrop"]))
                                                {
                                                    //CoverDownloaders.DownloadBackdropFromTMDB(Item, VideoFilename, location, ItemTitle, parent.FullName, FileServerIsOnline, IsUNC, ThisProgress);
                                                }
                                            }
                                        }
                                    }

                                    #endregion

                                    #region user cancels
                                    if (!Progress.Progress(ThisProgress.CurrentProgress, "Updating " + Item.Name + "..."))
                                    {
                                        MoviesSection.EndUpdate();
                                        Progress.Progress(ThisProgress.CurrentProgress, "You canceled MediaFairy. Completed jobs were saved to library.");
                                        return true;
                                    }
                                    #endregion

                                    #region download subtitles


                                    if (FileServerIsOnline || Importer.UseSameSubtitlesFolder)
                                    {
                                        if ((string)Item.Tags["HasSubtitles"] == "" || Item.Tags["HasSubtitles"] == null)
                                        {

                                            if (SubDloadLimitExceeded != true)
                                            {
                                                if (imdbid != "")
                                                {
                                                    //MessageBox.Show("imdbid before subs d/ling: " + imdbid);
                                                    bool subsDLresult = Downloaders.TryDownloadSubtitles(moviehash, OSisOnline, imdbid, Item, location, ThisProgress, connectionresult, UseSameSubtitlesFolder, SubtitlesFolder);
                                                }

                                            }

                                        }
                                    }

                                    #endregion

                                    #region user cancels
                                    if (!Progress.Progress(ThisProgress.CurrentProgress, "Updating " + Item.Name + "..."))
                                    {
                                        MoviesSection.EndUpdate();
                                        Progress.Progress(ThisProgress.CurrentProgress, "You canceled MediaFairy. Completed jobs were saved to library.");
                                        return true;
                                    }
                                    #endregion


                                    //Helpers.DownloadTrailer(Item,ItemTitle, ThisProgress);

                                    Helpers.ArchiveFilm(Item, parent, ThisProgress, ArchiveLocation);
                                }

                                if (!SelectiveUpdating || (string)Item.Tags["SelectiveUpdating"] == "DownloadMedia")
                                {
                                    if (SelectiveUpdating)
                                    {
                                        ThisProgress.Progress.Progress(ThisProgress.CurrentProgress, "Will selectively download online media for " + Item.Name + "...");
                                        Thread.Sleep(2000);
                                    }

                                    #region Download Online content

                                    if (DownloadOnlineContent == true)
                                    {

                                        try
                                        {

                                            DirectoryInfo d = new DirectoryInfo(DownloadFolder);

                                            if (d.Exists)
                                            {

                                                foreach (string OnlineContentField in OnlineContentFields)
                                                {
                                                    string OnlineContent = (string)Item.Tags[OnlineContentField];

                                                    if (OnlineContent != "" && OnlineContent != null)
                                                    {
                                                        if (OnlineContent.StartsWith("http") || OnlineContent.StartsWith("ftp"))
                                                        {
                                                            //CoverDownloaders.DownloadAnything(Item, DownloadFolder, ThisProgress);
                                                        }
                                                    }

                                                }

                                            }
                                        }
                                        catch (Exception exception)
                                        {
                                            Debugger.Debugger.LogMessageToFile("Could not download online content. The error was: " + exception.Message);
                                        }

                                    }
                                    #endregion


                                }



                            }
                            #endregion


                            #region scan and link filechains
                            if (!SelectiveUpdating && EnableChainLinking)
                            {
                                if (AllFilmItems != null && AllFilmItems.Count > 0)
                                {

                                    //ThisProgress.Progress.Progress(ThisProgress.CurrentProgress, "scanning for un-linked files...");
                                    Application.DoEvents();
                                    for (int i = 0; i < AllFilmItems.Count; i++)
                                    {
                                        IMLItem item = AllFilmItems[i];

                                        string item_ImdbID = "";
                                        string Item_ImdbID = "";
                                        try
                                        {
                                            item_ImdbID = (string)item.Tags["ImdbID"];
                                        }
                                        catch { }
                                        try
                                        {
                                            Item_ImdbID = (string)Item.Tags["ImdbID"];
                                        }
                                        catch { }


                                        if (item.Location != Item.Location && item_ImdbID == Item_ImdbID)
                                        {
                                            if (!item.Location.StartsWith("|") && !Item.Location.StartsWith("|"))
                                            {
                                                Item.Location = "|" + Item.Location + "|" + item.Location + "|";
                                                Item.SaveTags();
                                                MoviesSection.DeleteItem(item);
                                            }
                                        }
                                    }

                                }
                            }
                            #endregion

                            #region clear SelectiveUpdating tag
                            if (SelectiveUpdating)
                                Item.Tags["SelectiveUpdating"] = "";
                            #endregion

                            Item.SaveTags();
                            CompletedCount++;
                        }
                        MoviesSection.EndUpdate();

                        #endregion


                        #region Library Cleanup

                        try
                        {

                            #region Clean empty library fields


                            if (CleanEmptyFields == true)
                            {
                                Progress.Progress(98, "Cleaning empty library fields...");
                                Thread.Sleep(800);

                                string[] TagNames = (string[])MoviesSection.GetTagNames();


                                MoviesSection.BeginUpdate();
                                foreach (string TagName in TagNames)
                                {
                                    bool AllTagValuesAreEmpty = true;

                                    //ThisProgress.Progress.Progress(100, TagName);
                                    //Thread.Sleep(1000);

                                    if (TagName == "ToDelete")
                                    {

                                    }

                                    if (MoviesSection.GetTagValues(TagName) != null)
                                    {
                                        string[] TagValues = (string[])MoviesSection.GetTagValues(TagName);

                                        foreach (string tagvalue in TagValues)
                                        {
                                            if (tagvalue != "") AllTagValuesAreEmpty = false;

                                        }

                                        if (AllTagValuesAreEmpty == true) MoviesSection.DeleteTag(TagName);

                                    }


                                }
                                MoviesSection.EndUpdate();


                                MoviesSection.BeginUpdate();

                                Progress.Progress(98, "Removing marked for deletion items...");
                                Thread.Sleep(800);

                                IMLItemList ItemsToDelete = MoviesSection.SearchByTag("ToDelete", "true");
                                for (int i = 0; i < ItemsToDelete.Count; i++)
                                {
                                    MoviesSection.DeleteItem(ItemsToDelete[i]);
                                }

                                MoviesSection.EndUpdate();

                            }
                            #endregion;


                            #region Clean user-selected fields

                            if (CleanSelectedFields == true)
                            {
                                Progress.Progress(98, "Cleaning user-selected library fields...");
                                Thread.Sleep(800);


                                MoviesSection.BeginUpdate();

                                string[] FieldsToClean = SelectedFieldsToClean.Split(',');

                                foreach (string fieldtoclean in FieldsToClean)
                                {

                                    MoviesSection.DeleteTag(fieldtoclean.Trim());
                                }
                                MoviesSection.EndUpdate();

                            }

                            #endregion

                        }
                        catch (Exception e)
                        {
                            MessageBox.Show(e.ToString());
                        }

                        #endregion


                        Helpers.WriteXml(MoviesSection, XmlBackupsDirectory, ThisProgress);
                    }

                }
                #endregion
 


                #endregion



            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }


            #endregion




            return true;
        }

        #region Properties methods

        #if USE_MEEDIO 
        public bool EditCustomProperty(int Window, string PropertyName, ref string Value) 
        #elif USE_MEEDIOS
        public bool EditCustomProperty(IntPtr Window, string PropertyName, ref string Value)
        #endif
        {

            if (PropertyName == "customAdvancedOptions")
            {
                ImporterSettings.Caption = "Advanced Settings: ";
                ImporterSettings.Values = Value;
                if (ImporterSettings.ShowGUI(null))
                {
                    
                    Value = ImporterSettings.Values;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }







            //if (PropertyName == "customAdvancedOptions")
            //{
               
            //   myProperties.Initialize("Advanced Settings: ", Value);

            //   for (int i = 0; i < myProperties.Properties.Length; i++)
            //   {
            //       IMeedioExtensionsPluginProperty property = myProperties.Properties[i];

            //          if (String.IsNullOrEmpty(property.DefaultValue.ToString() ) )
            //          {
            //              //MessageBox.Show(property.Name );
            //          }          
            //   }


            //    if (myProperties.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            //    {
            //        Value = myProperties.Value;
            //        return true;
            //    }
            //    else return false;

            //}
            //else return true;

            return true;

        }


        public bool SetProperties(IMeedioItem Properties, out string ErrorText)
        {
            ErrorText = null;


            try
            {

                if (Properties["customAdvancedOptions"] != null)
                {
                    CustomProperty.SetProperties(Properties, "customAdvancedOptions");
                }


                //if (Properties["customAdvancedOptions"] != null)
                //MyProperties.SetProperties(ref Properties, "customAdvancedOptions");






                    if (Properties["WantFilesImportingProp"] != null) WantFilesImporting = (bool)Properties["WantFilesImportingProp"];

                if (Properties["ImportRootFolderProp"] != null)
                {
                    Type type = Properties["ImportRootFolderProp"].GetType();

                    if (type.Name != "DBNull")
                        ImportRootFolders = (string[])Properties["ImportRootFolderProp"];
                    else
                    {
                        ImportRootFolders = new string[1];
                        ImportRootFolders[0] = "";
                    }
                }

                if (Properties["WantToImportFilmsProp"] != null) WantToImportFilms = (bool)Properties["WantToImportFilmsProp"];
                if (Properties["WantToImportMusicProp"] != null) WantToImportMusic = (bool)Properties["WantToImportMusicProp"];
                if (Properties["WantToImportTvSeriesProp"] != null) WantToImportTvSeries = (bool)Properties["WantToImportTvSeriesProp"];
                if (Properties["WantToImportBooksProp"] != null) WantToImportBooks = (bool)Properties["WantToImportBooksProp"];

                if (Properties["WantMediaInfoProp"] != null) WantMediaInfo = (bool)Properties["WantMediaInfoProp"];

                if (Properties["PopulateIMDbDetailsProp"] != null) WantIMDbDetails = (bool)Properties["PopulateIMDbDetailsProp"];
                if (Properties["PopulateOSdbDetailsProp"] != null) WantOSdbDetails = (bool)Properties["PopulateOSdbDetailsProp"];

                if (Properties["DownloadPosterChoice"] != null) WantPosters = (bool)Properties["DownloadPosterChoice"];
                if (Properties["PrimaryPosterSourceChoice"] != null) PosterPrimarySource = (string)Properties["PrimaryPosterSourceChoice"];
                if (Properties["OverwritePosterChoice"] != null) OverwriteExistingPoster = (bool)Properties["OverwritePosterChoice"];
                if (Properties["PosterSizeChoice"] != null) PosterSize = (string)Properties["PosterSizeChoice"];
                if (Properties["WhereToSavePosterProp"] != null) WhereToSavePoster = (string)Properties["WhereToSavePosterProp"];
                if (Properties["PosterFolderProp"] != null) PosterFolder = (string)Properties["PosterFolderProp"];
                if (Properties["PosterNamingChoice"] != null) PosterNamingMethod = (string)Properties["PosterNamingChoice"];

                if (Properties["DownloadBackdropsChoice"] != null) WantBackdrops = (bool)Properties["DownloadBackdropsChoice"];
                if (Properties["WhereToSaveBackdropProp"] != null) WhereToSaveBackdrop = (string)Properties["WhereToSaveBackdropProp"];
                if (Properties["BackdropFolderProp"] != null) BackdropFolder = (string)Properties["BackdropFolderProp"];
                if (Properties["BackdropNamingChoice"] != null) BackdropNamingMethod = (string)Properties["BackdropNamingChoice"];

                if (Properties["WhereToSaveAlbumCoverProp"] != null) WhereToSaveAlbumCover = (string)Properties["WhereToSaveAlbumCoverProp"];
                if (Properties["AlbumCoversFolderProp"] != null) AlbumCoverFolder = (string)Properties["AlbumCoversFolderProp"];


                if (Properties["DownloadSubtitlesChoice"] != null) WantSubtitles = (bool)Properties["DownloadSubtitlesChoice"];
                if (Properties["LanguageProp"] != null) PrimaryLanguage = (string)Properties["LanguageProp"];
                if (Properties["LanguagePropSecondary"] != null) SecondaryLanguage = (string)Properties["LanguagePropSecondary"];
                if (Properties["SameSubsFolderProp"] != null) UseSameSubtitlesFolder = (bool)Properties["SameSubsFolderProp"];
                if (Properties["HashUploadingProp"] != null) EnableHashUploading = (bool)Properties["HashUploadingProp"];
                if (Properties["ConnectionDiagnosticsProp"] != null) WantConnectionDiagnostics = (bool)Properties["ConnectionDiagnosticsProp"];
                if (Properties["FileserverDiagnosticsProp"] != null) WantFileserverDiagnostics = (bool)Properties["FileserverDiagnosticsProp"];
                if (Properties["WantHashingProp"] != null) EnableHashing = (bool)Properties["WantHashingProp"];
                //if (Properties["SleepValueProp"] != null) SleepValue = (int)Properties["SleepValueProp"];
                if (Properties["DownloadOnlineContentProp"] != null) DownloadOnlineContent = (bool)Properties["DownloadOnlineContentProp"];
                if (Properties["DownloadFolderProp"] != null) DownloadFolder = (string)Properties["DownloadFolderProp"];


                if (Properties["OnlineContentFieldProp"] != null)
                {
                    Type type = Properties["OnlineContentFieldProp"].GetType();

                    if (type.Name != "DBNull")
                        OnlineContentFields = (string[])Properties["OnlineContentFieldProp"];
                    else
                    {
                        OnlineContentFields = new string[1];
                        OnlineContentFields[0] = "";
                    }
                }

                if (Properties["DeleteMissingProp"] != null) DeleteMissing = (bool)Properties["DeleteMissingProp"];
                if (Properties["ClearEmptyFieldsProp"] != null) CleanEmptyFields = (bool)Properties["ClearEmptyFieldsProp"];
                if (Properties["CleanSelectedFieldsProp"] != null) CleanSelectedFields = (bool)Properties["CleanSelectedFieldsProp"];
                if (Properties["SelectedFieldsToCleanProp"] != null) SelectedFieldsToClean = (string)Properties["SelectedFieldsToCleanProp"];


                if (Properties["WantSortingFilmsProp"] != null) WantSortingFilms = (bool)Properties["WantSortingFilmsProp"];
                if (Properties["EnableChainLinkingProp"] != null) EnableChainLinking = (bool)Properties["EnableChainLinkingProp"];

                if (Properties["WantSortingTvShowsProp"] != null) WantSortingTvShows = (bool)Properties["WantSortingTvShowsProp"];
                if (Properties["WantSortingMusicProp"] != null) WantSortingMusic = (bool)Properties["WantSortingMusicProp"];

                if (Properties["PromptBeforeMovingProp"] != null) PromptBeforeMoving = (bool)Properties["PromptBeforeMovingProp"];
                if (Properties["OnlyTaggedForMovingProp"] != null) OnlyTaggedForMoving = (bool)Properties["OnlyTaggedForMovingProp"];

                if (Properties["SortingDestinationFilmsProp"] != null) SortingDestinationFilms = (string)Properties["SortingDestinationFilmsProp"];
                if (Properties["SortingDestinationTvShowsProp"] != null) SortingDestinationTvShows = (string)Properties["SortingDestinationTvShowsProp"];
                if (Properties["SortingDestinationMusicProp"] != null) SortingDestinationMusic = (string)Properties["SortingDestinationMusicProp"];

                if (Properties["DirectoryTagMaskFilmsProp"] != null) DirectoryTagMaskFilms = (string)Properties["DirectoryTagMaskFilmsProp"];
                if (Properties["DirectoryTagMaskTvShowsProp"] != null) DirectoryTagMaskTvShows = (string)Properties["DirectoryTagMaskTvShowsProp"];
                if (Properties["DirectoryTagMaskMusicProp"] != null) DirectoryTagMaskMusic = (string)Properties["DirectoryTagMaskMusicProp"];

                if (Properties["DirectoryStructureProp"] != null) DirectoryStructure = (string)Properties["DirectoryStructureProp"];

                if (Properties["AssistModeProp"] != null) AssistType = (string)Properties["AssistModeProp"];

                //if (Properties["LimitFilesToImportProp"] != null) importing_limit = (int)Properties["LimitFilesToImportProp"];


                if (Properties["FilmsLibraryProp"] != null) FilmsLibrary = (string)Properties["FilmsLibraryProp"];
                if (Properties["TvShowsLibraryProp"] != null) TvShowsLibrary = (string)Properties["TvShowsLibraryProp"];
                if (Properties["TvSeriesLibraryProp"] != null) TvSeriesLibrary = (string)Properties["TvSeriesLibraryProp"];
                if (Properties["MusicLibraryProp"] != null) MusicLibrary = (string)Properties["MusicLibraryProp"];
                if (Properties["BooksLibraryProp"] != null) BooksLibrary = (string)Properties["BooksLibraryProp"];

                if (Properties["WantMusicDNSProp"] != null) WantMusicDNS = (bool)Properties["WantMusicDNSProp"];
                if (Properties["WantAudioTagsProp"] != null) WantAudioTags = (bool)Properties["WantAudioTagsProp"];
                if (Properties["SaveMetadataToAudioTagsProp"] != null) SaveMetadataToAudioTags = (bool)Properties["SaveMetadataToAudioTagsProp"];

                if (Properties["WantItunesSyncProp"] != null) WantItunesSync = (bool)Properties["WantItunesSyncProp"];

                if (Properties["UpdateMoviesProp"] != null) UpdateMoviesSection = (bool)Properties["UpdateMoviesProp"];
                if (Properties["UpdateTvShowsProp"] != null) UpdateTvShowsSection = (bool)Properties["UpdateTvShowsProp"];
                if (Properties["UpdateMusicProp"] != null) UpdateMusicSection = (bool)Properties["UpdateMusicProp"];
                if (Properties["UpdateBooksProp"] != null) UpdateBooksSection = (bool)Properties["UpdateBooksProp"];

                if (Properties["EnableUIProp"] != null) AlwaysPromptForImages = (bool)Properties["EnableUIProp"];

                if (Properties["EnableNfoParsingProp"] != null) EnableNfoParsing = (bool)Properties["EnableNfoParsingProp"];
                if (Properties["WriteFilmDescriptorsProp"] != null) WriteFilmDescriptors = (bool)Properties["WriteFilmDescriptorsProp"];



                if (Properties["EnableFilmReleasesProp"] != null) DownloadFilmReleases = (bool)Properties["EnableFilmReleasesProp"];

                try
                {
                    if (Properties["FilmRatingMinProp"] != null) FilmRatingMin2 = (string)Properties["FilmRatingMinProp"];

                    FilmRatingMin = Convert.ToDouble(FilmRatingMin2);

                }
                catch (Exception)
                {
                    FilmRatingMin = 0;
                    MessageBox.Show("The films rating filter you provided is not correct. Please use the numerical form n,n.");
                }


                if (Properties["SimpleWantFilmDetailsProp"] != null) SimpleWantFilmDetails = (bool)Properties["SimpleWantFilmDetailsProp"];

                if (SimpleWantFilmDetails)
                {
                    if (!WantIMDbDetails && !WantOSdbDetails)
                    {
                        WantIMDbDetails = true;
                        WantOSdbDetails = true;
                    }
                }
                if (!SimpleWantFilmDetails)
                {
                    if (WantIMDbDetails || WantOSdbDetails)
                    {
                        WantIMDbDetails = false;
                        WantOSdbDetails = false;
                    }
                }


                if (Properties["WantAlbumCoversProp"] != null) WantAlbumCovers = (bool)Properties["WantAlbumCoversProp"];

                if (Properties["EnableArchiveExtractorProp"] != null) EnableArchiveExtractor = (bool)Properties["EnableArchiveExtractorProp"];
                if (Properties["DeleteArchiveAfterExtractionProp"] != null) DeleteArchiveAfterExtraction = (bool)Properties["DeleteArchiveAfterExtractionProp"];
                if (Properties["freespaceThresholdProp"] != null) freespaceThreshold = (int)Properties["freespaceThresholdProp"];


                if (Properties["EnableMediaDetectionProp"] != null) EnableMediaDetection = (bool)Properties["EnableMediaDetectionProp"];
                if (Properties["FilmsFolderProp"] != null) FilmsFolders = (string[])Properties["FilmsFolderProp"];
                if (Properties["TvShowsFolderProp"] != null) TvShowsFolders = (string[])Properties["TvShowsFolderProp"];



                if (Properties["ArchiveWatchedFilmsProp"] != null) ArchiveWatchedFilms = (bool)Properties["ArchiveWatchedFilmsProp"];
                if (Properties["ArchiveLocationProp"] != null) ArchiveLocation = (string)Properties["ArchiveLocationProp"];


                if (Properties["EmbedAlbumCoverProp"] != null) EmbedAlbumCover = (bool)Properties["EmbedAlbumCoverProp"];


                if (Properties["SeriesBannersFolderProp"] != null) SeriesBannersFolder = (string)Properties["SeriesBannersFolderProp"];
                if (Properties["SeriesFanartFolderProp"] != null) SeriesFanartFolder = (string)Properties["SeriesFanartFolderProp"];
                if (Properties["SeasonImagesFolderProp"] != null) SeasonImagesFolder = (string)Properties["SeasonImagesFolderProp"];

                if (Properties["WantEpisodeDetailsProp"] != null) WantEpisodeDetails = (bool)Properties["WantEpisodeDetailsProp"];
                if (Properties["WantEpisodeThumbnailsProp"] != null) WantEpisodeThumbnails = (bool)Properties["WantEpisodeThumbnailsProp"];
                if (Properties["WantSeriesBannersProp"] != null) WantSeriesBanners = (bool)Properties["WantSeriesBannersProp"];
                if (Properties["WantSeriesBackdropsProp"] != null) WantSeriesBackdrops = (bool)Properties["WantSeriesBackdropsProp"];
                if (Properties["WantSeasonImagesProp"] != null) WantSeasonImages = (bool)Properties["WantSeasonImagesProp"];




            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return false;
            }
            return true;
        }


        public bool GetProperty(int Index, IMeedioPluginProperty Prop)
        {

            int counter = 1;
            Initialized = Initialize();

            String[] twochoices = new String[2];
            String[] threechoices = new String[3];
            String[] fourchoices = new String[4];


            #region list of properties


            try
            {

                #region initialize Sections
                string[] choices;


                #if USE_MEEDIO
                IMeedioLibrary MeedioLibrary = new MediaLibrary();
                #elif USE_MEEDIOS
                MeediOS.IMeedioLibrary MeedioLibrary = new MeediOS.MediaLibraryClass();
                #endif


                choices = new string[MeedioLibrary.SectionCount];

                for (int i = 0; i < MeedioLibrary.SectionCount; i++)
                {

                    #if USE_MEEDIO
                    choices[i] = MeedioLibrary.get_Sections(i);
                    #elif USE_MEEDIOS
                    choices[i] = MeedioLibrary.Sections(i);
                    #endif
                }
                #endregion


                #region Media Sections
                if (Index == counter++)
                {
                    //set the internal name
                    Prop.Name = "FilmsLibraryProp";
                    //set name shown to user
                    Prop.Caption = "Select your Films library: ";
                    //set the tool tip
                    Prop.GroupCaption = "                                Media Sections";
                    //Prop.IsMandatory = true;
                    Prop.HelpText = "Please select your Movies Section from the provided list. If you don't want MediaFairy to update this library, just uncheck the option above.";
                    Prop.DefaultValue = FilmsLibrary;
                    Prop.DataType = "string";
                    //Prop.IsMandatory = true;
                    //MessageBox.Show("Sections= " + MeedioLibrary.SectionCount);
                    Prop.Choices = choices;
                    return true;

                }

                if (Index == counter++)
                {
                    //set the internal name
                    Prop.Name = "TvShowsLibraryProp";
                    //set name shown to user
                    Prop.GroupCaption = "                                Media Sections";
                    Prop.Caption = "Select your TV Episodes library: ";
                    //set the tool tip
                    //Prop.IsMandatory = true;
                    Prop.HelpText = "Please select your Tv Shows Section from the provided list. If you don't want MediaFairy to update this library, just uncheck the option above.";
                    Prop.DefaultValue = TvShowsLibrary;
                    Prop.DataType = "string";
                    //Prop.IsMandatory = true;
                    //MessageBox.Show("Sections= " + MeedioLibrary.SectionCount);
                    Prop.Choices = choices;
                    return true;

                }
                if (Index == counter++)
                {
                    //set the internal name
                    Prop.Name = "TvSeriesLibraryProp";
                    //set name shown to user
                    Prop.GroupCaption = "                                Media Sections";
                    Prop.Caption = "Select your TV Series library: ";
                    //set the tool tip
                    //Prop.IsMandatory = true;
                    Prop.HelpText = "Please select your Tv Series Section from the provided list. The 'Tv Series' section is needed for MediaFairy to store already known series information. MediaFairy is compatible with MeeTvShows and can use the already known series information stored in the 'meeTVshows-Series' section created by that plugin.";
                    Prop.DefaultValue = TvSeriesLibrary;
                    Prop.DataType = "string";
                    //Prop.IsMandatory = true;
                    //MessageBox.Show("Sections= " + MeedioLibrary.SectionCount);
                    Prop.Choices = choices;
                    return true;

                }

                if (Index == counter++)
                {
                    //set the internal name
                    Prop.Name = "MusicLibraryProp";
                    //set name shown to user
                    Prop.GroupCaption = "                                Media Sections";
                    Prop.Caption = "Select your Music library: ";
                    //Prop.IsMandatory = true;
                    //set the tool tip
                    Prop.HelpText = "Please select your Music Section from the provided list. If you don't want MediaFairy to update this library, just uncheck the option above.";
                    Prop.DefaultValue = MusicLibrary;
                    Prop.DataType = "string";
                    //Prop.IsMandatory = true;
                    //MessageBox.Show("Sections= " + MeedioLibrary.SectionCount);
                    Prop.Choices = choices;
                    return true;
                }

                if (Index == counter++)
                {
                    //set the internal name
                    Prop.Name = "BooksLibraryProp";
                    //set name shown to user
                    Prop.GroupCaption = "                                Media Sections";
                    Prop.Caption = "Select your Books library: ";
                    //Prop.IsMandatory = true;
                    //set the tool tip
                    Prop.HelpText = "Please select your Books Section from the provided list. If you don't want MediaFairy to update this library, just uncheck the 'Update Books Section' option.";
                    Prop.DefaultValue = BooksLibrary;
                    Prop.DataType = "string";
                    //Prop.IsMandatory = true;
                    //MessageBox.Show("Sections= " + MeedioLibrary.SectionCount);
                    Prop.Choices = choices;
                    return true;
                }

                #endregion



                #region Media Importer

                if (Index == counter++)
                {
                    //set the internal name
                    Prop.Name = "WantFilesImportingProp";
                    //set name shown to user
                    Prop.Caption = "Enable Media Importer? ";
                    Prop.GroupCaption = "                                 Media Importer";
                    //set the tool tip
                    Prop.HelpText = "The Automated Media files Importer will scan the folder you provide below and automatically recognize and import the media files of your desired media types in their respective library sections, depending on the settings you specify below.";
                    Prop.DefaultValue = WantFilesImporting;
                    Prop.DataType = "bool";
                    return true;
                }

                if (Index == counter++)
                {
                    //set the internal name
                    Prop.Name = "ImportRootFolderProp";
                    //set name shown to user
                    Prop.Caption = "Media files Root Folders: ";
                    Prop.GroupCaption = "                                 Media Importer";
                    //set the tool tip
                    //Prop.IsMandatory = true;
                    Prop.HelpText = "Please specify the root folders which contain the media files you want to import. This folder can contain mixed media types and also any other type of files. You don't have to use any tag masks or filters, the automated media recognizing engine will take care of importing each media file into its appropriate library section.";
                    Prop.DefaultValue = ImportRootFolders;
                    Prop.DataType = "folderlist";
                    return true;
                }



                if (Index == counter++)
                {
                    //set the internal name
                    Prop.Name = "WantToImportFilmsProp";
                    //set name shown to user
                    Prop.Caption = "Import Movies?";
                    Prop.GroupCaption = "                                 Media Importer";
                    //set the tool tip
                    Prop.HelpText = " Would you like the Automated Media Importer to import the films it finds in your root folders? ";
                    Prop.DefaultValue = WantToImportFilms;
                    Prop.DataType = "bool";
                    return true;
                }



                if (Index == counter++)
                {
                    //set the internal name
                    Prop.Name = "WantToImportMusicProp";
                    //set name shown to user
                    Prop.Caption = "Import Music?";
                    Prop.GroupCaption = "                                 Media Importer";
                    //set the tool tip
                    Prop.HelpText = "Would you like the Automated Media Importer to import the music tracks it finds in your root folders?";
                    Prop.DefaultValue = WantToImportMusic;
                    Prop.DataType = "bool";
                    return true;
                }


                if (Index == counter++)
                {
                    //set the internal name
                    Prop.Name = "WantToImportTvSeriesProp";
                    //set name shown to user
                    Prop.Caption = "Import Tv Series?";
                    Prop.GroupCaption = "                                 Media Importer";
                    //set the tool tip
                    Prop.HelpText = "Would you like the Automated Media Importer to import the Tv Series episodes it finds in your root folders?";
                    Prop.DefaultValue = WantToImportTvSeries;
                    Prop.DataType = "bool";
                    return true;
                }

                if (Index == counter++)
                {
                    //set the internal name
                    Prop.Name = "WantToImportBooksProp";
                    //set name shown to user
                    Prop.Caption = "Import Books?";
                    Prop.GroupCaption = "                                 Media Importer";
                    //set the tool tip
                    Prop.HelpText = "Would you like the Automated Media Importer to import the digital books it finds in your root folders?";
                    Prop.DefaultValue = WantToImportBooks;
                    Prop.DataType = "bool";
                    return true;
                }


                #endregion


                #region Media Analyzer
                if (Index == counter++)
                {
                    //set the internal name
                    Prop.Name = "EnableMediaDetectionProp";
                    //set name shown to user
                    Prop.Caption = "Automatically detect media types";
                    Prop.GroupCaption = "                              Media Analyzer";
                    //set the tool tip
                    Prop.HelpText = " This setting will determine if a media file analyzing process will be performed for each imported media file. This analyzing process provides to the meida importer the ability to automatically recognize media files and distinguish their media type, without needing any configuration or manual files sorting. Please note however that with this feature enabled, the importing time may be significantly increased the first time you run the importer, especially with online files and/or large media collections. Disabling this feature will make the importing process significantly faster, but you will have to specify the root directories under which each media type is stored (under 'Override Media Analyzer' settings group), and those diretories will need to *only* contain media files belonging to the selected media type.   ";
                    Prop.DefaultValue = EnableMediaDetection;
                    Prop.DataType = "bool";
                    return true;
                }
                if (Index == counter++)
                {
                    //set the internal name
                    Prop.Name = "WantMediaInfoProp";
                    //set name shown to user
                    Prop.Caption = "Import media information";
                    Prop.GroupCaption = "                              Media Analyzer";
                    //set the tool tip
                    Prop.HelpText = "Modern themes for Meedio frontend have the capability to graphically display digital media information for films or series,\r for example the video/audio format or quality.\r Would you like the plugin to automatically import this information?";
                    Prop.DefaultValue = WantMediaInfo;
                    Prop.DataType = "bool";
                    return true;
                }
                #endregion


                #region Media Updaters
                if (Index == counter++)
                {
                    //set the internal name
                    Prop.Name = "UpdateMoviesProp";
                    //set name shown to user
                    Prop.Caption = "Update Films Section? ";
                    Prop.GroupCaption = "                                Media Updaters ";
                    //set the tool tip
                    Prop.HelpText = "Do you want MediaFairy to work with the films in your Movies library?";
                    Prop.DefaultValue = UpdateMoviesSection;
                    Prop.DataType = "bool";
                    return true;
                }


                if (Index == counter++)
                {
                    //set the internal name
                    Prop.Name = "UpdateTvShowsProp";
                    //set name shown to user
                    Prop.Caption = "Update Tv Shows Section? ";
                    Prop.GroupCaption = "                                Media Updaters ";
                    //set the tool tip
                    Prop.HelpText = "Do you want MediaFairy to work with the Tv Series in your Tv Shows library?";
                    Prop.DefaultValue = UpdateTvShowsSection;
                    Prop.DataType = "bool";
                    return true;
                }

                if (Index == counter++)
                {
                    //set the internal name
                    Prop.Name = "UpdateMusicProp";
                    //set name shown to user
                    Prop.Caption = "Update Music Section? ";
                    Prop.GroupCaption = "                                Media Updaters ";
                    //set the tool tip
                    Prop.HelpText = "Do you want MediaFairy to work with your Music library";
                    Prop.DefaultValue = UpdateMusicSection;
                    Prop.DataType = "bool";
                    return true;
                }

                if (Index == counter++)
                {
                    //set the internal name
                    Prop.Name = "UpdateBooksProp";
                    //set name shown to user
                    Prop.Caption = "Update Books Section? ";
                    Prop.GroupCaption = "                                Media Updaters ";
                    //set the tool tip
                    Prop.HelpText = "Do you want MediaFairy to work with and update your Books library section?";
                    Prop.DefaultValue = UpdateBooksSection;
                    Prop.DataType = "bool";
                    return true;
                }

                #endregion


                #region Films Updating Settings
                if (Index == counter++)
                {
                    //set the internal name
                    Prop.Name = "SimpleWantFilmDetailsProp";
                    //set name shown to user
                    Prop.Caption = "Auto-identify and import film information";
                    Prop.GroupCaption = "                                Film Updater";
                    //set the tool tip
                    Prop.HelpText = "";
                    Prop.DefaultValue = SimpleWantFilmDetails;
                    Prop.DataType = "bool";
                    return true;
                }

                if (Index == counter++)
                {
                    //set the internal name
                    Prop.Name = "DownloadPosterChoice";
                    //set name shown to user
                    Prop.Caption = "Also download cover images ";
                    Prop.GroupCaption = "                                Film Updater";
                    //set the tool tip
                    Prop.HelpText = "Do you want MediaFairy to download cover images for your films?";
                    Prop.DefaultValue = WantPosters;
                    Prop.DataType = "bool";
                    return true;
                }
                if (Index == counter++)
                {
                    //set the internal name
                    Prop.Name = "DownloadBackdropsChoice";
                    //set name shown to user
                    Prop.Caption = "Also download backdrop images ";
                    Prop.GroupCaption = "                                Film Updater";
                    //set the tool tip
                    Prop.HelpText = "Do you want MediaFairy to download high-resolution backdrop images for your films?";
                    Prop.DefaultValue = WantBackdrops;
                    Prop.DataType = "bool";
                    return true;
                }

                if (Index == counter++)
                {
                    //set the internal name
                    Prop.Name = "HashUploadingProp";
                    //set name shown to user
                    Prop.Caption = "Auto-upload video definitions to database ";
                    Prop.GroupCaption = "                                Film Updater";
                    //set the tool tip
                    Prop.HelpText = " When this option is enabled, the video identifier will scan the film's folder for an .nfo file which contains useful information about the film for instant and exact film identification. ";
                    Prop.DefaultValue = EnableHashUploading;
                    Prop.DataType = "bool";
                    return true;
                }


                #endregion


                #region TV Updater

                if (Index == counter++)
                {
                    //set the internal name
                    Prop.Name = "WantEpisodeDetailsProp";
                    //set name shown to user
                    Prop.Caption = "Download episode details";
                    Prop.GroupCaption = "                                   TV Updater";
                    //set the tool tip
                    Prop.HelpText = " Would you like the plugin to download online details from TVdb for each episode in your TV library? ";
                    Prop.DefaultValue = WantEpisodeDetails;
                    Prop.DataType = "bool";
                    return true;
                }

                if (Index == counter++)
                {
                    //set the internal name
                    Prop.Name = "WantEpisodeThumbnailsProp";
                    //set name shown to user
                    Prop.Caption = "Download episode thumbnails";
                    Prop.GroupCaption = "                                   TV Updater";
                    //set the tool tip
                    Prop.HelpText = " Would you like the plugin to download thumbnail images for each episode in your TV library? ";
                    Prop.DefaultValue = WantEpisodeThumbnails;
                    Prop.DataType = "bool";
                    return true;
                }

                if (Index == counter++)
                {
                    //set the internal name
                    Prop.Name = "WantSeriesBannersProp";
                    //set name shown to user
                    Prop.Caption = "Download series banners";
                    Prop.GroupCaption = "                                   TV Updater";
                    //set the tool tip
                    Prop.HelpText = " Would you like the plugin to download banner images for the series in your TV library? ";
                    Prop.DefaultValue = WantSeriesBanners;
                    Prop.DataType = "bool";
                    return true;
                }

                if (Index == counter++)
                {
                    //set the internal name
                    Prop.Name = "WantSeriesBackdropsProp";
                    //set name shown to user
                    Prop.Caption = "Download series backdrops";
                    Prop.GroupCaption = "                                   TV Updater";
                    //set the tool tip
                    Prop.HelpText = " Would you like the plugin to download high resolution backdrops for the series in your TV library? ";
                    Prop.DefaultValue = WantSeriesBackdrops;
                    Prop.DataType = "bool";
                    return true;
                }

                if (Index == counter++)
                {
                    //set the internal name
                    Prop.Name = "WantSeasonImagesProp";
                    //set name shown to user
                    Prop.Caption = "Download season covers";
                    Prop.GroupCaption = "                                   TV Updater";
                    //set the tool tip
                    Prop.HelpText = " Would you like the plugin to download cover images for each season of the tv series in your TV library? ";
                    Prop.DefaultValue = WantSeasonImages;
                    Prop.DataType = "bool";
                    return true;
                }


                #endregion


                #region Audio Identifying
                if (Index == counter++)
                {
                    //set the internal name
                    Prop.Name = "WantAudioTagsProp";
                    //set name shown to user
                    Prop.Caption = "Import track information from audio file";
                    Prop.GroupCaption = "                              Music Updater";
                    //set the tool tip
                    Prop.HelpText = "MediaFairy has the capability to extract the metadata contained in your audio files and import this information into your music library. Do you want to enable this feature?";
                    Prop.DefaultValue = WantAudioTags;
                    Prop.DataType = "bool";
                    return true;
                }
                if (Index == counter++)
                {
                    //set the internal name
                    Prop.Name = "WantMusicDNSProp";
                    //set name shown to user
                    Prop.Caption = "Also use online audio identifier ";
                    Prop.GroupCaption = "                              Music Updater";
                    //set the tool tip
                    Prop.HelpText = "MediaFairy contains an automated song recognition engine which uses the audio fingerprinting technology by MusicDNS to uniquely identify your audio tracks by their acoustic properties. Would you like this feature enabled? (If you want this feature, it's recommended that you have the Audio Tag Importing feature enabled also) ";
                    Prop.DefaultValue = WantMusicDNS;
                    Prop.DataType = "bool";
                    return true;
                }
                if (Index == counter++)
                {
                    //set the internal name
                    Prop.Name = "WantAlbumCoversProp";
                    //set name shown to user
                    Prop.Caption = "Also download album covers";
                    Prop.GroupCaption = "                              Music Updater";
                    //set the tool tip
                    Prop.HelpText = "Do you want MediaFairy to download album cover images for your music albums?";
                    Prop.DefaultValue = WantAlbumCovers;
                    Prop.DataType = "bool";
                    return true;
                }
                #endregion



                if (Index == counter++)
                {
                    Prop.CanTypeChoices = false;
                    Prop.Caption = "Advanced Settings: ";
                    Prop.DataType = "custom";
                    Prop.GroupCaption = "                                  - Advanced - ";
                    //Prop.DefaultValue = ImporterSettings.Default;
                    Prop.Name = "customAdvancedOptions";
                    return true;
                }


            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                return false;
            }

            #endregion

            return false;
        }



        private Boolean Initialize()
        {
            try
            {
                if (!Initialized)
                {
                    ImporterSettings = new MyProperties();

                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                return false;
            }
            return true;
        }


        #endregion

        #endregion


        #endregion



        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            //Snapshot();
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(Boolean disposing)
        {
            if (disposing)
            {
                //ImportFunc.Dispose();
                //ImportFunc = null;
                //ImporterSettings.Dispose();
                //ImporterSettings = null;
                //TagMasks.Clear();
                //TagMasks = null;
                //LogMessages.Dispose();
                //LogMessages = null;
                //Log = null;
                //Marshal.ReleaseComObject(Section);
                //Marshal.ReleaseComObject(Progress);
                //Progress = null;
                //Section = null;
                //Snapshot();
                // Free other state (managed objects).
            }
            // Free your own state (unmanaged objects).
            // Set large fields to null.
        }
 
        #endregion


        #region default properties


        #region init library sections
        #if USE_MEEDIO
            public static MeedioLibrary.MediaLibraryClass library = new MeedioLibrary.MediaLibraryClass();
        #elif USE_MEEDIOS
        public static MeediOS.MediaLibraryClass library = new MeediOS.MediaLibraryClass();
            #endif

        public static IMLSection MoviesSection;
        public static IMLSection MusicSection;
        public static IMLSection TvSection;
        public static IMLSection TvSeriesSection;
        public static IMLSection BooksSection;

        #endregion

        #region Runtime variables

        public static StatusForm statusForm = new StatusForm();       

        private MyProperties ImporterSettings;
        public static bool Initialized;


        #endregion

        #region Media Importer
        public static bool WantFilesImporting = false;
        public static string[] ImportRootFolders;
        public static bool WantToImportFilms = true;
        public static bool WantToImportMusic = true;
        public static bool WantToImportTvSeries = true;
        public static bool WantToImportBooks = true;
        #endregion

        #region Section Names
        public static string FilmsLibrary = "Movies";
        public static string TvShowsLibrary = "TvEpisodes";
        public static string TvSeriesLibrary = "TvSeries";
        public static string MusicLibrary = "Music";
        public static string BooksLibrary = "Books";
        public static string ReleasesLibrary = "Releases";
        #endregion

        #region Video Identifier
        public static bool EnableNfoParsing = true;
        public static bool EnableHashing = true;
        public static bool EnableHashUploading = false;
        public static bool WantMediaInfo = true;
        public static bool WriteFilmDescriptors = true;
        #endregion

        #region Audio Identifier
        public static bool WantMusicDNS = true;
        public static bool WantAudioTags = true;
        public static bool SaveMetadataToAudioTags = true;
        public static bool OverwriteExistingMusicTags = false;
        public static bool SkipLongTracks = true;
        public static bool OnlyIdFilesWithMissingInfo = true;
        public static bool WantItunesSync = false;
        #endregion

        #region Section Updaters
        public static bool UpdateMoviesSection = true;
        public static bool UpdateTvShowsSection = true;
        public static bool UpdateMusicSection = true;
        public static bool UpdateBooksSection = true;
        #endregion

        #region Film Details
        public static bool WantOSdbDetails = true;
        public static bool WantIMDbDetails = true;
        #endregion

        #region Film Posters
        public static bool WantPosters = true;
        public static string PosterPrimarySource = "TheMovieDatabase";
        public static string PosterSize = "Medium";
        public static string PosterNamingMethod = "folder.jpg";
        public static string WhereToSavePoster = "in the movie's folder";
        public static string PosterFolder = " ";
        public static bool OverwriteExistingPoster = false;
        #endregion

        #region Film Backdrops
        public static bool WantBackdrops = true;
        public static string BackdropNamingMethod = "folder.jpg";
        public static string WhereToSaveBackdrop = "in the movie's folder";
        public static string BackdropFolder = " ";
        #endregion

        #region Album Covers
        public static string WhereToSaveAlbumCover = "in the album's folder";
        public static string AlbumCoverFolder = "";
        public static bool EmbedAlbumCover = false;
        #endregion

        #region Subtitles
        public static bool WantSubtitles = false;
        public static string PrimaryLanguage = "eng";
        private static string SecondaryLanguage = "to be implemented soon";
        public static string WhenToUseSecondaryLanguage = "If no subtitles at all exist for primary language";
        public static string zipfilepath = "";
        public static bool UseSameSubtitlesFolder = false;
        public static string SubtitlesFolder = @"D:\";
        #endregion

        #region Diagnostics
        public static bool WantConnectionDiagnostics = false;
        public static bool WantOSdbDiagnostics = true;
        public static bool WantFileserverDiagnostics = true;
        #endregion

        #region Interoperability
        public static int SleepValue = 1500;
        #endregion

        #region Online Content Downloader
        public static bool DownloadOnlineContent = false;
        public static string DownloadFolder = " ";
        public static string[] OnlineContentFields = new string[2] { "Video", "Trailer" };
        #endregion

        #region Media Organizers
        public static bool WantSortingFilms = false;
        public static bool WantSortingTvShows = false;
        public static bool WantSortingMusic = false;
        public static bool OnlyTaggedForMoving = false;
        public static bool PromptBeforeMoving = false;
        public static string SortingDestinationFilms = "";
        public static string SortingDestinationTvShows = "";
        public static string SortingDestinationMusic = "";
        public static string DirectoryStructure = "Simple: each movie has a folder below the root folder named by that film's Title.";
        public static string DirectoryTagMaskFilms = "<Title>";
        public static string DirectoryTagMaskTvShows = @"<SeriesName>\<SeasonNumber>";
        public static string DirectoryTagMaskMusic = @"<Artist>\<Album>";
        #endregion

        #region Library Cleanup Settings
        public static bool EnableChainLinking = true;
        public static bool DeleteMissing = true;
        public static bool CleanEmptyFields = true;
        public static bool CleanSelectedFields = false;
        public static string SelectedFieldsToClean = " ";
        #endregion

        #region IMDb title matcher
        public static string AssistType = "When no exact match found";
        public static bool UseFirst = false;
        public static bool UseLatest = false;
        public static bool UseMostPopular = true;
        public static bool EnableNameMatching = true;
        public static bool NotListed = false;
        public static bool DeleteItem = false;
        #endregion

        #region Current Session Variables
        public static bool userCancels = false;
        public static bool ImportingStarted = false;
        public static bool ImportingCompleted = false;
        public static bool AlwaysPromptForImages = false;
        public static bool FileInUse = false;
        public static int KBytesPerSec = 0;
        public static bool SubDloadLimitExceeded = false;
        #endregion

        #region Releases Updater
        public static bool DownloadFilmReleases = false;
        public static string[] FilmsRssFeeds;
        public static string FilmRatingMin2 = "7,3";
        public static double FilmRatingMin = 0;
        public static string TorrentsFolder = "";
        #endregion



        #region Simple Settings
        public static bool SimpleWantFilmDetails = true;
        public static bool SimpleWantFilmPosters = true;
        public static bool WantAlbumCovers = true;
        #endregion

        #region Archive Extractor
        public static bool EnableArchiveExtractor = false;
        public static bool DeleteArchiveAfterExtraction = false;
        public static int freespaceThreshold = 1000;
        #endregion

        #region Watched film archiver
        public static bool ArchiveWatchedFilms = false;
        public static string ArchiveLocation = "";
        #endregion

        #region PC games importer
        public static bool WantToImportPcGames = false;
        public static string PcGamesFolderStr = "";
        #endregion

        #region Media Type Detector
        public static bool EnableMediaDetection = true;
        public static string[] FilmsFolders = new string[1];
      
        public static string[] TvShowsFolders = new string[1];
        #endregion



        #region TV Series Updater
        public static string SeriesBannersFolder = "";
        public static string SeriesFanartFolder = "";
        public static string EpisodeImagesFolder = "";
        public static string SeasonImagesFolder = "";

        public static bool WantEpisodeDetails = true;
        public static bool WantEpisodeThumbnails = true;
        public static bool WantSeriesBanners = true;
        public static bool WantSeriesBackdrops = true;
        public static bool WantSeasonImages = true;

        #endregion

        public static bool AnalyzeAudio = false;



        #region Miscelaneous
        public static bool EstimateAudioIdentification = true;
        public static bool EnableXmlBackup = false;
        public static string XmlBackupsDirectory = "";
        public static bool UIenabled = false;
        public static bool WantToDisableSources = true;
        public static string[] ImporterIDs = new string[1];
        public static int importing_limit = 0;
        public static string username = "";
        public static string password = "";


        #endregion

        public static bool EnableDurationMatching = true;


        public static bool WantBookDetails = true;

        #region Book Covers
        public static string WhereToSaveBookCover = "in the book's folder";
        public static string BookCoversFolder = "";
        #endregion


        

        #endregion




    }

    #endregion
}
