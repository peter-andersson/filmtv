// ReSharper disable UnusedParameter.Local
// ReSharper disable ConvertClosureToMethodGroup

using System.Security.Claims;
using FilmTV.Api.Common;

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
        group.MapPost("/{seriesId:int}", AddHandler)
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
        
        group.MapDelete("/{seriesId:int}", DeleteHandler)
            .Produces(StatusCodes.Status204NoContent)
            .WithSummary("Remove a tv series")
            .WithDescription("Remove a tv series from the system.")
            .WithOpenApi(operation =>
            {
                operation.Parameters[0].Description = "Id for the tv series";
                return operation;
            });
        
        group.MapGet("/{seriesId:int}", GetHandler)
            .Produces<SeriesResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .WithSummary("Get a tv series")
            .WithDescription("Get a tv series by specifying the id.")
            .WithOpenApi(operation =>
            {
                operation.Parameters[0].Description = "Id for the tv series";
                return operation;
            });
        
        group.MapPut("/refresh/{seriesId:int}", RefreshHandler)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .WithSummary("Update tv series data from the themoviedb.org")
            .WithDescription("Fetch new data for a tv series from themoviedb.org.")
            .WithOpenApi(operation =>
            {
                operation.Parameters[0].Description = "Id for the movie";
                return operation;
            });
        
        group.MapGet("/watchlist", WatchlistHandler)
            .Produces<IEnumerable<WatchlistSeriesResponse>>()
            .WithSummary("Get unwatched series")
            .WithDescription("Get all unwatched tv series from the user's watchlist.");
        
        group.MapPut("/{seriesId:int}", UpdateHandler)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .WithSummary("Update a tv series")
            .WithDescription("Update a tv series by specifying the id and the updated fields.")
            .WithOpenApi(operation =>
            {
                operation.Parameters[0].Description = "Id for the tv series";
                return operation;
            });        
        
        group.MapPut("/episode/{episodeId:int}/watched/{watched:bool}", MarkEpisodeAsWatchedHandler)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .WithSummary("Mark an episode as watched or unwatched")
            .WithDescription("Mark an episode as watched or unwatched by specifying the series id, season number, episode number and a boolean indicating whether the episode is watched or not.")
            .WithOpenApi(operation =>
            {
                operation.Parameters[0].Description = "Episode number";
                operation.Parameters[1].Description = "Boolean indicating whether the episode is watched or not";
                return operation;
            });
        
        return group;
    }
    
    private static async Task<IResult> AddHandler(
        int seriesId,
        ClaimsPrincipal user,
        ITVService tvService,
        CancellationToken cancellationToken)
    {
        var userId = user.GetUserId();

        var result = await tvService.Add(seriesId, userId, cancellationToken);

        return result.Match(
            seriesResponse =>  Results.Ok(seriesResponse),
            notFound => Results.NotFound(),
            conflict => Results.Conflict()
        );
    }
    
    private static async Task<IResult> DeleteHandler(
        int seriesId,
        ClaimsPrincipal user,
        ITVService tvService,
        CancellationToken cancellationToken)
    {
        var userId = user.GetUserId();

        await tvService.Delete(seriesId, userId, cancellationToken);

        return Results.NoContent();
    }
    
    private static async Task<IResult> GetHandler(
        int seriesId,
        ClaimsPrincipal user,
        ITVService tvService,
        CancellationToken cancellationToken)
    {
        var userId = user.GetUserId();

        await tvService.Delete(seriesId, userId, cancellationToken);

        return Results.NoContent();
    }   
    
    private static async Task<IResult> RefreshHandler(
        int seriesId,
        ITVService tvService,
        CancellationToken cancellationToken)
    {
        var result = await tvService.Refresh(seriesId, cancellationToken);

        return result.Match(
            success => Results.NoContent(),
            notFound => Results.NotFound()
        );
    }

    private static async Task<IResult> WatchlistHandler(ClaimsPrincipal user, ITVService tvService,
        CancellationToken cancellationToken)
    {
        var userId = user.GetUserId();

        var result = await tvService.GetWatchlist(userId, cancellationToken);
                    
        return Results.Ok(result);
    } 
    
    private static async Task<IResult> UpdateHandler(
        int seriesId,
        SeriesUpdateRequest request,
        ClaimsPrincipal user,
        ITVService tvService,
        CancellationToken cancellationToken)
    {
        var userId = user.GetUserId();

        var result = await tvService.Update(seriesId, request, userId, cancellationToken);

        return result.Match(
            success => Results.NoContent(),
            notFound => Results.NotFound()
        );
    }    
    
    private static async Task<IResult> MarkEpisodeAsWatchedHandler(
        int episodeId,
        bool watched,
        ClaimsPrincipal user,
        ITVService tvService,
        CancellationToken cancellationToken)
    {
        var userId = user.GetUserId();

        var result = await tvService.MarkEpisodeAsWatched(episodeId, watched, userId, cancellationToken);

        return result.Match(
            success => Results.NoContent(),
            notFound => Results.NotFound()
        );
    }
}