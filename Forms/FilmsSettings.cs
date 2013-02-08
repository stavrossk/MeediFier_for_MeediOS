using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MediaFairy.Forms
{
    public partial class FilmsSettings : Form
    {

        public FilmsSettings()
        {

            InitializeComponent();
        }

        private void FilmsSettings_Load(object sender, EventArgs e)
        {
            this.TopLevel = false;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Dock = DockStyle.Fill; 
        }
    }
}
