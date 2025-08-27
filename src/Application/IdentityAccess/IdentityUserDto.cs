namespace Application.IdentityAccess;

/// <summary>
/// Lightweight data transfer object representing a user in the external identity provider.
/// </summary>
public sealed class IdentityUserDto
{
    public string Email { get; }
    public string Username { get; }
    public bool EmailConfirmed { get; }

    public IdentityUserDto(string email, string username, bool emailConfirmed)
    {
        Email = email ?? throw new ArgumentNullException(nameof(email));
        Username = username ?? throw new ArgumentNullException(nameof(username));
        EmailConfirmed = emailConfirmed;
    }
}
