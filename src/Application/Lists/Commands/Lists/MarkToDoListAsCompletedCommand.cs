using System;
using MediatR;

namespace Application.Lists.Commands.Lists;

/// <summary>
/// Command to mark a To-Do list as completed.
/// </summary>
public sealed class MarkToDoListAsCompletedCommand : IRequest<bool>
{
    public Guid ListId { get; }
    public Guid UserId { get; }

    public MarkToDoListAsCompletedCommand(Guid listId, Guid userId)
    {
        ListId = listId;
        UserId = userId;
    }
}
