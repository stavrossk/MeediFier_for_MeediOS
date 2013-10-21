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
using MeediFier.Code.Metadata_Scrapers.Film.IMDb.Title_Matching_Engine;
using MeediFier.Code.Metadata_Scrapers.Film.IMDb.Title_Matching_Engines.IMDb;
using MeediOS;
#if USE_MEEDIO
using Meedio;
#elif USE_MEEDIOS

#endif

namespace MeediFier.IMDb
{
    static class FilmTitleMatchingEngine
    {

        public static string IdentifyFilmByFilmTitleMatchingEngines
            (IMLItem item, string imdbid, IMDbOperations imdbOp, 
             string year, ConnectionResult connectionresult)
        {


            #region Return Cases
            if (!Settings.FilmTitleMatchingEnginesAreEnabled)
            {
                Debugger.LogMessageToFile("[IMDb Film Title Matcher] IMDb Title matching is disabled. Returning...");
                return imdbid;
            }

            if (!String.IsNullOrEmpty(imdbid))
            {
                Debugger.LogMessageToFile("[IMDb Film Title Matcher] This item is already identified. Returning...");
                return imdbid;
            }
            #endregion

            if (Settings.PrimaryFilmTitleMatchingEngine == "TMDb")
            {


                imdbid = Code.Media_Identifiers.
                    Film_Identifiers.Title_Matching_Engines
                    .TMDb.TMDbTitleMatchingEngine.
                    IdentifyFilmByTMDbFilmTitleMatchingEngine(item);

                imdbid = IdentifyFilmByIMDbFilmTitleMatchingEngines
                    (item, imdbid, imdbOp, connectionresult);
            
            
            }
            else
            {


                imdbid = IdentifyFilmByIMDbFilmTitleMatchingEngines
                    (item, imdbid, imdbOp, connectionresult);


                imdbid = Code.Media_Identifiers.
                    Film_Identifiers.Title_Matching_Engines
                    .TMDb.TMDbTitleMatchingEngine.
                    IdentifyFilmByTMDbFilmTitleMatchingEngine(item);


            }



            return imdbid;
        }





        private static string IdentifyFilmByIMDbFilmTitleMatchingEngines
            (IMLItem item, string imdbid, IMDbOperations imdbOp,
             ConnectionResult connectionresult)
        {
            if (Settings.PrimaryIMDbFilmTitleMatchingEngine == "Internal")
            {
                IdentifyFilmByIMDbTitleMatchingEngineFirstInternalThenExternal
                    (item, imdbOp, connectionresult);
            }
            else
            {
                IdentifyFilmByIMDbTitleMatchingEngineFirstExternalThenInternal
                    (item, connectionresult);
            }

            return imdbid;
        }



        private static void IdentifyFilmByIMDbTitleMatchingEngineFirstExternalThenInternal
            (IMLItem item, ConnectionResult connectionresult)
        {

            Debugger.LogMessageToFile
                ("[IMDb Film Title Matcher] " +
                 "Primary film Title matching engine was set to 'External'.");

            IMDbTitleIdentifierExternal
                .IdentifyFilmByIMDbExternalTitleMatchingEngine
                (item, connectionresult);

            IMDbTitleMatchingEngineInternal.IdentifyFilmByIMDbInternalTitleMatchingEngine
                (item, connectionresult);
        
        }


        private static void IdentifyFilmByIMDbTitleMatchingEngineFirstInternalThenExternal
            (IMLItem item, IMDbOperations imdbOp, ConnectionResult connectionresult)
        {
            Debugger.LogMessageToFile
                ("[IMDb Film Title Matcher] " +
                 "Primary film Title matching engine was set to 'Internal'.");

            IMDbTitleMatchingEngineInternal.IdentifyFilmByIMDbInternalTitleMatchingEngine
                (item, connectionresult);

            IMDbTitleIdentifierExternal
                .IdentifyFilmByIMDbExternalTitleMatchingEngine
                (item, connectionresult);
        
        }



    }


}
