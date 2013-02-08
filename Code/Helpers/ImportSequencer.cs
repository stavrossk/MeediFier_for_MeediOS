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
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

//TODO: Add Meedio
using MediaFairy.ImportingEngine;
using MeediOS;


namespace MediaFairy
{
    static class ImportSequencer
    {
        public static void RunImports()
        {
            IEnumerable<string> importIDs = Settings.ImportIDs;

            #region Return Cases
            if (!Settings.ImportSequencer)
                return;

            if (importIDs == null)
                return;
            #endregion


            IMLImports imports = MainImportingEngine.Library.GetImports();
            int currentImport = 0;

            //MessageBox.Show( Convert.ToString( imports.Count) );
            foreach (IMLImport import in imports)
            {
                // ReSharper disable InconsistentNaming
                foreach (string IDstr in importIDs)
                // ReSharper restore InconsistentNaming
                {
                    try
                    {
                        int id = Convert.ToUInt16(IDstr);
                        //MessageBox.Show(import.Name);
                        if (import.ID == id)
                        {
                            string errorText;

                            MainImportingEngine.ThisProgress.Progress( ( (100 / imports.Count) * currentImport), "Preparing to run import " + import.Name + "...");
                            Thread.Sleep(2000);
                            import.Run(MainImportingEngine.ThisProgress, out errorText);

                            if (!String.IsNullOrEmpty(errorText))
                                StatusForm.statusForm.TrayIcon.ShowBalloonTip(10000, "Sequenced importer returned an error", "The importer " + import.Name + "set to run after MediaFairy returned an error." + Environment.NewLine + "Please refer to the plugin's debug.log for debugging information.", ToolTipIcon.Warning);
                            Debugger.LogMessageToFile("The sequenced importer " + import.Name + " returned an exception: " + Environment.NewLine + errorText);
                        }
                    }
                    catch (Exception e)
                    {
                        //TODO: Handle exception
                    }

                    currentImport++;

                }//endof foreach

            }//endof for



        }//endof function

    }//endof class

}//endof namespace
