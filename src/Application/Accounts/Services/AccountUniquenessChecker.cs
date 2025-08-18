using Domain.Accounts.ValueObjects;
using Domain.Accounts.Repositories;
using Domain.Accounts.Policies.Interfaces;


namespace Application.Accounts.Services;

/// <summary>
/// Implementation of account uniqueness validation using the account repository.
/// </summary>
public sealed class AccountUniquenessChecker : IAccountUniquenessPolicy
{
    private readonly IAccountRepository _accountRepository;

    public AccountUniquenessChecker(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
    }

    public async Task<bool> IsEmailUniqueAsync(AccountEmail email, CancellationToken cancellationToken)
    {
        var existingAccount = await _accountRepository.GetByEmailAsync(email, cancellationToken);
        return existingAccount is null;
    }

    public async Task<bool> IsUsernameUniqueAsync(AccountUsername username, CancellationToken cancellationToken)
    {
        var existingAccount = await _accountRepository.GetByUsernameAsync(username, cancellationToken);
        return existingAccount is null;
    }
}