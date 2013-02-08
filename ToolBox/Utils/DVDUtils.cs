using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml;
using DirectShowLib;
using MediaFairy.ToolBox;
using Debugger = MediaFairy.Debugger;

namespace ToolBox
{
    public class DVDUtils
    {
        public static class MCICDROM
        {
            [DllImport("winmm.dll", EntryPoint = "mciSendStringA")]
            private static extern void mciSendStringA(string lpstrCommand, string lpstrReturnString, long uReturnLength, long hwndCallback);

            public static void Eject(string driveLetter)
            {
                string returnString = "";
                mciSendStringA("set cdaudio!" + driveLetter + " door open", returnString, 0, 0);
            }

            public static void Close(string driveLetter)
            {
                string returnString = "";
                mciSendStringA("set cdaudio!" + driveLetter + " door closed", returnString, 0, 0);
            }
        }

        public static string GetVolumeLabel(string drive)
        {
            string isoLabel = new System.IO.DriveInfo(drive).VolumeLabel;
            return isoLabel;
        }

        public static string GetDVDID(string Drive)
        {
            long id = 0;
            int hr;
            object comobj = null;
            DirectShowLib.Dvd.AMDvdRenderStatus status;
            DirectShowLib.Dvd.IDvdGraphBuilder dvdGraph = (DirectShowLib.Dvd.IDvdGraphBuilder)new DirectShowLib.DvdGraphBuilder();
            hr = dvdGraph.RenderDvdVideoVolume(null, DirectShowLib.Dvd.AMDvdGraphFlags.None, out status);
            DsError.ThrowExceptionForHR(hr);
            hr = dvdGraph.GetDvdInterface(typeof(DirectShowLib.Dvd.IDvdInfo2).GUID, out comobj);
            DsError.ThrowExceptionForHR(hr);
            DirectShowLib.Dvd.IDvdInfo2 dvdInfo = (DirectShowLib.Dvd.IDvdInfo2)comobj;
            comobj = null;
            dvdInfo.GetDiscID(Drive + ":\\VIDEO_TS", out id);
            const int HEXADECIMAL = 16;
            String HexID = Convert.ToString(id, HEXADECIMAL);
            Debugger.LogMessageToFile("[DVDUtils] - DVDID: " + HexID);
            return HexID;
        }

        public static bool IsTVSeries(string SeriesName)
        {
            bool tvShow = false;
            try
            {
                Debugger.LogMessageToFile("[DVDUtils] - Starting TVDb search");
                Debugger.LogMessageToFile("[DVDUtils] - Search string: " + SeriesName);
                TVDBLibrary.TVDBLib newTVSearch = new TVDBLibrary.TVDBLib();
                IList<TVDBLibrary.ITVDBSeries> SeriesSearch = newTVSearch.SearchSeries(SeriesName);
                foreach (TVDBLibrary.ITVDBSeries r in SeriesSearch)
                {
                    Debugger.LogMessageToFile("[DVDUtils] - Series search result: " + r.SeriesName);
                    if (SeriesName.ToLower() == r.SeriesName.ToLower() || SeriesName.ToLower().Contains(r.SeriesName.ToLower()) || r.SeriesName.ToLower().Contains(SeriesName.ToLower()))
                    {
                        Debugger.LogMessageToFile("[DVDUtils] - DVD series name: " + SeriesName + " matched with TVDb series: " + r.SeriesName);
                        tvShow = true;
                        break;
                    }
                }
            }
            catch
            {
            }
            return tvShow;
        }

        public static void CreateISOwAnyDVD(string AnyTool2Path, string ISOFile, string DriveLetter, bool Log, StreamWriter swWriteLog)
        {
            //PATH TO DESTINATION
            string Destination = ISOFile;
            string rootD = Destination.Remove(3);
            string DestWithoutRoot = Destination.Substring(3, Destination.Length - 3);

            //PATH TO ANYTOOL2
            string root = AnyTool2Path.Remove(3);
            string pathwithoutRoot = AnyTool2Path.Substring(3, AnyTool2Path.Length - 3);

            string ISOCLI = root + "\"" + pathwithoutRoot + "\" ripiso " + DriveLetter + " " + rootD + "\"" + DestWithoutRoot + "\"";
            MeedioUtils.LogEntry(Log, swWriteLog, "[DVDUtils] - Create ISO command line : " + ISOCLI);

            try
            {
                ProcessStartInfo ISOinfo = new ProcessStartInfo("cmd", "/C " + ISOCLI);
                Process iso = new Process();
                iso.StartInfo = ISOinfo;
                iso.StartInfo.CreateNoWindow = true;
                iso.StartInfo.UseShellExecute = false;
                iso.Start();
                ToolBox.SystemUtils.NativeMethods.WaitForWindow("AnyDVD Ripper");
                //System.Threading.Thread.Sleep(500); //WAIT FOR WINDOWS TO OPEN
                ToolBox.SystemUtils.NativeMethods.SetWindowVisibility(false, "AnyDVD Ripper");
                iso.WaitForExit();
            }
            catch (Exception e)
            {
                MeedioUtils.LogEntry(Log, swWriteLog, "[DVDUtils] - Error creating ISO: " + e.ToString());
            }
        }

