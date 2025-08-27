using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain.Accounts.Entities;
using Domain.Accounts.ValueObjects;

namespace Domain.Accounts.Repositories;

public interface IAccountRepository
{
    /// <summary>
    /// Adds a newly created account aggregate to the unit of work.
    /// </summary>
    void Add(Account account);

    /// <summary>
    /// Marks an existing account aggregate as modified in the unit of work.
    /// </summary>
    void Update(Account account);

    /// <summary>
    /// Removes an existing account aggregate from the unit of work.
    /// </summary>
    void Remove(Account account);

    /// <summary>
    /// Retrieves an account by its aggregate identifier or null when not found.
    /// </summary>
    Task<Account?> GetByIdAsync(AccountId accountId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves an account by its email or null when not found.
    /// </summary>
    Task<Account?> GetByEmailAsync(AccountEmail email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves an account by its public username or null when not found.
    /// </summary>
    Task<Account?> GetByUsernameAsync(AccountUsername username, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves an account linked to the specified external credential or null when not found.
    /// </summary>
    Task<Account?> GetByCredentialIdAsync(CredentialId credentialId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves accounts by their identifiers.
    /// </summary>
    Task<IReadOnlyCollection<Account>> GetByIdsAsync(IEnumerable<AccountId> accountIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns true when an account with the specified email exists.
    /// </summary>
    Task<bool> ExistsByEmailAsync(AccountEmail email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns true when an account with the specified username exists.
    /// </summary>
    Task<bool> ExistsByUsernameAsync(AccountUsername username, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns all accounts.
    /// </summary>
    Task<IReadOnlyCollection<Account>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches accounts using optional filters and pagination. Passing null ignores the filter.
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
