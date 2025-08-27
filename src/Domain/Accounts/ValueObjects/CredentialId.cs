using System;

namespace Domain.Accounts.ValueObjects;

/// <summary>
/// Value Object that encapsulates an external credential identifier (IAM subject)
/// as a tagged union. Stores a single canonical textual representation alongside
/// its kind. Immutable and comparable by value.
/// </summary>
public sealed class CredentialId : IEquatable<CredentialId>
{
    /// <summary>
    /// Identifies the concrete format of the credential identifier.
    /// </summary>
    public CredentialIdKind Kind { get; }

    /// <summary>
    /// Canonical textual representation of the identifier.
    /// For <see cref="CredentialIdKind.Guid"/> it is the "D" format.
    /// </summary>
    public string Value { get; }

    private CredentialId(CredentialIdKind kind, string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(value));

        Kind = kind;
        Value = kind switch
        {
            CredentialIdKind.Guid   => NormalizeGuid(value),
            CredentialIdKind.String => value.Trim(),
            _                       => value.Trim()
        };
    }

    /// <summary>
    /// Creates a <see cref="CredentialId"/> from a <see cref="Guid"/>.
    /// </summary>
    public static CredentialId FromGuid(Guid id) => new(CredentialIdKind.Guid, id.ToString("D"));

    /// <summary>
    /// Creates a <see cref="CredentialId"/> from an arbitrary string subject.
    /// </summary>
    public static CredentialId FromString(string id) => new(CredentialIdKind.String, id);

    /// <summary>
    /// Creates a <see cref="CredentialId"/> by automatically inferring the kind.
    /// If the value parses as a Guid, it will be <see cref="CredentialIdKind.Guid"/>;
    /// otherwise it defaults to <see cref="CredentialIdKind.String"/>.
    /// </summary>
    public static CredentialId FromValue(string value)
    {
        if (Guid.TryParse(value, out var g))
            return FromGuid(g);

        return FromString(value);
    }

    /// <summary>
    /// Attempts to extract the underlying <see cref="Guid"/> when the kind is <see cref="CredentialIdKind.Guid"/>.
    /// </summary>
    public bool TryGetGuid(out Guid guid)
    {
        if (Kind == CredentialIdKind.Guid && Guid.TryParse(Value, out guid)) return true;
        guid = default;
        return false;
    }

    /// <summary>
    /// Returns the canonical textual representation of the identifier.
    /// </summary>
    public override string ToString() => Value;

    /// <summary>
    /// Value equality by <see cref="Kind"/> and canonical <see cref="Value"/>.
    /// </summary>
    public bool Equals(CredentialId? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Kind == other.Kind && string.Equals(Value, other.Value, StringComparison.Ordinal);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is CredentialId other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine((int)Kind, Value);

    private static string NormalizeGuid(string input)
    {
        if (!Guid.TryParse(input, out var g))
            throw new ArgumentException("Invalid GUID value.", nameof(input));
        return g.ToString("D");
    }
}
