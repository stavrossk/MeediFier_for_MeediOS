using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Diagnostics;
using System.Text;
using MediaFairy.ToolBox;

namespace ToolBox
{
    class TsMuxeR
    {
        public static void runTSMuxer(string DriveLetter, string MovieName, string DataDirectory, string MovieDirectory, string Eac3toPath, string TsMuxerPath, bool DownConvertAudio, bool PreferLPCM, string OutputFormat, bool Log, StreamWriter swWriteLog)
        {
            string PreferredLanguage = CultureInfo.CurrentUICulture.EnglishName;
            PreferredLanguage = PreferredLanguage.Remove(PreferredLanguage.IndexOf("(")).Trim();
            MeedioUtils.LogEntry(Log, swWriteLog, "[TSMUXER] - Language: " + PreferredLanguage);

            String eac3toLog = Path.Combine(DataDirectory, "eac3to_LogFile.txt");
            String TsmuxerMetaFile = Path.Combine(DataDirectory, "TsmuxerMetaFile.meta");
            String Drive = DriveLetter + ":";

            //Run eac3to and save log file
            MeedioUtils.LogEntry(Log, swWriteLog, "[TSMUXER] - Creating eac3to log file.");
            ProcessStartInfo cdinfo = new ProcessStartInfo("cmd", "/C " + Eac3toPath + " " + Drive + " -log=\"" + eac3toLog + "\"");
            MeedioUtils.LogEntry(Log, swWriteLog, "[TSMUXER] - Eac3To command line: /C " + Eac3toPath + " " + Drive + " -log=\"" + eac3toLog + "\"");
            Process cd = new Process();
            cd.StartInfo = cdinfo;
            cd.StartInfo.CreateNoWindow = true;
            cd.StartInfo.UseShellExecute = false;
            cd.Start();
            cd.WaitForExit();

            //Identify longest playlist
            string line = "";
            string tempPlayList = "";
            string timeString = "";
            DateTime tempTime;
            DateTime finalPlayListTime = new DateTime();
            string longestPlaylist = "";
            using (StreamReader reader = new StreamReader(eac3toLog, Encoding.GetEncoding("iso-8859-1")))
            {
                for (int i = 1; i < 20; i++)
                {
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.Trim().StartsWith(i.ToString()))
                        {
                            tempPlayList = line.Substring(0, 13);
                            timeString = line.Substring(line.Length - 7, 7);
                            tempTime = Convert.ToDateTime(timeString);
                            if (tempTime > finalPlayListTime)
                            {
                                finalPlayListTime = tempTime;
                                longestPlaylist = i.ToString();
                            }
                        }
                    }
                }
                MeedioUtils.LogEntry(Log, swWriteLog, "[TSMUXER] - Longest Playlist: " + longestPlaylist);
            }

            //Identify details of longest playlist
            string fPlayListNoExt = "";
            string fM2TSFile = "";
            string videoCodec = "";
            string video1 = "";
            string audio1 = "";
            string VorAorS = "";
            string[] lines = File.ReadAllLines(eac3toLog);
            int k = 100;

            //Record eac3Log to Log File
            foreach (string e in lines)
            {
                MeedioUtils.LogEntry(Log, swWriteLog, "[EAC3TO LOG FILE] - " + e);
            }

