using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TVSPlayerServer.Views
{
    public class SelectSeriesResult : UserControl {
        public SelectSeriesResult(Series series) {
            this.InitializeComponent();
            Series = series;
            SeriesName = series.SeriesName;
        }

        public string SeriesName { get; set; }
        public Series Series { get; set; }

        private void InitializeComponent()
        {
            this.DataContext = this;
            AvaloniaXamlLoader.Load(this);
        }
    }
}
