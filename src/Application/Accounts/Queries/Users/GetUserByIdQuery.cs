namespace Application.Accounts.Queries.Users;

/// <summary>
/// Query representing the intention to retrieve a user by unique identifier.
/// </summary>
public sealed class GetUserByIdQuery
{
    public Guid UserId { get; }

    public GetUserByIdQuery(Guid userId)
    {
        UserId = userId;
    }
}
