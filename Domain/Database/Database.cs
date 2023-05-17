using Contracts.Database;
using Microsoft.EntityFrameworkCore;

namespace Domain.Database
{
    public class TwitchBotDbContext : DbContext
    {
        public DbSet<Command> Commands { get; init; }
        public DbSet<SocialMedia> SocialMedias { get; init; }

        public TwitchBotDbContext() : base()
        {

        }

        public TwitchBotDbContext(DbContextOptions<TwitchBotDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }

        // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        // {
        //     _ = optionsBuilder.UseSqlite("Data Source=TwitchBotCommands.db");
        // }
    }
}