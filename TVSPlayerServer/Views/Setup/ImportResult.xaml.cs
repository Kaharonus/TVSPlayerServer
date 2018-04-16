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
        Grid Base;
        public string SeriesName { get; set; }
        public string DirPath { get; set; }
        public string ReleaseDate { get; set; }
        public Button Edit;
        public Button Remove;
        public Button Details;


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

        }
        private void ParseSeries(Series series) {
            SeriesName = series.SeriesName;
            ReleaseDate = Helper.FormatDate(series.FirstAired);
        }
    }
}
