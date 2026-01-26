namespace Gateway.Api.Security.Cors;

public class CorsPolicyOptions
{
    public string[] AllowedOrigins { get; set; } = [];
    public bool AllowCredentials { get; set; }
}
