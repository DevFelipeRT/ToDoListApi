using System;
using MediatR;

namespace Application.Lists.Commands.Lists;

/// <summary>
/// Command to delete a To-Do list.
/// </summary>
public sealed class DeleteToDoListCommand : IRequest<bool>
{
    public Guid ListId { get; }
    public Guid UserId { get; }

    public DeleteToDoListCommand(Guid listId, Guid userId)
    {
        ListId = listId;
        UserId = userId;
    }
}
