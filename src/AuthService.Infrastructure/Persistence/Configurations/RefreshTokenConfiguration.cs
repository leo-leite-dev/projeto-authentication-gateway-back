using AuthService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Infrastructure.Persistence.Configurations;

public sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("refresh_tokens");

        builder.HasKey(rt => rt.Id);

        builder.Property(rt => rt.Id).ValueGeneratedNever();

        builder.Property(rt => rt.Token).IsRequired();

        builder.Property(rt => rt.ExpiresAt).IsRequired();

        builder.Property(rt => rt.CreatedAt).IsRequired();

        builder.Property(rt => rt.IsRevoked).IsRequired();

        builder.HasIndex(rt => rt.Token).IsUnique();

        builder.HasOne(rt => rt.User).WithMany(u => u.RefreshTokens).HasForeignKey(rt => rt.UserId);
    }
}
