using System.ComponentModel.DataAnnotations;
using FilmTV.Api.Common.Models;

namespace FilmTV.Api.Features.Movies.Models;

public class UserMovie
{
    public int Id { get; init; }

    [MaxLength(256)]
    public string? Title { get; set; }

    public DateTime? WatchedDate { get; set; }

    public int Rating { get; set; }

    public DateTime? RatingDate { get; set; }

    [MaxLength(50)]
    public string UserId { get; init; } = null!;
    public AppUser User { get; init; } = null!;

    public int MovieId { get; init; }
    public Movie Movie { get; init; } = null!;
}