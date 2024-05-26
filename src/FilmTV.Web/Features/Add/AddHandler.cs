using System.Diagnostics.CodeAnalysis;
using FilmTV.Api.Features.TheMovieDatabase;
using FilmTV.Web.Common;
using FilmTV.Web.Data;
using FilmTV.Web.Features.TheMovieDatabase;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace FilmTV.Web.Features.Add;

public interface IAddHandler
{
    Task<OneOf<AddResponse, NotFound, Conflict>> AddMovie(int movieId, string userId);
    
    Task<OneOf<AddResponse, NotFound, Conflict>> AddShow(int showId, string userId);
}

public record AddResponse(string Title);

[SuppressMessage("Usage", "EF1001:Internal EF Core API usage.")]
public class AddHandler(ILogger<AddHandler> logger, IDbContextFactory<ApplicationDbContext> dbContextFactory, ITheMovieDatabaseService tmdbService) : IAddHandler
{
    public async Task<OneOf<AddResponse, NotFound, Conflict>> AddMovie(int movieId, string userId)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        
        var movie = await dbContext.Movies
            .TagWithCallSite()
            .AsNoTracking()
            .Where(m => m.MovieId == movieId)
            .FirstOrDefaultAsync();
        
        if (movie is null)
        {
            var tmdbMovie = await tmdbService.GetMovie(movieId, null);
            if (tmdbMovie is null)
            {
                logger.LogError("Movie with id {MovieId} not found on themoviedb.org", movieId);
                return new NotFound();
            }

            movie = new Movie
            {
                MovieId = tmdbMovie.Id,
                OriginalTitle = tmdbMovie.OriginalTitle,
                OriginalLanguage = tmdbMovie.OriginalLanguage,
                ImdbId = tmdbMovie.ImdbId,
                ReleaseDate = null,
                RunTime = tmdbMovie.RunTime,
                ETag = tmdbMovie.ETag,
            };
            
            if (tmdbMovie.ReleaseDate is not null)
            {
                movie.ReleaseDate =DateTime.SpecifyKind(tmdbMovie.ReleaseDate.Value, DateTimeKind.Utc).Date;
            }

            dbContext.Movies.Add(movie);
            
            await DownloadMoviePoster(tmdbMovie);
        }
        
        var userMovie = await dbContext.UserMovies
            .TagWithCallSite()
            .AsNoTracking()
            .Where(x => x.ApplicationUserId == userId && x.MovieId == movieId)
            .FirstOrDefaultAsync();

        if (userMovie is not null)
        {
            logger.LogError("User already has movie with id {Id}", movieId);
            return new Conflict();
        }

        userMovie = new UserMovie
        {
            MovieId = movieId,
            ApplicationUserId = userId
        };

        dbContext.UserMovies.Add(userMovie);

        await dbContext.SaveChangesAsync();
        
        logger.LogInformation("Added movie with id {Id}", movieId);

        return new AddResponse(movie.OriginalTitle);
    }

    public async Task<OneOf<AddResponse, NotFound, Conflict>> AddShow(int showId, string userId)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        var show = await dbContext.Shows.TagWithCallSite()
            .AsNoTracking()
            .Include(x => x.Episodes)
            .Where(x => x.ShowId == showId)
            .FirstOrDefaultAsync();

        if (show is null)
        {
            var tmdbShow = await tmdbService.GetShow(showId, null);
            if (tmdbShow is null)
            {
                logger.LogError("TV show with id {ShowId} not found on themoviedb.org", showId);
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
            }

            dbContext.Shows.Add(show);

            await DownloadPoster(tmdbShow);
        }
        
        var userShow = await dbContext.UserShows
            .TagWithCallSite()
            .AsNoTracking()
            .Where(s => s.ShowId == showId && s.ApplicationUserId == userId)
            .FirstOrDefaultAsync();

        if (userShow is not null)
        {
            logger.LogError("User already has series with id {Id}", showId);
            return new Conflict();
        }

        userShow = new UserShow()
        {
            ShowId = showId,
            ApplicationUserId = userId
        };
        
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

        dbContext.UserShows.Add(userShow);

        await dbContext.SaveChangesAsync();

        return new AddResponse(show.OriginalTitle);
    }
    
    private async Task DownloadMoviePoster(TMDbMovie tmdbMovie)
    {
        // TODO: Move to shared handler for poster downloads
        var posterUrl = await tmdbService.GetImageUrl(tmdbMovie.PosterPath);
        if (string.IsNullOrWhiteSpace(posterUrl))
        {
            return;
        }

        var stream = new MemoryStream();
        await tmdbService.DownloadImageUrlToStream(posterUrl, stream);

        var filename = ImagePath.MoviePath(tmdbMovie.Id);
        if (File.Exists(filename))
        {
            File.Delete(filename);
        }

        try
        {
            await using var fileStream = new FileStream(filename, FileMode.Create);
            await stream.CopyToAsync(fileStream);
        }
        catch (Exception e)
        {
            logger.LogError("Error saving movie poster: {Error}", e.Message);
        }
    }
    
    private async Task DownloadPoster(TMDbShow tmdbShow)
    {
        // TODO: Move to shared handler for poster downloads
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

        try
        {
            await using var fileStream = new FileStream(filename, FileMode.Create);
            await stream.CopyToAsync(fileStream);
        }
        catch (Exception e)
        {
            logger.LogError("Error saving series poster: {Error}", e.Message);
        }
    }  
}