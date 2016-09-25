using GctgsWeb.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;

namespace GctgsWeb.Services
{
    public class GctgsContext : DbContext
    {
        public GctgsContext(DbContextOptions<GctgsContext> options)
            : base(options)
        { }

        public DbSet<BoardGame> BoardGames { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Request> Requests { get; set; }

        public bool IsAdmin(ClaimsPrincipal User)
        {
            return Users.Single(user => user.Crsid == User.Identity.Name).Admin;
        }
    }
}
