using Events.Domain.DomainError;
using FluentResults;

namespace Events.Domain.Entities;

public sealed class Ticket
{
    public Guid Id { get; init; }

    public TicketStatus Status { get; private set; }

    public string Email { get; private set; }

    public string Token { get; private set; }

    public DateTime Emitted { get; private set; }

    internal static Ticket Rehydrate(Guid id, TicketStatus status, string email, DateTime emitted, string token)
    {
        Ticket ticket = new Ticket(id,status,email, emitted, token);
        return ticket;
    }

    private Ticket(Guid id, TicketStatus status, string email, DateTime emitted, string token)
    {
        this.Id = id;
        this.Status = status;
        this.Emitted = emitted;
        this.Email = email;
        this.Token = token;
    }

    public static Result<Ticket> Create(string email, DateTime inscriptionStart, DateTime inscriptionEnd, string token)
    {
        IList<IError> errors = [];
        DateTime date = DateTime.Now;

        if (string.IsNullOrEmpty(email) || string.IsNullOrWhiteSpace(email))
            errors.Add(EventErrors.TicketRequiredEmail);

        if (!(date.CompareTo(inscriptionStart) >= 0 && date.CompareTo(inscriptionEnd) <= 0))
            errors.Add(EventErrors.TicketInscriptionPeriodEnded);

        if (errors.Count != 0)
            return Result.Fail<Ticket>(errors);

        Ticket ticket = new Ticket(Guid.NewGuid(), TicketStatus.ToConfirm, email, date, token);
        return Result.Ok<Ticket>(ticket);
    }

    public Result Confirm(string token)
    {
        if (this.Status is not TicketStatus.ToConfirm)
            return Result.Fail(EventErrors.TicketMustBeToBeConfirmed);

        if (token != this.Token)
            return Result.Fail(EventErrors.TicketInvalidToken);

        this.Status = TicketStatus.Confirmed;
        return Result.Ok();
    }

    public Result Cancel(string token)
    {
        if (this.Status is not TicketStatus.ToConfirm || this.Status is not TicketStatus.Confirmed)
            return Result.Fail(EventErrors.TicketMustToBeConfirmedOrConfirmed);

        if (token != this.Token)
            return Result.Fail(EventErrors.TicketInvalidToken);

        this.Status = TicketStatus.Cancelled;
        return Result.Ok();
    }
}