using GovAuthSDK.Helpers;
using GovAuthSDK.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GovAuthSDK.Configurations
{
    internal class UserConfiguration: IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Login).HasColumnName("Login");
            builder.Property(x => x.PasswordHashed).HasColumnName("Password");
            builder.Property(x => x.AccessLevel);

            builder.HasData(new User("admin", HashHelper.HashPassword("admin")));
        }
    }
}
