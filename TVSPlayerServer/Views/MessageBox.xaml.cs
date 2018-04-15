using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Threading.Tasks;

namespace TVSPlayerServer.Views
{
    public class MessageBox : UserControl
    {
        public MessageBox(string header, string message) {
            this.InitializeComponent();
            var scope = this.FindNameScope();
            ((TextBlock)scope.Find("Header")).Text = header;
            ((TextBlock)scope.Find("Body")).Text = message;
            ((Button)scope.Find("Cancel")).Click += (s, ev) => { result = true; };
        }

        private static bool? result = null;

        public static async Task<bool> Show(string header, string message) {
            View.AddView(new MessageBox(header, message));
            await Task.Run( async () => {
                while (result == null) {
                    await Task.Delay(100);
                }
            });
            var local = result;
            result = null;
            View.RemoveView();
            return (bool)local;
        }


        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
