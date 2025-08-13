using Domain.Accounts.ValueObjects;

namespace Domain.Accounts;

/// <summary>
/// Represents a concrete user account in the system.
/// </summary>
public class User : Account
{
    /// <summary>
    /// Gets a value indicating whether the user account is active.
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="User"/> class.
    /// </summary>
    /// <param name="id">The unique user identifier.</param>
    /// <param name="email">The validated user email address.</param>
    /// <param name="username">The validated username.</param>
    /// <param name="name">The validated display name.</param>
    /// <param name="passwordHash">The password hash (never plaintext).</param>
    /// <param name="isActive">Indicates whether the user account is active.</param>
    public User(
        AccountId id,
        AccountEmail email,
        AccountUsername username,
        AccountName name,
        string passwordHash,
        bool isActive = false)
        : base(id, email, username, name, passwordHash)
    {
        IsActive = isActive;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="User"/> class.
    /// </summary>
    /// <param name="email">The validated user email address.</param>
    /// <param name="username">The validated username.</param>
    /// <param name="name">The validated display name.</param>
    /// <param name="passwordHash">The password hash (never plaintext).</param>
    /// <param name="isActive">Indicates whether the user account is active.</param>
    public User(
        AccountEmail email,
        AccountUsername username,
        AccountName name,
        string passwordHash,
        bool isActive = false)
        : this(AccountId.New(), email, username, name, passwordHash, isActive) {}

    /// <summary>
    /// Deactivates the user account.
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
    }

    /// <summary>
    /// Activates the user account.
    /// </summary>
    public void Activate()
    {
        IsActive = true;
    }
}
