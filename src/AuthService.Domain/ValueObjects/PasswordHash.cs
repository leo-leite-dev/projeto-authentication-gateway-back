namespace AuthService.Domain.ValueObjects;

public sealed class PasswordHash
{
    public string Value { get; }

    public PasswordHash(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidPasswordHashException();

        Value = value;
    }

    public override string ToString() => Value;

    public override bool Equals(object? obj) => obj is PasswordHash other && Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();

    public static bool operator ==(PasswordHash? left, PasswordHash? right) => Equals(left, right);

    public static bool operator !=(PasswordHash? left, PasswordHash? right) => !Equals(left, right);

    public static implicit operator string(PasswordHash hash) => hash.Value;
}
