using System.Threading;
using System.Threading.Tasks;
using Domain.Accounts.ValueObjects;

namespace Domain.Accounts.Policies.Interfaces;

/// <summary>
/// Policy that checks the uniqueness of account email and username.
/// </summary>
public interface IAccountUniquenessPolicy
{
    Task<bool> IsEmailUniqueAsync(AccountEmail email, CancellationToken cancellationToken);
    Task<bool> IsUsernameUniqueAsync(AccountUsername username, CancellationToken cancellationToken);
}
