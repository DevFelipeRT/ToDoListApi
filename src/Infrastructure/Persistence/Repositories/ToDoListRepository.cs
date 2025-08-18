using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Domain.Accounts.ValueObjects;
using Domain.Lists.Entities;
using Domain.Lists.ValueObjects;
using Domain.Lists.Repositories;

namespace Infrastructure.Persistence.Repositories;

/// <summary>
/// Entity Framework Core repository for the <see cref="ToDoList"/> aggregate.
/// Encapsulates data access to keep the domain model persistence-ignorant.
/// </summary>
public sealed class ToDoListRepository : IToDoListRepository
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Helper transport type returning a domain item together with its owning list id.
    /// </summary>
    public readonly record struct ItemWithListId(ToDoItem Item, ToDoListId ListId);

    /// <summary>
    /// Initializes a new <see cref="ToDoListRepository"/>.
    /// </summary>
    /// <param name="context">EF Core database context.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="context"/> is null.</exception>
    public ToDoListRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Persists a new <see cref="ToDoList"/> aggregate.
    /// </summary>
    public async Task AddAsync(ToDoList list, CancellationToken cancellationToken)
    {
        await _context.ToDoLists.AddAsync(list, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Retrieves all lists owned by the given account (read-only).
    /// Includes the EF-only backing collection for items.
    /// </summary>
    public async Task<IReadOnlyCollection<ToDoList>> GetAllByAccountAsync(
        AccountId accountId,
        CancellationToken cancellationToken)
    {
        return await _context.ToDoLists
            .AsNoTracking()
            .Include(l => l.ItemsForEfCore)
            .Where(l => l.AccountId == accountId)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Retrieves only the identifiers of lists owned by the given account (read-only).
    /// </summary>
    public async Task<IReadOnlyList<ToDoListId>> GetIdsByAccountAsync(
        AccountId accountId,
        CancellationToken cancellationToken)
    {
        return await _context.ToDoLists
            .AsNoTracking()
            .Where(l => l.AccountId == accountId)
            .Select(l => l.Id)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Loads a list aggregate by its identifier, including its EF backing items collection (tracked).
    /// </summary>
    public async Task<ToDoList?> GetByIdAsync(ToDoListId id, CancellationToken cancellationToken)
    {
        return await _context.ToDoLists
            .Include(l => l.ItemsForEfCore)
            .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
    }

    /// <summary>
    /// Updates an existing list aggregate (tracked) or adds it when not found.
    /// </summary>
    public async Task UpdateAsync(ToDoList list, CancellationToken cancellationToken)
    {
        var existing = await _context.ToDoLists
            .Include(l => l.ItemsForEfCore)
            .FirstOrDefaultAsync(l => l.Id == list.Id, cancellationToken);

        if (existing is null)
        {
            _context.ToDoLists.Add(list);
        }
        else
        {
            _context.Entry(existing).CurrentValues.SetValues(list);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Deletes a list aggregate by identifier, if it exists.
    /// </summary>
    public async Task DeleteAsync(ToDoListId id, CancellationToken cancellationToken)
    {
        var entity = await _context.ToDoLists.FindAsync(new object[] { id }, cancellationToken);
        if (entity is not null)
        {
            _context.ToDoLists.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    /// <summary>
    /// Retrieves a single item by its identifier, ensuring that it belongs to the given list
    /// and that the list is owned by the given account (read-only).
    /// </summary>
    public async Task<ToDoItem?> GetItemByIdAsync(
        ToDoListId listId,
        ToDoItemId itemId,
        AccountId accountId,
        CancellationToken cancellationToken)
    {
        return await _context.ToDoItems
            .AsNoTracking()
            .Where(i => i.Id == itemId && EF.Property<ToDoListId>(i, "ListId") == listId)
            .Where(_ => _context.ToDoLists.Any(l => l.Id == listId && l.AccountId == accountId))
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Retrieves all non-completed items that have a due date and belong to any of the specified lists.
    /// Optional window filtering (from/to) is applied on the provider type to ensure translation.
    /// The query is read-only and avoids JOINs against DbSets.
    /// </summary>
    public async Task<IReadOnlyCollection<(ToDoItem Item, ToDoListId ListId)>> GetItemsWithDueDateByListIdsAsync(
        IReadOnlyCollection<ToDoListId> listIds,
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken ct)
    {
        if (listIds is null || listIds.Count == 0)
            return Array.Empty<(ToDoItem, ToDoListId)>();

        var listIdsVo = listIds.ToArray();

        var q = _context.ToDoItems
            .AsNoTracking()
            .Where(i => listIdsVo.Contains(EF.Property<ToDoListId>(i, "ListId")))
            .Where(i => EF.Property<DateTime?>(i, "DueDate") != null && !i.IsCompleted);

        if (fromDate.HasValue)
            q = q.Where(i => EF.Property<DateTime?>(i, "DueDate")! >= fromDate.Value);

        if (toDate.HasValue)
            q = q.Where(i => EF.Property<DateTime?>(i, "DueDate")! <= toDate.Value);

        var rows = await q
            .OrderBy(i => EF.Property<DateTime?>(i, "DueDate"))
            .Select(i => new
            {
                Item = i,
                ListId = EF.Property<ToDoListId>(i, "ListId")
            })
            .ToListAsync(ct);

        return rows.Select(r => (r.Item, r.ListId)).ToList();
    }

    /// <summary>
    /// Retrieves all items for a given list owned by the given account (read-only).
    /// Returns an empty collection when the list does not exist or is not owned by the account.
    /// </summary>
    public async Task<IReadOnlyCollection<ToDoItem>> GetAllItemsByListIdAndAccountAsync(
        ToDoListId listId,
        AccountId accountId,
        CancellationToken cancellationToken)
    {
        var exists = await _context.ToDoLists
            .AnyAsync(l => l.Id == listId && l.AccountId == accountId, cancellationToken);

        if (!exists)
            return Array.Empty<ToDoItem>();

        return await _context.ToDoItems
            .AsNoTracking()
            .Where(i => EF.Property<ToDoListId>(i, "ListId") == listId)
            .ToListAsync(cancellationToken);
    }
}
