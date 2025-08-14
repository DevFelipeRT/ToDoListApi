using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Accounts;
using Domain.Accounts.ValueObjects;

namespace Infrastructure.Persistence.Configurations
{
    public sealed class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(u => u.Id);
            builder.Property(u => u.Id)
                   .HasConversion(v => v.Value, v => AccountId.FromGuid(v))
                   .ValueGeneratedNever();

            builder.Property(u => u.Email)
                   .HasConversion(v => v.Value, v => new AccountEmail(v))
                   .HasMaxLength(AccountEmail.MaxLength)
                   .IsRequired();

            builder.Property(u => u.Username)
                   .HasConversion(v => v.Value, v => new AccountUsername(v))
                   .HasMaxLength(AccountUsername.MaxLength)
                   .IsRequired();

            builder.Property(u => u.Name)
                   .HasConversion(v => v.Value, v => new AccountName(v))
                   .HasMaxLength(AccountName.MaxLength)
                   .IsRequired();

            builder.Property(u => u.CreatedAt).HasColumnType("datetime2");
            builder.Property(u => u.LastLoginAt).HasColumnType("datetime2");

            builder.HasIndex(u => u.Email).IsUnique();
            builder.HasIndex(u => u.Username).IsUnique();
            builder.HasIndex(u => u.IsActive);
        }
    }
}
