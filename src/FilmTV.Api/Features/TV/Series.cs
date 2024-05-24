using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FilmTV.Api.Features.TV;

public class Series
{
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

public class SeriesConfiguration : IEntityTypeConfiguration<Series>
{
    public void Configure(EntityTypeBuilder<Series> builder)
    {
        builder.Property(m => m.Id)
            .ValueGeneratedNever();
        
        builder.HasKey(x  => x.Id);        
        
        builder.Property(m => m.ImdbId).HasMaxLength(50);
        builder.Property(m => m.OriginalTitle).HasMaxLength(1024);
        builder.Property(m => m.Status).HasMaxLength(50);
        builder.Property(m => m.ETag).HasMaxLength(100);
    }
}