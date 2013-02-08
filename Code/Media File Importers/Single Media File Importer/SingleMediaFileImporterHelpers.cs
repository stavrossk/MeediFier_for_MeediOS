using System;
using System.IO;
using System.Windows.Forms;
using MediaFairy.ImporterPropertiesGroups;
using MediaFairy.MediaAnalyzer;
using MediaFairy.MediaImporters;
using MeediOS;



namespace MediaFairy.MediaFileImporters.SingleMediaFileImporter
{


    internal static class SingleMediaFileImporterHelpers
    {




        internal static bool ImportAdditionalMediaTypes
            (FileSystemInfo file, IMLSection moviesSection,
             string fileExtension, DirectoryInfo parent,
             string fileName, string parentName)
        {


            return DvdDirectoryImporter
                .ImportDvdDirectory
                (file, moviesSection,
                fileName, parentName, parent);
            //|| 
            //BookImporter.ImportBook(file, booksSection, fileName, fileExtension);


        }




        //internal static bool HandleArchive
        //    (FileInfo file, string fileExtension)
        //{
        //    Application.DoEvents();

        //    if (MediaTypeDetector
        //        .FileIsRarPart
        //        (fileExtension))
        //        return true;

        //    Application.DoEvents();

        //    if (ArchiveExtractor.DetectAndExtractArchive(file))
        //        return true;

        //    Application.DoEvents();
        //    return false;
        //}




        internal static bool RemoveExtensionFromFilename
            (FileSystemInfo file, out string fileName, out string fileExtension)
        {
            fileName = file.Name;
            fileExtension = file.Extension;

            if (String.IsNullOrEmpty(fileExtension))
            {
                Debugger.LogMessageToFile("The file's " + fileName + " extension is missing. This file will be skipped.");
                return true;
            }

            fileName = fileName.TrimEnd(fileExtension.ToCharArray());
        
            
            return false;
    
        }
    
    
    
    
    }


}
