using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain.Accounts.ValueObjects;
using Domain.Lists.Repositories;
using Domain.Lists.Services.Interfaces;
using Domain.Lists.ValueObjects;

namespace Application.Lists.Services;

/// <summary>
/// Provides functionality to check the uniqueness of a to-do list title for a given user.
/// </summary>
public class ToDoListTitleUniquenessChecker : IToDoListTitleUniquenessChecker
{
    private readonly IToDoListRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="ToDoListTitleUniquenessChecker"/> class.
    /// </summary>
    /// <param name="repository">The to-do list repository to access persisted lists.</param>
    public ToDoListTitleUniquenessChecker(IToDoListRepository repository)
    {
        _repository = repository;
    }

    /// <inheritdoc />
    public async Task<bool> IsTitleUniqueAsync(AccountId userId, Title title, CancellationToken cancellationToken)
    {
        var lists = await _repository.GetAllByUserAsync(userId, cancellationToken);

        // Title equality comparison is handled by the VO (case-insensitive, trimmed)
        return !lists.Any(list => list.Title == title);
    }
}
