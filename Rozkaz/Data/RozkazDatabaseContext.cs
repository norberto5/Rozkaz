using Microsoft.EntityFrameworkCore;

namespace Rozkaz.Models
{
    public class RozkazDatabaseContext : DbContext
    {
        public RozkazDatabaseContext (DbContextOptions<RozkazDatabaseContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    }
}
