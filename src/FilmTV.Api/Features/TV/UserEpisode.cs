using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FilmTV.Api.Common;

namespace FilmTV.Api.Features.TV;

public class UserEpisode
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int EpisodeId { get; set; }
    [ForeignKey(nameof(EpisodeId))]
    public Episode Episode { get; set; } = null!;
    
    public bool Watched { get; set; }
    
    [MaxLength(50)]
    public string UserId { get; init; } = null!;
    public AppUser User { get; init; } = null!;  
    
    public int SeriesId { get; set; }
    [ForeignKey(nameof(SeriesId))]
    public UserSeries UserSeries { get; set; } = null!;    
}