using System;
using System.Threading;
using System.Threading.Tasks;
using Domain.Accounts;
using Domain.Accounts.Services.Interfaces;
using Domain.Accounts.ValueObjects;
using Domain.Accounts.Repositories;
using Application.Accounts.Services.Interfaces;

namespace Application.Accounts.Services;

/// <summary>
/// Provides user authentication functionality, including account lockout, timing attack protection, 
/// and secure password verification.
/// </summary>
public sealed class AuthenticationService : IAuthenticationService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IAccountLockoutPolicy _lockoutPolicy;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticationService"/> class.
    /// </summary>
    /// <param name="userRepository">The user repository instance.</param>
    /// <param name="passwordHasher">The password hashing service.</param>
    /// <param name="lockoutPolicy">The account lockout policy service.</param>
    public AuthenticationService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IAccountLockoutPolicy lockoutPolicy)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        _lockoutPolicy = lockoutPolicy ?? throw new ArgumentNullException(nameof(lockoutPolicy));
    }

    /// <summary>
    /// Authenticates a user by email and password. Applies lockout policy and protects against timing and enumeration attacks.
    /// </summary>
    /// <param name="email">The user's email value object.</param>
    /// <param name="plainPassword">The user's plain text password.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>
    /// The <see cref="User"/> entity if authentication is successful and the account is not locked or inactive; otherwise, null.
    /// </returns>
    public async Task<User?> AuthenticateAsync(
        AccountEmail email,
        string plainPassword,
        CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByEmailAsync(email, cancellationToken);

        // Account enumeration and timing attack protection
        if (user == null)
        {
            // Perform dummy password verification to simulate timing
            _passwordHasher.VerifyPassword("DummyHashedPasswordValue:DummyHash:100000", plainPassword);
            return null;
        }

        if (!user.IsActive)
            return null;

        // Check if account is locked out due to failed attempts
        if (await _lockoutPolicy.IsLockedOut(user.Id, cancellationToken))
            return null;

        var isPasswordValid = _passwordHasher.VerifyPassword(user.PasswordHash, plainPassword);

        if (!isPasswordValid)
        {
            await _lockoutPolicy.RegisterFailedAttempt(user.Id, cancellationToken);
            return null;
        }

        await _lockoutPolicy.ResetLockout(user.Id, cancellationToken);

        user.UpdateLastLogin();
        await _userRepository.UpdateAsync(user, cancellationToken);

        return user;
    }

    /// <summary>
    /// Authenticates a user by username and password. Applies lockout policy and protects against timing and enumeration attacks.
    /// </summary>
    /// <param name="username">The user's username value object.</param>
    /// <param name="plainPassword">The user's plain text password.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>
    /// The <see cref="User"/> entity if authentication is successful and the account is not locked or inactive; otherwise, null.
    /// </returns>
    public async Task<User?> AuthenticateByUsernameAsync(
        AccountUsername username,
        string plainPassword,
        CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByUsernameAsync(username, cancellationToken);

        if (user == null)
        {
            _passwordHasher.VerifyPassword("DummyHashedPasswordValue:DummyHash:100000", plainPassword);
            return null;
        }

        if (!user.IsActive)
            return null;

        if (await _lockoutPolicy.IsLockedOut(user.Id, cancellationToken))
            return null;

        var isPasswordValid = _passwordHasher.VerifyPassword(user.PasswordHash, plainPassword);

        if (!isPasswordValid)
        {
            await _lockoutPolicy.RegisterFailedAttempt(user.Id, cancellationToken);
            return null;
        }

        await _lockoutPolicy.ResetLockout(user.Id, cancellationToken);

        user.UpdateLastLogin();
        await _userRepository.UpdateAsync(user, cancellationToken);

        return user;
    }

    /// <summary>
    /// Validates user credentials for email and password without updating login information or lockout state.
    /// </summary>
    /// <param name="email">The user's email value object.</param>
    /// <param name="plainPassword">The user's plain text password.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>True if credentials are valid and the account is active; otherwise, false.</returns>
    public async Task<bool> ValidateCredentialsAsync(
        AccountEmail email,
        string plainPassword,
        CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByEmailAsync(email, cancellationToken);

        if (user == null || !user.IsActive)
            return false;

        return _passwordHasher.VerifyPassword(user.PasswordHash, plainPassword);
    }

    /// <summary>
    /// Performs logout logic for the specified user. (No-op for stateless JWT authentication.)
    /// </summary>
    /// <param name="userId">The user's account identifier.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A completed task.</returns>
    public async Task LogoutAsync(
        AccountId userId,
        CancellationToken cancellationToken = default)
    {
        // No-op for JWT stateless authentication.
        await Task.CompletedTask;
    }
}
