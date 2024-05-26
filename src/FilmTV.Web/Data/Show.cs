using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

// ReSharper disable EntityFramework.ModelValidation.UnlimitedStringLength

namespace FilmTV.Web.Data;

public class Show
{
    public int ShowId { get; set; }
    
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

public class SeriesConfiguration : IEntityTypeConfiguration<Show>
{
    public void Configure(EntityTypeBuilder<Show> builder)
    {
        builder.Property(m => m.ShowId)
            .ValueGeneratedNever();
        
        builder.HasKey(x  => x.ShowId);        
    }
}