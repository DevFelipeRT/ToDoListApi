using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain.Accounts.ValueObjects;
using Domain.Lists.ValueObjects;
using Domain.Lists.Repositories;
using Domain.Lists.Policies;

namespace Application.Lists.Services;

/// <summary>
/// Provides functionality to check the uniqueness of a to-do list for a given account.
/// </summary>
public class ToDoListUniquenessChecker : IToDoListUniquenessPolicy
{
    private readonly IToDoListRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="ToDoListUniquenessChecker"/> class.
    /// </summary>
    /// <param name="repository">The to-do list repository to access persisted lists.</param>
    public ToDoListUniquenessChecker(IToDoListRepository repository)
    {
        _repository = repository;
    }

    /// <inheritdoc />
    public async Task<bool> IsTitleUniqueAsync(AccountId accountId, Title title, CancellationToken cancellationToken)
    {
        var lists = await _repository.GetAllByAccountAsync(accountId, cancellationToken);

        // Title equality comparison is handled by the VO (case-insensitive, trimmed)
        return !lists.Any(list => list.Title == title);
    }
}
