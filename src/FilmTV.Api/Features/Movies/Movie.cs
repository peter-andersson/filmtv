// ReSharper disable EntityFramework.ModelValidation.UnlimitedStringLength

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FilmTV.Api.Features.Movies;

/// <summary>
/// Shared data for a movie.
/// </summary>
public class Movie
{
    public int MovieId { get; init; }

    public string ImdbId { get; set; } = string.Empty;

    public string OriginalTitle { get; set; } = string.Empty;

    public string OriginalLanguage { get; set; } = string.Empty;

    public DateTime? ReleaseDate { get; set; }

    public int? RunTime { get; set; }
    
    public string ETag { get; set; } = string.Empty;
}

public class MovieConfiguration : IEntityTypeConfiguration<Movie>
{
    public void Configure(EntityTypeBuilder<Movie> builder)
    {
        builder.Property(m => m.MovieId)
            .ValueGeneratedNever();

        builder.Property(m => m.ImdbId)
            .HasMaxLength(20);

        builder.Property(m => m.OriginalTitle)
            .HasMaxLength(256);

        builder.Property(m => m.OriginalLanguage)
            .HasMaxLength(10);

        builder.Property(m => m.ETag)
            .HasMaxLength(100);

        builder.HasKey(m => m.MovieId);
    }
}