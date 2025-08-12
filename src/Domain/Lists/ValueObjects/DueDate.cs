using System;

namespace Domain.Lists.ValueObjects
{
    /// <summary>
    /// Represents the due date of a to-do item as a Value Object.
    /// This class enforces domain invariants and encapsulates all logic related to due dates.
    /// </summary>
    public sealed class DueDate : IEquatable<DueDate>
    {
        /// <summary>
        /// Gets the due date value in UTC.
        /// </summary>
        public DateTime Value { get; }

        /// <summary>
        /// Indicates whether the due date has a valid, non-default value.
        /// </summary>
        public bool HasValue => Value != default;

        /// <summary>
        /// Initializes a new instance of the <see cref="DueDate"/> class.
        /// Throws an exception if the specified date is in the past.
        /// </summary>
        /// <param name="value">The due date in UTC.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when the due date is earlier than the current UTC date and time.
        /// </exception>
        public DueDate(DateTime value)
        {
            if (value < DateTime.UtcNow)
                throw new ArgumentException("Due date must not be earlier than the current UTC date and time.", nameof(value));

            Value = value;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current <see cref="DueDate"/>.
        /// </summary>
        /// <param name="obj">The object to compare with the current <see cref="DueDate"/>.</param>
        /// <returns>true if the specified object is equal to the current <see cref="DueDate"/>; otherwise, false.</returns>
        public override bool Equals(object? obj) => Equals(obj as DueDate);

        /// <summary>
        /// Determines whether the specified <see cref="DueDate"/> is equal to the current instance.
        /// </summary>
        /// <param name="other">The <see cref="DueDate"/> to compare.</param>
        /// <returns>true if the due dates are equal; otherwise, false.</returns>
        public bool Equals(DueDate? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Value.Equals(other.Value);
        }

        /// <summary>
        /// Gets the hash code for the current <see cref="DueDate"/>.
        /// </summary>
        /// <returns>The hash code for the value.</returns>
        public override int GetHashCode() => Value.GetHashCode();

        /// <summary>
        /// Determines whether two <see cref="DueDate"/> instances are equal.
        /// </summary>
        public static bool operator ==(DueDate? left, DueDate? right) => Equals(left, right);

        /// <summary>
        /// Determines whether two <see cref="DueDate"/> instances are not equal.
        /// </summary>
        public static bool operator !=(DueDate? left, DueDate? right) => !Equals(left, right);

        /// <summary>
        /// Returns the string representation of the due date in ISO 8601 format.
        /// </summary>
        public override string ToString() => Value.ToString("O");
    }
}
