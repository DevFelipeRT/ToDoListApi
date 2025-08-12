using System;

namespace Domain.Accounts.ValueObjects;

/// <summary>
/// Value Object that represents a validated display name for an Account.
/// </summary>
public sealed class AccountName : IEquatable<AccountName>
{
    public const int MaxLength = 60;

    public string Value { get; }

    public AccountName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Name must not be null or empty.", nameof(value));
        if (value.Length < 2 || value.Length > 60)
            throw new ArgumentException("Name must be between 2 and 60 characters.", nameof(value));
        if (!IsValidName(value))
            throw new ArgumentException("Name contains invalid characters.", nameof(value));

        Value = value.Trim();
    }

    /// <summary>
    /// Creates a new AccountName from a string, performing validation.
    /// </summary>
    public static AccountName FromString(string value) => new AccountName(value);

    private static bool IsValidName(string value)
    {
        // Only letters, spaces, hyphens, and apostrophes are allowed. Adjust as needed.
        foreach (var ch in value)
        {
            if (!(char.IsLetter(ch) || ch == ' ' || ch == '-' || ch == '\''))
                return false;
        }
        return true;
    }

    public override bool Equals(object? obj) => Equals(obj as AccountName);

    public bool Equals(AccountName? other) =>
        other is not null &&
        StringComparer.OrdinalIgnoreCase.Equals(Value, other.Value);

    public override int GetHashCode() =>
        StringComparer.OrdinalIgnoreCase.GetHashCode(Value);

    public override string ToString() => Value;

    public static bool operator ==(AccountName left, AccountName right) => Equals(left, right);
    public static bool operator !=(AccountName left, AccountName right) => !Equals(left, right);
}

