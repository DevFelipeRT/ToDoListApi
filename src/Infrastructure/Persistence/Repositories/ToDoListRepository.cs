using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Domain.Lists;
using Domain.Lists.Repositories;
using Domain.Lists.ValueObjects;
using Domain.Accounts.ValueObjects;

namespace Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for the <see cref="ToDoList"/> aggregate using Entity Framework Core.
/// Encapsulates data-access concerns to keep the domain model persistence-ignorant.
/// </summary>
public sealed class ToDoListRepository : IToDoListRepository
{
    private readonly ApplicationDbContext _context;

    public readonly record struct ItemWithListId(ToDoItem Item, ToDoListId ListId);

    /// <summary>
    /// Initializes a new instance of <see cref="ToDoListRepository"/>.
    /// </summary>
    /// <param name="context">The EF Core database context.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="context"/> is null.</exception>
    public ToDoListRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc />
    public async Task AddAsync(ToDoList list, CancellationToken cancellationToken)
    {
        await _context.ToDoLists.AddAsync(list, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<ToDoList>> GetAllByUserAsync(AccountId userId, CancellationToken cancellationToken)
    {
        // Read-only query: AsNoTracking is appropriate here.
        return await _context.ToDoLists
            .AsNoTracking()
            .Include(l => l.ItemsForEfCore)
            .Where(x => x.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<ToDoList?> GetByIdAsync(ToDoListId id, CancellationToken cancellationToken)
    {
        // Tracking query (useful when the caller intends to modify the entity).
        return await _context.ToDoLists
            .Include(l => l.ItemsForEfCore)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task UpdateAsync(ToDoList list, CancellationToken cancellationToken)
    {
        // Load the existing aggregate (including items) to enable change tracking.
        var existing = await _context.ToDoLists
            .Include(l => l.ItemsForEfCore) // internal collection mapped for EF only
            .FirstOrDefaultAsync(x => x.Id == list.Id, cancellationToken);

        if (existing is null)
        {
            _context.ToDoLists.Add(list);
        }
        else
        {
            // Update root values. Item synchronization should be handled by the aggregate/config where possible.
            _context.Entry(existing).CurrentValues.SetValues(list);
            // If you maintain items externally, attach/sync them here as needed.
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(ToDoListId id, CancellationToken cancellationToken)
    {
        var entity = await _context.ToDoLists.FindAsync(new object[] { id }, cancellationToken);
        if (entity is not null)
        {
            _context.ToDoLists.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    /// <inheritdoc />
    public async Task<ToDoItem?> GetItemByIdAsync(
        ToDoListId listId,
        ToDoItemId itemId,
        AccountId userId,
        CancellationToken cancellationToken)
    {
        // Direct, read-only lookup for a single item, ensuring list ownership by user.
        return await _context.ToDoItems
            .AsNoTracking()
            .Where(i => i.Id == itemId && EF.Property<ToDoListId>(i, "ListId") == listId)
            .Where(_ => _context.ToDoLists.Any(l => l.Id == listId && l.UserId == userId))
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<(ToDoItem Item, ToDoListId ListId)>> GetItemsWithDueDateAndListIdAsync(
        AccountId userId,
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken cancellationToken)
    {
        var query =
            from item in _context.ToDoItems.AsNoTracking()
            join list in _context.ToDoLists.AsNoTracking()
                on EF.Property<Guid>(item, "ListId") equals list.Id.Value
            where list.UserId == userId && item.DueDate != null
            select new { item, listId = list.Id };

        if (fromDate.HasValue)
        {
            query = query.Where(x => x.item.DueDate!.Value >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(x => x.item.DueDate!.Value <= toDate.Value);
        }

        var result = await query
            .OrderBy(x => x.item.DueDate!.Value)
            .ToListAsync(cancellationToken);

        return result
            .Select(x => (x.item, x.listId))
            .ToList();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<ToDoItem>> GetAllItemsByListIdAndUserAsync(
        ToDoListId listId,
        AccountId userId,
        CancellationToken cancellationToken)
    {
        // First, ensure the list exists and belongs to the user.
        var exists = await _context.ToDoLists
            .AnyAsync(l => l.Id == listId && l.UserId == userId, cancellationToken);

        if (!exists)
            return new List<ToDoItem>();

        // Then, fetch items by the shadow FK.
        return await _context.ToDoItems
            .AsNoTracking()
            .Where(i => EF.Property<ToDoListId>(i, "ListId") == listId)
            .ToListAsync(cancellationToken);
    }
}
