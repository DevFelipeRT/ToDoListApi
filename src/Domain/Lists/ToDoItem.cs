using Domain.Lists.ValueObjects;

namespace Domain.Lists;

/// <summary>
/// Represents a single To-Do item.
/// All business rules for manipulating a To-Do item are encapsulated here.
/// </summary>
public class ToDoItem
{
    /// <summary>
    /// Gets the unique identifier for the To-Do item.
    /// </summary>
    public ToDoItemId Id { get; private set; }

    /// <summary>
    /// Gets the title of the task, represented as a Value Object.
    /// </summary>
    public Title Title { get; private set; } = null!;

    /// <summary>
    /// Gets a value indicating whether the task has been completed.
    /// </summary>
    public bool IsCompleted { get; private set; }

    /// <summary>
    /// Gets the date and time when the task is due. Null if no due date is set.
    /// </summary>
    public DueDate? DueDate { get; private set; }

    /// <summary>
    /// Gets the date and time when the task was created.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Gets the date and time when the task was completed. Null if not yet completed.
    /// </summary>
    public DateTime? CompletedAt { get; private set; }

    /// <summary>
    /// Indicates if the To-Do item has a due date set.
    /// </summary>
    public bool HasDueDate => DueDate != null;

    /// <summary>
    /// Indicates if the To-Do item is overdue.
    /// </summary>
    public bool IsOverdue => DueDate != null && !IsCompleted && DueDate.Value < DateTime.UtcNow;

    /// <summary>
    /// For ORM and serialization purposes only.
    /// </summary>
    private ToDoItem()
    {
        Id = default!;
        Title = default!;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ToDoItem"/> class with a specified data.
    /// </summary>
    /// <param name="id">The unique identifier for the To-Do item.</param>
    /// <param name="title">The title for the task.</param>
    /// <param name="dueDate">The due date for the task (optional).</param>
    public ToDoItem(ToDoItemId id, Title title, DateTime createdAt, bool isCompleted, DueDate? dueDate = null)
    {
        Id = id;
        Title = title;
        CreatedAt = createdAt;
        IsCompleted = isCompleted;
        DueDate = dueDate;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ToDoItem"/> class with generated ID.
    /// </summary>
    /// <param name="title">The title for the task.</param>
    /// <param name="dueDate">The due date for the task (optional).</param>
    public ToDoItem(Title title, DueDate? dueDate = null)
    {
        Id = ToDoItemId.New();

        if (dueDate is not null)
            ValidateDueDate(dueDate);
            
        Title = title;
        DueDate = dueDate;
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

    /// <summary>
    /// Updates the due date of the To-Do item.
    /// The new due date must not be in the past.
    /// </summary>
    /// <param name="newDueDate">The new due date for the task.</param>
    /// <exception cref="ArgumentException">Thrown if the due date is in the past.</exception>
    public void UpdateDueDate(DueDate newDueDate)
    {
        ValidateDueDate(newDueDate);
        DueDate = newDueDate;
    }

    /// <summary>
    /// Sets a reminder for this to-do item.
    /// </summary>
    /// <param name="reminderDate">The date and time when the reminder should be triggered.</param>
    /// <exception cref="ArgumentException">Thrown if the reminder date is in the past.</exception>
    public void SetDueDate(DueDate reminderDate)
    {
        ValidateDueDate(reminderDate);
        DueDate = reminderDate;
    }

    /// <summary>
    /// Removes the reminder from this to-do item.
    /// </summary>
    public void RemoveDueDate()
    {
        DueDate = null;
    }

    /// <summary>
    /// Validates that the due date is not in the past.
    /// </summary>
    /// <param name="dueDate">The due date to validate.</param>
    /// <exception cref="ArgumentException">Thrown if the due date is in the past.</exception>
    private void ValidateDueDate(DueDate dueDate)
    {
        if (dueDate.HasValue && dueDate.Value < DateTime.UtcNow)
            throw new ArgumentException("Due date must not be in the past.", nameof(dueDate));
    }
}
