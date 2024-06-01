using System.Diagnostics.CodeAnalysis;
using FilmTV.Web.Common;
using FilmTV.Web.Data;
using FilmTV.Web.Features.Posters;
using FilmTV.Web.Features.TheMovieDatabase;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace FilmTV.Web.Features.Add;

public interface IAddShow
{
    Task<OneOf<AddResponse, NotFound, Conflict>> AddShow(int showId, string userId);
}

[SuppressMessage("Usage", "EF1001:Internal EF Core API usage.")]
public class AddShowHandler(
    ILogger<AddShowHandler> logger,
    IDbContextFactory<ApplicationDbContext> dbContextFactory,
    ITheMovieDatabaseService tmdbService,
    IPosterDownload posterDownload
    ) : IAddShow
{
    private readonly ILogger<AddShowHandler> _logger = logger;
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory = dbContextFactory;
    private readonly ITheMovieDatabaseService _tmdbService = tmdbService;
    private readonly IPosterDownload _posterDownload = posterDownload;

    public async Task<OneOf<AddResponse, NotFound, Conflict>> AddShow(int showId, string userId)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        var show = await dbContext.Shows.TagWithCallSite()
            .AsNoTracking()
            .Include(x => x.Episodes)
            .Where(x => x.ShowId == showId)
            .FirstOrDefaultAsync();

        if (show is null)
        {
            var tmdbShow = await _tmdbService.GetShow(showId, null);
            if (tmdbShow is null)
            {
                _logger.LogError("TV show with id {ShowId} not found on themoviedb.org", showId);
                return new NotFound();
            }

            show = new Show()
            {
                ShowId = tmdbShow.Id,
                OriginalTitle = tmdbShow.Title,
                Status = tmdbShow.Status,
                ETag = tmdbShow.ETag,
                ImdbId = tmdbShow.ImdbId,
                TvDbId = tmdbShow.TvDbId
            };

            show.SetNextUpdate();
            dbContext.Shows.Add(show);
            await dbContext.SaveChangesAsync();

            foreach (var tmdbSeason in tmdbShow.Seasons)
            {
                foreach (var tmdbEpisode in tmdbSeason.Episodes)
                {
                    var episode = new Episode()
                    {
                        EpisodeNumber = tmdbEpisode.EpisodeNumber,
                        SeasonNumber = tmdbSeason.SeasonNumber,
                        Title = tmdbEpisode.Title,
                    };

                    if (tmdbEpisode.AirDate.HasValue)
                    {
                        episode.AirDate = DateTime.SpecifyKind(tmdbEpisode.AirDate.Value, DateTimeKind.Utc).Date;
                    }

                    show.Episodes.Add(episode);
                }

                await dbContext.SaveChangesAsync();
            }

            await _posterDownload.Download(tmdbShow.PosterPath, ImagePath.ShowFilename(tmdbShow.Id));
        }

        var userShow = await dbContext.UserShows
            .TagWithCallSite()
            .AsNoTracking()
            .Where(s => s.ShowId == showId && s.ApplicationUserId == userId)
            .FirstOrDefaultAsync();

        if (userShow is not null)
        {
            _logger.LogError("User already has series with id {Id}", showId);
            return new Conflict();
        }

        userShow = new UserShow()
        {
            ShowId = showId,
            ApplicationUserId = userId
        };
        dbContext.UserShows.Add(userShow);
        await dbContext.SaveChangesAsync();

        foreach (var episode in show.Episodes)
        {
            var userEpisode = new UserEpisode()
            {
                Episode = episode,
                UserShow = userShow,
                Watched = false
            };
            userShow.UserEpisodes.Add(userEpisode);
        }

        await dbContext.SaveChangesAsync();

        _logger.LogInformation("Added show with id {Id}", show.ShowId);

        return new AddResponse(show.OriginalTitle);
    }
}