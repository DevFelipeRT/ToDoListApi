using Domain.Accounts.Entities;
using Domain.Accounts.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Accounts.Configurations;

public sealed class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("Accounts");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .HasConversion(id => id.Value, v => AccountId.FromGuid(v))
            .ValueGeneratedNever();

        builder.OwnsOne(a => a.Email, email =>
        {
            email.Property(p => p.Value)
                .HasColumnName("Email")
                .HasMaxLength(AccountEmail.MaxLength)
                .IsRequired();

            email.HasIndex(p => p.Value).IsUnique();
        });

        builder.OwnsOne(a => a.Username, username =>
        {
            username.Property(p => p.Value)
                .HasColumnName("Username")
                .HasMaxLength(AccountUsername.MaxLength)
                .IsRequired();

            username.HasIndex(p => p.Value).IsUnique();
        });

        builder.OwnsOne(a => a.Name, name =>
        {
            name.Property(p => p.Value)
                .HasColumnName("DisplayName")
                .HasMaxLength(AccountName.MaxLength)
                .IsRequired();
        });

        builder.OwnsOne(a => a.CredentialId, cred =>
        {
            cred.Property(p => p.Kind)
                .HasColumnName("CredentialKind")
                .HasConversion<int>();

            cred.Property(p => p.Value)
                .HasColumnName("CredentialValue")
                .HasMaxLength(256);

            cred.WithOwner();
        });

        builder.Navigation(a => a.CredentialId).IsRequired(false);

        builder.Property(a => a.CreatedAt).IsRequired();
        builder.Property(a => a.LastLoginAt);
        builder.Property(a => a.ActivatedAt);
    }
}
