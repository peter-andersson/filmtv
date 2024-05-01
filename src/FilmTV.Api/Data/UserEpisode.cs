using System.ComponentModel.DataAnnotations;

namespace FilmTV.Data;

public class UserEpisode
{
    [Key]
    public int Id { get; set; }
    
    [MaxLength(100)]
    public string UserId { get; set; } = null!;    
    
    public int EpisodeId { get; set; }
    
    public Episode Episode { get; set; } = null!;
    
    public DateTime? WatchedDate { get; set; }
}