using System;
using System.Diagnostics;
using System.IO;
using IWshRuntimeLibrary;

using MeediOS;
using File = System.IO.File;

namespace MediaFairy.MediaImporters
{

    internal class PcGameImporter
    {

        internal static bool ImportPcGame(WshShell shell, string linkPathName, IMLSection pcGamesSection )
        {

            IWshShortcut link = (IWshShortcut)shell.CreateShortcut(linkPathName);
            FileInfo linkFile = new FileInfo(link.FullName);

            string shortcutName = linkFile.Name;
            shortcutName = shortcutName.Remove(shortcutName.Length - 4, 4);

            FileVersionInfo myFileVersionInfo;

            try
            {
                myFileVersionInfo = FileVersionInfo.GetVersionInfo(link.TargetPath);
            }
            catch (Exception e)
            {
                Debugger.LogMessageToFile("An unexpected error ocurred while trying to retrieve this game's version information from the game's executable. The error was: "
                    + Environment.NewLine + e );

                return false;
            }

            string title = myFileVersionInfo.ProductName;
            string company = myFileVersionInfo.CompanyName;
            string language = myFileVersionInfo.Language;
            string description = myFileVersionInfo.FileDescription;
            string version = myFileVersionInfo.FileVersion;

            IMLItem item = pcGamesSection.FindItemByLocation(link.TargetPath);

            if (item == null)
                item = pcGamesSection.AddNewItem(title, link.TargetPath);
            else return false; 


            if (!String.IsNullOrEmpty(title))
            {
                item.Name = title;
                item.Tags["Title"] = title;
            }
            else
            {
                item.Name = shortcutName;
                item.Tags["Title"] = shortcutName;
            }
           
            item.Tags["Company"] = company;
            item.Tags["Language"] = language;
            item.Tags["Description"] = description;
            item.Tags["Version"] = version;
            item.SaveTags();
          
            return true;
        }

        internal static void ImportPcGames(IMLImportProgress progress, IMLSection gamesSection)
        {

            if ((!Settings.EnableGamesUpdater && !Settings.EnableGamesImporter))
                return;
                            
            if (String.IsNullOrEmpty(Settings.GameShortcutsLocation) )
                return;

            if ( !File.Exists(Settings.GameShortcutsLocation) )
            return;


            Importer.ThisProgress = progress;
            //Importer.GeneralStatus = "Updating Games Section";
            Importer.SpecialStatus = String.Empty;
            Importer.ThisProgress.Progress(Importer.CurrentProgress, "Updating Games Section...");

            DirectoryInfo pcGamesFolder = new DirectoryInfo(Settings.GameShortcutsLocation);
            FileInfo[] shortcuts = pcGamesFolder.GetFiles("*.lnk", SearchOption.AllDirectories);
            //IMLSection GamesSection = Importer.library.FindSection("Games", true);

            #region Man importing engine

            int itemCounter = 0;
            WshShell shell = new WshShellClass();

            gamesSection.BeginUpdate();

            foreach (FileInfo shortcut in shortcuts)
            {
                int progressStep = 100 / shortcuts.Length;
                Importer.CurrentProgress = 100 - (progressStep * (shortcuts.Length - itemCounter));

                ImportPcGame(shell, shortcut.FullName, gamesSection);
                itemCounter++;
            }

            gamesSection.EndUpdate();

            #endregion

            #region Update

            if (!Settings.EnableGamesUpdater) return;


           

            gamesSection.BeginUpdate();
            itemCounter = 0;
            int[] allItemIDs = gamesSection.GetAllItemIDs();

            foreach (int id in allItemIDs)
            {
                IMLItem item = gamesSection.FindItemByID(id);
                int progressStep = 100 / allItemIDs.Length;
                Importer.CurrentProgress = 100 - (progressStep * (allItemIDs.Length - itemCounter));
                Downloaders.DownloadGameDetailsFromMobyGames(item);

                Helpers.UserCancels(Importer.SpecialStatus, item);

                itemCounter++;
            }

            gamesSection.EndUpdate();
            
            

            #endregion

        } //endof function


    } //endof class


} //endof namespace
