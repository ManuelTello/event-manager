using Events.Domain.Abstractions;
using Events.Domain.AggregateRoots;
using Events.Domain.DomainError;
using FluentResults;

namespace Events.Application.UseCases;

public class EventTicketPostUseCase
{
    private readonly IEventRepository _repository;

    public EventTicketPostUseCase(IEventRepository repository)
    {
        this._repository = repository;
    }

    public async Task<Result<string>> Execute(Guid id, string email)
    {
        string token = Guid.NewGuid().ToString();
        var @event = await this._repository.SearchEventById(id);
        if (@event is null)
            return Result.Fail<string>(EventErrors.EventNotFound);

        var postTicket = @event.PostTicket(email, token);
        if (postTicket.IsSuccess)
        {
            await this._repository.SaveEventTicket(id, @event.Tickets.Last());
            await this._repository.UpdateEventTicketCount(id, @event.TicketsAmount);

            if (@event.Status is EventStatus.SoldOut)
                await this._repository.SetEventStatus(id, EventStatus.SoldOut);

            return Result.Ok<string>(token);
        }
        else
        {
            return Result.Fail<string>(postTicket.Errors);
        }
    }
}