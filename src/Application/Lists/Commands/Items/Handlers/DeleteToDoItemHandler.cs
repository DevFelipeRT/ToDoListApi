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
/// Handles the deletion of a To-Do item from a list.
/// </summary>
public sealed class DeleteToDoItemHandler : IRequestHandler<DeleteToDoItemCommand, bool>
{
    private readonly IToDoListRepository _listRepository;
    private readonly IAuthorizationService _authorizationService;

    public DeleteToDoItemHandler(
        IToDoListRepository listRepository,
        IAuthorizationService authorizationService)
    {
        _listRepository = listRepository;
        _authorizationService = authorizationService;
    }

    public async Task<bool> Handle(DeleteToDoItemCommand request, CancellationToken cancellationToken)
    {
        // Convert primitives to Value Objects
        var userId = AccountId.FromGuid(request.UserId);
        var listId = new ToDoListId(request.ListId);
        var itemId = new ToDoItemId(request.ItemId);

        await _authorizationService.AssertUserListAccessAsync(userId, listId, cancellationToken);

        var list = await _listRepository.GetByIdAsync(listId, cancellationToken)
            ?? throw new InvalidOperationException("List not found.");

        list.DeleteItem(itemId);

        // Persist changes
        await _listRepository.UpdateAsync(list, cancellationToken);

        return true;
    }
}
