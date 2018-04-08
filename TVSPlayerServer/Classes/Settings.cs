using Avalonia.Media;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;

namespace TVSPlayerServer
{
    static class Settings {
        private static int _SecurityLevel = 1;
        private static int _LoggingLevel = 1;
        private static string _token;
        private static DateTime _tokenTimestamp;
        private static bool _setupComplete = false;

        /// <summary>
        /// Security level of application from 0 (least secure) to 2 (most secure)
        /// Right now main difference is request MAC checking. 0 = no checks, 1 = every +- 10 minutes, 2 = every time
        /// </summary>
        public static int SecurityLevel { get { return _SecurityLevel; } set { _SecurityLevel = value; SaveSettings(); } }

        /// <summary>
        /// How much info is logged to console
        /// 0 = Only basic logs (servers starts, closes, exceptions), 1 = Normal info also tells you about who requested what, 2 = EVERYTHING I CAN THINK OF 
        /// </summary>
        public static int LoggingLevel { get { return _LoggingLevel; } set { _LoggingLevel = value; SaveSettings(); } }

        /// <summary>
        /// Token for logging in into TheTVDB API
        /// </summary>
        public static string Token { get => _token; set { _token = value; SaveSettings(); } }

        /// <summary>
        /// Date time that stores time when token was last retrieved
        /// </summary>
        public static DateTime TokenTimestamp { get => _tokenTimestamp; set { _tokenTimestamp = value; SaveSettings(); } }

        /// <summary>
        /// Indicates if user has already passed the entire setup process
        /// </summary>
        public static bool SetupComplete { get => _setupComplete; set { _setupComplete = value; SaveSettings(); } }


        /// <summary>
        /// Saves Settings. Is called automatically whenever property value is changed
        /// </summary>
        public static void SaveSettings() {
            Type type = typeof(Settings);
            string filename = DatabaseFiles.Database + "Settings.tvsps";
            if (!Directory.Exists(DatabaseFiles.Database)) {
                Directory.CreateDirectory(DatabaseFiles.Database);
            }
            if (!File.Exists(filename)) {
                File.Create(filename).Dispose();
            }
            do {
                try {
                    FieldInfo[] properties = type.GetFields(BindingFlags.Static | BindingFlags.NonPublic);
                    object[,] a = new object[properties.Length, 2];
                    int i = 0;
                    foreach (FieldInfo field in properties) {
                        a[i, 0] = field.Name;
                        a[i, 1] = field.GetValue(null);
                        i++;
                    };
                    string json = JsonConvert.SerializeObject(a);
                    StreamWriter sw = new StreamWriter(filename);
                    sw.Write(json);
                    sw.Close();
                    return;
                } catch (IOException e) {
                    if (LoggingLevel == 2) {
                        ConsoleLog.WriteLine(e.Message, Brushes.Red);
                    }
                    Thread.Sleep(15);
                }
            } while (true);
        }
        /// <summary>
        /// Loads settings with default value if new settings has been added. In case of enums edit code - might get "crashy" if you dont
        /// </summary>
        public static void Load() {
            Type type = typeof(Settings);
            string filename = DatabaseFiles.Database + "Settings.tvsps";
            if (!Directory.Exists(DatabaseFiles.Database)) {
                Directory.CreateDirectory(DatabaseFiles.Database);
            }
            if (!File.Exists(filename)) {
                File.Create(filename).Dispose();
            }
            do {
                try {
                    FieldInfo[] fields = type.GetFields(BindingFlags.Static | BindingFlags.NonPublic);
                    object[,] a;
                    StreamReader sr = new StreamReader(filename);
                    string json = sr.ReadToEnd();
                    sr.Close();
                    if (!String.IsNullOrEmpty(json)) {
                        JArray ja = JArray.Parse(json);
                        a = ja.ToObject<object[,]>();
                        if (a.GetLength(0) != fields.Length) { }
                        int i = 0;
                        foreach (FieldInfo field in fields) {
                            try {
                                if (field.Name == (a[i, 0] as string)) {
                                    field.SetValue(null, Convert.ChangeType(a[i, 1], field.FieldType));
                                }
                            } catch (IndexOutOfRangeException) {
                                field.SetValue(null, GetDefault(field.FieldType));
                            }
                            i++;
                        };
                    }
                    return;
                } catch (IOException e) {
                    if (LoggingLevel == 2) {
                        ConsoleLog.WriteLine(e.Message, Brushes.Red);
                    }
                    Thread.Sleep(15);
                }
            } while (true);
        }

        public static object GetDefault(Type type) {
            if (type.IsValueType) {
                return Activator.CreateInstance(type);
            }
            return null;
        }

    }
}

