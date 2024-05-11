using System.Text.Json.Serialization;

namespace FilmTV.Api.Features.TheMovieDatabase.Models;

public sealed class Images
{
    [JsonPropertyName("secure_base_url")]
    public string? SecureBaseUrl { get; set; } = string.Empty;

    [JsonPropertyName("backdrop_sizes")]
    public List<string> BackdropSizes { get; set; } = [];

    [JsonPropertyName("logo_sizes")]
    public List<string> LogoSizes { get; set; } = [];

    [JsonPropertyName("poster_sizes")]
    public List<string> PosterSizes { get; set; } = [];

    [JsonPropertyName("profile_sizes")]
    public List<string> ProfileSizes { get; set; } = [];

    [JsonPropertyName("still_sizes")]
    public List<string> StillSizes { get; set; } = [];
}