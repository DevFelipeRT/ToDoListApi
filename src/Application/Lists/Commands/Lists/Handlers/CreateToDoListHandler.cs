using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Domain.Accounts.ValueObjects;
using Domain.Lists.Entities;
using Domain.Lists.Policies;
using Domain.Lists.ValueObjects;
using Domain.Lists.Repositories;
using Application.Abstractions.Persistence;

namespace Application.Lists.Commands.Lists.Handlers;

/// <summary>
/// Handles the command to create a new To-Do list for an account.
/// Ensures title encapsulation and aggregate consistency.
/// </summary>
public sealed class CreateToDoListHandler : IRequestHandler<CreateToDoListCommand, Guid>
{
    private readonly IToDoListRepository _listRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IToDoListUniquenessPolicy _uniquenessChecker;

    public CreateToDoListHandler(
        IToDoListRepository listRepository,
        IToDoListUniquenessPolicy uniquenessChecker,
        IUnitOfWork unitOfWork)
    {
        _listRepository = listRepository;
        _unitOfWork = unitOfWork;
        _uniquenessChecker = uniquenessChecker;
    }

    public async Task<Guid> Handle(CreateToDoListCommand request, CancellationToken cancellationToken)
    {
        // Convert primitives to Value Objects
        var accountId = AccountId.FromGuid(request.AccountId);
        var title = new Title(request.Title);
        var description = request.Description != null ? new Description(request.Description) : null;

        if (!await _uniquenessChecker.IsTitleUniqueAsync(accountId, title, cancellationToken))
            throw new InvalidOperationException("A list with the same title already exists for this account.");

        // Create new list
        var list = new ToDoList(accountId, title, description);

        // Persist list
        _listRepository.Add(list);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return list.Id.Value;
    }
}
