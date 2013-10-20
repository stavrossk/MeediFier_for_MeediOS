// Type: TVDBLibrary.TVDBLanguage
// Assembly: TVDBLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 34AA2317-12CB-4E3C-A53E-6904E324E0D4
// Assembly location: P:\Development\Assemblies\TVDBLibrary.dll

namespace TVDBLibrary
{
  internal class TVDBLanguage : ITVDBLanguage
  {
    private string _name = "";
    private string _abbrev = "";
    private string _id = "";

    public string Name
    {
      get
      {
        return this._name;
      }
      set
      {
        this._name = value;
      }
    }

    public string Abbreviation
    {
      get
      {
        return this._abbrev;
      }
      set
      {
        this._abbrev = value;
      }
    }

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
  }
}
