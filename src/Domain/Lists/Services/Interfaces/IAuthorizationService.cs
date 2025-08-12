using System;
using System.Threading;
using System.Threading.Tasks;
using Domain.Accounts.ValueObjects;
using Domain.Lists.ValueObjects;

namespace Domain.Lists.Services.Interfaces;

/// <summary>
/// Service responsible for authorization checks for To-Do List operations.
/// This service contains shared authorization logic used across multiple handlers.
/// </summary>
public interface IAuthorizationService
{

    /// <summary>
    /// Asserts that the specified user has access rights to the given To-Do list.
    /// Throws an exception if the user is not authorized or if the list does not exist.
    /// </summary>
    /// <param name="userId">The identifier of the user requesting access.</param>
    /// <param name="listId">The identifier of the To-Do list.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <exception cref="UnauthorizedAccessException">Thrown if the user does not have permission to access or modify the list.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the list does not exist.</exception>
    Task AssertUserListAccessAsync(AccountId userId, ToDoListId listId, CancellationToken cancellationToken);
}