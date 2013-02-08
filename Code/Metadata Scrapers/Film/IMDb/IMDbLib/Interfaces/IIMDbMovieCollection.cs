using System;
using System.Collections.Generic;
using System.Text;

namespace MediaFairy
{
    public interface IIMDbMovieCollection : IList<IIMDbMovie>
    {
        IIMDbMovie GetMovie(string imdbID);
        void Remove(string imdbID);
    }
}
