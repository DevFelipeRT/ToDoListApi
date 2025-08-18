using System;
using MediatR;

namespace Application.Lists.Commands.Lists;

/// <summary>
/// Command to update the title of an existing To-Do list.
/// </summary>
public sealed class UpdateToDoListTitleCommand : IRequest<bool>
{
    public Guid ListId { get; }
    public Guid AccountId { get; }
    public string NewTitle { get; }

    public UpdateToDoListTitleCommand(Guid listId, Guid accountId, string newTitle)
    {
        ListId = listId;
        AccountId = accountId;
        NewTitle = newTitle;
    }
}
