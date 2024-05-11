using FilmTV.Api.Features.Images;
using FilmTV.Api.Features.Movies;

namespace FilmTV.Api.Host;

public static class ApplicationRoutes
{
    /// <summary>
    /// Register routes used by the application.
    /// </summary>
    public static void MapApplicationRoutes(this IEndpointRouteBuilder app)
    {
        app.MapMovieRoutes();
        app.MapImageRoutes();
    }
}