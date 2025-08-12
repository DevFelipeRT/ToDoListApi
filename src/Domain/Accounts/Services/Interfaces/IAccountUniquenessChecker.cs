using System.Threading;
using System.Threading.Tasks;
using Domain.Accounts.ValueObjects;

namespace Domain.Accounts.Services.Interfaces;

/// <summary>
/// Service that checks the uniqueness of account email and username.
/// </summary>
public interface IAccountUniquenessChecker
{
    Task<bool> IsEmailUniqueAsync(AccountEmail email, CancellationToken cancellationToken);
    Task<bool> IsUsernameUniqueAsync(AccountUsername username, CancellationToken cancellationToken);
}
