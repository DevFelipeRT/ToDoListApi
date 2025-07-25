using MediatR;

namespace ToDoList.Application.ToDoItems.Commands.CreateToDoItem;

/// <summary>
/// Represents the command for creating a new To-Do item.
/// </summary>
public sealed class CreateToDoItemCommand : IRequest
{
    /// <summary>
    /// Gets the title for the new To-Do item.
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateToDoItemCommand"/> class.
    /// </summary>
    /// <param name="title">The title of the To-Do item.</param>
    public CreateToDoItemCommand(string title)
    {
        Title = title;
    }
}