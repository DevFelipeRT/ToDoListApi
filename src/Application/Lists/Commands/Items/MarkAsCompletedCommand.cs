using System;
using MediatR;

namespace Application.Lists.Commands.Items;

/// <summary>
/// Command to mark a specific To-Do item as completed within a given list.
/// This action updates the item's completion state in the context of its parent aggregate,
/// ensuring context and ownership validation.
/// </summary>
public sealed class MarkAsCompletedCommand : IRequest<bool>
{
    /// <summary>
    /// Gets the identifier of the To-Do list that contains the item.
    /// </summary>
    public Guid ListId { get; }

    /// <summary>
    /// Gets the identifier of the To-Do item to be marked as completed.
    /// </summary>
    public Guid ItemId { get; }

    /// <summary>
    /// Gets the identifier of the account performing the operation.
    /// </summary>
    public Guid AccountId { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MarkAsCompletedCommand"/> class.
    /// </summary>
    /// <param name="listId">The identifier of the list containing the item.</param>
    /// <param name="itemId">The identifier of the item to mark as completed.</param>
    /// <param name="accountId">The identifier of the account performing the operation.</param>
    public MarkAsCompletedCommand(Guid listId, Guid itemId, Guid accountId)
    {
        ListId = listId;
        ItemId = itemId;
        AccountId = accountId;
    }
}
