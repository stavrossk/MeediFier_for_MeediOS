using System;
using System.Collections.Generic;
using System.Text;

namespace MeediFier
{
    internal class IMDbMovie : IIMDbMovie
    {
        
        #region Constants and Variables

        private List<string> _genres = new List<string>();
        private string _imdbID = String.Empty;
        private string _overviewShort = String.Empty;
        private string _rating = String.Empty;
        private string _ratingDescription = String.Empty;
        private string _review = String.Empty;

        private string _Episode = String.Empty;

        private string _runtime = String.Empty;
        private string _tagline = String.Empty;
        private string _title = String.Empty;
        private string _year = String.Empty;
        private string _overviewLong = String.Empty;
        private string _studio = String.Empty;
        private string _releaseDate = String.Empty;
        private IMDbPersonCollection _people = new IMDbPersonCollection();
        private List<IIMDbGoof> _goofs = new List<IIMDbGoof>();
        private List<IList<IIMDbQuote>> _quotes = new List<IList<IIMDbQuote>>();
        private List<string> _trivia = new List<string>();

        #endregion

        
        #region Properties

        public IIMDbPersonCollection People
        {
            get { return _people; }
        }

        public IList<string> Genres
        {
            get { return _genres; }
        }

        public IList<IIMDbGoof> Goofs
        {
            get { return _goofs; }
        }

        public IList<IList<IIMDbQuote>> Quotes
        {
            get { return _quotes; }
        }

        public IList<string> Trivia
        {
            get { return _trivia; }
        }

        public string IMDb_ID
        {
            get { return _imdbID; }
            set { _imdbID = value; }
        }

        public string OverviewShort
        {
            get { return _overviewShort; }
            set { _overviewShort = value; }
        }

        public string OverviewLong
        {
            get { return _overviewLong; }
            set { _overviewLong = value; }
        }

        public string Rating
        {
            get { return _rating; }
            set { _rating = value; }
        }

        public string RatingDescription
        {
            get { return _ratingDescription; }
            set { _ratingDescription = value; }
        }


        public string Review
        {
            get { return _review; }
            set { _review = value; }
        }

        public string Episode
        {
            get { return _Episode; }
            set { _Episode = value; }
        }


        public string Runtime
        {
            get { return _runtime; }
            set { _runtime = value; }
        }

        public string Tagline
        {
            get { return _tagline; }
            set { _tagline = value; }

        }

        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        public string Year
        {
            get { return _year; }
            set { _year = value; }
        }

        public string Studio
        {
            get { return _studio; }
            set { _studio = value; }
        }

        public string Release_Date
        {
            get { return _releaseDate; }
            set { _releaseDate = value; }
        }

        #endregion


        #region Public Methods

        public string GetGenresString()
        {
            StringBuilder sbGenre = new StringBuilder();
            for (int i = 0; i < _genres.Count; i++)
            {
                if (i == 0 && _genres.Count > 1)
                    sbGenre.Append("|");

                sbGenre.Append(_genres[i]);
                
                if (_genres.Count > 1)
                    sbGenre.Append("|");
            }
            return sbGenre.ToString();
        }

        public string GetTriviaString()
        {
            StringBuilder sbTrivia = new StringBuilder();
            for (int i = 0; i < _trivia.Count; i++)
            {
                if (i == 0 && _trivia.Count > 1)
                    sbTrivia.Append("|");

                sbTrivia.Append(_trivia[i]);

                if (_trivia.Count > 1)
                    sbTrivia.Append("|");
            }
            return sbTrivia.ToString();
        }


        public string GetGoofsString()
        {
            StringBuilder sbGoofs = new StringBuilder();
            for (int i = 0; i < _goofs.Count; i++)
            {
                if (i == 0 && _goofs.Count > 1)
                    sbGoofs.Append("|");

                sbGoofs.Append(_goofs[i].Category + ": " + _goofs[i].Description);

                if (_goofs.Count > 1)
                    sbGoofs.Append("|");
            }
            return sbGoofs.ToString();
        }


        public string GetQuotesString()
        {
            StringBuilder sbQuotes = new StringBuilder();

            for (int i = 0; i < _quotes.Count; i++)
            //foreach (IList<IIMDbQuote> quoteBlock in _quotes)
            {
                if (i == 0 && _quotes.Count > 1)
                    sbQuotes.Append("|");
                
               
                    foreach (IIMDbQuote quote in _quotes[i] )
                    {
                        sbQuotes.Append(quote.Character + ": " + quote.Text + Environment.NewLine);
                    }

                    if (_quotes.Count > 1)
                    {
                        sbQuotes.Append(Environment.NewLine + Environment.NewLine);
                        sbQuotes.Append("|");
                    }

            }
            return sbQuotes.ToString();
        }





