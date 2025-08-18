namespace Api.Identity.Contracts.Requests;

/// <summary>
/// Represents the credentials required for account authentication.
/// </summary>
public sealed class LoginRequest
{
    /// <summary>
    /// Gets or sets the account's email address.
    /// </summary>
    public string Email { get; set; } = default!;

    /// <summary>
    /// Gets or sets the account's plain text password.
    /// </summary>
    public string Password { get; set; } = default!;
}
