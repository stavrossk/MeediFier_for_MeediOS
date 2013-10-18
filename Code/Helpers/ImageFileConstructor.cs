//'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
//'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
//''    MediaFairy                                                               ''
//''    Copyright (C) 2008-2010  Stavros Skamagkis                               ''
//''                                                                             ''
//''    This program is free software: you can redistribute it and/or modify     ''
//''    it under the terms of the GNU General Public License as published by     ''
//''    the Free Software Foundation, either version 3 of the License, or        ''
//''    (at your option) any later version.                                      ''
//''                                                                             ''
//''    This program is distributed in the hope that it will be useful,          ''
//''    but WITHOUT ANY WARRANTY; without even the implied warranty of           ''
//''    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the            ''
//''    GNU General Public License for more details.                             ''
//''                                                                             ''
//''    You should have received a copy of the GNU General Public License        ''
//''    along with this program.  If not, see <http://www.gnu.org/licenses/>.    ''
//'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
//'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''




using System;
using System.Net;
using System.IO;
#if USE_MEEDIO 
using Meedio; 
#elif USE_MEEDIOS
using MeediOS;
#endif





namespace MeediFier
{

    public class ImageFileConstructor
    {




        #region class variables

        public static System.Collections.ArrayList a = new System.Collections.ArrayList();
        public static System.Collections.ArrayList c = new System.Collections.ArrayList();


        internal static WebClient ObjWebClient = new WebClient();

        static FileStream _fileStream;

        #endregion








        public static string SaveImage
            (byte[] imagedata, IMLItem item,
            bool isPoster, string specificImageTag,string ImageFile)
        {
            

            #region Save image file
            try
            {

                //MessageBox.Show("imagefile: " + ImageFile);

                _fileStream = File.Create(ImageFile);
                _fileStream.Write(imagedata, 0, imagedata.Length);
                _fileStream.Close();

                if (isPoster)
                {
                    item.ImageFile = ImageFile;
                    item.SaveTags();
                }
                else
                {
                    item.Tags[specificImageTag] = ImageFile;
                    item.SaveTags();

                }

                return ImageFile;
            }
            catch (Exception ex)
            {
                Debugger.LogMessageToFile("Error while saving Poster image for " + item.Name + ": " + ex.Message);
                return "";
            }

            #endregion



        }
    }
 

}
