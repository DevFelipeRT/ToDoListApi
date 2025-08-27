using System;
using Application.Lists.Commands.Items;
using Domain.Accounts.Entities;

namespace Application.Accounts.DTOs;

/// <summary>
/// Data Transfer Object that represents the public data of any Account.
/// Serves as a base for specific account DTOs (e.g., UserDto).
/// </summary>
public class AccountDto
{
    public Guid Id { get; }
    public string CredentialId { get; }
    public string Email { get; }
    public string Username { get; }
    public string Name { get; }
    public DateTimeOffset CreatedAt { get; }
    public DateTimeOffset? LastLoginAt { get; }
    public bool IsActive => LastLoginAt.HasValue;

    public AccountDto(
        Guid id,
        string credentialId,
        string email,
        string username,
        string name,
        DateTimeOffset createdAt,
        DateTimeOffset? lastLoginAt)
    {
        Id = id;
        CredentialId = credentialId;
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
            account.Id.Value,
            account.CredentialId!.ToString(),
            account.Email.ToString(),
            account.Username.ToString(),
            account.Name.ToString(),
            account.CreatedAt,
            account.LastLoginAt
        );
    }
}
