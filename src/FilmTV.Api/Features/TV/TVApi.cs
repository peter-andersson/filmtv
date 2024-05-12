namespace FilmTV.Api.Features.TV;

public static class TVApi
{
    public static void MapTVRoutes(this IEndpointRouteBuilder app)
    {
        app.MapGroup("/tv")
            .MapTVApi()
            .RequireAuthorization()
            .WithTags("TV")
            .WithOpenApi();
    }

    private static RouteGroupBuilder MapTVApi(this RouteGroupBuilder group)
    {
        return group;
    }
}