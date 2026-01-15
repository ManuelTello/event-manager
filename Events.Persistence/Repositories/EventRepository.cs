using Events.Domain.Abstractions;
using Events.Domain.AggregateRoots;
using Events.Domain.Entities;
using Events.Domain.Enums;
using Events.Persistence.Abstractions;
using Npgsql;

namespace Events.Persistence.Repositories;

public class EventRepository : IEventRepository
{
    private readonly NpgsqlDataSource _source;

    public EventRepository(IDatabaseProvider databaseProvider)
    {
        this._source = databaseProvider.GetDataSource();
    }

    public async Task SaveEvent(Event @event)
    {
        const string query = """
            INSERT INTO 
                events(id,name,description,date_start,date_end,inscription_start,inscription_end,organization,tickets,status,location_type,url,coordinates,token)
            VALUES
                (@id,@name,@description,@dateStart,@dateEnd,@inscriptionStart,@inscriptionEnd,@organization,@tickets,@status,@locationType,@url,@coordinates,@token);
        """;

        await this._source.OpenConnectionAsync();
        await using var cmd = this._source.CreateCommand(query);
        cmd.Parameters.AddWithValue("id", @event.Id);
        cmd.Parameters.AddWithValue("name", @event.Name);
        cmd.Parameters.AddWithValue("description", @event.Description);
        cmd.Parameters.AddWithValue("dateStart", @event.StartDate);
        cmd.Parameters.AddWithValue("dateEnd", @event.EndDate);
        cmd.Parameters.AddWithValue("inscriptionStart", @event.InscriptionStartDate);
        cmd.Parameters.AddWithValue("inscriptionEnd", @event.InscriptionEndDate);
        cmd.Parameters.AddWithValue("organization", @event.Organization);
        cmd.Parameters.AddWithValue("tickets", @event.TicketsAmount);
        cmd.Parameters.AddWithValue("status", @event.Status);
        cmd.Parameters.AddWithValue("locationType", @event.LocationType);
        cmd.Parameters.AddWithValue("url", @event.LocationUrl ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("coordinates", @event.GeoCoordinates ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("token", @event.Token);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task SaveEventTicket(Guid eventId, Ticket ticket)
    {
        const string query = """
            INSERT INTO 
                tickets(id,status,emitted,email,token,event_id)
            VALUES
                (@id,@status,@emitted,@email,@token,@eventId);
        """;

        await this._source.OpenConnectionAsync();
        await using var cmd = this._source.CreateCommand(query);
        cmd.Parameters.AddWithValue("id", ticket.Id);
        cmd.Parameters.AddWithValue("status", ticket.Status);
        cmd.Parameters.AddWithValue("emitted", ticket.Emitted);
        cmd.Parameters.AddWithValue("email", ticket.Email);
        cmd.Parameters.AddWithValue("token", ticket.Token);
        cmd.Parameters.AddWithValue("eventId", eventId);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task SetEventStatus(Guid id, EventStatus status)
    {
        const string query = """
            UPDATE events SET status = @status WHERE status.id = @id;
        """;

        await this._source.OpenConnectionAsync();
        await using var cmd = this._source.CreateCommand(query);
        cmd.Parameters.AddWithValue("status", status);
        cmd.Parameters.AddWithValue("id", id);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task<Event?> SearchEventById(Guid eventId)
    {
        Event? @event = null;
        IList<Ticket> tickets = [];
        const string query = """
            SELECT
                e.id, e.name, e.description, e.start_date, e.end_date, e.inscription_start, e.inscription_end, e.organization, e.tickets, e.status, e.location_type, e.url, e.coordinates, e.token
                t.id, t.status, t.email, t.emitted, t.token 
            FROM events e 
            LEFT JOIN tickets t
            ON e.id = t.event_id
            WHERE e.id = @eventId;
        """;

        await this._source.OpenConnectionAsync();
        await using var cmd = this._source.CreateCommand(query);
        cmd.Parameters.AddWithValue("eventId", eventId);
        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            if (@event is null)
                @event = Event.Rehydrate(reader.GetGuid(0), reader.GetString(1), reader.GetString(2), reader.GetDateTime(3), reader.GetDateTime(4), reader.GetDateTime(5), reader.GetDateTime(6), reader.GetString(7), reader.GetInt32(8), reader.GetFieldValue<EventStatus>(9), reader.GetFieldValue<LocationType>(10), reader.GetFieldValue<string?>(11), reader.GetFieldValue<string?>(12), reader.GetString(13), []);

            if (!reader.IsDBNull(13))
            {
                @event._tickets.Add(Ticket.Rehydrate(reader.GetGuid(14), reader.GetFieldValue<TicketStatus>(15), reader.GetString(16), reader.GetDateTime(17), reader.GetString(18)));
            }
        }

        return @event;
    }

    public async Task SetTicketStatus(Guid id, TicketStatus status)
    {
        const string query = """
            UPDATE tickets SET status = @status WHERE tickets.id = @id;
        """;

        await this._source.OpenConnectionAsync();
        await using var cmd = this._source.CreateCommand(query);
        cmd.Parameters.AddWithValue("id", id);
        cmd.Parameters.AddWithValue("status", status);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task UpdateEventTicketCount(Guid id, int amount)
    {
        const string query = """
            UPDATE events SET events.tickets = @amount WHERE events.id = @id;
        """;

        await this._source.OpenConnectionAsync();
        await using var cmd = this._source.CreateCommand(query);
        cmd.Parameters.AddWithValue("id", id);
        cmd.Parameters.AddWithValue("amount", amount);
        await cmd.ExecuteNonQueryAsync();
    }
}