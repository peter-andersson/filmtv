using FilmTV.Api.Features.TheMovieDatabase.Models;

namespace FilmTV.Api.Features.TheMovieDatabase;

public interface ITheMovieDatabaseService
{
    Task<string> GetImageUrlAsync(string? path);

    Task<TMDbMovie?> GetMovieAsync(int id, string? etag);

    Task<TMDbShow?> GetShowAsync(int id, string? etag);

    Task<bool> DownloadImageUrlToStreamAsync(string url, Stream stream);
}