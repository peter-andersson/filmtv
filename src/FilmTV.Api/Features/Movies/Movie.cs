using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FilmTV.Api.Features.Movies;

/// <summary>
/// Shared data for a movie.
/// </summary>
public class Movie
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int MovieId { get; init; }

    [MaxLength(20)]
    public string ImdbId { get; set; } = string.Empty;

    [MaxLength(256)]
    public string OriginalTitle { get; set; } = string.Empty;

    [MaxLength(10)]
    public string OriginalLanguage { get; set; } = string.Empty;

    public DateTime? ReleaseDate { get; set; }

    public int? RunTime { get; set; }

    [MaxLength(100)]
    public string ETag { get; set; } = string.Empty;
}