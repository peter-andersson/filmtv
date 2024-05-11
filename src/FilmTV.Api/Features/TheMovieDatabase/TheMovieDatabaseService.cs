using System.Net.Http.Headers;
using System.Text.Json;

namespace FilmTV.Api.Features.TheMovieDatabase;

public interface ITheMovieDatabaseService
{
    Task<string> GetImageUrl(string? path);

    Task<TMDbMovie?> GetMovie(int id, string? etag);

    Task<TMDbShow?> GetShow(int id, string? etag);

    Task<bool> DownloadImageUrlToStream(string url, Stream stream);
}

public sealed class TheMovieDatabaseService : ITheMovieDatabaseService
{
    private const string BaseUrl = "https://api.themoviedb.org/3";
    
    private readonly ILogger<TheMovieDatabaseService> _logger;
    private readonly string _apiKey;
    private readonly IHttpClientFactory _httpClientFactory;
    private Configuration? _configuration;

    public TheMovieDatabaseService(ILogger<TheMovieDatabaseService> logger, IConfiguration configuration, IHttpClientFactory httpClientFactoryFactory)
    {
        _logger = logger;
        _apiKey = configuration["TheMovieDbAccessToken"] ?? string.Empty;
        _httpClientFactory = httpClientFactoryFactory;
    }

    public async Task<TMDbMovie?> GetMovie(int id, string? etag)
    {
        _logger.LogTrace("GetMovieAsync {id}, {etag}", id, etag);
        if (string.IsNullOrWhiteSpace(_apiKey))
        {
            _logger.LogCritical("Missing API key");
            return null;
        }

        using var request = new HttpRequestMessage(HttpMethod.Get, GetMovieUrl(id));
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

        if (!string.IsNullOrWhiteSpace(etag))
        {
            if (EntityTagHeaderValue.TryParse(etag, out var etagHeaderValue))
            {
                request.Headers.IfNoneMatch.Add(etagHeaderValue);
            }
        }

        try
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.SendAsync(request);

            if (response.StatusCode == System.Net.HttpStatusCode.NotModified)
            {
                return null;
            }

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrWhiteSpace(content))
                {
                    _logger.LogError("GetMovieAsync - No data in result");
                    return null;
                }

                var movie = JsonSerializer.Deserialize<TMDbMovie>(content);

                if (movie is null)
                {
                    _logger.LogError("GetMovieAsync - No movie in result");
                    return null;
                }

                if (response.Headers.ETag != null &&  !string.IsNullOrWhiteSpace(response.Headers.ETag.Tag))
                {
                    movie.ETag = response.Headers.ETag.Tag;
                }

