using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FilmTV.Data;

public class Movie
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }

    [MaxLength(100)]
    public string ImdbId { get; set; } = string.Empty;

    [MaxLength(512)]
    public string OriginalTitle { get; set; } = string.Empty;

    [MaxLength(100)]
    public string OriginalLanguage { get; set; } = string.Empty;
    
    public DateTime? ReleaseDate { get; set; }
    
    public int? RunTime { get; set; }

    [MaxLength(100)]
    public string ETag { get; set; } = string.Empty;
    

}