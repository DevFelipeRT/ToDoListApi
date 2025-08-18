using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Lists.Entities;

namespace Application.Lists.DTOs;

/// <summary>
/// Data Transfer Object (DTO) representing a To-Do list for application and presentation layers.
/// </summary>
public sealed class ToDoListDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the To-Do list.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Gets or sets the unique identifier of the account that owns the list.
    /// </summary>
    public Guid AccountId { get; init; }

    /// <summary>
    /// Gets or sets the title of the To-Do list.
    /// </summary>
    public string Title { get; init; } = null!;

    /// <summary>
    /// Gets or sets the description of the To-Do list.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether the To-Do list is completed.
    /// </summary>
    public bool IsCompleted { get; init; }

    /// <summary>
    /// Gets or sets the creation timestamp of the To-Do list.
    /// </summary>
    public DateTime CreatedAt { get; init; }

    /// <summary>
    /// Gets or sets the completion timestamp of the To-Do list, if any.
    /// </summary>
    public DateTime? CompletedAt { get; init; }

    /// <summary>
    /// Gets or sets the list of items (as DTOs) within this To-Do list.
    /// </summary>
    public IReadOnlyCollection<ToDoItemDto> Items { get; init; } = Array.Empty<ToDoItemDto>();

    /// <summary>
    /// Creates a new <see cref="ToDoListDto"/> from the specified domain entity.
    /// </summary>
    /// <param name="list">The domain ToDoList aggregate.</param>
    /// <returns>A mapped <see cref="ToDoListDto"/> instance.</returns>
    public static ToDoListDto FromDomain(ToDoList list)
    {
        return new ToDoListDto
        {
            Id = list.Id.Value,
            AccountId = list.AccountId.Value,
            Title = list.Title.Value,
            Description = list.Description?.Value,
            IsCompleted = list.IsCompleted,
            CreatedAt = list.CreatedAt,
            CompletedAt = list.CompletedAt,
            Items = list.GetAllItems().Select(ToDoItemDto.FromDomain).ToArray()
        };
    }
}
