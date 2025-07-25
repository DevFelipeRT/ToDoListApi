using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ToDoList.Domain.Entities;
using ToDoList.Domain.ValueObjects;

namespace ToDoList.Infrastructure.Persistence.Configurations;

/// <summary>
/// Defines the Entity Framework Core configuration for the ToDoItem entity.
/// </summary>
public class ToDoItemConfiguration : IEntityTypeConfiguration<ToDoItem>
{
    /// <summary>
    /// Configures the entity of type ToDoItem.
    /// </summary>
    /// <param name="builder">The builder to be used to configure the entity type.</param>
    public void Configure(EntityTypeBuilder<ToDoItem> builder)
    {
        builder.HasKey(item => item.Id);

        builder.Property(item => item.Title)
            .HasMaxLength(Title.MaxLength)
            .IsRequired()
            .HasConversion(
                title => title.Value,
                value => new Title(value));

        builder.Property(item => item.IsCompleted)
            .IsRequired();

        builder.Property(item => item.CreatedAt)
            .IsRequired();
    }
}