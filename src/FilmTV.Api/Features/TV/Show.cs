using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FilmTV.Api.Features.TV;

public class Show
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }
    
    public string ImdbId { get; set; } = string.Empty;

    public int TvDbId { get; set; } = 0;    
    
    public string OriginalTitle { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string ETag { get; set; } = string.Empty;

    public DateTime? NextUpdate { get; set; }

    public List<Episode> Episodes { get; private set; } = [];
}