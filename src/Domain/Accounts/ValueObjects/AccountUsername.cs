using System;

namespace Domain.Accounts.ValueObjects;

/// <summary>
/// Value Object that represents a validated username for an Account.
/// </summary>
public sealed class AccountUsername : IEquatable<AccountUsername>
{
    public const int MaxLength = 32;

    public string Value { get; }

    public AccountUsername(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Username cannot be null or empty.", nameof(value));
        if (value.Length < 3 || value.Length > 32)
            throw new ArgumentException("Username must be between 3 and 32 characters.", nameof(value));
        if (!IsValidUsername(value))
            throw new ArgumentException("Username contains invalid characters.", nameof(value));

        Value = value;
    }

    /// <summary>
    /// Creates a new AccountUsername from a string, performing validation.
    /// </summary>
    public static AccountUsername FromString(string value) => new AccountUsername(value);

    private static bool IsValidUsername(string value)
    {
        // Only allow alphanumeric characters, underscores, and dots
        foreach (var ch in value)
        {
            if (!(char.IsLetterOrDigit(ch) || ch == '_' || ch == '.'))
                return false;
        }
        return true;
    }

    public override bool Equals(object? obj) => Equals(obj as AccountUsername);

    public bool Equals(AccountUsername? other) =>
        other is not null &&
        StringComparer.OrdinalIgnoreCase.Equals(Value, other.Value);

    public override int GetHashCode() =>
        StringComparer.OrdinalIgnoreCase.GetHashCode(Value);

    public override string ToString() => Value;

    public static bool operator ==(AccountUsername left, AccountUsername right) => Equals(left, right);
    public static bool operator !=(AccountUsername left, AccountUsername right) => !Equals(left, right);
}
