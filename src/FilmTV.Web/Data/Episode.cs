using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

// ReSharper disable EntityFramework.ModelValidation.UnlimitedStringLength

namespace FilmTV.Web.Data;

public class Episode
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;
    
    public int SeasonNumber { get; set; }

    public int EpisodeNumber { get; set; }

    public DateTime? AirDate { get; set; }
    
    public int ShowId { get; set; }
    public Show Show { get; set; } = null!;
}

public class EpisodeConfiguration : IEntityTypeConfiguration<Episode>
{
    public void Configure(EntityTypeBuilder<Episode> builder)
    {
        builder.HasKey(x  => x.Id);        
        
        builder.HasOne(m => m.Show)
            .WithMany(s => s.Episodes)
            .HasForeignKey(m => m.ShowId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}