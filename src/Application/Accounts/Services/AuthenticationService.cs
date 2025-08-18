using System;
using System.Threading;
using System.Threading.Tasks;
using Domain.Accounts.Services.Interfaces;
using Domain.Accounts.ValueObjects;
using Domain.Accounts.Repositories;
using Domain.Accounts.Policies.Interfaces;
using Application.Accounts.Services.Interfaces;
using Domain.Accounts.Entities;

namespace Application.Accounts.Services;

/// <summary>
/// Provides account authentication functionality, including account lockout, timing attack protection, 
/// and secure password verification.
/// </summary>
public sealed class AuthenticationService : IAuthenticationService
{
    private readonly IAccountRepository _accountRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IAccountLockoutPolicy _lockoutPolicy;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticationService"/> class.
    /// </summary>
    /// <param name="accountRepository">The account repository instance.</param>
    /// <param name="passwordHasher">The password hashing service.</param>
    /// <param name="lockoutPolicy">The account lockout policy service.</param>
    public AuthenticationService(
        IAccountRepository accountRepository,
        IPasswordHasher passwordHasher,
        IAccountLockoutPolicy lockoutPolicy)
    {
        _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        _lockoutPolicy = lockoutPolicy ?? throw new ArgumentNullException(nameof(lockoutPolicy));
    }

    /// <summary>
    /// Authenticates an account by email and password. Applies lockout policy and protects against timing and enumeration attacks.
    /// </summary>
    /// <param name="email">The account's email value object.</param>
    /// <param name="plainPassword">The account's plain text password.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>
    /// The <see cref="Account"/> entity if authentication is successful and the account is not locked or inactive; otherwise, null.
    /// </returns>
    public async Task<Account?> AuthenticateAsync(
        AccountEmail email,
        string plainPassword,
        CancellationToken cancellationToken = default)
    {
        var account = await _accountRepository.GetByEmailAsync(email, cancellationToken);

        // Account enumeration and timing attack protection
        if (account == null)
        {
            // Perform dummy password verification to simulate timing
            _passwordHasher.VerifyPassword("DummyHashedPasswordValue:DummyHash:100000", plainPassword);
            return null;
        }

        if (!account.ActivationTokens.Any())
            return null;

        // Check if account is locked out due to failed attempts
        if (await _lockoutPolicy.IsLockedOut(account.Id, cancellationToken))
            return null;

        var isPasswordValid = _passwordHasher.VerifyPassword(account.PasswordHash, plainPassword);

        if (!isPasswordValid)
        {
            await _lockoutPolicy.RegisterFailedAttempt(account.Id, cancellationToken);
            return null;
        }

        await _lockoutPolicy.ResetLockout(account.Id, cancellationToken);

        account.UpdateLastLogin();
        await _accountRepository.UpdateAsync(account, cancellationToken);

        return account;
    }

    /// <summary>
    /// Authenticates an account by username and password. Applies lockout policy and protects against timing and enumeration attacks.
    /// </summary>
    /// <param name="username">The account's username value object.</param>
    /// <param name="plainPassword">The account's plain text password.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>
    /// The <see cref="Account"/> entity if authentication is successful and the account is not locked or inactive; otherwise, null.
    /// </returns>
    public async Task<Account?> AuthenticateByUsernameAsync(
        AccountUsername username,
        string plainPassword,
        CancellationToken cancellationToken = default)
    {
        var account = await _accountRepository.GetByUsernameAsync(username, cancellationToken);

        if (account == null)
        {
            _passwordHasher.VerifyPassword("DummyHashedPasswordValue:DummyHash:100000", plainPassword);
            return null;
        }

        if (!account.ActivationTokens.Any())
            return null;

        if (await _lockoutPolicy.IsLockedOut(account.Id, cancellationToken))
            return null;

        var isPasswordValid = _passwordHasher.VerifyPassword(account.PasswordHash, plainPassword);

        if (!isPasswordValid)
        {
            await _lockoutPolicy.RegisterFailedAttempt(account.Id, cancellationToken);
            return null;
        }

        await _lockoutPolicy.ResetLockout(account.Id, cancellationToken);

        account.UpdateLastLogin();
        await _accountRepository.UpdateAsync(account, cancellationToken);

        return account;
    }

    /// <summary>
    /// Validates account credentials for email and password without updating login information or lockout state.
    /// </summary>
    /// <param name="email">The account's email value object.</param>
    /// <param name="plainPassword">The account's plain text password.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>True if credentials are valid and the account is active; otherwise, false.</returns>
    public async Task<bool> ValidateCredentialsAsync(
        AccountEmail email,
        string plainPassword,
        CancellationToken cancellationToken = default)
    {
        var account = await _accountRepository.GetByEmailAsync(email, cancellationToken);

        if (account == null || !account.ActivationTokens.Any())
            return false;

        return _passwordHasher.VerifyPassword(account.PasswordHash, plainPassword);
    }

    /// <summary>
    /// Performs logout logic for the specified account. (No-op for stateless JWT authentication.)
    /// </summary>
    /// <param name="accountId">The account's account identifier.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A completed task.</returns>
    public async Task LogoutAsync(
        AccountId accountId,
        CancellationToken cancellationToken = default)
    {
        // No-op for JWT stateless authentication.
        await Task.CompletedTask;
    }
}