                return movie;
            }
            else
            {
                _logger.LogError("GetMovieAsync - Http call failed with status code {StatusCode}", response.StatusCode);
                return null;
            }
        }
        catch (TimeoutException)
        {
            _logger.LogError("GetMovieAsync - Timeout trying to get movie.");
            return null;
        }
    }

    public async Task<TMDbShow?> GetShow(int id, string? etag)
    {
        _logger.LogTrace("GetShowAsync {id}, {etag}", id, etag);

        if (string.IsNullOrWhiteSpace(_apiKey))
        {
            _logger.LogCritical("Missing API key");
            return null;
        }

        using var request = new HttpRequestMessage(HttpMethod.Get, GetTvShowUrl(id));
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

        if (!string.IsNullOrWhiteSpace(etag))
        {
            if (EntityTagHeaderValue.TryParse(etag, out var etagHeaderValue))
            {
                request.Headers.IfNoneMatch.Add(etagHeaderValue);
            }
        }

        try
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.SendAsync(request);

            if (response.StatusCode == System.Net.HttpStatusCode.NotModified)
            {
                return null;
            }

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(content))
                {
                    _logger.LogError("GetShowAsync - No data in result");
                    return null;
                }

                var show = JsonSerializer.Deserialize<TMDbShow>(content);

                if (show is null)
                {
                    _logger.LogError("GetShowAsync - No show in result");
                    return null;
                }

                if (response.Headers.ETag != null &&
                    !string.IsNullOrWhiteSpace(response.Headers.ETag.Tag))
                {
                    show.ETag = response.Headers.ETag.Tag;
                }

                foreach (var season in show.Seasons)
                {
                    var seasonData = await GetSeasonAsync(show.Id, season.SeasonNumber);

                    if (seasonData is not null)
                    {
                        season.Episodes = seasonData.Episodes;
                    }
                }

                var externalId = await GetExternalIdsAsync(show.Id);
                if (externalId is null)
                {
                    return show;
                }
                
                show.ImdbId = externalId.ImdbId;
                show.TvDbId = externalId.TvDbId ?? 0;

                return show;
            }
            else
            {
                _logger.LogError("GetShowAsync - Http call failed with status code {StatusCode}", response.StatusCode);
                return null;
            }
        }
        catch (TimeoutException)
        {
            _logger.LogError("GetShowAsync - Timeout trying to get show.");
            return null;
        }
    }

    public async Task<string> GetImageUrl(string? path)
    {
        _logger.LogTrace("GetImageUrlAsync {path}", path);

        if (string.IsNullOrWhiteSpace(_apiKey))
        {
            _logger.LogCritical("Missing API key");
            return string.Empty;
        }

        if (string.IsNullOrWhiteSpace(path))
        {
            _logger.LogError("Path shouldn't be empty");
            return string.Empty;
        }

        var configuration = await GetConfigurationAsync();

        if (configuration is null)
        {
            return string.Empty;
        }

        var imageUrl = $"{configuration.Images.SecureBaseUrl}w185{path}";
        _logger.LogInformation("ImageUrl {imageUrl}", imageUrl);
        return imageUrl;
    }

    private async Task<TMDbSeason?> GetSeasonAsync(int showId, int seasonNumber)
    {
        _logger.LogInformation("GetSeasonAsync {showId}, {seasonNumber}", showId, seasonNumber);

        using HttpRequestMessage request = new(HttpMethod.Get, GetTvSeasonUrl(showId, seasonNumber));
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

        try
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(content))
                {
                    return JsonSerializer.Deserialize<TMDbSeason>(content);
                }

                _logger.LogError("GetSeasonAsync - No Contents in response");
                return null;

            }
            else
            {
                _logger.LogError("GetSeasonAsync - Http call failed with status code {StatusCode}", response.StatusCode);
                return null;
            }
        }
        catch (TimeoutException)
        {
            _logger.LogError("GetSeasonAsync - Timeout trying to get season.");
            return null;
        }
    }

    private async Task<TMDbShowExternalId?> GetExternalIdsAsync(int showId)
    {
        _logger.LogInformation("GetExternalIds {showId}", showId);

        using HttpRequestMessage request = new(HttpMethod.Get, GetExternalUrl(showId));
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

        try
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(content))
                {
                    return JsonSerializer.Deserialize<TMDbShowExternalId>(content);
                }
                
                _logger.LogError("GetExternalIds - No Contents in response");
                return null;

            }
            else
            {
                _logger.LogError("GetExternalIds - Http call failed with status code {StatusCode}", response.StatusCode);
                return null;
            }
        }
        catch (TimeoutException)
        {
            _logger.LogError("GetExternalIds - Timeout trying to get external ids.");
            return null;
        }
    }

    private static string GetTvShowUrl(int id) => $"{BaseUrl}/tv/{id}";

    private static string GetTvSeasonUrl(int showId, int seasonNumber) => $"{BaseUrl}/tv/{showId}/season/{seasonNumber}";

    private static string GetExternalUrl(int showId) => $"{BaseUrl}/tv/{showId}/external_ids";

    private static string GetMovieUrl(int id) => $"{BaseUrl}/movie/{id}";

    private async Task<Configuration?> GetConfigurationAsync()
    {
        _logger.LogTrace("GetConfigurationAsync");
        if (_configuration is not null)
        {
            _logger.LogTrace("Reuse existing configuration");
            return _configuration;
        }

        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, $"{BaseUrl}/configuration");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

            var client = _httpClientFactory.CreateClient();
            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var configuration = JsonSerializer.Deserialize<Configuration>(content);

                if (configuration is not null)
                {
                    _configuration = configuration;
                }

                return _configuration;
            }
            else
            {
                _logger.LogError("GetConfigurationAsync - Http call failed with status code {StatusCode}", response.StatusCode);
                return null;
            }
        }
        catch (TimeoutException)
        {
            _logger.LogError("GetConfigurationAsync - Timeout trying to get configuration.");
            return null;
        }
    }

    public async Task<bool> DownloadImageUrlToStream(string url, Stream stream)
    {
        _logger.LogTrace("DownloadImageUrlToStreamAsync - {url}", url);

        if (string.IsNullOrWhiteSpace(url))
        {
            _logger.LogError("DownloadImageUrlToStreamAsync - Missing url");
            return false;
        }

        try
        {
            var client = _httpClientFactory.CreateClient();
            using var response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                await response.Content.CopyToAsync(stream);
                stream.Seek(0, SeekOrigin.Begin);
                return true;
            }
            else
            {
                _logger.LogError("DownloadImageUrlToStreamAsync - Http call failed with status code {StatusCode}", response.StatusCode);
            }
        }
        catch (TimeoutException)
        {
            _logger.LogError("DownloadImageUrlToStreamAsync - Timeout trying to get configuration.");
        }
        catch (InvalidOperationException e)
        {
            _logger.LogError(e, "DownloadImageUrlToStreamAsync - InvalidOperation.");
        }

        return false;
    }
}