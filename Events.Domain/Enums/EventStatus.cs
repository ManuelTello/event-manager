namespace Events.Domain.AggregateRoots;

public enum EventStatus
{
    Drafted,
    Published,
    Cancelled,
    Postponed,
    Completed,
    SoldOut
}
