// Type: TVDBLibrary.Utils
// Assembly: TVDBLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 34AA2317-12CB-4E3C-A53E-6904E324E0D4
// Assembly location: P:\Development\Assemblies\TVDBLibrary.dll

namespace TVDBLibrary
{
  internal static class Utils
  {
    public static string SafeSQL(string text)
    {
      return text.Replace("'", "''");
    }

    public static string StripInvalidCharacters(string text)
    {
      return text.Replace(":", "").Replace(";", "").Replace("\"", "");
    }
  }
}
