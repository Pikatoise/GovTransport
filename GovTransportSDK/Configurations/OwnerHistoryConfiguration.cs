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

            builder.HasOne(x => x.Transport).WithMany(x => x.OwnersHistory).HasForeignKey(x => x.TransportId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.Ownership).WithMany(x => x.TransportsHistory).HasForeignKey(x => x.OwnershipId).OnDelete(DeleteBehavior.SetNull);

            builder.HasData(new List<OwnerHistory>
            {
                new OwnerHistory()
                {
                    Id = Guid.Parse("9b4b8a7d-6d30-4b0f-8c9a-c0ef089a4e45"),
                    Start = new DateTime(2020, 3, 13),
                    End = new DateTime(2022, 5, 1),
                    OwnershipId = Guid.Parse("6f5c6765-1b7b-4f87-b871-2ef875e67e07"),
                    TransportId = Guid.Parse("4e7f5e6f-3f1b-4f49-b5b6-657a88b7e87e")
                },
                new OwnerHistory()
                {
                    Id = Guid.Parse("6f6dd9bc-4c18-4f84-996a-6c2b6e6f2cd6"),
                    Start = new DateTime(2022, 6, 24),
                    OwnershipId = Guid.Parse("6f5c6765-1b7b-4f87-b871-2ef875e67e07"),
                    TransportId = Guid.Parse("f36763c9-0b0b-48d1-9562-2be3e7bffcae")
                }
            });
        }
    }
}
