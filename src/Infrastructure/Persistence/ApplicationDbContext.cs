using System.Reflection;
using Microsoft.EntityFrameworkCore;
using ToDoList.Domain.Entities;

namespace ToDoList.Infrastructure.Persistence;

/// <summary>
/// Represents the application's database context, serving as the main entry point for database operations.
/// </summary>
public class ApplicationDbContext : DbContext
{
    /// <summary>
    /// Gets or sets the DbSet for ToDoItem entities. This represents the ToDoItems table in the database.
    /// </summary>
    public DbSet<ToDoItem> ToDoItems { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationDbContext"/> class.
    /// </summary>
    /// <param name="options">The options to be used by the DbContext.</param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    /// <summary>
    /// Overridden to apply configurations from the current assembly.
    /// </summary>
    /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }
}