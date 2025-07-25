using System.ComponentModel.DataAnnotations;

namespace ToDoList.ToDoApi.Contracts.ToDoItems;

/// <summary>
/// Represents the request contract for creating a new To-Do item.
/// </summary>
public sealed class CreateToDoItemRequest
{
    private const int MaxTitleLength = 150;

    /// <summary>
    /// Gets or sets the title for the new To-Do item.
    /// </summary>
    [Required]
    [MaxLength(MaxTitleLength)]
    public string Title { get; set; } = string.Empty;
}