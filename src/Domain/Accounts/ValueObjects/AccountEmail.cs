using System;
using System.Text.RegularExpressions;

namespace Domain.Accounts.ValueObjects;

/// <summary>
/// Value Object that represents a validated email address for an Account.
/// </summary>
public sealed class AccountEmail : IEquatable<AccountEmail>
{
    public const int MaxLength = 100;

    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public string Value { get; }

    public AccountEmail(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email cannot be null or empty.", nameof(value));
        if (!EmailRegex.IsMatch(value))
            throw new ArgumentException("Invalid email format.", nameof(value));

        Value = value;
    }

    /// <summary>
    /// Creates a new AccountEmail from a string, performing validation.
    /// </summary>
    public static AccountEmail FromString(string value) => new AccountEmail(value);

    public override bool Equals(object? obj) => Equals(obj as AccountEmail);

    public bool Equals(AccountEmail? other) =>
        other is not null &&
        StringComparer.OrdinalIgnoreCase.Equals(Value, other.Value);

    public override int GetHashCode() =>
        StringComparer.OrdinalIgnoreCase.GetHashCode(Value);

    public override string ToString() => Value;

    public static bool operator ==(AccountEmail left, AccountEmail right) => Equals(left, right);
    public static bool operator !=(AccountEmail left, AccountEmail right) => !Equals(left, right);
}
