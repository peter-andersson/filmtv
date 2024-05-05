using System.Reflection;
using System.Security.Claims;
using DbUp;
using FilmTV.Api;
using FilmTV.Api.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddOpenTelemetry(logging =>
{
    logging.IncludeFormattedMessage = true;
    logging.IncludeScopes = true;
});

builder.Services.AddOpenTelemetry()
    .WithMetrics(metrics =>
    {
        metrics.AddRuntimeInstrumentation()
            .AddMeter("Microsoft.AspNetCore.Hosting", "Microsoft.AspNetCore.Server.Kestrel", "System.Net.Http");
    })
    .WithTracing(tracing =>
    {
        if (builder.Environment.IsDevelopment())
        {
            tracing.SetSampler(new AlwaysOnSampler());
        }

        tracing.SetErrorStatusOnException();

        tracing.AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddEntityFrameworkCoreInstrumentation();
    });

// ReSharper disable once StringLiteralTypo
// ReSharper disable once IdentifierTypo
var useOtlpExporter = !string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);
if (useOtlpExporter)
{
    builder.Services.Configure<OpenTelemetryLoggerOptions>(logging => logging.AddOtlpExporter());
    builder.Services.ConfigureOpenTelemetryMeterProvider(metrics => metrics.AddOtlpExporter());
    builder.Services.ConfigureOpenTelemetryTracerProvider(tracing => tracing.AddOtlpExporter());
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = $"https://{builder.Configuration["Auth0:Domain"]}/";
        options.Audience = builder.Configuration["Auth0:Audience"];
        options.TokenValidationParameters = new TokenValidationParameters
        {
            NameClaimType = ClaimTypes.NameIdentifier
        };
    });

builder.Services
    .AddAuthorization(options =>
    {
        options.AddPolicy(
            "user",
            policy => policy.Requirements.Add(
                new HasScopeRequirement("user", $"https://{builder.Configuration["Auth0:Domain"]}/")
            )
        );
        
        options.AddPolicy(
            "admin",
            policy => policy.Requirements.Add(
                new HasScopeRequirement("admin", $"https://{builder.Configuration["Auth0:Domain"]}/")
            )
        );        
    });

builder.Services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = """
                      JWT Authorization header using the Bearer scheme. \r\n\r\n
                                            Enter 'Bearer' [space] and then your token in the text input below.
                                            \r\n\r\nExample: 'Bearer 12345'
                      """,
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,

            },
            new List<string>()
        }
    });    
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

// ReSharper disable once StringLiteralTypo
app.MapGet("/weatherforecast", () =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();
        
        return TypedResults.Ok(forecast);
    })
    .WithName("GetWeatherForecast")
    .RequireAuthorization()
    .Produces(StatusCodes.Status401Unauthorized)
    .WithOpenApi();

var logger = app.Services.GetRequiredService<ILogger<DbLogger>>();
var upgradeEngine = DeployChanges.To
    .PostgresqlDatabase(builder.Configuration["ConnectionStrings:DefaultConnection"])
    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
    .LogTo(new DbLogger(logger))
    .Build();

var result = upgradeEngine.PerformUpgrade();
if (!result.Successful)
{
    logger.LogCritical("Database upgrade failed: {Error}", result.Error);
}

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}