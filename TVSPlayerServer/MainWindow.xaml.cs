﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using TVSPlayerServer.Views;
using System.Windows.Input;

namespace TVSPlayerServer
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        INameScope nameScope;

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            //Init. of few important things
            nameScope = this.FindNameScope();
            new View((Grid)nameScope.Find("SwitchableContent"), (Grid)nameScope.Find("ContentOnTop"));
            new ConsoleLog((StackPanel)nameScope.Find("ConsoleLog"), (ScrollViewer)nameScope.Find("Scroll"));
            View.SetView(new Administration());
        }

        private void SetAdministration(object sender, RoutedEventArgs args) {
            var admin = new Administration();
            View.SetView(admin);
        }
        private void SetConfiguration(object sender, RoutedEventArgs args) {
            var conf = new Configuration();
            View.SetView(conf);
        }
        private void SetTorrents(object sender, RoutedEventArgs args) {
            var conf = new Torrents();
            View.SetView(conf);
        }
    }
}