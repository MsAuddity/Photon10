using Microsoft.EntityFrameworkCore;

namespace Photon10.Data
{
    public class PlayerDbContext : DbContext
    {
        public PlayerDbContext(DbContextOptions options)
            : base(options)
        { }

        public DbSet<Models.Player> Players { get; set; }
    }
}
