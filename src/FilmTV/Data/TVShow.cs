using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FilmTV.Data;

public class TVShow
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }
    
    [MaxLength(512)]
    public string OriginalTitle { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Status { get; set; } = string.Empty;

    [MaxLength(100)]
    public string ETag { get; set; } = string.Empty;

    public DateTime? NextUpdate { get; set; }

    [MaxLength(100)] 
    public string ImdbId { get; set; } = string.Empty;

    public int TvDbId { get; set; }
    
    public List<Episode> Episodes { get; private set; } = [];
}