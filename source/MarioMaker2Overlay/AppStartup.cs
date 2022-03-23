using MarioMaker2Overlay.Persistence;
using System;
using System.IO;
using System.Windows;


namespace MarioMaker2Overlay
{
    public partial class App : Application 
    {
        public void AppStartup(object sender, StartupEventArgs e)
        {
            InitializeDataBase();

            MainWindow mainwindow = new();

            mainwindow.Show();
        }

        private void InitializeDataBase()
        {
            MarioMaker2OverlayContext context = new();
            context.Database.EnsureCreated();
        }
    }
}
