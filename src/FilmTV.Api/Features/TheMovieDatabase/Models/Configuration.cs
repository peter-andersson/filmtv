using System.Text.Json.Serialization;

namespace FilmTV.Api.Features.TheMovieDatabase.Models;

public sealed class Configuration
{
    [JsonPropertyName("images")]
    public Images Images { get; init; } = new Images();
}
