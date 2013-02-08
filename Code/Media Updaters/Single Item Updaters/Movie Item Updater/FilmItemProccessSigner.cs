using System;
using MeediOS;



namespace MediaFairy.Code.Media_Updaters.Single_Item_Updaters.Movie_Item_Updater
{


    class FilmItemProccessSigner
    {




        internal static void SignFilmItemProcess(IMLItem item)
        {
// ReSharper disable ReplaceWithSingleAssignment.True
            bool itemWasProccesed = true;


            if (MediaFairy.Settings.FilmCoverArtDownloaderIsEnabled
                && String.IsNullOrEmpty(item.ImageFile))
                itemWasProccesed = false;
// ReSharper restore ReplaceWithSingleAssignment.True

            if (MediaFairy.Settings.FilmBackdropArtDownloaderIsEnabled
                && String.IsNullOrEmpty(Helpers.GetTagValueFromItem(item, "fanart")))
                itemWasProccesed = false;


            if (MediaFairy.Settings.SimpleFilmDetailsDownloaderIsEnabled
                && String.IsNullOrEmpty(Helpers.GetTagValueFromItem(item, "ImdbID")))
                itemWasProccesed = false;


            if (MediaFairy.Settings.SimpleFilmDetailsDownloaderIsEnabled
                && String.IsNullOrEmpty(Helpers.GetTagValueFromItem(item, "Year")))
                itemWasProccesed = false;


            if (MediaFairy.Settings.EnableVideoHasher
                && String.IsNullOrEmpty(Helpers.GetTagValueFromItem(item, "Hash")))
                itemWasProccesed = false;

            if (MediaFairy.Settings.SimpleFilmDetailsDownloaderIsEnabled
                && String.IsNullOrEmpty(Helpers.GetTagValueFromItem(item, "Title")))
                itemWasProccesed = false;

            if (String.IsNullOrEmpty(Helpers.GetTagValueFromItem(item, "OriginalTitle")))
                itemWasProccesed = false;

            if (String.IsNullOrEmpty(Helpers.GetTagValueFromItem(item, "SortTitle")))
                itemWasProccesed = false;


            if (String.IsNullOrEmpty(Helpers.GetTagValueFromItem(item, "TMDbID"))
                &&

                ( MediaFairy.Settings.TMDbFilmDetailsEnabled 
               || MediaFairy.Settings.FilmBackdropArtDownloaderIsEnabled 
               || MediaFairy.Settings.TMDbPostersEnabled)

                )




            if (itemWasProccesed) 
            {
                item.Tags["MediaFairy-processed"] = "--processed--";
                item.SaveTags();
            }
            else
            {
                item.Tags["MediaFairy-processed"] = String.Empty;
                item.SaveTags();
            }


        }



    }




}
