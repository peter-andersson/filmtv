namespace FilmTV.Api.Common;

public static class ImagePath
{
    public static string Directory => "app-data/images";
    
    public static string MoviePath(int id) => $"{Directory}/movie/{id}.jpg";
    
    public static string TvShowPath(int id) => $"{Directory}/tv/{id}.jpg";
}