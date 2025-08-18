using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Application.Common.Interfaces;
using Domain.Lists.Entities;
using Domain.Lists.ValueObjects;

namespace Infrastructure.Persistence;

/// <summary>
/// Seed data for the application database context.
/// </summary>
public class ApplicationDbContextSeed : IDatabaseSeeder
{
    private readonly ApplicationDbContext _context;

    public ApplicationDbContextSeed(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task MigrateAsync()
    {
        await _context.Database.MigrateAsync();
    }

    public async Task SeedAsync()
    {
        if (!_context.ToDoItems.Any())
        {
            var items = new List<ToDoItem>
            {
                new(new Title("Learn .NET Clean Architecture")),
                new(new Title("Read DDD book")),
                new(new Title("Refactor ToDo API")),
                new(new Title("Write unit tests")),
                new(new Title("Implement CQRS")),
                new(new Title("Try MediatR")),
                new(new Title("Explore EF Core")),
                new(new Title("Add Swagger docs")),
                new(new Title("Test PATCH endpoint")),
                new(new Title("Deploy to Azure"))
            };
            
            _context.ToDoItems.AddRange(items);
            await _context.SaveChangesAsync();
        }
    }
} 