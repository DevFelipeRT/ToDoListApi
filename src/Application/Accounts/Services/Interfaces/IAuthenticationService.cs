using System.Threading;
using System.Threading.Tasks;
using Domain.Accounts;
using Domain.Accounts.ValueObjects;

namespace Application.Accounts.Services.Interfaces;

public interface IAuthenticationService
{
    Task<User?> AuthenticateAsync(
        AccountEmail email,
        string plainPassword,
        CancellationToken cancellationToken = default);

    Task<User?> AuthenticateByUsernameAsync(
        AccountUsername username,
        string plainPassword,
        CancellationToken cancellationToken = default);

    Task<bool> ValidateCredentialsAsync(
        AccountEmail email,
        string plainPassword,
        CancellationToken cancellationToken = default);

    Task LogoutAsync(
        AccountId userId,
        CancellationToken cancellationToken = default);
}
