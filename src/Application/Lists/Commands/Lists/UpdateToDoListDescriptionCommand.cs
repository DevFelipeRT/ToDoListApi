using System;
using MediatR;

namespace Application.Lists.Commands.Lists;

/// <summary>
/// Command to update the description of an existing To-Do list.
/// </summary>
public sealed class UpdateToDoListDescriptionCommand : IRequest<bool>
{
    public Guid ListId { get; }
    public Guid AccountId { get; }
    public string? NewDescription { get; }

    public UpdateToDoListDescriptionCommand(Guid listId, Guid accountId, string? newDescription = null)
    {
        ListId = listId;
        AccountId = accountId;
        NewDescription = newDescription;
    }
}
