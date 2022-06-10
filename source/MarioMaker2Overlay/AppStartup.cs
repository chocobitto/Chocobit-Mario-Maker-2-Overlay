using MarioMaker2Overlay.Persistence;
using MarioMaker2Overlay.Utility;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data.Common;
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

            // check if the __EFMigrationsHistory exists, if not add it
            // with the initial Migration row included
            using (DbConnection myConnection = context.Database.GetDbConnection())
            using (DbCommand myCommand = myConnection.CreateCommand())
            {
                myConnection.Open();

                myCommand.CommandText = "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='__EFMigrationsHistory';";

                if (int.TryParse(myCommand!.ExecuteScalar()?.ToString(), out int count))
                {
                    if (count == 0)
                    {
                        context.Database.ExecuteSqlRaw(EmbeddedResourceUtility.GetEmbeddedResourceContentAsString("MarioMaker2Overlay.Persistence.ManuallyCreateEFMigrationsHistory.sql"));
                        context.Database.ExecuteSqlRaw("INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion) VALUES ('20220610023851_InitialMigration', '6.0.5')");
                    }
                }

                myConnection.Close();
            }

            context.Database.Migrate();
        }
    }
}
