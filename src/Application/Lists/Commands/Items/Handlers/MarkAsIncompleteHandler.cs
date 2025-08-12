using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Application.Lists.Commands.Items;
using Domain.Lists.Services.Interfaces;
using Domain.Lists.ValueObjects;
using Domain.Accounts.ValueObjects;
using Domain.Lists.Repositories;

namespace Application.Lists.Commands.Items.Handlers;

/// <summary>
/// Handles the command to mark a To-Do item as incomplete within its parent list.
/// Contains the application logic for marking items as incomplete.
/// </summary>
public sealed class MarkAsIncompleteHandler : IRequestHandler<MarkAsIncompleteCommand, bool>
{
    private readonly IToDoListRepository _listRepository;
    private readonly IAuthorizationService _authorizationService;

    public MarkAsIncompleteHandler(
        IToDoListRepository listRepository,
        IAuthorizationService authorizationService)
    {
        _listRepository = listRepository;
        _authorizationService = authorizationService;
    }

    public async Task<bool> Handle(MarkAsIncompleteCommand request, CancellationToken cancellationToken)
    {
        // Convert primitives to Value Objects
        var userId = new AccountId(request.UserId);
        var listId = new ToDoListId(request.ListId);
        var itemId = new ToDoItemId(request.ItemId);

        await _authorizationService.AssertUserListAccessAsync(userId, listId, cancellationToken);

        var list = await _listRepository.GetByIdAsync(listId, cancellationToken)
            ?? throw new InvalidOperationException("List not found.");

        var item = list.GetItem(itemId)
            ?? throw new InvalidOperationException("Item not found.");

        item.MarkAsIncomplete();

        // Persist changes
        await _listRepository.UpdateAsync(list, cancellationToken);

        return true;
    }
}
