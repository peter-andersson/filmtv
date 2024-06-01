using FilmTV.Web.Data;
using FilmTV.Web.Features.Posters;
using Microsoft.EntityFrameworkCore;

namespace FilmTV.Web.Features.Watchlist;

public interface IWatchlistHandler
{
    Task<List<WatchlistItem>> Get(string userId);
}

public class WatchlistHandler(IDbContextFactory<ApplicationDbContext> dbContextFactory) : IWatchlistHandler
{
    public async Task<List<WatchlistItem>> Get(string userId)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        var tmpShows = await dbContext.UserShows
            .TagWithCallSite()
            .AsNoTracking()
            .Include(s => s.UserEpisodes)
            .ThenInclude(e => e.Episode)
            .Where(x => x.ApplicationUserId == userId && x.UserEpisodes.Any(e => e.Watched == false && e.Episode.AirDate != null && e.Episode.AirDate <= DateTime.UtcNow))
            .GroupBy(e => e.ShowId)
            .Select(x => new { ShowId = x.Key, Count = x.Count() })
            .ToListAsync();

        var showTitles = await dbContext.UserShows
            .TagWithCallSite()
            .AsNoTracking()
            .Include(s => s.Show)
            .Where(u => u.ApplicationUserId == userId && tmpShows.Select(s => s.ShowId).Contains(u.ShowId))
            .Select(u => new { u.ShowId, u.Title, u.Show.OriginalTitle })
            .ToListAsync();

        var movies = await dbContext.UserMovies
            .TagWithCallSite()
            .AsNoTracking()
            .Include(m => m.Movie)
            .Where(m => m.ApplicationUserId == userId && m.WatchedDate == null)
            .Select(m => new WatchlistItem(m.Title ?? m.Movie.OriginalTitle, string.Empty, ImagePath.MovieUrl(m.MovieId), $"/Edit/Movie/{m.MovieId}"))
            .ToListAsync();

        var result = new List<WatchlistItem>(tmpShows.Count + movies.Count);

        foreach (var show in tmpShows)
        {
            var tmp = showTitles.FirstOrDefault(s => s.ShowId == show.ShowId);
            if (tmp is null)
            {
                continue;
            }
            
            result.Add(new WatchlistItem(tmp.Title ?? tmp.OriginalTitle, $"{show.Count} episodes", ImagePath.ShowUrl(show.ShowId), $"/Edit/Show/{show.ShowId}"));
        }

        result.AddRange(movies);

        result.Sort((x, y) => string.Compare(x.Title, y.Title, StringComparison.OrdinalIgnoreCase));

        return result;
    }
}

public record struct WatchlistItem(string Title, string SubTitle1, string ImageUrl, string EditUrl);