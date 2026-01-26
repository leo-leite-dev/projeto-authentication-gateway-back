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

builder
    .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var keysPath = Path.Combine(AppContext.BaseDirectory, "Keys");

        if (!Directory.Exists(keysPath))
            throw new InvalidOperationException($"Pasta de chaves não encontrada: {keysPath}");

        var publicKeys = new List<SecurityKey>();

        foreach (var file in Directory.GetFiles(keysPath, "*-public.pem"))
        {
            var rsa = RSA.Create();
            rsa.ImportFromPem(File.ReadAllText(file));

            var keyId = Path.GetFileNameWithoutExtension(file);

            publicKeys.Add(new RsaSecurityKey(rsa) { KeyId = keyId });
        }

        if (!publicKeys.Any())
            throw new InvalidOperationException("Nenhuma chave pública encontrada no Gateway");

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "auth-service",

            ValidateAudience = true,
            ValidAudience = "api",

            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            IssuerSigningKeyResolver = (token, securityToken, kid, parameters) => publicKeys,

            ClockSkew = TimeSpan.Zero,
            ValidAlgorithms = new[] { SecurityAlgorithms.RsaSha256 },
        };
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<SecurityHeadersMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();

app.UseCors(CorsPolicyExtensions.DefaultPolicy);

app.UseMiddleware<OriginValidationMiddleware>();
app.UseMiddleware<CsrfProtectionMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.Use(
    async (ctx, next) =>
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        await next();
        sw.Stop();

        Console.WriteLine(
            $"[Gateway] {ctx.Request.Method} {ctx.Request.Path} => {ctx.Response.StatusCode} ({sw.ElapsedMilliseconds}ms)"
        );
    }
);

app.MapControllers();
app.MapGet("/health", () => Results.Ok("Gateway OK"));

app.Run();
