using System;
using Domain.Accounts.ValueObjects;

namespace Domain.Accounts.Entities;

/// <summary>
/// Activation token owned by an account. Stores only the hashed secret and enforces validity rules.
/// </summary>
public sealed class ActivationToken
{
    /// <summary>Token identifier.</summary>
    public Guid Id { get; private set; }

    /// <summary>Owner account identifier.</summary>
    public AccountId AccountId { get; private set; } = null!;

    /// <summary>Uppercase hexadecimal SHA-256 hash of the raw token (64 chars).</summary>
    public string Hash { get; private set; } = null!;

    /// <summary>UTC issuance instant.</summary>
    public DateTimeOffset CreatedAt { get; private set; }

    /// <summary>UTC expiration instant (token becomes invalid at this instant).</summary>
    public DateTimeOffset ExpiresAt { get; private set; }

    /// <summary>UTC instant when the token was explicitly revoked, if any.</summary>
    public DateTimeOffset? RevokedAt { get; private set; }

    /// <summary>Reason for revocation, if any.</summary>
    public RevocationReason? RevokedReason { get; private set; }

    private ActivationToken() { }

    /// <summary>
    /// Creates a new activation token for the specified account.
    /// </summary>
    /// <param name="accountId">Owner account identifier.</param>
    /// <param name="hash">Uppercase hexadecimal SHA-256 hash of the raw token.</param>
    /// <param name="now">UTC issuance instant.</param>
    /// <param name="timeToLive">Lifetime; must be greater than zero.</param>
    public static ActivationToken Create(
        AccountId accountId,
        string hash,
        DateTimeOffset now,
        TimeSpan timeToLive)
    {
        if (accountId is null)
            throw new ArgumentNullException(nameof(accountId));

        if (timeToLive <= TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(timeToLive), "Lifetime must be greater than zero.");

        // Prevent overflow on addition.
        if (now > DateTimeOffset.MaxValue - timeToLive)
            throw new ArgumentOutOfRangeException(nameof(timeToLive), "Lifetime pushes expiration beyond supported range.");

        var token = new ActivationToken
        {
            Id = Guid.NewGuid(),
            AccountId = accountId,
            Hash = hash,
            CreatedAt = now,
            ExpiresAt = now + timeToLive
        };

        token.Validate();
        return token;
    }

    /// <summary>Whether the token was explicitly revoked.</summary>
    public bool IsRevoked => RevokedAt.HasValue;

    /// <summary>Whether the token is expired at <paramref name="now"/> (inclusive).</summary>
    public bool IsExpired(DateTimeOffset now) => now >= ExpiresAt;

    /// <summary>Whether the token is usable at <paramref name="now"/>.</summary>
    public bool IsActive(DateTimeOffset now) => !IsRevoked && !IsExpired(now);

    /// <summary>Whether the token can no longer be used (revoked or expired) at <paramref name="now"/>.</summary>
    public bool IsFinalized(DateTimeOffset now) => IsRevoked || IsExpired(now);

    /// <summary>
    /// Explicitly revokes the token at the given instant with a reason. Idempotent.
    /// </summary>
    public void Revoke(DateTimeOffset when, RevocationReason reason)
    {
        if (IsRevoked) return;
        RevokedAt = when;
        RevokedReason = reason;
    }

    /// <summary>
    /// Attempts to revoke the token if it is still active at <paramref name="when"/>.
    /// </summary>
    public bool TryRevoke(DateTimeOffset when, RevocationReason reason)
    {
        if (!IsActive(when)) return false;
        Revoke(when, reason);
        return true;
    }

    public override bool Equals(object? obj) => obj is ActivationToken other && other.Id == Id;
    public override int GetHashCode() => Id.GetHashCode();

    public override string ToString() =>
        $"ActivationToken(Id={Id}, AccountId={AccountId}, CreatedAt={CreatedAt:u}, ExpiresAt={ExpiresAt:u}, Revoked={(RevokedAt.HasValue ? "Yes" : "No")})";

    private void Validate()
    {
        if (AccountId.Value == Guid.Empty)
            throw new ArgumentException("AccountId must be provided.", nameof(AccountId));

        if (string.IsNullOrWhiteSpace(Hash) || Hash.Length != 64 || !IsUpperHex(Hash))
            throw new ArgumentException("Hash must be a 64-character uppercase hexadecimal string.", nameof(Hash));

        if (ExpiresAt <= CreatedAt)
            throw new ArgumentException("ExpiresAt must be greater than CreatedAt.");
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
