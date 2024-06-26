using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

// ReSharper disable EntityFramework.ModelValidation.UnlimitedStringLength

namespace FilmTV.Web.Data;

public class UserMovie
{
    public int UserMovieId { get; init; }

    public int MovieId { get; init; }
    public Movie Movie { get; init; } = null!;

    public string? Title { get; set; }

    public DateTime? WatchedDate { get; set; }

    public int? Rating { get; set; }

    public DateTime? RatedAt { get; set; }

    public string ApplicationUserId { get; init; } = null!;
    public ApplicationUser ApplicationUser { get; init; } = null!;
}

public class UserMovieConfiguration : IEntityTypeConfiguration<UserMovie>
{
    public void Configure(EntityTypeBuilder<UserMovie> builder)
    {
        builder.HasKey(x => x.UserMovieId);
        
        builder.HasOne(m => m.Movie)
            .WithMany(x => x.Users)
            .HasForeignKey(x => x.MovieId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(m => m.ApplicationUser)
            .WithMany(x => x.Movies)
            .HasForeignKey(m => m.ApplicationUserId)
            .OnDelete(DeleteBehavior.Cascade);        
    }
}