// ReSharper disable UnusedParameter.Local
// ReSharper disable ConvertClosureToMethodGroup

using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace FilmTV.Api.Features.Movies;

public static class MovieApi
{
    public static void MapMovieRoutes(this IEndpointRouteBuilder app)
    {
        app.MapGroup("/movie")
            .MapMovieApi()
            .RequireAuthorization()
            .WithTags("Movie")
            .WithOpenApi();
    }
    
    private static RouteGroupBuilder MapMovieApi(this RouteGroupBuilder group)
    {
        group.MapPost("/{id:int}", AddMovieAsync)
            .Produces<MovieResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict)
            .WithSummary("Add a new movie")
            .WithDescription("Add a new movie by specifying the id from themoviedb.org. If the movie doesn't exist it will be fetched from themoviedb.org and then added to the system.")
            .WithOpenApi(operation =>
            {
                operation.Parameters[0].Description = "Id for the movie from themoviedb.org";
                return operation;
            });    
        
        group.MapDelete("/{id:int}", DeleteMovieAsync)
            .Produces(StatusCodes.Status204NoContent)
            .WithSummary("Delete a movie")
            .WithDescription("Delete a movie from the user's list of movies.")
            .WithOpenApi(operation =>
            {
                operation.Parameters[0].Description = "Id for the movie";
                return operation;
            });
        
        group.MapGet("/{id:int}", GetMovieAsync)
            .Produces<MovieResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .WithSummary("Get a movie")
            .WithDescription("Get a movie by specifying the id.")
            .WithOpenApi(operation =>
            {
                operation.Parameters[0].Description = "Id for the movie";
                return operation;
            });
        
        group.MapPut("/movie/reload/{id:int}", RefreshMovieAsync)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .WithSummary("Update movie data from the themoviedb.org")
            .WithDescription("Fetch new data for a movie from themoviedb.org. This will update the movie with the latest data from themoviedb.org.")
            .WithOpenApi(operation =>
            {
                operation.Parameters[0].Description = "Id for the movie";
                return operation;
            });
            
        group.MapGet("/watchlist", GetWatchlistAsync)
            .Produces<IEnumerable<WatchlistMovieResponse>>()
            .WithSummary("Get unwatched movies")
            .WithDescription("Get all unwatched movies from the user's watchlist.");
        
        group.MapPut("/{id:int}", UpdateMovieAsync)
             .Produces<MovieResponse>()
             .ProducesValidationProblem()
             .Produces(StatusCodes.Status404NotFound)
             .WithSummary("Update user data for a movie")
             .WithDescription("Update user specific data for the movie. This can be the title, watched date or rating.")
             .WithOpenApi(operation =>
             {
                 operation.Parameters[0].Description = "Id for the movie";
                 return operation;
             });        

        return group;
    }
    
    private static async Task<IResult> AddMovieAsync(
        int id,
        ClaimsPrincipal user,
        IMovieService movieService,
        CancellationToken cancellationToken)
    {
        var userId = user.Identity?.Name ?? string.Empty;

        var result = await movieService.AddMovieAsync(id, userId, cancellationToken);

        return result.Match(
            movieDto => Results.Ok(movieDto),
            notFound => Results.NotFound(),
            conflict => Results.Conflict()
        );
    }
    
    private static async Task<IResult> DeleteMovieAsync(
        int id,
        ClaimsPrincipal user,
        IMovieService movieService,
        CancellationToken cancellationToken)
    {
        var userId = user.Identity?.Name ?? string.Empty;

        await movieService.DeleteMovieAsync(id, userId, cancellationToken);

        return Results.NoContent();
    }
    
    private static async Task<IResult> GetMovieAsync(
        int id,
        ClaimsPrincipal user,
        IMovieService movieService,
        CancellationToken cancellationToken)
    {
        var userId = user.Identity?.Name ?? string.Empty;

        var result = await movieService.GetMovieAsync(id, userId, cancellationToken);

        return result.Match(
            movieDto => Results.Ok(movieDto),
            notFound => Results.NotFound()
        );
    }
    
    private static async Task<IResult> RefreshMovieAsync(
        int id,
        IMovieService movieService,
        CancellationToken cancellationToken)
    {
        var result = await movieService.RefreshMovieAsync(id, cancellationToken);

        return result.Match(
            success => Results.NoContent(),
            notFound => Results.NotFound()
        );
    }
    
    private static async Task<IResult> GetWatchlistAsync(ClaimsPrincipal user, IMovieService movieService, CancellationToken cancellationToken)
    {
        var userId = user.Identity?.Name ?? string.Empty;

        var result = await movieService.GetWatchlistAsync(userId, cancellationToken);
                    
        return Results.Ok(result);
    }
    
    private static async Task<IResult> UpdateMovieAsync(
        int id,
        [FromBody] UpdateMovieRequest updateMovie,
        ClaimsPrincipal user,
        IMovieService movieService,
        CancellationToken cancellationToken)
    {
        var userId = user.Identity?.Name ?? string.Empty;

        var result = await movieService.UpdateMovieAsync(id, userId, updateMovie, cancellationToken);
                
        return result.Match(
            movieDto => Results.Ok(movieDto),
            notFound => Results.NotFound(),
            validationError => Results.ValidationProblem(validationError.Errors)
        );
    }
}