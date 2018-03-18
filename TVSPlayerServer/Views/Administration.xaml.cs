using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TVSPlayerServer.Views
{
    public class Administration : UserControl
    {
        public Administration()
        {
            this.InitializeComponent();
            Loaded();
        }

        INameScope scope;
        Button FileStart;

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            scope = this.FindNameScope();
            FileStart = (Button)scope.Find("FileStart");
            FileStart.Click += (s, ev) => StartMediaServer();

        }

        private void Loaded() {

        }

        private void StartMediaServer() {
            if (Servers.Media == null) {
                Servers.LoadMedia(8080);
            }
            if (Servers.Media.IsRunning) {
                Servers.Media.Stop();
                FileStart.Content = "Start";
            } else {
                Servers.Media.Start();
                FileStart.Content = "Stop";
            }
        }
    }
}
