using System;
using System.Threading;
using System.Threading.Tasks;
using Domain.Accounts.ValueObjects;
using Domain.Lists.Entities;
using Domain.Lists.ValueObjects;
using Domain.Lists.Repositories;
using Domain.Lists.Services.Interfaces;

namespace Application.Lists.Services;

/// <summary>
/// Service responsible for authorization checks for To-Do List operations.
/// This service contains shared authorization logic used across multiple handlers.
/// </summary>
public sealed class AuthorizationService : IAuthorizationService
{
    private readonly IToDoListRepository _listRepository;

    public AuthorizationService(IToDoListRepository listRepository)
    {
        _listRepository = listRepository;
    }


    /// <inheritdoc />
    public async Task AssertAccountListAccessAsync(AccountId accountId, ToDoListId listId, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(accountId);
        ArgumentNullException.ThrowIfNull(listId);

        var list = await RetrieveListAsync(listId, cancellationToken);

        if (!list.AccountId.Equals(accountId))
            throw new UnauthorizedAccessException("The account does not have permission to access or modify this list.");
    }

    private async Task<ToDoList> RetrieveListAsync(ToDoListId listId, CancellationToken cancellationToken)
    {
        var list = await _listRepository.GetByIdAsync(listId, cancellationToken);
        if (list == null)
            throw new InvalidOperationException("List not found.");
        return list;
    }
}