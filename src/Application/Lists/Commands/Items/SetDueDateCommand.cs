using System;
using MediatR;

namespace Application.Lists.Commands.Items;

/// <summary>
/// Command to set a due date for a specific to-do item.
/// </summary>
public sealed class SetDueDateCommand : IRequest
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
    /// Gets the due date and time.
    /// </summary>
    public DateTime DueDate { get; init; }

    /// <summary>
    /// Gets the identifier of the account performing the operation.
    /// </summary>
    public Guid AccountId { get; init; }
}
