using BackEndCaro.DTO;
using BackEndCaro.Models;
using BackEndCaro.Models;
using Microsoft.EntityFrameworkCore;

namespace BackEndCaro
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserRoom> UserRooms { get; set; }
    }
}
