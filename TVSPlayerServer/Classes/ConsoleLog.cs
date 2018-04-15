using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Media;
using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace TVSPlayerServer
{
    public class ConsoleLog {

        public static StackPanel Log { get; set; }
        public static ScrollViewer ScrollView { get; set; }
        private static Timer t = new Timer(500);
        private static bool scrollDown = false;
        private static IBrush CustomWhite = Brush.Parse("#F5F5F5");
    
        public ConsoleLog(StackPanel log, ScrollViewer viewer) {
            Log = log;
            ScrollView = viewer;
            WriteLineGUI("Welcome to TVS-Player Server with GUI. To run wihout GUI start with parameter \"-DisableGUI\". Right now it's useless so fuck off.");
            t.Elapsed += (s, ev) => {
                Dispatcher.UIThread.InvokeAsync(() => {
                    if (scrollDown) { 
                        var maxProperty = ScrollView.GetValue(ScrollViewer.VerticalScrollBarMaximumProperty);
                        ScrollView.SetValue(ScrollViewer.OffsetProperty, new Avalonia.Vector(0, maxProperty + 100));
                        scrollDown = false;
                    }
                });
            };
            t.Start();      
        }

        private static void WriteLineGUI(string text, ISolidColorBrush color = null) {
            if (Log != null) {
                TextBlock block = new TextBlock {
                    Foreground = color == null ? CustomWhite : color,
                    TextWrapping = TextWrapping.Wrap,
                    FontSize = 14
                };
                block.Text = "> " + text;
                scrollDown = true;
                Log.Children.Add(block);
            }
        }

        public static void WriteLine(string text, ISolidColorBrush color = null) {
            //Task.Run(() => {
            text = DateTime.Now.ToString("HH:mm:ss yyyy/MM/dd") + ": " + text;
            Dispatcher.UIThread.Post(() => {
                WriteLineGUI(text, color);
            });

            Console.WriteLine(text);
            //});
        }

    }
}
