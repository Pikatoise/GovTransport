using GovTransportSDK.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GovTransportSDK.Configurations
{
    internal class OwnershipConfiguration: IEntityTypeConfiguration<Ownership>
    {
        public void Configure(EntityTypeBuilder<Ownership> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.FullName);
            builder.Property(x => x.RegistrationAddress);
            builder.Property(x => x.Passport);
            builder.Property(x => x.Osago);
            builder.Property(x => x.IsLegal);

            builder.HasMany(x => x.TransportHistory).WithOne(x => x.Ownership);
        }
    }
}
