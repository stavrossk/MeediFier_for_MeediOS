//'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
//'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
//''    MediaFairy for Meedio and MeediOS 
//''    An import-type plugin for Meedio and MeediOS Home Theater applications     
//''
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
using System.IO;
using System.Windows.Forms;
using CustomProperties;
using MediaFolders;
using MeediFier.ImportingEngine;


#if USE_MEEDIO
using Meedio;
#elif USE_MEEDIOS
using MeediOS;
#endif



namespace MeediFier
{
    public class Importer : IMLImportPlugin, IDisposable, IMeedioInitializablePlugin
    {



        public bool Import(IMLSection section, IMLImportProgress progress)
        {


            return MainImportingEngine.DoImport(this, section, progress);


        }


#if USE_MEEDIO 
        public bool EditCustomProperty(int Window, string PropertyName, ref string Value)
#elif USE_MEEDIOS
        public bool EditCustomProperty(IntPtr window, string propertyName, ref string value)
#endif
        {
            if (propertyName == "customAdvancedOptions")
            {
                try
                {

                    #region Commented
                    //if (ImporterSettings == null)
                    //    MessageBox.Show("ImporterSettings is null");
                    //MessageBox.Show("Caption: " + ImporterSettings.Caption);
                    //MessageBox.Show("Values: " + ImporterSettings.Values);
                    //MessageBox.Show("Value: " + Value);
                    #endregion

                    _importerSettings.Caption = "MediaFairy Settings";
                    _importerSettings.Values = value;

                    if (_importerSettings.ShowGUI(null))
                    {
                        value = _importerSettings.Values;

                        return true;
                    }

                    return false;

                    //StatusForm.statusForm.GUIPage.Hide();
                    //StatusForm.statusForm.StatusPage.Hide();
                    //StatusForm.statusForm.PendingPage.Hide();
                    //StatusForm.statusForm.Show();

                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                    return false;
                }
            }


            return EditFoldersSettings(propertyName, ref value);
        }


        private bool EditFoldersSettings(string propertyName, ref string value)
        {


            string oldValue = value;
            if (String.Compare(propertyName, "AdvancedSettingsFolders", System.StringComparison.OrdinalIgnoreCase) == 0)
            {
                API.OpenSettingsWindow(ref value, Ibs);
                if (value == "Canceled") value = oldValue;
                return true;
            }

            string oldValueb = value;
            if (String.Compare(propertyName, "AdvancedSettingsFoldersb", System.StringComparison.OrdinalIgnoreCase) == 0)
            {
                API.OpenSettingsWindow(ref value, Ibs);
                if (value == "Canceled") value = oldValueb;
                return true;
            }


            string oldValuec = value;
            if (String.Compare(propertyName, "AdvancedSettingsFoldersc", System.StringComparison.OrdinalIgnoreCase) == 0)
            {
                API.OpenSettingsWindow(ref value, Ibs);
                if (value == "Canceled") value = oldValuec;
                return true;
            }




            string oldValued = value;
            if (String.Compare(propertyName, "AdvancedSettingsMovies",
                System.StringComparison.OrdinalIgnoreCase) == 0)
            {
                MetaDataFolders.Movies.OpenSettingsWindow(ref value, Ibs, true, true, true, false, false);
                if (value == "Canceled") value = oldValued;
                return true;
            }


            string oldValuee = value;
            if (String.Compare(propertyName, "AdvancedSettingsTVShows", 
                System.StringComparison.OrdinalIgnoreCase) == 0)
            {
                MetaDataFolders.TVShows.OpenSettingsWindow(ref value, Ibs, false, false, true, true, false, false, true,
                                                           true, false, false);
                if (value == "Canceled") value = oldValuee;
                return true;
            }

            string oldValuef = value;
            if (String.Compare(propertyName, "AdvancedSettingsMusic", 
                System.StringComparison.OrdinalIgnoreCase) == 0)
            {
                MetaDataFolders.Music.OpenSettingsWindow(ref value, Ibs, true, true, true, false, false, false, false,
                                                         false);
                if (value == "Canceled") value = oldValuef;
                return true;
            }
            return false;


        }


