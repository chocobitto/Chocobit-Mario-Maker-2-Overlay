using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace MarioMaker2Overlay.Persistence
{
    public class MarioMaker2OverlayContext : DbContext
    {
        public DbSet<LevelData> LevelData { get; set; }
        public DbSet<Player> Player { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("FileName=MarioMaker2OverlayDatabase.db",
                options => options.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName));
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
