using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using MeediOS;



namespace EMAD.ToolBox
{
    public class MeedioUtils
    {
        public static void LogEntry(bool writeLog, StreamWriter swWriteLog, string Message)
        {
            if (writeLog == true)
            {
                swWriteLog.WriteLine(DateTime.Now.ToString() + ":    " + Message);
                swWriteLog.Flush();
            }
        }

        public static void CloseLog()
        {
            StreamWriter swWriteLog = null;
            swWriteLog.Close();
        }

        public static string GetPluginDataDir(string PluginName, string PluginType)
        {
            String CurDir = GetPluginPath(PluginName, PluginType);
            if (!Directory.Exists(Path.Combine(CurDir, "data")))
                Directory.CreateDirectory(Path.Combine(CurDir, "data"));
            string DataDir = Path.Combine(CurDir, "data");
            return DataDir;
        }

        public static StreamWriter SetDebugLog(string PluginName, string PluginType)
        {
            String CurDir = GetPluginPath(PluginName, PluginType);
            string filePath = Path.Combine(CurDir, "data\\log.txt");
            if (!File.Exists(filePath))
            {
                if (!Directory.Exists(Path.Combine(CurDir, "data")))
                    Directory.CreateDirectory(Path.Combine(CurDir, "data"));
            }
            else File.Delete(filePath);
            StreamWriter swWriteLog = File.CreateText(filePath);
            Regex version = new Regex("<plugin-version>(?<version>[^<]+)");
            string plugin = File.ReadAllText(Path.Combine(CurDir, PluginName + ".plugin"));
            LogEntry(true, swWriteLog, "***** " + PluginName + " Begin Log v " + version.Matches(plugin)[0].Groups["version"].Value + " *****");
            return swWriteLog;
        }

        public static string GetPluginPath(string PluginName, string PluginType)
        {
            Microsoft.Win32.RegistryKey key1 = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("Software\\Meedio\\Meedio Essentials");
            if (key1 != null && key1.GetValue("plugins") != null)
            {
                string path = key1.GetValue("plugins").ToString();
                if (path.Trim().EndsWith("\\") == false)
                    path = path.Trim() + "\\";
                path += PluginType + "\\" + PluginName + "\\";
                if (Directory.Exists(path) == false)
                    Directory.CreateDirectory(path);
                return path;
            }
            return "";
        }

        public static string GetMeedioPath()
        {
            Microsoft.Win32.RegistryKey key1 = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("Software\\Meedio\\Meedio Essentials");
            if (key1 != null && key1.GetValue("root") != null)
            {
                string path = key1.GetValue("root").ToString();
                if (path.Trim().EndsWith("\\") == false)
                    path = path.Trim() + "\\";
                return path;
            }
            return "";
        }

        public static void ShowOnScreenKeyboard(IMeedioSystem MeedioSystem, IMeedioMessage Message, string Title, string PreText, bool Log, StreamWriter swWriteLog)
        {
            try
            {
                Message = MeedioSystem.NewMessage("osk.show");
                Message["text"] = PreText;
                Message["required"] = true;
                Message["title"] = Title;
                Message.Send();
            }
            catch (Exception e)
            {
                
                LogEntry(Log, swWriteLog, "[MeedioUtils] - Error launching on screen keyboard." + e);
            }
            return;
        }

        public static void ShowDialogBox(IMeedioSystem MeedioSystem, IMeedioMessage Message, string caption, string description, string dialogtype, string yescaption, string nocaption, bool Log, StreamWriter swWriteLog)
        {
            try
            {
                Message = MeedioSystem.NewMessage("dialogbox.show");
                Message["caption"] = caption;
                Message["description"] = description;
                //Message["height"] = "500";
                //Message["width"] = "500";
                //Message["align"] = "center";
                Message["dialog-type"] = dialogtype; // "yes/no" or "okcancel" or "information" or blank for OK button only
                if (dialogtype == "yes/no" && yescaption != "" && nocaption != "")
                {
                    Message["yes-caption"] = yescaption;
                    Message["no-caption"] = nocaption;
                }
                Message.Send();
                LogEntry(Log, swWriteLog, "[MeedioUtils] - Launching dialog box");
            }
            catch (Exception e)
            {
                LogEntry(Log, swWriteLog, "[MeedioUtils] - Error launching dialog box." + e);
            }
            return;
        }

        public static void ShowPopUpScreen(string PluginDirectory, IMeedioSystem MeedioSystem, IMeedioMessage Message, string caption, double Width, double Height, string HAlign, string VAlign, Int32 Time)
        {
            if (VAlign != "")
            {
                string PopUpScreenPath = Path.Combine(PluginDirectory, "PopUp.screen");
                IMeedioScreen PopUp = MeedioSystem.NewPopupScreen(PopUpScreenPath, null, Width, Height, HAlign, VAlign);
                PopUp.Data["caption"] = caption;
                PopUp.AutoCloseTime = Time;
                PopUp.Show();
            }
        }

        public static void RunImports(string importModule, IMeedioSystem MeedioSystem, bool Log, StreamWriter swWriteLog)
        {
            try
            {
                LogEntry(Log, swWriteLog, "[MeedioUtils] - Launching import module: " + importModule.ToString());
                var moduleProps = MeedioSystem.NewItem();
                moduleProps["section"] = importModule;
                moduleProps["progress"] = "True";
                MeedioSystem.LaunchModule("{82D8A05A-0503-477B-ACEE-3AC8AFBC8AA3}", moduleProps);
            }
            catch (Exception e)
            {
                LogEntry(Log, swWriteLog, "[MeedioUtils] - Error running importer: " + e.ToString());
            }
        }

        public class FocusMeedio
        {
            [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
            private static extern System.IntPtr FindWindowByCaption(int ZeroOnly, string lpWindowName);

            [DllImport("user32.dll")]
            private static extern bool SetForegroundWindow(IntPtr hWnd);

            public static void SetFocusToMeedio()
            {
                string caption = "Meedio Essentials";
                if (System.IO.File.Exists((GetMeedioPath() + "\\brand\\brand.ini")))
                {
                    StreamReader reader = new StreamReader(GetMeedioPath() + "\\brand\\brand.ini");
                    string content = reader.ReadToEnd();
                    reader.Close();
                    reader.Dispose();
                    if (content != string.Empty)
                    {
                        string[] sp = { System.Environment.NewLine.ToString() };
                        foreach (string entry in content.Split(sp, StringSplitOptions.RemoveEmptyEntries))
                        {
                            if (entry.StartsWith("title="))
                            {
                                caption = entry.Split('=')[1].ToString();
                                break;
                            }
                        }
                    }
                    content = string.Empty;
                }

                System.Diagnostics.Process[] p = System.Diagnostics.Process.GetProcessesByName(caption);
                if (p.Length > 0)
                {
                    SetForegroundWindow(p[0].MainWindowHandle);
                }
            }

        }
    }
}