using System;
using System.Drawing;
using System.Windows.Forms;

namespace MeediFier
{
    internal static class ImageResizer
    {

        public static void ResizeImage(string originalFile, string newFile, int newWidth, int maxHeight, bool onlyResizeIfWider)
        {

            try
            {
                #region Load image file
                Image fullsizeImage = Image.FromFile(originalFile);
                #endregion

                // Prevent using images internal thumbnail
                fullsizeImage.RotateFlip(RotateFlipType.Rotate180FlipNone);
                fullsizeImage.RotateFlip(RotateFlipType.Rotate180FlipNone);

                var newImage = PerformResize(newWidth, maxHeight, onlyResizeIfWider, fullsizeImage);

                // Clear handle to original file so that we can overwrite it if necessary
                fullsizeImage.Dispose();

                // Save resized picture
                newImage.Save(originalFile);

            }
            catch (Exception e)
            {
                Debugger.LogMessageToFile("An unexpcted error occured while trying to resize an image. The error was: " + e );
                StatusForm.statusForm.TrayIcon.ShowBalloonTip(2000, "Error resizing image", "MediaFairy encountered an error while trying to resize an image. Please Debug.log for details on this error.", ToolTipIcon.Warning);

            }

        }


        private static Image PerformResize(int newWidth, int maxHeight, bool onlyResizeIfWider, Image fullsizeImage)
        {
            if (onlyResizeIfWider)
            {
                if (fullsizeImage.Width <= newWidth)
                    newWidth = fullsizeImage.Width;
            }

            int widthRatio = newWidth/fullsizeImage.Width;
            int newHeight = fullsizeImage.Height*widthRatio;

            if (newHeight > maxHeight)
            {
                // Resize with height instead
                int heightRatio = maxHeight/fullsizeImage.Height;
                newWidth = fullsizeImage.Width*heightRatio;
                newHeight = maxHeight;
            }

            Image newImage = fullsizeImage.GetThumbnailImage(newWidth, newHeight, null, IntPtr.Zero);
            return newImage;
        }
    }
}
