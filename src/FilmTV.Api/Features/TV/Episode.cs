using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FilmTV.Api.Features.TV;

public class Episode
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;
    
    public int SeasonNumber { get; set; }

    public int EpisodeNumber { get; set; }

    public DateTime? AirDate { get; set; }
    
    public int SeriesId { get; set; }
    public Series Series { get; set; } = null!;
}

public class EpisodeConfiguration : IEntityTypeConfiguration<Episode>
{
    public void Configure(EntityTypeBuilder<Episode> builder)
    {
        builder.HasKey(x  => x.Id);        
        
        builder.Property(m => m.Title)
            .HasMaxLength(256);

        builder.HasOne(m => m.Series)
            .WithMany(s => s.Episodes)
            .HasForeignKey(m => m.SeriesId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}