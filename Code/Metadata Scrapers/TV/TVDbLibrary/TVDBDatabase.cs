// Type: TVDBLibrary.TVDBDatabase
// Assembly: TVDBLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 34AA2317-12CB-4E3C-A53E-6904E324E0D4
// Assembly location: P:\Development\Assemblies\TVDBLibrary.dll

using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Xml;

namespace TVDBLibrary
{
  internal class TVDBDatabase
  {
    private const string _url = "http://thetvdb.com/";
    private const string _apiKey = "C315A8864A6B7866";
    private long _serverTime;
    private List<string> _xmlMirrors;
    private List<string> _bannerMirrors;
    private List<string> _zipMirrors;

    public long ServerTime
    {
      get
      {
        return this._serverTime;
      }
    }

    public TVDBDatabase()
    {
      this.GetCurrentServerTime();
      this.GetMirrors();
    }

    public IList<ITVDBLanguage> GetLanguages()
    {
      try
      {
        List<ITVDBLanguage> list = new List<ITVDBLanguage>();
        string filename = this.GetXMLURL() + "C315A8864A6B7866/languages.xml";
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.Load(filename);
        XmlNodeList xmlNodeList = xmlDocument.SelectNodes("/Languages/Language");
        if (xmlNodeList != null && xmlNodeList.Count > 0)
        {
          foreach (XmlNode xmlNode1 in xmlNodeList)
          {
            TVDBLanguage tvdbLanguage = new TVDBLanguage();
            foreach (XmlNode xmlNode2 in xmlNode1.ChildNodes)
            {
              if (xmlNode2.InnerText != null && xmlNode2.InnerText.Trim() != "")
              {
                if (xmlNode2.Name.ToLower().Trim() == "name")
                  tvdbLanguage.Name = xmlNode2.InnerText.Trim();
                else if (xmlNode2.Name.ToLower().Trim() == "abbreviation")
                  tvdbLanguage.Abbreviation = xmlNode2.InnerText.Trim();
                else if (xmlNode2.Name.ToLower().Trim() == "id")
                  tvdbLanguage.ID = xmlNode2.InnerText.Trim();
              }
            }
            if (tvdbLanguage.Name.Trim() != "" && tvdbLanguage.Abbreviation.Trim() != "")
              list.Add((ITVDBLanguage) tvdbLanguage);
          }
        }
        return (IList<ITVDBLanguage>) list;
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    public IList<ITVDBSeries> SearchSeries(string seriesName, string languageAbrev)
    {
      try
      {
        List<ITVDBSeries> list = new List<ITVDBSeries>();
        string filename = this.GetXMLURL() + "GetSeries.php?seriesname=" + HttpUtility.UrlEncode(Utils.SafeSQL(Utils.StripInvalidCharacters(seriesName)));
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.Load(filename);
        XmlNodeList xmlNodeList = xmlDocument.SelectNodes("/Data/Series");
        if (xmlNodeList != null)
        {
          foreach (XmlNode node in xmlNodeList)
          {
            TVDBSeries seriesData = this.GetSeriesData(node);
            if (seriesData.ID.Trim() != "" && seriesData.SeriesName.Trim() != "")
              list.Add((ITVDBSeries) seriesData);
          }
        }
        return (IList<ITVDBSeries>) list;
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    public ITVDBSeries GetSeries(string seriesID, string langAbbrev)
    {
      try
      {
        TVDBSeries tvdbSeries = (TVDBSeries) null;
        string xmlurl = this.GetXMLURL();
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(xmlurl);
        stringBuilder.Append("C315A8864A6B7866");
        stringBuilder.Append("/series/");
        stringBuilder.Append(seriesID);
        stringBuilder.Append("/");
        stringBuilder.Append(langAbbrev);
        stringBuilder.Append(".xml");
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.Load(((object) stringBuilder).ToString());
        XmlNodeList xmlNodeList = xmlDocument.SelectNodes("/Data/Series");
        if (xmlNodeList != null && xmlNodeList.Count > 0)
          tvdbSeries = this.GetSeriesData(xmlNodeList[0]);
        return (ITVDBSeries) tvdbSeries;
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    public ITVDBEpisode GetEpisode(string episodeID, string langAbbrev)
    {
      try
      {
        TVDBEpisode tvdbEpisode = (TVDBEpisode) null;
        string xmlurl = this.GetXMLURL();
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(xmlurl);
        stringBuilder.Append("C315A8864A6B7866");
        stringBuilder.Append("/episodes/");
        stringBuilder.Append(episodeID);
        stringBuilder.Append("/");
        stringBuilder.Append(langAbbrev);
        stringBuilder.Append(".xml");
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.Load(((object) stringBuilder).ToString());
        XmlNodeList xmlNodeList = xmlDocument.SelectNodes("/Data/Episode");
        if (xmlNodeList != null && xmlNodeList.Count > 0)
          tvdbEpisode = this.GetEpisodeData(xmlNodeList[0]);
        return (ITVDBEpisode) tvdbEpisode;
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    public IList<ITVDBEpisode> GetAllEpisodes(string seriesID, string langAbbrev)
    {
      try
      {
        List<ITVDBEpisode> list = new List<ITVDBEpisode>();
        string xmlurl = this.GetXMLURL();
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(xmlurl);
        stringBuilder.Append("C315A8864A6B7866");
        stringBuilder.Append("/series/");
        stringBuilder.Append(seriesID);
        stringBuilder.Append("/all/");
        stringBuilder.Append(langAbbrev);
        stringBuilder.Append(".xml");
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.Load(((object) stringBuilder).ToString());
        XmlNodeList xmlNodeList = xmlDocument.SelectNodes("/Data/Episode");
        if (xmlNodeList != null)
        {
          foreach (XmlNode node in xmlNodeList)
          {
            TVDBEpisode episodeData = this.GetEpisodeData(node);
            if (episodeData != null)
              list.Add((ITVDBEpisode) episodeData);
          }
        }
        return (IList<ITVDBEpisode>) list;
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    public ITVDBEpisode GetEpisode(string seriesID, string langAbbrev, int seasonNum, int episodeNum, TVDBLib.EpisodeOrderEnum order)
    {
      try
      {
        TVDBEpisode tvdbEpisode = (TVDBEpisode) null;
        string xmlurl = this.GetXMLURL();
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(xmlurl);
        stringBuilder.Append("C315A8864A6B7866");
        stringBuilder.Append("/series/");
        stringBuilder.Append(seriesID);
        if (order == TVDBLib.EpisodeOrderEnum.Absolute)
        {
          stringBuilder.Append("/absolute/");
          stringBuilder.Append(episodeNum.ToString());
        }
        else if (order == TVDBLib.EpisodeOrderEnum.DVD)
        {
          stringBuilder.Append("/dvd/");
          stringBuilder.Append(seasonNum.ToString());
          stringBuilder.Append("/");
          stringBuilder.Append(episodeNum.ToString());
        }
        else
        {
          stringBuilder.Append("/default/");
          stringBuilder.Append(seasonNum.ToString());
          stringBuilder.Append("/");
          stringBuilder.Append(episodeNum.ToString());
        }
        stringBuilder.Append("/");
        stringBuilder.Append(langAbbrev);
        stringBuilder.Append(".xml");
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.Load(((object) stringBuilder).ToString());
        XmlNodeList xmlNodeList = xmlDocument.SelectNodes("/Data/Episode");
        if (xmlNodeList != null && xmlNodeList.Count > 0)
          tvdbEpisode = this.GetEpisodeData(xmlNodeList[0]);
        return (ITVDBEpisode) tvdbEpisode;
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
        List<ITVDBBanner> list = new List<ITVDBBanner>();
        string xmlurl = this.GetXMLURL();
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(xmlurl);
        stringBuilder.Append("C315A8864A6B7866");
        stringBuilder.Append("/series/");
        stringBuilder.Append(seriesID);
        stringBuilder.Append("/banners.xml");
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.Load(((object) stringBuilder).ToString());
        XmlNodeList xmlNodeList = xmlDocument.SelectNodes("/Banners/Banner");
        if (xmlNodeList != null)
        {
          foreach (XmlNode node in xmlNodeList)
          {
            TVDBBanner bannerData = this.GetBannerData(node);
            if (bannerData.Path.Trim() != "")
              list.Add((ITVDBBanner) bannerData);
          }
        }
        return (IList<ITVDBBanner>) list;
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
        List<ITVDBBanner> list = new List<ITVDBBanner>();
        string xmlurl = this.GetXMLURL();
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(xmlurl);
        stringBuilder.Append("C315A8864A6B7866");
        stringBuilder.Append("/series/");
        stringBuilder.Append(seriesID);
        stringBuilder.Append("/banners.xml");
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.Load(((object) stringBuilder).ToString());
        XmlNodeList xmlNodeList = xmlDocument.SelectNodes("/Banners/Banner");
        if (xmlNodeList != null)
        {
          foreach (XmlNode node in xmlNodeList)
          {
            TVDBBanner bannerData = this.GetBannerData(node);
            if (bannerData.Path.Trim() != "" && bannerData.Type1.ToLower().Trim() == "season")
            {
              if (seasonNum == 0)
                list.Add((ITVDBBanner) bannerData);
              else if (bannerData.Season == seasonNum)
                list.Add((ITVDBBanner) bannerData);
            }
          }
        }
        return (IList<ITVDBBanner>) list;
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
        List<ITVDBBanner> list = new List<ITVDBBanner>();
        string xmlurl = this.GetXMLURL();
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(xmlurl);
        stringBuilder.Append("C315A8864A6B7866");
        stringBuilder.Append("/series/");
        stringBuilder.Append(seriesID);
        stringBuilder.Append("/banners.xml");
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.Load(((object) stringBuilder).ToString());
        XmlNodeList xmlNodeList = xmlDocument.SelectNodes("/Banners/Banner");
        if (xmlNodeList != null)
        {
          foreach (XmlNode node in xmlNodeList)
          {
            TVDBBanner bannerData = this.GetBannerData(node);
            if (bannerData.Path.Trim() != "" && bannerData.Type1.ToLower().Trim() == "season" && bannerData.Type2.ToLower().Trim() == "season")
            {
              if (seasonNum == 0)
                list.Add((ITVDBBanner) bannerData);
              else if (bannerData.Season == seasonNum)
                list.Add((ITVDBBanner) bannerData);
            }
          }
        }
        return (IList<ITVDBBanner>) list;
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
        List<ITVDBBanner> list = new List<ITVDBBanner>();
        string xmlurl = this.GetXMLURL();
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(xmlurl);
        stringBuilder.Append("C315A8864A6B7866");
        stringBuilder.Append("/series/");
        stringBuilder.Append(seriesID);
        stringBuilder.Append("/banners.xml");
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.Load(((object) stringBuilder).ToString());
        XmlNodeList xmlNodeList = xmlDocument.SelectNodes("/Banners/Banner");
        if (xmlNodeList != null)
        {
          foreach (XmlNode node in xmlNodeList)
          {
            TVDBBanner bannerData = this.GetBannerData(node);
            if (bannerData.Path.Trim() != "" && bannerData.Type1.ToLower().Trim() == "season" && bannerData.Type2.ToLower().Trim() == "seasonwide")
            {
              if (seasonNum == 0)
                list.Add((ITVDBBanner) bannerData);
              else if (bannerData.Season == seasonNum)
                list.Add((ITVDBBanner) bannerData);
            }
          }
        }
        return (IList<ITVDBBanner>) list;
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
        List<ITVDBBanner> list = new List<ITVDBBanner>();
        string xmlurl = this.GetXMLURL();
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(xmlurl);
        stringBuilder.Append("C315A8864A6B7866");
        stringBuilder.Append("/series/");
        stringBuilder.Append(seriesID);
        stringBuilder.Append("/banners.xml");
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.Load(((object) stringBuilder).ToString());
        XmlNodeList xmlNodeList = xmlDocument.SelectNodes("/Banners/Banner");
        if (xmlNodeList != null)
        {
          foreach (XmlNode node in xmlNodeList)
          {
            TVDBBanner bannerData = this.GetBannerData(node);
            if (bannerData.Path.Trim() != "" && bannerData.Type1.ToLower().Trim() == "fanart")
              list.Add((ITVDBBanner) bannerData);
          }
        }
        return (IList<ITVDBBanner>) list;
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
        List<ITVDBBanner> list = new List<ITVDBBanner>();
        string xmlurl = this.GetXMLURL();
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(xmlurl);
        stringBuilder.Append("C315A8864A6B7866");
        stringBuilder.Append("/series/");
        stringBuilder.Append(seriesID);
        stringBuilder.Append("/banners.xml");
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.Load(((object) stringBuilder).ToString());
        XmlNodeList xmlNodeList = xmlDocument.SelectNodes("/Banners/Banner");
        if (xmlNodeList != null)
        {
          foreach (XmlNode node in xmlNodeList)
          {
            TVDBBanner bannerData = this.GetBannerData(node);
            if (bannerData.Path.Trim() != "" && bannerData.Type1.ToLower().Trim() == "series")
              list.Add((ITVDBBanner) bannerData);
          }
        }
        return (IList<ITVDBBanner>) list;
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
        List<ITVDBUpdate> list = new List<ITVDBUpdate>();
        FastZip fastZip = new FastZip();
        string targetDirectory = Path.GetTempPath();
        if (!targetDirectory.EndsWith("\\"))
          targetDirectory = targetDirectory + "\\";
        string str1 = targetDirectory + "updates.zip";
        string str2 = targetDirectory + "updates_" + ((object) timeConstraint).ToString().ToLower() + ".xml";
        string zipUrl = this.GetZipURL();
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(zipUrl);
        stringBuilder.Append("C315A8864A6B7866");
        stringBuilder.Append("/updates/updates_");
        stringBuilder.Append(((object) timeConstraint).ToString().ToLower());
        stringBuilder.Append(".zip");
        WebClient webClient = new WebClient();
        webClient.DownloadFile(((object) stringBuilder).ToString(), str1);
        webClient.Dispose();
        if (System.IO.File.Exists(str2))
          System.IO.File.Delete(str2);
        fastZip.ExtractZip(str1, targetDirectory, "");
        if (System.IO.File.Exists(str2))
        {
          XmlDocument xmlDocument = new XmlDocument();
          xmlDocument.Load(str2);
          XmlNodeList xmlNodeList1 = xmlDocument.SelectNodes("/Data/Series");
          if (xmlNodeList1 != null)
          {
            foreach (XmlNode node in xmlNodeList1)
            {
              TVDBUpdate updateData = this.GetUpdateData(node);
              if (updateData != null && updateData.ID.Trim() != "")
                list.Add((ITVDBUpdate) updateData);
            }
          }
          XmlNodeList xmlNodeList2 = xmlDocument.SelectNodes("/Data/Episode");
          if (xmlNodeList2 != null)
          {
            foreach (XmlNode node in xmlNodeList2)
            {
              TVDBUpdate updateData = this.GetUpdateData(node);
              if (updateData != null && updateData.ID.Trim() != "")
                list.Add((ITVDBUpdate) updateData);
            }
          }
          XmlNodeList xmlNodeList3 = xmlDocument.SelectNodes("/Data/Banner");
          if (xmlNodeList3 != null)
          {
            foreach (XmlNode node in xmlNodeList3)
            {
              TVDBUpdate updateData = this.GetUpdateData(node);
              if (updateData != null && updateData.ID.Trim() != "")
                list.Add((ITVDBUpdate) updateData);
            }
          }
        }
        return (IList<ITVDBUpdate>) list;
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    public string GetBannerURL()
    {
      try
      {
        if (this._bannerMirrors == null || this._bannerMirrors.Count <= 0)
          return "http://thetvdb.com/banners/";
        return this._bannerMirrors[new Random().Next(0, this._bannerMirrors.Count - 1)];
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    private TVDBEpisode GetEpisodeData(XmlNode node)
    {
      try
      {
        TVDBEpisode tvdbEpisode = new TVDBEpisode();
        foreach (XmlNode xmlNode in node.ChildNodes)
        {
          if (xmlNode.InnerText != null && xmlNode.InnerText.Trim() != "")
          {
            if (xmlNode.Name.ToLower().Trim() == "id")
              tvdbEpisode.ID = xmlNode.InnerText.Trim();
            else if (xmlNode.Name.ToLower().Trim() == "dvd_chapter")
              tvdbEpisode.DVD_Chapter = xmlNode.InnerText.Trim();
            else if (xmlNode.Name.ToLower().Trim() == "dvd_discid")
              tvdbEpisode.DVD_Disc_ID = xmlNode.InnerText.Trim();
            else if (xmlNode.Name.ToLower().Trim() == "director")
              tvdbEpisode.Director = xmlNode.InnerText.Trim();
            else if (xmlNode.Name.ToLower().Trim() == "episodename")
              tvdbEpisode.EpisodeName = xmlNode.InnerText.Trim();
            else if (xmlNode.Name.ToLower().Trim() == "gueststars")
              tvdbEpisode.GuestStars = xmlNode.InnerText.Trim();
            else if (xmlNode.Name.ToLower().Trim() == "language")
              tvdbEpisode.Language = xmlNode.InnerText.Trim();
            else if (xmlNode.Name.ToLower().Trim() == "overview")
              tvdbEpisode.Overview = xmlNode.InnerText.Trim();
            else if (xmlNode.Name.ToLower().Trim() == "productioncode")
              tvdbEpisode.ProductionCode = xmlNode.InnerText.Trim();
            else if (xmlNode.Name.ToLower().Trim() == "writer")
              tvdbEpisode.Writer = xmlNode.InnerText.Trim();
            else if (xmlNode.Name.ToLower().Trim() == "filename")
              tvdbEpisode.ImageFile = this.GetBannerURL() + xmlNode.InnerText.Trim();
            else if (xmlNode.Name.ToLower().Trim() == "lastupdated")
            {
              long result = 0L;
              long.TryParse(xmlNode.InnerText.Trim(), out result);
              tvdbEpisode.LastUpdated = result;
            }
            else if (xmlNode.Name.ToLower().Trim() == "seasonid")
              tvdbEpisode.SeasonID = xmlNode.InnerText.Trim();
            else if (xmlNode.Name.ToLower().Trim() == "seriesid")
              tvdbEpisode.SeriesID = xmlNode.InnerText.Trim();
            else if (xmlNode.Name.ToLower().Trim() == "rating")
            {
              Decimal result = new Decimal(0);
              Decimal.TryParse(xmlNode.InnerText.Trim(), out result);
              tvdbEpisode.Rating = result;
            }
            else if (xmlNode.Name.ToLower().Trim() == "dvd_episodenumber")
            {
              Decimal result = new Decimal(0);
              Decimal.TryParse(xmlNode.InnerText.Trim(), out result);
              tvdbEpisode.DVD_Episode_Number = result;
            }
            else if (xmlNode.Name.ToLower().Trim() == "dvd_season")
            {
              int result = 0;
              int.TryParse(xmlNode.InnerText.Trim(), out result);
              tvdbEpisode.DVD_Season_Number = result;
            }
            else if (xmlNode.Name.ToLower().Trim() == "episodenumber")
            {
              int result = 0;
              int.TryParse(xmlNode.InnerText.Trim(), out result);
              tvdbEpisode.EpisodeNumber = result;
            }
            else if (xmlNode.Name.ToLower().Trim() == "seasonnumber")
            {
              int result = 0;
              int.TryParse(xmlNode.InnerText.Trim(), out result);
              tvdbEpisode.SeasonNumber = result;
            }
            else if (xmlNode.Name.ToLower().Trim() == "firstaired")
            {
              DateTime result = DateTime.MinValue;
              DateTime.TryParseExact(xmlNode.InnerText.Trim(), "yyyy-MM-dd", (IFormatProvider) null, DateTimeStyles.None, out result);
              tvdbEpisode.FirstAired = result;
            }
          }
        }
        return tvdbEpisode;
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    private TVDBSeries GetSeriesData(XmlNode node)
    {
      try
      {
        TVDBSeries tvdbSeries = new TVDBSeries();
        foreach (XmlNode xmlNode in node.ChildNodes)
        {
          if (xmlNode.InnerText != null && xmlNode.InnerText.Trim() != "")
          {
            if (xmlNode.Name.ToLower().Trim() == "id")
              tvdbSeries.ID = xmlNode.InnerText.Trim();
            else if (xmlNode.Name.ToLower().Trim() == "actors")
              tvdbSeries.Actors = xmlNode.InnerText.Trim();
            else if (xmlNode.Name.ToLower().Trim() == "airs_dayofweek")
              tvdbSeries.Airs_DayOfWeek = xmlNode.InnerText.Trim();
            else if (xmlNode.Name.ToLower().Trim() == "airs_time")
              tvdbSeries.Airs_TimeOfDay = xmlNode.InnerText.Trim();
            else if (xmlNode.Name.ToLower().Trim() == "genre")
              tvdbSeries.Genre = xmlNode.InnerText.Trim();
            else if (xmlNode.Name.ToLower().Trim() == "imdb_id")
              tvdbSeries.IMDb_ID = xmlNode.InnerText.Trim();
            else if (xmlNode.Name.ToLower().Trim() == "zap2it_id")
              tvdbSeries.Zap2It_ID = xmlNode.InnerText.Trim();
            else if (xmlNode.Name.ToLower().Trim() == "language")
              tvdbSeries.Language = xmlNode.InnerText.Trim();
            else if (xmlNode.Name.ToLower().Trim() == "network")
              tvdbSeries.Network = xmlNode.InnerText.Trim();
            else if (xmlNode.Name.ToLower().Trim() == "overview")
              tvdbSeries.Overview = xmlNode.InnerText.Trim();
            else if (xmlNode.Name.ToLower().Trim() == "runtime")
              tvdbSeries.Runtime = xmlNode.InnerText.Trim();
            else if (xmlNode.Name.ToLower().Trim() == "seriesname")
              tvdbSeries.SeriesName = xmlNode.InnerText.Trim();
            else if (xmlNode.Name.ToLower().Trim() == "status")
              tvdbSeries.Status = xmlNode.InnerText.Trim();
            else if (xmlNode.Name.ToLower().Trim() == "lastupdated")
            {
              long result = 0L;
              long.TryParse(xmlNode.InnerText.Trim(), out result);
              tvdbSeries.LastUpdated = result;
            }
            else if (xmlNode.Name.ToLower().Trim() == "rating")
            {
              Decimal result = new Decimal(0);
              Decimal.TryParse(xmlNode.InnerText.Trim(), out result);
              tvdbSeries.Rating = result;
            }
            else if (xmlNode.Name.ToLower().Trim() == "firstaired")
            {
              DateTime result = DateTime.MinValue;
              DateTime.TryParseExact(xmlNode.InnerText.Trim(), "yyyy-MM-dd", (IFormatProvider) null, DateTimeStyles.None, out result);
              tvdbSeries.FirstAired = result;
            }
          }
        }
        return tvdbSeries;
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    private TVDBBanner GetBannerData(XmlNode node)
    {
      try
      {
        TVDBBanner tvdbBanner = new TVDBBanner();
        foreach (XmlNode xmlNode in node.ChildNodes)
        {
          if (xmlNode.InnerText != null && xmlNode.InnerText.Trim() != "")
          {
            if (xmlNode.Name.ToLower().Trim() == "id")
              tvdbBanner.ID = xmlNode.InnerText.Trim();
            else if (xmlNode.Name.ToLower().Trim() == "bannerpath")
              tvdbBanner.Path = this.GetBannerURL() + xmlNode.InnerText.Trim();
            else if (xmlNode.Name.ToLower().Trim() == "bannertype")
              tvdbBanner.Type1 = xmlNode.InnerText.Trim();
            else if (xmlNode.Name.ToLower().Trim() == "bannertype2")
              tvdbBanner.Type2 = xmlNode.InnerText.Trim();
            else if (xmlNode.Name.ToLower().Trim() == "language")
              tvdbBanner.Language = xmlNode.InnerText.Trim();
            else if (xmlNode.Name.ToLower().Trim() == "vignettepath")
              tvdbBanner.VignettePath = this.GetBannerURL() + xmlNode.InnerText.Trim();
            else if (xmlNode.Name.ToLower().Trim() == "season")
            {
              int result = 0;
              int.TryParse(xmlNode.InnerText.Trim(), out result);
              tvdbBanner.Season = result;
            }
          }
        }
        return tvdbBanner;
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    private TVDBUpdate GetUpdateData(XmlNode node)
    {
      try
      {
        TVDBUpdate tvdbUpdate = new TVDBUpdate();
        if (node.Name.ToLower().Trim() == "series")
          tvdbUpdate.UpdateType = TVDBLib.UpdateTypeEnum.Series;
        else if (node.Name.ToLower().Trim() == "episode")
          tvdbUpdate.UpdateType = TVDBLib.UpdateTypeEnum.Episode;
        else if (node.Name.ToLower().Trim() == "banner")
          tvdbUpdate.UpdateType = TVDBLib.UpdateTypeEnum.Banner;
        foreach (XmlNode xmlNode in node.ChildNodes)
        {
          if (xmlNode.InnerText != null && xmlNode.InnerText.Trim() != "")
          {
            if (xmlNode.Name.ToLower().Trim() == "id")
              tvdbUpdate.ID = xmlNode.InnerText.Trim();
            else if (xmlNode.Name.ToLower().Trim() == "time")
            {
              long result = 0L;
              long.TryParse(xmlNode.InnerText.Trim(), out result);
              tvdbUpdate.UpdateTime = result;
            }
          }
        }
        return tvdbUpdate;
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    private void GetCurrentServerTime()
    {
      try
      {
        string filename = "http://thetvdb.com/api/Updates.php?type=none";
        string s = "";
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.Load(filename);
        XmlNodeList xmlNodeList = xmlDocument.SelectNodes("/Items/Time");
        if (xmlNodeList != null && xmlNodeList.Count > 0)
          s = xmlNodeList[0].InnerText;
        if (s.Trim() != "")
        {
          long result = 0L;
          long.TryParse(s, out result);
          this._serverTime = result;
        }
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    private void GetMirrors()
    {
      try
      {
        string filename = "http://thetvdb.com/api/C315A8864A6B7866/mirrors.xml";
        this._xmlMirrors = new List<string>();
        this._zipMirrors = new List<string>();
        this._bannerMirrors = new List<string>();
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.Load(filename);
        XmlNodeList xmlNodeList = xmlDocument.SelectNodes("/Mirrors/Mirror");
        if (xmlNodeList != null && xmlNodeList.Count > 0)
        {
          foreach (XmlNode xmlNode1 in xmlNodeList)
          {
            string str1 = "";
            string str2 = "";
            foreach (XmlNode xmlNode2 in xmlNode1.ChildNodes)
            {
              if (xmlNode2.InnerText != null && xmlNode2.InnerText.Trim() != "")
              {
                if (xmlNode2.Name.ToLower().Trim() == "mirrorpath")
                  str1 = xmlNode2.InnerText.Trim();
                else if (xmlNode2.Name.ToLower().Trim() == "typemask")
                  str2 = xmlNode2.InnerText.Trim();
              }
            }
            if (str1.Trim() != "" && str2.Trim() != "")
            {
              if (!str1.EndsWith("/"))
                str1 = str1 + "/";
              if (str2 == "7")
              {
                this._bannerMirrors.Add(str1 + "banners/");
                this._xmlMirrors.Add(str1 + "api/");
                this._zipMirrors.Add(str1 + "api/");
              }
              else if (str2 == "6")
              {
                this._bannerMirrors.Add(str1 + "banners/");
                this._zipMirrors.Add(str1 + "api/");
              }
              else if (str2 == "5")
              {
                this._xmlMirrors.Add(str1 + "api/");
                this._zipMirrors.Add(str1 + "api/");
              }
              else if (str2 == "4")
                this._zipMirrors.Add(str1 + "api/");
              else if (str2 == "2")
                this._bannerMirrors.Add(str1 + "banners/");
              else if (str2 == "1")
                this._xmlMirrors.Add(str1 + "api/");
            }
          }
        }
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    private string GetXMLURL()
    {
      try
      {
        if (this._xmlMirrors == null || this._xmlMirrors.Count <= 0)
          return "http://thetvdb.com/api/";
        return this._xmlMirrors[new Random().Next(0, this._xmlMirrors.Count - 1)];
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    private string GetZipURL()
    {
      try
      {
        if (this._zipMirrors == null || this._zipMirrors.Count <= 0)
          return "http://thetvdb.com/api/";
        return this._zipMirrors[new Random().Next(0, this._zipMirrors.Count - 1)];
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }
  }
}
