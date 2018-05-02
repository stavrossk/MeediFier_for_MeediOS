using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MediaFairy.Memorial
{
    class IMDbLib
    {

        /// <summary>
        /// Retrieves list of search results matching specified movie title
        /// </summary>
        /// <param name="title">Title of movie to search for</param>
        /// <returns>IList of IIMDbSearchResult objects</returns>
        public IList<IIMDbSearchResult> SearchMovies(string title)
        {

            try
            {

                IList<IIMDbSearchResult> list = IMDb.IMDbConventionalFilmSearchEngine.SearchForTitleReturnResults(title);
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        //TODO: This function is not needed anymore!

        /// <summary>
        /// Retrievies list of movies matching specified movie title
        /// </summary>
        /// <param name="title">Title of movie to search for</param>
        /// <returns>IIMDbMovieCollection object</returns>
        public IIMDbMovieCollection SearchMovieDetails(string title)
        {

            IIMDbMovieCollection list = _db.SearchMovieInfo(title);
            return list;


        }




    }
}
