using System.Text.RegularExpressions;
using AuthService.Domain.Exceptions;

namespace BaitaHora.Domain.Features.Common.ValueObjects;

public readonly record struct Username
{
    public string Value { get; }

    private Username(string value) => Value = value;

    public static readonly Username Empty = new Username("unknown");

    private static readonly Regex Pattern = new(
        @"^(?=.{3,20}$)(?=.*[a-z])(?![._])(?!.*[._]{2})[a-z0-9._]+(?<![._])$",
        RegexOptions.Compiled
    );

    private static readonly HashSet<string> Reserved = new(StringComparer.OrdinalIgnoreCase)
    {
        "admin",
        "administrator",
        "root",
        "owner",
        "system",
        "support",
        "help",
        "login",
        "logout",
        "signin",
        "signup",
        "register",
        "settings",
        "config",
        "profile",
        "me",
        "about",
        "contact",
        "security",
        "api",
        "www",
        "null",
    };

    public static Username Parse(string input)
    {
        if (input is null)
            throw new UserException("Username obrigatório.");

        var norm = input.Trim().ToLowerInvariant();

        if (Reserved.Contains(norm))
            throw new UserException("Username reservado.");

        if (!Pattern.IsMatch(norm))
            throw new UserException(
                "Username inválido. Use 3–20 caracteres (letras, números, ponto ou underline); "
                    + "não comece/termine com . ou _; sem repetição de separadores."
            );

        return new Username(norm);
    }

    public static bool TryParse(string? input, out Username result)
    {
        result = default;
        if (string.IsNullOrWhiteSpace(input))
            return false;

        var norm = input.Trim().ToLowerInvariant();

        if (Reserved.Contains(norm))
            return false;

        if (!Pattern.IsMatch(norm))
            return false;

        result = new Username(norm);
        return true;
    }

    public override string ToString() => Value;

    public static implicit operator string(Username u) => u.Value;
}
