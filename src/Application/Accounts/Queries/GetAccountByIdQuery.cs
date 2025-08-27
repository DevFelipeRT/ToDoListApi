using MediatR;
using Application.Accounts.DTOs;

namespace Application.Accounts.Queries;

/// <summary>
/// Query representing the intention to retrieve an account by unique identifier.
/// </summary>
public sealed class GetAccountByIdQuery : IRequest<AccountDto?>
{
    public Guid AccountId { get; }

    public GetAccountByIdQuery(Guid accountId)
    {
        AccountId = accountId;
    }
}
