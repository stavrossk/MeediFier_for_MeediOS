using System;


namespace MeediFier.Code.Media_Updaters.Single_Item_Updaters.Movie_Item_Updater
{


    class MultipartFilmLibraryItemDetector
    {



        internal static void DetectMultipartItemRetrieveFirstItemLocation
            (ref string location, 
             ref bool itemIsMultipart,
             ref string[] multipart)
        {


            if (String.IsNullOrEmpty(location))
                return;

            if (location.StartsWith("|")
                && location.EndsWith("|"))
            {

                Debugger.LogMessageToFile
                    ("The item's location field" +
                     " contains multiple files: "
                     + location);
                
                itemIsMultipart = true;

                const string delimeter = "|";

                location = location.Trim
                    (delimeter.ToCharArray());

                multipart = location.Split('|');

                location = multipart[0];


                Debugger.LogMessageToFile
                    ("The item's location was set" +
                     " to the first media file " +
                     "of the chain: " + multipart[0] );


            }
            else
            {
                itemIsMultipart = false;
                multipart = new string[1];
                multipart[0] = location;
            }

        }


    }


}
