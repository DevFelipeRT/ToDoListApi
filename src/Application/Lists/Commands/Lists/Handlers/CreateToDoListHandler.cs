using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Domain.Accounts.ValueObjects;
using Domain.Lists.Entities;
using Domain.Lists.Policies;
using Domain.Lists.ValueObjects;
using Domain.Lists.Repositories;

namespace Application.Lists.Commands.Lists.Handlers;

/// <summary>
/// Handles the command to create a new To-Do list for an account.
/// Ensures title encapsulation and aggregate consistency.
/// </summary>
public sealed class CreateToDoListHandler : IRequestHandler<CreateToDoListCommand, Guid>
{
    private readonly IToDoListRepository _listRepository;
    private readonly IToDoListUniquenessPolicy _uniquenessChecker;

    public CreateToDoListHandler(
        IToDoListRepository listRepository,
        IToDoListUniquenessPolicy uniquenessChecker)
    {
        _listRepository = listRepository;
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
        await _listRepository.AddAsync(list, cancellationToken);

        return list.Id.Value;
    }
}
