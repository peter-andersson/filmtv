using FilmTV.Web.Data;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace FilmTV.Web.Features.Get;

public interface IGetMovieHandler
{
    Task<OneOf<Movie, NotFound>> Get(int movieId, string userId);
}

public class GetMovieHandler(IDbContextFactory<ApplicationDbContext> dbContextFactory)
{
    public async Task<OneOf<Movie, NotFound>> Get(int movieId, string userId)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        var movie = await dbContext.UserMovies
            .TagWithCallSite()
            .AsNoTracking()
            .Include(m => m.Movie)
            .Where(m => m.ApplicationUserId == userId && m.WatchedDate == null)
            .Select(m => new Movie(
                    m.MovieId,
                    $"https://www.themoviedb.org/movie/{m.MovieId}",
                    $"https://www.imdb.com/title/{m.Movie.ImdbId}",
                    m.Movie.OriginalTitle,
                    m.Title,
                    m.Movie.ReleaseDate,
                    m.WatchedDate,
                    m.Movie.RunTime,
                    m.Rating
                ))
            .FirstOrDefaultAsync();

        return movie is null ? new NotFound() : movie;
    }
}

public record Movie(
    int MovieId,
    string TheMovieDbUrl, string ImdbUrl, string? OriginalTitle, string? Title,
    DateTime? ReleaseDate, DateTime? WatchedDate, int? Runtime, int? Rating);