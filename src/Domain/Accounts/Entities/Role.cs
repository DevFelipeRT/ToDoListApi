using System;

namespace Domain.Accounts.Entities;

/// <summary>
/// Role entity used for RBAC. Examples: "ADMIN", "USER", "MODERATOR".
/// </summary>
public sealed class Role
{
    public Guid Id { get; private set; }
    /// <summary>Normalized role name (suggestion: store uppercase for uniqueness).</summary>
    public string Name { get; private set; } = null!;

    private Role() { } // EF

    private Role(Guid id, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Role name is required.", nameof(name));

        Id = id == Guid.Empty ? Guid.NewGuid() : id;
        Name = name.Trim().ToUpperInvariant();
    }

    public static Role Create(string name) => new Role(Guid.NewGuid(), name);

    public bool NameEquals(string other)
        => !string.IsNullOrWhiteSpace(other) &&
            string.Equals(Name, other.Trim(), StringComparison.OrdinalIgnoreCase);

    public override string ToString() => Name;
}
