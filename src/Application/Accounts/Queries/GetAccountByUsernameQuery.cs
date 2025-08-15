namespace Application.Accounts.Queries;

/// <summary>
/// Query representing the intention to retrieve an account by username.
/// </summary>
public sealed class GetAccountByUsernameQuery
{
    public string Username { get; }

    public GetAccountByUsernameQuery(string username)
    {
        Username = username;
    }
}
