namespace Gateway.Api.Security.Origin;

public interface IOriginValidator
{
    bool IsAllowed(string? origin);
}
