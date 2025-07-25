using ToDoList.Domain.Entities;

namespace ToDoList.Application.Common.Interfaces;

/// <summary>
/// Defines the contract for data persistence operations for ToDoItem entities.
/// </summary>
public interface IToDoItemRepository
{
    /// <summary>
    /// Adds a new ToDoItem to the data store.
    /// </summary>
    /// <param name="item">The ToDoItem entity to add.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous add operation.</returns>
    Task AddAsync(ToDoItem item, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves all ToDoItem entities from the data store.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A list of all ToDoItem entities.</returns>
    Task<List<ToDoItem>> GetAllAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a ToDoItem entity by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the ToDoItem.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The ToDoItem entity if found; otherwise, null.</returns>
    Task<ToDoItem?> GetByIdAsync(int id, CancellationToken cancellationToken);

    /// <summary>
    /// Updates an existing ToDoItem entity in the data store.
    /// </summary>
    /// <param name="item">The ToDoItem entity to update.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous update operation.</returns>
    Task UpdateAsync(ToDoItem item, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes an existing ToDoItem entity from the data store.
    /// </summary>
    /// <param name="item">The ToDoItem entity to delete.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous delete operation.</returns>
    Task DeleteAsync(ToDoItem item, CancellationToken cancellationToken);
}