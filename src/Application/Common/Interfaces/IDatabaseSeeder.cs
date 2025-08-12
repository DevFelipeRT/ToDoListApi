namespace Application.Common.Interfaces;

/// <summary>
/// Interface for database seeding operations.
/// This interface defines a contract for classes that will seed the database with initial data.
/// Implementations of this interface should provide the logic to populate the database with necessary data.
/// </summary>
public interface IDatabaseSeeder
{
    Task MigrateAsync();
    Task SeedAsync();
}