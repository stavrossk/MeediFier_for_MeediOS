using System;
using System.IO;
using System.Management;
using System.Threading;
using System.Windows.Forms;
using Chilkat;
using ICSharpCode.SharpZipLib.Zip;
using MediaFairy.ImportingEngine;

namespace MediaFairy.ArchiveProcessingEngines
{

    internal class ArchiveExtractor
    {

        internal static bool ExtractArchive(string archive, string destination)
        {

            //REFACTOR: Only create RAR and ZIP classes after discovering that the file actually has a .zip or .rar extension
            FileInfo archiveFI = new FileInfo(archive);
            Rar rar = new Rar();
            FastZip fz = new FastZip();

            double archivesize = archiveFI.Length * 2;
            char driveLetter = archiveFI.FullName[0];


            if (!CheckDiskSpaceQuota(archivesize, driveLetter)) return false;


            if (archiveFI.Extension == ".rar" || archiveFI.Extension == ".RAR")
                return ExtractRarArchive(archive, destination, archiveFI, rar);

            // ReSharper disable ConvertIfStatementToReturnStatement
            if (archiveFI.Extension == ".zip" || archiveFI.Extension == ".ZIP")
            // ReSharper restore ConvertIfStatementToReturnStatement
                return ExtractZipArchive(archive, destination, fz);

            //TODO: Should this return false?
            return true;

        }


        private static bool ExtractZipArchive(string archive, string destination, FastZip fz)
        {

            DirectoryInfo newdirFI;

            if (!Directory.Exists(destination))
            {
                newdirFI = Directory.CreateDirectory(destination);

                if (!Directory.Exists(newdirFI.FullName))
                {
                    //MessageBox.Show("Directory " + destination + " could not be created.");
                    return false;
                }
            }
            else newdirFI = new DirectoryInfo(destination);


            try
            {
                Thread.Sleep(500);
                fz.ExtractZip(archive, newdirFI.FullName, "");
            }
            catch (Exception e)
            {
                Debugger.LogMessageToFile("The archive " + archive + " could not be extracted to destination " + destination +
                                          ". The following error ocurred: " + e);
            }



            return true;
        }


        private static bool ExtractRarArchive(string archive, string destination, FileInfo archiveFI, Rar rar)
        {

            Debugger.LogMessageToFile("[Archive extractor] Found RAR archive: " + archiveFI.FullName);

            #region Open
            if (!rar.Open(archive))
            {
                Debugger.LogMessageToFile("[Archive extractor] Failed to open archive: " + rar.LastErrorText);
                //MessageBox.Show("Failed to open archive " + rar.LastErrorText);
                return false;
            }
            Debugger.LogMessageToFile("[Archive extractor] Archive opened succesfully.");
            #endregion

            #region Extract
            //TODO: Check if destination directory exists and if not, create it
            if (!rar.Unrar(destination))
            {
                Debugger.LogMessageToFile("[Archive extractor] Failed to extract archive: " + rar.LastErrorText);
                //MessageBox.Show("Failed to extract archive " + rar.LastErrorText);
                return false;
            }
            Debugger.LogMessageToFile("[Archive extractor] Archive was extracted succesfully.");
            #endregion

            rar.Close();

            DeleteLeftOfMultiparts(rar);

            return true;

        }



        private static bool CheckDiskSpaceQuota(double archivesize, char driveLetter)
        {
            if (driveLetter != '\\')
            {
                ManagementObject disk = new ManagementObject("win32_logicaldisk.deviceid=\"" + driveLetter + ":\"");
                disk.Get();
                double freespace = Convert.ToDouble(disk["FreeSpace"]);
                double minimalSpace = Settings.freespaceThreshold*1048576;

                if ((freespace - (archivesize + minimalSpace)) < 0)
                    return false;
            }
            return true;
        }


        private static void DeleteLeftOfMultiparts(Rar rar)
        {
            if (!Settings.DeleteArchiveAfterExtraction) return;

            if (rar.NumEntries <= 1) return;

            for (int i = 0; i < rar.NumEntries; i++)
            {

                RarEntry entry = rar.GetEntryByIndex(i);
                string filename = entry.Filename;

                #region TRY to delete the rar file
                try
                {
                    File.Delete(filename);
                }
                catch (Exception e)
                {
                    Debugger.LogMessageToFile("The extracted archive " + filename +
                                              " could not be deleted. The error was: " + e);
                }
                #endregion

            }

        }














        internal static bool DetectAndExtractArchive(FileInfo file)
        {
            Application.DoEvents();

            if (!Settings.EnableArchiveExtractor)
                return false;

            if (file.Extension != ".rar" && file.Extension != ".zip")
                return false;

            if (file.Name.StartsWith("tt"))
                return false;

            Application.DoEvents();
            Debugger.LogMessageToFile("Extracting archive " + file.Name + "...");
            MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress,
                                           "Extracting archive " + file.Name + "...");
            Application.DoEvents();


            var destination = ConstructExtractionDestination(file);

            Application.DoEvents();
            ExtractArchive(file.FullName, destination);
            Application.DoEvents();

            DeleteArchiveAfterExtraction(file);


            return true;


        }


        private static void DeleteArchiveAfterExtraction(FileSystemInfo file)
        {
            if (!Settings.DeleteArchiveAfterExtraction) return;

            Debugger.LogMessageToFile("Deleting archive " + file.Name + "...");
            File.Delete(file.FullName);
        }


        private static string ConstructExtractionDestination(FileInfo file)
        {
            string filename = file.Name.Remove((file.Name.Length - 4), 4) + "\\";
            filename = filename.Replace('.', ' ');
            string destination = file.DirectoryName + "\\" + filename;

            return destination;
        }





    }


} //endof namespace
