using System.Security.Claims;
using System.Security.Principal;
using FilmTV.Api.Common.Features;
using FilmTV.Api.Common.Persistence;
using FilmTV.Api.Features.Movies.Models;
using Microsoft.EntityFrameworkCore;
using TheMovieDatabase;

namespace FilmTV.Api.Features.Movies.Commands;

public sealed class AddMovie : IEndpoint
{
    public static void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/movie/{id:int}",
                async (int id, ClaimsPrincipal user, AppDbContext database, ITheMovieDatabaseService tmdbService,
                        CancellationToken cancellationToken) =>
                    await HandleAsync(id, user, database, tmdbService, cancellationToken)
            )
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status409Conflict)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithName("Add")
            .WithTags("Movie")
            .WithOpenApi(operation =>
            {
                operation.Description = "Add new movie";

                operation.Parameters[0].Description = "Id for the movie from themoviedb.org";

                return operation;
            });
    }

    private static async ValueTask<IResult> HandleAsync(int id, IPrincipal user, AppDbContext dbContext,
        ITheMovieDatabaseService tmdbService, CancellationToken cancellationToken)
    {
        var userId = user.Identity?.Name;
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Results.Unauthorized();
        }

        var movie = await dbContext.Movies.Where(m => m.MovieId == id).FirstOrDefaultAsync(cancellationToken);

        if (movie is not null)
        {
            var tmdbMovie = await tmdbService.GetMovieAsync(id, null);
            if (tmdbMovie is null)
            {
                return Results.NotFound();
            }

            movie = new Movie
            {
                MovieId = tmdbMovie.Id,
                OriginalTitle = tmdbMovie.OriginalTitle,
                OriginalLanguage = tmdbMovie.OriginalLanguage,
                ImdbId = tmdbMovie.ImdbId,
                ReleaseDate = tmdbMovie.ReleaseDate,
                RunTime = tmdbMovie.RunTime,
                ETag = tmdbMovie.ETag
            };

            dbContext.Movies.Add(movie);
        }

        if (movie is null)
        {
            return Results.NotFound();
        }

        var userMovie = await dbContext.UserMovies
            .Where(m => m.MovieId == id && m.UserId == userId)
            .FirstOrDefaultAsync(cancellationToken);

        if (userMovie is not null)
        {
            return Results.Conflict();
        }

        userMovie = new UserMovie
        {
            Movie = movie,
            UserId = userId
        };

        dbContext.UserMovies.Add(userMovie);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Results.NoContent();
    }
}