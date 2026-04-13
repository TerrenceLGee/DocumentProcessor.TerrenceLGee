using DocumentProcessor.Avalonia.TerrenceLGee.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace DocumentProcessor.IntegrationTests.TerrenceLGee.Setup;

public class DatabaseFixture : IAsyncLifetime
{
    private readonly SqliteConnection _connection = new SqliteConnection("Data Source=:memory:");

    public ContactDbContext CreateContext()
    {
        return new ContactDbContext(new DbContextOptionsBuilder<ContactDbContext>()
            .UseSqlite(_connection)
            .Options);
    }

    public async Task InitializeAsync()
    {
        await _connection.OpenAsync();

        using (var context = CreateContext())
        {
            await context.Database.EnsureCreatedAsync();
            await Setup.DatabaseSeeder.SeedDatabaseAsync(context);
        }
    }

    public async Task DisposeAsync()
    {
        await _connection.DisposeAsync();
    }
}
