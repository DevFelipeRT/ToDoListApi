using System.Threading;
using System.Threading.Tasks;
using Domain.Accounts.Entities;
using Domain.Accounts.ValueObjects;

namespace Application.Accounts.Services.Interfaces;

public interface IAuthenticationService
{
    Task<Account?> AuthenticateAsync(
        AccountEmail email,
        string plainPassword,
        CancellationToken cancellationToken = default);

    Task<Account?> AuthenticateByUsernameAsync(
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
