using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;

namespace Rozkaz.Models
{
  public class RozkazDatabaseContext : DbContext
  {
    public RozkazDatabaseContext(DbContextOptions<RozkazDatabaseContext> options) : base(options)
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

      modelBuilder.Entity<OrderEntry>()
      .Property(e => e.Order)
      .HasConversion(
          v => JsonConvert.SerializeObject(v),
          v => JsonConvert.DeserializeObject<OrderModel>(v));
    }

    public DbSet<User> Users { get; set; }

    public DbSet<OrderEntry> Orders { get; set; }
  }
}
