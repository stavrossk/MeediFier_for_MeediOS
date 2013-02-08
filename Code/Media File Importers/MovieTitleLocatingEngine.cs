using System.Collections.Generic;
using System.IO;

namespace MediaFairy.MediaSnapshotEngine
{

    class MovieTitleLocatingEngine
    {


        internal static bool MovieTitleLocator
            (FileSystemInfo file, 
            IEnumerable<string> videoexts,
            DirectoryInfo parentDirectoryInfo, 
            string fileName, ref string parentName,
            IEnumerable<string> combinedSceneTags)
        {



            parentName = parentName.Replace(".", " ");


            Debugger.LogMessageToFile
                ("Parent directory's name: " 
                + parentName);




            int videoFilesInDirectory
                = CountVideoFilesInDirectory
                (videoexts, parentDirectoryInfo);


            var directoryContainsMovieCollection
                = DetectMovieCollection(parentName);




            #region Clean File And Parent Directory Names

            string parentDirectoryNameFiltered;
            string filenameFiltered;
            CleanFileAndParentDirectoryNames(fileName, parentName, out parentDirectoryNameFiltered, out filenameFiltered, combinedSceneTags );

            #endregion



            var useParentFolder = LocateMovieTitleInPath
                (file, filenameFiltered, 
                parentDirectoryNameFiltered,
                directoryContainsMovieCollection, 
                videoFilesInDirectory);




            return useParentFolder;
        }







        internal static bool LocateMovieTitleInPath
            (
            FileSystemInfo file, 
            string filenameFiltered, string parentDirectoryNameFiltered,
            bool directoryContainsMovieCollection, int videoFilesInDirectory
            )
        {

            bool useParentFolder;

            if (Settings.ImportFilmsInRarArchives && file.Extension == ".r00")
                useParentFolder = true;
            else
            {
                if (!Settings.OverrideAutomatedMovieTitleLocator)
                    useParentFolder = parentDirectoryNameFiltered.Length >= filenameFiltered.Length;
                else
                    useParentFolder = Settings.MovieTitleLocationInPath == "Parent folder";
            }

            if (directoryContainsMovieCollection || videoFilesInDirectory > 3)
                useParentFolder = false;

            return useParentFolder;
        }





        internal static void CleanFileAndParentDirectoryNames
            (string fileName, string parentName,
            out string parentDirectoryNameFiltered, 
            out string filenameFiltered,
            IEnumerable<string> combinedSceneTags)
        {


            #region Filter parent and file names

            Debugger.LogMessageToFile("Filtering file and parent directory names from common keywords...");
            parentDirectoryNameFiltered = VideoFilenameCleaner.CleanVideoFilename(parentName, combinedSceneTags);
            filenameFiltered = VideoFilenameCleaner.CleanVideoFilename(fileName, combinedSceneTags);
            filenameFiltered = filenameFiltered.TrimEnd('.');
            Debugger.LogMessageToFile("File name after common keywords removal: " + filenameFiltered);
            Debugger.LogMessageToFile("Parent directory name after common keywords removal: " + parentDirectoryNameFiltered);

            #endregion



        }






        internal static int CountVideoFilesInDirectory
            (IEnumerable<string> videoExtensions, DirectoryInfo parent)
        {

            int videoFilesInDirectory = 0;

            Debugger.LogMessageToFile
                ("Counting video files" +
                 " inside parent directory...");


            foreach (string videoExtension in videoExtensions)
            {
                string extension = "*" + videoExtension;
                FileInfo[] files = parent.GetFiles(extension, SearchOption.TopDirectoryOnly);
                videoFilesInDirectory += files.Length;
            }




            return videoFilesInDirectory;
        }






        internal static bool DetectMovieCollection(string parentName)
        {
            #region Detect Collection or Trilogy

            bool isCollection = false;
            if (parentName.Contains("Collection") || parentName.Contains("Collector") ||
                parentName.Contains("Trilogy"))
            {
                Debugger.LogMessageToFile(
                    "This film will be considered part of a film collection or feature trilogy.");
                isCollection = true;
            }

            #endregion

            return isCollection;
        }


    }


}
