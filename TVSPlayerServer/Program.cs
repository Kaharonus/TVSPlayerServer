using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Logging.Serilog;
using TVSPlayerServer.Classes.Database;

namespace TVSPlayerServer
{
    class Program
    {
        static bool IsUIEnabeled { get; set; } = true;
        static bool TestRun { get; set; } = false;
        public static AppBuilder BuildAvaloniaApp() => AppBuilder.Configure<App>().UsePlatformDetect().LogToDebug();

        static async Task Main(string[] args) {
            //AppDomain.CurrentDomain.UnhandledException += (s, ev) => ExceptionCought(ev);
            AppDomain.CurrentDomain.ProcessExit += (s, ev) => Database.SaveDatabase();
            ParseLaunchParameters(args);
            await LoadData();
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

        private async static Task LoadData() {           
            if (!TestRun) {
                Settings.Load();
                await User.LoadUsers();
                await Database.LoadDatabase();
                if (IsUIEnabeled) {
                    BuildAvaloniaApp().Start<MainWindow>();
                } else {
                    Console.WriteLine("Mode without GUI is not implemented yet. Press any key to exit. Please don't close this app by the X sign");
                    Console.ReadLine();
                }
            } else {
                await TestMethod();
            }
        }

        private static void ExceptionCought(UnhandledExceptionEventArgs args) {
            Database.SaveDatabase();
            var ex = (Exception)args.ExceptionObject;
            ConsoleLog.WriteLine(ex.Message);
            ConsoleLog.WriteLine(ex.StackTrace);
        }

        public async static Task TestMethod() {
            Settings.Load();
            await User.LoadUsers();
            await Database.LoadDatabase();
            API api = new API(8080);
            api.Start();
            Console.ReadKey();
        }
        
    }
}
