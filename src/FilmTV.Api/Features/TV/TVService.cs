using System.Data.Common;
using FilmTV.Api.Common;
using Microsoft.EntityFrameworkCore;
using OneOf.Types;
using OneOf;

namespace FilmTV.Api.Features.TV;

public interface ITVService
{
    Task Delete(int id, string userId, CancellationToken cancellationToken);
    
    Task<OneOf<SeriesResponse, NotFound>> Get(int id, string userId, CancellationToken cancellationToken);
    
    Task<OneOf<Success, NotFound>> Refresh(int id, CancellationToken cancellationToken);
    
    Task<IEnumerable<WatchlistSeriesResponse>> GetWatchlist(string userId, CancellationToken cancellationToken);
    
    Task<OneOf<Success, NotFound>> Update(int id, SeriesUpdateRequest request, string userId, CancellationToken cancellationToken);
    
    Task<OneOf<Success, NotFound>> MarkEpisodeAsWatched(int episodeId, bool watched, string userId, CancellationToken cancellationToken);
}

public class TVService(ILogger<TVService> logger, AppDbContext dbContext) : ITVService
{
    public async Task Delete(int id, string userId, CancellationToken cancellationToken)
    {
        // await dbContext.UserSeries.Where(s => s.UserId == userId && s.SeriesId == id).ExecuteDeleteAsync(cancellationToken);
        throw new NotImplementedException();
    }

    public async Task<OneOf<SeriesResponse, NotFound>> Get(int id, string userId, CancellationToken cancellationToken)
    {
        // var userSeries = await dbContext.UserSeries
        //     .Include(s => s.Series)
        //     .Include(s => s.Episodes)
        //     .ThenInclude(e => e.Episode)
        //     .Where(s => s.SeriesId == id && s.UserId == userId)
        //     .SingleOrDefaultAsync(cancellationToken);
        //
        // if (userSeries is null)
        // {
        //     return new NotFound();
        // }
        //
        // return userSeries.ToDto();
        
        throw new NotImplementedException();
    }

