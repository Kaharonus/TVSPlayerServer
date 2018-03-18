using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TVSPlayerServer.Views
{
    public class Torrents : UserControl
    {
        public Torrents()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
