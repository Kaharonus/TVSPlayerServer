using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using Avalonia.Media;
using Newtonsoft.Json;

namespace TVSPlayerServer
{
    static class DatabaseFiles{


        /// <summary>
        /// Directory of database. Ends with \\.
        /// </summary>
        public static string Database { get; } = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\TVSPlayerServer\\";

        public static string Read(string path) {
            path = Database + path;
            CreateDataDir();
            if (!File.Exists(path)) File.Create(path).Dispose();
            do {
                try {
                    if (!Directory.Exists(Path.GetDirectoryName(path))) {
                        Directory.CreateDirectory(Path.GetDirectoryName(path));
                    }
                    StreamReader sr = new StreamReader(path);
                    string text = sr.ReadToEnd();
                    sr.Close();
                    try {
                        object obj = JsonConvert.DeserializeObject(text);
                    } catch (Exception e) {

                    }
                    return text;
                } catch (IOException e) {
                    if (Settings.LoggingLevel == 2) {
                        ConsoleLog.WriteLine(e.Message, Brushes.Red);
                    }
                    Thread.Sleep(10);
                }
                Thread.Sleep(10);
            } while (true);
        }

        public static void Write(string path, object obj) {
            path = Database + path;
            CreateDataDir();
            if (!File.Exists(path)) File.Create(path).Dispose();
            string json = ObjectToJson(obj);
            do {
                try {
                    if (!Directory.Exists(Path.GetDirectoryName(path))) {
                        Directory.CreateDirectory(Path.GetDirectoryName(path));
                    }
                    File.Copy(path, path + "temp", true);
                    StreamWriter sw = new StreamWriter(path);
                    sw.Write(json);
                    sw.Close();
                    return;
                } catch (IOException e) {
                    if (Settings.LoggingLevel == 2) {
                        ConsoleLog.WriteLine(e.Message, Brushes.Red);
                    }
                    Thread.Sleep(10);
                }
            } while (true);
        }

        private static string ObjectToJson(object obj) {
            return JsonConvert.SerializeObject(obj);
        }

        public static void CreateDataDir() {
            string dir = Database;
            if (!Directory.Exists(dir)) {
                Directory.CreateDirectory(dir);
                if (Environment.OSVersion.Platform == PlatformID.Unix) {
                    //Code to enable all user access on Unix platforms
                } else if (Environment.OSVersion.Platform == PlatformID.Win32NT) {
                    //Code to enable all user access on Win platforms
                }
            }
            
        }
    }
}
