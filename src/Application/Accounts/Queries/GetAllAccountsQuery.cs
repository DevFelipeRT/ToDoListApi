using MediatR;
using System.Collections.Generic;
using Application.Accounts.DTOs;

namespace Application.Accounts.Queries;

/// <summary>
/// Query that requests retrieval of all accounts in the system.
/// This query can be extended to support pagination or filtering if required.
/// </summary>
public sealed class GetAllAccountsQuery : IRequest<IReadOnlyCollection<AccountDto>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetAllAccountsQuery"/> class.
    /// </summary>
    public GetAllAccountsQuery()
    {
    }
}
