using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ToDoApi.Identity.Configuration;
using Application.Accounts.Services.Interfaces;

namespace ToDoApi.Identity.Services;

/// <summary>
/// Service responsible for generating JWT tokens for authenticated users, using secure, production-grade configuration.
/// </summary>
public sealed class JwtTokenService : IJwtTokenService
{
    private readonly JwtSettings _settings;
    private readonly byte[] _secretKeyBytes;

    /// <summary>
    /// Initializes a new instance of the <see cref="JwtTokenService"/> class.
    /// </summary>
    /// <param name="options">The JWT configuration options.</param>
    public JwtTokenService(IOptions<JwtSettings> options)
    {
        _settings = options?.Value ?? throw new ArgumentNullException(nameof(options));
        if (string.IsNullOrWhiteSpace(_settings.SecretKey))
            throw new ArgumentException("JWT secret key must be configured.", nameof(_settings.SecretKey));
        _secretKeyBytes = Encoding.UTF8.GetBytes(_settings.SecretKey);
    }

    /// <summary>
    /// Generates a signed JWT token containing the specified user information and claims.
    /// </summary>
    /// <param name="userId">The user's unique identifier.</param>
    /// <param name="username">The user's username.</param>
    /// <param name="roles">A collection of roles to be included as claims (optional).</param>
    /// <returns>A signed JWT token as a string.</returns>
    public string GenerateToken(Guid userId, string username, IEnumerable<string>? roles = null)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())
        };

        if (roles != null)
        {
            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(_secretKeyBytes),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddMinutes(_settings.ExpirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
