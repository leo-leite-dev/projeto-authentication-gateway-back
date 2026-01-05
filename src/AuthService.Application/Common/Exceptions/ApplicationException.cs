using AuthService.Domain.Exceptions;

namespace AuthService.Application.Common.Exceptions;

public sealed class ValidationException : DomainException
{
    public ValidationException(string message)
        : base(message) { }
}
