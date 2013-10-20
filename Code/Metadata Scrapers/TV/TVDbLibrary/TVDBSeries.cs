// Type: TVDBLibrary.TVDBSeries
// Assembly: TVDBLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 34AA2317-12CB-4E3C-A53E-6904E324E0D4
// Assembly location: P:\Development\Assemblies\TVDBLibrary.dll

using System;

namespace TVDBLibrary
{
  internal class TVDBSeries : ITVDBSeries
  {
    private string _id = "";
    private string _imdbID = "";
    private string _zap2itID = "";
    private string _actors = "";
    private string _airsDay = "";
    private string _airsTime = "";
    private DateTime _firstAired = DateTime.MinValue;
    private string _genre = "";
    private string _language = "";
    private string _network = "";
    private string _overview = "";
    private string _runtime = "";
    private string _seriesName = "";
    private string _status = "";
    private Decimal _rating;
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

    public string IMDb_ID
    {
      get
      {
        return this._imdbID;
      }
      set
      {
        this._imdbID = value;
      }
    }

    public string Zap2It_ID
    {
      get
      {
        return this._zap2itID;
      }
      set
      {
        this._zap2itID = value;
      }
    }

    public string Actors
    {
      get
      {
        return this._actors;
      }
      set
      {
        this._actors = value;
      }
    }

    public string Airs_DayOfWeek
    {
      get
      {
        return this._airsDay;
      }
      set
      {
        this._airsDay = value;
      }
    }

    public string Airs_TimeOfDay
    {
      get
      {
        return this._airsTime;
      }
      set
      {
        this._airsTime = value;
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

    public string Genre
    {
      get
      {
        return this._genre;
      }
      set
      {
        this._genre = value;
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

    public string Network
    {
      get
      {
        return this._network;
      }
      set
      {
        this._network = value;
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

    public string Runtime
    {
      get
      {
        return this._runtime;
      }
      set
      {
        this._runtime = value;
      }
    }

    public string SeriesName
    {
      get
      {
        return this._seriesName;
      }
      set
      {
        this._seriesName = value;
      }
    }

    public string Status
    {
      get
      {
        return this._status;
      }
      set
      {
        this._status = value;
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

    public string SortName
    {
      get
      {
        string str = this._seriesName;
        if (str.ToLower().Trim().StartsWith("the "))
          str = str.Trim().Substring(3).Trim();
        if (str.ToLower().Trim().StartsWith("a "))
          str = str.Trim().Substring(1).Trim();
        if (str.ToLower().Trim().StartsWith("an "))
          str = str.Trim().Substring(2).Trim();
        return str;
      }
    }
  }
}
