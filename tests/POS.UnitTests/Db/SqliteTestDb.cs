using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using POS.Infrastructure.Data;

namespace POS.UnitTests.Db;

public sealed class SqliteTestDb : IAsyncDisposable
{
    private readonly SqliteConnection _connection;

    public ApplicationDbContext Db { get; }

    public SqliteTestDb()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(_connection)
            .EnableSensitiveDataLogging()
            .Options;

        Db = new ApplicationDbContext(options);
        Db.Database.EnsureCreated();
    }

    public async ValueTask DisposeAsync()
    {
        await Db.DisposeAsync();
        await _connection.DisposeAsync();
    }
}