            //SET TSREMUXER VIDEO LINE
            bool multiFile = false;
            foreach (string L in lines)
            {
                try
                {
                    k++;
                    if (L.Trim().StartsWith(longestPlaylist))
                    {
                        fPlayListNoExt = GetPlaylistFile(Log, swWriteLog, L);
                        MeedioUtils.LogEntry(Log, swWriteLog, "[TSMUXER] - Playlist: " + fPlayListNoExt);
                        fM2TSFile = GetM2TSFile(Log, swWriteLog, L, Drive);
                        if (fM2TSFile != "")
                            MeedioUtils.LogEntry(Log, swWriteLog, "[TSMUXER] - M2TS File: " + fM2TSFile);
                        k = 1;
                    }
                    if (L.Trim().Contains("[") && k == 2)
                    {
                        fM2TSFile = GetM2TSFile(Log, swWriteLog, L, Drive);
                        MeedioUtils.LogEntry(Log, swWriteLog, "[TSMUXER] - M2TS File: " + fM2TSFile);
                        k = 1;
                        multiFile = true;
                    }
                    if (multiFile == false)
                    {
                        if (L.Trim().StartsWith("- MPEG2") && k == 2)
                        {
                            VorAorS = "V";
                            videoCodec = "MPEG-2";
                            video1 = (VorAorS + "_" + videoCodec + ", " + fM2TSFile + ", track=4113, mplsFile=" + fPlayListNoExt);
                            break;
                        }
                        if (L.Trim().StartsWith("- VC-1") && k == 2)
                        {
                            VorAorS = "V";
                            videoCodec = "MS/VFW/WVC1";
                            video1 = (VorAorS + "_" + videoCodec + ", " + fM2TSFile + ", track=4113, mplsFile=" + fPlayListNoExt);
                            break;
                        }
                        if (L.Trim().StartsWith("- h264/AVC") && k == 2)
                        {
                            VorAorS = "V";
                            videoCodec = "MPEG4/ISO/AVC";
                            video1 = (VorAorS + "_" + videoCodec + ", " + fM2TSFile + ", track=4113, mplsFile=" + fPlayListNoExt);
                            break;
                        }
                    }
                    if (multiFile == true)
                    {
                        if (L.Trim().StartsWith("- MPEG2") && k == 2)
                        {
                            VorAorS = "V";
                            videoCodec = "MPEG-2";
                            video1 = (VorAorS + "_" + videoCodec + ", " + fM2TSFile + ", insertSEI, contSPS, track=4113, mplsFile=" + fPlayListNoExt);
                            break;
                        }
                        if (L.Trim().StartsWith("- VC-1") && k == 2)
                        {
                            VorAorS = "V";
                            videoCodec = "MS/VFW/WVC1";
                            video1 = (VorAorS + "_" + videoCodec + ", " + fM2TSFile + ", insertSEI, contSPS, track=4113, mplsFile=" + fPlayListNoExt);
                            break;
                        }
                        if (L.Trim().StartsWith("- h264/AVC") && k == 2)
                        {
                            VorAorS = "V";
                            videoCodec = "MPEG4/ISO/AVC";
                            video1 = (VorAorS + "_" + videoCodec + ", " + fM2TSFile + ", insertSEI, contSPS, track=4113, mplsFile=" + fPlayListNoExt);
                            break;
                        }
                    }
                }
                catch (Exception e)
                {
                    MeedioUtils.LogEntry(Log, swWriteLog, "[TSMUXER] - Error creating video line: " + e.ToString());
                }
            }

            //SET TSREMUXER AUDIO LINE
            k = 100;
            bool audioSet = false;
            string audioFormat = "";
            string audioLanguage = "";
            String[] eac3AudioCodecs = { "- RAW/PCM, " + PreferredLanguage, "- TrueHD, " + PreferredLanguage, "- DTS Master Audio, " + PreferredLanguage, "- DTS, " + PreferredLanguage, "- AC3, " + PreferredLanguage, "- RAW/PCM", "- TrueHD", "- DTS Master Audio", "- DTS", "- AC3" }; 
            foreach (string s in eac3AudioCodecs)
            {
                if (audioSet == false)
                {
                    foreach (string L in lines)
                    {
                        try
                        {
                            k++;
                            if (L.Trim().StartsWith(longestPlaylist))
                            {
                                fPlayListNoExt = GetPlaylistFile(Log, swWriteLog, L);
                                fM2TSFile = GetM2TSFile(Log, swWriteLog, L, Drive);
                                k = 1;
                            }
                            if (L.Trim().Contains("[") && k == 2)
                            {
                                fM2TSFile = GetM2TSFile(Log, swWriteLog, L, Drive);
                                k = 1;
                            }   
                            if (L.Trim().StartsWith(s) && (k == 3 || k == 4 || k == 5 || k == 6 || k == 7)) //split code by , and search each string
                            {
                                audio1 = SetAudioLine(L, Drive, fM2TSFile, fPlayListNoExt, DownConvertAudio, PreferLPCM);
                                audioSet = true;
                                audioFormat = GetAudioFormat(L);
                                audioLanguage = GetLanguage(L);
                                break;
                            }
                        }
                        catch (Exception e)
                        {
                            MeedioUtils.LogEntry(Log, swWriteLog, "[TSMUXER] - Error creating audio line: " + e.ToString());
                        }
                    }
                }
                if (audioSet == true)
                {
                    break;
                }
            }

