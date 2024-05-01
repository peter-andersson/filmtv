using System.ComponentModel.DataAnnotations;

namespace FilmTV.Data;

public class Episode
{
    [Key]
    public int Id { get; set; }
    
    [MaxLength(512)]
    public string Title { get; set; } = string.Empty;
    
    public int EpisodeNumber { get; set; }
    
    public int SeasonId { get; set; }
    
    public int TVShowId { get; set; }

    public TVShow TVShow { get; set; } = null!;
    
    public DateTime? AirDate { get; set; }
}