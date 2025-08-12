namespace Application.Accounts.Services.Interfaces;

/// <summary>
/// Defines the contract for generating JSON Web Tokens (JWT) for authenticated users.
/// </summary>
public interface IJwtTokenService
{
    /// <summary>
    /// Generates a signed JWT token containing the specified user information and claims.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="username">The username of the user.</param>
    /// <param name="roles">A collection of roles to be included as claims (optional).</param>
    /// <returns>A signed JWT token as a string.</returns>
    string GenerateToken(Guid userId, string username, IEnumerable<string>? roles = null);
}
