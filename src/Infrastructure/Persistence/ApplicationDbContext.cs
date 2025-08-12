using System.Reflection;
using Domain.Lists;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

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
    /// Gets or sets the DbSet for ToDoList entities. This represents the ToDoLists table in the database.
    /// </summary>
    public DbSet<ToDoList> ToDoLists { get; set; }

    /// <summary>
    /// Gets or sets the DbSet for User entities. This represents the Users table in the database.
    /// </summary>
    public DbSet<Domain.Accounts.User> Users { get; set; }

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