using System;
using MediatR;

namespace Application.Lists.Commands.Lists;

/// <summary>
/// Command to mark a To-Do list as incomplete.
/// </summary>
public sealed class MarkToDoListAsIncompleteCommand : IRequest<bool>
{
    public Guid ListId { get; }
    public Guid AccountId { get; }

    public MarkToDoListAsIncompleteCommand(Guid listId, Guid accountId)
    {
        ListId = listId;
        AccountId = accountId;
    }
}
