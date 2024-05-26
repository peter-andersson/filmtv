using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FilmTV.Web.Data;

public class UserEpisode
{
    public int UserEpisodeId { get; set; }

    public int EpisodeId { get; set; }
    public Episode Episode { get; set; } = null!;
    
    public bool Watched { get; set; }
    
    public int UserShowId { get; set; }
    public UserShow UserShow { get; set; } = null!;    
}

public class UserEpisodeConfiguration : IEntityTypeConfiguration<UserEpisode>
{
    public void Configure(EntityTypeBuilder<UserEpisode> builder)
    {
        builder.HasKey(x => x.UserEpisodeId);        
        
        builder.HasOne(x => x.UserShow)
            .WithMany(x => x.UserEpisodes)
            .HasForeignKey(x => x.UserShowId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}