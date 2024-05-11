using System.Text.Json.Serialization;

namespace FilmTV.Api.Features.TheMovieDatabase;

public sealed class Configuration
{
    [JsonPropertyName("images")]
    public Images Images { get; init; } = new Images();
}
