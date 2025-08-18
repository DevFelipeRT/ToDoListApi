using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain.Accounts.ValueObjects;
using Domain.Lists.Entities;
using Domain.Lists.ValueObjects;

namespace Domain.Lists.Repositories;

/// <summary>
/// Defines the contract for data persistence operations for ToDoList aggregates.
/// </summary>
public interface IToDoListRepository
{
    /// <summary>
    /// Adds a new ToDoList aggregate to the data store.
    /// </summary>
    /// <param name="list">The ToDoList aggregate to add.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous add operation.</returns>
    Task AddAsync(ToDoList list, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves all ToDoList aggregates for the specified account from the data store.
    /// </summary>
    /// <param name="accountId">The identifier of the account whose lists to retrieve.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task that returns a collection of ToDoList aggregates for the account.</returns>
    Task<IReadOnlyCollection<ToDoList>> GetAllByAccountAsync(AccountId accountId, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves the unique identifiers of all ToDoList aggregates for the specified account.
    /// </summary>
    /// <param name="accountId">The identifier of the account whose list IDs to retrieve.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task that returns a collection of ToDoListId values for the account.</returns>
    Task<IReadOnlyList<ToDoListId>> GetIdsByAccountAsync(AccountId accountId, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a ToDoList aggregate by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the ToDoList.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>
    /// A task that returns the ToDoList aggregate if found; otherwise, <c>null</c>.
    /// </returns>
    Task<ToDoList?> GetByIdAsync(ToDoListId id, CancellationToken cancellationToken);

    /// <summary>
    /// Updates an existing ToDoList aggregate in the data store.
    /// </summary>
    /// <param name="list">The ToDoList aggregate to update.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous update operation.</returns>
    Task UpdateAsync(ToDoList list, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes an existing ToDoList aggregate from the data store by its id.
    /// </summary>
    /// <param name="id">The unique identifier of the ToDoList to delete.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous delete operation.</returns>
    Task DeleteAsync(ToDoListId id, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a specific to-do item by its identifier, checking ownership.
    /// </summary>
    /// <param name="listId">The unique identifier of the to-do list.</param>
    /// <param name="itemId">The unique identifier of the to-do item.</param>
    /// <param name="accountId">The unique identifier of the account (for ownership validation).</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The to-do item if found and owned by the account; otherwise, null.</returns>
    Task<ToDoItem?> GetItemByIdAsync(
        ToDoListId listId,
        ToDoItemId itemId,
        AccountId accountId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves all to-do items with due dates for a collection of list IDs.
    /// </summary>
    /// <param name="listIds">The unique identifiers of the to-do lists.</param>
    /// <param name="fromDate">Optional start date to filter due dates (inclusive).</param>
    /// <param name="toDate">Optional end date to filter due dates (inclusive).</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task that returns a collection of tuples where Item is the to-do item and ListId identifies its parent list.</returns>
    Task<IReadOnlyCollection<(ToDoItem Item, ToDoListId ListId)>> GetItemsWithDueDateByListIdsAsync(
        IReadOnlyCollection<ToDoListId> listIds,
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves all items in a specific list, checking ownership.
    /// </summary>
    /// <param name="listId">The unique identifier of the to-do list.</param>
    /// <param name="accountId">The unique identifier of the account (for ownership validation).</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A collection of to-do items in the list if owned by the account.</returns>
    Task<IReadOnlyCollection<ToDoItem>> GetAllItemsByListIdAndAccountAsync(
        ToDoListId listId,
        AccountId accountId,
        CancellationToken cancellationToken);
}
