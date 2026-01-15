using System.Runtime.CompilerServices;
using Events.Domain.DomainError;
using Events.Domain.Entities;
using Events.Domain.Enums;
using FluentResults;

[assembly: InternalsVisibleTo("Events.Persistence")]
[assembly: InternalsVisibleTo("Events.Unitary")]
namespace Events.Domain.AggregateRoots;

public sealed class Event
{
    public Guid Id { get; init; }

    public string Name { get; private set; }

    public string Description { get; private set; }

    public DateTime InscriptionStartDate { get; private set; }

    public DateTime InscriptionEndDate { get; private set; }

    public DateTime StartDate { get; private set; }

    public DateTime EndDate { get; private set; }

    public string Organization { get; private set; }

    public EventStatus Status { get; private set; }

    public int TicketsAmount { get; private set; }

    public LocationType LocationType { get; private set; }

    public string? LocationUrl { get; private set; }

    public string? GeoCoordinates { get; private set; }

    public string Token { get; private set; }

    internal IList<Ticket> _tickets;

    public IReadOnlyCollection<Ticket> Tickets => this._tickets.AsReadOnly();

    internal static Event Rehydrate(Guid id, string name, string description, DateTime startDate, DateTime endDate, DateTime inscriptionStart, DateTime inscriptionEnd, string organization, int ticketsAmount, EventStatus status, LocationType locationType, string? locationUrl, string? coordinates, string token, IList<Ticket> tickets)
    {
        Event @event = new Event(id,name, description, startDate, endDate, inscriptionStart, inscriptionEnd, organization, ticketsAmount, status, locationType, locationUrl, coordinates, token, tickets);
        return @event;
    }

    private Event(Guid id, string name, string description, DateTime startDate, DateTime endDate, DateTime inscriptionStart, DateTime inscriptionEnd, string organization, int ticketsAmount, EventStatus status, LocationType locationType, string? locationUrl, string? coordinates, string token, IList<Ticket> tickets)
    {
        this.Id = id;
        this.Name = name;
        this.Description = description;
        this.StartDate = startDate;
        this.EndDate = endDate;
        this.InscriptionStartDate = inscriptionStart;
        this.InscriptionEndDate = inscriptionEnd;
        this.Organization = organization;
        this.TicketsAmount = ticketsAmount;
        this.Status = status;
        this.LocationType = locationType;
        this.LocationUrl = locationUrl;
        this.GeoCoordinates = coordinates;
        this.Token = token;
        this._tickets = tickets;
    }

    public static Result<Event> Create(Guid id, string name, string description, DateTime startDate, DateTime endDate, DateTime inscriptionStart, DateTime inscriptionEnd, string organization, int ticketsAmount, LocationType locationType, string? locationUrl, string? coordinates, string token)
    {
        IList<IError> errors = [];

        if (string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name))
            errors.Add(EventErrors.RequiredName);

        if (startDate.CompareTo(endDate) >= 0 || startDate.CompareTo(DateTime.Now) < 0)
            errors.Add(EventErrors.InvalidStartDate);

        if (inscriptionStart.CompareTo(inscriptionEnd) >= 0 || inscriptionStart.CompareTo(DateTime.Now) < 0 || inscriptionStart.CompareTo(startDate) >= 0)
            errors.Add(EventErrors.InvalidInscriptionDate);

        if (string.IsNullOrEmpty(organization) || string.IsNullOrWhiteSpace(organization))
            errors.Add(EventErrors.RequiredOrganization);

        if (ticketsAmount <= 0)
            errors.Add(EventErrors.InvalidAmountOfTickets);

        if (errors.Any())
            return Result.Fail<Event>(errors);

        Event @event = new Event(id, name, description, startDate, endDate, inscriptionStart, inscriptionEnd, organization, ticketsAmount, EventStatus.Drafted, locationType, locationUrl, coordinates, token, []);
        return Result.Ok<Event>(@event);
    }

    public Result Publish(string token)
    {
        if (token != this.Token)
            return Result.Fail(EventErrors.EventTokenIsInvalid);

        if (this.Status is not EventStatus.Drafted)
            return Result.Fail(EventErrors.EventMustBeDrafted);

        this.Status = EventStatus.Published;
        return Result.Ok();
    }

    public Result Cancel(string token)
    {
        if (token != this.Token)
            return Result.Fail(EventErrors.EventTokenIsInvalid);

        if (this.Status is not EventStatus.Published)
            return Result.Fail(EventErrors.EventMustBePublished);

        this.Status = EventStatus.Cancelled;
        return Result.Ok();
    }

    public Result Postpone(string token)
    {
        if (token != this.Token)
            return Result.Fail(EventErrors.EventTokenIsInvalid);

        if (this.Status is not EventStatus.Published)
            return Result.Fail(EventErrors.EventMustBePublished);

        this.Status = EventStatus.Postponed;
        return Result.Ok();
    }

    public Result PostTicket(string email, string token)
    {
        if (this.Status is not EventStatus.Published)
            return Result.Fail(EventErrors.EventMustBePublished);

        if (this.Status is EventStatus.SoldOut)
            return Result.Fail(EventErrors.EventIsFull);

        Ticket? ticket = this._tickets.SingleOrDefault(t => t.Email == email);
        if (ticket is not null)
            return Result.Fail(EventErrors.TicketEmailAlreadyRequestedForThisEvent);


        Result<Ticket> create = Ticket.Create(email, this.InscriptionStartDate, this.InscriptionEndDate, token);
        if (create.IsFailed)
            return Result.Fail(create.Errors);

        this.TicketsAmount = this.TicketsAmount - 1;
        this._tickets.Add(create.Value);

        if (this.TicketsAmount == 0)
            this.Status = EventStatus.SoldOut;

        return Result.Ok();
    }

    public Result ConfirmTicket(Guid ticketId, string token)
    {
        Ticket? ticket = this._tickets.SingleOrDefault(t => t.Id == ticketId);
        if (ticket is null)
            return Result.Fail(EventErrors.TicketNotFound);

        Result confirm = ticket.Confirm(token);
        if (confirm.IsFailed)
            return Result.Fail(confirm.Errors[0]);

        return Result.Ok();
    }

    public Result CancelTicket(Guid ticketId, string token)
    {
        Ticket? ticket = this._tickets.SingleOrDefault(t => t.Id == ticketId);
        if (ticket is null)
            return Result.Fail(EventErrors.TicketNotFound);

        Result cancel = ticket.Cancel(token);
        if (cancel.IsFailed)
            return Result.Fail(cancel.Errors[0]);

        this.TicketsAmount = this.TicketsAmount + 1;
        return Result.Ok();
    }
}