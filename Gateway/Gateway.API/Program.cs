using System.Security.Cryptography;
using Gateway.API.Forwarding;
using Gateway.Api.Security.Cors;
using Gateway.API.Security.Csrf;
using Gateway.Api.Security.Headers;
using Gateway.Api.Security.Origin;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddStrongCors(builder.Configuration);

builder.Services.Configure<OriginValidationOptions>(
    builder.Configuration.GetSection("OriginValidation")
);
builder.Services.AddSingleton<IOriginValidator, OriginValidator>();

builder.Services.AddHttpClient<GatewayForwarder>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

if (string.IsNullOrWhiteSpace(jwtIssuer) || string.IsNullOrWhiteSpace(jwtAudience))
    throw new InvalidOperationException("Jwt:Issuer or Jwt:Audience not configured");

var keysPath = Path.Combine(builder.Environment.ContentRootPath, "Keys");

if (!Directory.Exists(keysPath))
    throw new DirectoryNotFoundException($"JWT keys folder not found at {keysPath}");

var publicKeys = new List<SecurityKey>();

foreach (var file in Directory.GetFiles(keysPath, "*-public.pem"))
{
    var rsa = RSA.Create();
    rsa.ImportFromPem(File.ReadAllText(file));

    var keyId = Path.GetFileNameWithoutExtension(file);
    publicKeys.Add(new RsaSecurityKey(rsa) { KeyId = keyId });
}

if (!publicKeys.Any())
    throw new InvalidOperationException("No JWT public keys found");

builder
    .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = true;
        options.SaveToken = false;

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if (context.Request.Cookies.TryGetValue("access_token", out var cookieToken))
                {
                    context.Token = cookieToken;
                    context.Request.Headers.Remove("Authorization");
                }
                else
                {
                    var authHeader = context.Request.Headers["Authorization"].ToString();

                    if (
                        !string.IsNullOrEmpty(authHeader)
                        && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                    )
                    {
                        var headerToken = authHeader["Bearer ".Length..].Trim();

                        if (headerToken.Count(c => c == '.') == 2)
                            context.Token = headerToken;
                    }
                }

                return Task.CompletedTask;
            },
        };

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,

            IssuerSigningKeyResolver = (token, securityToken, kid, parameters) => publicKeys,

            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,

            ValidateAudience = true,
            ValidAudience = jwtAudience,

            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30),
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseMiddleware<SecurityHeadersMiddleware>();

app.MapSwagger().AllowAnonymous();
app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();

app.UseCors(CorsPolicyExtensions.DefaultPolicy);

app.UseMiddleware<OriginValidationMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<CsrfProtectionMiddleware>();

app.MapControllers();
app.MapGet("/health", () => Results.Ok("Gateway OK"));

app.Run();
