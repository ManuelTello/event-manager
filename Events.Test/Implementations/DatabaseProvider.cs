using Events.Domain.AggregateRoots;
using Events.Domain.Entities;
using Events.Domain.Enums;
using Events.Persistence.Abstractions;
using Npgsql;

namespace Events.Test.Implementations;

public class DatabaseProvider:IDatabaseProvider,IAsyncDisposable
{
    private readonly NpgsqlDataSource _dataSource;

    public DatabaseProvider(string connectionString)
    {
        var builder = new NpgsqlDataSourceBuilder();
        builder.MapEnum<EventStatus>("event_status");
        builder.MapEnum<TicketStatus>("ticket_status");
        builder.MapEnum<LocationType>("location_type");
        this._dataSource = builder.Build();
    }
    
    public async ValueTask DisposeAsync()
    {
        await this._dataSource.DisposeAsync();
    }

    public NpgsqlDataSource GetDataSource()
    {
        return this._dataSource;
    }

    public async Task InitializeDatabase()
    {
        const string query = """

                             """;
        
        await this._dataSource.OpenConnectionAsync();
        await using var cmd = this._dataSource.CreateCommand(query);
        await cmd.ExecuteNonQueryAsync();
        await this._dataSource.ReloadTypesAsync();
    }
}