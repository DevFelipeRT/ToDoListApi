namespace Domain.Lists.ValueObjects;

/// <summary>
/// Value Object that represents the unique identifier for a To-Do item.
/// Ensures immutability, value-based equality, and prevents empty Guid.
/// </summary>
public sealed class ToDoItemId : IEquatable<ToDoItemId>
{
    /// <summary>
    /// Gets the Guid value of the identifier.
    /// </summary>
    public Guid Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ToDoItemId"/> class.
    /// </summary>
    /// <param name="value">The Guid value to use as identifier.</param>
    /// <exception cref="ArgumentException">Thrown when the value is Guid.Empty.</exception>
    public ToDoItemId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("ToDoItemId cannot be empty.", nameof(value));
        Value = value;
    }

    /// <summary>
    /// Creates a new unique ToDoItemId.
    /// </summary>
    /// <returns>A new ToDoItemId with a new Guid.</returns>
    public static ToDoItemId New() => new ToDoItemId(Guid.NewGuid());

    /// <summary>
    /// Creates a ToDoItemId from a Guid string representation.
    /// </summary>
    /// <param name="value">The string representation of the Guid.</param>
    /// <returns>A ToDoItemId instance.</returns>
    /// <exception cref="ArgumentException">Thrown when the string is not a valid Guid.</exception>
    public static ToDoItemId FromString(string value)
    {
        if (!Guid.TryParse(value, out var guid))
            throw new ArgumentException("Invalid GUID format.", nameof(value));
        return new ToDoItemId(guid);
    }

    /// <summary>
    /// Creates a ToDoItemId from a Guid.
    /// </summary>
    /// <param name="value">The Guid value to use as identifier.</param>
    /// <returns>A ToDoItemId instance.</returns>
    public static ToDoItemId FromGuid(Guid value) => new ToDoItemId(value);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as ToDoItemId);

    /// <inheritdoc/>
    public bool Equals(ToDoItemId? other) => other is not null && Value.Equals(other.Value);

    /// <inheritdoc/>
    public override int GetHashCode() => Value.GetHashCode();

    /// <summary>
    /// Returns the string representation of the ToDoItemId.
    /// </summary>
    public override string ToString() => Value.ToString();

    /// <summary>
    /// Equality operator.
    /// </summary>
    public static bool operator ==(ToDoItemId? left, ToDoItemId? right) => Equals(left, right);

    /// <summary>
    /// Inequality operator.
    /// </summary>
    public static bool operator !=(ToDoItemId? left, ToDoItemId? right) => !Equals(left, right);
}
