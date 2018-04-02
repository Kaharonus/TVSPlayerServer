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
        static void Main(string[] args)
        {
            bool testRun = true;
            foreach (var arg in args) {
                switch (arg.ToLower()) {
                    case "-disablegui":
                        IsUIEnabeled = false;
                        break;
                }
            }
            User.LoadUsers();
            if (IsUIEnabeled) {
                BuildAvaloniaApp().Start<MainWindow>();
            } else {
                if (testRun) {
                    TestMethod();
                }
                Console.WriteLine("Mode without GUI is not implemented yet. Press any key to exit");
                Console.ReadKey();
            }
        }


        public static void TestMethod() {
            var api = new MediaServer(8080,@"D:/");
            api.Start();
            Console.ReadKey();
        }


        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToDebug();
    }
}
