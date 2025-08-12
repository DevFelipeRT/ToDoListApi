using System;
using MediatR;

namespace Application.Lists.Commands.Items;

/// <summary>
/// Command to remove a reminder from a specific to-do item.
/// </summary>
public sealed class RemoveDueDateCommand : IRequest
{
    /// <summary>
    /// Gets the unique identifier of the to-do list.
    /// </summary>
    public Guid ListId { get; init; }

    /// <summary>
    /// Gets the unique identifier of the to-do item.
    /// </summary>
    public Guid ItemId { get; init; }

    /// <summary>
    /// Gets the identifier of the user performing the operation.
    /// </summary>
    public Guid UserId { get; init; }
}
