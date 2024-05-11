namespace FilmTV.Api.Features.Movies;

public record UpdateMovieRequest(int MovieId, string? Title, DateTime? WatchedDate, int Rating);