        public static void CreateISOwIMGBURN(string ImgBurnPath, string ISOFile, string DriveLetter, string InputFolder, bool Log, StreamWriter swWriteLog)
        {
            string root = ImgBurnPath.Remove(3);
            string pathwithoutRoot = ImgBurnPath.Substring(3, ImgBurnPath.Length - 3);
            string vLable = Path.GetFileNameWithoutExtension(ISOFile).Replace(" ", "_");
            string ISOCLI = "";
            if (DriveLetter != "")
            {
                ISOCLI = root + "\"" + pathwithoutRoot + "\" /MODE BUILD /BUILDMODE IMAGEFILE /SRC \"" + DriveLetter + ":\" /DEST \"" + ISOFile + "\" /FILESYSTEM \"UDF\" /UDFREVISION \"2.50\" /VOLUMELABEL \"" + vLable + "\" /CLOSE /NOIMAGEDETAILS /ROOTFOLDER \"YES\" /START";
            }
            if (DriveLetter == "")
            {
                ISOCLI = root + "\"" + pathwithoutRoot + "\" /MODE BUILD /BUILDMODE IMAGEFILE /SRC \"" + InputFolder + "\" /DEST \"" + ISOFile + "\" /FILESYSTEM \"UDF\" /UDFREVISION \"2.50\" /VOLUMELABEL \"" + vLable + "\" /CLOSE /NOIMAGEDETAILS /ROOTFOLDER \"YES\" /START";
            }
            MeedioUtils.LogEntry(Log, swWriteLog, "[DVDUtils] - Create ISO command line : " + ISOCLI);

            try
            {
                ProcessStartInfo ISOinfo = new ProcessStartInfo("cmd", "/C " + ISOCLI);
                Process iso = new Process();
                iso.StartInfo = ISOinfo;
                iso.StartInfo.CreateNoWindow = true;
                iso.StartInfo.UseShellExecute = false;
                iso.Start();
                MeedioUtils.FocusMeedio.SetFocusToMeedio();
                ToolBox.SystemUtils.NativeMethods.WaitForWindow("ImgBurn");
                ToolBox.SystemUtils.NativeMethods.WaitForWindow("ImgBurn Log");
                System.Threading.Thread.Sleep(1000); //WAIT FOR WINDOWS TO OPEN
                ToolBox.SystemUtils.NativeMethods.SetWindowVisibility(false, "ImgBurn");
                ToolBox.SystemUtils.NativeMethods.SetWindowVisibility(false, "ImgBurn Log");
                MeedioUtils.FocusMeedio.SetFocusToMeedio();
                iso.WaitForExit();
            }
            catch (Exception e)
            {
                MeedioUtils.LogEntry(Log, swWriteLog, "[DVDUtils] - Error creating ISO: " + e.ToString());
            }
        }

        public static void CreateISOwMKISOFS(string mkisofsPath, string ISOFile, string DriveLetter, bool Log, StreamWriter swWriteLog)
        {
            string drive = DriveLetter + ":\\";
            string isoLabel = ToolBox.DVDUtils.GetVolumeLabel(DriveLetter + ":");

            //PATH TO MKISOFS
            string root = mkisofsPath.Remove(3);
            string pathwithoutRoot = mkisofsPath.Substring(3, mkisofsPath.Length - 3);

            string mkISOCLI = root + "\"" + pathwithoutRoot + "\" -dvd-video -udf -v -o \"" + ISOFile + "\" " + drive;
            MeedioUtils.LogEntry(Log, swWriteLog, "[DVDUtils] - Create ISO command line : " + mkISOCLI);

            try
            {
                ProcessStartInfo ISOinfo = new ProcessStartInfo("cmd", "/C " + mkISOCLI);
                Process iso = new Process();
                iso.StartInfo = ISOinfo;
                iso.StartInfo.CreateNoWindow = true;
                iso.StartInfo.UseShellExecute = false;
                iso.Start();
                iso.WaitForExit();
            }
            catch (Exception e)
            {
                MeedioUtils.LogEntry(Log, swWriteLog, "[DVDUtils] - Error creating ISO: " + e.ToString());
            }
        }

