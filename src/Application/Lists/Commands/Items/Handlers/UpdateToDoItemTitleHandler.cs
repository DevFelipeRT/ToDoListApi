using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Application.Lists.Commands.Items;
using Domain.Lists.Services.Interfaces;
using Domain.Accounts.ValueObjects;
using Domain.Lists.ValueObjects;
using Domain.Lists.Repositories;

namespace Application.Lists.Commands.Items.Handlers;

/// <summary>
/// Handles the command to update the title of a To-Do item within its parent list.
/// Ensures aggregate consistency and user authorization.
/// </summary>
public sealed class UpdateToDoItemTitleHandler : IRequestHandler<UpdateToDoItemTitleCommand, bool>
{
    private readonly IToDoListRepository _listRepository;
    private readonly IAuthorizationService _authorizationService;

    public UpdateToDoItemTitleHandler(
        IToDoListRepository listRepository,
        IAuthorizationService authorizationService)
    {
        _listRepository = listRepository;
        _authorizationService = authorizationService;
    }

    public async Task<bool> Handle(UpdateToDoItemTitleCommand request, CancellationToken cancellationToken)
    {
        // Convert primitives to Value Objects
        var userId = AccountId.FromGuid(request.UserId);
        var listId = new ToDoListId(request.ListId);
        var itemId = new ToDoItemId(request.ItemId);
        var newTitle = new Title(request.NewTitle);

        await _authorizationService.AssertUserListAccessAsync(userId, listId, cancellationToken);

        var list = await _listRepository.GetByIdAsync(listId, cancellationToken)
            ?? throw new InvalidOperationException("List not found.");

        var item = list.GetItem(itemId)
            ?? throw new InvalidOperationException("Item not found.");

        item.UpdateTitle(newTitle);

        // Persist changes
        await _listRepository.UpdateAsync(list, cancellationToken);

        return true;
    }
}
