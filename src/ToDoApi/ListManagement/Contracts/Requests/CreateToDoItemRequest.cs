using System.ComponentModel.DataAnnotations;

namespace ToDoApi.ListManagement.Contracts.Requests;

/// <summary>
/// Request model for creating a new todo item within a list.
/// </summary>
public class CreateToDoItemRequest
{
    /// <summary>
    /// The title of the todo item.
    /// </summary>
    [Required(ErrorMessage = "Title is required")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 200 characters")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Optional due date for the todo item.
    /// </summary>
    public DateTime? DueDate { get; set; }
}
