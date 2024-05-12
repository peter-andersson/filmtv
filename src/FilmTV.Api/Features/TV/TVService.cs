using FilmTV.Api.Common;
using FilmTV.Api.Features.TheMovieDatabase;
using Microsoft.EntityFrameworkCore;
using OneOf.Types;
using OneOf;

namespace FilmTV.Api.Features.TV;

public interface ITVService
{
    Task<OneOf<SeriesResponse, NotFound, Conflict>> Add(int id, string userId, CancellationToken cancellationToken);
    
    Task Delete(int id, string userId, CancellationToken cancellationToken);
    
    Task<OneOf<SeriesResponse, NotFound>> Get(int id, string userId, CancellationToken cancellationToken);
    
    Task<OneOf<Success, NotFound>> Refresh(int id, CancellationToken cancellationToken);
}

public class TVService(ILogger<TVService> logger, AppDbContext dbContext, ITheMovieDatabaseService tmdbService) : ITVService
{
    public async Task<OneOf<SeriesResponse, NotFound, Conflict>> Add(int id, string userId, CancellationToken cancellationToken)
    {
        var series = await dbContext.Series
            .AsTracking()
            .Include(s => s.Episodes)
            .Where(s => s.Id == id)
            .SingleOrDefaultAsync(cancellationToken);

        if (series is null)
        {
            var tmdbShow = await tmdbService.GetShow(id, null);
            if (tmdbShow is null)
            {
                logger.LogError("TV series with id {SeriesId} not found on themoviedb.org", id);
                return new NotFound();
            }

            series = new Series()
            {
                Id = tmdbShow.Id,
                OriginalTitle = tmdbShow.Title,
                Status = tmdbShow.Status,
                ETag = tmdbShow.ETag,
                ImdbId = tmdbShow.ImdbId,
                TvDbId = tmdbShow.TvDbId
            };

            series.SetNextUpdate();
            
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

                    series.Episodes.Add(episode);
                }
            }

            dbContext.Series.Add(series);

