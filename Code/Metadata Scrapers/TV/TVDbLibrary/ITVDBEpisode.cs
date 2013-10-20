// Type: TVDBLibrary.ITVDBEpisode
// Assembly: TVDBLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 34AA2317-12CB-4E3C-A53E-6904E324E0D4
// Assembly location: P:\Development\Assemblies\TVDBLibrary.dll

using System;

namespace TVDBLibrary
{
  public interface ITVDBEpisode
  {
    string ID { get; set; }

    string DVD_Chapter { get; set; }

    string DVD_Disc_ID { get; set; }

    Decimal DVD_Episode_Number { get; set; }

    string Director { get; set; }

    string EpisodeName { get; set; }

    int EpisodeNumber { get; set; }

    DateTime FirstAired { get; set; }

    string GuestStars { get; set; }

    string ImageFile { get; set; }

    string Language { get; set; }

    string Overview { get; set; }

    string ProductionCode { get; set; }

    Decimal Rating { get; set; }

    int SeasonNumber { get; set; }

    string Writer { get; set; }

    long LastUpdated { get; set; }

    string SeasonID { get; set; }

    string SeriesID { get; set; }
  }
}
