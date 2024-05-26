using System.Text.Json.Serialization;
using FilmTV.Api.Features.TheMovieDatabase;

namespace FilmTV.Web.Features.TheMovieDatabase;

public sealed class Configuration
{
    [JsonPropertyName("images")]
    public Images Images { get; init; } = new Images();
}
