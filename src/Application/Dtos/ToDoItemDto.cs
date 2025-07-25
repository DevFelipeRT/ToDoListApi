namespace ToDoList.Application.Dtos;

/// <summary>
/// Data Transfer Object representing a To-Do item for query responses.
/// </summary>
public sealed class ToDoItemDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the To-Do item.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the title of the To-Do item.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the To-Do item is completed.
    /// </summary>
    public bool IsCompleted { get; set; }

    /// <summary>
    /// Gets or sets the creation date and time of the To-Do item (in UTC).
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the completion date and time of the To-Do item, if completed (in UTC).
    /// </summary>
    public DateTime? CompletedAt { get; set; }
} 