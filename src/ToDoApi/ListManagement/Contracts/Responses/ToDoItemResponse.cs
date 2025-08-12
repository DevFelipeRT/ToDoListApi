using ToDoApi.Common.Contracts;

namespace ToDoApi.ListManagement.Contracts.Responses;

/// <summary>
/// Response model for a todo item.
/// </summary>
public class ToDoItemResponse
{
    /// <summary>
    /// Unique identifier for the todo item.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The title of the todo item.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Indicates if the item is completed.
    /// </summary>
    public bool IsComplete { get; set; }

    /// <summary>
    /// Optional due date for the todo item.
    /// </summary>
    public DateTime? DueDate { get; set; }

    /// <summary>
    /// Date and time when the item was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date and time when the item was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Date and time when the item was completed, if applicable.
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// Identifier of the list that contains this item.
    /// </summary>
    public Guid ListId { get; set; }

    /// <summary>
    /// HATEOAS links related to this todo item.
    /// </summary>
    public List<Link> Links { get; set; } = new();
}
