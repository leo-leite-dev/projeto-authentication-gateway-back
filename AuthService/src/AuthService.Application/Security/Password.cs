namespace AuthService.Application.Security;

public sealed class Password
{
    public string Value { get; }

    public Password(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ApplicationException("Senha é obrigatória.");

        if (value.Length < 8)
            throw new ApplicationException("Senha deve ter no mínimo 8 caracteres.");

        Value = value;
    }
}
