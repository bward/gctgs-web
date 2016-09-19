using Microsoft.EntityFrameworkCore;

namespace GctgsWeb.Models
{
    public class GctgsContext : DbContext
    {
        public GctgsContext(DbContextOptions<GctgsContext> options)
            : base(options)
        { }

        public DbSet<BoardGame> BoardGames { get; set; }
    }
}
