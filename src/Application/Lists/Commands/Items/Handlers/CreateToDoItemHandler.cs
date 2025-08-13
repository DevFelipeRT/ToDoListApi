using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Domain.Lists.Services.Interfaces;
using Domain.Lists.ValueObjects;
using Domain.Lists;
using Domain.Lists.Repositories;
using Domain.Accounts.ValueObjects;

namespace Application.Lists.Commands.Items.Handlers;

/// <summary>
/// Handles the creation of a new To-Do item within a list,
/// including aggregate retrieval, authorization validation, item creation, and persistence.
/// </summary>
public sealed class CreateToDoItemHandler : IRequestHandler<CreateToDoItemCommand, Guid>
{
    private readonly IToDoListRepository _listRepository;
    private readonly IAuthorizationService _authorizationService;

    public CreateToDoItemHandler(
        IToDoListRepository listRepository,
        IAuthorizationService authorizationService)
    {
        _listRepository = listRepository;
        _authorizationService = authorizationService;
    }

    public async Task<Guid> Handle(CreateToDoItemCommand request, CancellationToken cancellationToken)
    {
        // Convert primitives to Value Objects
        var userId = AccountId.FromGuid(request.UserId);
        var listId = new ToDoListId(request.ListId);
        var title = new Title(request.Title);
        var dueDate = request.DueDate.HasValue ? new DueDate(request.DueDate.Value) : null;

        await _authorizationService.AssertUserListAccessAsync(userId, listId, cancellationToken);

        var list = await _listRepository.GetByIdAsync(listId, cancellationToken)
            ?? throw new InvalidOperationException("List not found.");

        var item = new ToDoItem(title, dueDate);

        list.AddItem(item);

        // Persist changes
        await _listRepository.UpdateAsync(list, cancellationToken);

        return item.Id.Value;
    }
}
