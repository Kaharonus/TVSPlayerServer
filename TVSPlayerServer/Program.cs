using System;
using System.Linq;
using Avalonia;
using Avalonia.Logging.Serilog;

namespace TVSPlayerServer
{
    class Program
    {
        static bool IsUIEnabeled { get; set; } = true;
        static void Main(string[] args)
        {
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
                Console.WriteLine("Mode without GUI is not implemented yet. Press any key to exit");
                Console.ReadKey();
            }
        }
  

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToDebug();
    }
}
