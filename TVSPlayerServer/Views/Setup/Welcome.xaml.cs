using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using Avalonia.Input;

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

            Logo.Source = new Bitmap("Images/Logo.png");
            DirectoryInput.PointerReleased += Handler;
            DownloadInput.PointerReleased += Handler;
            ExtraInput1.PointerReleased += Handler;
            ExtraInput2.PointerReleased += Handler;
            ExtraInput3.PointerReleased += Handler;
            ImportButton.Click += ImportHandler;
            FinishButton.Click += FinishHandler;
        }

        private async void ImportHandler(object sender, RoutedEventArgs e) {
            if (CheckDirectories()) {
                SaveSettings();
                View.AddView(new ImportDatabase());
            } else {
                await MessageBox.Show("Error", "You didn't enter correct data. \nPlease check the following:\nAll directories must be unique\nLibrary location must not be empty.");
            }
        }
        private async void FinishHandler(object sender, RoutedEventArgs e) {
            if (CheckDirectories()) {
                SaveSettings();
                Settings.SetupComplete = true;
                View.RemoveAllViews();
            } else {
                await MessageBox.Show("Error", "You didn't enter correct data.\n\nPlease check the following:\n- All directories must be unique\n- Library location must not be empty.");
            }
        }

        private void SaveSettings() {
            Settings.DownloadLocation = DownloadInput.Text;
            Settings.LibraryLocation = DirectoryInput.Text;
            Settings.ScanLocation1 = ExtraInput1.Text;
            Settings.ScanLocation2 = ExtraInput2.Text;
            Settings.ScanLocation3 = ExtraInput3.Text;
        }

        private bool CheckDirectories() {
            List<string> locations = new List<string>() {
                DirectoryInput.Text, DownloadInput.Text, ExtraInput1.Text, ExtraInput2.Text, ExtraInput3.Text
            };
            if (String.IsNullOrEmpty(DirectoryInput.Text)) {
                return false;
            }
            locations.Where(x => String.IsNullOrEmpty(x)).ToList().ForEach(x=> locations.Remove(x));
            foreach (var dir in locations) {
                if (!Directory.Exists(dir)) {
                    return false;
                }
            }
            for (int i = 0; i < locations.Count; i++) {
                for (int x = i + 1; x < locations.Count; x++) {
                    if ( locations[x] == locations[i]) {
                        return false;
                    }
                }
            }

            return true;
        }

        private async void Handler(object sender, PointerReleasedEventArgs e) {
            if (e.MouseButton == MouseButton.Right) { 
                OpenFolderDialog dia = new OpenFolderDialog();
                var result = await dia.ShowAsync();
                if (!string.IsNullOrEmpty(result)) {
                    ((TextBox)sender).Text = result;
                }
            }
        }

    }
}
