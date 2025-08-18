using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Domain.Accounts.Entities;
using Domain.Accounts.ValueObjects;
using Domain.Accounts.Repositories;

namespace Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for Account aggregate using Entity Framework Core.
/// </summary>
public class AccountRepository : IAccountRepository
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="AccountRepository"/> class.
    /// </summary>
    /// <param name="context">The database context to be used for data operations.</param>
    public AccountRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public void Add(Account account)
    {
        _context.Accounts.Add(account);
    }

    /// <inheritdoc/>
    public void Update(Account account)
    {
        _context.Accounts.Update(account);
    }

    /// <inheritdoc/>
    public async Task<Account?> GetByIdAsync(AccountId accountId, CancellationToken cancellationToken)
    {
        return await _context.Accounts
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == accountId, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<Account?> GetByEmailAsync(AccountEmail email, CancellationToken cancellationToken)
    {
        return await _context.Accounts
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Email == email, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<Account?> GetByUsernameAsync(AccountUsername username, CancellationToken cancellationToken)
    {
        return await _context.Accounts
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Username == username, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyCollection<Account>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Accounts
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyCollection<Account>> SearchAsync(
        AccountName? name = null,
        AccountUsername? username = null,
        AccountEmail? email = null,
        bool? isActive = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Accounts.AsNoTracking().AsQueryable();

        if (name is not null && !string.IsNullOrWhiteSpace(name.ToString()))
            query = query.Where(x => x.Name.Value.Contains(name.ToString()));
        if (username is not null)
            query = query.Where(x => x.Username == username);
        if (email is not null)
            query = query.Where(x => x.Email == email);
        if (isActive.HasValue)
            query = query.Where(x => x.ActivatedAt.HasValue == isActive.Value);

        return await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<Account?> GetForActivationByEmailAsync(AccountEmail email, CancellationToken cancellationToken)
    {
        // Tracked aggregate with activation tokens loaded via backing-field navigation.
        return await _context.Accounts
            .Include("_activationTokens")
            .FirstOrDefaultAsync(a => a.Email == email, cancellationToken);
    }
}