        #region MediaFolders Settings vars

        internal string MfSettingsMovies;
        internal string MfSettingsTvShows;
        internal string MfSettingsMusic;
        #endregion

        #region MetadataFolder Settings vars
        internal string MdfSettingsa;
        internal string MdfSettingsb;
        internal string MdfSettingsc;
        #endregion

        /// <summary>
        /// Retrieving config settings from configuration.exe and at startup from MeediOSApp 
        /// </summary>
        /// <PARAM name="properties">Containing the list of settings</PARAM><PARAM name="errorText">Returns the error text</PARAM>
        /// <returns>
        /// Returns true on success, false on error.
        /// </returns>
        public bool SetProperties(IMeedioItem Properties, out string errorText)
        {
            errorText = null;


            try
            {
                if (Properties["customAdvancedOptions"] != null)
                {
                    CustomProperty.SetProperties(Properties, "customAdvancedOptions");
                }

                //for (int i = 0; i < Properties.Count; i++ )
                //{
                string propertiesString = Properties[0].ToString();
                string[] properties = propertiesString.Split('¦');
                string pluginPath = Debugger.GetPluginPath();
                string debugLogPath = pluginPath + "Debug.log";

                #region Delete debug.log

                try
                {
                    File.Delete(debugLogPath);
                }
                catch (Exception e)
                {
                    Debugger.LogMessageToFile(
                        "An error occured while trying to delete the debug log file. The error was: " + e);
                }

                #endregion

                #region Write Plugin Settings to Debug.log

                File.AppendAllText(pluginPath + "Debug.log",
                                   @"------------------------   START OF PLUGIN SETTINGS   ---------------------");
                foreach (string property in properties)
                {
                    File.AppendAllText(pluginPath + "Debug.log", Environment.NewLine);
                    File.AppendAllText(pluginPath + "Debug.log", property);
                }
                File.AppendAllText(pluginPath + "Debug.log", Environment.NewLine);
                File.AppendAllText(pluginPath + "Debug.log",
                                   @"------------------------   END OF PLUGIN SETTINGS   ---------------------");
                File.AppendAllText(pluginPath + "Debug.log", Environment.NewLine);



                #endregion


                PropertiesSetter.SetProperties(Properties);


                #region MediaFolders and MetadataFolders

                MfSettingsMovies = Properties["AdvancedSettingsFolders"].ToString().Trim();
                MfSettingsTvShows = Properties["AdvancedSettingsFoldersb"].ToString().Trim();
                MfSettingsMusic = Properties["AdvancedSettingsFoldersc"].ToString().Trim();

                MdfSettingsa = Properties["AdvancedSettingsMovies"].ToString().Trim();
                MdfSettingsb = Properties["AdvancedSettingsTVShows"].ToString().Trim();
                MdfSettingsc = Properties["AdvancedSettingsMusic"].ToString().Trim();

                #endregion


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return false;
            }
            return true;
        }


