using MarioMaker2Overlay.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Windows;


namespace MarioMaker2Overlay
{
    public partial class App : Application 
    {
        public void AppStartup(object sender, StartupEventArgs e)
        {
            InitializeDatabase();

            MainWindow mainwindow = new();

            mainwindow.Show();
        }

        private void InitializeDatabase()
        {
            MarioMaker2OverlayContext context = new();
            context.Database.Migrate();
        }
    }
}
