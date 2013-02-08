using System;
using System.IO;
using System.Threading;
using MeediOS;

namespace MediaFairy
{


    internal static class MissingItemRemover
    {


        internal static bool DeleteMissingItem
            (bool deleteMissingLibraryEntries,
            bool isUNC, bool fileServerIsOnline, 
            string location, IMLSection section, 
            IMLItem Item)
        {


            if (!ReturnCases(deleteMissingLibraryEntries, isUNC,
                fileServerIsOnline, location, Item))
                return false;


            DeleteMediaImagesAndLibraryItem(section, Item);


            return true;
        }



        private static bool ReturnCases(bool deleteMissingLibraryEntries, 
            bool isUNC, bool fileServerIsOnline, string location,
                                        IMLItem item)
        {


            if (!deleteMissingLibraryEntries)
                return false;


            //if (!location.StartsWith(Settings.DvdDrive) || String.IsNullOrEmpty(Settings.DvdDrive))
            //{

            if (isUNC && !fileServerIsOnline)
                return false;


            Debugger.LogMessageToFile(String.Format("Validating item {0}...", item.Name));
            Helpers.UpdateProgress("Performing Diagnostic Operations", String.Format("Validating item {0}...", item.Name), item);


            return !File.Exists(location);
        
        
        }



        private static void DeleteMediaImagesAndLibraryItem
            (IMLSection section, IMLItem item)
        {


            Debugger.LogMessageToFile(String.Format("The non-existent item {0}will be deleted from section.",
                item.Name));


            Helpers.UpdateProgress
                ("Performing Diagnostic Operations",
                String.Format("The non-existent item " +
                              "{0} will be deleted from section.",
                                item.Name), item);

            Thread.Sleep(100);


            string imageFile = item.ImageFile;
            string fanart = (string) item.Tags["fanart"];


            DeleteCoverAndFanartImages
                (fanart, imageFile);


            section.DeleteItem(item);
        
        
        }






        private static void DeleteCoverAndFanartImages(string fanart, string imageFile)
        {
            DeleteCoverImageOfMissingMediaFile(imageFile);

            DeleteFanartImageOfMissingMediaFile(fanart);
        }



        private static void DeleteFanartImageOfMissingMediaFile(string fanart)
        {

            if (!Settings.DeleteFanartImageOfMissingMediaFiles) return;

            if (!String.IsNullOrEmpty(fanart))
                File.Delete(fanart);

        }



        private static void DeleteCoverImageOfMissingMediaFile(string imageFile)
        {

            if (!Settings.DeleteCoverImageOfMissingMediaFiles) return;


            if (!String.IsNullOrEmpty(imageFile))
                File.Delete(imageFile);

        }






    }



}
