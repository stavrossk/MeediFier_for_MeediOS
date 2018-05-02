
        //TODO: Remove this function, too slow!!!

        //public IIMDbMovieCollection SearchMovieInfo(string title )
       
        //{
        //    IMDbMovieCollection movies = new IMDbMovieCollection();

        //    try
        //    {
        //        IList<IIMDbSearchResult> results = IMDb.IMDbConventionalFilmSearchEngine.SearchMovies(title);
        //        if (results != null)
        //            foreach (IIMDbSearchResult result in results)
        //            {
        //                if (result.IMDb_ID.Trim() != "")
        //                {
        //                    IIMDbMovie movie = GetMovieInfoFromIMDb(result.IMDb_ID.Trim(), false);
        //                    if (movie != null)
        //                        movies.Add(movie);
        //                }
        //            }
        //        return movies;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}