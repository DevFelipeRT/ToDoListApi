using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Domain.Accounts.ValueObjects;
using Domain.Lists.Entities;
using Domain.Lists.ValueObjects;
using Domain.Lists.Repositories;
using Domain.Lists.Services.Interfaces;
using Application.Abstractions.Persistence;

namespace Application.Lists.Commands.Items.Handlers;

/// <summary>
/// Handles the creation of a new To-Do item within a list,
/// including aggregate retrieval, authorization validation, item creation, and persistence.
/// </summary>
public sealed class CreateToDoItemHandler : IRequestHandler<CreateToDoItemCommand, Guid>
{
    private readonly IToDoListRepository _listRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthorizationService _authorizationService;

    public CreateToDoItemHandler(
        IToDoListRepository listRepository,
        IUnitOfWork unitOfWork,
        IAuthorizationService authorizationService)
    {
        _listRepository = listRepository;
        _unitOfWork = unitOfWork;
        _authorizationService = authorizationService;
    }

    public async Task<Guid> Handle(CreateToDoItemCommand request, CancellationToken cancellationToken)
    {
        // Convert primitives to Value Objects
        var accountId = AccountId.FromGuid(request.AccountId);
        var listId = new ToDoListId(request.ListId);
        var title = new Title(request.Title);
        var dueDate = request.DueDate.HasValue ? new DueDate(request.DueDate.Value) : null;

        await _authorizationService.AssertAccountListAccessAsync(accountId, listId, cancellationToken);

        var list = await _listRepository.GetByIdAsync(listId, cancellationToken)
            ?? throw new InvalidOperationException("List not found.");

        var item = new ToDoItem(title, dueDate);

        list.AddItem(item);

        // Persist changes
        await _listRepository.UpdateAsync(list, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return item.Id.Value;
    }
}
