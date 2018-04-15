using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using TVSPlayerServer.Views;
using System.Windows.Input;
using TVSPlayerServer.Views.Setup;
using Avalonia.Styling;

namespace TVSPlayerServer
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        INameScope nameScope;

        private void InitializeComponent(){
            AvaloniaXamlLoader.Load(this);
            //Init. of few important things
            nameScope = this.FindNameScope();
            Settings.Load();
            new View((Grid)nameScope.Find("SwitchableContent"), (Grid)nameScope.Find("ContentOnTop"));
            new ConsoleLog((StackPanel)nameScope.Find("ConsoleLog"), (ScrollViewer)nameScope.Find("Scroll"));
            View.SetView(new Administration());
            if (Settings.SetupComplete) {
                View.AddView(new Welcome());
            }
        }

        private void SetAdministration(object sender, RoutedEventArgs args) {
            View.SetView(new Administration());
        }
        private void SetConfiguration(object sender, RoutedEventArgs args) {
            View.SetView(new Configuration());
        }
        private void SetTorrents(object sender, RoutedEventArgs args) {
            View.SetView(new Torrents());
        }
        private async void SetInfo(object sender, RoutedEventArgs args) {
            await MessageBox.Show("Error", "You just created this superb bullcrap of a messagebox");
        }
    }
}
