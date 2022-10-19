using System.Threading.Tasks;
using System.Windows;
using MarioMaker2Overlay.Persistence;
using Microsoft.EntityFrameworkCore;


namespace MarioMaker2Overlay
{
    public partial class App : Application 
    {
        public void AppStartup(object sender, StartupEventArgs e)
        {
            InitializeDatabase()
                .GetAwaiter();

            MainWindow mainwindow = new();

            mainwindow.Show();
        }

        private async Task InitializeDatabase()
        {
            using (MarioMaker2OverlayContext context = new())
            {
                if (!(await context.Player.AnyAsync()))
                {
                    await context.Player.AddAsync(new Player { PlayerName = "Player 1" });
                    await context.SaveChangesAsync();
                }

                context.Database.Migrate();
            }

            // If there are no defined players, create one



            //// check if the __EFMigrationsHistory exists, if not add it
            //// with the initial Migration row included
            //using (DbConnection myConnection = context.Database.GetDbConnection())
            //using (DbCommand myCommand = myConnection.CreateCommand())
            //{
            //    myConnection.Open();

            //    myCommand.CommandText = "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='__EFMigrationsHistory';";

            //    if (int.TryParse(myCommand!.ExecuteScalar()?.ToString(), out int count))
            //    {
            //        if (count == 0)
            //        {
            //            context.Database.ExecuteSqlRaw(EmbeddedResourceUtility.GetEmbeddedResourceContentAsString("Chocobit.Shared.Persistence.ManuallyCreateEFMigrationsHistory.sql"));
            //            context.Database.ExecuteSqlRaw("INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion) VALUES ('20220610023851_InitialMigration', '6.0.5')");
            //        }
            //    }

            //    myConnection.Close();
            //}

        }
    }
}
