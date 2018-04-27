using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TVSPlayerServer.Views
{
    public class SelectSeries : UserControl {

        public SelectSeries() {
            this.InitializeComponent();
        }

        TextBox ImportInput;
        StackPanel ResultPanel;
        INameScope nameScope;
        static Series result = null;


        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
            nameScope = this.FindNameScope();
            ResultPanel = (StackPanel)nameScope.Find("ResultPanel");
            ImportInput = (TextBox)nameScope.Find("ImportInput");
            ImportInput.TextInput += (s, ev) => Search();
        }


        public async static Task<Series> Show() {
            View.AddView(new SelectSeries());
            await Task.Run(async () => {
                while (result == null) {
                    await Task.Delay(100);
                }
            });
            var res = result;
            result = null;
            View.RemoveView();
            return res;
        }

        private async void Search() {
            string name = ImportInput.Text;
            var result = await Task.Run(() => Series.Search(name));
            if (name == ImportInput.Text) {
                if (result != null) {
                    FillUI(result);
                }
            }
        }

        List<Series> oldList = new List<Series>();
        private void FillUI(List<Series> list) {
            Task.Run(async () => {
                if (list.Where(y => oldList.Any(z => z.Id == y.Id)).ToList().Count == 0) {
                    oldList = list;
                    Dispatcher.UIThread.Post(() => ResultPanel.Children.Clear(), DispatcherPriority.Send);
                    foreach (var series in list) {
                        Dispatcher.UIThread.Post(new Action(() => {
                            SelectSeriesResult ssr = new SelectSeriesResult(series);
                            ssr.Select.Click += (s, ev) => {
                                result = ssr.Series;
                            };
                            ResultPanel.Children.Add(ssr);
                            Anim.FadeIn(ssr);
                        }), DispatcherPriority.Send);
                        await Task.Delay(7);
                    }
                }
            });

        }
    }
}
