using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FilmTV.Api.Common;

namespace FilmTV.Api.Features.TV;

public class UserSeries
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int SeriesId { get; set; }
    [ForeignKey(nameof(SeriesId))]
    public Series Series { get; set; } = null!;
    
    public string? Title { get; set; }
    
    public int Rating { get; set; }
    
    public DateTime? RatingDate { get; set; }
    
    [MaxLength(50)]
    public string UserId { get; init; } = null!;
    public AppUser User { get; init; } = null!;    
    
    public ICollection<UserEpisode> Episodes { get; set; } = [];
}