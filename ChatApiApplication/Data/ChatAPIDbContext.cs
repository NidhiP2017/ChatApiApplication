using ChatApiApplication.Model;
using Microsoft.EntityFrameworkCore;

namespace ChatApiApplication.Data
{
    public class ChatAPIDbContext : DbContext
    {
        public ChatAPIDbContext(DbContextOptions options) : base(options) { }
        public DbSet<ChatUsers> ChatUsers { get; set; }
        public DbSet<Messages> Messages { get; set; }
        public DbSet<Log> Logs { get; set; }
    }
}
