using System;
using MeediOS;
using TagLib;

namespace MediaFairy.SingleItemUpdaters
{
    class TagLibAudioTagPopulator
    {
        internal static void PopulateAudioTags(string album, string title, File audiofile, string artist, IMLItem item)
        {

            if (String.IsNullOrEmpty(artist) || Settings.OverwriteExistingMusicTags)
                item.Tags["Artist"] = audiofile.Tag.FirstAlbumArtist;

            if (String.IsNullOrEmpty(album) || Settings.OverwriteExistingMusicTags)
                item.Tags["Album"] = audiofile.Tag.Album;

            if (String.IsNullOrEmpty(title) || Settings.OverwriteExistingMusicTags)
                item.Tags["Title"] = audiofile.Tag.Title;

            if (!String.IsNullOrEmpty(audiofile.Tag.Title))
                item.Name = audiofile.Tag.Title;


            string genre = Helpers.GetTagValueFromItem(item, "Genre");
            if (String.IsNullOrEmpty(genre) || Settings.OverwriteExistingMusicTags)
                item.Tags["Genre"] = audiofile.Tag.JoinedGenres;


            string track = Helpers.GetTagValueFromItem(item, "Track");
            if (String.IsNullOrEmpty(track) || Settings.OverwriteExistingMusicTags)
            {
                if (audiofile.Tag.Track != 0)
                    item.Tags["Track"] = audiofile.Tag.Track;
            }


            string year = Helpers.GetTagValueFromItem(item, "Year");
            if (String.IsNullOrEmpty(year) || Settings.OverwriteExistingMusicTags)
            {
                if (audiofile.Tag.Year != 0)
                    item.Tags["Year"] = audiofile.Tag.Year;
            }


            string lyrics = Helpers.GetTagValueFromItem(item, "Lyrics");
            if (String.IsNullOrEmpty(lyrics) || Settings.OverwriteExistingMusicTags)
            {
                if (!String.IsNullOrEmpty(audiofile.Tag.Lyrics))
                    item.Tags["Lyrics"] = audiofile.Tag.Lyrics;
            }


            string beatsPerMinute = Helpers.GetTagValueFromItem(item, "BeatsPerMinute");
            if (String.IsNullOrEmpty(beatsPerMinute) ||
                Settings.OverwriteExistingMusicTags)
            {
                if (audiofile.Tag.BeatsPerMinute != 0)
                    item.Tags["BeatsPerMinute"] = audiofile.Tag.BeatsPerMinute;
            }


            string comment = Helpers.GetTagValueFromItem(item, "Comment");
            if (String.IsNullOrEmpty(comment) || Settings.OverwriteExistingMusicTags)
            {
                if (!String.IsNullOrEmpty(audiofile.Tag.Comment))
                    item.Tags["Comment"] = audiofile.Tag.Comment;
            }


            string copyright = Helpers.GetTagValueFromItem(item, "Copyright");
            if (String.IsNullOrEmpty(copyright) || Settings.OverwriteExistingMusicTags)
            {
                if (!String.IsNullOrEmpty(audiofile.Tag.Copyright))
                    item.Tags["Copyright"] = audiofile.Tag.Copyright;
            }


            string disc = Helpers.GetTagValueFromItem(item, "Disc");
            if (String.IsNullOrEmpty(disc) || Settings.OverwriteExistingMusicTags)
            {
                if (audiofile.Tag.Disc != 0)
                    item.Tags["Disc"] = audiofile.Tag.Disc;
            }


            string discCount = Helpers.GetTagValueFromItem(item, "DiscCount");
            if (String.IsNullOrEmpty(discCount) || Settings.OverwriteExistingMusicTags)
            {
                if (audiofile.Tag.DiscCount != 0)
                    item.Tags["DiscCount"] = audiofile.Tag.DiscCount;
            }


            string composer = Helpers.GetTagValueFromItem(item, "Composer");
            if (String.IsNullOrEmpty(composer) || Settings.OverwriteExistingMusicTags)
            {
                if (!String.IsNullOrEmpty(audiofile.Tag.FirstComposer))
                    item.Tags["Composer"] = audiofile.Tag.FirstComposer;
            }


            string performer = Helpers.GetTagValueFromItem(item, "Performer");
            if (String.IsNullOrEmpty(performer) || Settings.OverwriteExistingMusicTags)
            {
                if (!String.IsNullOrEmpty(audiofile.Tag.FirstPerformer))
                    item.Tags["Performer"] = audiofile.Tag.FirstPerformer;
            }


            string albumArtists = Helpers.GetTagValueFromItem(item, "AlbumArtists");

            if (String.IsNullOrEmpty(albumArtists) ||
                Settings.OverwriteExistingMusicTags)
            {
                if (!String.IsNullOrEmpty(audiofile.Tag.JoinedAlbumArtists))
                    item.Tags["AlbumArtists"] = audiofile.Tag.JoinedAlbumArtists;
            }


            item.SaveTags();

        }

        internal static bool PerformTagLibAudioTagExtraction(string title, string artist, string parentDir, string album,
                                                            IMLItem item, string location)
        {
            bool trackHasCover = false;

            Helpers.UpdateProgress("Updating Music Section",
                                   "Importing information from track '" + item.Name + "'...", item);


            File audiofile = File.Create(location);

            if (Settings.WantAudioTags)
            {
                Debugger.LogMessageToFile("Importing information from track '"
                                          + item.Name + "'...");

                Helpers.UpdateProgress("Updating Music Section",
                                       "Importing information from track '" + item.Name +
                                       "'...", item);

                PopulateAudioTags(album, title, audiofile, artist, item);


                trackHasCover = MusicTrackItemUpdaterHelpers.ImportAlbumCoverArt(parentDir, item, audiofile);
            }

            return trackHasCover;
        }
    }
}
