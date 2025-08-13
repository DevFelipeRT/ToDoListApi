using System;

namespace Domain.Accounts.ValueObjects;

/// <summary>
/// Value Object that represents the unique identifier for an Account.
/// </summary>
public sealed class AccountId : IEquatable<AccountId>
{
    public Guid Value { get; }

    private AccountId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("AccountId cannot be empty.", nameof(value));

        Value = value;
    }

    /// <summary>
    /// Creates a new AccountId from a Guid.
    /// </summary>
    public static AccountId FromGuid(Guid value) => new AccountId(value);

    /// <summary>
    /// Creates a new AccountId from a string (expects a Guid string).
    /// </summary>
    public static AccountId FromString(string value)
    {
        if (!Guid.TryParse(value, out var guid))
            throw new ArgumentException("Invalid GUID format.", nameof(value));

        return new AccountId(guid);
    }

    /// <summary>
    /// Creates a new AccountId with a new unique value.
    /// </summary>
    public static AccountId New() => new AccountId(Guid.NewGuid());

    public override bool Equals(object? obj) => Equals(obj as AccountId);

    public bool Equals(AccountId? other) => other is not null && Value.Equals(other.Value);

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    // Operators for equality
    public static bool operator ==(AccountId left, AccountId right) => Equals(left, right);
    public static bool operator !=(AccountId left, AccountId right) => !Equals(left, right);
}
