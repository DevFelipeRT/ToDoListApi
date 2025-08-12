using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Accounts;
using Domain.Accounts.ValueObjects;
using Domain.Lists;
using Domain.Lists.ValueObjects;

namespace Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework Core configuration for the <see cref="ToDoList"/> aggregate.
/// 
/// Relationship rules:
/// - A <see cref="User"/> can own many <see cref="ToDoList"/> entities (1:N).
/// - A <see cref="ToDoList"/> belongs to exactly one <see cref="User"/>.
/// - Deleting a <see cref="User"/> cascades to delete all their <see cref="ToDoList"/> rows.
/// 
/// Notes:
/// - The 1:N relationship from <see cref="ToDoList"/> to its <see cref="ToDoItem"/> children
///   (and its cascade delete) is configured in <c>ToDoItemConfiguration</c>, where the FK is defined.
/// </summary>
public sealed class ToDoListConfiguration : IEntityTypeConfiguration<ToDoList>
{
    public void Configure(EntityTypeBuilder<ToDoList> builder)
    {
        // Table
        builder.ToTable("ToDoLists");

        // Key
        builder.HasKey(list => list.Id);

        // PK conversion (Value Object -> Guid)
        builder.Property(list => list.Id)
            .HasConversion(id => id.Value, value => new ToDoListId(value))
            .IsRequired();

        // Required FK to User (Value Object -> Guid)
        builder.Property(list => list.UserId)
            .HasConversion(vo => vo.Value, guid => AccountId.FromGuid(guid))
            .IsRequired();

        // 1:N User -> ToDoLists with CASCADE delete
        // Deleting a User deletes all their lists.
        builder.HasOne<User>()                 // principal side (User)
            .WithMany()                        // a User has many lists
            .HasForeignKey(l => l.UserId)      // FK column on ToDoLists
            .OnDelete(DeleteBehavior.Cascade); // CASCADE: User deletion removes lists

        // Business properties (Value Objects and primitives)
        builder.Property(list => list.Title)
            .HasMaxLength(Title.MaxLength)
            .HasConversion(v => v.Value, v => new Title(v))
            .IsRequired();

        builder.Property(list => list.Description)
            .HasMaxLength(Description.MaxLength)
            .HasConversion(
                v => v == null ? null : v.Value,
                v => string.IsNullOrWhiteSpace(v) ? null : new Description(v));

        builder.Property(list => list.IsCompleted)
            .IsRequired();

        builder.Property(list => list.CreatedAt)
            .HasColumnType("datetime2")
            .IsRequired();

        builder.Property(list => list.CompletedAt)
            .HasColumnType("datetime2");

        // Indexes helpful for common queries
        builder.HasIndex(l => l.UserId);
        builder.HasIndex(l => l.IsCompleted);
        builder.HasIndex(l => l.CreatedAt);

        var navigation = builder.Metadata.FindNavigation("ItemsForEfCore");
        if (navigation != null)
        {
            navigation.SetPropertyAccessMode(PropertyAccessMode.Property);
        }
    }
}
