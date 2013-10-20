// Type: TVDBLibrary.ITVDBBanner
// Assembly: TVDBLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 34AA2317-12CB-4E3C-A53E-6904E324E0D4
// Assembly location: P:\Development\Assemblies\TVDBLibrary.dll

namespace TVDBLibrary
{
  public interface ITVDBBanner
  {
    string ID { get; set; }

    string Path { get; set; }

    string VignettePath { get; set; }

    string Type1 { get; set; }

    string Type2 { get; set; }

    string Language { get; set; }

    int Season { get; set; }
  }
}
