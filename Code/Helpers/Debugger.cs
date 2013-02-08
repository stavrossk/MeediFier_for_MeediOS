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
using Microsoft.Win32;
//using System.Linq;

// ReSharper disable CheckNamespace
namespace MediaFairy
// ReSharper restore CheckNamespace
{
    public class Debugger
    {

        public static string GetTempPath()
        {

            //IMeedioSystem msystem = null;
            //IMeedioItem pluginDir = msystem.GetSettings("plugin-path");

            //string path = pluginDir["plugin-path"].ToString();
            //string path = @"D:\";


            RegistryKey key = Registry.LocalMachine;
            //DirectoryInfo di;

            key = key.OpenSubKey("SOFTWARE\\Meedio\\Meedio Essentials", false);
            if (key != null)
            {
                string path = key.GetValue("data").ToString();

                if (!path.EndsWith("\\")) path += "\\";

                //path += "logs\\";



                return path;
            }


            return String.Empty;
        }




        public static string GetPluginPath()
        {
#if USE_MEEDIO
            RegistryKey key = Registry.LocalMachine;
            //DirectoryInfo di;

            key = key.OpenSubKey("SOFTWARE\\Meedio\\Meedio Essentials", false);
            path = key.GetValue("plugins").ToString();
            if (!path.EndsWith("\\"))
                path += "\\";

            path += @"import\MediaFairy\";
            return path;
            #elif USE_MEEDIOS



            if (MeediOS.Configuration.ConfigurationManager
                .GetPluginsDirectory
                ("import", @"MediaFairy\") != null)
            {


                string path = MeediOS.Configuration.ConfigurationManager
                    .GetPluginsDirectory
                    ("import", @"MediaFairy\");


                return path;
            }



            return string.Empty;

            #endif
        }


        public static string GetMeedioRoot()
        {
#if USE_MEEDIO
                        RegistryKey key = Registry.LocalMachine;
            //DirectoryInfo di;

            key = key.OpenSubKey("SOFTWARE\\Meedio\\Meedio Essentials", false);
            path = key.GetValue("root").ToString();
            if (!path.EndsWith("\\"))
                path += "\\";
            return path;
            #elif USE_MEEDIOS

            if (MeediOS.Configuration.ConfigurationManager.GetRootDirectory(null) != null)
            {
                string path = MeediOS.Configuration.ConfigurationManager.GetRootDirectory(null);
                return path;
            }

            return string.Empty;

            #endif


        }
        










        public static void LogMessageToFile(string msg)
        {

            if ( !Settings.WriteDebugLog)
                return;

            System.IO.StreamWriter sw = System.IO.File.AppendText(
                GetPluginPath() + "Debug.log");
            try
            {
                string logLine = String.Format(
                    "{0:G}: {1}", DateTime.Now, msg);
                sw.WriteLine(logLine);
            }
            finally
            {
                sw.Close();
            }
        }


    }//endof class


}//endof namespace
