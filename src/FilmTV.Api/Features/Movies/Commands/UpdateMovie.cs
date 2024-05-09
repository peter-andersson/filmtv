using System.Security.Claims;
using System.Security.Principal;
using FilmTV.Api.Common.Features;
using FilmTV.Api.Common.Persistence;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FilmTV.Api.Features.Movies.Commands;

public sealed class UpdateMovie : IEndpoint
{
    private static readonly UpdateMovieValidator Validator = new();
    
    public static void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPut("/movie/{id:int}",
                async (int id, [FromBody] UpdateMovieDto updateMovie, ClaimsPrincipal user, AppDbContext database, CancellationToken cancellationToken) =>
                    await HandleAsync(id, updateMovie, user, database, cancellationToken)
            )
            .RequireAuthorization()
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithName("Update")
            .WithTags("Movie")
            .WithOpenApi(operation =>
            {
                operation.Description = "Update a movie";

                operation.Parameters[0].Description = "Id for the movie";

                return operation;
            });
    }
    
    private static async ValueTask<IResult> HandleAsync(int id, UpdateMovieDto updateMovie, IPrincipal user, AppDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var userId = user.Identity?.Name;
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Results.Unauthorized();
        }        
        
        var validationResult = await Validator.ValidateAsync(updateMovie, cancellationToken);

        if (!validationResult.IsValid) 
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }
        
        var userMovie = await dbContext.UserMovies
            .AsTracking()
            .Where(m => m.MovieId == id && m.UserId == userId)
            .FirstOrDefaultAsync(cancellationToken);

        if (userMovie is null)
        {
            return Results.NotFound();
        }
        
        userMovie.Title = updateMovie.Title;
        userMovie.WatchedDate = updateMovie.WatchedDate;
        if (userMovie.Rating != updateMovie.Rating)
        {
            userMovie.Rating = updateMovie.Rating;
            userMovie.RatingDate = DateTime.UtcNow;
        }
        
        await dbContext.SaveChangesAsync(cancellationToken);
        
        return Results.Ok();
    }
    
    [UsedImplicitly]
    private record UpdateMovieDto(int MovieId, string? Title, DateTime? WatchedDate, int Rating);
    
    private class UpdateMovieValidator : AbstractValidator<UpdateMovieDto> 
    {
        public UpdateMovieValidator()
        {
            RuleFor(x => x.MovieId).GreaterThanOrEqualTo(1);
            RuleFor(x => x.Title).MaximumLength(256);
            RuleFor(x => x.Rating).LessThanOrEqualTo(10).GreaterThanOrEqualTo(0);
        }
    }
}