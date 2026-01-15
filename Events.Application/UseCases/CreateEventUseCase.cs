using Events.Domain.Abstractions;
using Events.Domain.AggregateRoots;
using Events.Domain.Enums;
using FluentResults;

namespace Events.Application.UseCases;

public class CreateEventUseCase
{
    private readonly IEventRepository _repository;
    
    public CreateEventUseCase(IEventRepository repository)
    {
        this._repository = repository;
    }
    
    public async Task<Result<Guid>> Execute(string name, string description, DateTime startDate, DateTime endDate, DateTime inscriptionStart, DateTime inscriptionEnd, string organization, int ticketsAmount,int locationType, string? url, string? coordinates)
    {
        var create = Event.Create(Guid.NewGuid(), name, description, startDate, endDate, inscriptionStart, inscriptionEnd, organization, ticketsAmount, (LocationType)locationType, url,
            coordinates, Guid.NewGuid().ToString());

        if (create.IsFailed)
        {
            return Result.Fail<Guid>(create.Errors);
        }
        else
        {
            await this._repository.SaveEvent(create.Value);
            return Result.Ok<Guid>(create.Value.Id);
        }
    }
}