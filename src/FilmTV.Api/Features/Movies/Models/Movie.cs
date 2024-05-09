using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FilmTV.Api.Features.Movies.Models;

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

    public DateTime? ReleaseDate { get; init; }

    public int? RunTime { get; init; }

    [MaxLength(100)]
    public string ETag { get; set; } = string.Empty;

    public ICollection<UserMovie> Users { get; } = new List<UserMovie>();
}