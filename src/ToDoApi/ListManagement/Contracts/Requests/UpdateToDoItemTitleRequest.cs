using System.ComponentModel.DataAnnotations;

namespace ToDoApi.ListManagement.Contracts.Requests;

/// <summary>
/// Request model for updating a todo item title.
/// </summary>
public class UpdateToDoItemTitleRequest
{
    /// <summary>
    /// The new title for the todo item.
    /// </summary>
    [Required(ErrorMessage = "Title is required")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 200 characters")]
    public string Title { get; set; } = string.Empty;
}
