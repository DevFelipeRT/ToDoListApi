namespace ToDoList.ToDoApi.Contracts.ToDoItems;

public sealed class ToDoItemHateoasResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public List<LinkDto> Links { get; set; } = new();
}

public sealed class LinkDto
{
    public string Rel { get; set; } = string.Empty;
    public string Href { get; set; } = string.Empty;
    public string Method { get; set; } = string.Empty;
} 