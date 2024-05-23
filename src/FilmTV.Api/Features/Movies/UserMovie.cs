using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FilmTV.Api.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
// ReSharper disable EntityFramework.ModelValidation.UnlimitedStringLength

namespace FilmTV.Api.Features.Movies;

/// <summary>
/// User specific data for a movie.
/// </summary>
public class UserMovie
{
    public int MovieId { get; init; }
    public Movie Movie { get; init; } = null!;

    public string? Title { get; set; }

    public DateTime? WatchedDate { get; set; }

    public int Rating { get; set; }

    public DateTime? RatingDate { get; set; }

    public string UserId { get; init; } = null!;
    public AppUser User { get; init; } = null!;
}

public class UserMovieConfiguration : IEntityTypeConfiguration<UserMovie>
{
    public void Configure(EntityTypeBuilder<UserMovie> builder)
    {
        builder.Property(m => m.MovieId)
            .ValueGeneratedNever();

        builder.Property(m => m.Title)
            .HasMaxLength(256);

        builder.Property(m => m.UserId)
            .HasMaxLength(50);

        builder.HasOne(m => m.Movie)
            .WithMany()
            .HasForeignKey(m => m.MovieId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(m => m.User)
            .WithMany()
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.Cascade);        
            
        builder.HasKey(x => new { x.MovieId, x.UserId });
    }
}