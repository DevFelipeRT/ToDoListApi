using System;
using Domain.Accounts;

namespace Application.Accounts.DTOs;

/// <summary>
/// Data Transfer Object that represents the public data of any Account.
/// Serves as a base for specific account DTOs (e.g., UserDto).
/// </summary>
public class AccountDto
{
    public string Id { get; }
    public string Email { get; }
    public string Username { get; }
    public string Name { get; }
    public DateTime CreatedAt { get; }
    public DateTime? LastLoginAt { get; }

    public AccountDto(
        string id,
        string email,
        string username,
        string name,
        DateTime createdAt,
        DateTime? lastLoginAt)
    {
        Id = id;
        Email = email;
        Username = username;
        Name = name;
        CreatedAt = createdAt;
        LastLoginAt = lastLoginAt;
    }

    /// <summary>
    /// Creates an <see cref="AccountDto"/> instance from an Account aggregate.
    /// </summary>
    public static AccountDto FromAggregate(Account account)
    {
        if (account is null) throw new ArgumentNullException(nameof(account));
        return new AccountDto(
            account.Id.ToString(),
            account.Email.ToString(),
            account.Username.ToString(),
            account.Name.ToString(),
            account.CreatedAt,
            account.LastLoginAt
        );
    }
}