            //Create Meta file for TSRemux
            String Meta1 = "";
            if (OutputFormat == "M2TS")
            {
                Meta1 = "MUXOPT --no-pcr-on-video-pid --new-audio-pes --vbr  --vbv-len=500";
            }
            if (OutputFormat == "BRStructure")
            {
                Meta1 = "MUXOPT --no-pcr-on-video-pid --new-audio-pes --blu-ray --vbr  --auto-chapters=5 --vbv-len=500";
            }
            String Meta2 = video1;
            String Meta3 = audio1;
            using (StreamWriter swBR = new StreamWriter(TsmuxerMetaFile))
            {
                try
                {
                    swBR.WriteLine(Meta1);
                    swBR.WriteLine(Meta2);
                    swBR.WriteLine(Meta3);
                    swBR.Close();
                    swBR.Dispose();
                }
                catch (Exception e)
                {
                    MeedioUtils.LogEntry(Log, swWriteLog, "[TSMUXER] - Error writing meta file: " + e.ToString());
                }
            }

            //Prepare Movie name for tsMuxeR command line
            String CleanMovieName = ToolBox.StringFunctions.replaceIllegalCharacters(MovieName, " ");
            MovieName = CleanMovieName;

            //Create movie directory
            String TempMovieDir = Path.Combine(DataDirectory, MovieName);
            Directory.CreateDirectory(TempMovieDir);

