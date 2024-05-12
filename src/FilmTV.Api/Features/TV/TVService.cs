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