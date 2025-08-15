using System;
using System.Collections.Generic;
using Domain.Accounts.Entities;
using Domain.Accounts.ValueObjects;
using Domain.Lists.ValueObjects;

namespace Domain.Lists.Entities;

/// <summary>
/// Represents a single To-Do list aggregate root.
/// Encapsulates all business rules for managing the list and its items.
/// </summary>
public class ToDoList
{
    /// <summary>
    /// Gets the unique identifier for the To-Do list.
    /// </summary>
    public ToDoListId Id { get; private set; }

    /// <summary>
    /// Gets the account identifier who owns this To-Do list.
    /// </summary>
    public AccountId AccountId { get; private set; }

    /// <summary>
    /// Gets the title of the list.
    /// </summary>
    public Title Title { get; private set; } = null!;

    /// <summary>
    /// Gets the description of the list.
    /// </summary>
    public Description? Description { get; private set; }

    /// <summary>
    /// Collection of To-Do items managed by this list.
    /// All business rules related to items are enforced through this collection.
    /// </summary>
    private readonly ToDoItemCollection _items = new();

    /// <summary>
    /// Internal accessor used exclusively by the infrastructure layer (EF Core) for persistence mapping.
    /// Not intended for use in business logic.
    /// </summary>
    internal List<ToDoItem> ItemsForEfCore => _items.InternalList;

    /// <summary>
    /// Gets whether the list has been marked as completed.
    /// </summary>
    public bool IsCompleted { get; private set; }

    /// <summary>
    /// Gets the date and time the list was created.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Gets the date and time the list was completed, or null if not completed.
    /// </summary>
    public DateTime? CompletedAt { get; private set; }

    /// <summary>
    /// For ORM and serialization purposes only.
    /// </summary>
    private ToDoList()
    {
        Id = default!;
        AccountId = default!;
        Title = default!;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ToDoList"/> class with a specified ID.
    /// </summary>
    /// <param name="id">The unique identifier of the list.</param>
    /// <param name="accountId">The identifier of the account who owns the list.</param>
    /// <param name="title">The validated title of the list.</param>
    /// <param name="description">The validated description of the list (optional).</param>
    public ToDoList(ToDoListId id, AccountId accountId, Title title, Description? description = null)
    {
        Id = id;
        AccountId = accountId;
        Title = title;
        CreatedAt = DateTime.UtcNow;
        IsCompleted = false;
        Description = description;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ToDoList"/> class with a generated ID.
    /// </summary>
    /// <param name="accountId">The identifier of the account who owns the list.</param>
    /// <param name="title">The validated title of the list.</param>
    /// <param name="description">The validated description of the list (optional).</param>
    public ToDoList(AccountId accountId, Title title, Description? description = null)
        : this(ToDoListId.New(), accountId, title, description) { }

    /// <summary>
    /// Verifies if the list belongs to a given account.
    /// </summary>
    /// <param name="account">The account to check ownership for.</param>
    /// <returns>True if the list belongs to the account; otherwise, false.</returns>
    public bool BelongsToAccount(Account account)
    {
        return AccountId == account.Id;
    }

    /// <summary>
    /// Marks the list as completed.
    /// </summary>
    public void MarkAsCompleted()
    {
        if (IsCompleted) return;
        IsCompleted = true;
        CompletedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks the list as incomplete.
    /// </summary>
    public void MarkAsIncomplete()
    {
        if (!IsCompleted) return;
        IsCompleted = false;
        CompletedAt = null;
    }

    /// <summary>
    /// Updates the title of the list.
    /// </summary>
    /// <param name="newTitle">The new validated title.</param>
    public void UpdateTitle(Title newTitle)
    {
        Title = newTitle;
    }

    /// <summary>
    /// Updates the description of the list.
    /// </summary>
    /// <param name="newDescription">The new validated description, or null to remove it.</param>
    public void UpdateDescription(Description? newDescription)
    {
        Description = newDescription;
    }

    /// <summary>
    /// Creates and adds a new item to the list.
    /// </summary>
    /// <param name="title">The title of the item.</param>
    /// <param name="dueDate">The optional due date of the item.</param>
    /// <returns>The created To-Do item.</returns>
    public ToDoItem CreateItem(Title title, DueDate? dueDate = null)
    {
        var item = new ToDoItem(title, dueDate);
        _items.Add(item);
        return item;
    }

    /// <summary>
    /// Adds an existing To-Do item to the list.
    /// This method should be used only when transferring items between lists,
    /// not for creating new items within this list.
    /// </summary>
    /// <param name="item">The existing To-Do item to add.</param>
    public void AddItem(ToDoItem item)
    {
        _items.Add(item);
    }

    /// <summary>
    /// Retrieves all items from the list.
    /// </summary>
    /// <returns>A read-only collection of all items in the list.</returns>
    public IReadOnlyCollection<ToDoItem> GetAllItems()
    {
        return _items.Items;
    }

    /// <summary>
    /// Retrieves a specific item from the list by its ID.
    /// </summary>
    public ToDoItem? GetItem(ToDoItemId itemId)
    {
        return _items.FindById(itemId);
    }

    /// <summary>
    /// Updates the title of a specific item in the list.
    /// </summary>
    /// <param name="itemId">The identifier of the item to update.</param>
    /// <param name="newTitle">The new validated title.</param>
    /// <returns>True if updated; otherwise, false.</returns>
    public bool UpdateItemTitle(ToDoItemId itemId, Title newTitle)
    {
        var item = _items.FindById(itemId);
        if (item is null) return false;
        item.UpdateTitle(newTitle);
        return true;
    }

    /// <summary>
    /// Marks a specific item as completed.
    /// </summary>
    /// <param name="itemId">The identifier of the item to mark.</param>
    /// <returns>True if marked; otherwise, false.</returns>
    public bool MarkItemAsCompleted(ToDoItemId itemId)
    {
        var item = _items.FindById(itemId);
        if (item is null) return false;
        item.MarkAsCompleted();
        return true;
    }

    /// <summary>
    /// Marks a specific item as incomplete.
    /// </summary>
    /// <param name="itemId">The identifier of the item to mark.</param>
    /// <returns>True if marked; otherwise, false.</returns>
    public bool MarkItemAsIncomplete(ToDoItemId itemId)
    {
        var item = _items.FindById(itemId);
        if (item is null) return false;
        item.MarkAsIncomplete();
        return true;
    }

    /// <summary>
    /// Removes a specific item from the list.
    /// </summary>
    /// <param name="itemId">The identifier of the item to remove.</param>
    /// <returns>True if removed; otherwise, false.</returns>
    public bool DeleteItem(ToDoItemId itemId)
    {
        var item = _items.FindById(itemId);
        if (item is null) return false;
        return _items.Remove(item);
    }
}
