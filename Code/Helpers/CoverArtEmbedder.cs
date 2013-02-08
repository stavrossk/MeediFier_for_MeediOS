using System.IO;
using System.Net.Mime;
using MeediOS;
using TagLib;
using File = TagLib.File;



namespace MediaFairy.Code.Helpers
{


    class CoverArtEmbedder
    {




        internal static bool EmbedCoverArt
            (IMLItem item, string imagePath ,
            bool trackHasCover)
        {
            if (!MediaFairy.Settings.EmbedAlbumCover)
                return false;

            if (trackHasCover)
                return false;

            FileInfo imagefile = new FileInfo(imagePath);

            if (imagefile.Extension != ".jpg" && imagefile.Extension != ".JPG" && imagefile.Extension != ".jpeg" && imagefile.Extension != ".JPEG")
                return false;


            
            File audiofile = File.Create(item.Location);
            IPicture[] pictures = audiofile.Tag.Pictures;

            if (pictures.Length < 1)
            {
                MediaFairy.Helpers.UpdateProgress("Updating Music Section", "Embedding cover image in audio file '" + item.Name + "'...", null);
                //create the picture for the album cover
                TagLib.Picture picture = new TagLib.Picture(imagePath);

                //create Id3v2 Picture Frame
                TagLib.Id3v2.AttachedPictureFrame albumCoverPictFrame = 
                    new TagLib.Id3v2.AttachedPictureFrame(picture)
                        {
                            MimeType =
                                MediaTypeNames.Image.Jpeg,
                            Type = PictureType.FrontCover
                        };

                //Id3v2 allows more than one type of image, just one needed
                TagLib.IPicture ipict = albumCoverPictFrame;
                ipict.Type = PictureType.FrontCover;
                ipict.Data = albumCoverPictFrame.Data;
                TagLib.IPicture[] pictFrame = { ipict };
                pictFrame[0] = ipict;
                audiofile.Tag.Pictures = pictFrame;
                audiofile.Save();
            }
            return true;
        }


    }




}
