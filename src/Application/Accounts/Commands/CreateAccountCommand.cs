using System;
using MediatR;

namespace Application.Accounts.Commands;

/// <summary>
/// Command representing the intention to create a new account.
/// </summary>
public sealed class CreateAccountCommand : IRequest<Guid>
{
    public string Email { get; }
    public string Username { get; }
    public string Name { get; }
    public string PlainPassword { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateAccountCommand"/> class.
    /// </summary>
    public CreateAccountCommand(
        string email,
        string username,
        string name,
        string plainPassword)
    {
        Email = email;
        Username = username;
        Name = name;
        PlainPassword = plainPassword;
    }
}
