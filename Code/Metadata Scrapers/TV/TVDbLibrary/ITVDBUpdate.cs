// Type: TVDBLibrary.ITVDBUpdate
// Assembly: TVDBLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 34AA2317-12CB-4E3C-A53E-6904E324E0D4
// Assembly location: P:\Development\Assemblies\TVDBLibrary.dll

namespace TVDBLibrary
{
  public interface ITVDBUpdate
  {
    string ID { get; set; }

    TVDBLib.UpdateTypeEnum UpdateType { get; set; }

    long UpdateTime { get; set; }
  }
}
