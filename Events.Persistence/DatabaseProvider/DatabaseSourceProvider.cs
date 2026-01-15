using Events.Domain.AggregateRoots;
using Events.Domain.Entities;
using Events.Domain.Enums;
using Events.Persistence.Abstractions;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Events.Persistence.DatabaseProvider;

public class DatabaseSourceProvider : IDatabaseProvider, IAsyncDisposable
{
    private readonly NpgsqlDataSource _dataSource;

    public DatabaseSourceProvider(IConfiguration configuration)
    {
        string? connectionString = configuration.GetConnectionString("PostgresDatabaseConnectionString");
        if (connectionString is null)
            throw new NullReferenceException("connection string not found");

        NpgsqlDataSourceBuilder builder = new NpgsqlDataSourceBuilder(connectionString);
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
}
