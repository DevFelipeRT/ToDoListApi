using MediatR;
using Application.Accounts.DTOs;

namespace Application.Accounts.Queries;

/// <summary>
/// Query representing the intention to retrieve an account by its credential ID.
/// </summary>
public sealed class GetAccountByCredentialIdQuery : IRequest<AccountDto?>
{
    public string CredentialIdValue { get; }

    public GetAccountByCredentialIdQuery(string credentialIdValue)
    {
        CredentialIdValue = credentialIdValue;
    }
}
