// Domain/Accounts/Entities/Account.cs
using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Abstractions.Aggregates;
using Domain.Accounts.Events;
using Domain.Accounts.Services.Interfaces;
using Domain.Accounts.ValueObjects;

namespace Domain.Accounts.Entities;

public sealed class Account : AggregateRoot
{
    public AccountId Id { get; private set; } = null!;
    public AccountEmail Email { get; private set; } = null!;
    public AccountUsername Username { get; private set; } = null!;
    public AccountName Name { get; private set; } = null!;
    public string PasswordHash { get; private set; } = string.Empty;

    /// <summary>UTC creation instant.</summary>
    public DateTimeOffset CreatedAt { get; private set; }

    /// <summary>UTC instant of the last successful sign-in, if any.</summary>
    public DateTimeOffset? LastLoginAt { get; private set; }

    /// <summary>UTC instant when the account became active, if active.</summary>
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

    /// <summary>Creates a new pending account and raises <see cref="AccountRegistered"/>.</summary>
    public static Account Create(
        AccountEmail email,
        AccountUsername username,
        AccountName name,
        string passwordHash)
    {
        var account = new Account(AccountId.New(), email, username, name, passwordHash);
        account.Raise(new AccountRegistered(account.Id, DateTimeOffset.UtcNow));
        return account;
    }

    /// <summary>Marks the account as active at the given instant (idempotent).</summary>
    public void Activate(DateTimeOffset whenUtc)
    {
        if (ActivatedAt is null) ActivatedAt = whenUtc;
    }

    /// <summary>Marks the account as inactive.</summary>
    public void Deactivate() => ActivatedAt = null;

    /// <summary>
    /// Issues a new activation token. Optionally revokes all currently active tokens first.
    /// Active = not revoked and not expired at <paramref name="now"/>.
    /// Expired tokens are not revoked by default.
    /// </summary>
    public ActivationToken CreateActivationToken(
        string hash,
        DateTimeOffset now,
        TimeSpan timeToLive,
        bool revokeExistingFirst = true)
    {
        if (revokeExistingFirst)
        {
            RevokeAllActiveTokens(now, RevocationReason.Reissued);
        }
        else if (HasActiveToken(now))
        {
            throw new InvalidOperationException("There is an active activation token for this account.");
        }

        var token = ActivationToken.Create(Id, hash, now, timeToLive);
        _activationTokens.Add(token);
        return token;
    }

    /// <summary>
    /// Activates the account using an owned token. Validates ownership and activity window,
    /// then revokes the token with <see cref="RevocationReason.UsedForActivation"/>.
    /// </summary>
    public void ActivateWithToken(ActivationToken token, DateTimeOffset whenUtc)
    {
        if (token is null) throw new ArgumentNullException(nameof(token));
        if (token.AccountId != Id) throw new InvalidOperationException("Token does not belong to this account.");
        if (!token.IsActive(whenUtc)) throw new InvalidOperationException("Token is revoked or expired.");

        token.Revoke(whenUtc, RevocationReason.UsedForActivation);
        Activate(whenUtc);
    }

    /// <summary>Revokes all tokens that are active at <paramref name="whenUtc"/>.</summary>
    public void RevokeAllActiveTokens(DateTimeOffset whenUtc, RevocationReason reason)
    {
        foreach (var t in _activationTokens)
            if (t.IsActive(whenUtc)) t.Revoke(whenUtc, reason);
    }

    /// <summary>
    /// Revokes all non-revoked tokens, including those already expired.
    /// Use for explicit audit/policy; not required for normal issuance.
    /// </summary>
    public void RevokeAllNonRevokedTokens(DateTimeOffset whenUtc, RevocationReason reason)
    {
        foreach (var t in _activationTokens)
            if (!t.IsRevoked) t.Revoke(whenUtc, reason);
    }

    /// <summary>Returns true if there is any token active at <paramref name="now"/>.</summary>
    public bool HasActiveToken(DateTimeOffset now)
        => _activationTokens.Any(t => t.IsActive(now));

    /// <summary>Returns true if there is any token finalized at <paramref name="now"/> (revoked or expired).</summary>
    public bool HasFinalizedToken(DateTimeOffset now)
        => _activationTokens.Any(t => t.IsFinalized(now));

    /// <summary>Returns true if there is any token that is not revoked (expired tokens still count as non-revoked).</summary>
    public bool HasNonRevokedToken()
        => _activationTokens.Any(t => !t.IsRevoked);

    /// <summary>Returns an active token by identifier or null if none matches at <paramref name="now"/>.</summary>
    public ActivationToken? FindActiveTokenById(Guid tokenId, DateTimeOffset now)
        => _activationTokens.FirstOrDefault(t => t.Id == tokenId && t.IsActive(now));

    /// <summary>Returns a finalized token by identifier or null if none matches at <paramref name="now"/>.</summary>
    public ActivationToken? FindFinalizedTokenById(Guid tokenId, DateTimeOffset now)
        => _activationTokens.FirstOrDefault(t => t.Id == tokenId && t.IsFinalized(now));

    public void UpdateName(AccountName newName)
        => Name = newName ?? throw new ArgumentNullException(nameof(newName));

    public void UpdateEmail(AccountEmail newEmail)
        => Email = newEmail ?? throw new ArgumentNullException(nameof(newEmail));

    public void UpdateUsername(AccountUsername newUsername)
        => Username = newUsername ?? throw new ArgumentNullException(nameof(newUsername));

    public void UpdateLastLogin() => LastLoginAt = DateTimeOffset.UtcNow;

    public void UpdatePassword(string newPasswordHash)
        => PasswordHash = newPasswordHash ?? throw new ArgumentNullException(nameof(newPasswordHash));

    public bool ValidatePassword(string plainPassword, IPasswordHasher hasher)
    {
        if (hasher is null) throw new ArgumentNullException(nameof(hasher));
        return hasher.VerifyPassword(PasswordHash, plainPassword);
    }

    public void AssignRole(Role role)
    {
        if (role is null) throw new ArgumentNullException(nameof(role));
        if (_roles.Any(r => r.Id == role.Id)) return;
        _roles.Add(role);
    }

    public void RemoveRole(Role role)
    {
        if (role is null) throw new ArgumentNullException(nameof(role));
        _roles.RemoveAll(r => r.Id == role.Id);
    }

    public bool HasRole(string roleName) => _roles.Any(r => r.NameEquals(roleName));
}
