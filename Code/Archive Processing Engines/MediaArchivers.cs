using System;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using MeediOS;

namespace MediaFairy.ArchiveProcessingEngines
{

    internal class MediaArchivers
    {

        internal static bool ArchiveFilm(IMLItem item, DirectoryInfo parent, string archiveLocation)
        {

            if (!Settings.ArchiveWatchedFilms)
                return false;

            FastZip fz = new FastZip();

            if (Helpers.GetTagValueFromItem(item, "Watched") != "true" 
                &&
                Helpers.GetTagValueFromItem(item, "Watched") != "True")
                return false;

            Helpers.UpdateProgress("Updating Films Section...", "Archiving watched film " + item.Name + "...", item);

            try
            {
                string year = String.Empty;

                if ( !String.IsNullOrEmpty(Helpers.GetTagValueFromItem(item, "Year") ) ) 
                    year = " (" + Helpers.GetTagValueFromItem(item, "Year") + ")";

                if (!archiveLocation.EndsWith("\\"))
                    archiveLocation = archiveLocation + "\\";

                //TODO: Use Path.Combine
                string zipFile = archiveLocation + parent.Name + year + ".zip";
                fz.CreateZip(zipFile, parent.FullName, true, ".*");

                Directory.Delete(parent.FullName, true);

            }
            catch (Exception e)
            {

                Debugger.LogMessageToFile(
                    "An unexpected error ocurred in the Film Archiving process. " +
                    "The error was: " + e
                    );

            }

            return true;

        }
    
    }

}
