using AuthService.API.Security;
using AuthService.Application.Abstractions.Security;
using AuthService.Infrastructure.Security.Jwt;

var builder = WebApplication.CreateBuilder(args);

Console.WriteLine($"[ENV] ASPNETCORE_ENVIRONMENT = {builder.Environment.EnvironmentName}");
Console.WriteLine($"[ENV] IsDevelopment = {builder.Environment.IsDevelopment()}");
Console.WriteLine($"[ENV] IsProduction  = {builder.Environment.IsProduction()}");

builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalAntiforgeryFilter>();
});

builder.Services.Configure<SecurityOptions>(builder.Configuration.GetSection("Security"));

builder.Services.AddSingleton<JwtKeyProvider>();
builder.Services.AddSingleton<IJwtKeyProvider>(sp => sp.GetRequiredService<JwtKeyProvider>());

builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddCustomAntiforgery(builder.Environment);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseHsts();

app.UseCookiePolicy(
    new CookiePolicyOptions
    {
        MinimumSameSitePolicy = SameSiteMode.Lax,
        Secure = builder.Environment.IsDevelopment()
            ? CookieSecurePolicy.None
            : CookieSecurePolicy.Always,
    }
);

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.MapGet("/health", () => Results.Ok("AuthService OK"));

app.Run();
