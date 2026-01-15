using FluentResults;

namespace Events.Domain.DomainError;

public static class EventErrors
{
    public static readonly IError RequiredName = new DomainError("EVENT_CREATE_NAME_IS_REQUIRED", "Name is required.");

    public static readonly IError InvalidStartDate = new DomainError("EVENT_CREATE_START_IS_INVALID", "The start must be in the future and less than the end date.");

    public static readonly IError InvalidInscriptionDate = new DomainError("EVENT_CREATE_INSCRIPTION_IS_INVALID", "The start date must be before the inscription end, the event start date and from today onwards.");

    public static readonly IError RequiredOrganization = new DomainError("EVENT_CREATE_ORGANIZATION_IS_REQUIRED", "Organization name is required.");

    public static readonly IError InvalidAmountOfTickets = new DomainError("EVENT_CREATE_TICKETS_AMOUNT_IS_INVALID", "Amounts of tickets must be greater than 0");

    public static readonly IError EventIsFull = new DomainError("EVENT_IS_FULL", "The event is full.");

    public static readonly IError EventTokenIsInvalid = new DomainError("EVENT_TOKEN_IS_INVALID", "The given tocket does not match the event tocken.");

    public static readonly IError EventNotFound = new DomainError("EVENT_NOT_FOUND", "The event was not found.");

    public static readonly IError EventMustBePublished = new DomainError("EVENT_MUST_BE_PUBLISHED", "Event must be published.");

    public static readonly IError EventMustBeDrafted = new DomainError("EVENT_MUST_BE_DRAFTED", "Event must be drafted.");

    public static readonly IError TicketRequiredEmail = new DomainError("EVENT_TICKET_CREATE_EMAIL_IS_REQUIRED", "Email is required.");

    public static readonly IError TicketInscriptionPeriodEnded = new DomainError("EVENT_TICKET_CREATE_INSCRIPTION_PERIOD_ENDED", "The period to request your ticket ended.");

    public static readonly IError TicketNotFound = new DomainError("EVENT_TICKET_NOT_FOUND", "The event ticket was not found.");

    public static readonly IError TicketInvalidToken = new DomainError("EVENT_TICKET_TOKEN_IS_INVALID", "The given tocket does not match the ticket token.");

    public static readonly IError TicketMustBeToBeConfirmed = new DomainError("EVENT_TICKET_MUST_BE_TO_CONFIRM", "Event must be to confirm.");

    public static readonly IError TicketMustToBeConfirmedOrConfirmed = new DomainError("EVENT_TICKET_MUST_BE_CONFIRMED", "Event must be confirmed.");

    public static readonly IError TicketEmailAlreadyRequestedForThisEvent = new DomainError("EVENT_TICKET_EMAIL_ALREADY_REQUESTED_IN_EVENT", "You already have a ticket request for this event.");
}
