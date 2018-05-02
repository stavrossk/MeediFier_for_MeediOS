//#region detect [102] structure
               //Regex regex = new Regex(@"\d{3,3}");
               //MatchCollection matches = regex.Matches(Item.Name);


               //if (matches.Count > 0 )
               //{
               //    Debugger.LogMessageToFile("[SeasonNumber][EpisodeNumber] episode naming scheme detected.");

               //    #region Retrieve first from returned matches

               //    string EpIDnums = matches[0].Value;
               //    //MessageBox.Show("imdbid from nfo: " + imdbid);

               //    if (!String.IsNullOrEmpty(EpIDnums))
               //    {
               //       
               //        char FirstDigit = EpIDnums[0];

               //        int EpIdStart = Item.Name.IndexOf(EpIDnums);

               //        if (FirstDigit != '0')
               //        {
               //         
               //            if (!Char.IsDigit(Item.Name[EpIdStart - 1]))
               //            {
               //                //MessageBox.Show("Step 3");
               //                if (!Char.IsDigit(Item.Name[EpIdStart - 2]))
               //                {
               //                    //MessageBox.Show("Step 4");
               //                    Debugger.LogMessageToFile("Retrieved combined Season and Episode numbers " + EpIDnums);
               //                    string SeasonNumber = EpIDnums.Substring(0, 1);
               //                    string EpisodeNumber = EpIDnums.Substring(1, 2);

               //                    int Z_index = Item.Name.IndexOf(EpIDnums);
               //                    string leftpart = Item.Name.Substring(0, Z_index);
               //                    string SeriesName = leftpart.Trim();

               //                    Item.Tags["SeriesName"] = SeriesName;
               //                    Debugger.LogMessageToFile("Extracted Series Name: " + SeriesName);
               //                    Item.Tags["SeasonNumber"] = SeasonNumber;
               //                    Debugger.LogMessageToFile("Extracted Season Number: " + SeasonNumber);
               //                    Item.Tags["EpisodeNumber"] = EpisodeNumber;
               //                    Debugger.LogMessageToFile("Extracted Episode Number: " + EpisodeNumber);
               //                    Item.SaveTags();

               //                    MoveToTvSection(Section, TvSection, Item, RanFromTvSection, SeasonNumber, EpisodeNumber, SeriesName);

               //                    return;
               //                }
               //            }
               //        }
               //    }
               //    #endregion
               //}
               //#endregion