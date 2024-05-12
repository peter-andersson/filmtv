namespace FilmTV.Api.Features.Movies;

public static class MovieMapper
{
    public static MovieResponse ToDto(this UserMovie userMovie)
    {
        return new MovieResponse(
            userMovie.MovieId,
            userMovie.Title,
            userMovie.Movie.OriginalTitle,
            userMovie.Movie.OriginalLanguage,
            userMovie.WatchedDate,
            userMovie.Rating,
            userMovie.Movie.ReleaseDate,
            userMovie.Movie.RunTime
        );
    }
}