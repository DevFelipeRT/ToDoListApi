using System;
using Domain.Lists;
using Domain.Lists.ValueObjects;

namespace Application.Lists.DTOs;

/// <summary>
/// Data Transfer Object (DTO) representing a To-Do item for application and presentation layers.
/// </summary>
public sealed class ToDoItemDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the To-Do item.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Gets or sets the title of the To-Do item.
    /// </summary>
    public string Title { get; init; } = null!;

    /// <summary>
    /// Gets or sets a value indicating whether the To-Do item has been completed.
    /// </summary>
    public bool IsCompleted { get; init; }

    /// <summary>
    /// Gets or sets the creation timestamp of the To-Do item.
    /// </summary>
    public DateTime CreatedAt { get; init; }

    /// <summary>
    /// Gets or sets the due date of the To-Do item, if any.
    /// </summary>
    public DateTime? DueDate { get; init; }

    /// <summary>
    /// Gets or sets the completion timestamp of the To-Do item, if any.
    /// </summary>
    public DateTime? CompletedAt { get; init; }

    /// <summary>
    /// Gets or sets the unique identifier of the list to which the To-Do item belongs.
    /// </summary>
    public Guid ListId { get; init; }

    /// <summary>
    /// Gets a value indicating whether the To-Do item has a reminder set.
    /// </summary>
    public bool HasReminder => DueDate.HasValue;

    /// <summary>
    /// Creates a new <see cref="ToDoItemDto"/> from the specified domain entity.
    /// </summary>
    /// <param name="item">The domain ToDoItem entity.</param>
    /// <returns>A mapped <see cref="ToDoItemDto"/> instance.</returns>
    public static ToDoItemDto FromDomain(ToDoItem item)
    {
        return new ToDoItemDto
        {
            Id = item.Id.Value,
            Title = item.Title.Value,
            IsCompleted = item.IsCompleted,
            CreatedAt = item.CreatedAt,
            DueDate = item.DueDate?.Value,
            CompletedAt = item.CompletedAt,
        };
    }
    
    /// <summary>
    /// Creates a new <see cref="ToDoItemDto"/> from the specified domain entity.
    /// </summary>
    /// <param name="item">The domain ToDoItem entity.</param>
    /// <param name="listId">The unique identifier of the list to which the item belongs.</param>
    /// <returns>A mapped <see cref="ToDoItemDto"/> instance.</returns>
    public static ToDoItemDto FromDomain(ToDoItem item, ToDoListId listId)
    {
        return new ToDoItemDto
        {
            Id = item.Id.Value,
            Title = item.Title.Value,
            IsCompleted = item.IsCompleted,
            CreatedAt = item.CreatedAt,
            DueDate = item.DueDate?.Value,
            CompletedAt = item.CompletedAt,
            ListId = listId.Value
        };
    }

}
