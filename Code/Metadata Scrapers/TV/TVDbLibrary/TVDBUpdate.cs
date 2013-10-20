// Type: TVDBLibrary.TVDBUpdate
// Assembly: TVDBLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 34AA2317-12CB-4E3C-A53E-6904E324E0D4
// Assembly location: P:\Development\Assemblies\TVDBLibrary.dll

namespace TVDBLibrary
{
  internal class TVDBUpdate : ITVDBUpdate
  {
    private string _id = "";
    private TVDBLib.UpdateTypeEnum _updateType;
    private long _updateTime;

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

    public TVDBLib.UpdateTypeEnum UpdateType
    {
      get
      {
        return this._updateType;
      }
      set
      {
        this._updateType = value;
      }
    }

    public long UpdateTime
    {
      get
      {
        return this._updateTime;
      }
      set
      {
        this._updateTime = value;
      }
    }
  }
}
