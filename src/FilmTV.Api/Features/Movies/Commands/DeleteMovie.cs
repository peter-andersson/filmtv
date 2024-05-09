using System.Security.Claims;
using System.Security.Principal;
using FilmTV.Api.Common.Features;
using FilmTV.Api.Common.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FilmTV.Api.Features.Movies.Commands;

public sealed class DeleteMovie : IEndpoint
{
    public static void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapDelete("/movie/{id:int}",
                async (int id, ClaimsPrincipal user, AppDbContext database, CancellationToken cancellationToken) =>
                    await HandleAsync(id, user, database, cancellationToken)
            )
            .RequireAuthorization()
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithName("Delete")
            .WithTags("Movie")
            .WithOpenApi(operation =>
            {
                operation.Description = "Delete a movie from the user's list of movies";

                operation.Parameters[0].Description = "Id for the movie";

                return operation;
            });
    }

    private static async ValueTask<IResult> HandleAsync(int id, IPrincipal user, AppDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var userId = user.Identity?.Name;
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Results.Unauthorized();
        }

        await dbContext.UserMovies
            .Where(m => m.MovieId == id && m.UserId == userId)
            .ExecuteDeleteAsync(cancellationToken);
        
        return Results.Ok();
    }
}