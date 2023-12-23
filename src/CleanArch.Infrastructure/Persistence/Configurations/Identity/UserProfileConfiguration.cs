using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CleanArch.Domain.Aggregates.Identity;

namespace CleanArch.Infrastructure.Persistence.Configurations.Identity;

internal sealed class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.ToTable("dbo.user_profile");

        // primary key
        builder.HasKey(u => u.UserProfileId);

        // configure properties
        builder.Property(u => u.UserProfileId)
            .HasColumnName("user_profile_id")
            .HasMaxLength(36)
            .IsRequired();

        builder.Property(u => u.FirstName)
            .HasColumnName("first_name")
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(u => u.LastName)
            .HasColumnName("last_name")
            .HasMaxLength(64);

        builder.Property(u => u.DateOfBirth)
            .HasColumnName("date_of_birth")
            .IsRequired();
    }
}
