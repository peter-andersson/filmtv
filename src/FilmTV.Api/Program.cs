using FilmTV.Api.Common;
using FilmTV.Api.Host;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.AddApplicationTelemetry();

builder.Services.AddAuthorization();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
});

builder.Services
    .AddIdentityApiEndpoints<AppUser>()
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddHttpClient();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = """
                      JWT Authorization header using the Bearer scheme. \
                      Enter 'Bearer' [space] and then your token in the text input below. \
                      Example: 'Bearer 12345'
                      """,
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new List<string>()
        }
    });
});

builder.AddApplicationServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapGroup("/account").MapIdentityApi<AppUser>().WithTags("Account");

app.MapApplicationRoutes();

var imagePath = Path.Combine(app.Environment.ContentRootPath, ImagePath.Directory);
if (!Directory.Exists(imagePath))
{
    Directory.CreateDirectory(imagePath);
    Directory.CreateDirectory(Path.Combine(imagePath, "movie"));
    Directory.CreateDirectory(Path.Combine(imagePath, "tv"));
}

if (app.Environment.IsDevelopment() || app.Configuration["AutomaticMigration"] == "1")
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;

    var dbContext = services.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();

    await dbContext.SaveChangesAsync();
}

app.Run();