using System;
using MediatR;

namespace Application.Lists.Commands.Lists;

/// <summary>
/// Command to mark a To-Do list as completed.
/// </summary>
public sealed class MarkToDoListAsCompletedCommand : IRequest<bool>
{
    public Guid ListId { get; }
    public Guid AccountId { get; }

    public MarkToDoListAsCompletedCommand(Guid listId, Guid accountId)
    {
        ListId = listId;
        AccountId = accountId;
    }
}
