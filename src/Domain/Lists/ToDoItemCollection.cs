using System.Collections.Generic;
using System.Linq;
using Domain.Lists.ValueObjects;

namespace Domain.Lists;

/// <summary>
/// Represents a collection of To-Do items within a To-Do List aggregate.
/// All business rules related to the collection are encapsulated here.
/// </summary>
public class ToDoItemCollection
{
    /// <summary>
    /// Underlying list of To-Do items.
    /// This field is accessed by the infrastructure layer (EF Core) via an internal accessor.
    /// </summary>
    private readonly List<ToDoItem> _items = new();

    /// <summary>
    /// Internal accessor for the underlying list.
    /// Used exclusively by the persistence layer to allow EF Core to map the collection.
    /// </summary>
    internal List<ToDoItem> InternalList => _items;

    /// <summary>
    /// Gets a read-only view of the To-Do items.
    /// </summary>
    public IReadOnlyCollection<ToDoItem> Items => _items.AsReadOnly();

    /// <summary>
    /// Adds a new To-Do item to the collection.
    /// Throws an exception if an item with the same Id already exists.
    /// </summary>
    /// <param name="item">The To-Do item to add.</param>
    public void Add(ToDoItem item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));
        if (_items.Exists(i => i.Id == item.Id))
            throw new InvalidOperationException("An item with the same ID already exists in the list.");
        _items.Add(item);
    }

    /// <summary>
    /// Checks whether an item with the given identifier exists in the collection.
    /// </summary>
    /// <param name="id">The identifier of the To-Do item to check.</param>
    /// <returns>True if the item exists; otherwise, false.</returns>
    public bool Exists(ToDoItemId id)
    {
        return _items.Any(item => item.Id == id);
    }

    /// <summary>
    /// Removes a To-Do item from the collection.
    /// Returns true if the item was removed; otherwise, false.
    /// </summary>
    /// <param name="item">The To-Do item to remove.</param>
    public bool Remove(ToDoItem item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));
        return _items.Remove(item);
    }

    /// <summary>
    /// Finds a To-Do item by Id.
    /// </summary>
    /// <param name="itemId">The Id of the item.</param>
    /// <returns>The To-Do item if found; otherwise, null.</returns>
    public ToDoItem? FindById(ToDoItemId itemId)
    {
        if (itemId == null)
            throw new ArgumentNullException(nameof(itemId));
        return _items.FirstOrDefault(i => i.Id == itemId);
    }

    /// <summary>
    /// Returns the number of To-Do items in the collection.
    /// </summary>
    public int Count => _items.Count;
}
