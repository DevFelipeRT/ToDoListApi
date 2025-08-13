using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Domain.Lists.Services.Interfaces;
using Domain.Lists.ValueObjects;
using Domain.Accounts.ValueObjects;
using Domain.Lists.Repositories;

namespace Application.Lists.Commands.Items.Handlers;

/// <summary>
/// Handles setting a due date for a specific to-do item,
/// including validation and domain logic execution.
/// </summary>
public sealed class SetDueDateHandler : IRequestHandler<SetDueDateCommand>
{
    private readonly IToDoListRepository _listRepository;
    private readonly IAuthorizationService _authorizationService;

    public SetDueDateHandler(
        IToDoListRepository listRepository,
        IAuthorizationService authorizationService)
    {
        _listRepository = listRepository;
        _authorizationService = authorizationService;
    }

    public async Task Handle(SetDueDateCommand request, CancellationToken cancellationToken)
    {
        // Convert primitives to Value Objects (assuming UserId is added to command)
        var listId = new ToDoListId(request.ListId);
        var itemId = new ToDoItemId(request.ItemId);
        var userId = AccountId.FromGuid(request.UserId);
        var dueDate = new DueDate(request.DueDate);

        await _authorizationService.AssertUserListAccessAsync(userId, listId, cancellationToken);

        var list = await _listRepository.GetByIdAsync(listId, cancellationToken)
            ?? throw new InvalidOperationException("List not found.");

        var item = list.GetItem(itemId)
            ?? throw new InvalidOperationException("Item not found.");

        item.SetDueDate(dueDate);

        // Persist changes
        await _listRepository.UpdateAsync(list, cancellationToken);
    }
}