        #region Set
        public void SetDirectorString(string directors)
        {
            int num1 = 0, num2 = 0;
            if (directors.IndexOf("|") < 0)
            {
                IMDbPerson person = GetPerson(directors);
                if (person != null)
                    person.IsDirector = true;
                else
                {
                    IMDbPerson newDirector = new IMDbPerson();
                    newDirector.Name = directors;
                    newDirector.IsDirector = true;
                    _people.Add(newDirector);
                }
            }
            else
                while (num1 > -1 && num1 < directors.Length)
                {
                    num1 = directors.IndexOf("|", num1);
                    if (num1 > -1)
                    {
                        num1++;
                        num2 = directors.IndexOf("|", num1);
                        if (num2 > -1)
                        {
                            string name = directors.Substring(num1, (num2 - num1)).Trim();
                            IMDbPerson person = GetPerson(name);
                            if (person != null)
                                person.IsDirector = true;
                            else
                            {
                                IMDbPerson newDirector = new IMDbPerson();
                                newDirector.Name = name;
                                newDirector.IsDirector = true;
                                _people.Add(newDirector);
                            }
                        }
                    }
                }
        }

        public void SetWriterString(string writers)
        {
            int num1 = 0, num2 = 0;
            if (writers.IndexOf("|") < 0)
            {
                IMDbPerson person = GetPerson(writers);
                if (person != null)
                    person.IsWriter = true;
                else
                {
                    IMDbPerson newWriter = new IMDbPerson();
                    newWriter.Name = writers;
                    newWriter.IsWriter = true;
                    _people.Add(newWriter);
                }
            }
            else
                while (num1 > -1 && num1 < writers.Length)
                {
                    num1 = writers.IndexOf("|", num1);
                    if (num1 > -1)
                    {
                        num1++;
                        num2 = writers.IndexOf("|", num1);
                        if (num2 > -1)
                        {
                            string name = writers.Substring(num1, (num2 - num1)).Trim();
                            IMDbPerson person = GetPerson(name);
                            if (person != null)
                                person.IsWriter = true;
                            else
                            {
                                IMDbPerson newWriter = new IMDbPerson();
                                newWriter.Name = name;
                                newWriter.IsWriter = true;
                                _people.Add(newWriter);
                            }
                        }
                    }
                }
        }

        public void SetActorsString(string actors)
        {
            int num1 = 0, num2 = 0;
            if (actors.IndexOf("|") < 0)
            {
                IMDbPerson person = GetPerson(actors);
                if (person != null)
                    person.IsActor = true;
                else
                {
                    IMDbPerson newActor = new IMDbPerson();
                    newActor.Name = actors;
                    newActor.IsActor = true;
                    _people.Add(newActor);
                }
            }
            else
                while (num1 > -1 && num1 < actors.Length)
                {
                    num1 = actors.IndexOf("|", num1);
                    if (num1 > -1)
                    {
                        num1++;
                        num2 = actors.IndexOf("|", num1);
                        if (num2 > -1)
                        {
                            string name = actors.Substring(num1, (num2 - num1)).Trim();
                            IMDbPerson person = GetPerson(name);
                            if (person != null)
                                person.IsActor = true;
                            else
                            {
                                IMDbPerson newActor = new IMDbPerson();
                                newActor.Name = name;
                                newActor.IsActor = true;
                                _people.Add(newActor);
                            }
                        }
                    }
                }
        }

        public void SetGenresString(string genres)
        {
            int num1 = 0, num2 = 0;
            if (genres.IndexOf("|") < 0)
            {
                bool exists = false;
                foreach (string genre in _genres)
                {
                    if (genre.ToLower().Trim() == genres.ToLower().Trim())
                    {
                        exists = true;
                        break;
                    }
                }
                if (!exists)
                    _genres.Add(genres);
            }
            else
                while (num1 > -1 && num1 < genres.Length)
                {
                    num1 = genres.IndexOf("|", num1);
                    if (num1 > -1)
                    {
                        num1++;
                        num2 = genres.IndexOf("|", num1);
                        if (num2 > -1)
                        {
                            string newGenre = genres.Substring(num1, (num2 - num1)).Trim();
                            bool exists = false;
                            foreach (string genre in _genres)
                            {
                                if (genre.ToLower().Trim() == newGenre.ToLower().Trim())
                                {
                                    exists = true;
                                    break;
                                }
                            }
                            if (!exists)
                                _genres.Add(newGenre);
                        }
                    }
                }
        }
        #endregion

        #endregion


        #region Private Methods

        private IMDbPerson GetPerson(string personName)
        {
            foreach (IMDbPerson person in _people)
            {
                if (person.Name.ToLower().Trim() == personName.ToLower().Trim())
                    return person;
            }
            return null;
        }

        #endregion

    }

    internal class IMDbGoof : IIMDbGoof
    {
        private string _category = "";
        public string Category
        {
            get { return _category; }
            set { _category = value; }
        }

        private string _description = "";
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }
    }

    internal class IMDbQuote : IIMDbQuote
    {
        private string _character = "";
        public string Character
        {
            get { return _character; }
            set { _character = value; }
        }

        private string _text = "";
        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }
    }
}
