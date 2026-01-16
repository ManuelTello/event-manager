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
                             create type event_status as enum('drafted','published','cancelled','postponed','completed','soldout');
                             create type ticket_status as enum('toconfirm','confirmed','expired','cancelled');
                             create type location_type as enum('virtual','inperson','hybrid');
                             
                             create table events(
                                 id uuid,
                                 name varchar(250) not null,
                                 description text not null,
                                 start_date timestamp not null,
                                 end_date timestamp not null,
                                 inscription_start timestamp,
                                 inscription_end timestamp,
                                 organization varchar(250) not null,
                                 status event_status,
                                 tickets integer,
                                 location_type location_type,
                                 url varchar(250),
                                 token varchar(36) not null,
                                 coordinates varchar(250),
                                 primary key(id)
                             );
                             
                             create table tickets(
                                 id uuid,
                                 status ticket_status,
                                 emitted timestamp,
                                 email varchar(250) not null,
                                 token varchar(36) not null,
                                 event_id uuid,
                                 primary key(id),
                                 constraint fk_event
                                     foreign key (event_id)
                                         references events(id)
                                             on delete cascade
                             );
                             """;
        
        await this._dataSource.OpenConnectionAsync();
        await using var cmd = this._dataSource.CreateCommand(query);
        await cmd.ExecuteNonQueryAsync();
        await this._dataSource.ReloadTypesAsync();
    }
}