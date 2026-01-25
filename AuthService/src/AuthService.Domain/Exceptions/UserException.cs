namespace AuthService.Domain.Exceptions;

public class UserException : DomainException
{
    public UserException(string message)
        : base(message) { }
}
