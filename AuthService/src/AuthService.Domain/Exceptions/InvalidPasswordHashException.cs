using AuthService.Domain.Exceptions;

public sealed class InvalidPasswordHashException : DomainException
{
    public InvalidPasswordHashException()
        : base("Hash de senha inválido.") { }
}
