using MediatR;

namespace ToDoList.Application.ToDoItems.Commands.DeleteToDoItem;

/// <summary>
/// Command to delete a To-Do item by its unique identifier.
/// </summary>
public sealed class DeleteToDoItemCommand : IRequest<bool>
{
    /// <summary>
    /// Gets the unique identifier of the To-Do item to delete.
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteToDoItemCommand"/> class.
    /// </summary>
    /// <param name="id">The unique identifier of the To-Do item.</param>
    public DeleteToDoItemCommand(int id)
    {
        Id = id;
    }
} 