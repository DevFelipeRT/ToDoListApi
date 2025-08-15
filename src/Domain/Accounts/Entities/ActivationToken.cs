using System;
using System.Diagnostics;
using Domain.Accounts.ValueObjects;

namespace Domain.Accounts.Entities;

/// <summary>
/// Domain entity representing a one-time activation token bound to an account.
/// The raw token is never persisted; only its cryptographic hash is stored.
/// </summary>
[DebuggerDisplay("ActivationToken {Id} (Active={IsActive(System.DateTimeOffset.UtcNow)})")]
public sealed class ActivationToken
{
    /// <summary>Surrogate identity for persistence and entity equality.</summary>
    public Guid Id { get; private set; }

    /// <summary>Owning account identifier (aggregate root).</summary>
    public AccountId UserId { get; private set; } = null!; // set by factory/EF

    /// <summary>SHA-256 of the raw token encoded as uppercase hexadecimal (64 chars). Never store the raw token.</summary>
    public string Hash { get; private set; } = null!;

    /// <summary>Issuance instant (token creation).</summary>
    public DateTimeOffset CreatedAt { get; private set; }

    /// <summary>Expiration instant (token is no longer valid after this moment).</summary>
    public DateTimeOffset ExpiresAt { get; private set; }

    /// <summary>Instant when the token was revoked (used, reissued, admin action, cleanup). Null if not revoked.</summary>
    public DateTimeOffset? RevokedAt { get; private set; }

    /// <summary>Reason that made the token unusable.</summary>
    public RevocationReason? RevokedReason { get; private set; }

    /// <summary>Required by EF Core and serializers.</summary>
    private ActivationToken() { }

    /// <summary>
    /// Issues a new activation token bound to <paramref name="userId"/>.
    /// </summary>
    /// <param name="userId">Owning account identifier.</param>
    /// <param name="hash">Uppercase hex SHA-256 of the raw token.</param>
    /// <param name="now">Clock reference for issuance.</param>
    /// <param name="timeToLive">Token lifetime; must be greater than zero.</param>
    public static ActivationToken Issue(
        AccountId userId,
        string hash,
        DateTimeOffset now,
        TimeSpan timeToLive)
    {
        if (timeToLive <= TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(timeToLive), "Token lifetime must be greater than zero.");

        var token = new ActivationToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Hash = hash,
            CreatedAt = now,
            ExpiresAt = now + timeToLive
        };

        token.ValidateInvariants();
        return token;
    }

    /// <summary>True when the token has been revoked.</summary>
    public bool IsRevoked => RevokedAt.HasValue;

    /// <summary>True when the token is expired at <paramref name="now"/>.</summary>
    public bool IsExpired(DateTimeOffset now) => now > ExpiresAt;

    /// <summary>True when the token is neither revoked nor expired at <paramref name="now"/>.</summary>
    public bool IsActive(DateTimeOffset now) => !IsRevoked && !IsExpired(now);

    /// <summary>
    /// Revokes the token with a reason. Idempotent if already revoked.
    /// </summary>
    /// <param name="when">Revocation timestamp.</param>
    /// <param name="reason">Reason for revocation.</param>
    public void Revoke(DateTimeOffset when, RevocationReason reason)
    {
        if (IsRevoked) return;
        RevokedAt = when;
        RevokedReason = reason;
    }

    /// <summary>
    /// Attempts to revoke only if active at <paramref name="when"/>. Returns true on state change.
    /// </summary>
    public bool TryRevoke(DateTimeOffset when, RevocationReason reason)
    {
        if (!IsActive(when)) return false;
        Revoke(when, reason);
        return true;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is ActivationToken other && other.Id == Id;

    /// <inheritdoc />
    public override int GetHashCode() => Id.GetHashCode();

    /// <summary>String representation that never exposes the token hash.</summary>
    public override string ToString() =>
        $"ActivationToken(Id={Id}, UserId={UserId}, CreatedAt={CreatedAt:u}, ExpiresAt={ExpiresAt:u}, Revoked={(RevokedAt.HasValue ? "Yes" : "No")})";

    private void ValidateInvariants()
    {
        if (UserId.Equals(default(AccountId)))
            throw new ArgumentException("UserId must be provided.", nameof(UserId));

        if (string.IsNullOrWhiteSpace(Hash) || Hash.Length != 64 || !IsUpperHex(Hash))
            throw new ArgumentException("Hash must be a 64-character uppercase hexadecimal string.", nameof(Hash));

        if (ExpiresAt <= CreatedAt)
            throw new ArgumentException("ExpiresAt must be strictly greater than CreatedAt.");
    }

    private static bool IsUpperHex(ReadOnlySpan<char> s)
    {
        foreach (var c in s)
        {
            var isDigit = (uint)(c - '0') <= 9;
            var isUpperHex = (uint)(c - 'A') <= 5;
            if (!isDigit && !isUpperHex) return false;
        }
        return true;
    }
}

/// <summary>Reason that made the activation token unusable.</summary>
public enum RevocationReason : byte
{
    /// <summary>Token was presented and accepted to activate the account.</summary>
    UsedForActivation = 1,

    /// <summary>A new token was issued; the previous one became unusable.</summary>
    Reissued = 2,

    /// <summary>Administrative action explicitly revoked the token.</summary>
    AdminRevoked = 3,

    /// <summary>System cleanup revoked an expired token.</summary>
    ExpiredCleanup = 4
}

