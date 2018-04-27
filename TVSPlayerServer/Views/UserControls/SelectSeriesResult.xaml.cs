using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

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
        public Button Select { get; set; }
        public Button Details { get; set; }

        private void InitializeComponent()
        {
            this.DataContext = this;
            AvaloniaXamlLoader.Load(this);
            var nameScope = this.FindNameScope();
            Select = (Button)nameScope.Find("Select");
            Details = (Button)nameScope.Find("Details");
            var Base = (Grid)nameScope.Find("Base");
            Base.PointerEnter += (s, ev) => { Base.Background = Brush.Parse("#444444"); };
            Base.PointerLeave += (s, ev) => { Base.Background = Brush.Parse("#333333"); };

        }
    }
}
