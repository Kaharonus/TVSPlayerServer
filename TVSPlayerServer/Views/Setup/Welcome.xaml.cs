using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TVSPlayerServer.Views.Setup
{
    public class Welcome : UserControl
    {
        public Welcome()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
