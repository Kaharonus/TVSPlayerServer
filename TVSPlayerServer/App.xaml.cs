using Avalonia;
using Avalonia.Markup.Xaml;

namespace TVSPlayerServer
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
