using GovTransportSDK.Enums;
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

            builder.HasMany(x => x.OwnersHistory).WithOne(x => x.Transport);

            builder.HasData(new List<Transport>
            {
                new Transport()
                {
                    Id = Guid.Parse("4e7f5e6f-3f1b-4f49-b5b6-657a88b7e87e"),
                    VIN = "4DRBWAFN06A207518",
                    Model = "Toyota Camry",
                    ReleaseYear = 2000,
                    Color = "White",
                    GovNumber = "А101МР56",
                    Status = TransportStatus.Ok,
                    BodyType = BodyType.Sedan
                },
                new Transport()
                {
                    Id = Guid.Parse("f36763c9-0b0b-48d1-9562-2be3e7bffcae"),
                    VIN = "JT2BF22K6Y0283641",
                    Model = "Lexus IS250",
                    ReleaseYear = 2007,
                    Color = "Black",
                    GovNumber = "М536МР56",
                    Status = TransportStatus.Ok,
                    BodyType = BodyType.Sedan
                }
            });
        }
    }
}