        public static void runDVDDecrypter(string DriveLetter, string DestDirectory, string DVDDecrypterPath, string ALLorMAINMOVIE, bool Log, StreamWriter swWriteLog)
        {
            try
            {
                String Drive = DriveLetter + ":";
                Process dd = new Process();
                dd.StartInfo.FileName = DVDDecrypterPath;
                if (ALLorMAINMOVIE == "all")
                {
                    dd.StartInfo.Arguments = "/MODE FILE /SRC " + Drive + " /DEST \"" + DestDirectory + "\" /SPEED MAX /OVERWRITE YES /START /CLOSE";
                }
                if (ALLorMAINMOVIE == "mainmovie")
                {
                    dd.StartInfo.Arguments = "/MODE FILE /SRC " + Drive + " /DEST \"" + DestDirectory + "\" /SPEED MAX /OVERWRITE YES /FILES MOVIE \"VIDEO_TS.IFO\" /START /CLOSE";
                }
                dd.StartInfo.CreateNoWindow = true;
                dd.StartInfo.UseShellExecute = false;
                dd.Start();
                MeedioUtils.FocusMeedio.SetFocusToMeedio();
                ToolBox.SystemUtils.NativeMethods.WaitForWindow("DVD Decrypter");
                ToolBox.SystemUtils.NativeMethods.WaitForWindow("DVD Decrypter Log");
                MeedioUtils.FocusMeedio.SetFocusToMeedio();
                System.Threading.Thread.Sleep(1000); //WAIT FOR WINDOWS TO OPEN
                ToolBox.SystemUtils.NativeMethods.SetWindowVisibility(false, "DVD Decrypter");
                ToolBox.SystemUtils.NativeMethods.SetWindowVisibility(false, "DVD Decrypter Log");
                MeedioUtils.FocusMeedio.SetFocusToMeedio();
                dd.WaitForExit();
            }
            catch (Exception e)
            {
                MeedioUtils.LogEntry(Log, swWriteLog, "[DVDUtils] - Error running DVDDecrypter: " + e.ToString());
            }
            return;
        }

        public static string GetSeasonNumber(string Drive)
        {
            //GET SEASON NUMBER FROM DISC VOLUME LABEL            
            string DiscVolumeLabel = ToolBox.DVDUtils.GetVolumeLabel(Drive);
            Debugger.LogMessageToFile("[DVDUtils] - Disc volume label: " + DiscVolumeLabel);
            string SeasonNumber = "";
            string[] SeasonAbbreviations = { " s", " season" };
            if (ToolBox.StringFunctions.HasNumbers(DiscVolumeLabel))
            {
                DiscVolumeLabel = DiscVolumeLabel.Replace("_", " ");
                foreach (string v in SeasonAbbreviations)
                {
                    if (DiscVolumeLabel.Contains(v))
                    {
                        int v1 = DiscVolumeLabel.IndexOf(v);
                        string v2 = DiscVolumeLabel.Substring(v1, DiscVolumeLabel.Length - v1);
                        foreach (char c in v2)
                        {
                            if (ToolBox.StringFunctions.IsNumeric(c.ToString()))
                            {
                                SeasonNumber = c.ToString();
                                break;
                            }
                        }
                    }
                }
                //TRY AND GET NUMBER IF NO IDENTIFIER
                if (SeasonNumber == "")
                {
                    int i = 0;
                    foreach (char c in DiscVolumeLabel)
                    {
                        i++;
                        int i2 = DiscVolumeLabel.Length - i;
                        char c2 = ToolBox.StringFunctions.CharRight(DiscVolumeLabel, i2);
                        if (ToolBox.StringFunctions.IsNumeric(c2.ToString()))
                        {
                            SeasonNumber = c2.ToString();
                            break;
                        }
                    }
                }
            }
            return SeasonNumber;
        }

