    #region Old Methods

        //public IMDbMovieCollection SearchMovies1(string title)
        //{
        //    //declarations
        //    IMDbMovieCollection movies = new IMDbMovieCollection();
        //    string text1="", text2="", text3 = "";
        //    int num1 = 0, num2 = 0, num3 = 0;
        //    bool flag1 = false;
      
        //    try
        //    {

        //        #region get html
        //    retry_download_html:
        //        try
        //        {
        //            text1 = JCUtils.WebUtils.GET("http://www.imdb.com/find?s=tt&q=" + System.Web.HttpUtility.UrlEncode(title));
        //            if (text1 == null || text1.Trim() == "")
        //            {
        //                if (num1 <= _retries)
        //                {
        //                    num1++;
        //                    System.Threading.Thread.Sleep(2000);
        //                    goto retry_download_html;
        //                }
        //            }
        //        }
        //        catch 
        //        {
        //            if (num3 <= _retries)
        //            {
        //                num3 +=1;
        //                System.Threading.Thread.Sleep(2000);
        //                goto retry_download_html;
        //            }
        //        }
        //        #endregion

        //        num1 = text1.ToLower().IndexOf("<b>popular titles");
        //        if (num1 > -1)
        //        {
        //            num1 += 3;
        //            num2 = text1.ToLower().IndexOf("more searches", num1);
        //            if (num2 > -1)
        //            {
        //                text1 = text1.Substring(num1, (num2 - num1)).Trim();
        //                flag1 = true;
        //            }
        //        }
        //        else
        //        {
        //            num1 = text1.ToLower().IndexOf("<b>titles");
        //            if (num1 > -1)
        //            {
        //                num1 += 3;
        //                num2 = text1.ToLower().IndexOf("more searches", num1);
        //                if (num2 > -1)
        //                {
        //                    text1 = text1.Substring(num1, (num2 - num1)).Trim();
        //                    flag1 = true;
        //                }
        //            }
        //            else
        //            {
        //                flag1 = false;
        //                num1 = text1.ToLower().IndexOf("<title>");
        //                if (num1 > -1)
        //                {
        //                    num1 += 7;
        //                    num2 = text1.ToLower().IndexOf("</title>", num1);
        //                    if (num2 > -1)
        //                    {
        //                        IMDbMovie movie = new IMDbMovie();
        //                        text2 = text1.Substring(num1, (num2 - num1)).Trim();
        //                        text2 = JCUtils.WebUtils.CleanUpHTML(text2, true).Trim();
        //                        text2 = JCUtils.WebUtils.FixString(text2).Trim();
        //                        num1 = text2.IndexOf("(");
        //                        if (num1 > -1)
        //                        {
        //                            movie.Title = text2.Substring(0, num1).Trim();
        //                            num1++;
        //                            num2 = text2.IndexOf(")", num1);
        //                            if (num2 > -1)
        //                                movie.Year = text2.Substring(num1, (num2 - num1)).Trim();
        //                        }
        //                        num1 = text1.ToLower().IndexOf("/title/");
        //                        if (num1 > -1)
        //                        {
        //                            num1 += 7;
        //                            num2 = text1.ToLower().IndexOf("/", num1);
        //                            if (num2 > -1)
        //                            {
        //                                movie.IMDb_ID = text1.Substring(num1, (num2 - num1)).Trim();
        //                                movies.Add(movie);
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        if (flag1)
        //        {
        //            num1 = 0;
        //            while (num1 > -1)
        //            {
        //            retry_title:
        //                num1 = text1.ToLower().IndexOf("/title/", num1);
        //                if (num1 > -1)
        //                {
        //                    num1 += 7;
        //                    num2 = text1.ToLower().IndexOf("/", num1);
        //                    if (num2 > -1)
        //                    {
        //                        IMDbMovie movie = new IMDbMovie();
        //                        movie.IMDb_ID = text1.Substring(num1, (num2 - num1)).Trim();
        //                        num1 = text1.ToLower().IndexOf(">", num2);
        //                        if (num1 > -1)
        //                        {
        //                            num1++;
        //                            num2 = text1.ToLower().IndexOf("<", num1);
        //                            if (num2 == num1)
        //                            {
        //                                goto retry_title;
        //                            }
        //                            if (num2 > -1)
        //                            {
                                        
        //                                text3 = text1.Substring(num1, (num2 - num1)).Trim();
        //                                text3 = JCUtils.WebUtils.CleanUpHTML(text3, true).Trim();
        //                                text3 = JCUtils.WebUtils.FixString(text3).Trim().Replace("(", "").Replace(")", "").Trim();
        //                                num1 = text1.ToLower().IndexOf(">", num2);
        //                                if (num1 > -1)
        //                                {
        //                                    num1++;
        //                                    num2 = text1.ToLower().IndexOf("<", num1);
        //                                    if (num2 > -1)
        //                                    {
        //                                        text3 += " (";
        //                                        text3 += text1.Substring(num1, (num2 - num1)).Trim().Replace("(", "").Replace(")", "") + ")";
        //                                    }
        //                                }
        //                                num2 = text3.IndexOf("(");
        //                                if (num2 > -1)
        //                                {
        //                                    movie.Title = text3.Substring(0, num2).Trim();
        //                                    num2++;
        //                                    num3 = text3.IndexOf(")", num2);
        //                                    if (num3 > -1)
        //                                        movie.Year = text3.Substring(num2, (num3 - num2)).Trim();
        //                                }
        //                                movies.Add(movie);
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        return movies;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public IMDbMovie GetMovieInfo1(string IMDbID)
        //{
        //    IMDbMovie movie = new IMDbMovie();
        //    string text1 = "", text2 = "", text3 = "", personURL = "";
        //    int num1 = 0, num2 = 0, num3 = 0, num4 = 0, num5 = 0;
        //    bool flag1 = false;

        //    try
        //    {

        //        #region Get HTML
        //    retry_download_html:
        //        try
        //        {
        //            text1 = JCUtils.WebUtils.GET("http://www.imdb.com/title/" + IMDbID.Trim());
        //            if (text1 == null || text1.Trim() == "")
        //                if (num1 <= _retries)
        //                {
        //                    num1++;
        //                    System.Threading.Thread.Sleep(2000);
        //                    goto retry_download_html;
        //                }
        //        }
        //        catch
        //        {
        //            if (num2 <= _retries)
        //            {
        //                System.Threading.Thread.Sleep(2000);
        //                num2++;
        //                goto retry_download_html;
        //            }
        //            else
        //            {
        //                return null;
        //            }
        //        }
        //        #endregion

        //        #region Clear garbage
        //        num2 = text1.ToLower().IndexOf("<a href=\"/\">imdb</a>");
        //        if (num2 > -1)
        //        {
        //            num2 += 20;
        //            num3 = text1.ToLower().IndexOf("show more recommendations</a>", num2);
        //            if (num3 > -1)
        //            {
        //                text1 = text1.Substring(num2, (num3 - num2)).Trim();
        //                flag1 = true;
        //            }
        //        }
        //        #endregion

        //        if (flag1)
        //        {
        //            movie.IMDb_ID = IMDbID;

        //            #region Get Title
        //            //Get Title
        //            text2 = "";
        //            num2 = text1.ToLower().IndexOf(">");
        //            if (num2 > -1)
        //            {
        //                num2++;
        //                num3 = text1.ToLower().IndexOf("<", num2);
        //                if (num3 > -1)
        //                {
        //                    text2 = text1.Substring(num2, (num3 - num2)).Trim();
        //                    text2 = JCUtils.WebUtils.CleanUpHTML(text2, true).Trim();
        //                    text2 = JCUtils.WebUtils.FixString(text2).Trim();

        //                    num1 = text2.IndexOf("(");
        //                    if (num1 > -1)
        //                    {
        //                        text2 = text2.Substring(0, num1).Trim();
        //                    }
        //                    movie.Title = text2.Trim();
        //                }
        //            }
        //            #endregion

        //            #region Get Production Year
        //            //Get Production Year
        //            text2 = "";
        //            num2 = text1.ToLower().IndexOf("/years/");
        //            if (num2 > -1)
        //            {
        //                num2 += 7;
        //                num3 = text1.ToLower().IndexOf("\"", num2);
        //                if (num3 > -1)
        //                {
        //                    text2 = text1.Substring(num2, (num3 - num2)).Trim();
        //                    text2 = JCUtils.WebUtils.CleanUpHTML(text2, true).Trim();
        //                    text2 = JCUtils.WebUtils.FixString(text2).Trim();
        //                    movie.Year = text2.Trim();
        //                }
        //            }
        //            #endregion

        //            #region Get Director(s)
        //            //Get Director(s)
        //            personURL = "";
        //            text2 = "";
        //            num2 = text1.ToLower().IndexOf("<h5>director");
        //            if (num2 > -1)
        //            {
        //                num2++;
        //                num3 = text1.ToLower().IndexOf("<h5>writer", num2);
        //                if (num3 < 0)
        //                {
        //                    num3 = text1.ToLower().IndexOf("<h5>genre", num2);
        //                }
        //                if (num3 > -1)
        //                {
        //                    text2 = text1.Substring(num2, (num3 - num2)).Trim();
        //                    num2 = 0;
        //                    do
        //                    {
        //                        num2 = text2.ToLower().IndexOf("/name/", num2);
        //                        if (num2 > -1)
        //                        {
        //                            num3 = text2.ToLower().IndexOf("\"", num2);
        //                            if (num3 > -1)
        //                            {
        //                                personURL = text2.Substring(num2, (num3 - num2)).Trim();
        //                            }
        //                            num2++;
        //                            num2 = text2.ToLower().IndexOf(">", num2);
        //                            if (num2 > -1)
        //                            {
        //                                num2++;
        //                                num3 = text2.ToLower().IndexOf("<", num2);
        //                                if (num3 > -1)
        //                                {
        //                                    text3 = text2.Substring(num2, (num3 - num2)).Trim();
        //                                    text3 = JCUtils.WebUtils.CleanUpHTML(text3, true).Trim();
        //                                    text3 = JCUtils.WebUtils.FixString(text3).Trim();
        //                                    IIMDbPerson person = movie.People.GetPersonByURL(personURL);
        //                                    if (person == null)
        //                                    {
        //                                        person = new IMDbPerson();
        //                                        movie.People.Add(person);
        //                                        person.Name = text3.Trim();
        //                                        person.URL = personURL;
        //                                    }
        //                                    person.IsDirector = true;
        //                                    personURL = "";
        //                                }
        //                            }
        //                        }
        //                    } while (num2 > -1);
        //                }
        //            }
        //            #endregion

        //            #region Get Writer(s)
        //            //Get Writer(s)
        //            personURL = "";
        //            text2 = "";
        //            num2 = text1.ToLower().IndexOf("<h5>writer");
        //            if (num2 > -1)
        //            {
        //                num2++;
        //                num3 = text1.ToLower().IndexOf("<h5>genre", num2);
        //                if (num3 > -1)
        //                {
        //                    text2 = text1.Substring(num2, (num3 - num2)).Trim();
        //                    num2 = 0;
        //                    do
        //                    {
        //                        num2 = text2.ToLower().IndexOf("/name/", num2);
        //                        if (num2 > -1)
        //                        {
        //                            num3 = text2.ToLower().IndexOf("\"", num2);
        //                            if (num3 > -1)
        //                            {
        //                                personURL = text2.Substring(num2, (num3 - num2)).Trim();
        //                            }
        //                            num2++;
        //                            num2 = text2.ToLower().IndexOf(">", num2);
        //                            if (num2 > -1)
        //                            {
        //                                num2++;
        //                                num3 = text2.ToLower().IndexOf("<", num2);
        //                                if (num3 > -1)
        //                                {
        //                                    text3 = text2.Substring(num2, (num3 - num2)).Trim();
        //                                    text3 = JCUtils.WebUtils.CleanUpHTML(text3, true).Trim();
        //                                    text3 = JCUtils.WebUtils.FixString(text3).Trim();
        //                                    IIMDbPerson person = movie.People.GetPersonByURL(personURL);
        //                                    if (person == null)
        //                                    {
        //                                        person = new IMDbPerson();
        //                                        movie.People.Add(person);
        //                                        person.Name = text3.Trim();
        //                                        person.URL = personURL;
        //                                    }
        //                                    person.IsWriter = true;
        //                                    personURL = "";
        //                                }
        //                            }
        //                        }
        //                    } while (num2 > -1);
        //                }
        //            }
        //            #endregion

        //            #region Get Genre(s)
        //            //Get Genre(s)
        //            text2 = "";
        //            num2 = text1.ToLower().IndexOf("<h5>genre");
        //            if (num2 > -1)
        //            {
        //                do
        //                {
        //                    num2 = text1.ToLower().IndexOf("/genres/", num2);
        //                    if (num2 > -1)
        //                    {
        //                        num2++;
        //                        num2 = text1.ToLower().IndexOf(">", num2);
        //                        if (num2 > -1)
        //                        {
        //                            num2++;
        //                            num3 = text1.ToLower().IndexOf("<", num2);
        //                            if (num3 > -1)
        //                            {
        //                                text2 = text1.Substring(num2, (num3 - num2)).Trim();
        //                                text2 = JCUtils.WebUtils.CleanUpHTML(text2, true).Trim();
        //                                text2 = JCUtils.WebUtils.FixString(text2).Trim();
        //                                movie.Genres.Add(text2.Trim());
        //                            }
        //                        }
        //                    }
        //                } while (num2 > -1);
        //            }
        //            #endregion

        //            #region Get Tagline
        //            //Get Tagline
        //            text2 = "";
        //            num2 = text1.ToLower().IndexOf("tagline:");
        //            if (num2 > -1)
        //            {
        //                num2 = text1.IndexOf(">", num2);
        //                if (num2 > -1)
        //                {
        //                    num2++;
        //                    num3 = text1.ToLower().IndexOf("<", num2);
        //                    if (num3 > -1)
        //                    {
        //                        text2 = text1.Substring(num2, (num3 - num2)).Trim();
        //                        text2 = JCUtils.WebUtils.CleanUpHTML(text2, true).Trim();
        //                        text2 = JCUtils.WebUtils.FixString(text2).Trim();
        //                        if (text2.ToLower().EndsWith("(more)"))
        //                        {
        //                            text2 = text2.Substring(0, text2.Length - 6).Trim();
        //                        }
        //                        else
        //                        {
        //                            text2 = text2.Trim();
        //                        }
        //                        num2 = text2.IndexOf("[");
        //                        if (num2 > -1)
        //                        {
        //                            text2 = text2.Substring(0, num2);
        //                        }
        //                        movie.Tagline = text2.Trim();
        //                    }
        //                }
        //            }
        //            #endregion

        //            #region Get Overview
        //            //Get Overview
        //            text2 = "";
        //            num2 = text1.ToLower().IndexOf("plot outline:");
        //            if (num2 < 0)
        //            {
        //                num2 = text1.ToLower().IndexOf("plot summary:");
        //            }
        //            if (num2 > -1)
        //            {
        //                num2 = text1.IndexOf(">", num2);
        //                if (num2 > -1)
        //                {
        //                    num2++;
        //                    num3 = text1.ToLower().IndexOf("<", num2);
        //                    if (num3 > -1)
        //                    {
        //                        text2 = text1.Substring(num2, (num3 - num2)).Trim();
        //                        text2 = JCUtils.WebUtils.CleanUpHTML(text2, true).Trim();
        //                        text2 = JCUtils.WebUtils.FixString(text2).Trim();
        //                        if (text2.ToLower().EndsWith("(more)"))
        //                        {
        //                            movie.OverviewShort = text2.Substring(0, text2.Length - 6).Trim() + "...";
        //                        }
        //                        else
        //                        {
        //                            movie.OverviewShort = text2.Trim();
        //                        }
        //                    }
        //                }
        //            }

        //            text2 = "";
        //            num2 = text1.ToLower().IndexOf("/" + IMDbID + "/plotsummary");
        //            if (num2 > -1)
        //            {
        //                string plothtml = "";
        //                string plottext = "";
        //                try
        //                {
        //                    plothtml = JCUtils.WebUtils.GET("http://www.imdb.com/title/" + IMDbID + "/plotsummary");
        //                }
        //                catch
        //                {
        //                    // do nothing
        //                }
        //                num2 = plothtml.ToLower().IndexOf("plotpar");
        //                if (num2 > -1)
        //                {
        //                    num2 = plothtml.IndexOf(">", num2);
        //                    if (num2 > -1)
        //                    {
        //                        num2++;
        //                        num3 = plothtml.ToLower().IndexOf("</p>", num2);
        //                        num4 = plothtml.ToLower().IndexOf("<i>", num2);
        //                        if (num3 > -1)
        //                        {
        //                            if (num4 > -1 && num4 < num3)
        //                            {
        //                                plottext = plothtml.Substring(num2, (num4 - num2)).Trim();
        //                                plottext = JCUtils.WebUtils.CleanUpHTML(plottext, true).Trim();
        //                                plottext = JCUtils.WebUtils.FixString(plottext).Trim();
        //                            }
        //                            else if (num3 > -1)
        //                            {
        //                                plottext = plothtml.Substring(num2, (num3 - num2)).Trim();
        //                                plottext = JCUtils.WebUtils.CleanUpHTML(plottext, true).Trim();
        //                                plottext = JCUtils.WebUtils.FixString(plottext).Trim();
        //                            }
        //                        }
        //                    }
        //                }
        //                if (plottext.Trim() != "")
        //                    movie.OverviewLong = plottext.Trim();
        //            }
        //            #endregion

        //            #region Get Review
        //            //Get Review
        //            text2 = "";
        //            num2 = text1.ToLower().IndexOf("user rating:");
        //            if (num2 > -1)
        //            {
        //                num2 = text1.IndexOf("<b>", num2);
        //                if (num2 > -1)
        //                {
        //                    num2 += 3;
        //                    num3 = text1.ToLower().IndexOf("/", num2);
        //                    if (num3 > -1)
        //                    {
        //                        text2 = text1.Substring(num2, (num3 - num2)).Trim();
        //                        text2 = JCUtils.WebUtils.CleanUpHTML(text2, true).Trim();
        //                        text2 = JCUtils.WebUtils.FixString(text2).Trim();
        //                        movie.Review = text2.Trim();
        //                    }
        //                }
        //            }
        //            #endregion

        //            #region Get Actors
        //            //Get Actors
        //            string actorText = JCUtils.WebUtils.GET("http://www.imdb.com/title/" + IMDbID.Trim() + "/fullcredits#cast");
        //            personURL = "";
        //            text2 = "";
        //            num2 = actorText.ToLower().IndexOf("cast</a>");
        //            if (num2 > -1)
        //            {
        //                num2++;
        //                num3 = actorText.ToLower().IndexOf("produced by<", num2);
        //                if (num3 > -1)
        //                {
        //                    text2 = actorText.Substring(num2, (num3 - num2)).Trim();
        //                    num1 = 0;
        //                    num2 = 0;
        //                    do
        //                    {
        //                        if (num1 < _actorCount)
        //                        {
        //                            num2 = text2.ToLower().IndexOf("/name/", num2);
        //                            if (num2 > -1)
        //                            {
        //                                num5 = num2;
        //                                num3 = text2.ToLower().IndexOf("\"", num2);
        //                                if (num3 > -1)
        //                                    personURL = text2.ToLower().Substring(num2, (num3 - num2)).Trim();
        //                                num2 = text2.ToLower().IndexOf(">", num2);
        //                                if (num2 > -1)
        //                                {
        //                                    num2++;
        //                                    num3 = text2.ToLower().IndexOf("<img", num2);
        //                                    if (num3 > -1)
        //                                    {
        //                                        if (num3 != num2)
        //                                        {
        //                                            num3 = text2.IndexOf("<", num2);
        //                                            if (num3 > -1 && num3 != num2)
        //                                            {
        //                                                text3 = text2.Substring(num2, (num3 - num2)).Trim();
        //                                                text3 = JCUtils.WebUtils.CleanUpHTML(text3, true).Trim();
        //                                                text3 = JCUtils.WebUtils.FixString(text3).Trim();
        //                                                IIMDbPerson person = movie.People.GetPersonByURL(personURL);
        //                                                if (person == null)
        //                                                {
        //                                                    person = new IMDbPerson();
        //                                                    movie.People.Add(person);
        //                                                    person.Name = text3.Trim();
        //                                                    person.URL = personURL;
        //                                                }
        //                                                person.IsActor = true;

        //                                                // get character roles
        //                                                string sPersonID = personURL;
        //                                                if (sPersonID.EndsWith("/"))
        //                                                    sPersonID = sPersonID.Substring(0, (sPersonID.Length - 1));
        //                                                sPersonID = sPersonID.Substring((sPersonID.LastIndexOf("/") + 1));
        //                                                num4 = text2.ToLower().IndexOf(" ... ", num3);
        //                                                if (num4 > -1)
        //                                                {
        //                                                    bool bCharacter = false;
        //                                                    num5 = text2.ToLower().IndexOf("/character/", num4);
        //                                                    if (num5 > -1)
        //                                                    {
        //                                                        if ((num5 - num4) < 50)
        //                                                        {
        //                                                            bCharacter = true;
        //                                                            num3 = text2.IndexOf(">", num5);
        //                                                            if (num3 > -1)
        //                                                            {
        //                                                                num3++;
        //                                                                num4 = text2.IndexOf("<", num3);
        //                                                                if (num4 > -1)
        //                                                                {
        //                                                                    text3 = text2.Substring(num3, (num4 - num3)).Trim();
        //                                                                    text3 = JCUtils.WebUtils.CleanUpHTML(text3, true).Trim();
        //                                                                    text3 = JCUtils.WebUtils.FixString(text3).Trim();
        //                                                                    person.AddRole(text3.Trim());
        //                                                                }
        //                                                            }
        //                                                        }
        //                                                    }
        //                                                    if (bCharacter == false)
        //                                                    {
        //                                                        num3 = num4 + 10;
        //                                                        num3 = text2.IndexOf(">", num3);
        //                                                        if (num3 > -1)
        //                                                        {
        //                                                            num3++;
        //                                                            num4 = text2.ToLower().IndexOf("<", num3);
        //                                                            if (num4 > -1)
        //                                                            {
        //                                                                text3 = text2.Substring(num3, (num4 - num3)).Trim();
        //                                                                text3 = JCUtils.WebUtils.CleanUpHTML(text3, true).Trim();
        //                                                                text3 = JCUtils.WebUtils.FixString(text3).Trim();
        //                                                                person.AddRole(text3.Trim());
        //                                                            }
        //                                                            else
        //                                                                person.AddRole("Unknown");
        //                                                        }
        //                                                        else
        //                                                            person.AddRole("Unknown");
        //                                                    }
        //                                                }
        //                                                else
        //                                                    person.AddRole("Unknown");
        //                                                num1++;
        //                                            }
        //                                        }
        //                                        else
        //                                        {
        //                                            num3++;
        //                                            num3 = text2.ToLower().IndexOf("/name/", num3);
        //                                            if (num3 > -1)
        //                                            {
        //                                                num2 = text2.IndexOf(">", num3);
        //                                                if (num2 > -1)
        //                                                {
        //                                                    num2++;
        //                                                    num3 = text2.IndexOf("<", num2);
        //                                                    if (num3 > -1 && num3 != num2)
        //                                                    {
        //                                                        text3 = text2.Substring(num2, (num3 - num2)).Trim();
        //                                                        text3 = JCUtils.WebUtils.CleanUpHTML(text3, true).Trim();
        //                                                        text3 = JCUtils.WebUtils.FixString(text3).Trim();
        //                                                        IIMDbPerson person = movie.People.GetPersonByURL(personURL);
        //                                                        if (person == null)
        //                                                        {
        //                                                            person = new IMDbPerson();
        //                                                            movie.People.Add(person);
        //                                                            person.Name = text3.Trim();
        //                                                            person.URL = personURL;
        //                                                        }
        //                                                        person.IsActor = true;

        //                                                        // get character roles
        //                                                        string sPersonID = personURL;
        //                                                        if (sPersonID.EndsWith("/"))
        //                                                            sPersonID = sPersonID.Substring(0, (sPersonID.Length - 1));
        //                                                        sPersonID = sPersonID.Substring((sPersonID.LastIndexOf("/") + 1));
        //                                                        num4 = text2.ToLower().IndexOf(" ... ", num3);
        //                                                        if (num4 > -1)
        //                                                        {
        //                                                            bool bCharacter = false;
        //                                                            num5 = text2.ToLower().IndexOf("/character/", num4);
        //                                                            if (num5 > -1)
        //                                                            {
        //                                                                if ((num5 - num4) < 50)
        //                                                                {
        //                                                                    bCharacter = true;
        //                                                                    num3 = text2.IndexOf(">", num5);
        //                                                                    if (num3 > -1)
        //                                                                    {
        //                                                                        num3++;
        //                                                                        num4 = text2.IndexOf("<", num3);
        //                                                                        if (num4 > -1)
        //                                                                        {
        //                                                                            text3 = text2.Substring(num3, (num4 - num3)).Trim();
        //                                                                            text3 = JCUtils.WebUtils.CleanUpHTML(text3, true).Trim();
        //                                                                            text3 = JCUtils.WebUtils.FixString(text3).Trim();
        //                                                                            person.AddRole(text3.Trim());
        //                                                                        }
        //                                                                    }
        //                                                                }
        //                                                            }
        //                                                            if (bCharacter == false)
        //                                                            {
        //                                                                num3 = num4 + 10;
        //                                                                num3 = text2.IndexOf(">", num3);
        //                                                                if (num3 > -1)
        //                                                                {
        //                                                                    num3++;
        //                                                                    num4 = text2.ToLower().IndexOf("<", num3);
        //                                                                    if (num4 > -1)
        //                                                                    {
        //                                                                        text3 = text2.Substring(num3, (num4 - num3)).Trim();
        //                                                                        text3 = JCUtils.WebUtils.CleanUpHTML(text3, true).Trim();
        //                                                                        text3 = JCUtils.WebUtils.FixString(text3).Trim();
        //                                                                        person.AddRole(text3.Trim());
        //                                                                    }
        //                                                                    else
        //                                                                        person.AddRole("Unknown");
        //                                                                }
        //                                                                else
        //                                                                    person.AddRole("Unknown");
        //                                                            }
        //                                                        }
        //                                                        else
        //                                                            person.AddRole("Unknown");
        //                                                        num1++;
        //                                                    }
        //                                                }
        //                                            }
        //                                        }
        //                                    }
        //                                    else
        //                                    {
        //                                        num2 = text2.IndexOf(">", num5);
        //                                        if (num2 > -1)
        //                                        {

        //                                            num2++;
        //                                            num3 = text2.IndexOf("<", num2);
        //                                            if (num3 > -1)
        //                                            {
        //                                                text3 = text2.Substring(num2, (num3 - num2)).Trim();
        //                                                text3 = JCUtils.WebUtils.CleanUpHTML(text3, true).Trim();
        //                                                text3 = JCUtils.WebUtils.FixString(text3).Trim();
        //                                                IIMDbPerson person = movie.People.GetPersonByURL(personURL);
        //                                                if (person == null)
        //                                                {
        //                                                    person = new IMDbPerson();
        //                                                    movie.People.Add(person);
        //                                                    person.Name = text3.Trim();
        //                                                    person.URL = personURL;
        //                                                }
        //                                                person.IsActor = true;
        //                                                num4 = text2.ToLower().IndexOf(" ... ", num3);
        //                                                if (num4 > -1)
        //                                                {
        //                                                    bool bCharacter = false;
        //                                                    num5 = text2.ToLower().IndexOf("/character/", num4);
        //                                                    if (num5 > -1)
        //                                                    {
        //                                                        if ((num5 - num4) < 50)
        //                                                        {
        //                                                            bCharacter = true;
        //                                                            num3 = text2.IndexOf(">", num5);
        //                                                            if (num3 > -1)
        //                                                            {
        //                                                                num3++;
        //                                                                num4 = text2.IndexOf("<", num3);
        //                                                                if (num4 > -1)
        //                                                                {
        //                                                                    text3 = text2.Substring(num3, (num4 - num3)).Trim();
        //                                                                    text3 = JCUtils.WebUtils.CleanUpHTML(text3, true).Trim();
        //                                                                    text3 = JCUtils.WebUtils.FixString(text3).Trim();
        //                                                                    person.AddRole(text3.Trim());
        //                                                                }
        //                                                            }
        //                                                        }
        //                                                    }
        //                                                    if (bCharacter == false)
        //                                                    {
        //                                                        num3 = num4 + 10;
        //                                                        num3 = text2.IndexOf(">", num3);
        //                                                        if (num4 > -1)
        //                                                        {
        //                                                            num3++;
        //                                                            num4 = text2.ToLower().IndexOf("<", num3);
        //                                                            if (num4 > -1)
        //                                                            {
        //                                                                text3 = text2.Substring(num3, (num4 - num3)).Trim();
        //                                                                text3 = JCUtils.WebUtils.CleanUpHTML(text3, true).Trim();
        //                                                                text3 = JCUtils.WebUtils.FixString(text3).Trim();
        //                                                                person.AddRole(text3.Trim());
        //                                                            }
        //                                                            else
        //                                                                person.AddRole("Unknown");
        //                                                        }
        //                                                        else
        //                                                            person.AddRole("Unknown");
        //                                                    }
        //                                                }
        //                                                else
        //                                                    person.AddRole("Unknown");
        //                                                num1++;
        //                                            }
        //                                        }
        //                                    }
        //                                }
        //                            }
        //                        }
        //                        else
        //                        {
        //                            break;
        //                        }
        //                    } while (num2 > -1);
        //                }
        //            }
        //            #endregion

        //            #region Get Runtime
        //            //Get Runtime
        //            text2 = "";
        //            num2 = text1.ToLower().IndexOf("runtime:");
        //            if (num2 > -1)
        //            {
        //                num2 = text1.IndexOf(">", num2);
        //                if (num2 > -1)
        //                {
        //                    num2++;
        //                    num3 = text1.ToLower().IndexOf("<", num2);
        //                    if (num3 > -1)
        //                    {
        //                        text2 = text1.Substring(num2, (num3 - num2)).Trim();
        //                        text2 = JCUtils.WebUtils.CleanUpHTML(text2, true).Trim();
        //                        text2 = JCUtils.WebUtils.FixString(text2).Trim();
        //                        text2 = FixRuntime(text2).Trim();
        //                        movie.Runtime = text2.Trim();
        //                    }
        //                }
        //            }
        //            #endregion

        //            #region Get Rating
        //            //Get Rating
        //            text2 = "";
        //            num2 = text1.ToLower().IndexOf("certification:");
        //            if (num2 > -1)
        //            {
        //                num2++;
        //                num2 = text1.ToLower().IndexOf(System.Web.HttpUtility.UrlEncode(_country).ToLower() + ":", num2);
        //                if (num2 > -1)
        //                {
        //                    num2++;
        //                    num2 = text1.ToLower().IndexOf(">", num2);
        //                    if (num2 > -1)
        //                    {
        //                        num2 += _country.Length;
        //                        num2 += 2;
        //                        num3 = text1.ToLower().IndexOf("</a>", num2);
        //                        if (num3 > -1)
        //                        {
        //                            text2 = text1.Substring(num2, (num3 - num2)).Trim();
        //                            text2 = JCUtils.WebUtils.CleanUpHTML(text2, true).Trim();
        //                            text2 = JCUtils.WebUtils.FixString(text2).Trim();
        //                            if (text2.Trim() == ""
        //                                | text2.ToLower().Trim() == "not rated")
        //                            {
        //                                text2 = "Unrated";
        //                            }
        //                            if (text2.ToLower().Trim() == "approved")
        //                            {
        //                                text2 = "G";
        //                            }
        //                            movie.Rating = text2.Trim();
        //                        }
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                num2 = text1.ToLower().IndexOf("mpaa</a>:</h5>");
        //                if (num2 > -1)
        //                {
        //                    num2 = text1.ToLower().IndexOf("rated", num2);
        //                    if (num2 > -1)
        //                    {
        //                        num2 += 5;
        //                        num3 = text1.ToLower().IndexOf("for ", num2);
        //                        if (num3 > -1)
        //                        {
        //                            movie.Rating = text1.Substring(num2, (num3 - num2)).Trim();
        //                        }
        //                    }
        //                }
        //            }
        //            #endregion

        //        }
        //        return movie;
        //    }
        //    catch (Exception ex)
        //    {
 
        //       throw ex;
        //    }
        //}

        //public IMDbMovieCollection SearchMovieInfo1(string title)
        //{
        //    IMDbMovieCollection movies = new IMDbMovieCollection();

        //    try
        //    {
        //        IMDbMovieCollection results = SearchMovies1(title);
        //        if (results != null)
        //        {
        //            foreach (IMDbMovie result in results)
        //            {
        //                if (result.IMDb_ID.Trim() != "")
        //                {
        //                    IMDbMovie movie = GetMovieInfo1(result.IMDb_ID.Trim());
        //                    if (movie != null)
        //                        movies.Add(movie);
        //                }
        //            }
        //        }
        //        return movies;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        #endregion
































    //public IIMDbMovie GetMovieInfo(string imdbID)
        //{
        //    string movieURL = _url + imdbID + "/";
        //    IMDbMovie movie = new IMDbMovie();

        //    string html = ToolBox.WebUtils.GetSiteContents(movieURL); //Old Code:  JCUtils.WebUtils.GET(movieURL);
        //    string trimmedHTML;
        //    string creditsURL = movieURL + "fullcredits";
        //    string longOverviewURL = movieURL + "plotsummary";
        //    string goofURL = movieURL + "goofs";
        //    string triviaURL = movieURL + "trivia";
        //    string quotesURL = movieURL + "quotes";

        //    // clean-up html (trim to area we are interested in)
        //    trimmedHTML = GetRegExString(html, _trimmedHTMLpattern);

        //    // Grab Title
        //    Match match = GetRegExMatch(trimmedHTML, _titlePattern);
        //    movie.IMDb_ID = imdbID;
        //    movie.Title = GetMatchValue(match, "Title", true);

        //    // Grab Year
        //    match = GetRegExMatch(trimmedHTML, _yearPattern);
        //    movie.Year = GetMatchValue(match, "Year", true);

        //    // Grab Release Date
        //    match = GetRegExMatch(trimmedHTML, _releaseDatePattern);
        //    movie.Release_Date = GetMatchValue(match, "ReleaseDate", true);

        //    // Grab Tagline
        //    match = GetRegExMatch(trimmedHTML, _taglinePattern);
        //    movie.Tagline = GetMatchValue(match, "Tagline", true);

        //    // Grab Runtime
        //    match = GetRegExMatch(trimmedHTML, _runtimePattern);
        //    string runtime = GetMatchValue(match, "Runtime", true);
        //    runtime = FixRuntime(runtime);
        //    movie.Runtime = runtime;

        //    // Grab Rating
        //    match = GetRegExMatch(trimmedHTML, _ratingPattern);
        //    movie.Rating = GetMatchValue(match, "Rating", true);

        //    // Grab Review
        //    match = GetRegExMatch(trimmedHTML, _reviewPattern);
        //    movie.Review = GetMatchValue(match, "Review", true);
        //    if (movie.Review.Contains("/"))
        //        movie.Review = movie.Review.Substring(0, movie.Review.IndexOf("/"));

        //    // Grab Studio
        //    match = GetRegExMatch(trimmedHTML, _studioPattern);
        //    movie.Studio = GetMatchValue(match, "Studio", true);

        //    // Grab Short Overview
        //    match = GetRegExMatch(html, _shortOverviewPattern);
        //    movie.OverviewShort = GetMatchValue(match, "ShortOverview", true);
        //    if (movie.OverviewShort.ToLower().EndsWith("more"))
        //        movie.OverviewShort = movie.OverviewShort.Substring(0, movie.OverviewShort.Length - 4).Trim();
        //    movie.OverviewShort = movie.OverviewShort.Trim() + "...";

        //    GetLongOverview(ref movie, longOverviewURL);

        //    GetDirectors(ref movie, trimmedHTML);

        //    GetWriters(ref movie, trimmedHTML);

        //    GetActors(ref movie, creditsURL);

        //    GetGenres(ref movie, trimmedHTML);

        //    GetTrivia(ref movie, triviaURL);

        //    GetGoofs(ref movie, goofURL);

        //    GetQuotes(ref movie, quotesURL);

        //    return movie;
        //}
