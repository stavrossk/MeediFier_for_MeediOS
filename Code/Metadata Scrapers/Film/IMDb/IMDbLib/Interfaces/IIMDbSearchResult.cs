using System;
using System.Collections.Generic;
using System.Text;

namespace MediaFairy
{
    public interface IIMDbSearchResult
    {
        string Title { get; set; }
        string URL { get; set; }
        string IMDb_ID { get; set; }
        string Year { get; set; }
        bool Ignore { get; set; }
        //bool DeleteItem { get; set; }
    }
}
