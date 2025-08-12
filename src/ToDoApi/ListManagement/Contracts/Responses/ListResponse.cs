using ToDoApi.Common.Contracts;

namespace ToDoApi.ListManagement.Contracts.Responses;

/// <summary>
/// Response model for a todo list.
/// </summary>
public class ListResponse
{
    /// <summary>
    /// Unique identifier for the list.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The title of the todo list.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Optional description of the todo list.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Indicates if the entire list is marked as complete.
    /// </summary>
    public bool IsComplete { get; set; }

    /// <summary>
    /// Total number of items in this list.
    /// </summary>
    public int ItemCount { get; set; }

    /// <summary>
    /// Number of completed items in this list.
    /// </summary>
    public int CompletedItemCount { get; set; }

    /// <summary>
    /// Date and time when the list was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date and time when the list was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// HATEOAS links related to this list.
    /// </summary>
    public List<Link> Links { get; set; } = new();
}
