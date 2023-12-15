using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CleanArch.Domain.Aggregates.Identity;

namespace CleanArch.Infrastructure.Persistence.Configurations.Identity;

internal sealed class UserAccountConfiguration : IEntityTypeConfiguration<UserAccount>
{
    public void Configure(EntityTypeBuilder<UserAccount> builder)
    {
        builder.ToTable("dbo.user_account");

        // primary key
        builder.HasKey(u => u.UserAccountId);

        // configure properties
        builder.Property(u => u.UserAccountId)
            .HasColumnName("user_account_id")
            .HasMaxLength(36)
            .IsRequired();

        builder.Property(u => u.UserRole)
            .HasColumnName("role")
            .IsRequired();

        builder.Property(u => u.IsVerified)
            .HasColumnName("is_verified")
            .IsRequired();

        builder.Property(u => u.PasswordHash)
            .HasColumnName("password_hash")
            .HasMaxLength(96) // SHA384 (48-byte)
            .IsRequired();

        builder.Property(u => u.PasswordSalt)
            .HasColumnName("password_salt")
            .HasMaxLength(8)
            .IsRequired();

        builder.Property(u => u.CreationDate)
            .HasColumnName("creation_date")
            .IsRequired();

        builder.Property(u => u.LastLoggedIn)
            .HasColumnType("last_logged_in")
            .IsRequired();

        // configure relationships
        builder.HasOne(u => u.User)
            .WithOne()
            .HasForeignKey<UserAccount>(u => u.UserAccountId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.RefreshTokens)
            .WithOne(rt => rt.UserAccount)
            .HasForeignKey(rt => rt.UserAccountId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}
