using MediatR;
using System.Collections.Generic;
using ToDoList.Application.Dtos;

namespace ToDoList.Application.ToDoItems.Queries.GetAllToDoItems;

/// <summary>
/// Query to retrieve all To-Do items.
/// </summary>
public sealed class GetAllToDoItemsQuery : IRequest<List<ToDoItemDto>>
{
    // No properties needed, as it returns all items.
}
