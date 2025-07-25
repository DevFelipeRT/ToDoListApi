using MediatR;

namespace ToDoList.Application.ToDoItems.Commands.UpdateToDoItem;

/// <summary>
/// Command to mark a To-Do item as incomplete.
/// </summary>
public sealed class MarkAsIncompleteCommand : IRequest<bool>
{
    /// <summary>
    /// Gets the unique identifier of the To-Do item to mark as incomplete.
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MarkAsIncompleteCommand"/> class.
    /// </summary>
    /// <param name="id">The unique identifier of the To-Do item.</param>
    public MarkAsIncompleteCommand(int id)
    {
        Id = id;
    }
}
