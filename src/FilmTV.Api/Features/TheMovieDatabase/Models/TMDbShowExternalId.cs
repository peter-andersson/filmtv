using System.Text.Json.Serialization;

namespace FilmTV.Api.Features.TheMovieDatabase.Models;

// ReSharper disable once InconsistentNaming
public sealed class TMDbShowExternalId
{
    // ReSharper disable once StringLiteralTypo
    [JsonPropertyName("tvdb_id")]
    public int? TvDbId { get; init; }

    [JsonPropertyName("imdb_id")] 
    public string ImdbId { get; init; } = string.Empty;
}