        /// <summary>
        /// Defining config entries
        /// </summary>
        /// <PARAM name="propertyIndex">Parameter number</PARAM><PARAM name="propertyDefinition">Parameter definition</PARAM>
        /// <returns>
        /// Return true if propertyDefinition is filled with valid data, false if no property with index propertyIndex exists.
        /// </returns>
        public bool GetProperty(int index, IMeedioPluginProperty prop)
        {
            try
            {
                int counter = 1;
                MainImportingEngine.Initialized = Initialize();

                #region list of properties

                try
                {

                    if (index == counter++)
                    {
                        prop.CanTypeChoices = true;
                        prop.Caption = "Settings: ";
                        prop.DataType = "custom";
                        //Prop.GroupCaption = "                                  - Advanced - ";
                        prop.DefaultValue = _importerSettings.Default;
                        prop.Name = "customAdvancedOptions";
                        return true;
                    }

                    #region Media Folders
                    if (index == counter++)
                    {
                        prop.Caption = "Movies Folders Settings";
                        prop.HelpText = "Customize the Folders settings";
                        prop.DataType = "custom";
                        prop.DefaultValue = "";
                        prop.Name = "AdvancedSettingsFolders";
                        return true;
                    }

                    if (index == counter++)
                    {
                        prop.Caption = "TV Shows Folders Settings";
                        prop.HelpText = "Customize the Folders settings";
                        prop.DataType = "custom";
                        prop.DefaultValue = "";
                        prop.Name = "AdvancedSettingsFoldersb";
                        return true;
                    }

                    if (index == counter++)
                    {
                        prop.Caption = "Music Folders Settings";
                        prop.HelpText = "Customize the Folders settings";
                        prop.DataType = "custom";
                        prop.DefaultValue = "";
                        prop.Name = "AdvancedSettingsFoldersc";
                        return true;
                    }
                    #endregion Media Folders

                    #region Metadata Folders

                    #region Movies Metadata
                    if (index == counter++)
                    {
                        prop.Caption = "Movies Metadata Settings";
                        prop.HelpText = "Customize the Movies settings";
                        prop.DataType = "custom";
                        prop.DefaultValue = "Default Settings";
                        prop.Name = "AdvancedSettingsMovies";
                        return true;
                    }
                    #endregion Movies Metadata

                    #region TV Series Metadata
                    if (index == counter++)
                    {
                        prop.Caption = "TV Series Metadata Settings";
                        prop.HelpText = "Customize the TVShows settings";
                        prop.DataType = "custom";
                        prop.DefaultValue = "Default Settings";
                        prop.Name = "AdvancedSettingsTVShows";
                        return true;
                    }
                    #endregion

                    #region Music Metadata
                    // ReSharper disable RedundantAssignment
                    if (index == counter++)
                    // ReSharper restore RedundantAssignment
                    {
                        prop.Caption = "Music Metadata Settings";
                        prop.HelpText = "Customize the Music settings";
                        prop.DataType = "custom";
                        prop.DefaultValue = "Default Settings";
                        prop.Name = "AdvancedSettingsMusic";
                        return true;
                    }
                    #endregion

                    #endregion Metadata Folders


                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                    return false;
                }

                #endregion
            }

            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }

            return false;
        }


        
        private Boolean Initialize()
                {
                    try
                    {
                        if (!MainImportingEngine.Initialized)
                        _importerSettings = new ImporterProperties();   
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.ToString());
                        return false;
                    }
                    return true;
                }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            //Snapshot();
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(Boolean disposing)
        {
            if (disposing)
            {
                //ImportFunc.Dispose();
                //ImportFunc = null;
                //ImporterSettings.Dispose();
                //ImporterSettings = null;
                //TagMasks.Clear();
                //TagMasks = null;
                //LogMessages.Dispose();
                //LogMessages = null;
                //Log = null;
                //Marshal.ReleaseComObject(Section);
                //Marshal.ReleaseComObject(Progress);
                //Progress = null;
                //Section = null;
                //Snapshot();
                // Free other state (managed objects).
            }
            // Free your own state (unmanaged objects).
            // Set large fields to null.
        }

        #endregion

        #region Runtime variables

        private ImporterProperties _importerSettings = new ImporterProperties();

        internal IBaseSystem Ibs;

        #endregion

        #region Import Progress

        #endregion

        public void Initalize(IBaseSystem baseSystem)
        {
            Ibs = baseSystem;
            return;
        }

        #region init library sections

        #if USE_MEEDIO
        public static MeedioLibrary.MediaLibraryClass library = new MeedioLibrary.MediaLibraryClass();
        #elif USE_MEEDIOS
#endif

        #endregion


    }//endof  class


}//endof namespace