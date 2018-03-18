using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TVSPlayerServer.Views
{
    public class Configuration : UserControl
    {
        public Configuration()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
