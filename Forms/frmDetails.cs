using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

#if USE_MEEDIO
using Meedio;
#elif USE_MEEDIOS
using MediaFairy.IMDb;
using MeediOS;
#endif

namespace MediaFairy
{
    public partial class frmDetails : Form
    {

        public frmDetails(IIMDbSearchResult result)
        {
            
            FixFPU();
            InitializeComponent();



            IIMDbMovie movie 
                = IMDbMovieDetailsDownloader
                .DownloadMovieDetailsFromIMDb
                (result.IMDb_ID, false);



            lblOverview.DataBindings.Clear();
            lblOverview.DataBindings.Add("Text", imdbBindingSource, "OverviewShort");
            lblRating.DataBindings.Clear();
            lblRating.DataBindings.Add("Text", imdbBindingSource, "Rating");
            lblRuntime.DataBindings.Clear();
            lblRuntime.DataBindings.Add("Text", imdbBindingSource, "Runtime");
            lblTitle.DataBindings.Clear();
            lblTitle.DataBindings.Add("Text", imdbBindingSource, "Title");
            lblYear.DataBindings.Clear();
            lblYear.DataBindings.Add("Text", imdbBindingSource, "Year");
            txtOverviewLong.DataBindings.Clear();
            txtOverviewLong.DataBindings.Add("Text", imdbBindingSource, "OverviewLong");
            imdbBindingSource.DataSource = movie;




        }



    }
}
