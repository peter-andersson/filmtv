using System.Security.Claims;
using System.Security.Principal;
using FilmTV.Api.Common.Features;
using FilmTV.Api.Common.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheMovieDatabase;

namespace FilmTV.Api.Features.Movies.Commands;

public sealed class RefreshMovie : IEndpoint
{
    public static void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPut("/movie/reload/{id:int}",
                async (int id, ClaimsPrincipal user, [FromServices] AppDbContext database, [FromServices] ITheMovieDatabaseService tmdbService, CancellationToken cancellationToken) =>
                    await HandleAsync(id, user, database, tmdbService, cancellationToken)
            )
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithName("Reload")
            .WithTags("Movie")
            .WithOpenApi(operation =>
            {
                operation.Description = "Reload a movie from themoviedb.org";

                operation.Parameters[0].Description = "Id for the movie";

                return operation;
            });
    }

    private static async ValueTask<IResult> HandleAsync(int id, IPrincipal user, AppDbContext dbContext,
        ITheMovieDatabaseService tmdbService,
        CancellationToken cancellationToken)
    {
        var userId = user.Identity?.Name;
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Results.Unauthorized();
        }

        var movie = await dbContext.Movies.AsTracking().Where(m => m.MovieId == id)
            .FirstOrDefaultAsync(cancellationToken);

        if (movie is null)
        {
            return Results.NotFound();
        }

        var tmdbMovie = await tmdbService.GetMovieAsync(id, movie.ETag);
        if (tmdbMovie is null)
        {
            return Results.NoContent();
        }

        movie.OriginalTitle = tmdbMovie.OriginalTitle;
        movie.OriginalLanguage = tmdbMovie.OriginalLanguage;
        movie.ImdbId = tmdbMovie.ImdbId;
        movie.ReleaseDate = tmdbMovie.ReleaseDate;
        movie.RunTime = tmdbMovie.RunTime;
        movie.ETag = tmdbMovie.ETag;

        await dbContext.SaveChangesAsync(cancellationToken);
        
        return Results.NoContent();
    }
}