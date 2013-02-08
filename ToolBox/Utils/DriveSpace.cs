using System.Runtime.InteropServices;
using System.IO;

namespace ToolBox
{
    public sealed class DriveInfo
    {
        [DllImport("kernel32.dll", EntryPoint = "GetDiskFreeSpaceExA")]
        private static extern long GetDiskFreeSpaceEx(string lpDirectoryName,
            out long lpFreeBytesAvailableToCaller,
            out long lpTotalNumberOfBytes,
            out long lpTotalNumberOfFreeBytes);

        public static long GetInfo(string drive, out long available, out long total, out long free)
        {
            return GetDiskFreeSpaceEx(drive, out available, out total, out free);
        }

        public static DriveInfoSystem GetInfo(string drive)
        {
            long available, total, free;
            long result = GetDiskFreeSpaceEx(drive, out available, out total, out free);
            return new DriveInfoSystem(drive, result, available, total, free);
        }

        //IDENTIFY DIRECTORY WITH THE MOST SPACE
        public static string DirWithMostSpace(string[] dirList)
        {
            long space = 0;
            string selectedDir = string.Empty;

            foreach (string s in dirList)
            {
                string drive = Path.GetPathRoot(s);

                DriveInfoSystem info = GetInfo(drive);
                long tempspace = info.Available;

                if (tempspace <= space)
                    continue;

                space = tempspace;
                selectedDir = s;
            
            }
            return selectedDir;

        }



        public static long DirAvailbleSpace(string path)
        {
            string drive = Path.GetPathRoot(path);
            DriveInfoSystem info = GetInfo(drive);
            long space = info.Available;      
            return space;
        }



    }



    public struct DriveInfoSystem
    {
        public readonly string Drive;
        public readonly long Result;
        public readonly long Available;
        public readonly long Total;
        public readonly long Free;


        public DriveInfoSystem(string drive, long result, long available, long total, long free)
        {
            Drive = drive;
            Result = result;
            Available = available;
            Total = total;
            Free = free;
        }


    }    



}