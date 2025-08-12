using System.ComponentModel.DataAnnotations;

namespace ToDoApi.ListManagement.Contracts.Requests;

/// <summary>
/// Request model for updating a todo list title.
/// </summary>
public class UpdateListTitleRequest
{
    /// <summary>
    /// The new title for the todo list.
    /// </summary>
    [Required(ErrorMessage = "Title is required")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 100 characters")]
    public string Title { get; set; } = string.Empty;
}
