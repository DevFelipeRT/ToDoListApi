using System;
using MediatR;

namespace Application.Lists.Commands.Lists;

/// <summary>
/// Command to create a new To-Do list for a user.
/// </summary>
public sealed class CreateToDoListCommand : IRequest<Guid>
{
    /// <summary>
    /// Gets the identifier of the user who owns the list.
    /// </summary>
    public Guid UserId { get; }

    /// <summary>
    /// Gets the title of the new To-Do list.
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Gets the optional description of the To-Do list.
    /// </summary>
    public string? Description { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateToDoListCommand"/> class.
    /// </summary>
    /// <param name="userId">The identifier of the user.</param>
    /// <param name="title">The title of the list.</param>
    /// <param name="description">The optional description of the list.</param>
    public CreateToDoListCommand(Guid userId, string title, string? description = null)
    {
        UserId = userId;
        Title = title;
        Description = description;
    }
}
