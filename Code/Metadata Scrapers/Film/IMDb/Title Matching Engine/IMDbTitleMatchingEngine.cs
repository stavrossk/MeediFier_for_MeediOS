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
using System.Windows.Forms;
using MediaFairy.IMDb;
#if USE_MEEDIO
using Meedio;
#elif USE_MEEDIOS
using MediaFairy.IMDb.Title_Matching_Engine;
using MeediOS;
#endif


namespace MediaFairy.IMDb
{
    static class IMDbTitleMatchingEngine
    {



        public static bool TitleMatchingEngineTakeAction(IMLItem item, string year, IMDbOperations imdbOp)
        {


            var resultsFound = false;

            if (imdbOp.ImdbList != null && imdbOp.ImdbList.Count > 0)
                resultsFound = true;

            if (resultsFound)
            {
                TitleMatchingEngineChooseFromResults(imdbOp, item, year);
            }
            else
            {
                //TODO: In the dialog shown, instruct the user to make a search for the Title.

                if (imdbOp._assistType == IMDbOperations.AssistType.Always | imdbOp._assistType == IMDbOperations.AssistType.NoPerfectMatch)
                    TitleMatchingEngineUserSelectsResult(imdbOp, false);
                else return false;
                //TODO: Add a "When no results found" option to "when to ask for film matching assistance".
            }


            return true;
        }




        private static void TitleMatchingEngineChooseFromResults(IMDbOperations imdbOp, IMLItem item, string year)
        {

            Debugger.LogMessageToFile("[IMDb film Title matching engine] Starting to perform selection from available film results according to user's preferences...");

            if (imdbOp._assistType == IMDbOperations.AssistType.Always)
                TitleMatchingEngineUserSelectsResult(imdbOp, true);
            else
            {
                var exactMatchFound = TitleMatchingEngineTryToFindExactMatch(imdbOp, item, year);

                #region Actions when no exact match was found

                if (!exactMatchFound)
                    TitleMatchingEngineDecideActionWhenNoPerfectMatch(imdbOp);

                #endregion
            }
        }


        private static bool TitleMatchingEngineTryToFindExactMatch(IMDbOperations imdbOp, IMLItem item, string year)
        {

            #region Vars

            var exactMatchFound = false;
            #endregion


            #region try to find exact match

            ////MessageBox.Show("Assited search disabled");
            // check first x results for exact match
            // if we have exact match, check for duplicates

            Debugger.LogMessageToFile("[IMDb video identifier] Attempting to find exact match...");
            imdbOp.ImdbResult = ExactMatchDetector.FindExactMatch(imdbOp.ImdbList.Count, imdbOp.Title, year, imdbOp.ImdbList, Helpers.GetTagValueFromItem(item, "VideoDuration"));

            if (imdbOp.ImdbResult != null)
                exactMatchFound = true;

            if (!exactMatchFound)
                return false;

            Debugger.LogMessageToFile("[IMDb video identifier] Exact match was found!");
            Debugger.LogMessageToFile("[IMDb video identifier] Detecting duplicates...");
            var duplicateExists = TitleMatchingEngineHelpers.DuplicatesExist(imdbOp.Title, year, imdbOp.ImdbList);


            if (duplicateExists)
            {
                Debugger.LogMessageToFile("[IMDb video identifier] A duplicate exists for this video.");
                imdbOp.ImdbId = String.Empty;
                imdbOp.ImdbResult = null;
                return false;
            }
            

            imdbOp.ImdbId = imdbOp.ImdbResult.IMDb_ID;
            imdbOp.Title = imdbOp.ImdbResult.Title;
            return true;

            #endregion





        }



        private static void TitleMatchingEngineDecideActionWhenNoPerfectMatch(IMDbOperations imdbOp)
        {


            Debugger.LogMessageToFile("[IMDb film Title matching engine] An exact match was not found.");

            if (imdbOp._assistType == IMDbOperations.AssistType.NoPerfectMatch)
            {
                #region If there is only one result found use it, else ask from user to select a result.
                Debugger.LogMessageToFile("[IMDb video identifier] Video matching assistance is enabled.");

                switch (imdbOp.ImdbList.Count)
                {
                    case 1:
                        Debugger.LogMessageToFile("[IMDb video identifier] Singular result detected. Will automatically select this match.");
                        imdbOp.ImdbResult = TitleMatchingEngineHelpers.GetFirstMovie(imdbOp.ImdbList);
                        if (imdbOp.ImdbResult == null) return;
                        imdbOp.ImdbId = imdbOp.ImdbResult.IMDb_ID;
                        imdbOp.Title = imdbOp.ImdbResult.Title;
                        break;
                    default:
                        TitleMatchingEngineUserSelectsResult(imdbOp, true);
                        break;
                }

                #endregion
            }
            else TitleMatchingEngineAutoDecideBestMatch(imdbOp);


        }


