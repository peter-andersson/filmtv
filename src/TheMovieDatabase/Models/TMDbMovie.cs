namespace TheMovieDatabase.Models;

// ReSharper disable once InconsistentNaming
public sealed class TMDbMovie
{
    [JsonPropertyName("id")]
    public int Id { get; init; }

    [JsonPropertyName("imdb_id")]
    public string ImdbId { get; init; } = null!;

    [JsonPropertyName("original_language")]
    public string OriginalLanguage { get; init; } = null!;

    [JsonPropertyName("original_title")]
    public string OriginalTitle { get; init; } = null!;

    [JsonPropertyName("title")]
    public string Title { get; init; } = null!;

    [JsonPropertyName("poster_path")]
    public string? PosterPath { get; init; }

    [JsonPropertyName("runtime")]
    public int? RunTime { get; init; }

    [JsonPropertyName("release_date")]
    [JsonConverter(typeof(DateTimeConverter))]
    public DateTime? ReleaseDate { get; init; }

    public string ETag { get; set; } = string.Empty;
}