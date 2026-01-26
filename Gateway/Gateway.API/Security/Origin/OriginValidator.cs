using Microsoft.Extensions.Options;

namespace Gateway.Api.Security.Origin;

public class OriginValidator : IOriginValidator
{
    private readonly HashSet<string> _allowed;

    public OriginValidator(IOptions<OriginValidationOptions> options)
    {
        _allowed = options
            .Value.AllowedOrigins.Where(o => !string.IsNullOrWhiteSpace(o))
            .Select(o => o.Trim().ToLowerInvariant())
            .ToHashSet();
    }

    public bool IsAllowed(string? origin) =>
        origin != null && _allowed.Contains(origin.ToLowerInvariant());
}
