namespace FilmTV.Api.Features.TV;

public class SeriesResponse
{
    public int Id { get; set; }
    
    public string? Title { get; set; }
    
    public string OriginalTitle { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;
    
    public int Rating { get; set; }
    
    public List<EpisodeResponse> Episodes { get; set; } = [];
    
    public int EpisodeCount
    {
        get
        {
            return Episodes.Count(e => e.AirDate.HasValue);
        }
    }
    
    public int SeasonCount
    {
        get
        {
            return Episodes
                .Where(e => e.SeasonNumber > 0)
                .GroupBy(e => e.EpisodeNumber)
                .Count();
        }
    }
}

public record EpisodeResponse(int SeasonNumber, int EpisodeNumber, string Title, DateTime? AirDate, bool Watched);