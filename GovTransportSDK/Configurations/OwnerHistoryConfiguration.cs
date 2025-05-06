using GovTransportSDK.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GovTransportSDK.Configurations
{
    internal class OwnerHistoryConfiguration: IEntityTypeConfiguration<OwnerHistory>
    {
        public void Configure(EntityTypeBuilder<OwnerHistory> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Start);
            builder.Property(x => x.End);

            builder.HasOne(x => x.Transport).WithMany(x => x.OwnerHistory).HasForeignKey(x => x.TransportId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.Ownership).WithMany(x => x.TransportHistory).HasForeignKey(x => x.OwnershipId).OnDelete(DeleteBehavior.SetNull);
        }
    }
}
