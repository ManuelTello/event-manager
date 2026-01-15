using Events.Domain.Abstractions;
using Events.Domain.AggregateRoots;
using Events.Domain.DomainError;
using FluentResults;

namespace Events.Application.UseCases;

public class PublishEventUseCase
{
    private readonly IEventRepository _repository;

    public PublishEventUseCase(IEventRepository repository)
    {
        this._repository = repository;
    }
    
    public async Task<Result> Execute(Guid eventId, string token)
    {
        var @event = await this._repository.SearchEventById(eventId);
        if (@event is null)
            return Result.Fail(EventErrors.EventNotFound);

        var publish = @event.Publish(token);
        if (publish.IsSuccess)
        {
            await this._repository.SetEventStatus(eventId, EventStatus.Published);
            return Result.Ok();
        }
        else
        {
            return Result.Fail(publish.Errors);
        }
    }
}