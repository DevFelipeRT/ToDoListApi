using MediatR;
using ToDoList.Application.Common.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace ToDoList.Application.ToDoItems.Commands.DeleteToDoItem;

/// <summary>
/// Handles the command to delete a To-Do item by its unique identifier.
/// </summary>
public sealed class DeleteToDoItemCommandHandler : IRequestHandler<DeleteToDoItemCommand, bool>
{
    private readonly IToDoItemRepository _toDoItemRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteToDoItemCommandHandler"/> class.
    /// </summary>
    /// <param name="toDoItemRepository">The repository for To-Do item data operations.</param>
    public DeleteToDoItemCommandHandler(IToDoItemRepository toDoItemRepository)
    {
        _toDoItemRepository = toDoItemRepository;
    }

    /// <summary>
    /// Handles the command to delete a To-Do item by its unique identifier.
    /// </summary>
    /// <param name="request">The command containing the To-Do item ID.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>True if the operation was successful; otherwise, false.</returns>
    public async Task<bool> Handle(DeleteToDoItemCommand request, CancellationToken cancellationToken)
    {
        var item = await _toDoItemRepository.GetByIdAsync(request.Id, cancellationToken);
        if (item is null)
            return false;

        await _toDoItemRepository.DeleteAsync(item, cancellationToken);
        return true;
    }
} 