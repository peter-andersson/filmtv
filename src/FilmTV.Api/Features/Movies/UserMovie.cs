using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FilmTV.Api.Common;

namespace FilmTV.Api.Features.Movies;

/// <summary>
/// User specific data for a movie.
/// </summary>
public class UserMovie
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int MovieId { get; init; }
    [ForeignKey(nameof(MovieId))]
    public Movie Movie { get; init; } = null!;

    [MaxLength(256)]
    public string? Title { get; set; }

    public DateTime? WatchedDate { get; set; }

    public int Rating { get; set; }

    public DateTime? RatingDate { get; set; }

    [MaxLength(50)]
    public string UserId { get; init; } = null!;
    public AppUser User { get; init; } = null!;
}