using System;

namespace Application.IdentityAccess.Contracts
{
    /// <summary>
    /// Represents a password reset token issued by the external identity provider.
    /// </summary>
    public sealed record PasswordResetToken(string Token, DateTimeOffset? ExpiresAt);
}
