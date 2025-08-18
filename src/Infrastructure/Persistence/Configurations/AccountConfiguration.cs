using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Accounts.Entities;
using Domain.Accounts.ValueObjects;

namespace Infrastructure.Persistence.Configurations;

public sealed class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("Accounts");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .HasConversion(v => v.Value, v => AccountId.FromGuid(v))
            .ValueGeneratedNever();

        builder.Property(a => a.Email)
            .HasConversion(v => v.Value, v => new AccountEmail(v))
            .HasMaxLength(AccountEmail.MaxLength)
            .IsRequired();

        builder.Property(a => a.Username)
            .HasConversion(v => v.Value, v => new AccountUsername(v))
            .HasMaxLength(AccountUsername.MaxLength)
            .IsRequired();

        builder.Property(a => a.Name)
            .HasConversion(v => v.Value, v => new AccountName(v))
            .HasMaxLength(AccountName.MaxLength)
            .IsRequired();

        builder.Property(a => a.PasswordHash).IsRequired();

        builder.Property(a => a.CreatedAt).HasColumnType("datetimeoffset").IsRequired();
        builder.Property(a => a.LastLoginAt).HasColumnType("datetimeoffset");
        builder.Property(a => a.ActivatedAt).HasColumnType("datetimeoffset");

        builder.HasIndex(a => a.Email).IsUnique();
        builder.HasIndex(a => a.Username).IsUnique();
        builder.HasIndex(a => a.ActivatedAt);

        // ActivationTokens: map the navigation and point it to the backing field
        builder.HasMany(a => a.ActivationTokens)
               .WithOne() // no inverse navigation on ActivationToken
               .HasForeignKey(t => t.AccountId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(a => a.ActivationTokens)
               .HasField("_activationTokens")
               .UsePropertyAccessMode(PropertyAccessMode.Field);

        // Roles: many-to-many via join table AccountsRoles
        builder.HasMany(a => a.Roles)
               .WithMany()
               .UsingEntity<Dictionary<string, object>>(
                   "AccountsRoles",
                   right => right.HasOne<Role>()
                                 .WithMany()
                                 .HasForeignKey("RoleId")
                                 .HasConstraintName("FK_AccountsRoles_Roles_RoleId"),
                   left  => left.HasOne<Account>()
                                 .WithMany()
                                 .HasForeignKey("AccountId")
                                 .HasConstraintName("FK_AccountsRoles_Accounts_AccountId"),
                   join =>
                   {
                       join.ToTable("AccountsRoles");
                       join.HasKey("AccountId", "RoleId");
                       join.Property<DateTimeOffset>("CreatedAt")
                           .HasColumnType("datetimeoffset")
                           .HasDefaultValueSql("SYSUTCDATETIME()");
                       join.HasIndex("RoleId");
                   });
    }
}
