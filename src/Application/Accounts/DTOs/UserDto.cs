using System;
using Domain.Accounts;

namespace Application.Accounts.DTOs;

/// <summary>
/// Data Transfer Object that represents the public data of a User account.
/// </summary>
public class UserDto : AccountDto
{
    public bool IsActive { get; }

    public UserDto(
        string id,
        string email,
        string username,
        string name,
        DateTime createdAt,
        DateTime? lastLoginAt,
        bool isActive)
        : base(id, email, username, name, createdAt, lastLoginAt)
    {
        IsActive = isActive;
    }

    /// <summary>
    /// Creates a <see cref="UserDto"/> instance from a User aggregate.
    /// </summary>
    public static UserDto FromAggregate(User user)
    {
        if (user is null) throw new ArgumentNullException(nameof(user));
        return new UserDto(
            user.Id.ToString(),
            user.Email.ToString(),
            user.Username.ToString(),
            user.Name.ToString(),
            user.CreatedAt,
            user.LastLoginAt,
            user.IsActive
        );
    }
}
