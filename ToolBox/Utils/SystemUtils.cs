using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Runtime.InteropServices;
using System.Threading;
using DirectShowLib;

namespace ToolBox
{
    public class SystemUtils
    {
        public static void WaitForFileToStopGrowing(string FilePath, Int32 interval)
        {
            string FP = Path.GetFullPath(FilePath);
            long FileSize1 = (new FileInfo(FP)).Length;
            Thread.Sleep(interval);
            long FileSize2 = (new FileInfo(FP)).Length;

            while (FileSize1 == 0)
            {
                Thread.Sleep(interval);
                FileSize1 = (new FileInfo(FP)).Length;
                Thread.Sleep(interval);
                FileSize2 = (new FileInfo(FP)).Length;
            }

            while (FileSize1 != FileSize2)
            {
                Thread.Sleep(interval);
                FileSize1 = (new FileInfo(FP)).Length;
                Thread.Sleep(interval);
                FileSize2 = (new FileInfo(FP)).Length;
            }

            if (FileSize1 == FileSize2)
            {
                Thread.Sleep(interval);
                FileSize1 = (new FileInfo(FP)).Length;
                Thread.Sleep(interval);
                FileSize2 = (new FileInfo(FP)).Length;
            }

            while (FileSize1 != FileSize2)
            {
                Thread.Sleep(interval);
                FileSize1 = (new FileInfo(FP)).Length;
                Thread.Sleep(interval);
                FileSize2 = (new FileInfo(FP)).Length;
            }

        }

        public static void CloseApplication(string applicationName)
        {
            Process[] processes = Process.GetProcessesByName(applicationName);
            foreach (Process p in processes)
            {
                p.CloseMainWindow();
            }
        }

        public static class NativeMethods
        {
            [DllImport("user32.dll")]
            private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

            [DllImport("user32.dll")]
            private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);


            public static void SetWindowVisibility(bool visible, string title)
            {
                if (string.IsNullOrEmpty(title))
                    throw new ArgumentNullException("title");

                IntPtr hWnd = FindWindow(null, title);
                
                if (hWnd == IntPtr.Zero)
                    return;

                if (!visible)                            
                    ShowWindow(hWnd, 0);                      
                else
                    ShowWindow(hWnd, 1);       

            }

            public static void WaitForWindow(string title)
            {
                if (string.IsNullOrEmpty(title))
                    throw new ArgumentNullException("title");
                IntPtr hWnd = IntPtr.Zero;                        
                while (hWnd == IntPtr.Zero)
                {
                    try
                    {
                        hWnd = NativeMethods.FindWindow(null, title);
                    }
                    catch
                    {
                    }
                }
            }
        }

    }

}