using Microsoft.EntityFrameworkCore;
using System;

namespace Rozkaz.Models
{
    public class RozkazDatabaseContext : DbContext
    {
        public RozkazDatabaseContext (DbContextOptions<RozkazDatabaseContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UnitModel>()
            .Property(e => e.SubtextLines)
            .HasConversion(
                v => string.Join(';', v),
                v => v.Split(';', StringSplitOptions.RemoveEmptyEntries));
        }

        public DbSet<User> Users { get; set; }
    }
}
