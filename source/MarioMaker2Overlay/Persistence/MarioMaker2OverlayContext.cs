using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MarioMaker2Overlay.Persistence
{
    class MarioMaker2OverlayContext : DbContext
    {
        public DbSet<LevelData> LevelData { get; set; }

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
