namespace ToDoApi.Identity.Contracts.Responses;

/// <summary>
/// Represents the response returned upon successful authentication, containing the JWT token and basic user information.
/// </summary>
public sealed class AuthResponse
{
    /// <summary>
    /// Gets or sets the JWT authentication token.
    /// </summary>
    public string Token { get; set; } = default!;

    /// <summary>
    /// Gets or sets the user's unique identifier.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the user's username.
    /// </summary>
    public string Username { get; set; } = default!;

    /// <summary>
    /// Gets or sets the user's display name.
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// Gets or sets the user's email address.
    /// </summary>
    public string Email { get; set; } = default!;
}
