using MediatR;
using ToDoList.Application.Common.Interfaces;
using ToDoList.Application.Dtos;
using System.Threading;
using System.Threading.Tasks;

namespace ToDoList.Application.ToDoItems.Queries.GetToDoItem;

/// <summary>
/// Handles the retrieval of a single To-Do item by its unique identifier.
/// </summary>
public sealed class GetToDoItemQueryHandler : IRequestHandler<GetToDoItemQuery, ToDoItemDto?>
{
    private readonly IToDoItemRepository _toDoItemRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetToDoItemQueryHandler"/> class.
    /// </summary>
    /// <param name="toDoItemRepository">The repository for To-Do item data operations.</param>
    public GetToDoItemQueryHandler(IToDoItemRepository toDoItemRepository)
    {
        _toDoItemRepository = toDoItemRepository;
    }

    /// <summary>
    /// Handles the incoming query to retrieve a single To-Do item by its unique identifier.
    /// </summary>
    /// <param name="request">The query request containing the To-Do item ID.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The <see cref="ToDoItemDto"/> if found; otherwise, null.</returns>
    public async Task<ToDoItemDto?> Handle(GetToDoItemQuery request, CancellationToken cancellationToken)
    {
        var item = await _toDoItemRepository.GetByIdAsync(request.Id, cancellationToken);
        if (item is null)
            return null;

        return new ToDoItemDto
        {
            Id = item.Id,
            Title = item.Title.Value,
            IsCompleted = item.IsCompleted,
            CreatedAt = item.CreatedAt,
            CompletedAt = item.CompletedAt
        };
    }
}
