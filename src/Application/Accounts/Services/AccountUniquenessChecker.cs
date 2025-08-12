using Domain.Accounts.ValueObjects;
using Domain.Accounts.Repositories;
using Domain.Accounts.Services.Interfaces;

namespace Application.Accounts.Services;

/// <summary>
/// Implementation of account uniqueness validation using the user repository.
/// </summary>
public sealed class AccountUniquenessChecker : IAccountUniquenessChecker
{
    private readonly IUserRepository _userRepository;

    public AccountUniquenessChecker(IUserRepository userRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    public async Task<bool> IsEmailUniqueAsync(AccountEmail email, CancellationToken cancellationToken)
    {
        var existingUser = await _userRepository.GetByEmailAsync(email, cancellationToken);
        return existingUser is null;
    }

    public async Task<bool> IsUsernameUniqueAsync(AccountUsername username, CancellationToken cancellationToken)
    {
        var existingUser = await _userRepository.GetByUsernameAsync(username, cancellationToken);
        return existingUser is null;
    }
}