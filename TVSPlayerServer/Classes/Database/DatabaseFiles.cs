using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Media;
using Newtonsoft.Json;

namespace TVSPlayerServer
{
    static class DatabaseFiles{


        /// <summary>
        /// Directory of database. Ends with \\.
        /// </summary>
        public static string Database { get; } = ChooseDirectory();

        /// <summary>
        /// Reads and parses file to desired type. Return value can be anything just make sure its same as specified type
        /// Takes care of automatic backups. If something would be truly f*cked it would let you know.
        /// </summary>
        /// <typeparam name="T">Type you want returned</typeparam>
        /// <param name="path">Path to file in the Database directory. Without extension</param>
        /// <returns></returns>      
        public async static Task<string> Read(string path) {
            return await Task.Run(() => {
                path = Database + path + ".tvsps";
                if (!Directory.Exists(Path.GetDirectoryName(path))) {
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                }
                bool exists = File.Exists(path);
                if (!exists) {
                    var fs = File.Create(path);
                    fs.Dispose();
                }
                do {
                    try {
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
            });      
        }

        /// <summary>
        /// Prases object to string and writes it to a file. Auto backups on write 
        /// </summary>
        /// <param name="path">Path in Database. Without extension</param>
        /// <param name="obj">Any object that will be parsed</param>
        public async static Task Write(string path, object obj) {
            await Task.Run(() => {
                path = Database + path + ".tvsps";
                if (!Directory.Exists(Path.GetDirectoryName(path))) Directory.CreateDirectory(Path.GetDirectoryName(path));
                if (!File.Exists(path)) File.Create(path).Dispose();
                string json = JsonConvert.SerializeObject(obj);
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
            });          
        }


        public static string ChooseDirectory(){
            string dir = "";
            if (Environment.OSVersion.Platform == PlatformID.Unix){
                dir = "/usr/local/TVSPlayerServer/";           
            } else if (Environment.OSVersion.Platform == PlatformID.Win32NT) {
                dir = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments) + "\\TVSPlayerServer\\";
            }
            return dir;    
        }      
    }
}
