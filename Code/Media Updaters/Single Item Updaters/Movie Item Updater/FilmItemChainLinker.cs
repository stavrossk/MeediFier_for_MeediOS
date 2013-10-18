using System.Windows.Forms;
using MeediFier.ImportingEngine;
using MeediOS;



namespace MeediFier.Code.Media_Updaters.Single_Item_Updaters.Movie_Item_Updater
{

    class FilmItemChainLinker
    {




        internal static void LinkFilmItemChains
            (IMLSection moviesSection,
             IMLItemList allFilmItems,
             IMLItem Item)
        {

            if (!MeediFier.Settings
                .FilmItemChainLinkerIsEnabled)
                return;


            if (allFilmItems == null
                || allFilmItems.Count <= 0)
                return;


            MainImportingEngine.ThisProgress.Progress
                (MainImportingEngine.CurrentProgress,
                 "scanning for un-linked files...");


            Application.DoEvents();


            foreach (IMLItem item in allFilmItems)
            {


                string itemImdbID = Helpers
                    .GetTagValueFromItem
                    (item, "ImdbID");

                string itemToCompareImdbID = Helpers
                    .GetTagValueFromItem
                    (Item, "ImdbID");



                if (item.Location == Item.Location
                    || itemImdbID != itemToCompareImdbID)
                    continue;


                if (item.Location.StartsWith("|")
                    || Item.Location.StartsWith("|"))
                    continue;

                Item.Location =
                    "|" + Item.Location +
                    "|" + item.Location + "|";

                Item.SaveTags();

                moviesSection.DeleteItem(item);
            }


        }


    }




}
