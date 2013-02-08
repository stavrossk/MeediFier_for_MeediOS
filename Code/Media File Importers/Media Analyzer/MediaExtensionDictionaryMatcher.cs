using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;



namespace MediaFairy.Code.Media_File_Importers.Media_Analyzer
{



    class MediaExtensionDictionaryMatcher
    {



        internal static bool FileTypeIsAudioExtension(IEnumerable<string> audioExtensions, string ext)
        {
            bool isAudio = false;

            foreach (string audioext in audioExtensions.Where(audioext => audioext == ext))
                isAudio = true;

            return isAudio;
        }

        internal static bool FileTypeIsVideoExtension(IEnumerable<string> videoExtensions, string ext)
        {
            bool isVideo = false;

            foreach (string videoext in videoExtensions.Where(videoext => videoext == ext))
                isVideo = true;

            return isVideo;

        }

        internal static bool FileTypeBelongsToIgnoredExtensions
            (FileSystemInfo file, string[] extensionsToIgnore)
        {
           

            return extensionsToIgnore.Any
                (ignoredExtension =>
                 file.Extension == ignoredExtension);

        }
    }
}
