using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TVSPlayerServer.Views
{
    public class Info : UserControl
    {
        public Info()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
