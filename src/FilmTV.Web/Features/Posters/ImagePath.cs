namespace FilmTV.Web.Features.Posters;

public static class ImagePath
{
    public static string Path { get; set; } = string.Empty;

    public static string MovieFilename(int id) => $"{Path}/movie/{id}.jpg";
    
    public static string ShowFilename(int id) => $"{Path}/tv/{id}.jpg";
    
    public static string MovieUrl(int id) => $"/images/movie/{id}.jpg";
    
    public static string ShowUrl(int id) => $"/images/tv/{id}.jpg";    

    public static void EnsureDirectories(string basePath)
    {
        Path = System.IO.Path.Combine(basePath, "app-data/images");

        Directory.CreateDirectory(System.IO.Path.Combine(Path, "movie"));
        Directory.CreateDirectory(System.IO.Path.Combine(Path, "tv"));
    }
}