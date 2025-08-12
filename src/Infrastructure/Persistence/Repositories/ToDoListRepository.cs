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

namespace Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Repository implementation for the <see cref="ToDoList"/> aggregate using Entity Framework Core.
    /// Encapsulates data-access concerns to keep the domain model persistence-ignorant.
    /// </summary>
    public sealed class ToDoListRepository : IToDoListRepository
    {
        private readonly ApplicationDbContext _context;

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
        public async Task<IReadOnlyCollection<ToDoItem>> GetItemsWithDueDateAsync(
            AccountId userId,
            DateTime? fromDate,
            DateTime? toDate,
            CancellationToken cancellationToken)
        {
            // Avoid using domain-calculated properties (e.g., HasDueDate) inside EF queries.
            // Filter by nullability and compare the VO's underlying value.
            var query = _context.ToDoItems
                .AsNoTracking()
                // ensure the item belongs to a list owned by the user
                .Where(i => _context.ToDoLists.Any(l => l.UserId == userId && l.Id == EF.Property<ToDoListId>(i, "ListId")))
                // due date must exist
                .Where(i => i.DueDate != null);

            if (fromDate.HasValue)
                query = query.Where(i => i.DueDate != null && i.DueDate!.Value >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(i => i.DueDate != null && i.DueDate!.Value <= toDate.Value);

            return await query
                .OrderBy(i => i.DueDate!.Value)
                .ToListAsync(cancellationToken);
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
}
