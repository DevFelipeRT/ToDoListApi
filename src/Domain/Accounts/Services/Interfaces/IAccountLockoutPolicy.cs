using System.Threading;
using System.Threading.Tasks;
using Domain.Accounts.ValueObjects;

namespace Domain.Accounts.Services.Interfaces;

/// <summary>
/// Service that handles account lockout policy for failed authentication attempts.
/// </summary>
public interface IAccountLockoutPolicy
{
    Task RegisterFailedAttempt(AccountId accountId, CancellationToken cancellationToken);
    Task<bool> IsLockedOut(AccountId accountId, CancellationToken cancellationToken);
    Task ResetLockout(AccountId accountId, CancellationToken cancellationToken);
}
