using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace TVSPlayerServer
{
    static class DatabaseFiles{

        public static string Database { get; } = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

        public static string Read(string path) {
            string test = Database;
            ConsoleLog.WriteLine(test);
            return "";
            /*if (!Directory.Exists(Path.GetDirectoryName(path))) Directory.CreateDirectory(Path.GetDirectoryName(path));
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
                } catch (IOException e) { }
                Thread.Sleep(10);
            } while (true);*/
        }

        public static void Write(string path, string json) {
            /*if (!Directory.Exists(Path.GetDirectoryName(path))) Directory.CreateDirectory(Path.GetDirectoryName(path));
            if (!File.Exists(path)) File.Create(path).Dispose();
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
                } catch (IOException e) { }
                Thread.Sleep(10);

            } while (true);*/
        }
    }
}
