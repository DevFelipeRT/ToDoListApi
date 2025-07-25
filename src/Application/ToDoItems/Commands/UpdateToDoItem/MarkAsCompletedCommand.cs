using MediatR;

namespace ToDoList.Application.ToDoItems.Commands.UpdateToDoItem;

/// <summary>
/// Command to mark a To-Do item as completed.
/// </summary>
public sealed class MarkAsCompletedCommand : IRequest<bool>
{
    /// <summary>
    /// Gets the unique identifier of the To-Do item to mark as completed.
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MarkAsCompletedCommand"/> class.
    /// </summary>
    /// <param name="id">The unique identifier of the To-Do item.</param>
    public MarkAsCompletedCommand(int id)
    {
        Id = id;
    }
}
