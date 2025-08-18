using System;
using Domain.Accounts.ValueObjects;

namespace Domain.Accounts.Entities;

public sealed class ActivationToken
{
    public Guid Id { get; private set; }
    public AccountId AccountId { get; private set; } = null!;
    public string Hash { get; private set; } = null!;
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset ExpiresAt { get; private set; }
    public DateTimeOffset? RevokedAt { get; private set; }
    public RevocationReason? RevokedReason { get; private set; }

    private ActivationToken() { }

    public static ActivationToken Create(
        AccountId accountId,
        string hash,
        DateTimeOffset now,
        TimeSpan timeToLive)
    {
        if (timeToLive <= TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(timeToLive), "Lifetime must be greater than zero.");

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

    public bool IsRevoked => RevokedAt.HasValue;
    public bool IsExpired(DateTimeOffset now) => now > ExpiresAt;
    public bool IsActive(DateTimeOffset now) => !IsRevoked && !IsExpired(now);

    public void Revoke(DateTimeOffset when, RevocationReason reason)
    {
        if (IsRevoked) return;
        RevokedAt = when;
        RevokedReason = reason;
    }

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
