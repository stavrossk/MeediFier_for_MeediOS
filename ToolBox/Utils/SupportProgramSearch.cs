using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using MediaFairy.ToolBox;

namespace ToolBox
{
    class SupportPrograms
    {
        public static void SearchForSupportPrograms(List<string> ProgramList, string DataDirectory, bool Log, StreamWriter swWriteLog)
        {
            string SupportProgramFile = Path.Combine(DataDirectory, "SupportPrograms.txt");
            string PathToTsMuxer = "";
            string PathToEac3To = "";
            string PathToImgBurn = "";
            string PathToVirtualCloneDrive = "";
            string PathToHandBrake = "";

            List<string> SearchList = DirSearch(ProgramList, Log, swWriteLog);
            foreach (string s in SearchList)
            {
                if (Path.GetFileName(s).ToLower() == "tsmuxer.exe")
                {
                    PathToTsMuxer = "Path to TsMuxeR: " + s;
                }
                if (Path.GetFileName(s).ToLower() == "eac3to.exe")
                {
                    PathToEac3To = "Path to Eac3To: " + s;
                }
                if (Path.GetFileName(s).ToLower() == "imgburn.exe")
                {
                    PathToImgBurn = "Path to ImgBurn: " + s;
                }
                if (Path.GetFileName(s).ToLower() == "vcdmount.exe")
                {
                    PathToVirtualCloneDrive = "Path to Virtual Clone Drive: " + s;
                }
                if (Path.GetFileName(s).ToLower() == "handbrakecli.exe")
                {
                    PathToHandBrake = "Path to HandBrake: " + s;
                }                
            }

            using (StreamWriter sw = new StreamWriter(SupportProgramFile))
            {
                try
                {
                    if (PathToTsMuxer != "")
                        sw.WriteLine(PathToTsMuxer);
                    if (PathToEac3To != "")
                        sw.WriteLine(PathToEac3To);
                    if (PathToImgBurn != "")
                        sw.WriteLine(PathToImgBurn);
                    if (PathToVirtualCloneDrive != "")
                        sw.WriteLine(PathToVirtualCloneDrive);
                    if (PathToHandBrake != "")
                        sw.WriteLine(PathToHandBrake);                    
                    sw.Close();
                    sw.Dispose();
                }
                catch (Exception e)
                {
                    MeedioUtils.LogEntry(Log, swWriteLog, "[SupportPrograms] - Error writing file: " + e.ToString());
                }
            }
        }

        private static List<string> DirSearch(List<string> ProgramList, bool Log, StreamWriter swWriteLog)
        {
            List<string> PList = new List<string>();
            try
            {
                foreach (string p in ProgramList)
                {
                    foreach (string s in Directory.GetLogicalDrives())
                    {
                        try
                        {
                            foreach (string d in Directory.GetDirectories(s))
                            {
                                try
                                {
                                    foreach (string f in Directory.GetFiles(d, "*.exe", SearchOption.AllDirectories))
                                    {
                                        if (p.ToLower() == Path.GetFileName(f.ToLower()))
                                        {
                                            PList.Add(Path.GetFullPath(f));
                                        }
                                    }
                                }
                                catch
                                {
                                }
                            }
                        }
                        catch
                        {
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MeedioUtils.LogEntry(Log, swWriteLog, "[SupportPrograms] - Search error: " + e.ToString());
            }
            return PList;
        }

        public static string GetProgramPath(string Program, string DataDirectory)
        {
            string ProgramPath = "";
            string SupportProgramFile = Path.Combine(DataDirectory, "SupportPrograms.txt");
            using (StreamReader reader = new StreamReader(SupportProgramFile, Encoding.GetEncoding("iso-8859-1")))
            {
                string line = "";
                for (int i = 1; i < 10; i++)
                {
                    try
                    {
                        while ((line = reader.ReadLine()) != null || (line = reader.ReadLine()) != "")
                        {
                            int c = line.IndexOf(":");
                            string p1 = line.Substring(c + 1, line.Length - (c + 1));
                            if (Path.GetFileName(p1).ToLower() == Program.ToLower())
                            {
                                ProgramPath = p1;
                                return ProgramPath.Trim();
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }
            return ProgramPath;
        }

    }
}