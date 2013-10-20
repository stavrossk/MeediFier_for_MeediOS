// Type: TVDBLibrary.TVDBBanner
// Assembly: TVDBLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 34AA2317-12CB-4E3C-A53E-6904E324E0D4
// Assembly location: P:\Development\Assemblies\TVDBLibrary.dll

namespace TVDBLibrary
{
  internal class TVDBBanner : ITVDBBanner
  {
    private string _id = "";
    private string _path = "";
    private string _vignettePath = "";
    private string _type1 = "";
    private string _type2 = "";
    private string _language = "";
    private int _season;

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

    public string Path
    {
      get
      {
        return this._path;
      }
      set
      {
        this._path = value;
      }
    }

    public string VignettePath
    {
      get
      {
        return this._vignettePath;
      }
      set
      {
        this._vignettePath = value;
      }
    }

    public string Type1
    {
      get
      {
        return this._type1;
      }
      set
      {
        this._type1 = value;
      }
    }

    public string Type2
    {
      get
      {
        return this._type2;
      }
      set
      {
        this._type2 = value;
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

    public int Season
    {
      get
      {
        return this._season;
      }
      set
      {
        this._season = value;
      }
    }
  }
}
