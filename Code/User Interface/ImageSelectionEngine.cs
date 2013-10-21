using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using MeediFier.Properties;
using MeediOS;



namespace MeediFier.Code.User_Interface
{


    class ImageSelectionEngine
    {






        internal static string UserSelectsImage
            (IMLItem item,List<string> imageUrls,
             List<string> previewImages, string imageType)
        {
            string posterUrl;

            if (imageUrls.Count == 0 || previewImages.Count == 0)

                #region Preview first image
                if (StatusForm.statusForm.Visible)
                {
                    if (previewImages[0].Contains(".jpg"))
                        UI.PreviewImage(previewImages[0], true); //preview first image on list               
                }
            #endregion


            #region ask user to select desired image
            if (imageUrls.Count > 1 && MeediFier.Settings.AlwaysPromptForImages)
            {

                Helpers.UpdateProgress
                    ("Updating Films Section", 
                     "Waiting for " + imageType +  " selection...", item);
                
                StatusForm.statusForm
                    .ConfirmImageCheckBox.Visible = false;
                
                StatusForm.statusForm
                    .backdropSelectorGroupbox.Visible = true;
                
                StatusForm.statusForm
                    .SelectBackropDialogResult = DialogResult.Cancel;
                
                StatusForm.statusForm
                    .backdropSelectorLabe.Text 
                    = Resources.ImageSelectionEngine_UserSelectsImage_displaying_ 
                    + imageType + " 1 of " 
                    + Convert.ToString(imageUrls.Count) + "";


                #region Preview first image
                if (!StatusForm.statusForm.Visible)
                {
                    if (previewImages[0].Contains(".jpg"))
                    {
                        UI.PreviewImage(previewImages[0], true); //preview first image on list
                    }
                }
                #endregion


                StatusForm.statusForm.Show();

                if (StatusForm.statusForm.Width == 560)
                    UI.ShowControls();

                Application.DoEvents();


                int selectedBackdrop = 0;

                while (StatusForm.statusForm.SelectBackropDialogResult != DialogResult.OK)
                {
                    Application.DoEvents();
                    Thread.Sleep(50);

                    #region Prev pressed
                    if (StatusForm.statusForm.SelectBackropDialogResult == DialogResult.No)
                    {

                        if (selectedBackdrop != 0)
                        {
                            selectedBackdrop--;
                            StatusForm.statusForm.backdropSelectorLabe.Text = Resources.ImageSelectionEngine_UserSelectsImage_displaying_ + imageType + " " + (selectedBackdrop + 1) + Resources.ImageSelectionEngine_UserSelectsImage__of_ +  imageUrls.Count + "";
                            UI.PreviewImage(previewImages[selectedBackdrop], true);
                            StatusForm.statusForm.SelectBackropDialogResult = DialogResult.Cancel;
                        }

                    }
                    #endregion

                    #region Next pressed

                    if (StatusForm.statusForm.SelectBackropDialogResult
                        != DialogResult.Yes) 
                        continue;


                    if (selectedBackdrop >= (imageUrls.Count - 1))
                        continue;


                    selectedBackdrop++;
                    StatusForm.statusForm.backdropSelectorLabe.Text = Resources.ImageSelectionEngine_UserSelectsImage_displaying_ + imageType + " " + (selectedBackdrop + 1) + Resources.ImageSelectionEngine_UserSelectsImage__of_ + imageUrls.Count + "";
                    UI.PreviewImage(previewImages[selectedBackdrop], true);
                    StatusForm.statusForm.SelectBackropDialogResult = DialogResult.Cancel;

                    #endregion


                }

                posterUrl = imageUrls[selectedBackdrop];
                StatusForm.statusForm.ConfirmImageCheckBox.Visible = true;
                StatusForm.statusForm.backdropSelectorGroupbox.Visible = false;
                StatusForm.statusForm.Hide();
            }
                #endregion
            else
            {
                posterUrl = imageUrls[0];
            }

            return posterUrl;
        }





    }





}
