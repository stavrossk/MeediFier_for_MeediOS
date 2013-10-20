// Type: TVDBLibrary.TVDBLib
// Assembly: TVDBLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 34AA2317-12CB-4E3C-A53E-6904E324E0D4
// Assembly location: P:\Development\Assemblies\TVDBLibrary.dll

using System;
using System.Collections.Generic;

namespace TVDBLibrary
{
  public class TVDBLib
  {
    private string _langAbbrev = "en";
    private TVDBDatabase _db;

    public string LanguageAbbreviation
    {
      get
      {
        return this._langAbbrev;
      }
      set
      {
        this._langAbbrev = value;
      }
    }

    public long ServerTime
    {
      get
      {
        return this._db.ServerTime;
      }
    }

    public TVDBLib()
    {
      this._db = new TVDBDatabase();
    }

    public IList<ITVDBLanguage> GetLanguages()
    {
      try
      {
        return this._db.GetLanguages();
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    public ITVDBSeries GetSeries(string seriesID)
    {
      try
      {
        return this._db.GetSeries(seriesID, this._langAbbrev);
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    public IList<ITVDBSeries> SearchSeries(string seriesName)
    {
      try
      {
        return this._db.SearchSeries(seriesName, this._langAbbrev);
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    public ITVDBEpisode GetEpisode(string episodeID)
    {
      try
      {
        return this._db.GetEpisode(episodeID, this._langAbbrev);
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    public ITVDBEpisode GetEpisode(string seriesID, int seasonNum, int episodeNum, TVDBLib.EpisodeOrderEnum order)
    {
      try
      {
        return this._db.GetEpisode(seriesID, this._langAbbrev, seasonNum, episodeNum, order);
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    public IList<ITVDBEpisode> GetAllEpisodes(string seriesID)
    {
      try
      {
        return this._db.GetAllEpisodes(seriesID, this._langAbbrev);
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    public IList<ITVDBBanner> GetSeriesImagesAll(string seriesID)
    {
      try
      {
        return this._db.GetSeriesImagesAll(seriesID);
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    public IList<ITVDBBanner> GetSeasonImagesAll(string seriesID, int seasonNum)
    {
      try
      {
        return this._db.GetSeasonImagesAll(seriesID, seasonNum);
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    public IList<ITVDBBanner> GetSeasonImagesPoster(string seriesID, int seasonNum)
    {
      try
      {
        return this._db.GetSeasonImagesPoster(seriesID, seasonNum);
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    public IList<ITVDBBanner> GetSeasonImagesBanner(string seriesID, int seasonNum)
    {
      try
      {
        return this._db.GetSeasonImagesBanner(seriesID, seasonNum);
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    public IList<ITVDBBanner> GetSeriesFanart(string seriesID)
    {
      try
      {
        return this._db.GetSeriesFanart(seriesID);
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    public IList<ITVDBBanner> GetSeriesBanners(string seriesID)
    {
      try
      {
        return this._db.GetSeriesBanners(seriesID);
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    public IList<ITVDBUpdate> GetUpdates(TVDBLib.TimeConstraintEnum timeConstraint)
    {
      try
      {
        return this._db.GetUpdates(timeConstraint);
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    public string GetBannerURL()
    {
      return this._db.GetBannerURL();
    }

    public enum EpisodeOrderEnum
    {
      Default,
      DVD,
      Absolute,
    }

    public enum UpdateTypeEnum
    {
      Series,
      Episode,
      Banner,
    }

    public enum TimeConstraintEnum
    {
      Day,
      Week,
      Month,
    }
  }
}
