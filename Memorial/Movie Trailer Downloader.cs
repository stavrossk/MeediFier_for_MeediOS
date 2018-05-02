
        public static bool DownloadTrailer(IMLItem Item, string ItemTitle)
        {
            WebClient client = new WebClient();
            client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");

            ItemTitle = ItemTitle.Replace(" ", "");
            ItemTitle = ItemTitle.ToLower();
            string studio = "fox_searchlight";
            string quality = "_h.640w";
            string baseUrl = "http://movies.apple.com/movies/" + studio + "/" + ItemTitle + "/" + ItemTitle + quality + ".mov";

            Helpers.UpdateProgress("Updating Films Section...", "Searching for trailer for " + ItemTitle + "...",  Item);

            #region switch studios until match is found
            try
            {
                Helpers.UpdateProgress("Updating Films Section...", "Downloading trailer for " + ItemTitle + "...",  Item);
                client.DownloadFile(baseUrl, @"D:\" + ItemTitle + ".mov");
            }
            catch (Exception)
            {
                studio = "fox";
                baseUrl = "http://movies.apple.com/movies/" + studio + "/" + ItemTitle + "/" + ItemTitle + quality + ".mov";
                try
                {
                    Helpers.UpdateProgress("Updating Films Section...", "Downloading trailer for " + ItemTitle + "...",  Item);
                    client.DownloadFile(baseUrl, @"D:\" + ItemTitle + ".mov");
                }
                catch (Exception)
                {
                    studio = "dreamworks";
                    baseUrl = "http://movies.apple.com/movies/" + studio + "/" + ItemTitle + "/" + ItemTitle + quality + ".mov";
                    try
                    {
                        Helpers.UpdateProgress("Updating Films Section...", "Downloading trailer for " + ItemTitle + "...",  Item);
                        client.DownloadFile(baseUrl, @"D:\" + ItemTitle + ".mov");
                    }
                    catch (Exception)
                    {
                        studio = "focus_features";
                        baseUrl = "http://movies.apple.com/movies/" + studio + "/" + ItemTitle + "/" + ItemTitle + quality + ".mov";
                        try
                        {
                            Helpers.UpdateProgress("Updating Films Section...", "Downloading trailer for " + ItemTitle + "...",  Item);
                            client.DownloadFile(baseUrl, @"D:\" + ItemTitle + ".mov");
                        }
                        catch (Exception)
                        {
                            studio = "independent";
                            baseUrl = "http://movies.apple.com/movies/" + studio + "/" + ItemTitle + "/" + ItemTitle + quality + ".mov";
                            try
                            {
                                Helpers.UpdateProgress("Updating Films Section...", "Downloading trailer for " + ItemTitle + "...",  Item);
                                client.DownloadFile(baseUrl, @"D:\" + ItemTitle + ".mov");
                            }
                            catch (Exception)
                            {
                                studio = "lions_gate";
                                baseUrl = "http://movies.apple.com/movies/" + studio + "/" + ItemTitle + "/" + ItemTitle + quality + ".mov";
                                try
                                {
                                    Helpers.UpdateProgress("Updating Films Section...", "Downloading trailer for " + ItemTitle + "...",  Item);
                                    client.DownloadFile(baseUrl, @"D:\" + ItemTitle + ".mov");
                                }
                                catch (Exception)
                                {
                                    studio = "magnolia";
                                    baseUrl = "http://movies.apple.com/movies/" + studio + "/" + ItemTitle + "/" + ItemTitle + quality + ".mov";
                                    try
                                    {
                                        Helpers.UpdateProgress("Updating Films Section...", "Downloading trailer for " + ItemTitle + "...",  Item);
                                        client.DownloadFile(baseUrl, @"D:\" + ItemTitle + ".mov");
                                    }
                                    catch (Exception)
                                    {
                                        studio = "mgm";
                                        baseUrl = "http://movies.apple.com/movies/" + studio + "/" + ItemTitle + "/" + ItemTitle + quality + ".mov";
                                        try
                                        {
                                            Helpers.UpdateProgress("Updating Films Section...", "Downloading trailer for " + ItemTitle + "...",  Item);
                                            client.DownloadFile(baseUrl, @"D:\" + ItemTitle + ".mov");
                                        }
                                        catch (Exception)
                                        {
                                            studio = "miramax";
                                            baseUrl = "http://movies.apple.com/movies/" + studio + "/" + ItemTitle + "/" + ItemTitle + quality + ".mov";
                                            try
                                            {
                                                Helpers.UpdateProgress("Updating Films Section...", "Downloading trailer for " + ItemTitle + "...",  Item);
                                                client.DownloadFile(baseUrl, @"D:\" + ItemTitle + ".mov");
                                            }
                                            catch (Exception)
                                            {
                                                studio = "paramount";
                                                baseUrl = "http://movies.apple.com/movies/" + studio + "/" + ItemTitle + "/" + ItemTitle + quality + ".mov";
                                                try
                                                {
                                                    Helpers.UpdateProgress("Updating Films Section...", "Downloading trailer for " + ItemTitle + "...",  Item);
                                                    client.DownloadFile(baseUrl, @"D:\" + ItemTitle + ".mov");
                                                }
                                                catch (Exception)
                                                {
                                                    studio = "sony_pictures";
                                                    baseUrl = "http://movies.apple.com/movies/" + studio + "/" + ItemTitle + "/" + ItemTitle + quality + ".mov";
                                                    try
                                                    {
                                                        Helpers.UpdateProgress("Updating Films Section...", "Downloading trailer for " + ItemTitle + "...",  Item);
                                                        client.DownloadFile(baseUrl, @"D:\" + ItemTitle + ".mov");
                                                    }
                                                    catch (Exception)
                                                    {
                                                        studio = "sony";
                                                        baseUrl = "http://movies.apple.com/movies/" + studio + "/" + ItemTitle + "/" + ItemTitle + quality + ".mov";
                                                        try
                                                        {
                                                            Helpers.UpdateProgress("Updating Films Section...", "Downloading trailer for " + ItemTitle + "...",  Item);
                                                            client.DownloadFile(baseUrl, @"D:\" + ItemTitle + ".mov");
                                                        }
                                                        catch (Exception)
                                                        {
                                                            studio = "touchstone";
                                                            baseUrl = "http://movies.apple.com/movies/" + studio + "/" + ItemTitle + "/" + ItemTitle + quality + ".mov";
                                                            try
                                                            {
                                                                Helpers.UpdateProgress("Updating Films Section...", "Downloading trailer for " + ItemTitle + "...",  Item);
                                                                client.DownloadFile(baseUrl, @"D:\" + ItemTitle + ".mov");
                                                            }
                                                            catch (Exception)
                                                            {
                                                                studio = "universal";
                                                                baseUrl = "http://movies.apple.com/movies/" + studio + "/" + ItemTitle + "/" + ItemTitle + quality + ".mov";
                                                                try
                                                                {
                                                                    Helpers.UpdateProgress("Updating Films Section...", "Downloading trailer for " + ItemTitle + "...",  Item);
                                                                    client.DownloadFile(baseUrl, @"D:\" + ItemTitle + ".mov");
                                                                }
                                                                catch (Exception)
                                                                {
                                                                    studio = "disney";
                                                                    baseUrl = "http://movies.apple.com/movies/" + studio + "/" + ItemTitle + "/" + ItemTitle + quality + ".mov";
                                                                    try
                                                                    {
                                                                        Helpers.UpdateProgress("Updating Films Section...", "Downloading trailer for " + ItemTitle + "...",  Item);
                                                                        client.DownloadFile(baseUrl, @"D:\" + ItemTitle + ".mov");
                                                                    }
                                                                    catch (Exception)
                                                                    {
                                                                        return false;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            #endregion

            studio = "fox_searchlight";
            quality = "_h.640";
            baseUrl = "http://movies.apple.com/movies/" + studio + "/" + ItemTitle + "/" + ItemTitle + quality + ".mov";

            #region switch studios until match is found
            try
            {
                Helpers.UpdateProgress("Updating Films Section...", "Downloading trailer for " + ItemTitle + "...",  Item);
                client.DownloadFile(baseUrl, @"D:\" + ItemTitle + ".mov");
            }
            catch (Exception)
            {
                studio = "fox";
                baseUrl = "http://movies.apple.com/movies/" + studio + "/" + ItemTitle + "/" + ItemTitle + quality + ".mov";
                try
                {
                    Helpers.UpdateProgress("Updating Films Section...", "Downloading trailer for " + ItemTitle + "...",  Item);
                    client.DownloadFile(baseUrl, @"D:\" + ItemTitle + ".mov");
                }
                catch (Exception)
                {
                    studio = "dreamworks";
                    baseUrl = "http://movies.apple.com/movies/" + studio + "/" + ItemTitle + "/" + ItemTitle + quality + ".mov";
                    try
                    {
                        Helpers.UpdateProgress("Updating Films Section...", "Downloading trailer for " + ItemTitle + "...",  Item);
                        client.DownloadFile(baseUrl, @"D:\" + ItemTitle + ".mov");
                    }
                    catch (Exception)
                    {
                        studio = "focus_features";
                        baseUrl = "http://movies.apple.com/movies/" + studio + "/" + ItemTitle + "/" + ItemTitle + quality + ".mov";
                        try
                        {
                            Helpers.UpdateProgress("Updating Films Section...", "Downloading trailer for " + ItemTitle + "...",  Item);
                            client.DownloadFile(baseUrl, @"D:\" + ItemTitle + ".mov");
                        }
                        catch (Exception)
                        {
                            studio = "independent";
                            baseUrl = "http://movies.apple.com/movies/" + studio + "/" + ItemTitle + "/" + ItemTitle + quality + ".mov";
                            try
                            {
                                Helpers.UpdateProgress("Updating Films Section...", "Downloading trailer for " + ItemTitle + "...",  Item);
                                client.DownloadFile(baseUrl, @"D:\" + ItemTitle + ".mov");
                            }
                            catch (Exception)
                            {
                                studio = "lions_gate";
                                baseUrl = "http://movies.apple.com/movies/" + studio + "/" + ItemTitle + "/" + ItemTitle + quality + ".mov";
                                try
                                {
                                    Helpers.UpdateProgress("Updating Films Section...", "Downloading trailer for " + ItemTitle + "...",  Item);
                                    client.DownloadFile(baseUrl, @"D:\" + ItemTitle + ".mov");
                                }
                                catch (Exception)
                                {
                                    studio = "magnolia";
                                    baseUrl = "http://movies.apple.com/movies/" + studio + "/" + ItemTitle + "/" + ItemTitle + quality + ".mov";
                                    try
                                    {
                                        Helpers.UpdateProgress("Updating Films Section...", "Downloading trailer for " + ItemTitle + "...",  Item);
                                        client.DownloadFile(baseUrl, @"D:\" + ItemTitle + ".mov");
                                    }
                                    catch (Exception)
                                    {
                                        studio = "mgm";
                                        baseUrl = "http://movies.apple.com/movies/" + studio + "/" + ItemTitle + "/" + ItemTitle + quality + ".mov";
                                        try
                                        {
                                            Helpers.UpdateProgress("Updating Films Section...", "Downloading trailer for " + ItemTitle + "...",  Item);
                                            client.DownloadFile(baseUrl, @"D:\" + ItemTitle + ".mov");
                                        }
                                        catch (Exception)
                                        {
                                            studio = "miramax";
                                            baseUrl = "http://movies.apple.com/movies/" + studio + "/" + ItemTitle + "/" + ItemTitle + quality + ".mov";
                                            try
                                            {
                                                Helpers.UpdateProgress("Updating Films Section...", "Downloading trailer for " + ItemTitle + "...",  Item);
                                                client.DownloadFile(baseUrl, @"D:\" + ItemTitle + ".mov");
                                            }
                                            catch (Exception)
                                            {
                                                studio = "paramount";
                                                baseUrl = "http://movies.apple.com/movies/" + studio + "/" + ItemTitle + "/" + ItemTitle + quality + ".mov";
                                                try
                                                {
                                                    Helpers.UpdateProgress("Updating Films Section...", "Downloading trailer for " + ItemTitle + "...",  Item);
                                                    client.DownloadFile(baseUrl, @"D:\" + ItemTitle + ".mov");
                                                }
                                                catch (Exception)
                                                {
                                                    studio = "sony_pictures";
                                                    baseUrl = "http://movies.apple.com/movies/" + studio + "/" + ItemTitle + "/" + ItemTitle + quality + ".mov";
                                                    try
                                                    {
                                                        Helpers.UpdateProgress("Updating Films Section...", "Downloading trailer for " + ItemTitle + "...",  Item);
                                                        client.DownloadFile(baseUrl, @"D:\" + ItemTitle + ".mov");
                                                    }
                                                    catch (Exception)
                                                    {
                                                        studio = "sony";
                                                        baseUrl = "http://movies.apple.com/movies/" + studio + "/" + ItemTitle + "/" + ItemTitle + quality + ".mov";
                                                        try
                                                        {
                                                            Helpers.UpdateProgress("Updating Films Section...", "Downloading trailer for " + ItemTitle + "...",  Item);
                                                            client.DownloadFile(baseUrl, @"D:\" + ItemTitle + ".mov");
                                                        }
                                                        catch (Exception)
                                                        {
                                                            studio = "touchstone";
                                                            baseUrl = "http://movies.apple.com/movies/" + studio + "/" + ItemTitle + "/" + ItemTitle + quality + ".mov";
                                                            try
                                                            {
                                                                Helpers.UpdateProgress("Updating Films Section...", "Downloading trailer for " + ItemTitle + "...",  Item);
                                                                client.DownloadFile(baseUrl, @"D:\" + ItemTitle + ".mov");
                                                            }
                                                            catch (Exception)
                                                            {
                                                                studio = "universal";
                                                                baseUrl = "http://movies.apple.com/movies/" + studio + "/" + ItemTitle + "/" + ItemTitle + quality + ".mov";
                                                                try
                                                                {
                                                                    Helpers.UpdateProgress("Updating Films Section...", "Downloading trailer for " + ItemTitle + "...",  Item);
                                                                    client.DownloadFile(baseUrl, @"D:\" + ItemTitle + ".mov");
                                                                }
                                                                catch (Exception)
                                                                {
                                                                    studio = "disney";
                                                                    baseUrl = "http://movies.apple.com/movies/" + studio + "/" + ItemTitle + "/" + ItemTitle + quality + ".mov";
                                                                    try
                                                                    {
                                                                        Helpers.UpdateProgress("Updating Films Section...", "Downloading trailer for " + ItemTitle + "...",  Item);
                                                                        client.DownloadFile(baseUrl, @"D:\" + ItemTitle + ".mov");
                                                                    }
                                                                    catch (Exception)
                                                                    {
                                                                        return false;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            #endregion


            return true;
        }
        
