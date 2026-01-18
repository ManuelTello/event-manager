using Events.Domain.Abstractions;
using Events.Domain.AggregateRoots;
using Events.Domain.DomainError;
using FluentResults;

namespace Events.Application.UseCases;

public class EventCancelUseCase
{
    private readonly IEventRepository _repository;

    public EventCancelUseCase(IEventRepository repository)
    {
        this._repository = repository;
    }

    public async Task<Result> Execute(Guid eventId, string token)
    {
        var @event = await this._repository.SearchEventById(eventId);
        if (@event is null)
            return Result.Fail(EventErrors.EventNotFound);

        var cancel = @event.Cancel(token);
        if (cancel.IsSuccess)
        {
            await this._repository.SetEventStatus(eventId, EventStatus.Cancelled);
            return Result.Ok();
        }
        else
        {
            return Result.Fail(cancel.Errors);
        }
    }
}