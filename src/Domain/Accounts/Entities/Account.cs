using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Accounts.Services.Interfaces;
using Domain.Accounts.ValueObjects;

namespace Domain.Accounts.Entities;

/// <summary>
/// Aggregate root representing an account with credentials, activation lifecycle, and role-based authorization.
/// Raw token secrets are never stored; only activation token hashes are handled.
/// </summary>
public sealed class Account
{
    public AccountId Id { get; private set; }
    public AccountEmail Email { get; private set; }
    public AccountUsername Username { get; private set; }
    public AccountName Name { get; private set; }
    public string PasswordHash { get; private set; }

    /// <summary>Creation timestamp (UTC).</summary>
    public DateTimeOffset CreatedAt { get; private set; }

    /// <summary>Last successful sign-in (UTC), if any.</summary>
    public DateTimeOffset? LastLoginAt { get; private set; }

    /// <summary>Activation timestamp (UTC), if the account is active.</summary>
    public DateTimeOffset? ActivatedAt { get; private set; }

    private readonly List<Role> _roles = new();
    public IReadOnlyCollection<Role> Roles => _roles.AsReadOnly();

    private readonly List<ActivationToken> _activationTokens = new();
    public IReadOnlyCollection<ActivationToken> ActivationTokens => _activationTokens.AsReadOnly();

    private Account() { }

    public Account(
        AccountId id,
        AccountEmail email,
        AccountUsername username,
        AccountName name,
        string passwordHash,
        DateTimeOffset? activatedAt = null)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        Username = username ?? throw new ArgumentNullException(nameof(username));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        PasswordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
        CreatedAt = DateTimeOffset.UtcNow;
        ActivatedAt = activatedAt;
    }

    /// <summary>Creates a new pending account.</summary>
    public static Account Create(
        AccountEmail email,
        AccountUsername username,
        AccountName name,
        string passwordHash)
        => new(AccountId.New(), email, username, name, passwordHash);

    /// <summary>Marks the account as active at the given instant. Idempotent.</summary>
    public void Activate(DateTimeOffset whenUtc)
    {
        if (ActivatedAt is null) ActivatedAt = whenUtc;
    }

    /// <summary>Marks the account as inactive.</summary>
    public void Deactivate() => ActivatedAt = null;

    /// <summary>
    /// Issues a new activation token for this account. By default, any non-finalized token is revoked first.
    /// </summary>
    public ActivationToken IssueActivationToken(
        string hash,
        DateTimeOffset now,
        TimeSpan timeToLive,
        bool revokeExistingFirst = true)
    {
        if (revokeExistingFirst)
        {
            RevokeAllNonFinalizedTokens(now, RevocationReason.Reissued);
        }
        else if (HasNonFinalizedToken())
        {
            throw new InvalidOperationException("There is an unfinalized activation token for this account.");
        }

        var token = ActivationToken.Issue(Id, hash, now, timeToLive);
        _activationTokens.Add(token);
        return token;
    }

    /// <summary>
    /// Activates the account using a token that belongs to this aggregate.
    /// The caller must validate the raw secret against the token hash before invoking this method.
    /// </summary>
    public void ActivateWithToken(ActivationToken token, DateTimeOffset whenUtc)
    {
        if (token == null) throw new ArgumentNullException(nameof(token));
        if (token.UserId != Id) throw new InvalidOperationException("Token does not belong to this account.");
        if (!token.IsActive(whenUtc)) throw new InvalidOperationException("Token is revoked or expired.");

        token.Revoke(whenUtc, RevocationReason.UsedForActivation);
        Activate(whenUtc);
    }

    /// <summary>Revokes all non-finalized tokens.</summary>
    public void RevokeAllNonFinalizedTokens(DateTimeOffset whenUtc, RevocationReason reason)
    {
        foreach (var t in _activationTokens)
            if (!t.IsRevoked) t.Revoke(whenUtc, reason);
    }

    /// <summary>Returns true if there is any non-finalized token.</summary>
    public bool HasNonFinalizedToken() => _activationTokens.Any(t => !t.IsRevoked);

    /// <summary>Returns an active token by identifier or null if none matches.</summary>
    public ActivationToken? FindActiveTokenById(Guid tokenId, DateTimeOffset now)
        => _activationTokens.FirstOrDefault(t => t.Id == tokenId && t.IsActive(now));

    /// <summary>Updates the display name.</summary>
    public void UpdateName(AccountName newName)
        => Name = newName ?? throw new ArgumentNullException(nameof(newName));

    /// <summary>Updates the e-mail address.</summary>
    public void UpdateEmail(AccountEmail newEmail)
        => Email = newEmail ?? throw new ArgumentNullException(nameof(newEmail));

    /// <summary>Updates the username.</summary>
    public void UpdateUsername(AccountUsername newUsername)
        => Username = newUsername ?? throw new ArgumentNullException(nameof(newUsername));

    /// <summary>Sets the last successful sign-in to now (UTC).</summary>
    public void UpdateLastLogin() => LastLoginAt = DateTimeOffset.UtcNow;

    /// <summary>Replaces the password hash.</summary>
    public void UpdatePassword(string newPasswordHash)
        => PasswordHash = newPasswordHash ?? throw new ArgumentNullException(nameof(newPasswordHash));

    /// <summary>Verifies a plain password using the provided hasher.</summary>
    public bool ValidatePassword(string plainPassword, IPasswordHasher hasher)
    {
        if (hasher == null) throw new ArgumentNullException(nameof(hasher));
        return hasher.VerifyPassword(PasswordHash, plainPassword);
    }

    /// <summary>Assigns a role to the account. Idempotent.</summary>
    public void AssignRole(Role role)
    {
        if (role == null) throw new ArgumentNullException(nameof(role));
        if (_roles.Any(r => r.Id == role.Id)) return;
        _roles.Add(role);
    }

    /// <summary>Removes a role from the account.</summary>
    public void RemoveRole(Role role)
    {
        if (role == null) throw new ArgumentNullException(nameof(role));
        _roles.RemoveAll(r => r.Id == role.Id);
    }

    /// <summary>Returns true if the account has the given role name.</summary>
    public bool HasRole(string roleName) => _roles.Any(r => r.NameEquals(roleName));
}
