using System.Text.Json.Serialization;

namespace FilmTV.Api.Features.TheMovieDatabase;

// ReSharper disable once InconsistentNaming
public sealed class TMDbSeason
{
    [JsonPropertyName("season_number")]
    public int SeasonNumber { get; init; }

    [JsonPropertyName("episodes")]
    public List<TMDbEpisode> Episodes { get; set; } = [];
}
