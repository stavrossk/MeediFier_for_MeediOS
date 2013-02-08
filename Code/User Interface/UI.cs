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
using System.Threading;

#if USE_MEEDIO
using Meedio;
#elif USE_MEEDIOS
using MediaFairy.Properties;
using MeediOS;
#endif

namespace MediaFairy
{
    class UI
    {


        #region GUI functions

        public static void ShowControls()
        {

            if (StatusForm.statusForm.Width != 640)
            {

                for (int i = 560; i <= 860; i = i + 10)
                {
                    StatusForm.statusForm.Width = i;
                    StatusForm.statusForm.pictureBox1.Refresh();
                    StatusForm.statusForm.ItemImageBox.Refresh();
                    StatusForm.statusForm.backdropSelectorGroupbox.Refresh();
                    StatusForm.statusForm.OperationControlsGroup.Refresh();
                    StatusForm.statusForm.ConfirmImagePanel.Refresh();
                    StatusForm.statusForm.ConfirmImageCheckBox.Refresh();

                    //Thread.Sleep(5);

                }

                StatusForm.statusForm.ShowHideButton.Image = Resources.hide;
            }


        }

        public static void HideControls()
        {


            for (int i = 640; i >= 560; i = i - 10)
            {
                StatusForm.statusForm.Width = i;
                StatusForm.statusForm.ItemImageBox.Refresh();
                StatusForm.statusForm.backdropSelectorGroupbox.Refresh();
                StatusForm.statusForm.OperationControlsGroup.Refresh();
                StatusForm.statusForm.ConfirmImagePanel.Refresh();

                //Thread.Sleep(5);

            }



            StatusForm.statusForm.ShowHideButton.Image = Resources.show;
        }


        public static bool PreviewImage(string imagePath, bool IsBackdrop)
        {

            StatusForm.statusForm.ItemImageBox.SizeMode = PictureBoxSizeMode.Zoom;
            //StatusForm.statusForm.ItemImageBox.ImageLocation = imagePath;

            #region Try to load image
            try
            {
                StatusForm.statusForm.ItemImageBox.Load(imagePath);
            }
            catch
            {
                try
                {
                    StatusForm.statusForm.ItemImageBox.Load(imagePath);
                }
                catch
                {
                    return false;
                }
            }
            #endregion

            Uri imageUrl = new Uri(imagePath);

            string OnlineSource = imageUrl.Host;

            System.Drawing.Image image = StatusForm.statusForm.ItemImageBox.Image;

            StatusForm.ImgToolTipText = "Image Source: " + OnlineSource + Environment.NewLine +
            "Resolution: " + image.Width + "x" + image.Height + Environment.NewLine +
            "Format: JPEG" + Environment.NewLine +
            "Filename: " + StatusForm.statusForm.ItemImageBox.ImageLocation;



            if (!StatusForm.statusForm.Visible)
                StatusForm.statusForm.Show();

            Application.DoEvents();

            if (IsBackdrop)
                return true;

            //Thread.Sleep(1000);

            if (Settings.AlwaysPromptForImages)
                StatusForm.statusForm.ConfirmImageCheckBox.Checked = true;

            StatusForm.statusForm.ConfirmImageCheckBox.Visible = true;

            StatusForm.statusForm.Refresh();
            Application.DoEvents();

            ShowControls();

            Helpers.UpdateProgress("Updating Films Section", "Waiting for poster selection...", null);

            if (ConfirmImage())
                return true;
            else return false;

        }


        public static bool ConfirmImage()
        {

            if (StatusForm.statusForm.ConfirmImage || Settings.AlwaysPromptForImages)
            {
                StatusForm.statusForm.ConfirmImageDialogResult = DialogResult.Cancel;
                StatusForm.statusForm.ConfirmImagePanel.Visible = true;
                StatusForm.statusForm.ConfirmImageCheckBox.Visible = false;

                while (StatusForm.statusForm.ConfirmImageDialogResult == DialogResult.Cancel)
                {
                    Thread.Sleep(50);
                    Application.DoEvents();
                }

                StatusForm.statusForm.ConfirmImagePanel.Visible = false;
                StatusForm.statusForm.ConfirmImageCheckBox.Visible = true;

                if (StatusForm.statusForm.ConfirmImageDialogResult == DialogResult.Yes)
                {
                    StatusForm.statusForm.Hide();
                    return true;
                }
                else return false;

            }
            else return true;

        }


        public static string UserSelectsSeriesBanner(IMLItem Item, IList<TVDBLibrary.ITVDBBanner> banners, string BannerType)
        {
            string posterUrl = "";
            bool StatusFormWasVisible = StatusForm.statusForm.Visible;

            if (StatusFormWasVisible)
            {
                UI.PreviewImage(banners[0].Path, true); //preview first image on list
            }


            #region ask user to select desired backdrop
            if (banners.Count > 1 && Settings.AlwaysPromptForImages)
            {
                Helpers.UpdateProgress("Updating TV Section", "Waiting for " + BannerType + " selection...", Item);
                StatusForm.statusForm.ConfirmImageCheckBox.Visible = false;
                StatusForm.statusForm.backdropSelectorGroupbox.Visible = true;
                StatusForm.statusForm.SelectBackropDialogResult = DialogResult.Cancel;
                StatusForm.statusForm.backdropSelectorLabe.Text = Resources.ImageSelectionEngine_UserSelectsImage_displaying_ + BannerType + " 1 of " + banners.Count + "";

                UI.PreviewImage(banners[0].Path, true); //preview first image on list




                int SelectedBackdrop = 0;

                while (StatusForm.statusForm.SelectBackropDialogResult != DialogResult.OK)
                {
                    Application.DoEvents();
                    Thread.Sleep(50);

                    #region Prev pressed
                    if (StatusForm.statusForm.SelectBackropDialogResult == DialogResult.No)
                    {

                        if (SelectedBackdrop != 0)
                        {
                            SelectedBackdrop--;
                            StatusForm.statusForm.backdropSelectorLabe.Text = Resources.ImageSelectionEngine_UserSelectsImage_displaying_ + BannerType + " " + (SelectedBackdrop + 1) + Resources.ImageSelectionEngine_UserSelectsImage__of_ + banners.Count + "";
                            UI.PreviewImage(banners[SelectedBackdrop].Path, true);
                            StatusForm.statusForm.SelectBackropDialogResult = DialogResult.Cancel;
                        }

                    }
                    #endregion

                    #region Next pressed
                    if (StatusForm.statusForm.SelectBackropDialogResult == DialogResult.Yes)
                    {

                        if (SelectedBackdrop < (banners.Count - 1))
                        {
                            SelectedBackdrop++;
                            StatusForm.statusForm.backdropSelectorLabe.Text = Resources.ImageSelectionEngine_UserSelectsImage_displaying_ + BannerType + " " + (SelectedBackdrop + 1) + Resources.ImageSelectionEngine_UserSelectsImage__of_ + banners.Count + "";
                            UI.PreviewImage(banners[SelectedBackdrop].Path, true);
                            StatusForm.statusForm.SelectBackropDialogResult = DialogResult.Cancel;
                        }

                    }
                    #endregion


                }

                posterUrl = banners[SelectedBackdrop].Path;
                StatusForm.statusForm.ConfirmImageCheckBox.Visible = true;
                StatusForm.statusForm.backdropSelectorGroupbox.Visible = false;

                if (!StatusFormWasVisible)
                    StatusForm.statusForm.Hide();
            }
            #endregion
            else
            {
                posterUrl = banners[0].Path;
            }

            return posterUrl;

        }


        #endregion




    }
}
