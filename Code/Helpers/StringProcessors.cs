using System;
using System.Text;



namespace MediaFairy.Code
{



    class StringProcessors
    {




        internal static string ConvertToUnicode(string asciiString)
        {
            //unicodeString = "This string contains the unicode character Pi(\u03a0)";

            // Create two different encodings.
            Encoding ascii = Encoding.ASCII;
            Encoding unicode = Encoding.Unicode;

            // Convert the string into a byte[].
            byte[] asciiBytes = ascii.GetBytes(asciiString);

            // Perform the conversion from one encoding to the other.
            byte[] unicodeBytes = Encoding.Convert(ascii, unicode, asciiBytes);

            // Convert the new byte[] into a char[] and then into a string.
            // This is a slightly different approach to converting to illustrate
            // the use of GetCharCount/GetChars.
            char[] unicodeChars = new char[unicode.GetCharCount(unicodeBytes, 0, unicodeBytes.Length)];
            unicode.GetChars(unicodeBytes, 0, unicodeBytes.Length, unicodeChars, 0);
            string unicodeString = new string(unicodeChars);

            // Display the strings created before and after the conversion.
            Console.WriteLine(@"Original string: {0}", unicodeString);
            Console.WriteLine(@"Ascii converted string: {0}", asciiString);

            return unicodeString;
        }




        internal static string NormalizePath(string path)
        {

            try
            {
                path = path.Replace(':', '-');
                path = path.Replace('/', '-');
                //Path = Path.Replace('\\', '-');
                path = path.Replace('*', '\'');
                path = path.Replace('?', ';');
                path = path.Replace('"', '\'');
                path = path.Replace('<', '[');
                path = path.Replace('>', ']');
                path = path.Replace('|', '-');
            }
            catch (Exception e)
            {
                Debugger.LogMessageToFile("An unexpected error occurred in the String Normalizer. " +
                                          "The error was: " + e );

            }



            return path;
        }
    }
}
