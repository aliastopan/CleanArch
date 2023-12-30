using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CleanArch.Domain.Aggregates.Identity;
using CleanArch.Domain.Enums;

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

        builder.Property(u => u.UserRoles)
            .HasColumnName("user_roles")
            .HasConversion(
                x => string.Join(',', x.Select(role => role.ToString())),
                x => x.Split(',', StringSplitOptions.RemoveEmptyEntries)
                      .Select(roleString => Enum.Parse<UserRole>(roleString))
                      .ToList())
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

        builder.Property(u => u.LastSignedIn)
            .HasColumnType("last_signed_in")
            .IsRequired();

        // foreign key
        builder.Property(u => u.FkUserId)
            .HasColumnName("fk_user_id")
            .IsRequired();

        builder.Property(u => u.FkUserProfileId)
            .HasColumnName("fk_user_profile_id")
            .IsRequired();

        // configure relationships
        builder.HasOne(u => u.User)
            .WithOne()
            .HasForeignKey<User>(u => u.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(u => u.UserProfile)
            .WithOne()
            .HasForeignKey<UserProfile>(u => u.UserProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.RefreshTokens)
            .WithOne(rt => rt.UserAccount)
            .HasForeignKey(rt => rt.FkUserAccountId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}
