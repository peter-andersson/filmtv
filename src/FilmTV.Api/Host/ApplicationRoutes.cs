using FilmTV.Api.Features.Images;
using FilmTV.Api.Features.TV;

namespace FilmTV.Api.Host;

public static class ApplicationRoutes
{
    /// <summary>
    /// Register routes used by the application.
    /// </summary>
    public static void MapApplicationRoutes(this IEndpointRouteBuilder app)
    {
        app.MapImageRoutes();
        app.MapTVRoutes();
    }
}