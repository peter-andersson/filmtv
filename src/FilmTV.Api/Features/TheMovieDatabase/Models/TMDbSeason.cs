using System.Text.Json.Serialization;

namespace FilmTV.Api.Features.TheMovieDatabase.Models;

// ReSharper disable once InconsistentNaming
public sealed class TMDbSeason
{
    [JsonPropertyName("season_number")]
    public int SeasonNumber { get; init; }

    [JsonPropertyName("episodes")]
    public List<TMDbEpisode> Episodes { get; set; } = [];
}