            await DownloadMoviePoster(tmdbShow, cancellationToken);
        }
        
        var userSeries = await dbContext.UserSeries
            .Where(s => s.SeriesId == id && s.UserId == userId)
            .FirstOrDefaultAsync(cancellationToken);

        if (userSeries is not null)
        {
            logger.LogError("User already has series with id {Id}", id);
            return new Conflict();
        }

        userSeries = new UserSeries()
        {
            Series = series,
            UserId = userId
        };
        
        foreach (var episode in series.Episodes)
        {
            var userEpisode = new UserEpisode()
            {
                Episode = episode,
                UserId = userId,
                Watched = false
            };
            
            userSeries.Episodes.Add(userEpisode);
        }

        dbContext.UserSeries.Add(userSeries);

        await dbContext.SaveChangesAsync(cancellationToken);
        
        return userSeries.ToDto();
    }

    public async Task Delete(int id, string userId, CancellationToken cancellationToken)
    {
        await dbContext.UserSeries.Where(s => s.UserId == userId && s.SeriesId == id).ExecuteDeleteAsync(cancellationToken);
    }

    public async Task<OneOf<SeriesResponse, NotFound>> Get(int id, string userId, CancellationToken cancellationToken)
    {
        var userSeries = await dbContext.UserSeries
            .Include(s => s.Series)
            .Include(s => s.Episodes)
            .ThenInclude(e => e.Episode)
            .Where(s => s.SeriesId == id && s.UserId == userId)
            .SingleOrDefaultAsync(cancellationToken);

        if (userSeries is null)
        {
            return new NotFound();
        }
        
        return userSeries.ToDto();
    }

    public async Task<OneOf<Success, NotFound>> Refresh(int id, CancellationToken cancellationToken)
    {
        var series = await dbContext.Series
            .AsTracking()
            .Include(e => e.Episodes)
            .Where(s => s.Id == id)
            .SingleOrDefaultAsync(cancellationToken);

        if (series is null)
        {
            return new NotFound();
        }
        
        var tmdbShow = await tmdbService.GetShow(id, series.ETag);
        if (tmdbShow is null)
        {
            return new Success();
        }
        
        await DownloadMoviePoster(tmdbShow, cancellationToken);

        series.OriginalTitle = tmdbShow.Title;
        series.Status = tmdbShow.Status;
        series.ETag = tmdbShow.ETag;
        series.ImdbId = tmdbShow.ImdbId;
        series.TvDbId = tmdbShow.TvDbId;

        series.SetNextUpdate();
        
        // Remove episodes for seasons that no longer exists.
        var removedEpisodes = series.Episodes
            .Where(e => tmdbShow.Seasons.All(ts => ts.SeasonNumber != e.SeasonNumber))
            .Select(e => e.Id)
            .ToList();
        
        // Find if any episode have been removed in existing seasons
        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (var season in tmdbShow.Seasons)
        {
            var existingSeason = series.Episodes
                .Where(e => e.SeasonNumber == season.SeasonNumber)
                .ToList();

            var removedEpisodesInSeason = existingSeason
                .Where(e => season.Episodes.All(te => te.EpisodeNumber != e.EpisodeNumber))
                .Select(e => e.Id)
                .ToList();
            
            removedEpisodes.AddRange(removedEpisodesInSeason);
        }
        
        await dbContext.Episodes.Where(e => removedEpisodes.Contains(e.Id)).ExecuteDeleteAsync(cancellationToken);        
        
        // Update or add new episodes
        foreach (var tmdbSeason in tmdbShow.Seasons)
        {
            foreach (var tmdbEpisode in tmdbSeason.Episodes)
            {
                var episode = series.Episodes
                    .SingleOrDefault(e => e.SeasonNumber == tmdbSeason.SeasonNumber && e.EpisodeNumber == tmdbEpisode.EpisodeNumber);

                if (episode is null)
                {
                    episode = new Episode()
                    {
                        EpisodeNumber = tmdbEpisode.EpisodeNumber,
                        SeasonNumber = tmdbSeason.SeasonNumber,
                        Title = tmdbEpisode.Title,
                    };

                    series.Episodes.Add(episode);
                    
                    // Add episode to all users that are following the series
                    var users = await dbContext.UserSeries
                        .Where(s => s.SeriesId == id)
                        .Select(s => s.UserId)
                        .ToListAsync(cancellationToken);

                    // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
                    foreach (var user in users)
                    {
                        var userEpisode = new UserEpisode()
                        {
                            Episode = episode,
                            UserId = user,
                            Watched = false,
                            SeriesId = id
                        };
                        
                        dbContext.UserEpisodes.Add(userEpisode);
                    }
                }

                episode.Title = tmdbEpisode.Title;
                if (tmdbEpisode.AirDate.HasValue)
                {
                    episode.AirDate = DateTime.SpecifyKind(tmdbEpisode.AirDate.Value, DateTimeKind.Utc).Date;
                }
            }
        }
        
        await dbContext.SaveChangesAsync(cancellationToken);

        return new Success();
    }
    
    private async Task DownloadMoviePoster(TMDbShow tmdbShow, CancellationToken cancellationToken)
    {
        var posterUrl = await tmdbService.GetImageUrl(tmdbShow.PosterPath);
        if (string.IsNullOrWhiteSpace(posterUrl))
        {
            return;
        }

        var stream = new MemoryStream();
        await tmdbService.DownloadImageUrlToStream(posterUrl, stream);

        var filename = ImagePath.SeriesPath(tmdbShow.Id);
        if (File.Exists(filename))
        {
            File.Delete(filename);
        }

        await using var fileStream = new FileStream(filename, FileMode.Create);
        await stream.CopyToAsync(fileStream, cancellationToken);
    }    
}