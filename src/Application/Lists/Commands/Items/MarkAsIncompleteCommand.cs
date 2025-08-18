using System;
using MediatR;

namespace Application.Lists.Commands.Items;

/// <summary>
/// Command to mark a specific To-Do item as incomplete within its parent list.
/// This operation reverses the completion state previously set, 
/// with account authorization and aggregate context.
/// </summary>
public sealed class MarkAsIncompleteCommand : IRequest<bool>
{
    /// <summary>
    /// Gets the identifier of the To-Do list containing the item.
    /// </summary>
    public Guid ListId { get; }

    /// <summary>
    /// Gets the identifier of the To-Do item to be marked as incomplete.
    /// </summary>
    public Guid ItemId { get; }

    /// <summary>
    /// Gets the identifier of the account performing the operation.
    /// </summary>
    public Guid AccountId { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MarkAsIncompleteCommand"/> class.
    /// </summary>
    /// <param name="listId">The identifier of the list that owns the item.</param>
    /// <param name="itemId">The identifier of the item to mark as incomplete.</param>
    /// <param name="accountId">The identifier of the account performing the operation.</param>
    public MarkAsIncompleteCommand(Guid listId, Guid itemId, Guid accountId)
    {
        ListId = listId;
        ItemId = itemId;
        AccountId = accountId;
    }
}
