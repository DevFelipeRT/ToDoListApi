using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Accounts.Entities;
using Domain.Accounts.ValueObjects;

namespace Infrastructure.Persistence.Configurations;

public sealed class ActivationTokenConfiguration : IEntityTypeConfiguration<ActivationToken>
{
    public void Configure(EntityTypeBuilder<ActivationToken> b)
    {
        b.ToTable("ActivationTokens");

        b.HasKey(t => t.Id);

        b.Property(t => t.AccountId)
         .HasConversion(v => v.Value, v => AccountId.FromGuid(v))
         .IsRequired();

        b.Property(t => t.Hash)
         .HasMaxLength(64)
         .IsRequired();

        b.Property(t => t.CreatedAt).HasColumnType("datetimeoffset").IsRequired();
        b.Property(t => t.ExpiresAt).HasColumnType("datetimeoffset").IsRequired();
        b.Property(t => t.RevokedAt).HasColumnType("datetimeoffset");
        b.Property(t => t.RevokedReason);
    }
}
