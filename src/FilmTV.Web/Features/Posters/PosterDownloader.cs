using FilmTV.Web.Features.TheMovieDatabase;

namespace FilmTV.Web.Features.Posters;

public interface IPosterDownload
{
    Task Download(string? posterPath, string filename);
}

public class PosterDownloader(ILogger<PosterDownloader> logger, ITheMovieDatabaseService tmdbService) : IPosterDownload
{
    private readonly ILogger<PosterDownloader> _logger = logger;
    private readonly ITheMovieDatabaseService _tmdbService = tmdbService;
    
    public async Task Download(string? posterPath, string filename)
    {
        var posterUrl = await _tmdbService.GetImageUrl(posterPath);
        if (string.IsNullOrWhiteSpace(posterUrl))
        {
            return;
        }

        var stream = new MemoryStream();
        await _tmdbService.DownloadImageUrlToStream(posterUrl, stream);

        if (File.Exists(filename))
        {
            File.Delete(filename);
        }

        try
        {
            await using var fileStream = new FileStream(filename, FileMode.Create);
            await stream.CopyToAsync(fileStream);
        }
        catch (Exception e)
        {
            _logger.LogError("Error saving series poster: {Error}", e.Message);
        }
    }  
}