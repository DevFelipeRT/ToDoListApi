using System;
using MediatR;

namespace Application.Lists.Commands.Items;

/// <summary>
/// Command to update the title of a To-Do item within a specific list.
/// The new title will be validated and encapsulated by the domain layer.
/// </summary>
public sealed class UpdateToDoItemTitleCommand : IRequest<bool>
{
    /// <summary>
    /// Gets the identifier of the To-Do list that owns the item.
    /// </summary>
    public Guid ListId { get; }

    /// <summary>
    /// Gets the identifier of the To-Do item to be updated.
    /// </summary>
    public Guid ItemId { get; }

    /// <summary>
    /// Gets the identifier of the account performing the update.
    /// </summary>
    public Guid AccountId { get; }

    /// <summary>
    /// Gets the new raw title input for the To-Do item.
    /// This value must be validated and converted to a <see cref="Domain.Lists.ValueObjects.Title"/> in the application layer.
    /// </summary>
    public string NewTitle { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateToDoItemTitleCommand"/> class.
    /// </summary>
    /// <param name="listId">The identifier of the parent list.</param>
    /// <param name="itemId">The identifier of the item to be updated.</param>
    /// <param name="accountId">The identifier of the account performing the update.</param>
    /// <param name="newTitle">The new title string.</param>
    public UpdateToDoItemTitleCommand(Guid listId, Guid itemId, Guid accountId, string newTitle)
    {
        ListId = listId;
        ItemId = itemId;
        AccountId = accountId;
        NewTitle = newTitle;
    }
}
