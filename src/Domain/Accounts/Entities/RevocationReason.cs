namespace Domain.Accounts.Entities;

/// <summary>Reason that makes an activation token unusable.</summary>
public enum RevocationReason : byte
{
    /// <summary>Token was used to activate the account.</summary>
    UsedForActivation = 1,

    /// <summary>A new token was issued, invalidating the previous one.</summary>
    Reissued = 2,

    /// <summary>Administrative action explicitly revoked the token.</summary>
    AdminRevoked = 3,

    /// <summary>System cleanup revoked an expired token.</summary>
    ExpiredCleanup = 4
}
