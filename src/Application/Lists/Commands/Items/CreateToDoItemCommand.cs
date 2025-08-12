using System;
using MediatR;

namespace Application.Lists.Commands.Items;

/// <summary>
/// Command to request the creation of a new To-Do item within a specific To-Do list.
/// Carries all primitive input data required for the operation, 
/// and serves as the transport object between the application and domain layers.
/// </summary>
public sealed class CreateToDoItemCommand : IRequest<Guid>
{
    /// <summary>
    /// Gets the unique identifier of the list where the item should be created.
    /// </summary>
    public Guid ListId { get; init; }

    /// <summary>
    /// Gets the unique identifier of the user attempting to create the item.
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// Gets the title of the To-Do item to be created.
    /// </summary>
    public string Title { get; init; } = string.Empty;

    /// <summary>
    /// Gets the optional due date for the To-Do item.
    /// </summary>
    public DateTime? DueDate { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateToDoItemCommand"/> class.
    /// </summary>
    /// <param name="listId">The unique identifier of the list in which to create the item.</param>
    /// <param name="userId">The unique identifier of the user creating the item.</param>
    /// <param name="title">The title of the new To-Do item.</param>
    /// <param name="dueDate">The optional due date of the item.</param>
    public CreateToDoItemCommand(Guid listId, Guid userId, string title, DateTime? dueDate = null)
    {
        ListId = listId;
        UserId = userId;
        Title = title;
        DueDate = dueDate;
    }
}
