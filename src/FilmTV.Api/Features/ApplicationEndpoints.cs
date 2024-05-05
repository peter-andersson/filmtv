using FilmTV.Api.Features.Add;

namespace FilmTV.Api.Features;

public static class ApplicationEndpoints
{
    public static void MapApplicationEndpoints(this WebApplication app)
    {
        app.MapAddEndpoints();
    }
}