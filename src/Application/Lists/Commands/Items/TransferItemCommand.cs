using System;
using MediatR;

namespace Application.Lists.Commands.Items;

/// <summary>
/// Command to transfer a to-do item from one list to another.
/// </summary>
public sealed class TransferItemCommand : IRequest
{
    /// <summary>
    /// Gets the unique identifier of the account performing the transfer.
    /// </summary>
    public Guid AccountId { get; init; }

    /// <summary>
    /// Gets the unique identifier of the source to-do list.
    /// </summary>
    public Guid SourceListId { get; init; }

    /// <summary>
    /// Gets the unique identifier of the target to-do list.
    /// </summary>
    public Guid TargetListId { get; init; }

    /// <summary>
    /// Gets the unique identifier of the to-do item to transfer.
    /// </summary>
    public Guid ItemId { get; init; }
}
