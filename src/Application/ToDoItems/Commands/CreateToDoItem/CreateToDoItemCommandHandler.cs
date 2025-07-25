using MediatR;
using ToDoList.Application.Common.Interfaces;
using ToDoList.Domain.Entities;
using ToDoList.Domain.ValueObjects;

namespace ToDoList.Application.ToDoItems.Commands.CreateToDoItem;

/// <summary>
/// Handles the creation of a new To-Do item.
/// </summary>
public sealed class CreateToDoItemCommandHandler : IRequestHandler<CreateToDoItemCommand>
{
    private readonly IToDoItemRepository _toDoItemRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateToDoItemCommandHandler"/> class.
    /// </summary>
    /// <param name="toDoItemRepository">The repository for To-Do item data operations.</param>
    public CreateToDoItemCommandHandler(IToDoItemRepository toDoItemRepository)
    {
        _toDoItemRepository = toDoItemRepository;
    }

    /// <summary>
    /// Handles the incoming command to create a new To-Do item.
    /// </summary>
    /// <param name="command">The command containing the data for the new item.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the completion of the handler's execution.</returns>
    public async Task Handle(CreateToDoItemCommand command, CancellationToken cancellationToken)
    {
        var title = new Title(command.Title);
        
        var toDoItem = new ToDoItem(title);

        await _toDoItemRepository.AddAsync(toDoItem, cancellationToken);
    }
}