using ToDoList.Domain.ValueObjects;

namespace ToDoList.Domain.Entities;

/// <summary>
/// Represents a single To-Do item, serving as the Aggregate Root.
/// All business rules for manipulating a To-Do item are encapsulated here.
/// </summary>
public class ToDoItem
{
    /// <summary>
    /// Gets the unique identifier for the To-Do item.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    /// Gets the title of the task, represented as a Value Object.
    /// </summary>
    public Title Title { get; private set; } = null!;

    /// <summary>
    /// Gets a value indicating whether the task has been completed.
    /// </summary>
    public bool IsCompleted { get; private set; }

    /// <summary>
    /// Gets the date and time when the task was created.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Gets the date and time when the task was completed. Null if not yet completed.
    /// </summary>
    public DateTime? CompletedAt { get; private set; }

    private ToDoItem() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ToDoItem"/> class.
    /// </summary>
    /// <param name="title">The title for the task.</param>
    public ToDoItem(Title title)
    {
        Title = title;
        IsCompleted = false;
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks the To-Do item as completed.
    /// </summary>
    public void MarkAsCompleted()
    {
        if (IsCompleted) return;
        IsCompleted = true;
        CompletedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks the To-Do item as incomplete.
    /// </summary>
    public void MarkAsIncomplete()
    {
        if (!IsCompleted) return;
        IsCompleted = false;
        CompletedAt = null;
    }
    
    /// <summary>
    /// Updates the title of the To-Do item.
    /// </summary>
    /// <param name="newTitle">The new title for the task.</param>
    public void UpdateTitle(Title newTitle)
    {
        Title = newTitle;
    }
}