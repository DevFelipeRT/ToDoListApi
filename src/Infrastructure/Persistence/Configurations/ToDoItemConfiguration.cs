using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Lists;
using Domain.Lists.ValueObjects;

namespace Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework Core configuration for the <see cref="ToDoItem"/> entity.
///
/// Relationship rules:
/// - A <see cref="ToDoList"/> can contain many <see cref="ToDoItem"/> rows (1:N).
/// - A <see cref="ToDoItem"/> belongs to exactly one <see cref="ToDoList"/>.
/// - Deleting a <see cref="ToDoList"/> cascades to delete all its <see cref="ToDoItem"/> rows.
///
/// Notes:
/// - The foreign key is defined on the dependent side (<c>ToDoItem</c>) using a shadow property
///   named <c>ListId</c> (type <see cref="ToDoListId"/>). This keeps the domain model free from
///   persistence-only concerns if you do not expose navigations upward to the parent list.
/// - If you later introduce a navigation such as <c>ToDoList.Items</c>, you can keep this mapping
///   and simply replace <c>WithMany()</c> with <c>WithMany(l => l.Items)</c>.
/// </summary>
public sealed class ToDoItemConfiguration : IEntityTypeConfiguration<ToDoItem>
{
    public void Configure(EntityTypeBuilder<ToDoItem> builder)
    {
        // Table
        builder.ToTable("ToDoItems");

        // Key
        builder.HasKey(item => item.Id);

        // PK conversion (Value Object -> Guid)
        builder.Property(item => item.Id)
            .HasConversion(id => id.Value, value => new ToDoItemId(value))
            .IsRequired();

        // Business properties (Value Objects and primitives)
        builder.Property(item => item.Title)
            .HasMaxLength(Title.MaxLength)
            .HasConversion(v => v.Value, v => new Title(v))
            .IsRequired();

        builder.Property(item => item.IsCompleted)
            .IsRequired();

        builder.Property(item => item.CreatedAt)
            .HasColumnType("datetime2")
            .IsRequired();

        builder.Property(item => item.CompletedAt)
            .HasColumnType("datetime2");

        builder.Property(item => item.DueDate)
            .HasConversion(
                toProvider => toProvider != null ? toProvider.Value : (DateTime?)null,
                fromProvider => fromProvider.HasValue ? new DueDate(fromProvider.Value) : null
            )
            .HasColumnType("datetime2");

        // Required FK to ToDoList via shadow property "ListId"
        // Deleting a list deletes all its items (CASCADE).
        builder.Property<ToDoListId>("ListId")
            .HasConversion(id => id.Value, value => new ToDoListId(value))
            .IsRequired();

        builder.HasOne<ToDoList>()           // principal side (ToDoList)
            .WithMany()                      // a list has many items
            .HasForeignKey("ListId")         // FK column on ToDoItems
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes helpful for common queries
        builder.HasIndex("ListId");
        builder.HasIndex(i => i.IsCompleted);
        builder.HasIndex(i => i.CreatedAt);
        builder.HasIndex(i => i.DueDate);
    }
}
