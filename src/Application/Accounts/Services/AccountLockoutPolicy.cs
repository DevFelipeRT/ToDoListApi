using System.Collections.Concurrent;
using Domain.Accounts.Services.Interfaces;
using Domain.Accounts.ValueObjects;

namespace Application.Accounts.Services;

/// <summary>
/// Implementation of account lockout policy service.
/// </summary>
public sealed class AccountLockoutPolicy : IAccountLockoutPolicy
{
    private readonly ConcurrentDictionary<AccountId, FailedAttemptInfo> _failedAttempts = new();
    private const int MaxFailedAttempts = 5;
    private static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(15);

    public Task RegisterFailedAttempt(AccountId accountId, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        
        _failedAttempts.AddOrUpdate(accountId, 
            new FailedAttemptInfo(1, now),
            (key, existing) =>
            {
                // Reset counter if last attempt was too long ago
                if (now - existing.LastAttempt > LockoutDuration)
                {
                    return new FailedAttemptInfo(1, now);
                }
                
                return new FailedAttemptInfo(existing.Count + 1, now);
            });

        return Task.CompletedTask;
    }

    public Task<bool> IsLockedOut(AccountId accountId, CancellationToken cancellationToken)
    {
        if (!_failedAttempts.TryGetValue(accountId, out var attemptInfo))
            return Task.FromResult(false);

        var now = DateTime.UtcNow;
        
        // Check if lockout period has expired
        if (now - attemptInfo.LastAttempt > LockoutDuration)
        {
            _failedAttempts.TryRemove(accountId, out _);
            return Task.FromResult(false);
        }

        // Account is locked if failed attempts exceed threshold
        return Task.FromResult(attemptInfo.Count >= MaxFailedAttempts);
    }

    public Task ResetLockout(AccountId accountId, CancellationToken cancellationToken)
    {
        _failedAttempts.TryRemove(accountId, out _);
        return Task.CompletedTask;
    }

    private sealed record FailedAttemptInfo(int Count, DateTime LastAttempt);
}