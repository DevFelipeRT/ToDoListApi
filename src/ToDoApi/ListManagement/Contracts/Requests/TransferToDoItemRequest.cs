using System.ComponentModel.DataAnnotations;

namespace ToDoApi.ListManagement.Contracts.Requests;

/// <summary>
/// Request model for transferring a todo item between lists.
/// </summary>
public class TransferToDoItemRequest
{
    /// <summary>
    /// The ID of the target list where the item will be moved.
    /// </summary>
    [Required(ErrorMessage = "Target list ID is required")]
    public Guid TargetListId { get; set; }
}
