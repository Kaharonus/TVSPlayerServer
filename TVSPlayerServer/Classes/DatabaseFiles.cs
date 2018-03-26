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

        /// <summary>
        /// Reads and parses file to desired type. Return value can be anything just make sure its same as specified type
        /// Takes care of automatic backups. If something would be truly f*cked it would let you know.
        /// </summary>
        /// <typeparam name="T">Type you want returned</typeparam>
        /// <param name="path">Path to file in the Database directory. Without extension</param>
        /// <returns></returns>
        public static dynamic Read<T>(string path) {
            path = Database + path;
            do {
                try {
                    string text = Read(path);                  
                    var obj = JsonConvert.DeserializeObject(text,typeof(T));
                    obj = obj ?? Activator.CreateInstance<T>();
                    return obj;
                } catch (JsonSerializationException ex) {
                    if (File.Exists(path + ".tvspstemp")) {
                        File.Delete(path + ".tvsps");
                        File.Copy(path + ".tvspstemp", path + ".tvsps");
                    } else {
                        ConsoleLog.WriteLine(ex.Message, Brushes.Red);
                        ConsoleLog.WriteLine("File" + path + ".tvsps cannot be recovered", Brushes.Red);
                        return new object();
                    }
                }
            } while (true);

        }

        private static string Read(string path) {
            path += ".tvsps";
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
                    return text;
                } catch (IOException e) {
                    if (Settings.LoggingLevel == 2) {
                        ConsoleLog.WriteLine(e.Message, Brushes.Red);
                    }
                    Thread.Sleep(10);
                }
            } while (true);
        }

        /// <summary>
        /// Prases object to string and writes it to a file. Auto backups on write 
        /// </summary>
        /// <param name="path">Path in Database. Without extension</param>
        /// <param name="obj">Any object that will be parsed</param>
        public static void Write(string path, object obj) {
            path = Database + path + ".tvsps";
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
