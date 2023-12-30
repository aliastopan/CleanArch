using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CleanArch.Domain.Aggregates.Identity;

namespace CleanArch.Infrastructure.Persistence.Configurations.Identity;

internal sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
       builder.ToTable("dbo.refresh_token");

       // primary key
        builder.HasKey(rt => rt.RefreshTokenId);

       // configure properties
       builder.Property(rt => rt.RefreshTokenId)
              .HasColumnName("refresh_token_id")
              .HasMaxLength(36)
              .IsRequired();

       builder.Property(rt => rt.Token)
              .HasColumnName("token")
              .IsRequired();

       builder.Property(rt => rt.Jti)
              .HasColumnName("jti")
              .IsRequired();

       builder.Property(rt => rt.CreationDate)
              .HasColumnName("creation_date")
              .IsRequired();

       builder.Property(rt => rt.ExpiryDate)
              .HasColumnName("expiry_date")
              .IsRequired();

       builder.Property(rt => rt.IsUsed)
              .HasColumnName("is_used")
              .IsRequired();

       builder.Property(rt => rt.IsInvalidated)
              .HasColumnName("is_invalidated")
              .IsRequired();

       builder.Property(rt => rt.IsDeleted)
              .HasColumnName("is_deleted")
              .IsRequired();

       // foreign key
       builder.Property(rt => rt.FkUserAccountId)
              .HasColumnName("fk_user_account_id")
              .IsRequired();

       // configure relationships
       builder.HasOne(rt => rt.UserAccount)
              .WithMany(u => u.RefreshTokens)
              .HasForeignKey(rt => rt.FkUserAccountId)
              .OnDelete(DeleteBehavior.Cascade);
    }
}
