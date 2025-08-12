namespace ToDoApi.Identity.Contracts.Requests;

/// <summary>
/// Represents the credentials required for user authentication.
/// </summary>
public sealed class LoginRequest
{
    /// <summary>
    /// Gets or sets the user's email address.
    /// </summary>
    public string Email { get; set; } = default!;

    /// <summary>
    /// Gets or sets the user's plain text password.
    /// </summary>
    public string Password { get; set; } = default!;
}
