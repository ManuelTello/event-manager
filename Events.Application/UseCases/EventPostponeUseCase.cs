using Events.Domain.Abstractions;
using Events.Domain.AggregateRoots;
using Events.Domain.DomainError;
using FluentResults;

namespace Events.Application.UseCases;

public class EventPostponeUseCase
{
    private readonly IEventRepository _repository;

    public EventPostponeUseCase(IEventRepository repository)
    {
        this._repository = repository;
    }

    public async Task<Result> Execute(Guid eventId, string token)
    {
        var @event = await this._repository.SearchEventById(eventId);
        if (@event is null)
            return Result.Fail(EventErrors.EventNotFound);

        var postpone = @event.Postpone(token);
        if (postpone.IsSuccess)
        {
            await this._repository.SetEventStatus(eventId, EventStatus.Postponed);
            return Result.Ok();
        }
        else
        {
            return Result.Fail(postpone.Errors);
        }
    }
}