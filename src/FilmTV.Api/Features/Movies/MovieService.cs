using FilmTV.Api.Common;
using FilmTV.Api.Features.TheMovieDatabase;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace FilmTV.Api.Features.Movies;

public interface IMovieService
{
    Task<OneOf<MovieResponse, NotFound, Conflict>> Add(int id, string userId, CancellationToken cancellationToken);
    
    Task Delete(int id, string userId, CancellationToken cancellationToken);
    
    Task<OneOf<MovieResponse, NotFound>> Get(int id, string userId, CancellationToken cancellationToken);

    Task<IEnumerable<WatchlistMovieResponse>> GetWatchlist(string userId, CancellationToken cancellationToken);
    
    Task<OneOf<Success, NotFound>> Refresh(int id, CancellationToken cancellationToken);
    
    Task<OneOf<MovieResponse, NotFound, ValidationError>> Update(int id, string userId, UpdateMovieRequest updateMovie, CancellationToken cancellationToken);
}

public class MovieService(ILogger<MovieService> logger, AppDbContext dbContext, ITheMovieDatabaseService tmdbService) : IMovieService
{
    private static readonly UpdateMovieValidator UpdateValidator = new();
    
    public async Task<OneOf<MovieResponse, NotFound, Conflict>> Add(int id, string userId, CancellationToken cancellationToken)
    {
        var movie = await dbContext.Movies.Where(m => m.MovieId == id).FirstOrDefaultAsync(cancellationToken);

        if (movie is not null)
        {
            var tmdbMovie = await tmdbService.GetMovie(id, null);
            if (tmdbMovie is null)
            {
                logger.LogError("Movie with id {MovieId} not found on themoviedb.org", id);
                return new NotFound();
            }

            movie = new Movie
            {
                MovieId = tmdbMovie.Id,
                OriginalTitle = tmdbMovie.OriginalTitle,
                OriginalLanguage = tmdbMovie.OriginalLanguage,
                ImdbId = tmdbMovie.ImdbId,
                ReleaseDate = tmdbMovie.ReleaseDate,
                RunTime = tmdbMovie.RunTime,
                ETag = tmdbMovie.ETag
            };

            dbContext.Movies.Add(movie);
            
            await DownloadMoviePoster(tmdbMovie, cancellationToken);
        }

        if (movie is null)
        {
            logger.LogError("Movie with id {MovieId} not found", id);
            return new NotFound();
        }

        var userMovie = await dbContext.UserMovies
            .Where(m => m.MovieId == id && m.UserId == userId)
            .FirstOrDefaultAsync(cancellationToken);

        if (userMovie is not null)
        {
            logger.LogError("User already has movie with id {Id}", id);
            return new Conflict();
        }

        userMovie = new UserMovie
        {
            Movie = movie,
            UserId = userId
        };

        dbContext.UserMovies.Add(userMovie);

        await dbContext.SaveChangesAsync(cancellationToken);
        
        return userMovie.ToDto();
    }
    
    public async Task Delete(int id, string userId, CancellationToken cancellationToken)
    {
        await dbContext.UserMovies
             .Where(m => m.MovieId == id && m.UserId == userId)
             .ExecuteDeleteAsync(cancellationToken);
    }

    public async Task<OneOf<MovieResponse, NotFound>> Get(int id, string userId, CancellationToken cancellationToken)
    {
         var userMovie = await dbContext.UserMovies
             .Where(m => m.MovieId == id).Include(m => m.Movie)
             .FirstOrDefaultAsync(cancellationToken);

         if (userMovie is null)
         {
             return new NotFound();
         }
         
         return userMovie.ToDto();
    }
    
    public async Task<IEnumerable<WatchlistMovieResponse>> GetWatchlist(string userId, CancellationToken cancellationToken)
    {
        var movies = await dbContext.UserMovies
            .Where(m => m.UserId == userId && m.WatchedDate == null)
            .Include(m => m.Movie)
            .ToListAsync(cancellationToken);

        return movies.Select(userMovie => new WatchlistMovieResponse(userMovie.MovieId, userMovie.Title ?? userMovie.Movie.OriginalTitle));
    }

    public async Task<OneOf<Success, NotFound>> Refresh(int id, CancellationToken cancellationToken)
    {
        var movie = await dbContext.Movies.AsTracking().Where(m => m.MovieId == id).FirstOrDefaultAsync(cancellationToken);

         if (movie is null)
         {
             return new NotFound();
         }

         var tmdbMovie = await tmdbService.GetMovie(id, movie.ETag);
         if (tmdbMovie is null)
         {
             return new Success();
         }
         
         await DownloadMoviePoster(tmdbMovie, cancellationToken);

         movie.OriginalTitle = tmdbMovie.OriginalTitle;
         movie.OriginalLanguage = tmdbMovie.OriginalLanguage;
         movie.ImdbId = tmdbMovie.ImdbId;
         movie.ReleaseDate = tmdbMovie.ReleaseDate;
         movie.RunTime = tmdbMovie.RunTime;
         movie.ETag = tmdbMovie.ETag;

         await dbContext.SaveChangesAsync(cancellationToken);
         
         return new Success();
    }
    
    public async Task<OneOf<MovieResponse, NotFound, ValidationError>> Update(int id, string userId, UpdateMovieRequest updateMovie, CancellationToken cancellationToken)
    {
            var validationResult = await UpdateValidator.ValidateAsync(updateMovie, cancellationToken);

            if (!validationResult.IsValid)
            {
                return new ValidationError(validationResult.ToDictionary());
            }
         
            var userMovie = await dbContext.UserMovies
             .AsTracking()
             .Include(m => m.Movie)
             .Where(m => m.MovieId == id && m.UserId == userId)
             .FirstOrDefaultAsync(cancellationToken);

         if (userMovie is null)
         {
             return new NotFound();
         }
         
         userMovie.Title = updateMovie.Title;
         userMovie.WatchedDate = updateMovie.WatchedDate;
         if (userMovie.Rating != updateMovie.Rating)
         {
             userMovie.Rating = updateMovie.Rating;
             userMovie.RatingDate = DateTime.UtcNow;
         }
         
         await dbContext.SaveChangesAsync(cancellationToken);

         return userMovie.ToDto();
    }

    private async Task DownloadMoviePoster(TMDbMovie tmdbMovie, CancellationToken cancellationToken)
    {
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

        await using var fileStream = new FileStream(filename, FileMode.Create);
        await stream.CopyToAsync(fileStream, cancellationToken);
    }

    private class UpdateMovieValidator : AbstractValidator<UpdateMovieRequest>
    {
        public UpdateMovieValidator()
        {
            RuleFor(x => x.MovieId).GreaterThanOrEqualTo(1);
            RuleFor(x => x.Title).MaximumLength(256);
            RuleFor(x => x.Rating).LessThanOrEqualTo(10).GreaterThanOrEqualTo(0);
        }
    }
}