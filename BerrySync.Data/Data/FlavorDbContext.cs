using BerrySync.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BerrySync.Data.Data
{
    public class FlavorDbContext : DbContext
    {
        public FlavorDbContext(DbContextOptions<FlavorDbContext> options) : base(options)
        {
        }

        public DbSet<FlavorOfTheDay> Dates { get; set; }
        public DbSet<CalendarEvent> CalendarEvents { get; set; }
        public DbSet<CalendarImage> CalendarImages { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=/data/db/sqlite.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FlavorOfTheDay>()
                .HasOne(f => f.Event)
                .WithOne(e => e.FlavorOfTheDay)
                .HasForeignKey<CalendarEvent>(f => f.Date);
        }
    }
}
