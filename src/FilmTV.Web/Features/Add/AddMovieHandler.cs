using FilmTV.Web.Common;
using FilmTV.Web.Data;
using FilmTV.Web.Features.Posters;
using FilmTV.Web.Features.TheMovieDatabase;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace FilmTV.Web.Features.Add;

public interface IAddMovie
{
    Task<OneOf<AddResponse, NotFound, Conflict>> AddMovie(int movieId, string userId);
}

public class AddMovieHandler(
    ILogger<AddMovieHandler> logger,
    IDbContextFactory<ApplicationDbContext> dbContextFactory,
    ITheMovieDatabaseService tmdbService,
    IPosterDownload posterDownload
    ) : IAddMovie
{
    private readonly ILogger<AddMovieHandler> _logger = logger;
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory = dbContextFactory;
    private readonly ITheMovieDatabaseService _tmdbService = tmdbService;
    private readonly IPosterDownload _posterDownload = posterDownload;
    
    public async Task<OneOf<AddResponse, NotFound, Conflict>> AddMovie(int movieId, string userId)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        
        var movie = await dbContext.Movies
            .TagWithCallSite()
            .AsNoTracking()
            .Where(m => m.MovieId == movieId)
            .FirstOrDefaultAsync();
        
        if (movie is null)
        {
            var tmdbMovie = await _tmdbService.GetMovie(movieId, null);
            if (tmdbMovie is null)
            {
                _logger.LogError("Movie with id {MovieId} not found on themoviedb.org", movieId);
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
            
            await _posterDownload.Download(tmdbMovie.PosterPath, ImagePath.MovieFilename(tmdbMovie.Id));
        }
        
        var userMovie = await dbContext.UserMovies
            .TagWithCallSite()
            .AsNoTracking()
            .Where(x => x.ApplicationUserId == userId && x.MovieId == movieId)
            .FirstOrDefaultAsync();

        if (userMovie is not null)
        {
            _logger.LogError("User already has movie with id {Id}", movieId);
            return new Conflict();
        }

        userMovie = new UserMovie
        {
            MovieId = movieId,
            ApplicationUserId = userId
        };

        dbContext.UserMovies.Add(userMovie);

        await dbContext.SaveChangesAsync();
        
        _logger.LogInformation("Added movie with id {Id}", movieId);

        return new AddResponse(movie.OriginalTitle);
    }
}