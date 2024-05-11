namespace FilmTV.Api.Features.Movies;

public record MovieResponse(
    int Id,
    string? Title,
    string OriginalTitle,
    string OriginalLanguage,
    DateTime? WatchedDate,
    int Rating,
    DateTime? ReleaseDate,
    int? RunTime);