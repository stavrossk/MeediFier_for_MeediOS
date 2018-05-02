
        //Used by the film releases downloader
        public static void ExtractMetadataFromFilename(IMLItem Item, ref string imdbid)
        {
            if (!Settings.ExtractMetadataFromFilenames)
                return;

            if (String.IsNullOrEmpty(Item.Name))
                return;

            try
            {
                //Importer.thisProgress.Progress(Importer.CurrentProgress, "Performing meta-information extraction...");
                Helpers.UpdateProgress("Performing diagnostic operations", "Performing meta-information extraction...", Item);
                Debugger.LogMessageToFile("Performing meta-information extraction...");

            

                #region extract imdbid from filename

                if (Item.Name.Contains("tt"))
                {
                    //MessageBox.Show("Step 3");

                    #region if imdb index is found...
                    int imdbid_index = Item.Name.IndexOf("tt");

                    if ((Item.Name.Length - imdbid_index) > 2)
                    {

                        //MessageBox.Show("Step 4");
                        Char firstDigit = Item.Name[imdbid_index + 2];
                        if (imdbid_index >= 0 && Char.IsNumber(firstDigit))
                        {
                            //MessageBox.Show("Step 5");
                            int imdb_index_start = imdbid_index;
                            int imdb_index_end = 0;
                            imdbid_index = imdbid_index + 2;
                            string substring = Item.Name.Substring(imdbid_index, Item.Name.Length - imdbid_index - 1);


                            for (int i = imdbid_index; i <= Item.Name.Length; i++)
                            {

                                if (Char.IsNumber(Item.Name[i]))
                                {
                                    imdb_index_end = i;
                                }
                                else break;

                            }

                            //MessageBox.Show("Substring: " + substring); 
                            //MessageBox.Show( Convert.ToString("Index start: " + imdb_index_start) );
                            //MessageBox.Show(Convert.ToString("Index end: " + imdb_index_end));
                            int imdbid_length = imdb_index_end - imdbid_index + 1;
                            //MessageBox.Show("IMDbID length: " + imdbid_length);
                            //MessageBox.Show(Convert.ToString("start index: " + imdb_index_start));
                            imdbid = Item.Name.Substring(imdbid_index, imdbid_length);
                            imdbid = "tt" + imdbid;
                            //MessageBox.Show("imdbid after extracting from filename: "+ imdbid);
                            //imdbOP._imdbID = imdbid;
                            Item.Tags["ImdbID"] = imdbid;
                            string left_namepart = Item.Name.Substring(0, imdb_index_start);
                            //MessageBox.Show("left part: " + left_namepart);
                            string right_namepart = Item.Name.Substring(imdb_index_end + 1, Item.Name.Length - imdb_index_end - 1);
                            //MessageBox.Show("Right part: " + right_namepart);
                            Item.Name = left_namepart + right_namepart;
                            Item.SaveTags();

                        }

                    }
                    #endregion

                    Debugger.LogMessageToFile("ImdbID " + imdbid + " was extracted from item's name...");
                }


                #endregion



                

                //TODO: Only one filter filename pass should be used!

                Item.Name = FilterReleaseFilename(Item.Name).Trim();
                Item.Name = FilterReleaseFilename(Item.Name).Trim();
                Item.Name = FilterReleaseFilename(Item.Name).Trim();
                Item.SaveTags();

                //MessageBox.Show("Step 4");

                #region extract Year from filename

                int year_index = 0;
                int temp_index = 0;

                #region First Try: Year in parenthesis
                temp_index = Item.Name.IndexOf("[19");
                if (temp_index >= 0) year_index = temp_index + 1;

                temp_index = Item.Name.IndexOf("(19");
                if (temp_index >= 0) year_index = temp_index + 1;

                temp_index = Item.Name.IndexOf(".19");
                if (temp_index >= 0) year_index = temp_index + 1;

                temp_index = Item.Name.IndexOf("[20");
                if (temp_index >= 0) year_index = temp_index + 1;

                temp_index = Item.Name.IndexOf("(20");
                if (temp_index >= 0) year_index = temp_index + 1;

                temp_index = Item.Name.IndexOf(".20");
                if (temp_index >= 0) year_index = temp_index + 1;
                #endregion

                #region Second Try: Year without parenthesis
                if (year_index == 0)
                {
                    int LastChar = Item.Name.Length - 1;
                    int FirstYearChar = LastChar - 3;
                    string NumberWord = Item.Name.Substring(FirstYearChar, 4);

                    if (!NumberWord.Contains("x") && !NumberWord.Contains("E") && !NumberWord.Contains("S")) //the number extracted must Not belong to episode numbers
                    {
                        if (Item.Name.Length >= 8)
                        {
                            if (Char.IsDigit(Item.Name[LastChar]))
                            {
                                if (Char.IsDigit(Item.Name[FirstYearChar]))
                                {
                                    #region Second Try: Year without parenthesis
                                    temp_index = Item.Name.IndexOf("19");
                                    if (temp_index >= 0) year_index = temp_index;

                                    temp_index = Item.Name.IndexOf("20");
                                    if (temp_index >= 0) year_index = temp_index;
                                    #endregion
                                }
                            }
                        }
                    }
                }
                #endregion

                #region Extract Year and store it in item's tag
                if (year_index > 0)
                {
                    //MessageBox.Show(Convert.ToString("year compariosn result: " + year_index));
                    string year = Item.Name.Substring(year_index, 4);
                    //MessageBox.Show("Year: " + year);
                    Item.Tags["Year"] = year;
                    Item.Name = Item.Name.Trim();

                    Char YearPrevChar = Item.Name[year_index - 1];
                    if (YearPrevChar == '(' || YearPrevChar == '[' || YearPrevChar == '.')
                        year_index = year_index - 1;

                    Item.Name = Item.Name.Substring(0, year_index);
                    Item.Name = Item.Name.Trim();
                    Item.SaveTags();

                    Debugger.LogMessageToFile("The film's Year (" + year + ") was extracted from item's name.");
                }

                #endregion

                #endregion

                //MessageBox.Show("Step 5");




                //RecognizeSeries(TvSection, Section, RanFromTvSection, Item, location);




            }
            catch (Exception e)
            {
                StatusForm.statusForm.TrayIcon.ShowBalloonTip(5000, "Error parsing filename", "An unexpected error ocurred in the filename parser. " + Environment.NewLine +
                                                                                              "Please see the Debug.log file for details.", ToolTipIcon.Warning);
                Debugger.LogMessageToFile("An unexpected error occured in the filename parsing method. The error was: " + e.ToString());
            }

            return;
        }
