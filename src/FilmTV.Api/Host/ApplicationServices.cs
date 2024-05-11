using FilmTV.Api.Features.Movies;
using FilmTV.Api.Features.TheMovieDatabase;

namespace FilmTV.Api.Host;

public static class ApplicationServices
{
    /// <summary>
    /// Register services used by the application.
    /// </summary>
    public static void AddApplicationServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<ITheMovieDatabaseService, TheMovieDatabaseService>();
        builder.Services.AddScoped<IMovieService, MovieService>();
    }
}