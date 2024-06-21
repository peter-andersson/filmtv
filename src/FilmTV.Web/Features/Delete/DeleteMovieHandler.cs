using FilmTV.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace FilmTV.Web.Features.Delete;

public interface IDeleteMovieHandler
{
    Task Delete(int movieId, string userId);
}

public class DeleteMovieHandler(IDbContextFactory<ApplicationDbContext> dbContextFactory) : IDeleteMovieHandler
{
    public async Task Delete(int movieId, string userId)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        await dbContext.UserMovies
            .TagWithCallSite()
            .AsNoTracking()
            .Where(m => m.ApplicationUserId == userId && m.MovieId == movieId)
            .ExecuteDeleteAsync();
    }
}