using Events.Domain.Abstractions;
using Events.Domain.DomainError;
using Events.Domain.Entities;
using FluentResults;

namespace Events.Application.UseCases;

public class EventTicketConfirmUseCase
{
    private readonly IEventRepository _repository;

    public EventTicketConfirmUseCase(IEventRepository repository)
    {
        this._repository = repository;
    }

    public async Task<Result> Execute(Guid eventId, Guid ticketId, string token)
    {
        var @event = await this._repository.SearchEventById(eventId);
        if (@event is null)
            return Result.Fail(EventErrors.EventNotFound);

        var confirm = @event.ConfirmTicket(ticketId, token);
        if (confirm.IsFailed)
            return Result.Fail(confirm.Errors);

        await this._repository.SetTicketStatus(ticketId, TicketStatus.Confirmed);
        return Result.Ok();
    }
}