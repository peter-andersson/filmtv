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
        
        group.MapPut("/refresh/{id:int}", RefreshHandler)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .WithSummary("Update tv series data from the themoviedb.org")
            .WithDescription("Fetch new data for a tv series from themoviedb.org.")
            .WithOpenApi(operation =>
            {
                operation.Parameters[0].Description = "Id for the movie";
                return operation;
            });
        
        // group.MapGet("/watchlist", WatchlistHandler)
        //     .Produces<IEnumerable<WatchlistMovieResponse>>()
        //     .WithSummary("Get unwatched movies")
        //     .WithDescription("Get all unwatched movies from the user's watchlist.");
        
        // group.MapPut("/{id:int}", UpdateHandler)
        //     .Produces<MovieResponse>()
        //     .ProducesValidationProblem()
        //     .Produces(StatusCodes.Status404NotFound)
        //     .WithSummary("Update user data for a movie")
        //     .WithDescription("Update user specific data for the movie. This can be the title, watched date or rating.")
        //     .WithOpenApi(operation =>
        //     {
        //         operation.Parameters[0].Description = "Id for the movie";
        //         return operation;
        //     });         
        
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
    
    private static async Task<IResult> RefreshHandler(
        int id,
        ITVService tvService,
        CancellationToken cancellationToken)
    {
        var result = await tvService.Refresh(id, cancellationToken);

        return result.Match(
            success => Results.NoContent(),
            notFound => Results.NotFound()
        );
    }    
}