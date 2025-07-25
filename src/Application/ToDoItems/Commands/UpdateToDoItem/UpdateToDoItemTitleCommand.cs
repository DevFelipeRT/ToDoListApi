using MediatR;

namespace ToDoList.Application.ToDoItems.Commands.UpdateToDoItem;

/// <summary>
/// Command to update the title of a To-Do item.
/// </summary>
public sealed class UpdateToDoItemTitleCommand : IRequest<bool>
{
    /// <summary>
    /// Gets the unique identifier of the To-Do item to update.
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// Gets the new title for the To-Do item.
    /// </summary>
    public string NewTitle { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateToDoItemTitleCommand"/> class.
    /// </summary>
    /// <param name="id">The unique identifier of the To-Do item.</param>
    /// <param name="newTitle">The new title for the To-Do item.</param>
    public UpdateToDoItemTitleCommand(int id, string newTitle)
    {
        Id = id;
        NewTitle = newTitle;
    }
}
