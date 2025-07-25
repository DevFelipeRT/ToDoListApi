using System.ComponentModel.DataAnnotations;

namespace ToDoList.ToDoApi.Contracts.ToDoItems;

/// <summary>
/// Request contract for updating the title of a To-Do item.
/// </summary>
public sealed class UpdateToDoItemTitleRequest
{
    private const int MaxTitleLength = 150;

    /// <summary>
    /// Gets or sets the new title for the To-Do item.
    /// </summary>
    [Required]
    [MaxLength(MaxTitleLength)]
    public string Title { get; set; } = string.Empty;
}
