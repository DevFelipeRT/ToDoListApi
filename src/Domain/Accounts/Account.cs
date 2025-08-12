using System;
using Domain.Accounts.ValueObjects;
using Domain.Accounts.Services.Interfaces;

namespace Domain.Accounts;

/// <summary>
/// Represents an abstract base account entity with identity, authentication and core attributes.
/// </summary>
public abstract class Account
{
    /// <summary>
    /// Gets the unique identifier for the account.
    /// </summary>
    public AccountId Id { get; private set; }

    /// <summary>
    /// Gets the account's email address.
    /// </summary>
    public AccountEmail Email { get; private set; }

    /// <summary>
    /// Gets the username associated with the account.
    /// </summary>
    public AccountUsername Username { get; private set; }

    /// <summary>
    /// Gets the display name of the account holder.
    /// </summary>
    public AccountName Name { get; private set; }

    /// <summary>
    /// Gets the password hash (never expose outside the domain).
    /// </summary>
    public string PasswordHash { get; private set; }

    /// <summary>
    /// Gets the account creation timestamp in UTC.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Gets the timestamp of the last login in UTC, or null if never logged in.
    /// </summary>
    public DateTime? LastLoginAt { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Account"/> class.
    /// </summary>
    /// <param name="id">The unique account identifier.</param>
    /// <param name="email">The validated account email address.</param>
    /// <param name="username">The validated username.</param>
    /// <param name="name">The validated display name.</param>
    /// <param name="passwordHash">The hashed password (never plaintext).</param>
    protected Account(AccountId id, AccountEmail email, AccountUsername username, AccountName name, string passwordHash)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        Username = username ?? throw new ArgumentNullException(nameof(username));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        PasswordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the display name of the account holder.
    /// </summary>
    public void UpdateName(AccountName newName)
    {
        Name = newName ?? throw new ArgumentNullException(nameof(newName));
    }

    /// <summary>
    /// Updates the email address of the account.
    /// </summary>
    public void UpdateEmail(AccountEmail newEmail)
    {
        Email = newEmail ?? throw new ArgumentNullException(nameof(newEmail));
    }

    /// <summary>
    /// Updates the username of the account.
    /// </summary>
    public void UpdateUsername(AccountUsername newUsername)
    {
        Username = newUsername ?? throw new ArgumentNullException(nameof(newUsername));
    }

    /// <summary>
    /// Updates the last login timestamp.
    /// </summary>
    public void UpdateLastLogin()
    {
        LastLoginAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the password hash for the account.
    /// </summary>
    /// <param name="newPasswordHash">The new password hash (never plaintext).</param>
    public void UpdatePassword(string newPasswordHash)
    {
        PasswordHash = newPasswordHash ?? throw new ArgumentNullException(nameof(newPasswordHash));
    }

    /// <summary>
    /// Validates the provided plaintext password against the stored hash.
    /// </summary>
    /// <param name="plainPassword">The plaintext password to validate.</param>
    /// <param name="hasher">The password hashing service.</param>
    /// <returns>True if valid, false otherwise.</returns>
    public bool ValidatePassword(string plainPassword, IPasswordHasher hasher)
    {
        if (hasher == null) throw new ArgumentNullException(nameof(hasher));
        return hasher.VerifyPassword(PasswordHash, plainPassword);
    }
}
