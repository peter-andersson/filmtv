using System.ComponentModel.DataAnnotations;

namespace FilmTV.Data;

public class UserMovie
{
    [Key]
    public int Id { get; set; }

    [MaxLength(100)]
    public string UserId { get; set; } = null!;
    
    public int MovieId { get; set; }
    
    public Movie Movie { get; set; } = null!;       
    
    [MaxLength(512)]
    public string Title { get; set; } = string.Empty;    

    public DateTime? WatchedDate { get; set; }    

    public int? Rating { get; set; }

    public DateTime? RatingDate { get; set; }
}