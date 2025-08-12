using MediatR;

namespace Application.Accounts.Commands.Users;

/// <summary>
/// Command representing the intention to create a new user.
/// </summary>
public sealed class CreateUserCommand : IRequest
{
    public Guid UserId { get; }
    public string Email { get; }
    public string Username { get; }
    public string Name { get; }
    public string PlainPassword { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateUserCommand"/> class.
    /// </summary>
    public CreateUserCommand(
        Guid userId,
        string email,
        string username,
        string name,
        string plainPassword)
    {
        UserId = userId;
        Email = email;
        Username = username;
        Name = name;
        PlainPassword = plainPassword;
    }
}
