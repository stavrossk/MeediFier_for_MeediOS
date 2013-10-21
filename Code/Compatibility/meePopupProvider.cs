using System.Globalization;
using System.Threading;
using MeediFier.ImportingEngine;
using MeediOS;

// ReSharper disable CheckNamespace
namespace MeediFier
// ReSharper restore CheckNamespace
{



    internal class MeePopupProvider
    {





        internal static void ImportMeePopupProviderField(IMLSection section)
        {

            if (!Settings.MeePopupProviderCompatibility)
                return;

            #region Process initialization feedback
            MainImportingEngine.ThisProgress.Progress(0, "Adding field meePopupProvider...");
            Thread.Sleep(300);
            #endregion

            section.AddNewTag("meePopupProvider");
      
            var itemsProcessed = 0; 

            section.BeginUpdate();
 
            #region Update Section items 
            foreach (var readOnlyItem in section.GetReadOnlyItems()) 
            { 
                string id = readOnlyItem.ID.ToString(CultureInfo.InvariantCulture);
 
                #region Calculate progress
                int percentComplete = (int)(itemsProcessed * (100.0 / section.ItemCount ) ); 
                if (percentComplete > 100) 
                    percentComplete = 100;

                itemsProcessed++;
 
                #endregion

                #region Poll for user cancellation request
                if (!MainImportingEngine.ThisProgress.Progress(percentComplete, "Processing Items")) 
                { 
                    section.CancelUpdate(); 
                    MainImportingEngine.ThisProgress.Progress(100, "Import Canceled");
                    return;
                }
                #endregion

                #region If field is populated, continue to next item
                if ( (string) readOnlyItem.Tags.Get("meePopupProvider", "") == id)
                    continue;
                #endregion

                #region Populate the item's field
                IMLItem item = section.FindItemByID(readOnlyItem.ID); 
                item.Tags["meePopupProvider"] = id; 
                item.SaveTags();
                #endregion

            } //endof foreach
            #endregion

            section.EndUpdate();
        } //endof function

    } //endof class

} //endof namespace
