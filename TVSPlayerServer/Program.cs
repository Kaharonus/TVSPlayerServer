using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Logging.Serilog;
using TVSPlayerServer.Classes.Database;

namespace TVSPlayerServer
{
    class Program
    {
        static bool IsUIEnabeled { get; set; } = false;
        static bool TestRun { get; set; } = true;
        //public static AppBuilder BuildAvaloniaApp() => AppBuilder.Configure<App>().UsePlatformDetect().LogToDebug();

        static void Main(string[] args) {
            AppDomain.CurrentDomain.UnhandledException += (s, ev) => Database.SaveDatabase();
            AppDomain.CurrentDomain.ProcessExit += (s, ev) => Database.SaveDatabase();
            ParseLaunchParameters(args);
            LoadData();
        }

        private static void ParseLaunchParameters(string[] args) {
            foreach (var arg in args) {
                switch (arg.ToLower()) {
                    case "-disablegui":
                        IsUIEnabeled = false;
                        break;
                }
            }
        }

        private async static void LoadData() {
            Settings.Load();
            await User.LoadUsers();
            await Database.LoadDatabase();
            if (!TestRun) {
                if (IsUIEnabeled) {
                    //BuildAvaloniaApp().Start<MainWindow>();
                } else {
                    Console.WriteLine("Mode without GUI is not implemented yet. Press any key to exit");
                    Console.ReadKey();
                }
            } else {
                TestMethod();
            }
        }

        public static void TestMethod() {
            API api = new API(8080);
            api.Start();
            Console.ReadKey();
        }
        
    }
}
