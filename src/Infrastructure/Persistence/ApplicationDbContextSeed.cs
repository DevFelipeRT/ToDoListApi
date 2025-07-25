using ToDoList.Domain.Entities;
using ToDoList.Domain.ValueObjects;

namespace ToDoList.Infrastructure.Persistence;

public static class ApplicationDbContextSeed
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (!context.ToDoItems.Any())
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
            context.ToDoItems.AddRange(items);
            await context.SaveChangesAsync();
        }
    }
} 