using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain.Accounts.ValueObjects;

namespace Domain.Accounts.Repositories;

/// <summary>
/// Repository contract for persistence and retrieval of User aggregates.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Adds a new user aggregate to the data store.
    /// </summary>
    Task AddAsync(User user, CancellationToken cancellationToken);

    /// <summary>
    /// Updates an existing user aggregate in the data store.
    /// </summary>
    Task UpdateAsync(User user, CancellationToken cancellationToken);

    /// <summary>
    /// Gets a user by unique identifier.
    /// Returns null if not found.
    /// </summary>
    Task<User?> GetByIdAsync(AccountId userId, CancellationToken cancellationToken);

    /// <summary>
    /// Gets a user by email address.
    /// Returns null if not found.
    /// </summary>
    Task<User?> GetByEmailAsync(AccountEmail email, CancellationToken cancellationToken);

    /// <summary>
    /// Gets a user by username.
    /// Returns null if not found.
    /// </summary>
    Task<User?> GetByUsernameAsync(AccountUsername username, CancellationToken cancellationToken);

    /// <summary>
    /// Gets all users in the data store.
    /// </summary>
    Task<IReadOnlyCollection<User>> GetAllAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Searches users by optional filters and pagination.
    /// Any filter parameter can be null.
    /// </summary>
    Task<IReadOnlyCollection<User>> SearchAsync(
        AccountName? name = null,
        AccountUsername? username = null,
        AccountEmail? email = null,
        bool? isActive = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);
}
