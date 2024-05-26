using System.Text.Json.Serialization;

namespace FilmTV.Api.Features.TheMovieDatabase;

// ReSharper disable once InconsistentNaming
public sealed class TMDbShow
{
    [JsonPropertyName("id")]
    public int Id { get; init; }

    [JsonPropertyName("status")]
    public string Status { get; init; } = string.Empty;

    [JsonPropertyName("original_name")]
    public string Title { get; init; } = string.Empty;

    [JsonPropertyName("poster_path")]
    public string? PosterPath { get; init; }

    public string ETag { get; set; } = string.Empty;

    [JsonPropertyName("seasons")]
    public List<TMDbSeason> Seasons { get; init; } = [];

    public string ImdbId { get; set; } = string.Empty;

    public int TvDbId { get; set; }
}