            //Run tsMuxeR  
            MeedioUtils.LogEntry(Log, swWriteLog, "[TSMUXER] - Video format: " + videoCodec);
            MeedioUtils.LogEntry(Log, swWriteLog, "[TSMUXER] - Audio format: " + audioFormat);
            MeedioUtils.LogEntry(Log, swWriteLog, "[TSMUXER] - Audio language: " + audioLanguage);
            MeedioUtils.LogEntry(Log, swWriteLog, "[TSMUXER] - Starting TsMuxeR");
            if (OutputFormat == "M2TS")
            {
                String ContainerFile = Path.Combine(TempMovieDir, MovieName + ".m2ts");
                Process ts = new Process();
                ts.StartInfo.FileName = TsMuxerPath;
                ts.StartInfo.Arguments = "\"" + TsmuxerMetaFile + "\" \"" + ContainerFile + "\"";
                ts.StartInfo.CreateNoWindow = true;
                ts.StartInfo.UseShellExecute = false;
                ts.Start();
                ts.WaitForExit();
                return;
            }
            if (OutputFormat == "BRStructure")
            {
                Process ts = new Process();
                ts.StartInfo.FileName = TsMuxerPath;
                ts.StartInfo.Arguments = "\"" + TsmuxerMetaFile + "\" \"" + TempMovieDir + "\"";
                ts.StartInfo.CreateNoWindow = true;
                ts.StartInfo.UseShellExecute = false;
                ts.Start();
                ts.WaitForExit();
                return;
            }
        }     

        //Get three letter country code
        private static string ThreeLetterLanguage(string Language)
        {
            foreach (CultureInfo ci in CultureInfo.GetCultures(CultureTypes.NeutralCultures))
            {
                if (ci.EnglishName == Language)
                    return ci.ThreeLetterISOLanguageName.ToString().ToLower();
            }
            return "";
        }

        //Get playlist file
        private static string GetPlaylistFile(bool Log, StreamWriter swWriteLog, string Line)
        {
            String Playlist = "";
            try
            {                
                string TrimedLine = Line.Substring(3, Line.Length - 3).Trim();
                string[] split = TrimedLine.Split(new Char[] { ',' });
                foreach (string s in split)
                {
                    if (s.Trim().EndsWith(".mpls"))
                    {
                        Playlist = s.Substring(0, s.Length - 5);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                MeedioUtils.LogEntry(Log, swWriteLog, "[TSMUXER] - Playlist Error: " + e.ToString());
            }
            return Playlist.Trim();
        }

        //Get M2TS file
        private static string GetM2TSFile(bool Log, StreamWriter swWriteLog, string Line, string DriveLetter)
        {
            String M2TSFile = "";
            try
            {                
                if (Line.Contains("["))
                {
                    int S = Line.IndexOf('['); //end index
                    int E = Line.IndexOf(']'); //start index
                    string m2tsfiles = Line.Substring(S + 1, E - (S + 1));
                    MeedioUtils.LogEntry(Log, swWriteLog, "[TSMUXER] - M2TS Files: " + m2tsfiles);
                    string[] split = m2tsfiles.Split(new Char[] { '+' });
                    StringBuilder sb = new StringBuilder();
                    foreach (string num in split)
                    {
                        string num2 = num.PadLeft(5, '0');
                        sb.Append("\"" + DriveLetter + "\\BDMV\\STREAM\\" + num2 + ".m2ts\"+");
                        M2TSFile = sb.ToString().Trim();
                        string M2 = M2TSFile.TrimEnd(new Char[] { '+' });
                        M2TSFile = M2;
                    }            
                }
                else
                {
                    string TrimedLine = Line.Substring(3, Line.Length - 3).Trim();
                    string[] split = TrimedLine.Split(new Char[] { ',' });
                    foreach (string s in split)
                    {
                        if (s.Trim().EndsWith(".m2ts"))
                        {
                            M2TSFile = s.Trim();
                            String M2 = ("\"" + DriveLetter + "\\BDMV\\STREAM\\" + M2TSFile + "\"");
                            M2TSFile = M2;
                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MeedioUtils.LogEntry(Log, swWriteLog, "[TSMUXER] - M2TS File Error: " + e.ToString());
            }
            return M2TSFile.Trim();
        }

        //Set audio line
        private static string SetAudioLine(string Line, string DriveLetter, string M2TSFile, string PlayList, bool DownConvertAudio, bool PreferLPCM)
        {
            string VorAorS = "A";
            string audioLine = "";

            //set audiolanguage
            string audioLanguage = "";
            string[] split = Line.Split(new Char[] { ',' });
            foreach (string s in split)
            {
                audioLanguage = ThreeLetterLanguage(s.Trim());
                if (audioLanguage != "")
                {
                    break;
                }
            }

            //set audio codec
            int P = Line.IndexOf(','); //end index
            int D = Line.IndexOf('-'); //start index
            string audioCodec = Line.Substring(D + 2, P - (D + 2));
            if (PreferLPCM == true)
            {
                if (audioCodec == "RAW/PCM")
                {
                    audioCodec = "LPCM"; 
                }
            }
            if (audioCodec == "DTS Master Audio")
            {
                audioCodec = "DTS";
                if (DownConvertAudio == true)
                {
                    audioLine = (VorAorS + "_" + audioCodec + ", " + M2TSFile + ", down-to-dts, track=4352, lang=" + audioLanguage + ", mplsFile=" + PlayList);
                }
                if (DownConvertAudio == false)
                {
                    audioLine = (VorAorS + "_" + audioCodec + ", " + M2TSFile + ", track=4352, lang=" + audioLanguage + ", mplsFile=" + PlayList);
                }
                return audioLine;
            }
            if (audioCodec == "TrueHD")
            {
                audioCodec = "AC3";
                if (DownConvertAudio == true)
                {
                    audioLine = (VorAorS + "_" + audioCodec + ", " + M2TSFile + ", down-to-ac3, track=4352, lang=" + audioLanguage + ", mplsFile=" + PlayList);
                }
                if (DownConvertAudio == false)
                {
                    audioLine = (VorAorS + "_" + audioCodec + ", " + M2TSFile + ", track=4352, lang=" + audioLanguage + ", mplsFile=" + PlayList);
                }
                return audioLine;
            }  
            audioLine = (VorAorS + "_" + audioCodec + ", " + M2TSFile + ", track=4352, lang=" + audioLanguage + ", mplsFile=" + PlayList);
            return audioLine;
        }

        //Get Audio Format
        private static string GetAudioFormat(string Line)
        {
            //set audio codec
            int P = Line.IndexOf(','); //end index
            int D = Line.IndexOf('-'); //start index
            string audioCodec = Line.Substring(D + 2, P - (D + 2));
            return audioCodec;
        }

        //Get Langauage
        private static string GetLanguage(string Line)
        {
            string audioLanguage = "";
            string[] split = Line.Split(new Char[] { ',' });
            foreach (string s in split)
            {
                audioLanguage = ThreeLetterLanguage(s.Trim());
                if (audioLanguage != "")
                {
                    break;
                }
            }
            return audioLanguage;
        }
                
    }
}
