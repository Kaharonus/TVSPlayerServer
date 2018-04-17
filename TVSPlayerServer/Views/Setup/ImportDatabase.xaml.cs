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
        Button Return;
        StackPanel ResultPanel;

        private void InitializeComponent(){
            AvaloniaXamlLoader.Load(this);
            nameScope = this.FindNameScope();
            ImportInput = (TextBox)nameScope.Find("ImportInput");
            ResultPanel = (StackPanel)nameScope.Find("ResultPanel");
            Return = (Button)nameScope.Find("Return");
            Return.Click += (s, ev) => View.RemoveView();
            ImportInput.TextInput += (s, ev) => LoadDirectory();
            ImportInput.PointerReleased += async (s, ev) => {
                if (ev.MouseButton == Avalonia.Input.MouseButton.Right) { 
                    OpenFolderDialog ofd = new OpenFolderDialog();
                    var res = await ofd.ShowAsync();
                    if (!String.IsNullOrEmpty(res)) {
                        ImportInput.Text = res;
                        LoadDirectory();
                }
                }
            };
        }

        private async void LoadDirectory() {
            var text = ImportInput.Text;
            if (!String.IsNullOrEmpty(text) && Directory.Exists(text)) {
                ResultPanel.Focus();
                ResultPanel.Children.Clear();
                await Task.Run(() => {
                    var dirs = Directory.GetDirectories(text);
                    foreach (var dir in dirs) {
                        var ser = Series.SearchSingle(Path.GetFileName(dir));
                        if (ser != null) {
                            Dispatcher.UIThread.Post(async () => {
                                var result = new ImportResult(ser,dir);
                                result.Remove.Click += async (s, ev) =>  {
                                    await Anim.FadeOut(result);
                                    ResultPanel.Children.Remove(result);
                                };
                                result.Edit.Click += async (s, ev) => {
                                    var res = await SelectSeries.Show();
                                };
                                ResultPanel.Children.Add(result);
                                Anim.FadeIn(result);
                            });
                        }

                    }
                });

            }
        }
    }
}
