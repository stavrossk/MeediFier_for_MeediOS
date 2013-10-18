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
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using MeediFier.ImportingEngine;
using MeediOS;

namespace MeediFier
{
    public static class LibraryCleaners
    {

        internal static void CleanupSection
            (bool cleanEmptyFields, bool cleanSelectedFields, 
            string selectedFieldsToClean, IMLSection section)
        {


            try
            {

                CleanEmptyFields(cleanEmptyFields, section);

                CleanUserFields(section, cleanSelectedFields, selectedFieldsToClean);

            }
            catch (Exception e)
            {
                StatusForm.statusForm.TrayIcon.ShowBalloonTip(10000, "An unexpected error occured", "An error occured in the Library Cleanup process. Please refer to the plugin's Debug.log file for details.", ToolTipIcon.Warning);
                Debugger.LogMessageToFile("The Library Cleanup process produced and unexpected error: " + e);
            }



        }

        private static void CleanEmptyFields(bool cleanEmptyFields, IMLSection section)
        {

            if (!cleanEmptyFields) 
                return;

            //Importer.CurrentProgress++;
            MainImportingEngine.ThisProgress.Progress
                (MainImportingEngine.CurrentProgress, 
                "Cleaning empty library fields...");
            //Thread.Sleep(800);

            string[] tagNames = section.GetTagNames();


            section.BeginUpdate();

            foreach (string tagName in tagNames)
            {
                bool allTagValuesAreEmpty = true;

                //Importer.thisProgress.Progress(100, TagName);
                //Thread.Sleep(1000);

                if (tagName == "ToDelete")
                {

                }

                if (section.GetTagValues(tagName) == null) continue;

                string[] TagValues = section.GetTagValues(tagName);

                foreach (string tagvalue in TagValues.Where(tagvalue => !String.IsNullOrEmpty(tagvalue)))
                    allTagValuesAreEmpty = false;


                if (allTagValuesAreEmpty)
                    section.DeleteTag(tagName);

            }
            section.EndUpdate();


            section.BeginUpdate();

            //Importer.CurrentProgress++;
            MainImportingEngine.ThisProgress.Progress
                (MainImportingEngine.CurrentProgress, 
                "Removing items marked for deletion...");
            
            Thread.Sleep(800);

            IMLItemList itemsToDelete = section.SearchByTag("ToDelete", "true");
            
            foreach (IMLItem t in itemsToDelete)
                section.DeleteItem(t);

            section.EndUpdate();

        }



        private static void CleanUserFields
            (IMLSection section, bool cleanSelectedFields, string selectedFieldsToClean)
        {

            if (!cleanSelectedFields) 
                return;

            //Importer.CurrentProgress++;
            MainImportingEngine.ThisProgress.Progress
                (MainImportingEngine.CurrentProgress, 
                "Cleaning user-specified library fields...");
            
            Thread.Sleep(800);


            section.BeginUpdate();

            string[] fieldsToClean = selectedFieldsToClean.Split(',');

            foreach (string fieldtoclean in fieldsToClean)
                section.DeleteTag(fieldtoclean.Trim());

            section.EndUpdate();

        }



    }
}