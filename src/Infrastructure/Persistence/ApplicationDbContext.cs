using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Domain.Lists.Entities;
using Infrastructure.IdentityAccess;
using Domain.Accounts.Entities;

namespace Infrastructure.Persistence
{
    /// <summary>
    /// EF Core database context combining ASP.NET Core Identity stores and application aggregates.
    /// Uses <see cref="Guid"/> as the primary key type for identity entities.
    /// </summary>
    public sealed class ApplicationDbContext
        : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        /// <summary>
        /// Accounts aggregate root set.
        /// </summary>
        public DbSet<Account> Accounts { get; set; } = null!;

        /// <summary>
        /// To-do items aggregate root set.
        /// </summary>
        public DbSet<ToDoItem> ToDoItems { get; set; } = null!;

        /// <summary>
        /// To-do lists aggregate root set.
        /// </summary>
        public DbSet<ToDoList> ToDoLists { get; set; } = null!;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationDbContext"/>.
        /// </summary>
        /// <param name="options">The context configuration options.</param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        /// <summary>
        /// Configures the model for the context.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}

