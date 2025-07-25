using MediatR;
using ToDoList.Application.Common.Interfaces;
using ToDoList.Application.Dtos;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ToDoList.Application.ToDoItems.Queries.GetAllToDoItems;

/// <summary>
/// Handles the retrieval of all To-Do items.
/// </summary>
public sealed class GetAllToDoItemsQueryHandler : IRequestHandler<GetAllToDoItemsQuery, List<ToDoItemDto>>
{
    private readonly IToDoItemRepository _toDoItemRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetAllToDoItemsQueryHandler"/> class.
    /// </summary>
    /// <param name="toDoItemRepository">The repository for To-Do item data operations.</param>
    public GetAllToDoItemsQueryHandler(IToDoItemRepository toDoItemRepository)
    {
        _toDoItemRepository = toDoItemRepository;
    }

    /// <summary>
    /// Handles the incoming query to retrieve all To-Do items.
    /// </summary>
    /// <param name="request">The query request.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A list of <see cref="ToDoItemDto"/> representing all To-Do items.</returns>
    public async Task<List<ToDoItemDto>> Handle(GetAllToDoItemsQuery request, CancellationToken cancellationToken)
    {
        var items = await _toDoItemRepository.GetAllAsync(cancellationToken);

        var result = new List<ToDoItemDto>(items.Count);
        foreach (var item in items)
        {
            result.Add(new ToDoItemDto
            {
                Id = item.Id,
                Title = item.Title.Value,
                IsCompleted = item.IsCompleted,
                CreatedAt = item.CreatedAt,
                CompletedAt = item.CompletedAt
            });
        }

        return result;
    }
}
