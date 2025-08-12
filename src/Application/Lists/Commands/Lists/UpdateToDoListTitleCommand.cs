using System;
using MediatR;

namespace Application.Lists.Commands.Lists;

/// <summary>
/// Command to update the title of an existing To-Do list.
/// </summary>
public sealed class UpdateToDoListTitleCommand : IRequest<bool>
{
    public Guid ListId { get; }
    public Guid UserId { get; }
    public string NewTitle { get; }

    public UpdateToDoListTitleCommand(Guid listId, Guid userId, string newTitle)
    {
        ListId = listId;
        UserId = userId;
        NewTitle = newTitle;
    }
}
