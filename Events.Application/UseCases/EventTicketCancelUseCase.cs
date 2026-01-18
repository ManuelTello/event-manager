using Events.Domain.Abstractions;
using Events.Domain.DomainError;
using FluentResults;

namespace Events.Application.UseCases;

public class EventTicketCancelUseCase
{
    private readonly IEventRepository _repository;

    public EventTicketCancelUseCase(IEventRepository repository)
    {
        this._repository = repository;
    }

    public async Task<Result> Execute(Guid eventId, Guid ticketId, string token)
    {
        var @event = await this._repository.SearchEventById(eventId);
        if (@event is null)
            return Result.Fail(EventErrors.EventNotFound);

        var cancel = @event.CancelTicket(ticketId, token);
        if (cancel.IsFailed)
            return Result.Fail(cancel.Errors);

        await this._repository.SetTicketStatus(ticketId, Domain.Entities.TicketStatus.Cancelled);
        await this._repository.UpdateEventTicketCount(eventId, @event.TicketsAmount);

        return Result.Ok();
    }
}
