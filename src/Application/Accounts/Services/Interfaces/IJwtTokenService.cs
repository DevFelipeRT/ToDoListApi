namespace Application.Accounts.Services.Interfaces;

/// <summary>
/// Defines the contract for generating JSON Web Tokens (JWT) for authenticated accounts.
/// </summary>
public interface IJwtTokenService
{
    /// <summary>
    /// Generates a signed JWT token containing the specified account information and claims.
    /// </summary>
    /// <param name="accountId">The unique identifier of the account.</param>
    /// <param name="username">The username of the account.</param>
    /// <param name="roles">A collection of roles to be included as claims (optional).</param>
    /// <returns>A signed JWT token as a string.</returns>
    string GenerateToken(Guid accountId, string username, IEnumerable<string>? roles = null);
}
