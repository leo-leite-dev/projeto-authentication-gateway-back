using AuthService.Domain.Entities;
using AuthService.Domain.ValueObjects;
using BaitaHora.Domain.Features.Common.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).ValueGeneratedNever();

        builder.Property(u => u.CreatedAt).IsRequired();
        builder.Property(u => u.Status).HasConversion<int>().IsRequired();

        builder
            .Property(u => u.Email)
            .HasConversion(email => email.Value, value => Email.Parse(value))
            .HasColumnName("email")
            .HasMaxLength(255)
            .IsRequired();

        builder
            .Property(u => u.Username)
            .HasConversion(username => username.Value, value => Username.Parse(value))
            .HasColumnName("username")
            .HasMaxLength(50)
            .IsRequired();

        builder
            .Property(u => u.PasswordHash)
            .HasConversion(hash => hash.Value, value => new PasswordHash(value))
            .HasColumnName("password_hash")
            .IsRequired();

        builder.HasIndex(u => u.Email).IsUnique();
        builder.HasIndex(u => u.Username).IsUnique();

        builder.HasMany(u => u.RefreshTokens).WithOne(rt => rt.User).HasForeignKey(rt => rt.UserId);
    }
}
