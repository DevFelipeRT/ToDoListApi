using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.IdentityAccess;

public sealed class ApplicationRole : IdentityRole<Guid>
{
    /// <summary>
    /// Human-friendly role name for display purposes.
    /// </summary>
    [StringLength(128)]
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Optional role description to clarify responsibilities/usage.
    /// </summary>
    [StringLength(256)]
    public string? Description { get; set; }

    /// <summary>
    /// UTC timestamp when the role was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Parameterless constructor required by ASP.NET Identity and EF Core.
    /// </summary>
    public ApplicationRole() { }

    /// <summary>
    /// Rich constructor used by infrastructure when explicitly creating a role.
    /// </summary>
    public ApplicationRole(Guid id, string name, string? displayName = null, string? description = null)
    {
        Id = id;
        Name = name;
        NormalizedName = name.ToUpperInvariant();
        DisplayName = displayName ?? name;
        Description = description;
        CreatedAt = DateTimeOffset.UtcNow;
        ConcurrencyStamp = Guid.NewGuid().ToString();
    }
}
