using System;
using System.Collections.Generic;
using System.Windows.Forms;


namespace MeediFier
{




    public partial class FrmSelect : Form
    {


  




        #region Variables


        private IList<IIMDbSearchResult> _imdbList;
        private IIMDbSearchResult _imdbMovie;


        private readonly string _originalTitle = string.Empty;
        private bool _isCustom;

        public enum SearchType { MovieXml, IMDb };
        #endregion

        #region Constructor


        public FrmSelect(string title, string country, IList<IIMDbSearchResult> list)
        {
            FixFPU();
            InitializeComponent();
            _imdbList = list;


            txtSearch.KeyPress += new KeyPressEventHandler(TxtSearchKeyPress);


            _originalTitle = title;
            lblMatchTitle.Text = "Matches found for: " + title;
            FillList(true);
        }




        public FrmSelect(string title, string country, SearchType searchProvider)
        {
            FixFPU();
            InitializeComponent();
            if (searchProvider == SearchType.MovieXml)
            {
                txtSearch.KeyPress += new KeyPressEventHandler(TxtSearchKeyPress);
                lblMatchTitle.Text = "Matches found for: " + title;
            }
            else
            {
                _imdbList = new List<IIMDbSearchResult>();
                txtSearch.KeyPress += new KeyPressEventHandler(TxtSearchKeyPress);
                lblMatchTitle.Text = "Matches found for: " + title;
            }



        }
        
        #endregion

        #region Properties
        public object SelectedResult
        {
            get
            {
    
                
                return _imdbMovie;
            }
        }

        public bool IsCustom
        {
            get { return _isCustom; }
        }
        #endregion



        private void TxtSearchKeyPress
            (object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
                btnSearch.PerformClick();
        }

        private void Search()
        {

            if (txtSearch.Text.Trim() == "") 
                return;


            Cursor = Cursors.WaitCursor;
            btnSearch.Enabled = false;

           
            _imdbList = IMDb.IMDbConventionalFilmSearchEngine
                .SearchForTitleReturnResults
                (txtSearch.Text.Trim());
 
            lblMatchTitle.Text = "Matching: " + txtSearch.Text;
            FillList(false);
            btnSearch.Enabled = true;
            this.Cursor = Cursors.Default;
        }

        private void FillList(bool suppressMsg)
        {
            lvwItems.Items.Clear();

           
            if (_imdbList != null && _imdbList.Count > 0)
                for (int i = 0; i < _imdbList.Count; i++)
                {
                    ListViewItem item = new ListViewItem();
                    item.Text = _imdbList[i].Title;
                    item.SubItems.Add(_imdbList[i].Year);
                    item.SubItems.Add(_imdbList[i].IMDb_ID);
                    lvwItems.Items.Add(item);
                }
            else
                if (!suppressMsg)
                    MessageBox.Show("No results found.", "Search Results", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void RemoveVideoGameTitles(ref IIMDbMovieCollection list)
        {
            try
            {
                if (list != null)
                    for (int i = list.Count - 1; i >= 0; i--)
                    {
                        if (list[i].Year.ToLower().Trim().IndexOf("vg") > -1)
                            list.RemoveAt(i);
                    }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            Search();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _imdbMovie = null;
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (lvwItems.SelectedItems.Count > 0)
            {
    
                _imdbMovie = _imdbList[lvwItems.SelectedItems[0].Index];

                this.DialogResult = DialogResult.OK;
            }
            else
                MessageBox.Show("Please select a movie.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        private void btnSubmitMyTitle_Click(object sender, EventArgs e)
        {
            if (lvwItems.SelectedItems.Count > 0)
            {
                    _imdbMovie = _imdbList[lvwItems.SelectedItems[0].Index];
                    _imdbMovie.Title = _originalTitle;
                
                _isCustom = true;

                this.DialogResult = DialogResult.OK;
            }
            else
                MessageBox.Show("Please select a movie.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        private void viewInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lvwItems.SelectedItems.Count > 0)
            {
         
                


                IIMDbSearchResult result = _imdbList[lvwItems.SelectedItems[0].Index];
                frmDetails frm = new frmDetails(result );
                frm.ShowDialog();
                frm.Close();
                frm.Dispose();
                frm = null;
              
                
            }
        }

        private void frmSelect_Load(object sender, EventArgs e)
        {











        }

        private void button1_Click(object sender, EventArgs e)
        {
            _imdbMovie = null;
            this.DialogResult = DialogResult.Abort;
        }
        


    }
}