// Type: TVDBLibrary.ITVDBSeries
// Assembly: TVDBLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 34AA2317-12CB-4E3C-A53E-6904E324E0D4
// Assembly location: P:\Development\Assemblies\TVDBLibrary.dll

using System;

namespace TVDBLibrary
{
  public interface ITVDBSeries
  {
    string ID { get; set; }

    string IMDb_ID { get; set; }

    string Zap2It_ID { get; set; }

    string Actors { get; set; }

    string Airs_DayOfWeek { get; set; }

    string Airs_TimeOfDay { get; set; }

    DateTime FirstAired { get; set; }

    string Genre { get; set; }

    string Language { get; set; }

    string Network { get; set; }

    string Overview { get; set; }

    Decimal Rating { get; set; }

    string Runtime { get; set; }

    string SeriesName { get; set; }

    string Status { get; set; }

    long LastUpdated { get; set; }

    string SortName { get; }
  }
}
