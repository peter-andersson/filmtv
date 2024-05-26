using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
// ReSharper disable EntityFramework.ModelValidation.UnlimitedStringLength

namespace FilmTV.Web.Data;

public class UserShow
{
    public int UserShowId { get; init; }    
    
    public int ShowId { get; set; }
    public Show Show { get; set; } = null!;
    
    public string? Title { get; set; }
    
    public int Rating { get; set; }
    
    public DateTime? RatingDate { get; set; }
    
    public string ApplicationUserId { get; init; } = null!;
    public ApplicationUser ApplicationUser { get; init; } = null!;
    
    public ICollection<UserEpisode> UserEpisodes { get; set; } = [];
}

public class UserSeriesConfiguration : IEntityTypeConfiguration<UserShow>
{
    public void Configure(EntityTypeBuilder<UserShow> builder)
    {
        builder.HasKey(x => x.UserShowId);

        builder.HasOne(m => m.Show)
            .WithMany()
            .HasForeignKey(m => m.ShowId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(m => m.ApplicationUser)
            .WithMany(x => x.Shows)
            .HasForeignKey(m => m.ApplicationUserId)
            .OnDelete(DeleteBehavior.Cascade);        
            
    }
}