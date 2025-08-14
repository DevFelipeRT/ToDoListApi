using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Lists;
using Domain.Lists.ValueObjects;

namespace Infrastructure.Persistence.Configurations
{
    public sealed class ToDoItemConfiguration : IEntityTypeConfiguration<ToDoItem>
    {
        public void Configure(EntityTypeBuilder<ToDoItem> builder)
        {
            builder.ToTable("ToDoItems");

            builder.HasKey(i => i.Id);
            builder.Property(i => i.Id)
                   .HasConversion(v => v.Value, v => ToDoItemId.FromGuid(v))
                   .ValueGeneratedNever();

            builder.Property(i => i.Title)
                   .HasMaxLength(Title.MaxLength)
                   .HasConversion(v => v.Value, v => new Title(v))
                   .IsRequired();

            builder.Property(i => i.IsCompleted).IsRequired();

            builder.Property(i => i.DueDate)
                   .HasConversion(
                       v => v != null ? (DateTime?)v.Value : null,
                       v => v.HasValue ? new DueDate(v.Value) : null
                   )
                   .HasColumnType("datetime2");

            builder.Property(i => i.CreatedAt).HasColumnType("datetime2").IsRequired();
            builder.Property(i => i.CompletedAt).HasColumnType("datetime2");

            builder.Property<ToDoListId>("ListId")
                   .HasConversion(v => v.Value, v => ToDoListId.FromGuid(v))
                   .IsRequired();

            builder.HasOne<ToDoList>()
                   .WithMany(nameof(ToDoList.ItemsForEfCore))
                   .HasForeignKey("ListId")
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex("ListId");
            builder.HasIndex(i => i.IsCompleted);
            builder.HasIndex(i => i.CreatedAt);
            builder.HasIndex(i => i.DueDate);
        }
    }
}