using FilmTV.Web.Features.TheMovieDatabase;
using FilmTV.Web.Features.Add;

// ReSharper disable once CheckNamespace
namespace Microsoft.AspNetCore.Builder;

public static class ApplicationServices
{
    /// <summary>
    /// Register services used by the application.
    /// </summary>
    public static void AddApplicationServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<ITheMovieDatabaseService, TheMovieDatabaseService>();
        builder.Services.AddScoped<IAddHandler, AddHandler>();
    }
}