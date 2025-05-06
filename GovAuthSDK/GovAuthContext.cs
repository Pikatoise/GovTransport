using GovAuthSDK.Configurations;
using GovAuthSDK.Models;
using Microsoft.EntityFrameworkCore;

namespace GovAuthSDK
{
    internal sealed class GovAuthContext: DbContext
    {
        private readonly string _dbPath = "auth.db";

        public DbSet<User> Users { get; set; }
        public DbSet<Token> Tokens { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={_dbPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new TokenConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
