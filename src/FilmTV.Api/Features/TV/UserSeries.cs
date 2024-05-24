using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FilmTV.Api.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FilmTV.Api.Features.TV;

public class UserSeries
{
    public int SeriesId { get; set; }
    public Series Series { get; set; } = null!;
    
    public string? Title { get; set; }
    
    public int Rating { get; set; }
    
    public DateTime? RatingDate { get; set; }
    
    public string UserId { get; init; } = null!;
    public AppUser User { get; init; } = null!;    
    
    public ICollection<UserEpisode> Episodes { get; set; } = [];
}

public class UserSeriesConfiguration : IEntityTypeConfiguration<UserSeries>
{
    public void Configure(EntityTypeBuilder<UserSeries> builder)
    {
        builder.Property(m => m.SeriesId)
            .ValueGeneratedNever();

        builder.Property(m => m.Title)
            .HasMaxLength(256);

        builder.Property(m => m.UserId)
            .HasMaxLength(50);

        builder.HasOne(m => m.Series)
            .WithMany()
            .HasForeignKey(m => m.SeriesId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(m => m.User)
            .WithMany()
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.Cascade);        
            
        builder.HasKey(x => new { x.SeriesId, x.UserId });
    }
}