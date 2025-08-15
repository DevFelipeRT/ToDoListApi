namespace Application.Accounts.Queries;

/// <summary>
/// Query representing the intention to retrieve an account by email address.
/// </summary>
public sealed class GetAccountByEmailQuery
{
    public string Email { get; }

    public GetAccountByEmailQuery(string email)
    {
        Email = email;
    }
}
