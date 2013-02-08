using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using CookComputing.XmlRpc;

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
    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public string token;
    public string status;
    [XmlRpcMissingMapping(MappingAction.Ignore)]
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
    public bool InternetConnectionAvailable;
    public bool OSDbIsOnline;
    public LoginResult OsDbLoginResult;
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

}

public struct itemUpdateParams
{
    public bool FileServerChecked;
    public bool FileServerIsOnline;
    public bool FileInUse;
    public bool CanWorkOnline;
    public bool OSIsOnline;
    public int CompletedItems;
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
        public static StatusForm statusForm = new StatusForm();
        public static string ImgToolTipText;


        public StatusForm()
        {
            InitializeComponent();
        }

        #region UI Methods


        private void StatusForm_Load(object sender, EventArgs e)
        {

            #region initialize form components
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowInTaskbar = true;
            this.ControlBox = false;
            this.Width = 560;
            this.Height = 450;

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


            if (Settings.UIenabled)
            {
                #region Fill Sections Datagrids

                if (Settings.EnableMoviesSectionUpdater)
                {
                    UpdateMoviescheckBox.Checked = true;

                    #region fill Movies Datagrid

                    foreach (int id in (int[])MediaSectionsAllocator.MoviesSection.GetAllItemIDs())
                    {
                        IMLItem Item = MediaSectionsAllocator.MoviesSection.FindItemByID(id);
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

                        if (Helpers.GetTagValueFromItem(Item,"fanart") != "" && Item.Tags["fanart"] != null)
                        {
                            if (File.Exists(Helpers.GetTagValueFromItem(Item,"fanart") ) )
                            {
                                ItemFanart = System.Drawing.Image.FromFile(Helpers.GetTagValueFromItem(Item,"fanart"));
                                ItemValues[3] = ItemFanart;
                            }

                        }

                        if ( !String.IsNullOrEmpty( Helpers.GetTagValueFromItem(Item,"ImdbID")) )
                        {
                            ItemValues[4] = Helpers.GetTagValueFromItem(Item,"ImdbID");
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

                if (Settings.EnableMusicSectionUpdater)
                {
                    UpdateMusicCheckbox.Checked = true;

                    #region fill Music Datagrid

                    foreach (int id in (int[])MediaSectionsAllocator.MusicSection.GetAllItemIDs())
                    {
                        IMLItem Item = MediaSectionsAllocator.MusicSection.FindItemByID(id);
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


                        if (Helpers.GetTagValueFromItem(Item,"Artist") != "" && Item.Tags["Artist"] != null)
                        {
                            ItemValues[3] = Helpers.GetTagValueFromItem(Item,"Artist");
                        }
                        if (Helpers.GetTagValueFromItem(Item,"Album") != "" && Item.Tags["Album"] != null)
                        {
                            ItemValues[4] = Helpers.GetTagValueFromItem(Item,"Album");
                        }
                        if (Helpers.GetTagValueFromItem(Item,"Title") != "" && Item.Tags["Title"] != null)
                        {
                            ItemValues[5] = Helpers.GetTagValueFromItem(Item,"Title");
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


            //if (Importer.EnableTvShowsSectionUpdater)
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
                StatusForm.statusForm.PauseButton.Text = "Pause";
            }
            else
            {
                ImportingPaused = true;
                StatusForm.statusForm.PauseButton.Text = "Continue";
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
            Settings.UserCancels = true;
        }

    
        private void tabPage3_Enter(object sender, EventArgs e)
        {
            UI.ShowControls();

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
            StatusForm.statusForm.tabControl1.SelectedTab = StatusForm.statusForm.StatusPage;
            Settings.ImportingStarted = true;
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void UpdateMoviescheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (UpdateMoviescheckBox.Checked)
                Settings.EnableMoviesSectionUpdater = true;
            else Settings.EnableMoviesSectionUpdater = false;
        }

        #endregion

        private void backdropSelectorGroupbox_Enter(object sender, EventArgs e)
        {

        }

        private void ShowHideButton_Click(object sender, EventArgs e)
        {
            if (this.Width == 560)
                UI.ShowControls();
            else UI.HideControls();

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(@"https://www.paypal.com/uk/cgi-bin/webscr?cmd=_flow&SESSION=xQI8Km-9XyllLC196GoWfmDlrPY1HEMWgroAD05JEv331xtKV7noE6YPVR8&dispatch=5885d80a13c0db1fa798f5a5f5ae42e779d4b5655493f6179c7b0ecd252eac84");
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(@"http://www.meedios.com/forum/viewtopic.php?t=3912");
        }

        private void ItemImageBox_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show(ImgToolTipText, ItemImageBox);
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        private void SettingsTree_Click(object sender, EventArgs e)
        {
            foreach (TreeNode rootNode in SettingsTree.Nodes)
            {
                MessageBox.Show(rootNode.Name);
                foreach (TreeNode childNode in rootNode.Nodes)
                {
                    if (childNode.Name == "FilmsNode")
                    {
                        MessageBox.Show(childNode.Name);
                        Forms.Settings f = new Forms.Settings();
                        this.splitContainer1.Panel2.Controls.Add(f);
                        f.Show();

                    }
                }

            }

        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void groupBox5_Enter(object sender, EventArgs e)
        {

        }

        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void StatusPage_Click(object sender, EventArgs e)
        {

        }


    }


}
