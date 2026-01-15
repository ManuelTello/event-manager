using FluentResults;

namespace Events.Domain.DomainError;

public class DomainError : Error
{
    public DomainError(string code, string message) : base(message)
    {
        this.Metadata["code"] = code;
    }
}
