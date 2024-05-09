using System.Security.Claims;
using System.Security.Principal;
using FilmTV.Api.Common.Features;
using FilmTV.Api.Common.Persistence;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace FilmTV.Api.Features.Movies.Queries;

public sealed class GetMovie : IEndpoint
{
    public static void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/movie/{id:int}",
                async (int id, ClaimsPrincipal user, AppDbContext database, CancellationToken cancellationToken) =>
                    await HandleAsync(id, user, database, cancellationToken)
            )
            .RequireAuthorization()
            .Produces<MovieDto>()
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithName("Get")
            .WithTags("Movie")
            .WithOpenApi(operation =>
            {
                operation.Description = "Get a specific movie";

                operation.Parameters[0].Description = "Id for the movie";

                return operation;
            });
    }

    [UsedImplicitly]
    private record MovieDto(
        int Id,
        string? Title,
        string OriginalTitle,
        string OriginalLanguage,
        DateTime? WatchedDate,
        int Rating,
        DateTime? ReleaseDate,
        int? RunTime);
    
    private static async ValueTask<IResult> HandleAsync(int movieId, IPrincipal user, AppDbContext dbContext, CancellationToken cancellationToken)
    {
        var userId = user.Identity?.Name;
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Results.Unauthorized();
        }        
        
        var userMovie = await dbContext.UserMovies
            .Where(m => m.MovieId == movieId).Include(m => m.Movie)
            .FirstOrDefaultAsync(cancellationToken);
        
        if (userMovie is null)
        {
            return Results.NotFound();
        }
        
        return Results.Ok(new MovieDto(
            userMovie.Id,
            userMovie.Title,
            userMovie.Movie.OriginalTitle,
            userMovie.Movie.OriginalLanguage,
            userMovie.WatchedDate,
            userMovie.Rating,
            userMovie.Movie.ReleaseDate,
            userMovie.Movie.RunTime
        ));
    }
}