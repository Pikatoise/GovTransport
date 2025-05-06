using GovTransportSDK.Configurations;
using GovTransportSDK.Models;
using Microsoft.EntityFrameworkCore;

namespace GovTransportSDK
{
    internal class GovTransportContext: DbContext
    {
        private readonly string _dbPath = "transport.db";

        public DbSet<Transport> Transports { get; set; }
        public DbSet<Ownership> Owners { get; set; }
        public DbSet<OwnerHistory> OwnerHistories { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={_dbPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new OwnershipConfiguration());
            modelBuilder.ApplyConfiguration(new TransportConfiguration());
            modelBuilder.ApplyConfiguration(new OwnerHistoryConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
