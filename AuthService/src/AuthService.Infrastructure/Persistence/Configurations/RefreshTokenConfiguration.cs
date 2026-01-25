using AuthService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Infrastructure.Persistence.Configurations;

public sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Token).IsRequired().HasMaxLength(512);

        builder.HasIndex(x => x.Token).IsUnique();

        builder.Property(x => x.UserId).IsRequired();

        builder.Property(x => x.CreatedAt).IsRequired();

        builder.Property(x => x.ExpiresAt).IsRequired();

        builder.Property(x => x.IsRevoked).IsRequired();

        builder
            .HasOne(x => x.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.UserId);
    }
}
