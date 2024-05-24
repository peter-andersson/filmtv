using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FilmTV.Api.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FilmTV.Api.Features.TV;

public class UserEpisode
{
    public int EpisodeId { get; set; }
    public Episode Episode { get; set; } = null!;
    
    public bool Watched { get; set; }
    
    [MaxLength(50)]
    public string UserId { get; init; } = null!;
    public AppUser User { get; init; } = null!;  
    
    public int SeriesId { get; set; }
    public UserSeries UserSeries { get; set; } = null!;    
}

public class UserEpisodeConfiguration : IEntityTypeConfiguration<UserEpisode>
{
    public void Configure(EntityTypeBuilder<UserEpisode> builder)
    {
        builder.Property(m => m.EpisodeId)
            .ValueGeneratedNever();

        builder.HasKey(x => new { x.EpisodeId, x.UserId });        
        
        builder.Property(m => m.UserId)
            .HasMaxLength(50);

        builder.HasOne(m => m.UserSeries)
            .WithMany()
            .HasForeignKey(m => new {m.SeriesId, m.UserId})
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(m => m.User)
            .WithMany()
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.Cascade);        
    }
}