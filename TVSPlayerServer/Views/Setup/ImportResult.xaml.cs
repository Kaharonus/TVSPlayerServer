using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace TVSPlayerServer.Views.Setup
{
    public class ImportResult : UserControl
    {
        public ImportResult(Series series, string dir) {
            this.InitializeComponent();
            ParseSeries(series);
            DirPath = dir;
        }

        INameScope nameScope;
        public string DirPath { get; set; }
        public string ReleaseDate { get; set; }
        public Button Edit;
        public Button Remove;
        public Button Details;
        public Series Series { get; set; }
        Grid Base;

        private void InitializeComponent()
        {

            AvaloniaXamlLoader.Load(this);
            this.DataContext = this;
            nameScope = this.FindNameScope();
            Edit = (Button)nameScope.Find("Edit");
            Remove = (Button)nameScope.Find("Remove");
            Details = (Button)nameScope.Find("Details");
            Base = (Grid)nameScope.Find("Base");
            Base.PointerEnter += (s, ev) => { Base.Background = Brush.Parse("#444444"); };
            Base.PointerLeave += (s, ev) => { Base.Background = Brush.Parse("#333333"); };
            Edit.Click += async (s, ev) => {
                var res = await SelectSeries.Show();
                ParseSeries(res);
            };
        }
        private void ParseSeries(Series series) {
            Series = series;
            ((TextBlock)nameScope.Find("SeriesName")).Text = series.SeriesName;
            ((TextBlock)nameScope.Find("ReleaseDate")).Text = Helper.FormatDate(series.FirstAired);
        }
    }
}
