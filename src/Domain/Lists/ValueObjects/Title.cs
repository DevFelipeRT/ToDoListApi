namespace Domain.Lists.ValueObjects;

/// <summary>
/// Represents the title of a To-Do item or list.
/// This Value Object guarantees the title is always valid and compares ignoring case and extra spaces.
/// </summary>
public sealed class Title : IEquatable<Title>
{
    /// <summary>
    /// The maximum allowed length for a title.
    /// </summary>
    public const int MaxLength = 150;

    /// <summary>
    /// Gets the normalized value of the title.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Title"/> class.
    /// </summary>
    /// <param name="value">The string value of the title.</param>
    /// <exception cref="ArgumentNullException">Thrown when the value is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the value is empty, whitespace, or exceeds max length.</exception>
    public Title(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var normalized = value.Trim();
        if (string.IsNullOrWhiteSpace(normalized))
            throw new ArgumentException("Title cannot be empty or whitespace.", nameof(value));
        if (normalized.Length > MaxLength)
            throw new ArgumentException($"Title cannot be longer than {MaxLength} characters.", nameof(value));

        Value = normalized;
    }

    /// <summary>
    /// Implicitly converts a Title object to its string representation.
    /// </summary>
    public static implicit operator string(Title title) => title.Value;

    /// <summary>
    /// Explicitly converts a string to a Title object.
    /// </summary>
    public static explicit operator Title(string value) => new(value);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as Title);

    /// <inheritdoc/>
    public bool Equals(Title? other) =>
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
