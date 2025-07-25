namespace ToDoList.ToDoApi.Contracts.ToDoItems;

public sealed class ToDoItemHateoasCollectionResponse
{
    public List<ToDoItemHateoasResponse> Items { get; set; } = new();
    public List<LinkDto> Links { get; set; } = new();
}