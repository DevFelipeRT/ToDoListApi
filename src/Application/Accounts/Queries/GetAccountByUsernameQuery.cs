using MediatR;
using Application.Accounts.DTOs;

namespace Application.Accounts.Queries;

/// <summary>
/// Query representing the intention to retrieve an account by username.
/// </summary>
public sealed class GetAccountByUsernameQuery : IRequest<AccountDto?>
{
    public string Username { get; }

    public GetAccountByUsernameQuery(string username)
    {
        Username = username;
    }
}
