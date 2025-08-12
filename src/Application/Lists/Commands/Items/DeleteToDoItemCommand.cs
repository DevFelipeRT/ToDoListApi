using System;
using MediatR;

namespace Application.Lists.Commands.Items;

/// <summary>
/// Command to delete a To-Do item from a specific To-Do list.
/// Carries all data necessary to authorize and perform the deletion operation.
/// </summary>
public sealed class DeleteToDoItemCommand : IRequest<bool>
{
    /// <summary>
    /// Gets the identifier of the To-Do list that owns the item.
    /// </summary>
    public Guid ListId { get; }

    /// <summary>
    /// Gets the identifier of the To-Do item to be deleted.
    /// </summary>
    public Guid ItemId { get; }

    /// <summary>
    /// Gets the identifier of the user performing the operation.
    /// </summary>
    public Guid UserId { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteToDoItemCommand"/> class.
    /// </summary>
    /// <param name="listId">The identifier of the parent To-Do list.</param>
    /// <param name="itemId">The identifier of the item to delete.</param>
    /// <param name="userId">The identifier of the user performing the operation.</param>
    public DeleteToDoItemCommand(Guid listId, Guid itemId, Guid userId)
    {
        ListId = listId;
        ItemId = itemId;
        UserId = userId;
    }
}
