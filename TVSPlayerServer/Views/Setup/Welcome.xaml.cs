using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using System;
using System.Threading.Tasks;

namespace TVSPlayerServer.Views.Setup
{
    public class Welcome : UserControl
    {
        public Welcome()
        {
            this.InitializeComponent();
        }

        INameScope nameScope;
        TextBox DirectoryInput;
        TextBox DownloadInput;
        TextBox ExtraInput1;
        TextBox ExtraInput2;
        TextBox ExtraInput3;
        Button ImportButton;
        Button FinishButton;

        Image Logo;



        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            nameScope = this.FindNameScope();
            DirectoryInput = (TextBox)nameScope.Find("DirectoryInput");
            DownloadInput = (TextBox)nameScope.Find("DownloadInput");
            ExtraInput1 = (TextBox)nameScope.Find("ExtraInput1");
            ExtraInput2 = (TextBox)nameScope.Find("ExtraInput2");
            ExtraInput3 = (TextBox)nameScope.Find("ExtraInput3");
            ImportButton = (Button)nameScope.Find("ImportButton");
            FinishButton = (Button)nameScope.Find("FinishButton");
            Logo = (Image)nameScope.Find("Logo");

            Logo.Source = new Bitmap("Images/logo.png");
            DirectoryInput.DoubleTapped += Handler;
            DownloadInput.DoubleTapped += Handler;
            ExtraInput1.DoubleTapped += Handler;
            ExtraInput2.DoubleTapped += Handler;
            ExtraInput3.DoubleTapped += Handler;
            ImportButton.Click += ImportHandler;
            FinishButton.Click += FinishHandler;
        }

        private async void ImportHandler(object sender, RoutedEventArgs e) {
            
        }
        private async void FinishHandler(object sender, RoutedEventArgs e) {

        }

        private async void Handler(object sender, RoutedEventArgs e) {
            OpenFolderDialog dia = new OpenFolderDialog();
            var result = await dia.ShowAsync();
            if (!string.IsNullOrEmpty(result)) {
                ((TextBox)sender).Text = result;
            }
        }

    }
}
