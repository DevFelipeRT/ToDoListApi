using System.Threading;
using System.Threading.Tasks;
using Domain.Accounts.ValueObjects;
using Domain.Lists.ValueObjects;

namespace Domain.Lists.Services.Interfaces;

/// <summary>
/// Contract for checking the uniqueness of a To-Do list title for a given user.
/// </summary>
public interface IToDoListTitleUniquenessChecker
{
    /// <summary>
    /// Checks if the provided title is unique for the specified user.
    /// </summary>
    /// <param name="userId">The identifier of the user to check the title against.</param>
    /// <param name="title">The title to check for uniqueness.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>True if the title is unique for the user; otherwise, false.</returns>
    Task<bool> IsTitleUniqueAsync(AccountId userId, Title title, CancellationToken cancellationToken);
}
