using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

// ReSharper disable EntityFramework.ModelValidation.UnlimitedStringLength

namespace FilmTV.Web.Data;

public class Movie
{
    public int MovieId { get; init; }

    public string ImdbId { get; init; } = string.Empty;

    public string OriginalTitle { get; init; } = string.Empty;

    public string OriginalLanguage { get; init; } = string.Empty;

    public DateTime? ReleaseDate { get; set; }

    public int? RunTime { get; init; }
    
    public string ETag { get; init; } = string.Empty;

    public ICollection<UserMovie> Users { get; set; } = [];
}

public class MovieConfiguration : IEntityTypeConfiguration<Movie>
{
    public void Configure(EntityTypeBuilder<Movie> builder)
    {
        builder.Property(m => m.MovieId)
            .ValueGeneratedNever();
        
        builder.HasKey(m => m.MovieId);
    }
}