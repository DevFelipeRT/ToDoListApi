namespace Domain.Lists.ValueObjects;

/// <summary>
/// Represents the title of a To-Do item or list.
/// This Value Object guarantees the title is always valid and compares ignoring case and extra spaces.
/// </summary>
public sealed class Description
{
    /// <summary>
    /// The maximum allowed length for a description.
    /// </summary>
    public const int MaxLength = 500;

    /// <summary>
    /// Gets the normalized value of the description.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Parameterless constructor required by Entity Framework Core for materialization.
    /// Do not use directly in business logic.
    /// </summary>
    private Description()
    {
        Value = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Description"/> class with a validated value.
    /// </summary>
    /// <param name="value">The string value of the description.</param>
    /// <exception cref="ArgumentException">Thrown when the value exceeds max length.</exception>
    public Description(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var normalized = value.Trim();
        if (normalized.Length > MaxLength)
            throw new ArgumentException($"Description cannot be longer than {MaxLength} characters.", nameof(value));

        Value = normalized;
    }

    /// <summary>
    /// Implicitly converts a Description object to its string representation.
    /// </summary>
    public static implicit operator string(Description description) => description.Value;

    /// <summary>
    /// Explicitly converts a string to a Description object.
    /// </summary>
    public static explicit operator Description(string value) => new(value);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as Description);

    /// <inheritdoc/>
    public bool Equals(Description? other) =>
        other is not null &&
        string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);

    /// <inheritdoc/>
    public override int GetHashCode() =>
        StringComparer.OrdinalIgnoreCase.GetHashCode(Value);

    /// <summary>
    /// Returns the string representation of the title.
    /// </summary>
    public override string ToString() => Value;
}
