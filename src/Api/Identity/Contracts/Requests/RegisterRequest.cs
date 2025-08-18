namespace Api.Identity.Contracts.Requests;

/// <summary>
/// Represents the information required to register a new account account.
/// </summary>
public sealed class RegisterRequest
{
    /// <summary>
    /// Gets or sets the account's display name.
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// Gets or sets the desired username for the account.
    /// </summary>
    public string Username { get; set; } = default!;

    /// <summary>
    /// Gets or sets the account's email address.
    /// </summary>
    public string Email { get; set; } = default!;

    /// <summary>
    /// Gets or sets the account's plain text password.
    /// </summary>
    public string Password { get; set; } = default!;
}
