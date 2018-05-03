using CustomProperties.PropertyData;
using MeediOS.Cache;




namespace MediaFairy.Code.Importer_Properties
{




    class FilmReleasesUpdaterProperties
    {







        internal static bool GetFilmReleasesUpdaterProperties
            (ImporterProperties importerProperties,
             int index, OptionSettings prop, ref int counter)
        {



            if (index == counter++)
            {
                prop.GroupName = "RssUpdater";
                //set the internal name
                prop.Name = "EnableFilmReleasesProp";
                //set name shown to user
                prop.GroupCaption = "                        RSS Releases Updater";
                prop.Caption = "Download film releases via BitTorrent";
                prop.Caption = TranslationProvider.Translate(prop.Caption, importerProperties);

                //set the tool tip
                prop.HelpText =
                    "The Film Releases Updater will scan for new film releases the rss feeds you provide in the RSS Sources.txt file (in the plugin's direcotry), \n  and download those films using your default BitTorrent client. Adittional filters can be provided like the film's production year or IMDb rating.  ";
                prop.DefaultValue = Settings.DownloadFilmReleases;
                prop.DataType = "bool";
                prop.Dependencies = "False:HIDE FilmRatingMinProp,TorrentsFolderProp" +
                                    ";True:SHOW FilmRatingMinProp,TorrentsFolderProp";
                return true;
            }

            if (index == counter++)
            {
                prop.GroupName = "RssUpdater";
                //set the internal name
                prop.Name = "TorrentsFolderProp";
                //set name shown to user
                prop.Caption = "Store torrent files in: ";
                prop.Caption = TranslationProvider.Translate(prop.Caption, importerProperties);
                //set the tool tip
                prop.HelpText = @"Please specify the folder where downloaded torrent files should be stored. ";
                prop.DefaultValue = Settings.TorrentsFolder;
                prop.DataType = "folder";
                prop.CanTypeChoices = false;
                return true;
            }

            //if (Index == counter++)
            //{
            //    //set the internal name
            //    Prop.Name = "FilmReleasesRssProp";
            //    //set name shown to user
            //    Prop.Caption = " RSS feeds for films releases: ";
            //    //set the tool tip
            //    Prop.HelpText = "Please provide here the rss feed URLs which will be scanned for new film releases. Important Notice: Please make sure that the sources you provide contain public domain media, in order to comply with copyright regulations.  ";
            //    Prop.DefaultValue = Importer.FilmsRssFeeds;
            //    Prop.DataType = "stringlist";
            //    return true;
            //}


            if (index == counter++)
            {
                prop.GroupName = "RssUpdater";
                //set the internal name
                prop.Name = "FilmRatingMinProp";
                prop.Caption = "Minimum film IMDb rating: ";
                prop.Caption = TranslationProvider.Translate(prop.Caption, importerProperties);
                //set the tool tip
                prop.HelpText =
                    "This value represents the minimum IMDb rating that a film must have, in order to be downloaded. Set this value to zero (0) to disable the rating filter.";
                prop.DefaultValue = Settings.FilmRatingMin2;
                prop.DataType = "string";

                return true;
            }

            return false;
        }
    }
}
