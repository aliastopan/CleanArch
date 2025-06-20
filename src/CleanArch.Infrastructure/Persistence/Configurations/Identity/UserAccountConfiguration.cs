using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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

        builder.ComplexProperty(u => u.User,
            user =>
            {
                user.IsRequired();

                user.Property(u => u.Username)
                    .HasColumnName("username")
                    .HasMaxLength(16)
                    .IsRequired();

                user.Property(u => u.EmailAddress)
                    .HasColumnName("email_address")
                    .HasMaxLength(255)
                    .IsRequired();

                user.Property(u => u.UserRole)
                    .HasColumnName("user_role")
                    .IsRequired();

                user.Property(u => u.UserPrivileges)
                    .HasColumnName("user_privileges")
                    .HasConversion(
                        x => string.Join(',', x.Select(privilege => privilege.ToString())),
                        x => x.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                .Select(privilegeStr => Enum.Parse<UserPrivilege>(privilegeStr))
                                .ToList(),
                        new ValueComparer<IReadOnlyCollection<UserPrivilege>>(
                            (c1, c2) => c1!.SequenceEqual(c2!),
                            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                            c => c.ToList()))
                    .IsRequired();
            });

        builder.Property(u => u.IsVerified)
            .HasColumnName("is_verified")
            .IsRequired();

        builder.Property(u => u.PasswordHash)
            .HasColumnName("password_hash")
            .HasMaxLength(96) // SHA384 (48-byte)
            .IsRequired();

        builder.Property(u => u.PasswordSalt)
            .HasColumnName("password_salt")
            .HasMaxLength(16)
            .IsRequired();

        builder.Property(u => u.CreationDate)
            .HasColumnName("creation_date")
            .IsRequired();

        builder.Property(u => u.LastSignedIn)
            .HasColumnName("last_signed_in")
            .IsRequired();

        // foreign key
        builder.Property(u => u.FkUserProfileId)
            .HasColumnName("fk_user_profile_id")
            .IsRequired();

        // configure relationships
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
