using System;
using System.Security.Cryptography;
using System.Text;
using Domain.Accounts.Entities;
using Domain.Accounts.ValueObjects;

namespace Application.Accounts.Services;

/// <summary>
/// Application service for issuing and validating activation tokens, aligned with the domain entity semantics.
/// 
/// Responsibilities:
/// - Generate high-entropy raw tokens (Base64Url, no padding).
/// - Hash raw tokens using SHA-256 (uppercase hex) without ever persisting the raw value.
/// - Create <see cref="ActivationToken"/> instances via the domain factory, returning the raw token to the caller.
/// - Validate account-presented raw tokens against persisted hashes using constant-time comparison.
/// - Provide a convenience operation to verify-and-consume an active token.
/// 
/// This service contains no persistence or I/O concerns; orchestration belongs to the application layer,
/// and storage/dispatch (e.g., e-mail) should be handled by dedicated components.
/// </summary>
public static class ActivationTokenGenerator
{
    /// <summary>
    /// Creates a new activation token for the given account and returns both:
    /// - The domain entity (hash persisted by the domain).
    /// - The raw token (to be sent to the account and NEVER persisted).
    /// </summary>
    /// <param name="accountId">Owning account identifier.</param>
    /// <param name="now">Clock reference for issuance.</param>
    /// <param name="timeToLive">Token lifetime; must be greater than zero.</param>
    /// <returns>(Token, RawToken)</returns>
    public static (ActivationToken Token, string RawToken) Create(AccountId accountId, DateTimeOffset now, TimeSpan timeToLive)
    {
        var raw = GenerateRawToken();
        var hash = ComputeHash(raw);
        var token = ActivationToken.Create(accountId, hash, now, timeToLive);
        return (token, raw);
    }

    /// <summary>
    /// Verifies whether the account-provided <paramref name="rawToken"/> matches the <paramref name="token"/>'s hash
    /// using a constant-time comparison. No state mutation occurs.
    /// </summary>
    /// <param name="token">Persisted activation token entity.</param>
    /// <param name="rawToken">Raw token presented by the account.</param>
    /// <returns>True if the hashes match; otherwise false.</returns>
    public static bool VerifyRaw(ActivationToken token, string rawToken)
        => VerifyRaw(rawToken, token.Hash);

    /// <summary>
    /// Attempts to verify and consume the token atomically:
    /// - Fails fast if token is expired or already consumed at <paramref name="when"/>.
    /// - Compares the derived hash from <paramref name="rawToken"/> to the stored hash in constant time.
    /// - If valid and active, consumes the token (single-use).
    /// </summary>
    /// <param name="token">Persisted activation token entity.</param>
    /// <param name="rawToken">Raw token presented by the account.</param>
    /// <param name="when">Clock reference used for validation and stamping consumption.</param>
    /// <returns>True if the token was valid and transitioned to consumed; otherwise false.</returns>
    public static bool VerifyAndConsume(ActivationToken token, string rawToken, DateTimeOffset when)
    {
        // Guard on activity window first (cheap checks, no hashing if already invalid).
        if (!token.IsActive(when))
            return false;

        // Verify in constant time.
        if (!VerifyRaw(token, rawToken))
            return false;

        // Idempotent, non-throwing transition.
        token.Revoke(when, RevocationReason.Reissued);

        return token.IsRevoked;
    }

    /// <summary>
    /// Generates a 256-bit (32 bytes) cryptographically strong random token encoded as Base64Url without padding.
    /// </summary>
    public static string GenerateRawToken(int numBytes = 32)
    {
        if (numBytes <= 0) throw new ArgumentOutOfRangeException(nameof(numBytes));
        Span<byte> buffer = stackalloc byte[numBytes];
        RandomNumberGenerator.Fill(buffer);
        return Base64UrlEncode(buffer);
    }

    /// <summary>
    /// Computes the SHA-256 hash of a raw token as an uppercase hexadecimal string.
    /// </summary>
    public static string ComputeHash(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            throw new ArgumentException("Raw token must be provided.", nameof(raw));

        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(raw));
        return Convert.ToHexString(hashBytes); // uppercase hex
    }

    /// <summary>
    /// Verifies a raw token against a stored uppercase hex hash using constant-time comparison.
    /// </summary>
    /// <param name="rawToken">Raw token provided by the account.</param>
    /// <param name="storedHashHex">Uppercase hex representation of the stored hash.</param>
    public static bool VerifyRaw(string rawToken, string storedHashHex)
    {
        if (string.IsNullOrEmpty(storedHashHex)) return false;

        // Derive hash from the presented raw token.
        var presentedHashHex = ComputeHash(rawToken);

        // Fast-path length check to avoid allocations if clearly different.
        if (presentedHashHex.Length != storedHashHex.Length)
            return false;

        // Constant-time comparison over the underlying bytes.
        try
        {
            var a = Convert.FromHexString(presentedHashHex);
            var b = Convert.FromHexString(storedHashHex);
            return CryptographicOperations.FixedTimeEquals(a, b);
        }
        catch (FormatException)
        {
            // Stored hash should always be valid hex; treat malformed data as a mismatch.
            return false;
        }
    }

    /// <summary>
    /// Convenience that bundles generation of raw and hashed forms for callers that only need materials.
    /// Never persist the returned <c>RawToken</c>.
    /// </summary>
    /// <returns>(RawToken, HashedToken)</returns>
    public static (string RawToken, string HashedToken) CreateMaterials()
    {
        var raw = GenerateRawToken();
        var hash = ComputeHash(raw);
        return (raw, hash);
    }

    // --- Internal helpers ---

    private static string Base64UrlEncode(ReadOnlySpan<byte> bytes)
    {
        // Base64Url without padding per RFC 4648 §5.
        return Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }
}
