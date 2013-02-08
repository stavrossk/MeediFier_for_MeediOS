using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;

namespace ToolBox
{
    class CreateISO
    {
        public static void WriteISO(string AnyTool2Path, string DriveLetter, string DestinationFolder, string MovieName)
        {
            DriveLetter = "E";
            string DVDDrive = DriveLetter + ":";
            
            DestinationFolder = "C:\\ISOs";
            MovieName = "TEST" + ".iso";
            string Destination = Path.Combine(DestinationFolder, MovieName);

            string root = AnyTool2Path.Remove(3);
            string pathwithoutRoot = AnyTool2Path.Substring(3, AnyTool2Path.Length - 3);

            //AnyTool2 [enable][disable][-help][rip <Drive Letter> <Target>] [ripiso <Drive Letter> <Target>]



            string HBCLI = root + "\"" + pathwithoutRoot + "\" --burn-iso -device:0     ";                 //root + "\"" + pathwithoutRoot + "\"";
            //ToolBox.MeedioUtils.LogEntry(Log, swWriteLog, "[HandBrake] - Command line: " + HBCLI);

            //ToolBox.MeedioUtils.LogEntry(Log, swWriteLog, "[HandBrake] - Output: " + OutputDirectory2 + "\\" + FileTitle + ".mkv");
            //Launch Handbrake
            ProcessStartInfo HBinfo = new ProcessStartInfo("cmd", "/C " + HBCLI);
            Process hb = new Process();
            hb.StartInfo = HBinfo;
            hb.StartInfo.CreateNoWindow = false;
            hb.StartInfo.UseShellExecute = false;
            hb.Start();
            hb.WaitForExit();
            
            
        }
    }
}