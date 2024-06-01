using FilmTV.Web.Features.TheMovieDatabase;
using FilmTV.Web.Features.Add;
using FilmTV.Web.Features.Posters;
using FilmTV.Web.Features.Watchlist;

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
        builder.Services.AddScoped<IPosterDownload, PosterDownloader>();
        builder.Services.AddScoped<IAddMovie, AddMovieHandler>();
        builder.Services.AddScoped<IAddShow, AddShowHandler>();
        builder.Services.AddScoped<IWatchlistHandler, WatchlistHandler>();
    }
}