using System.ComponentModel.DataAnnotations;

namespace FilmTV.Data;

public class UserTVShow
{
    [Key]
    public int Id { get; set; }

    [MaxLength(100)]
    public string UserId { get; set; } = null!;
    
    public int TVShowId { get; set; }
    
    public TVShow TVShow { get; set; } = null!;    
    
    public int? Rating { get; set; }

    public DateTime? RatingDate { get; set; }
    
    [MaxLength(512)]
    public string Title { get; set; } = string.Empty;    
}