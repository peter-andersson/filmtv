namespace FilmTV.Api.Features.TV;

public class Episode
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;
    
    public int SeasonNumber { get; set; }

    public int EpisodeNumber { get; set; }

    public DateTime? AirDate { get; set; }
    
    public int SeriesId { get; set; }
    public Series Series { get; set; } = null!;
}