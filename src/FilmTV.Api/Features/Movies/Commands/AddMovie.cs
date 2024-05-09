using System.Security.Claims;
using System.Security.Principal;
using FilmTV.Api.Common.Features;
using FilmTV.Api.Common.Persistence;
using FilmTV.Api.Features.Movies.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TheMovieDatabase;

namespace FilmTV.Api.Features.Movies.Commands;

public sealed class AddMovie : IEndpoint
{
    public static void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/movie/add/{id:int}",
            async (int id, ClaimsPrincipal user, AppDbContext database, ITheMovieDatabaseService tmdbService, CancellationToken cancellationToken) =>
                    await HandleAsync(id, user, database, tmdbService, cancellationToken)
                )
            .RequireAuthorization()
            .Produces(StatusCodes.Status401Unauthorized)            
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithName("Add")
            .WithTags("Movie")
            .WithOpenApi(generatedResponse =>
            {
                generatedResponse.Description = "Add new movie";

                generatedResponse.Parameters.Add(new OpenApiParameter
                {
                    Name = "id",
                    In = ParameterLocation.Path,
                    Required = true,
                    Description = "Id for the movie from themoviedb.org",
                    Schema = new OpenApiSchema
                    {
                        Type = "integer",
                        Format = "int32"
                    }
                });

                return generatedResponse;
            });
    }
    
    private static async ValueTask<IResult> HandleAsync(int id, IPrincipal user, AppDbContext dbContext, ITheMovieDatabaseService tmdbService, CancellationToken cancellationToken)
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

            movie = new Models.Movie()
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
            return Results.Ok();            
        }
        
        userMovie = new UserMovie()
        {
            Movie = movie,
            UserId = userId
        };

        dbContext.UserMovies.Add(userMovie);

        await dbContext.SaveChangesAsync(cancellationToken);
        
        return Results.Ok();
    }
}