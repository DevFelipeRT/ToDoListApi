using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain.Accounts.Entities;
using Domain.Accounts.Repositories;
using Domain.Accounts.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Accounts;

public sealed class AccountRepository : IAccountRepository
{
    private readonly ApplicationDbContext _db;

    public AccountRepository(ApplicationDbContext db)
    {
        _db = db ?? throw new ArgumentNullException(nameof(db));
    }

    /// <inheritdoc />
    public void Add(Account account)
    {
        if (account is null) throw new ArgumentNullException(nameof(account));
        _db.Set<Account>().Add(account);
    }

    /// <inheritdoc />
    public void Update(Account account)
    {
        if (account is null) throw new ArgumentNullException(nameof(account));
        _db.Set<Account>().Update(account);
    }

    /// <inheritdoc />
    public void Remove(Account account)
    {
        if (account is null) throw new ArgumentNullException(nameof(account));
        _db.Set<Account>().Remove(account);
    }

    /// <inheritdoc />
    public async Task<Account?> GetByIdAsync(AccountId accountId, CancellationToken cancellationToken = default)
    {
        return await Query().FirstOrDefaultAsync(a => a.Id == accountId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Account?> GetByEmailAsync(AccountEmail email, CancellationToken cancellationToken = default)
    {
        return await Query().FirstOrDefaultAsync(a => a.Email.Value == email.Value, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Account?> GetByUsernameAsync(AccountUsername username, CancellationToken cancellationToken = default)
    {
        return await Query().FirstOrDefaultAsync(a => a.Username.Value == username.Value, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Account?> GetByCredentialIdAsync(CredentialId credentialId, CancellationToken cancellationToken = default)
    {
        return await Query().FirstOrDefaultAsync(
            a => a.CredentialId != null &&
                 a.CredentialId.Kind == credentialId.Kind &&
                 a.CredentialId.Value == credentialId.Value,
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<Account>> GetByIdsAsync(IEnumerable<AccountId> accountIds, CancellationToken cancellationToken = default)
    {
        if (accountIds is null) throw new ArgumentNullException(nameof(accountIds));
        var ids = accountIds.ToArray();
        var list = await Query().Where(a => ids.Contains(a.Id)).ToListAsync(cancellationToken);
        return list;
    }

    /// <inheritdoc />
    public async Task<bool> ExistsByEmailAsync(AccountEmail email, CancellationToken cancellationToken = default)
    {
        return await _db.Set<Account>().AnyAsync(a => a.Email.Value == email.Value, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> ExistsByUsernameAsync(AccountUsername username, CancellationToken cancellationToken = default)
    {
        return await _db.Set<Account>().AnyAsync(a => a.Username.Value == username.Value, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<Account>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var list = await Query().OrderBy(a => a.CreatedAt).ToListAsync(cancellationToken);
        return list;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<Account>> SearchAsync(
        AccountName? name = null,
        AccountUsername? username = null,
        AccountEmail? email = null,
        bool? isActive = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;

        IQueryable<Account> query = Query();

        if (name is not null)
            query = query.Where(a => a.Name.Value.Contains(name.Value));

        if (username is not null)
            query = query.Where(a => a.Username.Value.Contains(username.Value));

        if (email is not null)
            query = query.Where(a => a.Email.Value.Contains(email.Value));

        if (isActive.HasValue)
            query = query.Where(a => a.ActivatedAt.HasValue == isActive.Value);

        var list = await query
            .OrderBy(a => a.Name.Value)
            .ThenBy(a => a.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return list;
    }

    /// <summary>
    /// Base query for accounts with tracking enabled (required for aggregate modifications elsewhere).
    /// Adjust to AsNoTracking() if read-only scenarios dominate and attach on mutation.
    /// </summary>
    private IQueryable<Account> Query() => _db.Set<Account>();

}
