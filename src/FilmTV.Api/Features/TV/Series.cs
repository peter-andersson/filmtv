using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FilmTV.Api.Features.TV;

public class Series
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }
    
    public string ImdbId { get; set; } = string.Empty;

    public int TvDbId { get; set; } = 0;    
    
    public string OriginalTitle { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string ETag { get; set; } = string.Empty;

    public DateTime? NextUpdate { get; set; }

    public ICollection<Episode> Episodes { get; private set; } = [];
    
    public void SetNextUpdate()
    {
        NextUpdate = Status.ToUpperInvariant() switch
        {
            "RETURNING SERIES" => DateTime.UtcNow.AddDays(1),
            "PLANNED" or "PILOT" or "IN PRODUCTION" => DateTime.UtcNow.AddDays(7),
            "ENDED" or "CANCELED" => DateTime.UtcNow.AddMonths(1),
            _ => DateTime.UtcNow.AddMonths(1),
        };
    }
}