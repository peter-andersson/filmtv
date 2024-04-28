namespace TheMovieDatabase.Models;

public sealed class Configuration
{
    [JsonPropertyName("images")]
    public Images Images { get; init; } = new Images();
}