    public async Task<OneOf<Success, NotFound>> Refresh(int id, CancellationToken cancellationToken)
    {
        // var series = await dbContext.Series
        //     .AsTracking()
        //     .Include(e => e.Episodes)
        //     .Where(s => s.Id == id)
        //     .SingleOrDefaultAsync(cancellationToken);
        //
        // if (series is null)
        // {
        //     return new NotFound();
        // }
        //
        // var tmdbShow = await tmdbService.GetShow(id, series.ETag);
        // if (tmdbShow is null)
        // {
        //     return new Success();
        // }
        //
        // await DownloadPoster(tmdbShow, cancellationToken);
        //
        // series.OriginalTitle = tmdbShow.Title;
        // series.Status = tmdbShow.Status;
        // series.ETag = tmdbShow.ETag;
        // series.ImdbId = tmdbShow.ImdbId;
        // series.TvDbId = tmdbShow.TvDbId;
        //
        // series.SetNextUpdate();
        //
        // // Remove episodes for seasons that no longer exists.
        // var removedEpisodes = series.Episodes
        //     .Where(e => tmdbShow.Seasons.All(ts => ts.SeasonNumber != e.SeasonNumber))
        //     .Select(e => e.Id)
        //     .ToList();
        //
        // // Find if any episode have been removed in existing seasons
        // // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        // foreach (var season in tmdbShow.Seasons)
        // {
        //     var existingSeason = series.Episodes
        //         .Where(e => e.SeasonNumber == season.SeasonNumber)
        //         .ToList();
        //
        //     var removedEpisodesInSeason = existingSeason
        //         .Where(e => season.Episodes.All(te => te.EpisodeNumber != e.EpisodeNumber))
        //         .Select(e => e.Id)
        //         .ToList();
        //     
        //     removedEpisodes.AddRange(removedEpisodesInSeason);
        // }
        //
        // await dbContext.Episodes.Where(e => removedEpisodes.Contains(e.Id)).ExecuteDeleteAsync(cancellationToken);
        //
        // // Get users that have this series.
        // var users = await dbContext.UserSeries
        //     .Where(s => s.SeriesId == id)
        //     .Select(s => s.UserId)
        //     .ToListAsync(cancellationToken);        
        //
        // // Update or add new episodes
        // foreach (var tmdbSeason in tmdbShow.Seasons)
        // {
        //     foreach (var tmdbEpisode in tmdbSeason.Episodes)
        //     {
        //         var episode = series.Episodes
        //             .SingleOrDefault(e => e.SeasonNumber == tmdbSeason.SeasonNumber && e.EpisodeNumber == tmdbEpisode.EpisodeNumber);
        //
        //         if (episode is null)
        //         {
        //             episode = new Episode()
        //             {
        //                 EpisodeNumber = tmdbEpisode.EpisodeNumber,
        //                 SeasonNumber = tmdbSeason.SeasonNumber,
        //                 Title = tmdbEpisode.Title,
        //             };
        //
        //             series.Episodes.Add(episode);
        //             
        //             // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        //             foreach (var user in users)
        //             {
        //                 var userEpisode = new UserEpisode()
        //                 {
        //                     Episode = episode,
        //                     UserId = user,
        //                     Watched = false,
        //                     SeriesId = id
        //                 };
        //                 
        //                 dbContext.UserEpisodes.Add(userEpisode);
        //             }
        //         }
        //
        //         episode.Title = tmdbEpisode.Title;
        //         if (tmdbEpisode.AirDate.HasValue)
        //         {
        //             episode.AirDate = DateTime.SpecifyKind(tmdbEpisode.AirDate.Value, DateTimeKind.Utc).Date;
        //         }
        //     }
        // }
        //
        // await dbContext.SaveChangesAsync(cancellationToken);
        //
        // return new Success();
        
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<WatchlistSeriesResponse>> GetWatchlist(string userId,
        CancellationToken cancellationToken)
    {
        // var episodes = await dbContext.UserEpisodes
        //     .TagWithCallSite()
        //     .AsNoTracking()
        //     .AsSplitQuery()
        //     .Include(x => x.Episode)
        //     .Include(x => x.UserSeries)
        //     .ThenInclude(x => x.Series)
        //     .Where(x => x.UserId == userId && x.Watched == false && x.Episode.AirDate <= DateTime.UtcNow)
        //     .ToListAsync(cancellationToken);
        //
        // var series = episodes.Select(e => e.UserSeries);
        //
        // return series.Select(s => new WatchlistSeriesResponse(
        //     s.SeriesId,
        //     s.Title ?? s.Series.OriginalTitle,
        //     s.Episodes.Count(e => e.Watched == false && e.Episode.AirDate <= DateTime.UtcNow)));
        
        throw new NotImplementedException();
    }

    public async Task<OneOf<Success, NotFound>> Update(int id, SeriesUpdateRequest request, string userId,
        CancellationToken cancellationToken)
    {
        // var series = await dbContext.UserSeries
        //     .AsTracking()
        //     .Where(s => s.SeriesId == id && s.UserId == userId)
        //     .SingleOrDefaultAsync(cancellationToken);
        //
        // if (series is null)
        // {
        //     return new NotFound();
        // }
        //
        // series.Title = request.Title;
        // if (series.Rating != request.Rating)
        // {
        //     series.Rating = request.Rating;
        //     series.RatingDate = DateTime.UtcNow;
        // }
        //
        // await dbContext.SaveChangesAsync(cancellationToken);
        //
        // return new Success();
        
        throw new NotImplementedException();
    }

    public async Task<OneOf<Success, NotFound>> MarkEpisodeAsWatched(int episodeId,
        bool watched, string userId, CancellationToken cancellationToken)
    {
        // var episode = await dbContext.UserEpisodes
        //     .AsTracking()
        //     .Where(e => e.EpisodeId == episodeId && e.UserId == userId)
        //     .SingleOrDefaultAsync(cancellationToken);
        //
        // if (episode is null)
        // {
        //     return new NotFound();
        // }
        //
        // episode.Watched = watched;
        // await dbContext.SaveChangesAsync(cancellationToken);
        //
        // return new Success();
        
        throw new NotImplementedException();
    }
}