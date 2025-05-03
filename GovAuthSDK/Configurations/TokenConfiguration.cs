using GovAuthSDK.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GovAuthSDK.Configurations
{
    internal class TokenConfiguration: IEntityTypeConfiguration<Token>
    {
        public void Configure(EntityTypeBuilder<Token> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.AuthToken).HasColumnName("AuthToken");
            builder.Property(x => x.AccessLevel);
        }
    }
}
