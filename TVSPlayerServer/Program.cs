using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Logging.Serilog;

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
            API api = new API(8080);
            api.Start();
            Console.ReadLine();
        }


        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToDebug();
    }
}
