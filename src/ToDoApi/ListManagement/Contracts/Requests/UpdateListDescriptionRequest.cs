using System.ComponentModel.DataAnnotations;

namespace ToDoApi.ListManagement.Contracts.Requests;

/// <summary>
/// Request model for updating a todo list description.
/// </summary>
public class UpdateListDescriptionRequest
{
    /// <summary>
    /// The new description for the todo list.
    /// </summary>
    [StringLength(500, ErrorMessage = "Description must be up to 500 characters long")]
    public string? Description { get; set; }
}