        public static string GetSeasonDiscNumber(string Drive)
        {
            //GET DISC NUMBER FROM DISC VOLUME LABEL            
            string DiscVolumeLabel = ToolBox.DVDUtils.GetVolumeLabel(Drive);
            Debugger.LogMessageToFile("[DVDUtils] - Disc volume label: " + DiscVolumeLabel);
            string DiscNumber = "";
            string[] SeasonAbbreviations = { " d", " disc", " disk" };
            if (ToolBox.StringFunctions.HasNumbers(DiscVolumeLabel))
            {
                DiscVolumeLabel = DiscVolumeLabel.Replace("_", " ");
                foreach (string v in SeasonAbbreviations)
                {
                    if (DiscVolumeLabel.Contains(v))
                    {
                        int v1 = DiscVolumeLabel.IndexOf(v);
                        string v2 = DiscVolumeLabel.Substring(v1, DiscVolumeLabel.Length - v1);
                        foreach (char c in v2)
                        {
                            if (ToolBox.StringFunctions.IsNumeric(c.ToString()))
                            {
                                DiscNumber = c.ToString();
                                break;
                            }
                        }
                    }
                }
                //TRY AND GET NUMBER IF NO IDENTIFIER
                if (DiscNumber == "")
                {
                    int i = 0;
                    foreach (char c in DiscVolumeLabel)
                    {
                        i++;
                        int i2 = DiscVolumeLabel.Length - i;
                        char c2 = ToolBox.StringFunctions.CharRight(DiscVolumeLabel, i2);
                        if (ToolBox.StringFunctions.IsNumeric(c2.ToString()))
                        {
                            DiscNumber = c2.ToString();
                            break;
                        }
                    }
                }
            }
            return DiscNumber;
        }

        public static bool IsAnyDVDRunning()
        {
            bool running = false;
            String Pname = "AnyDVD";
            Process[] procs;
            procs = Process.GetProcesses();
            foreach (Process P in procs)
            {
                if (P.ProcessName.StartsWith(Pname))
                {
                    running = true;
                    break;
                }
            }
            return running;
        }

        public static string GetBluRayTitle(string Drive)
        {
            String BRTitle1 = "";
            String BRTitle3 = "";
            if (File.Exists(Drive + ":\\BDMV\\META\\DL\\bdmt_eng.xml"))
            {
                String MetaPath = (Drive + ":\\BDMV\\META\\DL\\bdmt_eng.xml");
                XmlDocument doc1 = new XmlDocument();
                doc1.Load(MetaPath);
                doc1.PreserveWhitespace = false;
                XmlTextReader rdr1 = new XmlTextReader(MetaPath);
                rdr1.Read();
                doc1.Load(rdr1);
                XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc1.NameTable);
                nsmgr.AddNamespace("di", "urn:BDA:bdmv;discinfo");
                if (doc1.SelectSingleNode("//di:discinfo/di:title/di:name", nsmgr) != null)
                {
                    BRTitle1 = doc1.SelectSingleNode("//di:discinfo/di:title/di:name", nsmgr).InnerText.ToString();
                }
                rdr1.Close();

                //CLEAN UP MOVIE TITLE
                String[] stringToTrim = { "- Blu-ray", "Disc 1", "Disc 2", "Disc1", "Disc2" };
                bool titleTrimed = false;
                foreach (string s in stringToTrim)
                {
                    if (BRTitle1.Contains(s))
                    {
                        int Pos = BRTitle1.IndexOf(s);
                        String BRTitle2 = BRTitle1.Remove(Pos);
                        BRTitle3 = BRTitle2.Trim();
                        BRTitle1 = BRTitle2.Trim();
                        titleTrimed = true;
                    }
                }
                if (titleTrimed == false)
                {
                    BRTitle3 = BRTitle1.Trim();
                }
            }
            return BRTitle3;
        }

        public static void BackUpAllDVDFiles(string DriveLetter, string DestDirectory, bool Log, StreamWriter swWriteLog)
        {
            MeedioUtils.LogEntry(Log, swWriteLog, "[DVDUtils] - Attempting to backup files");
            string[] FileList = Directory.GetFiles(DriveLetter + ":", "*.*", SearchOption.AllDirectories);
            foreach (string f in FileList)
            {
                try
                {
                    MeedioUtils.LogEntry(Log, swWriteLog, "[DVDUtils] - DVD file: " + f);
                    File.Copy(Path.GetFullPath(f), (DestDirectory + "\\" + Path.GetFileName(f)));
                }
                catch (Exception e)
                {
                    MeedioUtils.LogEntry(Log, swWriteLog, "[DVDUtils] - Error backing up files: " + e.ToString());
                }
            }
        }

