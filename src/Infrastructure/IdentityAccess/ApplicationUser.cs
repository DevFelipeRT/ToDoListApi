using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.IdentityAccess;

public sealed class ApplicationUser : IdentityUser<Guid>
{
    /// <summary>
    /// Display name associated with the user.
    /// </summary>
    [PersonalData]
    [StringLength(128)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// UTC timestamp when the user account was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// UTC timestamp when the user last logged in.
    /// </summary>
    public DateTimeOffset? LastLoginAt { get; private set; }

    /// <summary>
    /// Parameterless constructor required by ASP.NET Identity and EF Core.
    /// </summary>
    public ApplicationUser() { }

    /// <summary>
    /// Rich constructor used by infrastructure when explicitly creating a user.
    /// </summary>
    public ApplicationUser(Guid id, string userName, string email, string displayName)
    {
        Id = id;
        UserName = userName;
        Email = email;
        Name = displayName;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Updates the last login timestamp to the current UTC time.
    /// </summary>
    public void UpdateLastLogin() => LastLoginAt = DateTimeOffset.UtcNow;
}
