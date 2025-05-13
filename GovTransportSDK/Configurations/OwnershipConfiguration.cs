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

            builder.HasMany(x => x.TransportsHistory).WithOne(x => x.Ownership);

            builder.HasData(new List<Ownership>()
            {
                new Ownership()
                {
                    Id = Guid.Parse("6f5c6765-1b7b-4f87-b871-2ef875e67e07"),
                    FullName = "Петров Петр Петрович",
                    IsLegal = false,
                    Osago = "7189671298",
                    Passport = "6433 629663",
                    RegistrationAddress = "Г.Орск Ул.Пушкина 4, Кв. 1"
                },
                new Ownership()
                {
                    Id = Guid.Parse("a8eaf1bb-c5ef-4b61-921f-6c0b2b93e121"),
                    FullName = "Иванов Иван Иванович",
                    IsLegal = false,
                    Osago = "92874823316",
                    Passport = "7544 730774",
                    RegistrationAddress = "Г.Орск Ул.Колотушкина 5, Кв. 20"
                },
                new Ownership()
                {
                    Id = Guid.Parse("d840c96f-2c4f-4a10-9f5e-3dc936947d88"),
                    FullName = "Сидорова Ольга Ивановна",
                    IsLegal = true,
                    Osago = "84726872393",
                    Passport = "4211 407441",
                    RegistrationAddress = "Г.Орск Пр. Мира 1, Кв. 4"
                }
            });
        }
    }
}