        public static void BackUpMainMovieDVDFiles(string DriveLetter, string DestDirectory, bool Log, StreamWriter swWriteLog)
        {
            MeedioUtils.LogEntry(Log, swWriteLog, "[DVDUtils] - Attempting to backup files");
            string MovieTitle = "";
            string[] FileList = Directory.GetFiles(DriveLetter + ":", "*.*", SearchOption.AllDirectories);
            foreach (string f in FileList)
            {
                FileInfo FI = new FileInfo(Path.GetFullPath(f));
                long fSize = FI.Length;
                if (fSize > 1000000000)
                {
                    MovieTitle = Path.GetFileNameWithoutExtension(f).Substring(4, 2);
                    break;
                }
            }
            MeedioUtils.LogEntry(Log, swWriteLog, "[DVDUtils] - Main movie title: " + MovieTitle);

            foreach (string f in FileList)
            {
                if (Path.GetFileName(f).Contains("VTS_" + MovieTitle) || Path.GetFileName(f).ToLower() == ("video_ts.ifo"))
                {
                    if (Path.GetExtension(f).ToLower() != ".bup" && !Path.GetFileName(f).ToLower().EndsWith("0.vob"))
                    {
                        try
                        {
                            MeedioUtils.LogEntry(Log, swWriteLog, "[DVDUtils] - DVD file: " + f);
                            File.Copy(Path.GetFullPath(f), (DestDirectory + "\\" + Path.GetFileName(f)));
                        }
                        catch (Exception e)
                        {
                            MeedioUtils.LogEntry(Log, swWriteLog, "[DVDUtils] - Error backing up files: " + e.ToString());
                        }
                    }
                }
            }
        }

        public static void VOBFilesToMpeg(string outputFileMpg, string SourceDirectory, bool Log, StreamWriter swWriteLog)
        {
            string[] FileList = Directory.GetFiles(SourceDirectory, "*.*", SearchOption.AllDirectories);
            string vobFiles = "";
            foreach (string f in FileList)
            {
                if (Path.GetExtension(f).ToLower() == ".vob")
                {
                    vobFiles = vobFiles + "\"" + Path.GetFullPath(f) + "\"" +" + ";
                }
            }
            vobFiles = vobFiles.Remove(vobFiles.LastIndexOf("+")).Trim();
            MeedioUtils.LogEntry(Log, swWriteLog, "[DVDUtils] - Vob file list: " + vobFiles);            
            string cli = "copy /B " + vobFiles + " \"" + outputFileMpg + "\"";
            MeedioUtils.LogEntry(Log, swWriteLog, "[DVDUtils] - MPG command line: " + cli);
            ProcessStartInfo MPGinfo = new ProcessStartInfo("cmd", "/C " + cli);
            Process MPG = new Process();
            MPG.StartInfo = MPGinfo;
            MPG.StartInfo.CreateNoWindow = true;
            MPG.StartInfo.UseShellExecute = false;
            MPG.Start();
            MPG.WaitForExit();

        }

        public static void DVDMainMovieToMpeg(string DriveLetter, string outputFileMpg, bool Log, StreamWriter swWriteLog)
        {
            string MovieTitle = "";
            string vobFiles = "";
            string[] FileList = Directory.GetFiles(DriveLetter + ":", "*.*", SearchOption.AllDirectories);
            foreach (string f in FileList)
            {
                FileInfo FI = new FileInfo(Path.GetFullPath(f));
                long fSize = FI.Length;
                if (fSize > 1000000000)
                {
                    MovieTitle = Path.GetFileNameWithoutExtension(f).Substring(4, 2);
                    break;
                }
            }
            MeedioUtils.LogEntry(Log, swWriteLog, "[DVDUtils] - Main movie title: " + MovieTitle);

            foreach (string f in FileList)
            {
                if (Path.GetFileName(f).Contains("VTS_" + MovieTitle) && Path.GetExtension(f).ToLower() == (".vob"))
                {
                    if (!Path.GetFileName(f).ToLower().EndsWith("0.vob"))
                    {
                        MeedioUtils.LogEntry(Log, swWriteLog, "[DVDUtils] - Processing: " + f);
                        vobFiles = vobFiles + "\"" + Path.GetFullPath(f) + "\"" + " + ";
                    }
                }
            }           
            vobFiles = vobFiles.Remove(vobFiles.LastIndexOf("+")).Trim();
            MeedioUtils.LogEntry(Log, swWriteLog, "[DVDUtils] - Vob file list: " + vobFiles);
            string cli = "copy /B " + vobFiles + " \"" + outputFileMpg + "\"";
            MeedioUtils.LogEntry(Log, swWriteLog, "[DVDUtils] - MPG command line: " + cli);
            ProcessStartInfo MPGinfo = new ProcessStartInfo("cmd", "/C " + cli);
            Process MPG = new Process();
            MPG.StartInfo = MPGinfo;
            MPG.StartInfo.CreateNoWindow = true;
            MPG.StartInfo.UseShellExecute = false;
            MPG.Start();
            MPG.WaitForExit();
        }
    }
}