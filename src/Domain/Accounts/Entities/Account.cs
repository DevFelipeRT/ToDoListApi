using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Abstractions.Aggregates;
using Domain.Accounts.Events;
using Domain.Accounts.ValueObjects;

namespace Domain.Accounts.Entities;

/// <summary>
/// Account aggregate root that represents user-related business concepts.
/// Authentication and credential lifecycle are owned by an external IAM.
/// The aggregate may hold an optional reference to the external credential identifier.
/// </summary>
public sealed class Account : AggregateRoot
{
    /// <summary>
    /// Aggregate identifier.
    /// </summary>
    public AccountId Id { get; private set; } = null!;

    /// <summary>
    /// Optional reference to the external credential identifier managed by IAM.
    /// </summary>
    public CredentialId? CredentialId { get; private set; }

    /// <summary>
    /// Email address associated with the account.
    /// </summary>
    public AccountEmail Email { get; private set; } = null!;

    /// <summary>
    /// Public username associated with the account.
    /// </summary>
    public AccountUsername Username { get; private set; } = null!;

    /// <summary>
    /// Display name associated with the account.
    /// </summary>
    public AccountName Name { get; private set; } = null!;

    /// <summary>
    /// UTC timestamp when the account was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; private set; }

    /// <summary>
    /// UTC timestamp of the last successful sign-in, if any.
    /// </summary>
    public DateTimeOffset? LastLoginAt { get; private set; }

    /// <summary>
    /// UTC timestamp when the account became active, if active.
    /// </summary>
    public DateTimeOffset? ActivatedAt { get; private set; }

    /// <summary>
    /// Indicates whether the account is currently active.
    /// </summary>
    public bool IsActive => ActivatedAt.HasValue;

    private Account() { }

    private Account(
        AccountId id,
        AccountEmail email,
        AccountUsername username,
        AccountName name,
        CredentialId? credentialId = null,
        DateTimeOffset? activatedAt = null,
        DateTimeOffset? lastLoginAt = null,
        DateTimeOffset? createdAt = null)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        Username = username ?? throw new ArgumentNullException(nameof(username));
        Name = name ?? throw new ArgumentNullException(nameof(name));

        CredentialId = credentialId;
        ActivatedAt = activatedAt;
        LastLoginAt = lastLoginAt;
        CreatedAt = createdAt ?? DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Creates a pending account and raises <see cref="AccountRegistered"/>.
    /// </summary>
    public static Account Create(AccountEmail email, AccountUsername username, AccountName name)
    {
        var account = new Account(AccountId.New(), email, username, name);
        account.Raise(new AccountRegistered(account.Id, account.CreatedAt));
        return account;
    }

    /// <summary>
    /// Creates an account already linked to external credentials.
    /// Raises <see cref="AccountRegistered"/> and <see cref="AccountLinkedToCredentials"/> for consistency.
    /// </summary>
    public static Account CreateLinked(CredentialId credentialId, AccountEmail email, AccountUsername username, AccountName name)
    {
        if (credentialId is null) throw new ArgumentNullException(nameof(credentialId));

        var account = new Account(AccountId.New(), email, username, name, credentialId);
        // Signal registration and the external link so handlers can react (audits, projections, etc.)
        account.Raise(new AccountRegistered(account.Id, account.CreatedAt));
        account.Raise(new AccountLinkedToCredentials(account.Id, credentialId, DateTimeOffset.UtcNow));
        return account;
    }

    /// <summary>
    /// Restores an account from persistence.
    /// </summary>
    public static Account Restore(
        AccountId id,
        CredentialId? credentialId,
        AccountEmail email,
        AccountUsername username,
        AccountName name,
        DateTimeOffset createdAt,
        DateTimeOffset? lastLoginAt,
        DateTimeOffset? activatedAt)
    {
        return new Account(id, email, username, name, credentialId, activatedAt, lastLoginAt, createdAt);
    }

    /// <summary>
    /// Activates the account and raises <see cref="AccountActivated"/>.
    /// </summary>
    public void Activate(DateTimeOffset whenUtc)
    {
        if (IsActive) return;
        ActivatedAt = whenUtc;
        Raise(new AccountActivated(Id, whenUtc));
    }

    /// <summary>
    /// Deactivates the account and raises <see cref="AccountDeactivated"/>.
    /// </summary>
    public void Deactivate()
    {
        if (!IsActive) return;
        ActivatedAt = null;
        Raise(new AccountDeactivated(Id, DateTimeOffset.UtcNow));
    }

    /// <summary>
    /// Requests an activation flow (domain-level intent) and raises <see cref="AccountActivationRequested"/>.
    /// Handlers should perform notification or token issuance in the IAM.
    /// </summary>
    public void RequestActivation(CredentialId? initiatorCredentialId = null)
    {
        Raise(new AccountActivationRequested(Id, initiatorCredentialId, DateTimeOffset.UtcNow));
    }

    /// <summary>
    /// Updates the display name and raises <see cref="AccountNameChanged"/> if changed.
    /// </summary>
    public void UpdateName(AccountName newName)
    {
        if (newName is null) throw new ArgumentNullException(nameof(newName));
        var old = Name;
        if (old.Equals(newName)) return;

        Name = newName;
        Raise(new AccountNameChanged(Id, old, newName, DateTimeOffset.UtcNow));
    }

    /// <summary>
    /// Updates the email and raises <see cref="AccountEmailChanged"/> if changed.
    /// </summary>
    public void UpdateEmail(AccountEmail newEmail)
    {
        if (newEmail is null) throw new ArgumentNullException(nameof(newEmail));
        var old = Email;
        if (old.Equals(newEmail)) return;

        Email = newEmail;
        Raise(new AccountEmailChanged(Id, old, newEmail, DateTimeOffset.UtcNow));
    }

    /// <summary>
    /// Updates the username and raises <see cref="AccountUsernameChanged"/> if changed.
    /// </summary>
    public void UpdateUsername(AccountUsername newUsername)
    {
        if (newUsername is null) throw new ArgumentNullException(nameof(newUsername));
        var old = Username;
        if (old.Equals(newUsername)) return;

        Username = newUsername;
        Raise(new AccountUsernameChanged(Id, old, newUsername, DateTimeOffset.UtcNow));
    }

    /// <summary>
    /// Updates the last successful login timestamp.
    /// </summary>
    public void UpdateLastLogin(DateTimeOffset? whenUtc = null)
    {
        LastLoginAt = whenUtc ?? DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Links this account to external credentials. Prevents relinking to a different credential.
    /// Raises <see cref="AccountLinkedToCredentials"/> when a new link is established.
    /// </summary>
    public void LinkToCredentials(CredentialId credentialId)
    {
        if (credentialId is null) throw new ArgumentNullException(nameof(credentialId));

        if (CredentialId is null)
        {
            CredentialId = credentialId;
            Raise(new AccountLinkedToCredentials(Id, credentialId, DateTimeOffset.UtcNow));
            return;
        }

        if (CredentialId.Equals(credentialId)) return;

        throw new InvalidOperationException("Account is already linked to a different credential.");
    }
}

