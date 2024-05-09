using FilmTV.Api.Common.Models;

namespace FilmTV.Api.Features.Movies.Models;

public class UserMovie
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public DateTime? WatchedDate { get; set; }

    public int Rating { get; set; } = 0;

    public DateTime? RatingDate { get; set; }

    public string UserId { get; set; } = null!;
    public AppUser User { get; set; } = null!;

    public int MovieId { get; set; }
    public Movie Movie { get; set; } = null!;
}