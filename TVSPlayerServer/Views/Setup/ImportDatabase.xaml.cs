using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using System;
using System.IO;
using System.Threading.Tasks;

namespace TVSPlayerServer.Views.Setup {
    public class ImportDatabase : UserControl {
        public ImportDatabase() {
            this.InitializeComponent();
        }

        INameScope nameScope;
        TextBox ImportInput;
        StackPanel ResultPanel;

        private void InitializeComponent(){
            AvaloniaXamlLoader.Load(this);
            nameScope = this.FindNameScope();
            ImportInput = (TextBox)nameScope.Find("ImportInput");
            ResultPanel = (StackPanel)nameScope.Find("ResultPanel");
            ImportInput.TextInput += (s, ev) => LoadDirectory();
            ImportInput.DoubleTapped += async (s, ev) => {
                OpenFolderDialog ofd = new OpenFolderDialog();
                var res = await ofd.ShowAsync();
                if (!String.IsNullOrEmpty(res)) {
                    ImportInput.Text = res;
                    LoadDirectory();
                }
            };
        }

        private async void LoadDirectory() {
            var text = ImportInput.Text;
            if (!String.IsNullOrEmpty(text) && Directory.Exists(text)) {
                ResultPanel.Children.Clear();
                await Task.Run(() => {
                    var dirs = Directory.GetDirectories(text);
                    foreach (var dir in dirs) {
                        var ser = Series.SearchSingle(Path.GetFileName(dir));
                        if (ser != null) {
                            Dispatcher.UIThread.Post(async () => {
                                TextBlock block = new TextBlock();
                                block.Foreground = Brushes.White;
                                block.Text = "Name: " + ser.SeriesName + ", ID: " + ser.Id + ", Location: " + dir;
                                block.Height = 45;
                                block.FontSize = 16;
                                block.Opacity = 0;
                                ResultPanel.Children.Add(block);
                                await Task.Delay(25);
                                Anim.FadeIn(block);
                            });
                        }

                    }
                });

            }
        }
    }
}
