using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FilmTV.Api.Features.Movies.Models;

public class Movie
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int MovieId { get; set; }

    public string ImdbId { get; set; } = string.Empty;

    public string OriginalTitle { get; set; } = string.Empty;

    public string OriginalLanguage { get; set; } = string.Empty;

    public DateTime? ReleaseDate { get; set; }

    public int? RunTime { get; set; }

    public string ETag { get; set; } = string.Empty;

    public ICollection<UserMovie> Users { get; } = new List<UserMovie>();
}