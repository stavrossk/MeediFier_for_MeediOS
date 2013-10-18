namespace MeediFier
{
    partial class frmDetails
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        [System.Runtime.InteropServices.DllImport("msvcrt.dll")]
        static extern int _controlfp(int IN_New, int IN_Mask);
        private const int _MCW_EW = 0x8001F;
        private const int _EM_INVALID = 0x10;
        private void FixFPU()
        {
            _controlfp(_MCW_EW, _EM_INVALID);
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblYear = new System.Windows.Forms.Label();
            this.lblOverview = new System.Windows.Forms.Label();
            this.lblRating = new System.Windows.Forms.Label();
            this.lblRuntime = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtOverviewLong = new System.Windows.Forms.TextBox();
            this.movieXmlBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.imdbBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.movieXmlBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imdbBindingSource)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.imdbBindingSource, "Title", true));
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.lblTitle.Location = new System.Drawing.Point(16, 11);
            this.lblTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(357, 23);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Title";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblYear
            // 
            this.lblYear.AutoSize = true;
            this.lblYear.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.imdbBindingSource, "Year", true));
            this.lblYear.Location = new System.Drawing.Point(147, 18);
            this.lblYear.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblYear.Name = "lblYear";
            this.lblYear.Size = new System.Drawing.Size(38, 17);
            this.lblYear.TabIndex = 1;
            this.lblYear.Text = "Year";
            // 
            // lblOverview
            // 
            this.lblOverview.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.imdbBindingSource, "Overview", true));
            this.lblOverview.Location = new System.Drawing.Point(14, 149);
            this.lblOverview.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblOverview.Name = "lblOverview";
            this.lblOverview.Size = new System.Drawing.Size(357, 78);
            this.lblOverview.TabIndex = 2;
            this.lblOverview.Text = "Overview";
            // 
            // lblRating
            // 
            this.lblRating.AutoSize = true;
            this.lblRating.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.imdbBindingSource, "Rating", true));
            this.lblRating.Location = new System.Drawing.Point(147, 48);
            this.lblRating.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblRating.Name = "lblRating";
            this.lblRating.Size = new System.Drawing.Size(49, 17);
            this.lblRating.TabIndex = 3;
            this.lblRating.Text = "Rating";
            // 
            // lblRuntime
            // 
            this.lblRuntime.AutoSize = true;
            this.lblRuntime.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.imdbBindingSource, "Runtime", true));
            this.lblRuntime.Location = new System.Drawing.Point(147, 74);
            this.lblRuntime.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblRuntime.Name = "lblRuntime";
            this.lblRuntime.Size = new System.Drawing.Size(32, 17);
            this.lblRuntime.TabIndex = 4;
            this.lblRuntime.Text = "999";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(178, 74);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 17);
            this.label1.TabIndex = 5;
            this.label1.Text = "min.";
            // 
            // txtOverviewLong
            // 
            this.txtOverviewLong.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.imdbBindingSource, "OverviewLong", true));
            this.txtOverviewLong.Location = new System.Drawing.Point(19, 269);
            this.txtOverviewLong.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtOverviewLong.Multiline = true;
            this.txtOverviewLong.Name = "txtOverviewLong";
            this.txtOverviewLong.ReadOnly = true;
            this.txtOverviewLong.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtOverviewLong.Size = new System.Drawing.Size(352, 104);
            this.txtOverviewLong.TabIndex = 6;
            // 
            // imdbBindingSource
            // 
            this.imdbBindingSource.DataSource = typeof(IIMDbMovie);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.label2.Location = new System.Drawing.Point(135, 248);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(112, 17);
            this.label2.TabIndex = 7;
            this.label2.Text = "Plot Overview:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.lblYear);
            this.groupBox1.Controls.Add(this.lblRating);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.lblRuntime);
            this.groupBox1.Location = new System.Drawing.Point(19, 37);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(352, 109);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Basic info:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.label3.Location = new System.Drawing.Point(98, 18);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 17);
            this.label3.TabIndex = 6;
            this.label3.Text = "Year:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.label4.Location = new System.Drawing.Point(85, 48);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(60, 17);
            this.label4.TabIndex = 7;
            this.label4.Text = "Rating:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.label5.Location = new System.Drawing.Point(74, 74);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(72, 17);
            this.label5.TabIndex = 8;
            this.label5.Text = "Runtime:";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(138, 380);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(98, 34);
            this.button1.TabIndex = 9;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // frmDetails
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(389, 437);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtOverviewLong);
            this.Controls.Add(this.lblOverview);
            this.Controls.Add(this.lblTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmDetails";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Movie Info";
            ((System.ComponentModel.ISupportInitialize)(this.movieXmlBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imdbBindingSource)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.BindingSource imdbBindingSource;
        private System.Windows.Forms.Label lblYear;
        private System.Windows.Forms.Label lblOverview;
        private System.Windows.Forms.Label lblRating;
        private System.Windows.Forms.Label lblRuntime;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtOverviewLong;
        private System.Windows.Forms.BindingSource movieXmlBindingSource;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button1;
    }
}