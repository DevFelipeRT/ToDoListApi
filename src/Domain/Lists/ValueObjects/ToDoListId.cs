namespace Domain.Lists.ValueObjects;

/// <summary>
/// Value Object that represents the unique identifier for a To-Do list.
/// Ensures immutability, value-based equality, and prevents empty Guid.
/// </summary>
public sealed class ToDoListId : IEquatable<ToDoListId>
{
    /// <summary>
    /// Gets the Guid value of the identifier.
    /// </summary>
    public Guid Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ToDoListId"/> class.
    /// </summary>
    /// <param name="value">The Guid value to use as identifier.</param>
    /// <exception cref="ArgumentException">Thrown when the value is Guid.Empty.</exception>
    public ToDoListId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("ToDoListId cannot be empty.", nameof(value));
        Value = value;
    }

    /// <summary>
    /// Creates a new unique ToDoListId.
    /// </summary>
    /// <returns>A new ToDoListId with a new Guid.</returns>
    public static ToDoListId New() => new ToDoListId(Guid.NewGuid());

    /// <summary>
    /// Creates a ToDoListId from a Guid string representation.
    /// </summary>
    /// <param name="value">The string representation of the Guid.</param>
    /// <returns>A ToDoListId instance.</returns>
    /// <exception cref="ArgumentException">Thrown when the string is not a valid Guid.</exception>
    public static ToDoListId FromString(string value)
    {
        if (!Guid.TryParse(value, out var guid))
            throw new ArgumentException("Invalid GUID format.", nameof(value));
        return new ToDoListId(guid);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as ToDoListId);

    /// <inheritdoc/>
    public bool Equals(ToDoListId? other) => other is not null && Value.Equals(other.Value);

    /// <inheritdoc/>
    public override int GetHashCode() => Value.GetHashCode();

    /// <summary>
    /// Returns the string representation of the ToDoListId.
    /// </summary>
    public override string ToString() => Value.ToString();

    /// <summary>
    /// Equality operator.
    /// </summary>
    public static bool operator ==(ToDoListId? left, ToDoListId? right) => Equals(left, right);

    /// <summary>
    /// Inequality operator.
    /// </summary>
    public static bool operator !=(ToDoListId? left, ToDoListId? right) => !Equals(left, right);
}
