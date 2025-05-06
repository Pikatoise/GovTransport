using GovTransportSDK.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GovTransportSDK.Configurations
{
    internal class TransportConfiguration: IEntityTypeConfiguration<Transport>
    {
        public void Configure(EntityTypeBuilder<Transport> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.VIN);
            builder.Property(x => x.Model);
            builder.Property(x => x.ReleaseYear);
            builder.Property(x => x.Color);
            builder.Property(x => x.GovNumber);
            builder.Property(x => x.Status);
            builder.Property(x => x.BodyType);

            builder.HasMany(x => x.OwnerHistory).WithOne(x => x.Transport);
        }
    }
}
