using System.ComponentModel.DataAnnotations;

namespace Api.ListManagement.Contracts.Requests;

/// <summary>
/// Request model for setting a due date on a todo item.
/// </summary>
public class SetDueDateRequest
{
    /// <summary>
    /// The date and time when the due date should be triggered.
    /// </summary>
    [Required(ErrorMessage = "Due date is required")]
    public DateTime DueDate { get; set; }
}
