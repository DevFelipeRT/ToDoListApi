using Microsoft.EntityFrameworkCore;
using ToDoList.Application.Common.Interfaces;
using ToDoList.Domain.Entities;

namespace ToDoList.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implements the repository for ToDoItem, providing concrete data access logic using Entity Framework Core.
/// </summary>
public class ToDoItemRepository : IToDoItemRepository
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="ToDoItemRepository"/> class.
    /// </summary>
    /// <param name="context">The database context to be used for data operations.</param>
    public ToDoItemRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public async Task AddAsync(ToDoItem item, CancellationToken cancellationToken)
    {
        await _context.ToDoItems.AddAsync(item, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<List<ToDoItem>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.ToDoItems
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<ToDoItem?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await _context.ToDoItems
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task UpdateAsync(ToDoItem item, CancellationToken cancellationToken)
    {
        _context.ToDoItems.Update(item);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(ToDoItem item, CancellationToken cancellationToken)
    {
        _context.ToDoItems.Remove(item);
        await _context.SaveChangesAsync(cancellationToken);
    }
}