        private static void TitleMatchingEngineAutoDecideBestMatch(IMDbOperations imdbOp)
        {

            #region Return if Assist Mode is set to 'Skip unknown items'
            if (imdbOp._assistType == IMDbOperations.AssistType.SkipUnknownItems)
            {
                imdbOp.ImdbId = String.Empty;
                imdbOp.Title = String.Empty;
                return;

            }
            #endregion


            #region give automated IMDb result

            if (imdbOp.UseFirst)
            {
                #region Use First match Found
                //MessageBox.Show("Will use First Found");
                imdbOp.ImdbResult = TitleMatchingEngineHelpers.GetFirstMovie(imdbOp.ImdbList);
                if (imdbOp.ImdbResult == null) return;
                imdbOp.ImdbId = imdbOp.ImdbResult.IMDb_ID;
                imdbOp.Title = imdbOp.ImdbResult.Title;

                #endregion

            }
            else if (imdbOp.UseLatest)
            {
                #region Use Latest match
                ////MessageBox.Show("Will Use Latest");
                imdbOp.ImdbResult = TitleMatchingEngineHelpers.GetLatestMovie(imdbOp.ImdbList, imdbOp.Title);
                if (imdbOp.ImdbResult == null) return;

                imdbOp.ImdbId = imdbOp.ImdbResult.IMDb_ID;
                imdbOp.Title = imdbOp.ImdbResult.Title;

                #endregion
            }
            else if (imdbOp.UseMostPopular)
            {
                #region Use Most Popular match
                //MessageBox.Show("Will use Most Popular");
                
                //TODO: Why is this the same with the "Use First Found" setting?
                imdbOp.ImdbResult = TitleMatchingEngineHelpers.GetFirstMovie(imdbOp.ImdbList);

                if (imdbOp.ImdbResult == null) return;

                imdbOp.ImdbId = imdbOp.ImdbResult.IMDb_ID;
                imdbOp.Title = imdbOp.ImdbResult.Title;

                #endregion
            }

            #endregion




        }



        private static void TitleMatchingEngineUserSelectsResult(IMDbOperations imdbOp, bool resultsFound)
        {


            #region show film selection dialog

            imdbOp.NotListed = false;
            Debugger.LogMessageToFile("[IMDb video identifier] Showing video match selection dialog...");

            FrmSelect tmpFrmSelect;

// ReSharper disable ConvertIfStatementToConditionalTernaryExpression
            if (resultsFound)
// ReSharper restore ConvertIfStatementToConditionalTernaryExpression
                tmpFrmSelect = new FrmSelect(imdbOp.Title, IMDbOperations.Country, imdbOp.ImdbList);
            else tmpFrmSelect = new FrmSelect(imdbOp.Title, IMDbOperations.Country, FrmSelect.SearchType.IMDb);

            var dlResult = tmpFrmSelect.ShowDialog();


            if (dlResult == DialogResult.OK)
            {
                //MessageBox.Show("User pressed Submit");

                imdbOp.ImdbResult = (IIMDbSearchResult)tmpFrmSelect.SelectedResult;
                imdbOp.ImdbId = imdbOp.ImdbResult.IMDb_ID;
                imdbOp.Title = imdbOp.ImdbResult.Title;
                imdbOp.IsCustom = tmpFrmSelect.IsCustom;
            }
            if (dlResult == DialogResult.Cancel)
            {
                //MessageBox.Show("passed in the Just Ignore case");
                imdbOp.NotListed = true;
                tmpFrmSelect.Close();
                tmpFrmSelect.Dispose();
                //tmpFrmSelect = null;
            }
            if (dlResult == DialogResult.Abort)
            {
                //MessageBox.Show("passed in the Delete Item case");
                imdbOp.NotListed = true;
                imdbOp.DeleteItem = true;
                tmpFrmSelect.Close();
                tmpFrmSelect.Dispose();
                //tmpFrmSelect = null;
            }

            #endregion




        }









    }
}
