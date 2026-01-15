using Events.Domain.AggregateRoots;
using Events.Domain.Entities;

namespace Events.Domain.Abstractions;

public interface IEventRepository
{
    public Task SaveEvent(Event @event);

    public Task SaveEventTicket(Guid eventId, Ticket ticket);

    public Task UpdateEventTicketCount(Guid id, int amount);

    public Task SetEventStatus(Guid id, EventStatus status);

    public Task SetTicketStatus(Guid id, TicketStatus status);

    public Task<Event?> SearchEventById(Guid id);
}