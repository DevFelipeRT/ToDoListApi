using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Accounts;
using Domain.Accounts.ValueObjects;
using Domain.Lists;
using Domain.Lists.ValueObjects;

namespace Infrastructure.Persistence.Configurations
{
    public sealed class ToDoListConfiguration : IEntityTypeConfiguration<ToDoList>
    {
        public void Configure(EntityTypeBuilder<ToDoList> builder)
        {
            builder.ToTable("ToDoLists");

            builder.HasKey(l => l.Id);
            builder.Property(l => l.Id)
                   .HasConversion(v => v.Value, v => ToDoListId.FromGuid(v))
                   .ValueGeneratedNever();

            builder.Property(l => l.UserId)
                   .HasConversion(v => v.Value, v => AccountId.FromGuid(v))
                   .HasColumnName("UserId")
                   .IsRequired();

            builder.Property(l => l.Title)
                   .HasMaxLength(Title.MaxLength)
                   .HasConversion(v => v.Value, v => new Title(v))
                   .IsRequired();

            builder.Property(l => l.Description)
                   .HasConversion(v => v != null ? v.Value : null,
                                  v => v != null ? new Description(v) : null)
                   .HasMaxLength(Description.MaxLength);

            builder.Property(l => l.IsCompleted).IsRequired();
            builder.Property(l => l.CreatedAt).HasColumnType("datetime2").IsRequired();
            builder.Property(l => l.CompletedAt).HasColumnType("datetime2");

            builder.HasOne<User>()
                   .WithMany()
                   .HasForeignKey(l => l.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(l => l.UserId);
            builder.HasIndex(l => l.IsCompleted);
            builder.HasIndex(l => l.CreatedAt);

            var nav = builder.Metadata.FindNavigation(nameof(ToDoList.ItemsForEfCore));
            if (nav is not null)
                nav.SetPropertyAccessMode(PropertyAccessMode.Property);
        }
    }
}
