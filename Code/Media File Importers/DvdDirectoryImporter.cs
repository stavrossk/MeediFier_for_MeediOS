using System.IO;
using System.Threading;
using System.Windows.Forms;
using MediaFairy.Code.Settings;
using MediaFairy.ImportingEngine;
using MediaFairy.MediaSnapshotEngine;
using MeediOS;

namespace MediaFairy.MediaImporters
{


    class DvdDirectoryImporter
    {


        internal static bool ImportDvdDirectory(FileSystemInfo file, IMLSection moviesSection, string fileName, string parentName,
                                               DirectoryInfo parent)
        {

            if (!MediaImportersAndUpdatersCoreSettings.EnableMovieImporter || !Settings.ImportDvdFolders)
                return false;

            if (!file.Name.Contains("video_ts.ifo") && !file.Name.Contains("VIDEO_TS.IFO"))
                return false;

            Application.DoEvents();
            MainImportingEngine.ThisProgress.Progress(MainImportingEngine.CurrentProgress, "Importing dvd film " + fileName + "...");
            Thread.Sleep(1500);

            string filmTitle;
            IMLItem item;

            if (parentName == "video_ts" || parentName == "VIDEO_TS")
            {
                DirectoryInfo secondParent = parent.Parent;

                if (secondParent != null)
                {
                    filmTitle = secondParent.Name;
                    MediaSectionPopulator.AddFileToSection(out item, moviesSection, filmTitle, file.FullName, file.FullName);
                    return true;
                }

                return false;
            }

            filmTitle = parent.Name;
            MediaSectionPopulator.AddFileToSection(out item, moviesSection, filmTitle, file.FullName, file.FullName);

            return true;

        }
    }


}
