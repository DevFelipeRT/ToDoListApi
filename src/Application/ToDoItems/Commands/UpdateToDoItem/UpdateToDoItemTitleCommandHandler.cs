using MediatR;
using ToDoList.Application.Common.Interfaces;
using ToDoList.Domain.ValueObjects;
using System.Threading;
using System.Threading.Tasks;

namespace ToDoList.Application.ToDoItems.Commands.UpdateToDoItem;

/// <summary>
/// Handles the command to update the title of a To-Do item.
/// </summary>
public sealed class UpdateToDoItemTitleCommandHandler : IRequestHandler<UpdateToDoItemTitleCommand, bool>
{
    private readonly IToDoItemRepository _toDoItemRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateToDoItemTitleCommandHandler"/> class.
    /// </summary>
    /// <param name="toDoItemRepository">The repository for To-Do item data operations.</param>
    public UpdateToDoItemTitleCommandHandler(IToDoItemRepository toDoItemRepository)
    {
        _toDoItemRepository = toDoItemRepository;
    }

    /// <summary>
    /// Handles the command to update the title of a To-Do item.
    /// </summary>
    /// <param name="request">The command containing the To-Do item ID and new title.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>True if the operation was successful; otherwise, false.</returns>
    public async Task<bool> Handle(UpdateToDoItemTitleCommand request, CancellationToken cancellationToken)
    {
        var item = await _toDoItemRepository.GetByIdAsync(request.Id, cancellationToken);
        if (item is null)
            return false;

        item.UpdateTitle(new Title(request.NewTitle));
        await _toDoItemRepository.UpdateAsync(item, cancellationToken);
        return true;
    }
}
