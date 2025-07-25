using MediatR;
using ToDoList.Application.Dtos;

namespace ToDoList.Application.ToDoItems.Queries.GetToDoItem;

/// <summary>
/// Query to retrieve a single To-Do item by its unique identifier.
/// </summary>
public sealed class GetToDoItemQuery : IRequest<ToDoItemDto?>
{
    /// <summary>
    /// Gets the unique identifier of the To-Do item to retrieve.
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GetToDoItemQuery"/> class.
    /// </summary>
    /// <param name="id">The unique identifier of the To-Do item.</param>
    public GetToDoItemQuery(int id)
    {
        Id = id;
    }
}
