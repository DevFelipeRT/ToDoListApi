namespace Api.Identity.Contracts.Responses;

/// <summary>
/// Represents the response returned upon successful authentication, containing the JWT token and basic account information.
/// </summary>
public sealed class AuthResponse
{
    /// <summary>
    /// Gets or sets the JWT authentication token.
    /// </summary>
    public string Token { get; set; } = default!;

    /// <summary>
    /// Gets or sets the account's unique identifier.
    /// </summary>
    public Guid AccountId { get; set; }

    /// <summary>
    /// Gets or sets the account's username.
    /// </summary>
    public string Username { get; set; } = default!;

    /// <summary>
    /// Gets or sets the account's display name.
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// Gets or sets the account's email address.
    /// </summary>
    public string Email { get; set; } = default!;
}
