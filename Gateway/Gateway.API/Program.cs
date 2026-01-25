using Gateway.API.Forwarding;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "FrontEnd",
        policy =>
        {
            policy
                .WithOrigins("http://localhost:4200")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        }
    );
});

builder.Services.AddHttpClient<GatewayForwarder>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();

app.UseCors("FrontEnd");

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
