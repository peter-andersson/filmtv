// ReSharper disable UnusedParameter.Local
// ReSharper disable ConvertClosureToMethodGroup

using System.Security.Claims;

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
        group.MapPost("/{id:int}", AddHandler)
            .Produces<SeriesResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict)
            .WithSummary("Add a new tv series")
            .WithDescription("Add a new tv series by specifying the id from themoviedb.org. If the tv series doesn't exist it will be fetched from themoviedb.org and then added to the system.")
            .WithOpenApi(operation =>
            {
                operation.Parameters[0].Description = "Id for the tv series from themoviedb.org";
                return operation;
            });  
        
        group.MapDelete("/{id:int}", DeleteHandler)
            .Produces(StatusCodes.Status204NoContent)
            .WithSummary("Remove a tv series")
            .WithDescription("Remove a tv series from the system.")
            .WithOpenApi(operation =>
            {
                operation.Parameters[0].Description = "Id for the tv series";
                return operation;
            });
        
        group.MapGet("/{id:int}", GetHandler)
            .Produces<SeriesResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .WithSummary("Get a tv series")
            .WithDescription("Get a tv series by specifying the id.")
            .WithOpenApi(operation =>
            {
                operation.Parameters[0].Description = "Id for the tv series";
                return operation;
            });        
        
        return group;
    }
    
    private static async Task<IResult> AddHandler(
        int id,
        ClaimsPrincipal user,
        ITVService tvService,
        CancellationToken cancellationToken)
    {
        var userId = user.Identity?.Name ?? string.Empty;

        var result = await tvService.Add(id, userId, cancellationToken);

        return result.Match(
            seriesResponse =>  Results.Ok(seriesResponse),
            notFound => Results.NotFound(),
            conflict => Results.Conflict()
        );
    }
    
    private static async Task<IResult> DeleteHandler(
        int id,
        ClaimsPrincipal user,
        ITVService tvService,
        CancellationToken cancellationToken)
    {
        var userId = user.Identity?.Name ?? string.Empty;

        await tvService.Delete(id, userId, cancellationToken);

        return Results.NoContent();
    }
    
    private static async Task<IResult> GetHandler(
        int id,
        ClaimsPrincipal user,
        ITVService tvService,
        CancellationToken cancellationToken)
    {
        var userId = user.Identity?.Name ?? string.Empty;

        await tvService.Delete(id, userId, cancellationToken);

        return Results.NoContent();
    }    
}