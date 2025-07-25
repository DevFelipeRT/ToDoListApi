using MediatR;
using ToDoList.Application.Common.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace ToDoList.Application.ToDoItems.Commands.UpdateToDoItem;

/// <summary>
/// Handles the command to mark a To-Do item as incomplete.
/// </summary>
public sealed class MarkAsIncompleteCommandHandler : IRequestHandler<MarkAsIncompleteCommand, bool>
{
    private readonly IToDoItemRepository _toDoItemRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="MarkAsIncompleteCommandHandler"/> class.
    /// </summary>
    /// <param name="toDoItemRepository">The repository for To-Do item data operations.</param>
    public MarkAsIncompleteCommandHandler(IToDoItemRepository toDoItemRepository)
    {
        _toDoItemRepository = toDoItemRepository;
    }

    /// <summary>
    /// Handles the command to mark a To-Do item as incomplete.
    /// </summary>
    /// <param name="request">The command containing the To-Do item ID.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>True if the operation was successful; otherwise, false.</returns>
    public async Task<bool> Handle(MarkAsIncompleteCommand request, CancellationToken cancellationToken)
    {
        var item = await _toDoItemRepository.GetByIdAsync(request.Id, cancellationToken);
        if (item is null)
            return false;

        item.MarkAsIncomplete();
        await _toDoItemRepository.UpdateAsync(item, cancellationToken);
        return true;
    }
}
