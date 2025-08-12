using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Accounts;
using Domain.Accounts.ValueObjects;

namespace Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework Core configuration for the <see cref="User"/> aggregate root.
/// 
/// Notes:
/// - This configuration maps only the User fields.
/// - The 1:N relationship (User → ToDoList) with delete cascade is defined on the
///   dependent side inside <c>ToDoListConfiguration</c>, where the foreign key is.
///   This is the recommended place to configure cascade behavior for clarity.
/// </summary>
public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // Table
        builder.ToTable("Users");

        // Key
        builder.HasKey(user => user.Id);

        // Primary key conversion (Value Object -> Guid)
        builder.Property(user => user.Id)
            .HasConversion(id => id.Value, value => AccountId.FromGuid(value))
            .IsRequired();

        // Email (VO) with max length and required
        builder.Property(user => user.Email)
            .HasMaxLength(AccountEmail.MaxLength)
            .HasConversion(v => v.Value, v => new AccountEmail(v))
            .IsRequired();

        // Username (VO) with max length and required
        builder.Property(user => user.Username)
            .HasMaxLength(AccountUsername.MaxLength)
            .HasConversion(v => v.Value, v => new AccountUsername(v))
            .IsRequired();

        // Name (VO) with max length and required
        builder.Property(user => user.Name)
            .HasMaxLength(AccountName.MaxLength)
            .HasConversion(v => v.Value, v => new AccountName(v))
            .IsRequired();

        // PasswordHash (plain string, required)
        builder.Property(user => user.PasswordHash)
            .IsRequired();

        // IsActive (simple boolean, required)
        builder.Property(user => user.IsActive)
            .IsRequired();

        // CreatedAt (UTC recommended), required
        builder.Property(user => user.CreatedAt)
            .HasColumnType("datetime2")
            .IsRequired();

        // LastLoginAt (nullable)
        builder.Property(user => user.LastLoginAt)
            .HasColumnType("datetime2");

        // Useful indexes
        builder.HasIndex(u => u.Email).IsUnique();
        builder.HasIndex(u => u.Username).IsUnique();
        builder.HasIndex(u => u.IsActive);
    }
}
