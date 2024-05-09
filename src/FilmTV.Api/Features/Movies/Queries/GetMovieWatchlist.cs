using System.Security.Claims;
using System.Security.Principal;
using FilmTV.Api.Common.Features;
using FilmTV.Api.Common.Persistence;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace FilmTV.Api.Features.Movies.Queries;

public sealed class GetMovieWatchlist : IEndpoint
{
    public static void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/movie/watchlist",
                async (ClaimsPrincipal user, AppDbContext database, CancellationToken cancellationToken) =>
                    await HandleAsync(user, database, cancellationToken)
            )
            .RequireAuthorization()
            .Produces<List<WatchlistMovieDto>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithName("Watchlist")
            .WithTags("Movie")
            .WithOpenApi(operation =>
            {
                operation.Description = "Get all unwatched movies.";
                
                return operation;
            });
    }

    [UsedImplicitly]
    private record WatchlistMovieDto(
        int Id,
        string? Title
    );
    
    private static async ValueTask<IResult> HandleAsync(IPrincipal user, AppDbContext dbContext, CancellationToken cancellationToken)
    {
        var userId = user.Identity?.Name;
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Results.Unauthorized();
        }    
        
        var movies = await dbContext.UserMovies
            .Where(m => m.UserId == userId && m.WatchedDate == null)
            .Include(m => m.Movie)
            .ToListAsync(cancellationToken);

        List<WatchlistMovieDto> result = [];
        result.AddRange(movies.Select(userMovie => new WatchlistMovieDto(userMovie.MovieId, userMovie.Title ?? userMovie.Movie.OriginalTitle)));

        return Results.Ok(result);
    }
}