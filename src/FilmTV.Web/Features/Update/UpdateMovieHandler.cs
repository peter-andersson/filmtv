using FilmTV.Web.Data;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace FilmTV.Web.Features.Update;

public interface IUpdateMovieHandler
{
    Task<OneOf<Success, NotFound>> Update(int movieId, string userId, string? title, int? rating, DateTime? watchDate);
}

public class UpdateMovieHandler(IDbContextFactory<ApplicationDbContext> dbContextFactory) : IUpdateMovieHandler
{
    public async Task<OneOf<Success, NotFound>> Update(int movieId, string userId, string? title, int? rating,
        DateTime? watchDate)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        var userMovie = await dbContext.UserMovies
            .TagWithCallSite()
            .AsTracking()
            .Where(m => m.ApplicationUserId == userId && m.MovieId == movieId)
            .FirstOrDefaultAsync();

        if (userMovie is null)
        {
            return new NotFound();
        }

        userMovie.Title = title;
        userMovie.WatchedDate = watchDate;
        if (userMovie.Rating.GetValueOrDefault(0) != rating.GetValueOrDefault(0))
        {
            userMovie.Rating = rating;
            userMovie.RatedAt = userMovie.Rating.HasValue ? DateTime.UtcNow : null; 
        }

        await dbContext.SaveChangesAsync();

        return new Success();
    }
}