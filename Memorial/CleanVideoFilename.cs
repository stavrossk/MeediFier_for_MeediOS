            #region HD Resolution
            temp_index = filename.IndexOf("1080p", 0, filename.Length);
            if (temp_index > 0 && release_index == 0) release_index = temp_index;

            temp_index = filename.IndexOf("720p", 0, filename.Length);
            if (temp_index > 0 && release_index == 0) release_index = temp_index;
            #endregion
            
            
            
                        #region Unrated
            temp_index = filename.IndexOf("[unrated]", 0, filename.Length, StringComparison.OrdinalIgnoreCase);
            if (temp_index > 0 && release_index == 0) release_index = temp_index;

            temp_index = filename.IndexOf("(unrated)", 0, filename.Length, StringComparison.OrdinalIgnoreCase);
            if (temp_index > 0 && release_index == 0) release_index = temp_index;

            temp_index = filename.IndexOf("unrated", 0, filename.Length, StringComparison.OrdinalIgnoreCase);
            if (temp_index > 0 && release_index == 0) release_index = temp_index;
            #endregion
            
            
                        #region Disc Type

            temp_index = filename.IndexOf("DVDR", 0, filename.Length, StringComparison.OrdinalIgnoreCase);
            if (temp_index > 0 && release_index == 0) release_index = temp_index;

            temp_index = filename.IndexOf("DVD-R", 0, filename.Length, StringComparison.OrdinalIgnoreCase);
            if (temp_index > 0 && release_index == 0) release_index = temp_index;

            #endregion
            
            
            
            


            #region Language definitions
            temp_index = filename.IndexOf("[Audio Dual]", 0, filename.Length,StringComparison.OrdinalIgnoreCase);
            if (temp_index > 0 && release_index == 0) release_index = temp_index;

            temp_index = filename.IndexOf("[Eng-Spa]", 0, filename.Length,StringComparison.OrdinalIgnoreCase);
            if (temp_index > 0 && release_index == 0) release_index = temp_index;

            temp_index = filename.IndexOf("[Eng]", 0, filename.Length, StringComparison.OrdinalIgnoreCase);
            if (temp_index > 0 && release_index == 0) release_index = temp_index;
           
            temp_index = filename.IndexOf("(Eng)", 0, filename.Length, StringComparison.OrdinalIgnoreCase);
            if (temp_index > 0 && release_index == 0) release_index = temp_index;

            //temp_index = filename.IndexOf("ENGLISH", 0, filename.Length);
            //if (temp_index > 0 && release_index == 0) release_index = temp_index;

            //temp_index = filename.IndexOf("FRENCH", 0, filename.Length);
            //if (temp_index > 0 && release_index == 0) release_index = temp_index;

            temp_index = filename.IndexOf("SUBFRENCH", 0, filename.Length);
            if (temp_index > 0 && release_index == 0) release_index = temp_index;
            #endregion


            #region Release Type

            temp_index = filename.IndexOf("BRrip", 0, filename.Length, StringComparison.OrdinalIgnoreCase);
            if (temp_index > 0 && release_index == 0) release_index = temp_index;

            temp_index = filename.IndexOf("BDrip", 0, filename.Length, StringComparison.OrdinalIgnoreCase);
            if (temp_index > 0 && release_index == 0) release_index = temp_index;

            temp_index = filename.IndexOf("BR-rip", 0, filename.Length, StringComparison.OrdinalIgnoreCase);
            if (temp_index > 0 && release_index == 0) release_index = temp_index;

            temp_index = filename.IndexOf("HDRip", 0, filename.Length, StringComparison.OrdinalIgnoreCase);
            if (temp_index > 0 && release_index == 0) release_index = temp_index;

            temp_index = filename.IndexOf("dvd-rip", 0, filename.Length, StringComparison.OrdinalIgnoreCase);
            if (temp_index > 0 && release_index == 0) release_index = temp_index;

            temp_index = filename.IndexOf("[dvd-rip]", 0, filename.Length, StringComparison.OrdinalIgnoreCase);
            if (temp_index > 0 && release_index == 0) release_index = temp_index;

            temp_index = filename.IndexOf("(dvd-rip)", 0, filename.Length, StringComparison.OrdinalIgnoreCase);
            if (temp_index > 0 && release_index == 0) release_index = temp_index;

            temp_index = filename.IndexOf("dvdrip", 0, filename.Length, StringComparison.OrdinalIgnoreCase);
            if (temp_index > 0 && release_index == 0) release_index = temp_index;

            temp_index = filename.IndexOf("[dvdrip]", 0, filename.Length, StringComparison.OrdinalIgnoreCase);
            if (temp_index > 0 && release_index == 0) release_index = temp_index;

            temp_index = filename.IndexOf("(dvdrip)", 0, filename.Length, StringComparison.OrdinalIgnoreCase);
            if (temp_index > 0 && release_index == 0) release_index = temp_index;

            temp_index = filename.IndexOf("dvd rip", 0, filename.Length, StringComparison.OrdinalIgnoreCase);
            if (temp_index > 0 && release_index == 0) release_index = temp_index;

            temp_index = filename.IndexOf("R5", 0, filename.Length);
            if (temp_index > 0 && release_index == 0) release_index = temp_index;

            temp_index = filename.IndexOf("CAM", 0, filename.Length);
            if (temp_index > 0 && release_index == 0) release_index = temp_index;

            temp_index = filename.IndexOf("TS", 0, filename.Length);
            if (temp_index > 0 && release_index == 0) release_index = temp_index;

            temp_index = filename.IndexOf("telesync", 0, filename.Length, StringComparison.OrdinalIgnoreCase);
            if (temp_index > 0 && release_index == 0) release_index = temp_index;

            temp_index = filename.IndexOf("dvdscr", 0, filename.Length, StringComparison.OrdinalIgnoreCase);
            if (temp_index > 0 && release_index == 0) release_index = temp_index;

            temp_index = filename.IndexOf("screener", 0, filename.Length, StringComparison.OrdinalIgnoreCase);
            if (temp_index > 0 && release_index == 0) release_index = temp_index;

            temp_index = filename.IndexOf("HDTV", 0, filename.Length, StringComparison.OrdinalIgnoreCase);
            if (temp_index > 0 && release_index == 0) release_index = temp_index;

            temp_index = filename.IndexOf("PDTV", 0, filename.Length, StringComparison.OrdinalIgnoreCase);
            if (temp_index > 0 && release_index == 0) release_index = temp_index;


            temp_index = filename.IndexOf("WS", 0, filename.Length, StringComparison.Ordinal);
            if (temp_index > 0 && release_index == 0) release_index = temp_index;


            #endregion


            #region Video Type
            temp_index = filename.IndexOf("xvid", 0, filename.Length, StringComparison.OrdinalIgnoreCase);
            if (temp_index > 0 && release_index == 0) release_index = temp_index;

            temp_index = filename.IndexOf("divx", 0, filename.Length, StringComparison.OrdinalIgnoreCase);
            if (temp_index > 0 && release_index == 0) release_index = temp_index;

            temp_index = filename.IndexOf("bluray", 0, filename.Length, StringComparison.OrdinalIgnoreCase);
            if (temp_index > 0 && release_index == 0) release_index = temp_index;
            #endregion
            
            
            
            #region Group Name
            temp_index = filename.IndexOf("PROPER", 0, filename.Length);
            if (temp_index > 0 && release_index == 0) release_index = temp_index;

            temp_index = filename.IndexOf("KLAXXON", 0, filename.Length);
            if (temp_index > 0 && release_index == 0) release_index = temp_index;

            temp_index = filename.IndexOf("DEViSE", 0, filename.Length);
            if (temp_index > 0 && release_index == 0) release_index = temp_index;

            temp_index = filename.IndexOf("LiMiTED", 0, filename.Length);
            if (temp_index > 0 && release_index == 0) release_index = temp_index;

            temp_index = filename.IndexOf("FLAWL3SS", 0, filename.Length);
            if (temp_index > 0 && release_index == 0) release_index = temp_index;

            #endregion