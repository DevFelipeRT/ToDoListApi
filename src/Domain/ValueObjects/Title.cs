namespace ToDoList.Domain.ValueObjects;

/// <summary>
/// Represents the title of a To-Do item.
/// This Value Object ensures that the title is always in a valid state.
/// </summary>
public sealed class Title : IEquatable<Title>
{
    /// <summary>
    /// The maximum allowed length for a title.
    /// </summary>
    public const int MaxLength = 150;

    /// <summary>
    /// Gets the value of the title.
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

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Title cannot be empty or whitespace.", nameof(value));
        }

        if (value.Length > MaxLength)
        {
            throw new ArgumentException($"Title cannot be longer than {MaxLength} characters.", nameof(value));
        }
        
        Value = value;
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
    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj is Title other && Equals(other);
    }

    /// <inheritdoc/>
    public bool Equals(Title? other) => Value == other?.Value;

    /// <inheritdoc/>
    public override int GetHashCode() => Value.GetHashCode();
}