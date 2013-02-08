using System;
using System.Collections.Generic;
using System.Text;

namespace MediaFairy
{
    public interface IIMDbMovie
    {
        IIMDbPersonCollection People { get; }
        IList<string> Genres { get; }
        IList<IIMDbGoof> Goofs { get; }
        IList<IList<IIMDbQuote>> Quotes { get; }
        IList<string> Trivia { get; }
        string IMDb_ID { get; set; }
        string OverviewShort { get; set; }
        string OverviewLong { get; set; }
        string Rating { get; set; }
        string RatingDescription { get; set; }
        string Review { get; set; }

        string Episode { get; set; }
        

        string Runtime { get; set; }
        string Tagline { get; set; }
        string Title { get; set; }
        string Year { get; set; }
        string Studio { get; set; }
        string Release_Date { get; set; }

        string GetGenresString();
        string GetTriviaString();
        string GetGoofsString();

        void SetDirectorString(string directors);
        void SetWriterString(string writers);
        void SetActorsString(string actors);
        void SetGenresString(string genres);
    }

    public interface IIMDbGoof
    {
        string Category { get; set; }
        string Description { get; set; }
    }

    public interface IIMDbQuote
    {
        string Character { get; set; }
        string Text { get; set; }
    }

}
