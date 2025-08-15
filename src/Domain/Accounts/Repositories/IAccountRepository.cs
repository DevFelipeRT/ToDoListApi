using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain.Accounts.Entities;
using Domain.Accounts.ValueObjects;

namespace Domain.Accounts.Repositories;

/// <summary>
/// Repository contract for persistence and retrieval of Account aggregates.
/// </summary>
public interface IAccountRepository
{
    /// <summary>
    /// Adds a new account aggregate to the data store.
    /// </summary>
    Task AddAsync(Account account, CancellationToken cancellationToken);

    /// <summary>
    /// Updates an existing account aggregate in the data store.
    /// </summary>
    Task UpdateAsync(Account account, CancellationToken cancellationToken);

    /// <summary>
    /// Gets an account by unique identifier.
    /// Returns null if not found.
    /// </summary>
    Task<Account?> GetByIdAsync(AccountId accountId, CancellationToken cancellationToken);

    /// <summary>
    /// Gets an account by email address.
    /// Returns null if not found.
    /// </summary>
    Task<Account?> GetByEmailAsync(AccountEmail email, CancellationToken cancellationToken);

    /// <summary>
    /// Gets an account by username.
    /// Returns null if not found.
    /// </summary>
    Task<Account?> GetByUsernameAsync(AccountUsername username, CancellationToken cancellationToken);

    /// <summary>
    /// Gets all accounts in the data store.
    /// </summary>
    Task<IReadOnlyCollection<Account>> GetAllAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Searches accounts by optional filters and pagination.
    /// Any filter parameter can be null.
    /// </summary>
    Task<IReadOnlyCollection<Account>> SearchAsync(
        AccountName? name = null,
        AccountUsername? username = null,
        AccountEmail? email = null,
        bool? isActive = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);
}
