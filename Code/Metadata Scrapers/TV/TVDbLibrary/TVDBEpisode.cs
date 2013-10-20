// Type: TVDBLibrary.TVDBEpisode
// Assembly: TVDBLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 34AA2317-12CB-4E3C-A53E-6904E324E0D4
// Assembly location: P:\Development\Assemblies\TVDBLibrary.dll

using System;

namespace TVDBLibrary
{
  internal class TVDBEpisode : ITVDBEpisode
  {
    private string _id = "";
    private string _dvdChapter = "";
    private string _dvdDiscID = "";
    private string _director = "";
    private string _episodeName = "";
    private DateTime _firstAired = DateTime.MinValue;
    private string _guestStars = "";
    private string _imageFile = "";
    private string _language = "";
    private string _overview = "";
    private string _productionCode = "";
    private string _writer = "";
    private string _seasonID = "";
    private string _seriesID = "";
    private Decimal _dvdEpisodeNumber;
    private int _dvdSeason;
    private int _episodeNumber;
    private Decimal _rating;
    private int _seasonNumber;
    private long _lastUpdated;

    public string ID
    {
      get
      {
        return this._id;
      }
      set
      {
        this._id = value;
      }
    }

    public string DVD_Chapter
    {
      get
      {
        return this._dvdChapter;
      }
      set
      {
        this._dvdChapter = value;
      }
    }

    public string DVD_Disc_ID
    {
      get
      {
        return this._dvdDiscID;
      }
      set
      {
        this._dvdDiscID = value;
      }
    }

    public Decimal DVD_Episode_Number
    {
      get
      {
        return this._dvdEpisodeNumber;
      }
      set
      {
        this._dvdEpisodeNumber = value;
      }
    }

    public int DVD_Season_Number
    {
      get
      {
        return this._dvdSeason;
      }
      set
      {
        this._dvdSeason = value;
      }
    }

    public string Director
    {
      get
      {
        return this._director;
      }
      set
      {
        this._director = value;
      }
    }

    public string EpisodeName
    {
      get
      {
        return this._episodeName;
      }
      set
      {
        this._episodeName = value;
      }
    }

    public int EpisodeNumber
    {
      get
      {
        return this._episodeNumber;
      }
      set
      {
        this._episodeNumber = value;
      }
    }

    public DateTime FirstAired
    {
      get
      {
        return this._firstAired;
      }
      set
      {
        this._firstAired = value;
      }
    }

    public string GuestStars
    {
      get
      {
        return this._guestStars;
      }
      set
      {
        this._guestStars = value;
      }
    }

    public string ImageFile
    {
      get
      {
        return this._imageFile;
      }
      set
      {
        this._imageFile = value;
      }
    }

    public string Language
    {
      get
      {
        return this._language;
      }
      set
      {
        this._language = value;
      }
    }

    public string Overview
    {
      get
      {
        return this._overview;
      }
      set
      {
        this._overview = value;
      }
    }

    public string ProductionCode
    {
      get
      {
        return this._productionCode;
      }
      set
      {
        this._productionCode = value;
      }
    }

    public Decimal Rating
    {
      get
      {
        return this._rating;
      }
      set
      {
        this._rating = value;
      }
    }

    public int SeasonNumber
    {
      get
      {
        return this._seasonNumber;
      }
      set
      {
        this._seasonNumber = value;
      }
    }

    public string Writer
    {
      get
      {
        return this._writer;
      }
      set
      {
        this._writer = value;
      }
    }

    public long LastUpdated
    {
      get
      {
        return this._lastUpdated;
      }
      set
      {
        this._lastUpdated = value;
      }
    }

    public string SeasonID
    {
      get
      {
        return this._seasonID;
      }
      set
      {
        this._seasonID = value;
      }
    }

    public string SeriesID
    {
      get
      {
        return this._seriesID;
      }
      set
      {
        this._seriesID = value;
      }
    }
  }
}
