using ChatApiApplication.Model;
using Microsoft.EntityFrameworkCore;

namespace ChatApiApplication.Data
{
    public class ChatAPIDbContext : DbContext
    {
        public ChatAPIDbContext(DbContextOptions options) : base(options) { }
        public DbSet<Users> Users { get; set; }

        public DbSet<Messages> Messages { get; set; }
    }
}
